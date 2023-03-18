using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.UserInterface;
using Content.Shared.Eye.Blinding;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Eye.Blinding
{
	// Token: 0x02000504 RID: 1284
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActivatableUIRequiresVisionSystem : EntitySystem
	{
		// Token: 0x06001A7F RID: 6783 RVA: 0x0008BA6E File Offset: 0x00089C6E
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresVisionComponent, ActivatableUIOpenAttemptEvent>(this.OnOpenAttempt), null, null);
			base.SubscribeLocalEvent<BlindableComponent, BlindnessChangedEvent>(new ComponentEventHandler<BlindableComponent, BlindnessChangedEvent>(this.OnBlindnessChanged), null, null);
		}

		// Token: 0x06001A80 RID: 6784 RVA: 0x0008BAA0 File Offset: 0x00089CA0
		private void OnOpenAttempt(EntityUid uid, ActivatableUIRequiresVisionComponent component, ActivatableUIOpenAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			BlindableComponent blindable;
			if (base.TryComp<BlindableComponent>(args.User, ref blindable) && blindable.Sources > 0)
			{
				this._popupSystem.PopupCursor(Loc.GetString("blindness-fail-attempt"), args.User, PopupType.MediumCaution);
				args.Cancel();
			}
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x0008BAF4 File Offset: 0x00089CF4
		private void OnBlindnessChanged(EntityUid uid, BlindableComponent component, BlindnessChangedEvent args)
		{
			if (!args.Blind)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(uid, ref actor))
			{
				return;
			}
			List<BoundUserInterface> uiList = this._userInterfaceSystem.GetAllUIsForSession(actor.PlayerSession);
			if (uiList == null)
			{
				return;
			}
			Queue<BoundUserInterface> closeList = new Queue<BoundUserInterface>();
			foreach (BoundUserInterface ui in uiList)
			{
				if (base.HasComp<ActivatableUIRequiresVisionComponent>(ui.Owner))
				{
					closeList.Enqueue(ui);
				}
			}
			foreach (BoundUserInterface ui2 in closeList)
			{
				this._userInterfaceSystem.CloseUi(ui2, actor.PlayerSession, null);
			}
		}

		// Token: 0x040010E0 RID: 4320
		[Dependency]
		private readonly ActivatableUISystem _activatableUISystem;

		// Token: 0x040010E1 RID: 4321
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040010E2 RID: 4322
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;
	}
}
