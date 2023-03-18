using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Power.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.DeviceNetwork;
using Content.Shared.SurveillanceCamera;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000144 RID: 324
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraRouterSystem : EntitySystem
	{
		// Token: 0x06000619 RID: 1561 RVA: 0x0001D250 File Offset: 0x0001B450
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SurveillanceCameraRouterComponent, ComponentInit>(new ComponentEventHandler<SurveillanceCameraRouterComponent, ComponentInit>(this.OnInitialize), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraRouterComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<SurveillanceCameraRouterComponent, DeviceNetworkPacketEvent>(this.OnPacketReceive), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraRouterComponent, SurveillanceCameraSetupSetNetwork>(new ComponentEventHandler<SurveillanceCameraRouterComponent, SurveillanceCameraSetupSetNetwork>(this.OnSetNetwork), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraRouterComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SurveillanceCameraRouterComponent, GetVerbsEvent<AlternativeVerb>>(this.AddVerbs), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraRouterComponent, PowerChangedEvent>(new ComponentEventRefHandler<SurveillanceCameraRouterComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0001D2C4 File Offset: 0x0001B4C4
		private void OnInitialize(EntityUid uid, SurveillanceCameraRouterComponent router, ComponentInit args)
		{
			DeviceFrequencyPrototype subnetFrequency;
			if (router.SubnetFrequencyId == null || !this._prototypeManager.TryIndex<DeviceFrequencyPrototype>(router.SubnetFrequencyId, ref subnetFrequency))
			{
				return;
			}
			router.SubnetFrequency = subnetFrequency.Frequency;
			router.Active = true;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0001D304 File Offset: 0x0001B504
		private void OnPacketReceive(EntityUid uid, SurveillanceCameraRouterComponent router, DeviceNetworkPacketEvent args)
		{
			string command;
			if (!router.Active || string.IsNullOrEmpty(args.SenderAddress) || !args.Data.TryGetValue<string>("command", out command))
			{
				return;
			}
			uint num = <PrivateImplementationDetails>.ComputeStringHash(command);
			if (num > 1524739277U)
			{
				if (num <= 3964192565U)
				{
					if (num != 1809284963U)
					{
						if (num != 3964192565U)
						{
							return;
						}
						if (!(command == "surveillance_camera_ping_subnet"))
						{
							return;
						}
						this.PingSubnet(uid, router);
						return;
					}
					else
					{
						if (!(command == "surveillance_camera_data"))
						{
							return;
						}
						this.SendCameraInfo(uid, args.Data, router);
					}
				}
				else if (num != 4134956651U)
				{
					if (num == 4148141967U)
					{
						if (!(command == "surveillance_camera_connect"))
						{
							return;
						}
						string address;
						if (!args.Data.TryGetValue<string>("surveillance_camera_data_origin", out address))
						{
							return;
						}
						this.ConnectCamera(uid, args.SenderAddress, address, router);
						return;
					}
				}
				else
				{
					if (!(command == "surveillance_camera_subnet_connect"))
					{
						return;
					}
					this.AddMonitorToRoute(uid, args.SenderAddress, router);
					this.PingSubnet(uid, router);
					return;
				}
				return;
			}
			if (num != 199586393U)
			{
				if (num != 1309913935U)
				{
					if (num != 1524739277U)
					{
						return;
					}
					if (!(command == "surveillance_camera_subnet_disconnect"))
					{
						return;
					}
					this.RemoveMonitorFromRoute(uid, args.SenderAddress, router);
					return;
				}
				else
				{
					if (!(command == "surveillance_camera_ping"))
					{
						return;
					}
					this.SubnetPingResponse(uid, args.SenderAddress, router);
					return;
				}
			}
			else
			{
				if (!(command == "surveillance_camera_heartbeat"))
				{
					return;
				}
				string camera;
				if (!args.Data.TryGetValue<string>("surveillance_camera_data_origin", out camera))
				{
					return;
				}
				this.SendHeartbeat(uid, args.SenderAddress, camera, router);
				return;
			}
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0001D499 File Offset: 0x0001B699
		private void OnPowerChanged(EntityUid uid, SurveillanceCameraRouterComponent component, ref PowerChangedEvent args)
		{
			component.MonitorRoutes.Clear();
			component.Active = args.Powered;
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0001D4B4 File Offset: 0x0001B6B4
		private void AddVerbs(EntityUid uid, SurveillanceCameraRouterComponent component, GetVerbsEvent<AlternativeVerb> verbs)
		{
			if (!this._actionBlocker.CanInteract(verbs.User, new EntityUid?(uid)))
			{
				return;
			}
			if (component.SubnetFrequencyId != null)
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb();
			verb.Text = Loc.GetString("surveillance-camera-setup");
			verb.Act = delegate()
			{
				this.OpenSetupInterface(uid, verbs.User, component, null);
			};
			verbs.Verbs.Add(verb);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0001D550 File Offset: 0x0001B750
		private void OnSetNetwork(EntityUid uid, SurveillanceCameraRouterComponent component, SurveillanceCameraSetupSetNetwork args)
		{
			Enum uiKey = args.UiKey;
			if (uiKey is SurveillanceCameraSetupUiKey)
			{
				SurveillanceCameraSetupUiKey key = (SurveillanceCameraSetupUiKey)uiKey;
				if (key == SurveillanceCameraSetupUiKey.Router)
				{
					if (args.Network < 0 || args.Network >= component.AvailableNetworks.Count)
					{
						return;
					}
					DeviceFrequencyPrototype frequency;
					if (!this._prototypeManager.TryIndex<DeviceFrequencyPrototype>(component.AvailableNetworks[args.Network], ref frequency))
					{
						return;
					}
					component.SubnetFrequencyId = component.AvailableNetworks[args.Network];
					component.SubnetFrequency = frequency.Frequency;
					component.Active = true;
					this.UpdateSetupInterface(uid, component, null);
					return;
				}
			}
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0001D5EC File Offset: 0x0001B7EC
		[NullableContext(2)]
		private void OpenSetupInterface(EntityUid uid, EntityUid player, SurveillanceCameraRouterComponent camera = null, ActorComponent actor = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref camera, true) || !base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			this._userInterface.GetUiOrNull(uid, SurveillanceCameraSetupUiKey.Router, null).Open(actor.PlayerSession);
			this.UpdateSetupInterface(uid, camera, null);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0001D63C File Offset: 0x0001B83C
		[NullableContext(2)]
		private void UpdateSetupInterface(EntityUid uid, SurveillanceCameraRouterComponent router = null, DeviceNetworkComponent deviceNet = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent, DeviceNetworkComponent>(uid, ref router, ref deviceNet, true))
			{
				return;
			}
			if (router.AvailableNetworks.Count == 0 || router.SubnetFrequencyId != null)
			{
				this._userInterface.TryCloseAll(uid, SurveillanceCameraSetupUiKey.Router, null);
				return;
			}
			SurveillanceCameraSetupBoundUiState state = new SurveillanceCameraSetupBoundUiState(router.SubnetName, deviceNet.ReceiveFrequency.GetValueOrDefault(), router.AvailableNetworks, true, router.SubnetFrequencyId != null);
			this._userInterface.TrySetUiState(uid, SurveillanceCameraSetupUiKey.Router, state, null, null, true);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0001D6C0 File Offset: 0x0001B8C0
		private void SendHeartbeat(EntityUid uid, string origin, string destination, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_heartbeat"
				},
				{
					"surveillance_camera_data_origin",
					origin
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, destination, payload, new uint?(router.SubnetFrequency), null);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0001D718 File Offset: 0x0001B918
		private void SubnetPingResponse(EntityUid uid, string origin, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true) || router.SubnetFrequencyId == null)
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_data_subnet"
				},
				{
					"surveillance_camera_data_subnet",
					router.SubnetFrequencyId
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, origin, payload, null, null);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0001D77C File Offset: 0x0001B97C
		private void ConnectCamera(EntityUid uid, string origin, string address, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_connect"
				},
				{
					"surveillance_camera_data_origin",
					origin
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, address, payload, new uint?(router.SubnetFrequency), null);
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0001D7D4 File Offset: 0x0001B9D4
		private void AddMonitorToRoute(EntityUid uid, string address, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			router.MonitorRoutes.Add(address);
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0001D7F0 File Offset: 0x0001B9F0
		private void RemoveMonitorFromRoute(EntityUid uid, string address, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			router.MonitorRoutes.Remove(address);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0001D80C File Offset: 0x0001BA0C
		[NullableContext(2)]
		private void PingSubnet(EntityUid uid, SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_ping"
				},
				{
					"surveillance_camera_data_subnet",
					router.SubnetName
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, null, payload, new uint?(router.SubnetFrequency), null);
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0001D868 File Offset: 0x0001BA68
		private void SendCameraInfo(EntityUid uid, NetworkPayload payload, [Nullable(2)] SurveillanceCameraRouterComponent router = null)
		{
			if (!base.Resolve<SurveillanceCameraRouterComponent>(uid, ref router, true))
			{
				return;
			}
			foreach (string address in router.MonitorRoutes)
			{
				this._deviceNetworkSystem.QueuePacket(uid, address, payload, null, null);
			}
		}

		// Token: 0x0400038E RID: 910
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x0400038F RID: 911
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04000390 RID: 912
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000391 RID: 913
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;
	}
}
