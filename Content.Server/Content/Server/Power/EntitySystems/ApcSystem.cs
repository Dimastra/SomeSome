using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.APC;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000289 RID: 649
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ApcSystem : EntitySystem
	{
		// Token: 0x06000CF9 RID: 3321 RVA: 0x00043BCC File Offset: 0x00041DCC
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(PowerNetSystem));
			base.SubscribeLocalEvent<ApcComponent, MapInitEvent>(new ComponentEventHandler<ApcComponent, MapInitEvent>(this.OnApcInit), null, null);
			base.SubscribeLocalEvent<ApcComponent, ChargeChangedEvent>(new ComponentEventHandler<ApcComponent, ChargeChangedEvent>(this.OnBatteryChargeChanged), null, null);
			base.SubscribeLocalEvent<ApcComponent, ApcToggleMainBreakerMessage>(new ComponentEventHandler<ApcComponent, ApcToggleMainBreakerMessage>(this.OnToggleMainBreaker), null, null);
			base.SubscribeLocalEvent<ApcComponent, GotEmaggedEvent>(new ComponentEventRefHandler<ApcComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<ApcSystem.ApcToolFinishedEvent>(new EntityEventHandler<ApcSystem.ApcToolFinishedEvent>(this.OnToolFinished), null, null);
			base.SubscribeLocalEvent<ApcComponent, InteractUsingEvent>(new ComponentEventHandler<ApcComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ApcComponent, ExaminedEvent>(new ComponentEventHandler<ApcComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00043C80 File Offset: 0x00041E80
		private void OnBatteryChargeChanged(EntityUid uid, ApcComponent component, ChargeChangedEvent args)
		{
			this.UpdateApcState(uid, component, null);
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00043C8B File Offset: 0x00041E8B
		private void OnApcInit(EntityUid uid, ApcComponent component, MapInitEvent args)
		{
			this.UpdateApcState(uid, component, null);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00043C98 File Offset: 0x00041E98
		private void OnToggleMainBreaker(EntityUid uid, ApcComponent component, ApcToggleMainBreakerMessage args)
		{
			AccessReaderComponent access;
			base.TryComp<AccessReaderComponent>(uid, ref access);
			if (args.Session.AttachedEntity == null)
			{
				return;
			}
			if (access == null || this._accessReader.IsAllowed(args.Session.AttachedEntity.Value, access))
			{
				this.ApcToggleBreaker(uid, component, null);
				return;
			}
			this._popupSystem.PopupCursor(Loc.GetString("apc-component-insufficient-access"), args.Session, PopupType.Medium);
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x00043D10 File Offset: 0x00041F10
		[NullableContext(2)]
		public void ApcToggleBreaker(EntityUid uid, ApcComponent apc = null, PowerNetworkBatteryComponent battery = null)
		{
			if (!base.Resolve<ApcComponent, PowerNetworkBatteryComponent>(uid, ref apc, ref battery, true))
			{
				return;
			}
			apc.MainBreakerEnabled = !apc.MainBreakerEnabled;
			battery.CanDischarge = apc.MainBreakerEnabled;
			this.UpdateUIState(uid, apc, null, null);
			SoundSystem.Play(apc.OnReceiveMessageSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00043D87 File Offset: 0x00041F87
		private void OnEmagged(EntityUid uid, ApcComponent comp, ref GotEmaggedEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x00043D90 File Offset: 0x00041F90
		[NullableContext(2)]
		public void UpdateApcState(EntityUid uid, ApcComponent apc = null, BatteryComponent battery = null)
		{
			if (!base.Resolve<ApcComponent, BatteryComponent>(uid, ref apc, ref battery, true))
			{
				return;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				this.UpdatePanelAppearance(uid, appearance, apc);
			}
			ApcChargeState newState = this.CalcChargeState(uid, apc, battery);
			if (newState != apc.LastChargeState && apc.LastChargeStateTime + ApcComponent.VisualsChangeDelay < this._gameTiming.CurTime)
			{
				apc.LastChargeState = newState;
				apc.LastChargeStateTime = this._gameTiming.CurTime;
				if (appearance != null)
				{
					this._appearance.SetData(uid, ApcVisuals.ChargeState, newState, appearance);
				}
			}
			ApcExternalPowerState extPowerState = this.CalcExtPowerState(uid, apc, battery);
			if (extPowerState != apc.LastExternalState || apc.LastUiUpdate + ApcComponent.VisualsChangeDelay < this._gameTiming.CurTime)
			{
				apc.LastExternalState = extPowerState;
				apc.LastUiUpdate = this._gameTiming.CurTime;
				this.UpdateUIState(uid, apc, battery, null);
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x00043E80 File Offset: 0x00042080
		[NullableContext(2)]
		public void UpdateUIState(EntityUid uid, ApcComponent apc = null, BatteryComponent battery = null, ServerUserInterfaceComponent ui = null)
		{
			if (!base.Resolve<ApcComponent, BatteryComponent, ServerUserInterfaceComponent>(uid, ref apc, ref battery, ref ui, true))
			{
				return;
			}
			PowerNetworkBatteryComponent netBattery = base.Comp<PowerNetworkBatteryComponent>(uid);
			float power = (netBattery != null) ? netBattery.CurrentSupply : 0f;
			BoundUserInterface bui = this._userInterfaceSystem.GetUiOrNull(uid, ApcUiKey.Key, ui);
			if (bui != null)
			{
				bui.SetState(new ApcBoundInterfaceState(apc.MainBreakerEnabled, (int)MathF.Ceiling(power), apc.LastExternalState, battery.CurrentCharge / battery.MaxCharge), null, true);
			}
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x00043EFC File Offset: 0x000420FC
		[NullableContext(2)]
		public ApcChargeState CalcChargeState(EntityUid uid, ApcComponent apc = null, BatteryComponent battery = null)
		{
			if (apc != null && base.HasComp<EmaggedComponent>(uid))
			{
				return ApcChargeState.Emag;
			}
			if (!base.Resolve<ApcComponent, BatteryComponent>(uid, ref apc, ref battery, true))
			{
				return ApcChargeState.Lack;
			}
			if (battery.CurrentCharge / battery.MaxCharge > 0.9f)
			{
				return ApcChargeState.Full;
			}
			PowerNetworkBatteryComponent netBattery = base.Comp<PowerNetworkBatteryComponent>(uid);
			if (netBattery.CurrentSupply - netBattery.CurrentReceiving >= 0f)
			{
				return ApcChargeState.Lack;
			}
			return ApcChargeState.Charging;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x00043F5C File Offset: 0x0004215C
		[NullableContext(2)]
		public ApcExternalPowerState CalcExtPowerState(EntityUid uid, ApcComponent apc = null, BatteryComponent battery = null)
		{
			if (!base.Resolve<ApcComponent, BatteryComponent>(uid, ref apc, ref battery, true))
			{
				return ApcExternalPowerState.None;
			}
			PowerNetworkBatteryComponent netBat = base.Comp<PowerNetworkBatteryComponent>(uid);
			if (netBat.CurrentReceiving == 0f && !MathHelper.CloseTo(battery.CurrentCharge / battery.MaxCharge, 1f, 1E-07f))
			{
				return ApcExternalPowerState.None;
			}
			float delta = netBat.CurrentReceiving - netBat.CurrentSupply;
			if (!MathHelper.CloseToPercent(delta, 0f, 0.10000000149011612) && delta < 0f)
			{
				return ApcExternalPowerState.Low;
			}
			return ApcExternalPowerState.Good;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x00043FDD File Offset: 0x000421DD
		public static ApcPanelState GetPanelState(ApcComponent apc)
		{
			if (apc.IsApcOpen)
			{
				return ApcPanelState.Open;
			}
			return ApcPanelState.Closed;
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x00043FEC File Offset: 0x000421EC
		private void OnInteractUsing(EntityUid uid, ApcComponent component, InteractUsingEvent args)
		{
			ToolComponent tool;
			if (!this.EntityManager.TryGetComponent<ToolComponent>(args.Used, ref tool))
			{
				return;
			}
			ToolEventData toolEvData = new ToolEventData(new ApcSystem.ApcToolFinishedEvent(uid), 0f, null, null);
			if (this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), 2f, new string[]
			{
				"Screwing"
			}, toolEvData, 0f, tool, null, null))
			{
				args.Handled = true;
			}
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0004406C File Offset: 0x0004226C
		private void OnToolFinished(ApcSystem.ApcToolFinishedEvent args)
		{
			ApcComponent component;
			if (!this.EntityManager.TryGetComponent<ApcComponent>(args.Target, ref component))
			{
				return;
			}
			component.IsApcOpen = !component.IsApcOpen;
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(args.Target, ref appearance))
			{
				this.UpdatePanelAppearance(args.Target, appearance, null);
			}
			if (component.IsApcOpen)
			{
				SoundSystem.Play(component.ScrewdriverOpenSound.GetSound(null, null), Filter.Pvs(args.Target, 2f, null, null, null), args.Target, null);
				return;
			}
			SoundSystem.Play(component.ScrewdriverCloseSound.GetSound(null, null), Filter.Pvs(args.Target, 2f, null, null, null), args.Target, null);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0004412F File Offset: 0x0004232F
		[NullableContext(2)]
		private void UpdatePanelAppearance(EntityUid uid, AppearanceComponent appearance = null, ApcComponent apc = null)
		{
			if (!base.Resolve<AppearanceComponent, ApcComponent>(uid, ref appearance, ref apc, false))
			{
				return;
			}
			this._appearance.SetData(uid, ApcVisuals.PanelState, ApcSystem.GetPanelState(apc), appearance);
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0004415E File Offset: 0x0004235E
		private void OnExamine(EntityUid uid, ApcComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString(component.IsApcOpen ? "apc-component-on-examine-panel-open" : "apc-component-on-examine-panel-closed"));
		}

		// Token: 0x040007DD RID: 2013
		[Dependency]
		private readonly AccessReaderSystem _accessReader;

		// Token: 0x040007DE RID: 2014
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040007DF RID: 2015
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040007E0 RID: 2016
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040007E1 RID: 2017
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x040007E2 RID: 2018
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040007E3 RID: 2019
		private const float ScrewTime = 2f;

		// Token: 0x0200093E RID: 2366
		[NullableContext(0)]
		private sealed class ApcToolFinishedEvent : EntityEventArgs
		{
			// Token: 0x1700080D RID: 2061
			// (get) Token: 0x060031BE RID: 12734 RVA: 0x000FFC7C File Offset: 0x000FDE7C
			public EntityUid Target { get; }

			// Token: 0x060031BF RID: 12735 RVA: 0x000FFC84 File Offset: 0x000FDE84
			public ApcToolFinishedEvent(EntityUid target)
			{
				this.Target = target;
			}
		}
	}
}
