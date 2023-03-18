using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Borgs
{
	// Token: 0x020000A0 RID: 160
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InnateItemSystem : EntitySystem
	{
		// Token: 0x0600027F RID: 639 RVA: 0x0000D753 File Offset: 0x0000B953
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InnateItemComponent, MindAddedMessage>(new ComponentEventHandler<InnateItemComponent, MindAddedMessage>(this.OnMindAdded), null, null);
			base.SubscribeLocalEvent<InnateItemComponent, InnateAfterInteractActionEvent>(new ComponentEventHandler<InnateItemComponent, InnateAfterInteractActionEvent>(this.StartAfterInteract), null, null);
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000D783 File Offset: 0x0000B983
		private void OnMindAdded(EntityUid uid, InnateItemComponent component, MindAddedMessage args)
		{
			if (!component.AlreadyInitialized)
			{
				this.RefreshItems(uid);
			}
			component.AlreadyInitialized = true;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000D79C File Offset: 0x0000B99C
		private void RefreshItems(EntityUid uid)
		{
			ItemSlotsComponent slotsComp;
			if (!base.TryComp<ItemSlotsComponent>(uid, ref slotsComp))
			{
				return;
			}
			foreach (ItemSlot slot in slotsComp.Slots.Values)
			{
				if (slot.ContainerSlot != null)
				{
					EntityUid? sourceItem = slot.ContainerSlot.ContainedEntity;
					if (sourceItem != null && !this._tagSystem.HasTag(sourceItem.Value, "NoAction"))
					{
						this._actionsSystem.AddAction(uid, this.CreateAction(sourceItem.Value), new EntityUid?(uid), null, true);
					}
				}
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000D850 File Offset: 0x0000BA50
		private EntityTargetAction CreateAction(EntityUid uid)
		{
			return new EntityTargetAction
			{
				DisplayName = base.MetaData(uid).EntityName,
				Description = base.MetaData(uid).EntityDescription,
				EntityIcon = new EntityUid?(uid),
				Event = new InnateAfterInteractActionEvent(uid)
			};
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
		private void StartAfterInteract(EntityUid uid, InnateItemComponent component, InnateAfterInteractActionEvent args)
		{
			AfterInteractEvent ev = new AfterInteractEvent(args.Performer, args.Item, new EntityUid?(args.Target), base.Transform(args.Target).Coordinates, true);
			base.RaiseLocalEvent<AfterInteractEvent>(args.Item, ev, false);
		}

		// Token: 0x040001D3 RID: 467
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x040001D4 RID: 468
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
