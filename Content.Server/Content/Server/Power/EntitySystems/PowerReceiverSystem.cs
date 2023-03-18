using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Hands.Components;
using Content.Server.Power.Components;
using Content.Server.Power.NodeGroups;
using Content.Server.Power.Pow3r;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Power;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x02000299 RID: 665
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerReceiverSystem : EntitySystem
	{
		// Token: 0x06000D82 RID: 3458 RVA: 0x00046AE4 File Offset: 0x00044CE4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, ExaminedEvent>(new ComponentEventHandler<ApcPowerReceiverComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, ExtensionCableSystem.ProviderConnectedEvent>(new ComponentEventHandler<ApcPowerReceiverComponent, ExtensionCableSystem.ProviderConnectedEvent>(this.OnProviderConnected), null, null);
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, ExtensionCableSystem.ProviderDisconnectedEvent>(new ComponentEventHandler<ApcPowerReceiverComponent, ExtensionCableSystem.ProviderDisconnectedEvent>(this.OnProviderDisconnected), null, null);
			base.SubscribeLocalEvent<ApcPowerProviderComponent, ComponentShutdown>(new ComponentEventHandler<ApcPowerProviderComponent, ComponentShutdown>(this.OnProviderShutdown), null, null);
			base.SubscribeLocalEvent<ApcPowerProviderComponent, ExtensionCableSystem.ReceiverConnectedEvent>(new ComponentEventHandler<ApcPowerProviderComponent, ExtensionCableSystem.ReceiverConnectedEvent>(this.OnReceiverConnected), null, null);
			base.SubscribeLocalEvent<ApcPowerProviderComponent, ExtensionCableSystem.ReceiverDisconnectedEvent>(new ComponentEventHandler<ApcPowerProviderComponent, ExtensionCableSystem.ReceiverDisconnectedEvent>(this.OnReceiverDisconnected), null, null);
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<ApcPowerReceiverComponent, GetVerbsEvent<Verb>>(this.OnGetVerbs), null, null);
			base.SubscribeLocalEvent<PowerSwitchComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<PowerSwitchComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSwitchPowerVerb), null, null);
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x00046B98 File Offset: 0x00044D98
		private void OnGetVerbs(EntityUid uid, ApcPowerReceiverComponent component, GetVerbsEvent<Verb> args)
		{
			if (!this._adminManager.HasAdminFlag(args.User, AdminFlags.Admin))
			{
				return;
			}
			args.Verbs.Add(new Verb
			{
				Text = Loc.GetString("verb-debug-toggle-need-power"),
				Category = VerbCategory.Debug,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/smite.svg.192dpi.png", "/")),
				Act = delegate()
				{
					component.NeedsPower = !component.NeedsPower;
				}
			});
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x00046C20 File Offset: 0x00044E20
		private void OnExamined(EntityUid uid, ApcPowerReceiverComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("power-receiver-component-on-examine-main", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("stateText", Loc.GetString(component.Powered ? "power-receiver-component-on-examine-powered" : "power-receiver-component-on-examine-unpowered"))
			}));
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x00046C70 File Offset: 0x00044E70
		private void OnProviderShutdown(EntityUid uid, ApcPowerProviderComponent component, ComponentShutdown args)
		{
			foreach (ApcPowerReceiverComponent apcPowerReceiverComponent in component.LinkedReceivers)
			{
				apcPowerReceiverComponent.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
				IApcNet net = component.Net;
				if (net != null)
				{
					net.QueueNetworkReconnect();
				}
			}
			component.LinkedReceivers.Clear();
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x00046CE8 File Offset: 0x00044EE8
		private void OnProviderConnected(EntityUid uid, ApcPowerReceiverComponent receiver, ExtensionCableSystem.ProviderConnectedEvent args)
		{
			EntityUid providerUid = args.Provider.Owner;
			ApcPowerProviderComponent provider;
			if (!this.EntityManager.TryGetComponent<ApcPowerProviderComponent>(providerUid, ref provider))
			{
				return;
			}
			receiver.Provider = provider;
			this.ProviderChanged(receiver);
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x00046D20 File Offset: 0x00044F20
		private void OnProviderDisconnected(EntityUid uid, ApcPowerReceiverComponent receiver, ExtensionCableSystem.ProviderDisconnectedEvent args)
		{
			receiver.Provider = null;
			this.ProviderChanged(receiver);
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x00046D30 File Offset: 0x00044F30
		private void OnReceiverConnected(EntityUid uid, ApcPowerProviderComponent provider, ExtensionCableSystem.ReceiverConnectedEvent args)
		{
			ApcPowerReceiverComponent receiver;
			if (this.EntityManager.TryGetComponent<ApcPowerReceiverComponent>(args.Receiver.Owner, ref receiver))
			{
				provider.AddReceiver(receiver);
			}
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x00046D60 File Offset: 0x00044F60
		private void OnReceiverDisconnected(EntityUid uid, ApcPowerProviderComponent provider, ExtensionCableSystem.ReceiverDisconnectedEvent args)
		{
			ApcPowerReceiverComponent receiver;
			if (this.EntityManager.TryGetComponent<ApcPowerReceiverComponent>(args.Receiver.Owner, ref receiver))
			{
				provider.RemoveReceiver(receiver);
			}
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x00046D90 File Offset: 0x00044F90
		private void AddSwitchPowerVerb(EntityUid uid, PowerSwitchComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			if (!base.HasComp<HandsComponent>(args.User))
			{
				return;
			}
			ApcPowerReceiverComponent receiver;
			if (!base.TryComp<ApcPowerReceiverComponent>(uid, ref receiver))
			{
				return;
			}
			if (!receiver.NeedsPower)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.TogglePower(uid, true, null, new EntityUid?(args.User));
				},
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png", "/")),
				Text = Loc.GetString("power-switch-component-toggle-verb"),
				Priority = -3
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x00046E60 File Offset: 0x00045060
		private void ProviderChanged(ApcPowerReceiverComponent receiver)
		{
			receiver.NetworkLoad.LinkedNetwork = default(PowerState.NodeId);
			PowerChangedEvent ev = new PowerChangedEvent(receiver.Powered, receiver.NetworkLoad.ReceivingPower);
			base.RaiseLocalEvent<PowerChangedEvent>(receiver.Owner, ref ev, false);
			this._appearance.SetData(receiver.Owner, PowerDeviceVisuals.Powered, receiver.Powered, null);
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00046EC8 File Offset: 0x000450C8
		[NullableContext(2)]
		public bool IsPowered(EntityUid uid, ApcPowerReceiverComponent receiver = null)
		{
			return !base.Resolve<ApcPowerReceiverComponent>(uid, ref receiver, false) || receiver.Powered;
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00046EE0 File Offset: 0x000450E0
		[NullableContext(2)]
		public bool TogglePower(EntityUid uid, bool playSwitchSound = true, ApcPowerReceiverComponent receiver = null, EntityUid? user = null)
		{
			if (!base.Resolve<ApcPowerReceiverComponent>(uid, ref receiver, false))
			{
				return true;
			}
			if (!receiver.NeedsPower)
			{
				receiver.PowerDisabled = false;
				return true;
			}
			receiver.PowerDisabled = !receiver.PowerDisabled;
			if (user != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(32, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "player", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" hit power button on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(", it's now ");
				logStringHandler.AppendFormatted((!receiver.PowerDisabled) ? "on" : "off");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			if (playSwitchSound)
			{
				this._audio.PlayPvs(new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg", null), uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			}
			return !receiver.PowerDisabled;
		}

		// Token: 0x0400080D RID: 2061
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400080E RID: 2062
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x0400080F RID: 2063
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x04000810 RID: 2064
		[Dependency]
		private readonly AudioSystem _audio;
	}
}
