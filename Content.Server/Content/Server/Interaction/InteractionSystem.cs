using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Storage.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Interaction
{
	// Token: 0x02000446 RID: 1094
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InteractionSystem : SharedInteractionSystem
	{
		// Token: 0x0600160E RID: 5646 RVA: 0x00074A71 File Offset: 0x00072C71
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<DragDropRequestEvent>(new EntitySessionEventHandler<DragDropRequestEvent>(this.HandleDragDropRequestEvent), null, null);
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x00074A90 File Offset: 0x00072C90
		public override bool CanAccessViaStorage(EntityUid user, EntityUid target)
		{
			if (base.Deleted(target, null))
			{
				return false;
			}
			IContainer container;
			if (!this._container.TryGetContainingContainer(target, ref container, null, null))
			{
				return false;
			}
			ServerStorageComponent storage;
			if (!base.TryComp<ServerStorageComponent>(container.Owner, ref storage))
			{
				return false;
			}
			Container storage2 = storage.Storage;
			ActorComponent actor;
			return !(((storage2 != null) ? storage2.ID : null) != container.ID) && base.TryComp<ActorComponent>(user, ref actor) && this._uiSystem.SessionHasOpenUi(container.Owner, SharedStorageComponent.StorageUiKey.Key, actor.PlayerSession, null);
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x00074B1C File Offset: 0x00072D1C
		private void HandleDragDropRequestEvent(DragDropRequestEvent msg, EntitySessionEventArgs args)
		{
			if (base.Deleted(msg.Dragged, null) || base.Deleted(msg.Target, null))
			{
				return;
			}
			EntityUid? user = args.SenderSession.AttachedEntity;
			if (user == null || !this._actionBlockerSystem.CanInteract(user.Value, new EntityUid?(msg.Target)))
			{
				return;
			}
			if (!base.InRangeUnobstructed(user.Value, msg.Dragged, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true) || !base.InRangeUnobstructed(user.Value, msg.Target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
			{
				return;
			}
			DragDropDraggedEvent dragArgs = new DragDropDraggedEvent(user.Value, msg.Target);
			base.RaiseLocalEvent<DragDropDraggedEvent>(msg.Dragged, ref dragArgs, false);
			if (dragArgs.Handled)
			{
				return;
			}
			DragDropTargetEvent dropArgs = new DragDropTargetEvent(user.Value, msg.Dragged);
			base.RaiseLocalEvent<DragDropTargetEvent>(msg.Target, ref dropArgs, false);
		}

		// Token: 0x04000DCE RID: 3534
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000DCF RID: 3535
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000DD0 RID: 3536
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000DD1 RID: 3537
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000DD2 RID: 3538
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;
	}
}
