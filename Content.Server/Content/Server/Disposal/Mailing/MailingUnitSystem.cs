using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Configurable;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Disposal.Unit.EntitySystems;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.Disposal;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Disposal.Mailing
{
	// Token: 0x02000560 RID: 1376
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MailingUnitSystem : EntitySystem
	{
		// Token: 0x06001D14 RID: 7444 RVA: 0x0009A594 File Offset: 0x00098794
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MailingUnitComponent, ComponentInit>(new ComponentEventHandler<MailingUnitComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<MailingUnitComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, BeforeDisposalFlushEvent>(new ComponentEventHandler<MailingUnitComponent, BeforeDisposalFlushEvent>(this.OnBeforeFlush), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, ConfigurationSystem.ConfigurationUpdatedEvent>(new ComponentEventHandler<MailingUnitComponent, ConfigurationSystem.ConfigurationUpdatedEvent>(this.OnConfigurationUpdated), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, ActivateInWorldEvent>(new ComponentEventHandler<MailingUnitComponent, ActivateInWorldEvent>(this.HandleActivate), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, DisposalUnitUIStateUpdatedEvent>(new ComponentEventHandler<MailingUnitComponent, DisposalUnitUIStateUpdatedEvent>(this.OnDisposalUnitUIStateChange), null, null);
			base.SubscribeLocalEvent<MailingUnitComponent, TargetSelectedMessage>(new ComponentEventHandler<MailingUnitComponent, TargetSelectedMessage>(this.OnTargetSelected), null, null);
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x0009A633 File Offset: 0x00098833
		private void OnComponentInit(EntityUid uid, MailingUnitComponent component, ComponentInit args)
		{
			this.UpdateTargetList(uid, component, null);
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x0009A640 File Offset: 0x00098840
		private void OnPacketReceived(EntityUid uid, MailingUnitComponent component, DeviceNetworkPacketEvent args)
		{
			string command;
			if (!args.Data.TryGetValue<string>("command", out command) || !this.IsPowered(uid, null))
			{
				return;
			}
			string a = command;
			if (a == "get_mailer_tag")
			{
				this.SendTagRequestResponse(uid, args, component.Tag);
				return;
			}
			if (!(a == "mailer_tag"))
			{
				return;
			}
			string tag;
			if (args.Data.TryGetValue<string>("tag", out tag))
			{
				component.TargetList.Add(tag);
				this.UpdateUserInterface(component);
			}
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x0009A6C0 File Offset: 0x000988C0
		private void SendTagRequestResponse(EntityUid uid, DeviceNetworkPacketEvent args, [Nullable(2)] string tag)
		{
			if (tag == null)
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "mailer_tag";
			networkPayload["tag"] = tag;
			NetworkPayload payload = networkPayload;
			this._deviceNetworkSystem.QueuePacket(uid, args.Address, payload, new uint?(args.Frequency), null);
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x0009A713 File Offset: 0x00098913
		private void OnBeforeFlush(EntityUid uid, MailingUnitComponent component, BeforeDisposalFlushEvent args)
		{
			if (string.IsNullOrEmpty(component.Target))
			{
				args.Cancel();
				return;
			}
			args.Tags.Add("mail");
			args.Tags.Add(component.Target);
			this.BroadcastSentMessage(uid, component, null);
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x0009A754 File Offset: 0x00098954
		private void BroadcastSentMessage(EntityUid uid, MailingUnitComponent component, [Nullable(2)] DeviceNetworkComponent device = null)
		{
			if (string.IsNullOrEmpty(component.Tag) || string.IsNullOrEmpty(component.Target) || !base.Resolve<DeviceNetworkComponent>(uid, ref device, true))
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "mail_sent";
			networkPayload["src"] = component.Tag;
			networkPayload["target"] = component.Target;
			NetworkPayload payload = networkPayload;
			this._deviceNetworkSystem.QueuePacket(uid, null, payload, null, device);
		}

		// Token: 0x06001D1A RID: 7450 RVA: 0x0009A7DC File Offset: 0x000989DC
		private void UpdateTargetList(EntityUid uid, MailingUnitComponent component, [Nullable(2)] DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<DeviceNetworkComponent>(uid, ref device, false))
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "get_mailer_tag";
			NetworkPayload payload = networkPayload;
			component.TargetList.Clear();
			this._deviceNetworkSystem.QueuePacket(uid, null, payload, null, device);
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x0009A830 File Offset: 0x00098A30
		private void OnConfigurationUpdated(EntityUid uid, MailingUnitComponent component, ConfigurationSystem.ConfigurationUpdatedEvent args)
		{
			Dictionary<string, string> configuration = args.Configuration.Config;
			if (!configuration.ContainsKey("tag") || configuration["tag"] == string.Empty)
			{
				component.Tag = null;
				return;
			}
			component.Tag = configuration["tag"];
			this.UpdateUserInterface(component);
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x0009A890 File Offset: 0x00098A90
		private void HandleActivate(EntityUid uid, MailingUnitComponent component, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			args.Handled = true;
			this.UpdateTargetList(uid, component, null);
			BoundUserInterface uiOrNull = this._userInterfaceSystem.GetUiOrNull(uid, MailingUnitUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.Open(actor.PlayerSession);
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x0009A8E7 File Offset: 0x00098AE7
		private void OnDisposalUnitUIStateChange(EntityUid uid, MailingUnitComponent component, DisposalUnitUIStateUpdatedEvent args)
		{
			component.DisposalUnitInterfaceState = args.State;
			this.UpdateUserInterface(component);
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x0009A8FC File Offset: 0x00098AFC
		private void UpdateUserInterface(MailingUnitComponent component)
		{
			if (component.DisposalUnitInterfaceState == null)
			{
				return;
			}
			MailingUnitBoundUserInterfaceState state = new MailingUnitBoundUserInterfaceState(component.DisposalUnitInterfaceState, component.Target, component.TargetList, component.Tag);
			BoundUserInterface uiorNull = component.Owner.GetUIOrNull(MailingUnitUiKey.Key);
			if (uiorNull == null)
			{
				return;
			}
			uiorNull.SetState(state, null, true);
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x0009A94E File Offset: 0x00098B4E
		private void OnTargetSelected(EntityUid uid, MailingUnitComponent component, TargetSelectedMessage args)
		{
			if (string.IsNullOrEmpty(args.target))
			{
				component.Target = null;
			}
			component.Target = args.target;
			this.UpdateUserInterface(component);
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x0009A977 File Offset: 0x00098B77
		[NullableContext(2)]
		private bool IsPowered(EntityUid uid, ApcPowerReceiverComponent powerReceiver = null)
		{
			return !base.Resolve<ApcPowerReceiverComponent>(uid, ref powerReceiver, true) || powerReceiver.Powered;
		}

		// Token: 0x0400128E RID: 4750
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x0400128F RID: 4751
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04001290 RID: 4752
		private const string MailTag = "mail";

		// Token: 0x04001291 RID: 4753
		private const string TagConfigurationKey = "tag";

		// Token: 0x04001292 RID: 4754
		private const string NetTag = "tag";

		// Token: 0x04001293 RID: 4755
		private const string NetSrc = "src";

		// Token: 0x04001294 RID: 4756
		private const string NetTarget = "target";

		// Token: 0x04001295 RID: 4757
		private const string NetCmdSent = "mail_sent";

		// Token: 0x04001296 RID: 4758
		private const string NetCmdRequest = "get_mailer_tag";

		// Token: 0x04001297 RID: 4759
		private const string NetCmdResponse = "mailer_tag";
	}
}
