using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Actions;
using Content.Server.Buckle.Systems;
using Content.Server.Humanoid;
using Content.Server.Inventory;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Content.Server.Polymorph.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage;
using Content.Shared.GameTicking;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Systems;
using Content.Shared.Polymorph;
using Robust.Server.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Polymorph.Systems
{
	// Token: 0x020002C3 RID: 707
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PolymorphableSystem : EntitySystem
	{
		// Token: 0x06000E4A RID: 3658 RVA: 0x00048274 File Offset: 0x00046474
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			PolymorphQueuedData data;
			while (this._queuedPolymorphUpdates.TryDequeue(out data))
			{
				if (!base.Deleted(data.Ent, null))
				{
					EntityUid? ent = this.PolymorphEntity(data.Ent, data.Polymorph);
					if (ent != null)
					{
						SoundSystem.Play(data.Sound.GetSound(null, null), Filter.Pvs(ent.Value, 2f, this.EntityManager, null, null), ent.Value, new AudioParams?(data.Sound.Params));
					}
				}
			}
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x00048307 File Offset: 0x00046507
		private void InitializeCollide()
		{
			base.SubscribeLocalEvent<PolymorphOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<PolymorphOnCollideComponent, StartCollideEvent>(this.OnPolymorphCollide), null, null);
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x00048320 File Offset: 0x00046520
		private void OnPolymorphCollide(EntityUid uid, PolymorphOnCollideComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID != "projectile")
			{
				return;
			}
			EntityUid other = args.OtherFixture.Body.Owner;
			if (!component.Whitelist.IsValid(other, null) || (component.Blacklist != null && component.Blacklist.IsValid(other, null)))
			{
				return;
			}
			this._queuedPolymorphUpdates.Enqueue(new PolymorphQueuedData(other, component.Sound, component.Polymorph));
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x0004839A File Offset: 0x0004659A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PolymorphableComponent, ComponentStartup>(new ComponentEventHandler<PolymorphableComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<PolymorphableComponent, PolymorphActionEvent>(new ComponentEventHandler<PolymorphableComponent, PolymorphActionEvent>(this.OnPolymorphActionEvent), null, null);
			this.InitializeCollide();
			this.InitializeMap();
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x000483D8 File Offset: 0x000465D8
		private void OnStartup(EntityUid uid, PolymorphableComponent component, ComponentStartup args)
		{
			if (component.InnatePolymorphs != null)
			{
				foreach (string morph in component.InnatePolymorphs)
				{
					this.CreatePolymorphAction(morph, uid);
				}
			}
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x00048434 File Offset: 0x00046634
		private void OnPolymorphActionEvent(EntityUid uid, PolymorphableComponent component, PolymorphActionEvent args)
		{
			this.PolymorphEntity(uid, args.Prototype);
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00048444 File Offset: 0x00046644
		public EntityUid? PolymorphEntity(EntityUid target, string id)
		{
			PolymorphPrototype proto;
			if (!this._proto.TryIndex<PolymorphPrototype>(id, ref proto))
			{
				this._saw.Error("Invalid polymorph prototype");
				return null;
			}
			return this.PolymorphEntity(target, proto);
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00048484 File Offset: 0x00046684
		public EntityUid? PolymorphEntity(EntityUid target, PolymorphPrototype proto)
		{
			if (!proto.AllowRepeatedMorphs && base.HasComp<PolymorphedEntityComponent>(target))
			{
				return null;
			}
			this._buckle.TryUnbuckle(target, target, true, null);
			TransformComponent targetTransformComp = base.Transform(target);
			EntityUid child = base.Spawn(proto.Entity, targetTransformComp.Coordinates);
			MakeSentientCommand.MakeSentient(child, this.EntityManager, true, true);
			PolymorphedEntityComponent comp = this._compFact.GetComponent<PolymorphedEntityComponent>();
			comp.Owner = child;
			comp.Parent = target;
			comp.Prototype = proto.ID;
			this.EntityManager.AddComponent<PolymorphedEntityComponent>(child, comp, false);
			base.Transform(child).LocalRotation = targetTransformComp.LocalRotation;
			IContainer cont;
			if (this._container.TryGetContainingContainer(target, ref cont, null, null))
			{
				cont.Insert(child, null, null, null, null, null);
			}
			DamageableComponent damageParent;
			DamageSpecifier damage;
			if (proto.TransferDamage && base.TryComp<DamageableComponent>(child, ref damageParent) && this._mobThresholdSystem.GetScaledDamage(target, child, out damage) && damage != null)
			{
				this._damageable.SetDamage(damageParent, damage);
			}
			if (proto.Inventory == PolymorphInventoryChange.Transfer)
			{
				this._inventory.TransferEntityInventories(target, child);
				using (IEnumerator<EntityUid> enumerator2 = this._sharedHands.EnumerateHeld(target, null).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						EntityUid hand = enumerator2.Current;
						ContainerHelpers.TryRemoveFromContainer(hand, false, null);
						this._sharedHands.TryPickupAnyHand(child, hand, true, false, null, null);
					}
					goto IL_1D4;
				}
			}
			if (proto.Inventory == PolymorphInventoryChange.Drop)
			{
				InventorySystem.ContainerSlotEnumerator enumerator;
				if (this._inventory.TryGetContainerSlotEnumerator(target, out enumerator, null))
				{
					ContainerSlot slot;
					while (enumerator.MoveNext(out slot))
					{
						ContainerHelpers.EmptyContainer(slot, false, null, false, null);
					}
				}
				foreach (EntityUid entityUid in this._sharedHands.EnumerateHeld(target, null))
				{
					ContainerHelpers.TryRemoveFromContainer(entityUid, false, null);
				}
			}
			IL_1D4:
			MetaDataComponent targetMeta;
			MetaDataComponent childMeta;
			if (proto.TransferName && base.TryComp<MetaDataComponent>(target, ref targetMeta) && base.TryComp<MetaDataComponent>(child, ref childMeta))
			{
				childMeta.EntityName = targetMeta.EntityName;
			}
			if (proto.TransferHumanoidAppearance)
			{
				this._humanoid.CloneAppearance(target, child, null, null);
			}
			MindComponent mind;
			if (base.TryComp<MindComponent>(target, ref mind) && mind.Mind != null)
			{
				mind.Mind.TransferTo(new EntityUid?(child), false, false);
			}
			this.EnsurePausesdMap();
			if (this.PausedMap != null)
			{
				targetTransformComp.AttachParent(base.Transform(this.PausedMap.Value));
			}
			return new EntityUid?(child);
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x00048724 File Offset: 0x00046924
		public void CreatePolymorphAction(string id, EntityUid target)
		{
			PolymorphPrototype polyproto;
			if (!this._proto.TryIndex<PolymorphPrototype>(id, ref polyproto))
			{
				this._saw.Error("Invalid polymorph prototype");
				return;
			}
			PolymorphableComponent polycomp;
			if (!base.TryComp<PolymorphableComponent>(target, ref polycomp))
			{
				return;
			}
			EntityPrototype entproto = this._proto.Index<EntityPrototype>(polyproto.Entity);
			InstantAction act = new InstantAction
			{
				Event = new PolymorphActionEvent
				{
					Prototype = polyproto
				},
				DisplayName = Loc.GetString("polymorph-self-action-name", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", entproto.Name)
				}),
				Description = Loc.GetString("polymorph-self-action-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", entproto.Name)
				}),
				Icon = new SpriteSpecifier.EntityPrototype(polyproto.Entity),
				ItemIconStyle = ItemActionIconStyle.NoItem
			};
			if (polycomp.PolymorphActions == null)
			{
				polycomp.PolymorphActions = new Dictionary<string, InstantAction>();
			}
			polycomp.PolymorphActions.Add(id, act);
			this._actions.AddAction(target, act, new EntityUid?(target), null, true);
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x00048838 File Offset: 0x00046A38
		public void RemovePolymorphAction(string id, EntityUid target)
		{
			PolymorphPrototype polyproto;
			if (!this._proto.TryIndex<PolymorphPrototype>(id, ref polyproto))
			{
				return;
			}
			PolymorphableComponent comp;
			if (!base.TryComp<PolymorphableComponent>(target, ref comp))
			{
				return;
			}
			if (comp.PolymorphActions == null)
			{
				return;
			}
			InstantAction val;
			comp.PolymorphActions.TryGetValue(id, out val);
			if (val != null)
			{
				this._actions.RemoveAction(target, val, null);
			}
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x0004888B File Offset: 0x00046A8B
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x00048893 File Offset: 0x00046A93
		public EntityUid? PausedMap { get; private set; }

		// Token: 0x06000E56 RID: 3670 RVA: 0x0004889C File Offset: 0x00046A9C
		private void InitializeMap()
		{
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000488B4 File Offset: 0x00046AB4
		private void OnRoundRestart(RoundRestartCleanupEvent _)
		{
			if (this.PausedMap == null || !base.Exists(this.PausedMap))
			{
				return;
			}
			this.EntityManager.DeleteEntity(this.PausedMap.Value);
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x000488FC File Offset: 0x00046AFC
		private void EnsurePausesdMap()
		{
			if (this.PausedMap != null && base.Exists(this.PausedMap))
			{
				return;
			}
			MapId newmap = this._mapManager.CreateMap(null);
			this._mapManager.SetMapPaused(newmap, true);
			this.PausedMap = new EntityUid?(this._mapManager.GetMapEntityId(newmap));
			base.Dirty(this.PausedMap.Value, null);
		}

		// Token: 0x04000857 RID: 2135
		private Queue<PolymorphQueuedData> _queuedPolymorphUpdates = new Queue<PolymorphQueuedData>();

		// Token: 0x04000858 RID: 2136
		private readonly ISawmill _saw;

		// Token: 0x04000859 RID: 2137
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x0400085A RID: 2138
		[Dependency]
		private readonly BuckleSystem _buckle;

		// Token: 0x0400085B RID: 2139
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x0400085C RID: 2140
		[Dependency]
		private readonly IComponentFactory _compFact;

		// Token: 0x0400085D RID: 2141
		[Dependency]
		private readonly ServerInventorySystem _inventory;

		// Token: 0x0400085E RID: 2142
		[Dependency]
		private readonly SharedHandsSystem _sharedHands;

		// Token: 0x0400085F RID: 2143
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x04000860 RID: 2144
		[Dependency]
		private readonly MobThresholdSystem _mobThresholdSystem;

		// Token: 0x04000861 RID: 2145
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000862 RID: 2146
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoid;

		// Token: 0x04000863 RID: 2147
		[Dependency]
		private readonly ContainerSystem _container;
	}
}
