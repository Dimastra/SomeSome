using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Power.Components;
using Content.Server.UserInterface;
using Content.Shared.SurveillanceCamera;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000143 RID: 323
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraMonitorSystem : EntitySystem
	{
		// Token: 0x060005FA RID: 1530 RVA: 0x0001C7FC File Offset: 0x0001A9FC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraDeactivateEvent>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraDeactivateEvent>(this.OnSurveillanceCameraDeactivate), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, BoundUIClosedEvent>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, BoundUIClosedEvent>(this.OnBoundUiClose), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, PowerChangedEvent>(new ComponentEventRefHandler<SurveillanceCameraMonitorComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraMonitorSwitchMessage>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraMonitorSwitchMessage>(this.OnSwitchMessage), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraMonitorSubnetRequestMessage>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraMonitorSubnetRequestMessage>(this.OnSubnetRequest), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, ComponentStartup>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, ComponentStartup>(this.OnComponentStartup), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, AfterActivatableUIOpenEvent>(this.OnToggleInterface), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraRefreshCamerasMessage>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraRefreshCamerasMessage>(this.OnRefreshCamerasMessage), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraRefreshSubnetsMessage>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraRefreshSubnetsMessage>(this.OnRefreshSubnetsMessage), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraMonitorComponent, SurveillanceCameraDisconnectMessage>(new ComponentEventHandler<SurveillanceCameraMonitorComponent, SurveillanceCameraDisconnectMessage>(this.OnDisconnectMessage), null, null);
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0001C8E8 File Offset: 0x0001AAE8
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<ActiveSurveillanceCameraMonitorComponent, SurveillanceCameraMonitorComponent> valueTuple in base.EntityQuery<ActiveSurveillanceCameraMonitorComponent, SurveillanceCameraMonitorComponent>(false))
			{
				SurveillanceCameraMonitorComponent monitor = valueTuple.Item2;
				if (!base.Paused(monitor.Owner, null))
				{
					monitor.LastHeartbeatSent += frameTime;
					this.SendHeartbeat(monitor.Owner, monitor);
					monitor.LastHeartbeat += frameTime;
					if (monitor.LastHeartbeat > 300f)
					{
						this.DisconnectCamera(monitor.Owner, true, monitor);
						this.EntityManager.RemoveComponent<ActiveSurveillanceCameraMonitorComponent>(monitor.Owner);
					}
				}
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0001C99C File Offset: 0x0001AB9C
		private void OnComponentStartup(EntityUid uid, SurveillanceCameraMonitorComponent component, ComponentStartup args)
		{
			this.RefreshSubnets(uid, component);
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0001C9A8 File Offset: 0x0001ABA8
		private void OnSubnetRequest(EntityUid uid, SurveillanceCameraMonitorComponent component, SurveillanceCameraMonitorSubnetRequestMessage args)
		{
			if (args.Session.AttachedEntity != null)
			{
				this.SetActiveSubnet(uid, args.Subnet, component);
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0001C9D8 File Offset: 0x0001ABD8
		private void OnPacketReceived(EntityUid uid, SurveillanceCameraMonitorComponent component, DeviceNetworkPacketEvent args)
		{
			if (string.IsNullOrEmpty(args.SenderAddress))
			{
				return;
			}
			string command;
			if (args.Data.TryGetValue<string>("command", out command))
			{
				if (command == "surveillance_camera_connect")
				{
					if (component.NextCameraAddress == args.SenderAddress)
					{
						component.ActiveCameraAddress = args.SenderAddress;
						this.TrySwitchCameraByUid(uid, args.Sender, component);
					}
					component.NextCameraAddress = null;
					return;
				}
				if (!(command == "surveillance_camera_heartbeat"))
				{
					if (!(command == "surveillance_camera_data"))
					{
						if (!(command == "surveillance_camera_data_subnet"))
						{
							return;
						}
						string subnet;
						if (args.Data.TryGetValue<string>("surveillance_camera_data_subnet", out subnet) && !component.KnownSubnets.ContainsKey(subnet))
						{
							component.KnownSubnets.Add(subnet, args.SenderAddress);
						}
						this.UpdateUserInterface(uid, component, null);
					}
					else
					{
						string name;
						string subnetData;
						string address;
						if (!args.Data.TryGetValue<string>("surveillance_camera_data_name", out name) || !args.Data.TryGetValue<string>("surveillance_camera_data_subnet", out subnetData) || !args.Data.TryGetValue<string>("surveillance_camera_data_origin", out address))
						{
							return;
						}
						if (component.ActiveSubnet != subnetData)
						{
							this.DisconnectFromSubnet(uid, subnetData, null);
						}
						if (!component.KnownCameras.ContainsKey(address))
						{
							component.KnownCameras.Add(address, name);
						}
						this.UpdateUserInterface(uid, component, null);
						return;
					}
				}
				else if (args.SenderAddress == component.ActiveCameraAddress)
				{
					component.LastHeartbeat = 0f;
					return;
				}
			}
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0001CB63 File Offset: 0x0001AD63
		private void OnDisconnectMessage(EntityUid uid, SurveillanceCameraMonitorComponent component, SurveillanceCameraDisconnectMessage message)
		{
			this.DisconnectCamera(uid, true, component);
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0001CB6E File Offset: 0x0001AD6E
		private void OnRefreshCamerasMessage(EntityUid uid, SurveillanceCameraMonitorComponent component, SurveillanceCameraRefreshCamerasMessage message)
		{
			component.KnownCameras.Clear();
			this.RequestActiveSubnetInfo(uid, component);
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x0001CB83 File Offset: 0x0001AD83
		private void OnRefreshSubnetsMessage(EntityUid uid, SurveillanceCameraMonitorComponent component, SurveillanceCameraRefreshSubnetsMessage message)
		{
			this.RefreshSubnets(uid, component);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0001CB8D File Offset: 0x0001AD8D
		private void OnSwitchMessage(EntityUid uid, SurveillanceCameraMonitorComponent component, SurveillanceCameraMonitorSwitchMessage message)
		{
			this.TrySwitchCameraByAddress(uid, message.Address, component);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0001CB9D File Offset: 0x0001AD9D
		private void OnPowerChanged(EntityUid uid, SurveillanceCameraMonitorComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				this.RemoveActiveCamera(uid, component);
				component.NextCameraAddress = null;
				component.ActiveSubnet = string.Empty;
			}
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0001CBC1 File Offset: 0x0001ADC1
		private void OnToggleInterface(EntityUid uid, SurveillanceCameraMonitorComponent component, AfterActivatableUIOpenEvent args)
		{
			this.AfterOpenUserInterface(uid, args.User, component, null);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0001CBD2 File Offset: 0x0001ADD2
		private void OnSurveillanceCameraDeactivate(EntityUid uid, SurveillanceCameraMonitorComponent monitor, SurveillanceCameraDeactivateEvent args)
		{
			this.DisconnectCamera(uid, false, monitor);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0001CBE0 File Offset: 0x0001ADE0
		private void OnBoundUiClose(EntityUid uid, SurveillanceCameraMonitorComponent component, BoundUIClosedEvent args)
		{
			if (args.Session.AttachedEntity == null)
			{
				return;
			}
			this.RemoveViewer(uid, args.Session.AttachedEntity.Value, component);
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0001CC20 File Offset: 0x0001AE20
		[NullableContext(2)]
		private void SendHeartbeat(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null)
		{
			string subnetAddress;
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || monitor.LastHeartbeatSent < 30f || !monitor.KnownSubnets.TryGetValue(monitor.ActiveSubnet, out subnetAddress))
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
					monitor.ActiveCameraAddress
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, subnetAddress, payload, null, null);
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0001CC9C File Offset: 0x0001AE9C
		[NullableContext(2)]
		private void DisconnectCamera(EntityUid uid, bool removeViewers, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			if (removeViewers)
			{
				this.RemoveActiveCamera(uid, monitor);
			}
			monitor.ActiveCamera = null;
			monitor.ActiveCameraAddress = string.Empty;
			this.EntityManager.RemoveComponent<ActiveSurveillanceCameraMonitorComponent>(uid);
			this.UpdateUserInterface(uid, monitor, null);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0001CCF9 File Offset: 0x0001AEF9
		[NullableContext(2)]
		private void RefreshSubnets(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			monitor.KnownSubnets.Clear();
			this.PingCameraNetwork(uid, monitor);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x0001CD1C File Offset: 0x0001AF1C
		[NullableContext(2)]
		private void PingCameraNetwork(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_ping"
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, null, payload, null, null);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0001CD68 File Offset: 0x0001AF68
		private void SetActiveSubnet(EntityUid uid, string subnet, [Nullable(2)] SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !monitor.KnownSubnets.ContainsKey(subnet))
			{
				return;
			}
			this.DisconnectFromSubnet(uid, monitor.ActiveSubnet, null);
			this.DisconnectCamera(uid, true, monitor);
			monitor.ActiveSubnet = subnet;
			monitor.KnownCameras.Clear();
			this.UpdateUserInterface(uid, monitor, null);
			this.ConnectToSubnet(uid, subnet, null);
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x0001CDD4 File Offset: 0x0001AFD4
		[NullableContext(2)]
		private void RequestActiveSubnetInfo(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null)
		{
			string address;
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !monitor.KnownSubnets.TryGetValue(monitor.ActiveSubnet, out address))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_ping_subnet"
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, address, payload, null, null);
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x0001CE34 File Offset: 0x0001B034
		private void ConnectToSubnet(EntityUid uid, string subnet, [Nullable(2)] SurveillanceCameraMonitorComponent monitor = null)
		{
			string address;
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !monitor.KnownSubnets.TryGetValue(subnet, out address))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_subnet_connect"
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, address, payload, null, null);
			this.RequestActiveSubnetInfo(uid, null);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0001CE98 File Offset: 0x0001B098
		private void DisconnectFromSubnet(EntityUid uid, string subnet, [Nullable(2)] SurveillanceCameraMonitorComponent monitor = null)
		{
			string address;
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !monitor.KnownSubnets.TryGetValue(subnet, out address))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"surveillance_camera_subnet_disconnect"
				}
			};
			this._deviceNetworkSystem.QueuePacket(uid, address, payload, null, null);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0001CEF4 File Offset: 0x0001B0F4
		[NullableContext(2)]
		private void AddViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			monitor.Viewers.Add(player);
			if (monitor.ActiveCamera != null)
			{
				this._surveillanceCameras.AddActiveViewer(monitor.ActiveCamera.Value, player, new EntityUid?(uid), null, null);
			}
			this.UpdateUserInterface(uid, monitor, new EntityUid?(player));
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0001CF5C File Offset: 0x0001B15C
		[NullableContext(2)]
		private void RemoveViewer(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			monitor.Viewers.Remove(player);
			if (monitor.ActiveCamera != null)
			{
				this._surveillanceCameras.RemoveActiveViewer(monitor.ActiveCamera.Value, player, null, null, null);
			}
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0001CFB8 File Offset: 0x0001B1B8
		[NullableContext(2)]
		private void SetCamera(EntityUid uid, EntityUid camera, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || monitor.ActiveCamera != null)
			{
				return;
			}
			this._surveillanceCameras.AddActiveViewers(camera, monitor.Viewers, new EntityUid?(uid), null);
			monitor.ActiveCamera = new EntityUid?(camera);
			base.AddComp<ActiveSurveillanceCameraMonitorComponent>(uid);
			this.UpdateUserInterface(uid, monitor, null);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x0001D020 File Offset: 0x0001B220
		[NullableContext(2)]
		private void SwitchCamera(EntityUid uid, EntityUid camera, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || monitor.ActiveCamera == null)
			{
				return;
			}
			this._surveillanceCameras.SwitchActiveViewers(monitor.ActiveCamera.Value, camera, monitor.Viewers, new EntityUid?(uid), null, null);
			monitor.ActiveCamera = new EntityUid?(camera);
			this.UpdateUserInterface(uid, monitor, null);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0001D090 File Offset: 0x0001B290
		private void TrySwitchCameraByAddress(EntityUid uid, string address, [Nullable(2)] SurveillanceCameraMonitorComponent monitor = null)
		{
			string subnetAddress;
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !monitor.KnownSubnets.TryGetValue(monitor.ActiveSubnet, out subnetAddress))
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
					address
				}
			};
			monitor.NextCameraAddress = address;
			this._deviceNetworkSystem.QueuePacket(uid, subnetAddress, payload, null, null);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x0001D104 File Offset: 0x0001B304
		[NullableContext(2)]
		private void TrySwitchCameraByUid(EntityUid uid, EntityUid newCamera, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			if (monitor.ActiveCamera == null)
			{
				this.SetCamera(uid, newCamera, monitor);
				return;
			}
			this.SwitchCamera(uid, newCamera, monitor);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0001D144 File Offset: 0x0001B344
		[NullableContext(2)]
		private void RemoveActiveCamera(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || monitor.ActiveCamera == null)
			{
				return;
			}
			this._surveillanceCameras.RemoveActiveViewers(monitor.ActiveCamera.Value, monitor.Viewers, new EntityUid?(uid), null);
			this.UpdateUserInterface(uid, monitor, null);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0001D1A5 File Offset: 0x0001B3A5
		[NullableContext(2)]
		public void AfterOpenUserInterface(EntityUid uid, EntityUid player, SurveillanceCameraMonitorComponent monitor = null, ActorComponent actor = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true) || !base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			this.AddViewer(uid, player, null);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0001D1CC File Offset: 0x0001B3CC
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, SurveillanceCameraMonitorComponent monitor = null, EntityUid? player = null)
		{
			if (!base.Resolve<SurveillanceCameraMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			ActorComponent actor;
			if (player != null && base.TryComp<ActorComponent>(player, ref actor))
			{
				IPlayerSession playerSession = actor.PlayerSession;
			}
			SurveillanceCameraMonitorUiState state = new SurveillanceCameraMonitorUiState(monitor.ActiveCamera, monitor.KnownSubnets.Keys.ToHashSet<string>(), monitor.ActiveCameraAddress, monitor.ActiveSubnet, monitor.KnownCameras);
			this._userInterface.TrySetUiState(uid, SurveillanceCameraMonitorUiKey.Key, state, null, null, true);
		}

		// Token: 0x04000389 RID: 905
		[Dependency]
		private readonly SurveillanceCameraSystem _surveillanceCameras;

		// Token: 0x0400038A RID: 906
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x0400038B RID: 907
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x0400038C RID: 908
		private const float _maxHeartbeatTime = 300f;

		// Token: 0x0400038D RID: 909
		private const float _heartbeatDelay = 30f;
	}
}
