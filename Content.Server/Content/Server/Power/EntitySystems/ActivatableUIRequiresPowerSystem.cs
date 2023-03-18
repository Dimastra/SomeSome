using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Server.Wires;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000288 RID: 648
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ActivatableUIRequiresPowerSystem : EntitySystem
	{
		// Token: 0x06000CF5 RID: 3317 RVA: 0x00043B08 File Offset: 0x00041D08
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, PowerChangedEvent>(new ComponentEventRefHandler<ActivatableUIRequiresPowerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x00043B38 File Offset: 0x00041D38
		private void OnActivate(EntityUid uid, ActivatableUIRequiresPowerComponent component, ActivatableUIOpenAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			if (this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			WiresComponent wires;
			if (base.TryComp<WiresComponent>(uid, ref wires) && wires.IsPanelOpen)
			{
				return;
			}
			args.User.PopupMessageCursor(Loc.GetString("base-computer-ui-component-not-powered", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("machine", uid)
			}));
			args.Cancel();
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00043BAB File Offset: 0x00041DAB
		private void OnPowerChanged(EntityUid uid, ActivatableUIRequiresPowerComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				this._activatableUISystem.CloseAll(uid, null);
			}
		}

		// Token: 0x040007DC RID: 2012
		[Dependency]
		private readonly ActivatableUISystem _activatableUISystem;
	}
}
