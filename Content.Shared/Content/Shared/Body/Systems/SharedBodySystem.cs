using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DragDrop;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Random.Helpers;
using Content.Shared.Standing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Body.Systems
{
	// Token: 0x02000652 RID: 1618
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedBodySystem : EntitySystem
	{
		// Token: 0x06001380 RID: 4992 RVA: 0x00040AC4 File Offset: 0x0003ECC4
		public void InitializeBody()
		{
			base.SubscribeLocalEvent<BodyComponent, ComponentInit>(new ComponentEventHandler<BodyComponent, ComponentInit>(this.OnBodyInit), null, null);
			base.SubscribeLocalEvent<BodyComponent, ComponentGetState>(new ComponentEventRefHandler<BodyComponent, ComponentGetState>(this.OnBodyGetState), null, null);
			base.SubscribeLocalEvent<BodyComponent, ComponentHandleState>(new ComponentEventRefHandler<BodyComponent, ComponentHandleState>(this.OnBodyHandleState), null, null);
			base.SubscribeLocalEvent<BodyComponent, CanDragEvent>(new ComponentEventRefHandler<BodyComponent, CanDragEvent>(this.OnBodyCanDrag), null, null);
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x00040B21 File Offset: 0x0003ED21
		private void OnBodyCanDrag(EntityUid uid, BodyComponent component, ref CanDragEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x00040B2C File Offset: 0x0003ED2C
		private void OnBodyInit(EntityUid bodyId, BodyComponent body, ComponentInit args)
		{
			if (body.Prototype == null || body.Root != null)
			{
				return;
			}
			BodyPrototype prototype = this.Prototypes.Index<BodyPrototype>(body.Prototype);
			this.InitBody(body, prototype);
			base.Dirty(body, null);
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x00040B72 File Offset: 0x0003ED72
		private void OnBodyGetState(EntityUid uid, BodyComponent body, ref ComponentGetState args)
		{
			args.State = new BodyComponentState(body.Root, body.GibSound);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x00040B8C File Offset: 0x0003ED8C
		private void OnBodyHandleState(EntityUid uid, BodyComponent body, ref ComponentHandleState args)
		{
			BodyComponentState state = args.Current as BodyComponentState;
			if (state == null)
			{
				return;
			}
			body.Root = state.Root;
			body.GibSound = state.GibSound;
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x00040BC4 File Offset: 0x0003EDC4
		[NullableContext(2)]
		public bool TryCreateBodyRootSlot(EntityUid? bodyId, [Nullable(1)] string slotId, [NotNullWhen(true)] out BodyPartSlot slot, BodyComponent body = null)
		{
			slot = null;
			if (bodyId == null || !base.Resolve<BodyComponent>(bodyId.Value, ref body, false) || body.Root != null)
			{
				return false;
			}
			slot = new BodyPartSlot(slotId, bodyId.Value, null);
			body.Root = slot;
			return true;
		}

		// Token: 0x06001386 RID: 4998
		protected abstract void InitBody(BodyComponent body, BodyPrototype prototype);

		// Token: 0x06001387 RID: 4999 RVA: 0x00040C24 File Offset: 0x0003EE24
		protected void InitPart(BodyPartComponent parent, BodyPrototype prototype, string slotId, [Nullable(new byte[]
		{
			2,
			1
		})] HashSet<string> initialized = null)
		{
			if (initialized == null)
			{
				initialized = new HashSet<string>();
			}
			if (initialized.Contains(slotId))
			{
				return;
			}
			initialized.Add(slotId);
			string text;
			HashSet<string> hashSet;
			Dictionary<string, string> dictionary;
			prototype.Slots[slotId].Deconstruct(out text, out hashSet, out dictionary);
			IEnumerable<string> collection = hashSet;
			Dictionary<string, string> organs = dictionary;
			HashSet<string> hashSet2 = new HashSet<string>(collection);
			hashSet2.ExceptWith(initialized);
			EntityCoordinates coordinates = parent.Owner.ToCoordinates();
			List<ValueTuple<BodyPartComponent, string>> subConnections = new List<ValueTuple<BodyPartComponent, string>>();
			this.Containers.EnsureContainer<Container>(parent.Owner, "BodyContainer", null);
			foreach (string connection in hashSet2)
			{
				BodyPrototypeSlot childSlot = prototype.Slots[connection];
				if (childSlot.Part != null)
				{
					EntityUid childPart = base.Spawn(childSlot.Part, coordinates);
					BodyPartComponent childPartComponent = base.Comp<BodyPartComponent>(childPart);
					BodyPartSlot slot = this.CreatePartSlot(connection, parent.Owner, childPartComponent.PartType, parent);
					if (slot == null)
					{
						Logger.Error("Could not create slot for connection " + connection + " in body " + prototype.ID);
					}
					else
					{
						this.AttachPart(new EntityUid?(childPart), slot, childPartComponent);
						subConnections.Add(new ValueTuple<BodyPartComponent, string>(childPartComponent, connection));
					}
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in organs)
			{
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string organSlotId = text;
				string organId = text2;
				EntityUid organ = base.Spawn(organId, coordinates);
				OrganComponent organComponent = base.Comp<OrganComponent>(organ);
				OrganSlot slot2 = this.CreateOrganSlot(organSlotId, parent.Owner, parent);
				if (slot2 == null)
				{
					Logger.Error("Could not create slot for connection " + organSlotId + " in body " + prototype.ID);
				}
				else
				{
					this.InsertOrgan(new EntityUid?(organ), slot2, organComponent);
				}
			}
			foreach (ValueTuple<BodyPartComponent, string> connection2 in subConnections)
			{
				this.InitPart(connection2.Item1, prototype, connection2.Item2, initialized);
			}
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x00040E74 File Offset: 0x0003F074
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"Id",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, BodyPartComponent>> GetBodyChildren(EntityUid? id, BodyComponent body = null)
		{
			if (id != null && base.Resolve<BodyComponent>(id.Value, ref body, false))
			{
				BodyPartSlot root = body.Root;
				BodyPartComponent part;
				if (base.TryComp<BodyPartComponent>((root != null) ? root.Child : null, ref part))
				{
					yield return new ValueTuple<EntityUid, BodyPartComponent>(body.Root.Child.Value, part);
					foreach (ValueTuple<EntityUid, BodyPartComponent> child in this.GetPartChildren(body.Root.Child, null))
					{
						yield return child;
					}
					IEnumerator<ValueTuple<EntityUid, BodyPartComponent>> enumerator = null;
					yield break;
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x00040E92 File Offset: 0x0003F092
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"Id",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, OrganComponent>> GetBodyOrgans(EntityUid? bodyId, BodyComponent body = null)
		{
			if (bodyId == null || !base.Resolve<BodyComponent>(bodyId.Value, ref body, false))
			{
				yield break;
			}
			foreach (ValueTuple<EntityUid, BodyPartComponent> part in this.GetBodyChildren(bodyId, body))
			{
				foreach (ValueTuple<EntityUid, OrganComponent> organ in this.GetPartOrgans(new EntityUid?(part.Item1), part.Item2))
				{
					yield return organ;
				}
				IEnumerator<ValueTuple<EntityUid, OrganComponent>> enumerator2 = null;
			}
			IEnumerator<ValueTuple<EntityUid, BodyPartComponent>> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x00040EB0 File Offset: 0x0003F0B0
		public IEnumerable<BodyPartSlot> GetBodyAllSlots(EntityUid? bodyId, [Nullable(2)] BodyComponent body = null)
		{
			if (bodyId == null || !base.Resolve<BodyComponent>(bodyId.Value, ref body, false))
			{
				yield break;
			}
			BodyPartSlot root = body.Root;
			foreach (BodyPartSlot slot in this.GetPartAllSlots((root != null) ? root.Child : null, null))
			{
				yield return slot;
			}
			IEnumerator<BodyPartSlot> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x00040ECE File Offset: 0x0003F0CE
		public IEnumerable<BodyPartSlot> GetAllBodyPartSlots(EntityUid partId, [Nullable(2)] BodyPartComponent part = null)
		{
			if (!base.Resolve<BodyPartComponent>(partId, ref part, false))
			{
				yield break;
			}
			foreach (BodyPartSlot slot in part.Children.Values)
			{
				BodyPartComponent childPart;
				if (base.TryComp<BodyPartComponent>(slot.Child, ref childPart))
				{
					yield return slot;
					foreach (BodyPartSlot child in this.GetAllBodyPartSlots(slot.Child.Value, childPart))
					{
						yield return child;
					}
					IEnumerator<BodyPartSlot> enumerator2 = null;
					childPart = null;
					slot = null;
				}
			}
			Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator enumerator = default(Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x00040EEC File Offset: 0x0003F0EC
		public virtual HashSet<EntityUid> GibBody(EntityUid? partId, bool gibOrgans = false, [Nullable(2)] BodyComponent body = null, bool deleteItems = false)
		{
			if (partId == null || !base.Resolve<BodyComponent>(partId.Value, ref body, false))
			{
				return new HashSet<EntityUid>();
			}
			ValueTuple<EntityUid, BodyPartComponent>[] array = this.GetBodyChildren(partId, body).ToArray<ValueTuple<EntityUid, BodyPartComponent>>();
			HashSet<EntityUid> gibs = new HashSet<EntityUid>(array.Length);
			foreach (ValueTuple<EntityUid, BodyPartComponent> part in array)
			{
				this.DropPart(new EntityUid?(part.Item1), part.Item2);
				gibs.Add(part.Item1);
				if (gibOrgans)
				{
					foreach (ValueTuple<EntityUid, OrganComponent> organ in this.GetPartOrgans(new EntityUid?(part.Item1), part.Item2))
					{
						this.DropOrgan(new EntityUid?(organ.Item1), organ.Item2);
						gibs.Add(organ.Item1);
					}
				}
			}
			return gibs;
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x00040FF0 File Offset: 0x0003F1F0
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeBody();
			this.InitializeParts();
			this.InitializeOrgans();
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x0004100A File Offset: 0x0003F20A
		private void InitializeOrgans()
		{
			base.SubscribeLocalEvent<OrganComponent, ComponentGetState>(new ComponentEventRefHandler<OrganComponent, ComponentGetState>(this.OnOrganGetState), null, null);
			base.SubscribeLocalEvent<OrganComponent, ComponentHandleState>(new ComponentEventRefHandler<OrganComponent, ComponentHandleState>(this.OnOrganHandleState), null, null);
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x00041034 File Offset: 0x0003F234
		[NullableContext(2)]
		private OrganSlot CreateOrganSlot([Nullable(1)] string slotId, EntityUid parent, BodyPartComponent part = null)
		{
			if (!base.Resolve<BodyPartComponent>(parent, ref part, false))
			{
				return null;
			}
			OrganSlot slot = new OrganSlot(slotId, parent);
			part.Organs.Add(slotId, slot);
			return slot;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x00041068 File Offset: 0x0003F268
		private bool CanInsertOrgan(EntityUid? organId, OrganSlot slot, [Nullable(2)] OrganComponent organ = null)
		{
			IContainer container;
			return organId != null && slot.Child == null && base.Resolve<OrganComponent>(organId.Value, ref organ, false) && this.Containers.TryGetContainer(slot.Parent, "BodyContainer", ref container, null) && container.CanInsert(organId.Value, null);
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x000410CB File Offset: 0x0003F2CB
		private void OnOrganGetState(EntityUid uid, OrganComponent organ, ref ComponentGetState args)
		{
			args.State = new OrganComponentState(organ.Body, organ.ParentSlot);
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x000410E4 File Offset: 0x0003F2E4
		private void OnOrganHandleState(EntityUid uid, OrganComponent organ, ref ComponentHandleState args)
		{
			OrganComponentState state = args.Current as OrganComponentState;
			if (state == null)
			{
				return;
			}
			organ.Body = state.Body;
			organ.ParentSlot = state.Parent;
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0004111C File Offset: 0x0003F31C
		public bool InsertOrgan(EntityUid? organId, OrganSlot slot, [Nullable(2)] OrganComponent organ = null)
		{
			if (organId == null || !base.Resolve<OrganComponent>(organId.Value, ref organ, false) || !this.CanInsertOrgan(organId, slot, organ))
			{
				return false;
			}
			this.DropOrgan(slot.Child, null);
			this.DropOrgan(organId, organ);
			if (!this.Containers.EnsureContainer<Container>(slot.Parent, "BodyContainer", null).Insert(organId.Value, null, null, null, null, null))
			{
				return false;
			}
			slot.Child = organId;
			organ.ParentSlot = slot;
			OrganComponent organComponent = organ;
			BodyPartComponent bodyPartComponent = base.CompOrNull<BodyPartComponent>(slot.Parent);
			organComponent.Body = ((bodyPartComponent != null) ? bodyPartComponent.Body : null);
			base.Dirty(slot.Parent, null);
			base.Dirty(organId.Value, null);
			if (organ.Body == null)
			{
				base.RaiseLocalEvent<AddedToPartEvent>(organId.Value, new AddedToPartEvent(slot.Parent), false);
			}
			else
			{
				base.RaiseLocalEvent<AddedToPartInBodyEvent>(organId.Value, new AddedToPartInBodyEvent(organ.Body.Value, slot.Parent), false);
			}
			return true;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x00041230 File Offset: 0x0003F430
		[NullableContext(2)]
		public bool AddOrganToFirstValidSlot(EntityUid? childId, EntityUid? parentId, OrganComponent child = null, BodyPartComponent parent = null)
		{
			if (childId == null || !base.Resolve<OrganComponent>(childId.Value, ref child, false) || parentId == null || !base.Resolve<BodyPartComponent>(parentId.Value, ref parent, false))
			{
				return false;
			}
			foreach (OrganSlot slot in parent.Organs.Values)
			{
				if (slot.Child != null)
				{
					this.InsertOrgan(childId, slot, child);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x000412DC File Offset: 0x0003F4DC
		[NullableContext(2)]
		public bool DropOrgan(EntityUid? organId, OrganComponent organ = null)
		{
			if (organId != null && base.Resolve<OrganComponent>(organId.Value, ref organ, false))
			{
				OrganSlot slot = organ.ParentSlot;
				if (slot != null)
				{
					BodyPartComponent oldParent = base.CompOrNull<BodyPartComponent>(organ.ParentSlot.Parent);
					slot.Child = null;
					organ.ParentSlot = null;
					organ.Body = null;
					IContainer container;
					if (this.Containers.TryGetContainer(slot.Parent, "BodyContainer", ref container, null))
					{
						container.Remove(organId.Value, null, null, null, true, false, null, null);
					}
					TransformComponent transform;
					if (base.TryComp<TransformComponent>(organId, ref transform))
					{
						transform.AttachToGridOrMap();
					}
					organ.Owner.RandomOffset(0.25f);
					if (oldParent == null)
					{
						return true;
					}
					if (oldParent.Body != null)
					{
						base.RaiseLocalEvent<RemovedFromPartInBodyEvent>(organId.Value, new RemovedFromPartInBodyEvent(oldParent.Body.Value, oldParent.Owner), false);
					}
					else
					{
						base.RaiseLocalEvent<RemovedFromPartEvent>(organId.Value, new RemovedFromPartEvent(oldParent.Owner), false);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x000413FC File Offset: 0x0003F5FC
		[NullableContext(2)]
		public bool DropOrganAt(EntityUid? organId, EntityCoordinates dropAt, OrganComponent organ = null)
		{
			if (organId == null || !this.DropOrgan(organId, organ))
			{
				return false;
			}
			TransformComponent transform;
			if (base.TryComp<TransformComponent>(organId.Value, ref transform))
			{
				transform.Coordinates = dropAt;
			}
			return true;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x00041438 File Offset: 0x0003F638
		[NullableContext(2)]
		public bool DeleteOrgan(EntityUid? id, OrganComponent part = null)
		{
			if (id == null || !base.Resolve<OrganComponent>(id.Value, ref part, false))
			{
				return false;
			}
			this.DropOrgan(id, part);
			if (base.Deleted(id.Value, null))
			{
				return false;
			}
			base.Del(id.Value);
			return true;
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0004148C File Offset: 0x0003F68C
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Comp",
			"Organ"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public List<ValueTuple<T, OrganComponent>> GetBodyOrganComponents<T>(EntityUid uid, [Nullable(2)] BodyComponent body = null) where T : Component
		{
			if (!base.Resolve<BodyComponent>(uid, ref body, true))
			{
				return new List<ValueTuple<T, OrganComponent>>();
			}
			EntityQuery<T> query = base.GetEntityQuery<T>();
			List<ValueTuple<T, OrganComponent>> list = new List<ValueTuple<T, OrganComponent>>(3);
			foreach (ValueTuple<EntityUid, OrganComponent> organ in this.GetBodyOrgans(new EntityUid?(uid), body))
			{
				T comp;
				if (query.TryGetComponent(organ.Item1, ref comp))
				{
					list.Add(new ValueTuple<T, OrganComponent>(comp, organ.Item2));
				}
			}
			return list;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00041520 File Offset: 0x0003F720
		[NullableContext(0)]
		public bool TryGetBodyOrganComponents<T>(EntityUid uid, [TupleElementNames(new string[]
		{
			"Comp",
			"Organ"
		})] [Nullable(new byte[]
		{
			2,
			0,
			1,
			1
		})] [NotNullWhen(true)] out List<ValueTuple<T, OrganComponent>> comps, [Nullable(2)] BodyComponent body = null) where T : Component
		{
			if (!base.Resolve<BodyComponent>(uid, ref body, true))
			{
				comps = null;
				return false;
			}
			comps = this.GetBodyOrganComponents<T>(uid, body);
			if (comps.Count != 0)
			{
				return true;
			}
			comps = null;
			return false;
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x0004154C File Offset: 0x0003F74C
		private void InitializeParts()
		{
			base.SubscribeLocalEvent<BodyPartComponent, ComponentRemove>(new ComponentEventHandler<BodyPartComponent, ComponentRemove>(this.OnPartRemoved), null, null);
			base.SubscribeLocalEvent<BodyPartComponent, ComponentGetState>(new ComponentEventRefHandler<BodyPartComponent, ComponentGetState>(this.OnPartGetState), null, null);
			base.SubscribeLocalEvent<BodyPartComponent, ComponentHandleState>(new ComponentEventRefHandler<BodyPartComponent, ComponentHandleState>(this.OnPartHandleState), null, null);
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x0004158A File Offset: 0x0003F78A
		private void OnPartGetState(EntityUid uid, BodyPartComponent part, ref ComponentGetState args)
		{
			args.State = new BodyPartComponentState(part.Body, part.ParentSlot, part.Children, part.Organs, part.PartType, part.IsVital, part.Symmetry);
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x000415C4 File Offset: 0x0003F7C4
		private void OnPartHandleState(EntityUid uid, BodyPartComponent part, ref ComponentHandleState args)
		{
			BodyPartComponentState state = args.Current as BodyPartComponentState;
			if (state == null)
			{
				return;
			}
			part.Body = state.Body;
			part.ParentSlot = state.ParentSlot;
			part.Children = state.Children;
			part.Organs = state.Organs;
			part.PartType = state.PartType;
			part.IsVital = state.IsVital;
			part.Symmetry = state.Symmetry;
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x00041638 File Offset: 0x0003F838
		private void OnPartRemoved(EntityUid uid, BodyPartComponent part, ComponentRemove args)
		{
			BodyPartSlot slot = part.ParentSlot;
			if (slot != null)
			{
				slot.Child = null;
				base.Dirty(slot.Parent, null);
			}
			foreach (BodyPartSlot childSlot in part.Children.Values.ToArray<BodyPartSlot>())
			{
				this.DropPart(childSlot.Child, null);
			}
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x000416A0 File Offset: 0x0003F8A0
		[NullableContext(2)]
		private BodyPartSlot CreatePartSlot([Nullable(1)] string slotId, EntityUid parent, BodyPartType partType, BodyPartComponent part = null)
		{
			if (!base.Resolve<BodyPartComponent>(parent, ref part, false))
			{
				return null;
			}
			BodyPartSlot slot = new BodyPartSlot(slotId, parent, new BodyPartType?(partType));
			part.Children.Add(slotId, slot);
			return slot;
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x000416D8 File Offset: 0x0003F8D8
		[NullableContext(2)]
		public bool TryCreatePartSlot(EntityUid? parentId, [Nullable(1)] string id, [NotNullWhen(true)] out BodyPartSlot slot, BodyPartComponent parent = null)
		{
			slot = null;
			if (parentId == null || !base.Resolve<BodyPartComponent>(parentId.Value, ref parent, false))
			{
				return false;
			}
			slot = new BodyPartSlot(id, parentId.Value, null);
			if (!parent.Children.TryAdd(id, slot))
			{
				slot = null;
				return false;
			}
			return true;
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x00041734 File Offset: 0x0003F934
		[NullableContext(2)]
		public bool TryCreatePartSlotAndAttach(EntityUid? parentId, [Nullable(1)] string id, EntityUid? childId, BodyPartComponent parent = null, BodyPartComponent child = null)
		{
			BodyPartSlot slot;
			return this.TryCreatePartSlot(parentId, id, out slot, parent) && this.AttachPart(childId, slot, child);
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0004175B File Offset: 0x0003F95B
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"Id",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, BodyPartComponent>> GetPartChildren(EntityUid? id, BodyPartComponent part = null)
		{
			if (id == null || !base.Resolve<BodyPartComponent>(id.Value, ref part, false))
			{
				yield break;
			}
			foreach (BodyPartSlot slot in part.Children.Values)
			{
				BodyPartComponent childPart;
				if (base.TryComp<BodyPartComponent>(slot.Child, ref childPart))
				{
					yield return new ValueTuple<EntityUid, BodyPartComponent>(slot.Child.Value, childPart);
					foreach (ValueTuple<EntityUid, BodyPartComponent> subChild in this.GetPartChildren(slot.Child, childPart))
					{
						yield return subChild;
					}
					IEnumerator<ValueTuple<EntityUid, BodyPartComponent>> enumerator2 = null;
					childPart = null;
					slot = null;
				}
			}
			Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator enumerator = default(Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x00041779 File Offset: 0x0003F979
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"Id",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, OrganComponent>> GetPartOrgans(EntityUid? partId, BodyPartComponent part = null)
		{
			if (partId == null || !base.Resolve<BodyPartComponent>(partId.Value, ref part, false))
			{
				yield break;
			}
			foreach (OrganSlot slot in part.Organs.Values)
			{
				OrganComponent organ;
				if (base.TryComp<OrganComponent>(slot.Child, ref organ))
				{
					yield return new ValueTuple<EntityUid, OrganComponent>(slot.Child.Value, organ);
				}
			}
			Dictionary<string, OrganSlot>.ValueCollection.Enumerator enumerator = default(Dictionary<string, OrganSlot>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060013A3 RID: 5027 RVA: 0x00041797 File Offset: 0x0003F997
		public IEnumerable<BodyPartSlot> GetPartAllSlots(EntityUid? partId, [Nullable(2)] BodyPartComponent part = null)
		{
			if (partId == null || !base.Resolve<BodyPartComponent>(partId.Value, ref part, false))
			{
				yield break;
			}
			foreach (BodyPartSlot slot in part.Children.Values)
			{
				yield return slot;
				BodyComponent childPart;
				if (base.TryComp<BodyComponent>(slot.Child, ref childPart))
				{
					foreach (BodyPartSlot subChild in this.GetBodyAllSlots(slot.Child, childPart))
					{
						yield return subChild;
					}
					IEnumerator<BodyPartSlot> enumerator2 = null;
					slot = null;
				}
			}
			Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator enumerator = default(Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x000417B8 File Offset: 0x0003F9B8
		public bool CanAttachPart([NotNullWhen(true)] EntityUid? partId, BodyPartSlot slot, [Nullable(2)] BodyPartComponent part = null)
		{
			if (partId != null && slot.Child == null && base.Resolve<BodyPartComponent>(partId.Value, ref part, false))
			{
				if (slot.Type != null)
				{
					BodyPartType? type = slot.Type;
					BodyPartType partType = part.PartType;
					if (!(type.GetValueOrDefault() == partType & type != null))
					{
						return false;
					}
				}
				IContainer container;
				if (this.Containers.TryGetContainer(slot.Parent, "BodyContainer", ref container, null))
				{
					return container.CanInsert(partId.Value, null);
				}
			}
			return false;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00041850 File Offset: 0x0003FA50
		public virtual bool AttachPart(EntityUid? partId, BodyPartSlot slot, [Nullable(2)] [NotNullWhen(true)] BodyPartComponent part = null)
		{
			if (partId == null || !base.Resolve<BodyPartComponent>(partId.Value, ref part, false) || !this.CanAttachPart(partId, slot, part))
			{
				return false;
			}
			this.DropPart(slot.Child, null);
			this.DropPart(partId, part);
			if (!this.Containers.EnsureContainer<Container>(slot.Parent, "BodyContainer", null).Insert(partId.Value, null, null, null, null, null))
			{
				return false;
			}
			slot.Child = partId;
			part.ParentSlot = slot;
			BodyPartComponent parentPart;
			BodyComponent parentBody;
			if (base.TryComp<BodyPartComponent>(slot.Parent, ref parentPart))
			{
				part.Body = parentPart.Body;
			}
			else if (base.TryComp<BodyComponent>(slot.Parent, ref parentBody))
			{
				part.Body = new EntityUid?(parentBody.Owner);
			}
			else
			{
				part.Body = null;
			}
			base.Dirty(slot.Parent, null);
			base.Dirty(partId.Value, null);
			EntityUid? body = part.Body;
			if (body != null)
			{
				EntityUid newBody = body.GetValueOrDefault();
				if (part.PartType == BodyPartType.Leg)
				{
					this.UpdateMovementSpeed(newBody, null, null);
				}
				BodyPartAddedEvent partAddedEvent = new BodyPartAddedEvent(slot.Id, part);
				base.RaiseLocalEvent<BodyPartAddedEvent>(newBody, ref partAddedEvent, false);
				foreach (ValueTuple<EntityUid, OrganComponent> organ in this.GetPartOrgans(partId, part))
				{
					base.RaiseLocalEvent<AddedToBodyEvent>(organ.Item1, new AddedToBodyEvent(newBody), true);
				}
				base.Dirty(newBody, null);
			}
			return true;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x000419E0 File Offset: 0x0003FBE0
		[NullableContext(2)]
		public virtual bool DropPart(EntityUid? partId, [NotNullWhen(true)] BodyPartComponent part = null)
		{
			if (partId != null && base.Resolve<BodyPartComponent>(partId.Value, ref part, false))
			{
				BodyPartSlot slot = part.ParentSlot;
				if (slot != null)
				{
					EntityUid? oldBodyNullable = part.Body;
					slot.Child = null;
					part.ParentSlot = null;
					part.Body = null;
					IContainer container;
					if (this.Containers.TryGetContainer(slot.Parent, "BodyContainer", ref container, null))
					{
						container.Remove(partId.Value, null, null, null, true, false, null, null);
					}
					TransformComponent transform;
					if (base.TryComp<TransformComponent>(partId, ref transform))
					{
						transform.AttachToGridOrMap();
					}
					part.Owner.RandomOffset(0.25f);
					if (oldBodyNullable != null)
					{
						EntityUid oldBody = oldBodyNullable.GetValueOrDefault();
						BodyPartRemovedEvent args = new BodyPartRemovedEvent(slot.Id, part);
						base.RaiseLocalEvent<BodyPartRemovedEvent>(oldBody, ref args, false);
						if (part.PartType == BodyPartType.Leg)
						{
							this.UpdateMovementSpeed(oldBody, null, null);
							if (!this.GetBodyChildrenOfType(new EntityUid?(oldBody), BodyPartType.Leg, null).Any<ValueTuple<EntityUid, BodyPartComponent>>())
							{
								this.Standing.Down(oldBody, true, true, null, null, null);
							}
						}
						if (part.IsVital && !this.GetBodyChildrenOfType(new EntityUid?(oldBody), part.PartType, null).Any<ValueTuple<EntityUid, BodyPartComponent>>())
						{
							DamageSpecifier damage = new DamageSpecifier(this.Prototypes.Index<DamageTypePrototype>("Bloodloss"), 300);
							this.Damageable.TryChangeDamage(new EntityUid?(part.Owner), damage, false, true, null, null);
						}
						foreach (OrganSlot organSlot in part.Organs.Values)
						{
							EntityUid? child2 = organSlot.Child;
							if (child2 != null)
							{
								EntityUid child = child2.GetValueOrDefault();
								base.RaiseLocalEvent<RemovedFromBodyEvent>(child, new RemovedFromBodyEvent(oldBody), true);
							}
						}
					}
					base.Dirty(slot.Parent, null);
					base.Dirty(partId.Value, null);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x00041C00 File Offset: 0x0003FE00
		[NullableContext(2)]
		public void UpdateMovementSpeed(EntityUid body, BodyComponent component = null, MovementSpeedModifierComponent movement = null)
		{
			if (!base.Resolve<BodyComponent, MovementSpeedModifierComponent>(body, ref component, ref movement, false))
			{
				return;
			}
			if (component.RequiredLegs <= 0)
			{
				return;
			}
			BodyPartSlot root2 = component.Root;
			EntityUid? entityUid = (root2 != null) ? root2.Child : null;
			if (entityUid != null)
			{
				EntityUid root = entityUid.GetValueOrDefault();
				HashSet<BodyPartSlot> hashSet = this.GetAllBodyPartSlots(root, null).ToHashSet<BodyPartSlot>();
				HashSet<EntityUid> allLegs = new HashSet<EntityUid>();
				foreach (BodyPartSlot slot in hashSet)
				{
					BodyPartType? type = slot.Type;
					BodyPartType bodyPartType = BodyPartType.Leg;
					if (type.GetValueOrDefault() == bodyPartType & type != null)
					{
						entityUid = slot.Child;
						if (entityUid != null)
						{
							EntityUid child = entityUid.GetValueOrDefault();
							allLegs.Add(child);
						}
					}
				}
				float walkSpeed = 0f;
				float sprintSpeed = 0f;
				float acceleration = 0f;
				foreach (EntityUid leg in allLegs)
				{
					MovementSpeedModifierComponent legModifier;
					if (base.TryComp<MovementSpeedModifierComponent>(leg, ref legModifier))
					{
						walkSpeed += legModifier.BaseWalkSpeed;
						sprintSpeed += legModifier.BaseSprintSpeed;
						acceleration += legModifier.Acceleration;
					}
				}
				walkSpeed /= (float)component.RequiredLegs;
				sprintSpeed /= (float)component.RequiredLegs;
				acceleration /= (float)component.RequiredLegs;
				this.Movement.ChangeBaseSpeed(body, walkSpeed, sprintSpeed, acceleration, movement);
				return;
			}
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x00041D94 File Offset: 0x0003FF94
		[NullableContext(2)]
		public bool DropPartAt(EntityUid? partId, EntityCoordinates dropAt, BodyPartComponent part = null)
		{
			if (partId == null || !this.DropPart(partId, part))
			{
				return false;
			}
			TransformComponent transform;
			if (base.TryComp<TransformComponent>(partId.Value, ref transform))
			{
				transform.Coordinates = dropAt;
			}
			return true;
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00041DD0 File Offset: 0x0003FFD0
		[NullableContext(2)]
		public bool OrphanPart(EntityUid? partId, BodyPartComponent part = null)
		{
			if (partId == null || !base.Resolve<BodyPartComponent>(partId.Value, ref part, false))
			{
				return false;
			}
			this.DropPart(partId, part);
			foreach (BodyPartSlot slot in part.Children.Values)
			{
				this.DropPart(slot.Child, null);
			}
			return false;
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x00041E58 File Offset: 0x00040058
		[NullableContext(2)]
		public bool DeletePart(EntityUid? id, BodyPartComponent part = null)
		{
			if (id == null || !base.Resolve<BodyPartComponent>(id.Value, ref part, false))
			{
				return false;
			}
			this.DropPart(id, part);
			if (base.Deleted(id.Value, null))
			{
				return false;
			}
			base.Del(id.Value);
			return true;
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x00041EAB File Offset: 0x000400AB
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"Id",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, BodyPartComponent>> GetBodyChildrenOfType(EntityUid? bodyId, BodyPartType type, BodyComponent body = null)
		{
			foreach (ValueTuple<EntityUid, BodyPartComponent> part in this.GetBodyChildren(bodyId, body))
			{
				if (part.Item2.PartType == type)
				{
					yield return part;
				}
			}
			IEnumerator<ValueTuple<EntityUid, BodyPartComponent>> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x00041ED0 File Offset: 0x000400D0
		[NullableContext(2)]
		public bool BodyHasChildOfType(EntityUid? bodyId, BodyPartType type, BodyComponent body = null)
		{
			return this.GetBodyChildrenOfType(bodyId, type, body).Any<ValueTuple<EntityUid, BodyPartComponent>>();
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x00041EE0 File Offset: 0x000400E0
		[NullableContext(2)]
		public bool BodyHasChild(EntityUid? parentId, EntityUid? childId, BodyComponent parent = null, BodyPartComponent child = null)
		{
			if (parentId == null || !base.Resolve<BodyComponent>(parentId.Value, ref parent, false) || childId == null || !base.Resolve<BodyPartComponent>(childId.Value, ref child, false))
			{
				return false;
			}
			BodyPartSlot parentSlot = child.ParentSlot;
			if (parentSlot == null)
			{
				return parentId == null;
			}
			return parentSlot.Child == parentId;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x00041F73 File Offset: 0x00040173
		public IEnumerable<EntityUid> GetBodyPartAdjacentParts(EntityUid partId, [Nullable(2)] BodyPartComponent part = null)
		{
			if (!base.Resolve<BodyPartComponent>(partId, ref part, false))
			{
				yield break;
			}
			if (part.ParentSlot != null)
			{
				yield return part.ParentSlot.Parent;
			}
			foreach (BodyPartSlot slot in part.Children.Values)
			{
				if (slot.Child != null)
				{
					yield return slot.Child.Value;
				}
			}
			Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator enumerator = default(Dictionary<string, BodyPartSlot>.ValueCollection.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00041F91 File Offset: 0x00040191
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"AdjacentId",
			"Component"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IEnumerable<ValueTuple<EntityUid, T>> GetBodyPartAdjacentPartsComponents<T>(EntityUid partId, [Nullable(2)] BodyPartComponent part = null) where T : Component
		{
			if (!base.Resolve<BodyPartComponent>(partId, ref part, false))
			{
				yield break;
			}
			EntityQuery<T> query = base.GetEntityQuery<T>();
			foreach (EntityUid adjacentId in this.GetBodyPartAdjacentParts(partId, part))
			{
				T component;
				if (query.TryGetComponent(adjacentId, ref component))
				{
					yield return new ValueTuple<EntityUid, T>(adjacentId, component);
				}
			}
			IEnumerator<EntityUid> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00041FB0 File Offset: 0x000401B0
		[NullableContext(0)]
		public bool TryGetBodyPartAdjacentPartsComponents<T>(EntityUid partId, [TupleElementNames(new string[]
		{
			"AdjacentId",
			"Component"
		})] [Nullable(new byte[]
		{
			2,
			0,
			1
		})] [NotNullWhen(true)] out List<ValueTuple<EntityUid, T>> comps, [Nullable(2)] BodyPartComponent part = null) where T : Component
		{
			if (!base.Resolve<BodyPartComponent>(partId, ref part, false))
			{
				comps = null;
				return false;
			}
			EntityQuery<T> query = base.GetEntityQuery<T>();
			comps = new List<ValueTuple<EntityUid, T>>();
			foreach (EntityUid adjacentId in this.GetBodyPartAdjacentParts(partId, part))
			{
				T component;
				if (query.TryGetComponent(adjacentId, ref component))
				{
					comps.Add(new ValueTuple<EntityUid, T>(adjacentId, component));
				}
			}
			if (comps.Count != 0)
			{
				return true;
			}
			comps = null;
			return false;
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x00042040 File Offset: 0x00040240
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Comp",
			"Organ"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public List<ValueTuple<T, OrganComponent>> GetBodyPartOrganComponents<T>(EntityUid uid, [Nullable(2)] BodyPartComponent part = null) where T : Component
		{
			if (!base.Resolve<BodyPartComponent>(uid, ref part, true))
			{
				return new List<ValueTuple<T, OrganComponent>>();
			}
			EntityQuery<T> query = base.GetEntityQuery<T>();
			List<ValueTuple<T, OrganComponent>> list = new List<ValueTuple<T, OrganComponent>>();
			foreach (ValueTuple<EntityUid, OrganComponent> organ in this.GetPartOrgans(new EntityUid?(uid), part))
			{
				T comp;
				if (query.TryGetComponent(organ.Item1, ref comp))
				{
					list.Add(new ValueTuple<T, OrganComponent>(comp, organ.Item2));
				}
			}
			return list;
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x000420D4 File Offset: 0x000402D4
		[NullableContext(0)]
		public bool TryGetBodyPartOrganComponents<T>(EntityUid uid, [TupleElementNames(new string[]
		{
			"Comp",
			"Organ"
		})] [Nullable(new byte[]
		{
			2,
			0,
			1,
			1
		})] [NotNullWhen(true)] out List<ValueTuple<T, OrganComponent>> comps, [Nullable(2)] BodyPartComponent part = null) where T : Component
		{
			if (!base.Resolve<BodyPartComponent>(uid, ref part, true))
			{
				comps = null;
				return false;
			}
			comps = this.GetBodyPartOrganComponents<T>(uid, part);
			if (comps.Count != 0)
			{
				return true;
			}
			comps = null;
			return false;
		}

		// Token: 0x0400137F RID: 4991
		protected const string BodyContainerId = "BodyContainer";

		// Token: 0x04001380 RID: 4992
		[Dependency]
		protected readonly IPrototypeManager Prototypes;

		// Token: 0x04001381 RID: 4993
		[Dependency]
		protected readonly SharedContainerSystem Containers;

		// Token: 0x04001382 RID: 4994
		[Dependency]
		protected readonly DamageableSystem Damageable;

		// Token: 0x04001383 RID: 4995
		[Dependency]
		protected readonly StandingStateSystem Standing;

		// Token: 0x04001384 RID: 4996
		[Dependency]
		protected readonly MovementSpeedModifierSystem Movement;
	}
}
