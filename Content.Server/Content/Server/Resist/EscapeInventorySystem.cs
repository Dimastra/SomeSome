using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Contests;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.ActionBlocker;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Resist
{
	// Token: 0x02000237 RID: 567
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EscapeInventorySystem : EntitySystem
	{
		// Token: 0x06000B51 RID: 2897 RVA: 0x0003BBCC File Offset: 0x00039DCC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CanEscapeInventoryComponent, MoveInputEvent>(new ComponentEventRefHandler<CanEscapeInventoryComponent, MoveInputEvent>(this.OnRelayMovement), null, null);
			base.SubscribeLocalEvent<CanEscapeInventoryComponent, DoAfterEvent<EscapeInventorySystem.EscapeInventoryEvent>>(new ComponentEventHandler<CanEscapeInventoryComponent, DoAfterEvent<EscapeInventorySystem.EscapeInventoryEvent>>(this.OnEscape), null, null);
			base.SubscribeLocalEvent<CanEscapeInventoryComponent, DroppedEvent>(new ComponentEventHandler<CanEscapeInventoryComponent, DroppedEvent>(this.OnDropped), null, null);
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0003BC1C File Offset: 0x00039E1C
		private void OnRelayMovement(EntityUid uid, CanEscapeInventoryComponent component, ref MoveInputEvent args)
		{
			IContainer container;
			if (!this._containerSystem.TryGetContainingContainer(uid, ref container, null, null) || !this._actionBlockerSystem.CanInteract(uid, new EntityUid?(container.Owner)))
			{
				return;
			}
			Hand inHand;
			if (!this._handsSystem.IsHolding(container.Owner, new EntityUid?(uid), out inHand, null))
			{
				if (base.HasComp<SharedStorageComponent>(container.Owner) || base.HasComp<InventoryComponent>(container.Owner))
				{
					this.AttemptEscape(uid, container.Owner, component, 1f);
				}
				return;
			}
			float contestResults = this._contests.MassContest(uid, container.Owner, null, null);
			if (contestResults != 0f)
			{
				contestResults = 1f / contestResults;
			}
			else
			{
				contestResults = 1f;
			}
			if (contestResults >= 6f)
			{
				return;
			}
			this.AttemptEscape(uid, container.Owner, component, contestResults);
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0003BCE8 File Offset: 0x00039EE8
		private void AttemptEscape(EntityUid user, EntityUid container, CanEscapeInventoryComponent component, float multiplier = 1f)
		{
			EscapeInventorySystem.EscapeInventoryEvent escapeEvent = new EscapeInventorySystem.EscapeInventoryEvent();
			float delay = component.BaseResistTime * multiplier;
			EntityUid? target = new EntityUid?(container);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, delay, default(CancellationToken), target, null)
			{
				BreakOnTargetMove = false,
				BreakOnUserMove = false,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = false
			};
			this._popupSystem.PopupEntity(Loc.GetString("escape-inventory-component-start-resisting"), user, user, PopupType.Small);
			this._popupSystem.PopupEntity(Loc.GetString("escape-inventory-component-start-resisting-target"), container, container, PopupType.Small);
			this._doAfterSystem.DoAfter<EscapeInventorySystem.EscapeInventoryEvent>(doAfterEventArgs, escapeEvent);
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0003BD87 File Offset: 0x00039F87
		private void OnEscape(EntityUid uid, CanEscapeInventoryComponent component, DoAfterEvent<EscapeInventorySystem.EscapeInventoryEvent> args)
		{
			if (args.Handled || args.Cancelled)
			{
				return;
			}
			ContainerHelpers.AttachParentToContainerOrGrid(base.Transform(uid), this.EntityManager);
			args.Handled = true;
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0003BDB3 File Offset: 0x00039FB3
		private void OnDropped(EntityUid uid, CanEscapeInventoryComponent component, DroppedEvent args)
		{
		}

		// Token: 0x040006F7 RID: 1783
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x040006F8 RID: 1784
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040006F9 RID: 1785
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x040006FA RID: 1786
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x040006FB RID: 1787
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040006FC RID: 1788
		[Dependency]
		private readonly ContestsSystem _contests;

		// Token: 0x040006FD RID: 1789
		public const float MaximumMassDisadvantage = 6f;

		// Token: 0x0200091B RID: 2331
		[NullableContext(0)]
		private sealed class EscapeInventoryEvent : EntityEventArgs
		{
		}
	}
}
