using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Implants.Components;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Implants
{
	// Token: 0x020003F0 RID: 1008
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedSubdermalImplantSystem : EntitySystem
	{
		// Token: 0x06000BD8 RID: 3032 RVA: 0x0002713C File Offset: 0x0002533C
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<SubdermalImplantComponent, EntGotInsertedIntoContainerMessage>(this.OnInsert), null, null);
			base.SubscribeLocalEvent<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>(new ComponentEventHandler<SubdermalImplantComponent, ContainerGettingRemovedAttemptEvent>(this.OnRemoveAttempt), null, null);
			base.SubscribeLocalEvent<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<SubdermalImplantComponent, EntGotRemovedFromContainerMessage>(this.OnRemove), null, null);
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0002717C File Offset: 0x0002537C
		private void OnInsert(EntityUid uid, SubdermalImplantComponent component, EntGotInsertedIntoContainerMessage args)
		{
			if (component.ImplantedEntity == null)
			{
				return;
			}
			if (component.ImplantAction != null)
			{
				InstantAction action = new InstantAction(this._prototypeManager.Index<InstantActionPrototype>(component.ImplantAction));
				this._actionsSystem.AddAction(component.ImplantedEntity.Value, action, new EntityUid?(uid), null, true);
			}
			IContainer implantContainer;
			if (this._container.TryGetContainer(component.ImplantedEntity.Value, "implant", ref implantContainer, null) && this._tag.HasTag(uid, "MacroBomb"))
			{
				foreach (EntityUid implant in implantContainer.ContainedEntities)
				{
					if (this._tag.HasTag(implant, "MicroBomb"))
					{
						implantContainer.Remove(implant, null, null, null, true, false, null, null);
						base.QueueDel(implant);
					}
				}
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0002727C File Offset: 0x0002547C
		private void OnRemoveAttempt(EntityUid uid, SubdermalImplantComponent component, ContainerGettingRemovedAttemptEvent args)
		{
			if (component.Permanent && component.ImplantedEntity != null)
			{
				args.Cancel();
			}
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0002729C File Offset: 0x0002549C
		private void OnRemove(EntityUid uid, SubdermalImplantComponent component, EntGotRemovedFromContainerMessage args)
		{
			if (component.ImplantedEntity == null || base.Terminating(component.ImplantedEntity.Value, null))
			{
				return;
			}
			if (component.ImplantAction != null)
			{
				this._actionsSystem.RemoveProvidedActions(component.ImplantedEntity.Value, uid, null);
			}
			IContainer storageImplant;
			if (!this._container.TryGetContainer(uid, "storagebase", ref storageImplant, null))
			{
				return;
			}
			EntityCoordinates entCoords = base.Transform(component.ImplantedEntity.Value).Coordinates;
			foreach (EntityUid entity in storageImplant.ContainedEntities.ToArray<EntityUid>())
			{
				if (!base.Terminating(entity, null))
				{
					this._container.RemoveEntity(storageImplant.Owner, entity, null, null, null, true, true, new EntityCoordinates?(entCoords), null);
				}
			}
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0002736F File Offset: 0x0002556F
		public void ForceImplant(EntityUid target, EntityUid implant, SubdermalImplantComponent component)
		{
			BaseContainer implantContainer = base.EnsureComp<ImplantedComponent>(target).ImplantContainer;
			component.ImplantedEntity = new EntityUid?(target);
			implantContainer.Insert(implant, null, null, null, null, null);
			base.RaiseLocalEvent<SubdermalImplantInserted>(new SubdermalImplantInserted(target, component));
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x000273A4 File Offset: 0x000255A4
		public void ForceRemove(EntityUid target, EntityUid implant)
		{
			ImplantedComponent implanted;
			if (!base.TryComp<ImplantedComponent>(target, ref implanted))
			{
				return;
			}
			implanted.ImplantContainer.Remove(implant, null, null, null, true, false, null, null);
			base.QueueDel(implant);
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x000273E8 File Offset: 0x000255E8
		public void WipeImplants(EntityUid target)
		{
			ImplantedComponent implanted;
			if (!base.TryComp<ImplantedComponent>(target, ref implanted))
			{
				return;
			}
			Container implantContainer = implanted.ImplantContainer;
			this._container.CleanContainer(implantContainer);
		}

		// Token: 0x04000BC5 RID: 3013
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000BC6 RID: 3014
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04000BC7 RID: 3015
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000BC8 RID: 3016
		[Dependency]
		private readonly TagSystem _tag;

		// Token: 0x04000BC9 RID: 3017
		public const string BaseStorageId = "storagebase";
	}
}
