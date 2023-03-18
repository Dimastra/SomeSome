using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Resist
{
	// Token: 0x02000239 RID: 569
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResistLockerSystem : EntitySystem
	{
		// Token: 0x06000B58 RID: 2904 RVA: 0x0003BDD0 File Offset: 0x00039FD0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ResistLockerComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<ResistLockerComponent, ContainerRelayMovementEntityEvent>(this.OnRelayMovement), null, null);
			base.SubscribeLocalEvent<ResistLockerComponent, DoAfterEvent>(new ComponentEventHandler<ResistLockerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<ResistLockerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ResistLockerComponent, EntRemovedFromContainerMessage>(this.OnRemoved), null, null);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0003BE20 File Offset: 0x0003A020
		private void OnRelayMovement(EntityUid uid, ResistLockerComponent component, ref ContainerRelayMovementEntityEvent args)
		{
			if (component.IsResisting)
			{
				return;
			}
			EntityStorageComponent storageComponent;
			if (!base.TryComp<EntityStorageComponent>(uid, ref storageComponent))
			{
				return;
			}
			LockComponent lockComponent;
			if ((base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked) || storageComponent.IsWeldedShut)
			{
				this.AttemptResist(args.Entity, uid, storageComponent, component);
			}
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0003BE6C File Offset: 0x0003A06C
		[NullableContext(2)]
		private void AttemptResist(EntityUid user, EntityUid target, EntityStorageComponent storageComponent = null, ResistLockerComponent resistLockerComponent = null)
		{
			if (!base.Resolve<EntityStorageComponent, ResistLockerComponent>(target, ref storageComponent, ref resistLockerComponent, true))
			{
				return;
			}
			float resistTime = resistLockerComponent.ResistTime;
			EntityUid? target2 = new EntityUid?(target);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, resistTime, default(CancellationToken), target2, null)
			{
				BreakOnTargetMove = false,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = false
			};
			resistLockerComponent.IsResisting = true;
			this._popupSystem.PopupEntity(Loc.GetString("resist-locker-component-start-resisting"), user, user, PopupType.Large);
			resistLockerComponent.DoAfter = this._doAfterSystem.DoAfter(doAfterEventArgs);
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0003BF07 File Offset: 0x0003A107
		private void OnRemoved(EntityUid uid, ResistLockerComponent component, EntRemovedFromContainerMessage args)
		{
			if (component.DoAfter != null)
			{
				this._doAfterSystem.Cancel(uid, component.DoAfter, null);
			}
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0003BF24 File Offset: 0x0003A124
		private void OnDoAfter(EntityUid uid, ResistLockerComponent component, DoAfterEvent args)
		{
			if (args.Cancelled)
			{
				component.IsResisting = false;
				this._popupSystem.PopupEntity(Loc.GetString("resist-locker-component-resist-interrupted"), args.Args.User, args.Args.User, PopupType.Medium);
				return;
			}
			if (args.Handled || args.Args.Target == null)
			{
				return;
			}
			component.IsResisting = false;
			EntityStorageComponent storageComponent;
			if (base.TryComp<EntityStorageComponent>(uid, ref storageComponent))
			{
				if (storageComponent.IsWeldedShut)
				{
					storageComponent.IsWeldedShut = false;
				}
				LockComponent lockComponent;
				if (base.TryComp<LockComponent>(args.Args.Target.Value, ref lockComponent))
				{
					this._lockSystem.Unlock(uid, new EntityUid?(args.Args.User), lockComponent);
				}
				this._entityStorage.TryOpenStorage(args.Args.User, uid, false);
			}
			args.Handled = true;
		}

		// Token: 0x04000701 RID: 1793
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000702 RID: 1794
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000703 RID: 1795
		[Dependency]
		private readonly LockSystem _lockSystem;

		// Token: 0x04000704 RID: 1796
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;
	}
}
