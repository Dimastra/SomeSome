using System;
using System.Collections.Generic;
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
	// Token: 0x02000146 RID: 326
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurveillanceCameraSystem : EntitySystem
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x0001DAE8 File Offset: 0x0001BCE8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SurveillanceCameraComponent, ComponentShutdown>(new ComponentEventHandler<SurveillanceCameraComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraComponent, PowerChangedEvent>(new ComponentEventRefHandler<SurveillanceCameraComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<SurveillanceCameraComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraComponent, SurveillanceCameraSetupSetName>(new ComponentEventHandler<SurveillanceCameraComponent, SurveillanceCameraSetupSetName>(this.OnSetName), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraComponent, SurveillanceCameraSetupSetNetwork>(new ComponentEventHandler<SurveillanceCameraComponent, SurveillanceCameraSetupSetNetwork>(this.OnSetNetwork), null, null);
			base.SubscribeLocalEvent<SurveillanceCameraComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SurveillanceCameraComponent, GetVerbsEvent<AlternativeVerb>>(this.AddVerbs), null, null);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0001DB70 File Offset: 0x0001BD70
		private void OnPacketReceived(EntityUid uid, SurveillanceCameraComponent component, DeviceNetworkPacketEvent args)
		{
			if (!component.Active)
			{
				return;
			}
			DeviceNetworkComponent deviceNet;
			if (!base.TryComp<DeviceNetworkComponent>(uid, ref deviceNet))
			{
				return;
			}
			string command;
			if (args.Data.TryGetValue<string>("command", out command))
			{
				NetworkPayload payload = new NetworkPayload
				{
					{
						"command",
						string.Empty
					},
					{
						"surveillance_camera_data_origin",
						deviceNet.Address
					},
					{
						"surveillance_camera_data_name",
						component.CameraId
					},
					{
						"surveillance_camera_data_subnet",
						string.Empty
					}
				};
				string dest = string.Empty;
				if (!(command == "surveillance_camera_connect"))
				{
					if (!(command == "surveillance_camera_heartbeat"))
					{
						if (command == "surveillance_camera_ping")
						{
							string subnet;
							if (!args.Data.TryGetValue<string>("surveillance_camera_data_subnet", out subnet))
							{
								return;
							}
							dest = args.SenderAddress;
							payload["surveillance_camera_data_subnet"] = subnet;
							payload["command"] = "surveillance_camera_data";
						}
					}
					else
					{
						if (!args.Data.TryGetValue<string>("surveillance_camera_data_origin", out dest) || string.IsNullOrEmpty(args.Address))
						{
							return;
						}
						payload["command"] = "surveillance_camera_heartbeat";
					}
				}
				else
				{
					if (!args.Data.TryGetValue<string>("surveillance_camera_data_origin", out dest) || string.IsNullOrEmpty(args.Address))
					{
						return;
					}
					payload["command"] = "surveillance_camera_connect";
				}
				this._deviceNetworkSystem.QueuePacket(uid, dest, payload, null, null);
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0001DCE0 File Offset: 0x0001BEE0
		private void AddVerbs(EntityUid uid, SurveillanceCameraComponent component, GetVerbsEvent<AlternativeVerb> verbs)
		{
			if (!this._actionBlocker.CanInteract(verbs.User, new EntityUid?(uid)))
			{
				return;
			}
			if (component.NameSet && component.NetworkSet)
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

		// Token: 0x0600062F RID: 1583 RVA: 0x0001DD88 File Offset: 0x0001BF88
		private void OnPowerChanged(EntityUid camera, SurveillanceCameraComponent component, ref PowerChangedEvent args)
		{
			this.SetActive(camera, args.Powered, component);
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0001DD98 File Offset: 0x0001BF98
		private void OnShutdown(EntityUid camera, SurveillanceCameraComponent component, ComponentShutdown args)
		{
			this.Deactivate(camera, component);
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0001DDA4 File Offset: 0x0001BFA4
		private void OnSetName(EntityUid uid, SurveillanceCameraComponent component, SurveillanceCameraSetupSetName args)
		{
			Enum uiKey = args.UiKey;
			if (!(uiKey is SurveillanceCameraSetupUiKey) || (SurveillanceCameraSetupUiKey)uiKey != SurveillanceCameraSetupUiKey.Camera || string.IsNullOrEmpty(args.Name) || args.Name.Length > 32)
			{
				return;
			}
			component.CameraId = args.Name;
			component.NameSet = true;
			this.UpdateSetupInterface(uid, component, null);
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0001DE04 File Offset: 0x0001C004
		private void OnSetNetwork(EntityUid uid, SurveillanceCameraComponent component, SurveillanceCameraSetupSetNetwork args)
		{
			Enum uiKey = args.UiKey;
			if (uiKey is SurveillanceCameraSetupUiKey)
			{
				SurveillanceCameraSetupUiKey key = (SurveillanceCameraSetupUiKey)uiKey;
				if (key == SurveillanceCameraSetupUiKey.Camera)
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
					this._deviceNetworkSystem.SetReceiveFrequency(uid, new uint?(frequency.Frequency), null);
					component.NetworkSet = true;
					this.UpdateSetupInterface(uid, component, null);
					return;
				}
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0001DE94 File Offset: 0x0001C094
		[NullableContext(2)]
		private void OpenSetupInterface(EntityUid uid, EntityUid player, SurveillanceCameraComponent camera = null, ActorComponent actor = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(uid, ref camera, true) || !base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			this._userInterface.GetUiOrNull(uid, SurveillanceCameraSetupUiKey.Camera, null).Open(actor.PlayerSession);
			this.UpdateSetupInterface(uid, camera, null);
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0001DEE4 File Offset: 0x0001C0E4
		[NullableContext(2)]
		private void UpdateSetupInterface(EntityUid uid, SurveillanceCameraComponent camera = null, DeviceNetworkComponent deviceNet = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent, DeviceNetworkComponent>(uid, ref camera, ref deviceNet, true))
			{
				return;
			}
			if (camera.NameSet && camera.NetworkSet)
			{
				this._userInterface.TryCloseAll(uid, SurveillanceCameraSetupUiKey.Camera, null);
				return;
			}
			if (camera.AvailableNetworks.Count == 0)
			{
				if (deviceNet.ReceiveFrequencyId != null)
				{
					camera.AvailableNetworks.Add(deviceNet.ReceiveFrequencyId);
				}
				else if (!camera.NetworkSet)
				{
					this._userInterface.TryCloseAll(uid, SurveillanceCameraSetupUiKey.Camera, null);
					return;
				}
			}
			SurveillanceCameraSetupBoundUiState state = new SurveillanceCameraSetupBoundUiState(camera.CameraId, deviceNet.ReceiveFrequency.GetValueOrDefault(), camera.AvailableNetworks, camera.NameSet, camera.NetworkSet);
			this._userInterface.TrySetUiState(uid, SurveillanceCameraSetupUiKey.Camera, state, null, null, true);
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0001DFAC File Offset: 0x0001C1AC
		[NullableContext(2)]
		private void Deactivate(EntityUid camera, SurveillanceCameraComponent component = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true))
			{
				return;
			}
			SurveillanceCameraDeactivateEvent ev = new SurveillanceCameraDeactivateEvent(camera);
			this.RemoveActiveViewers(camera, new HashSet<EntityUid>(component.ActiveViewers), null, component);
			component.Active = false;
			foreach (EntityUid monitor in component.ActiveMonitors)
			{
				base.RaiseLocalEvent<SurveillanceCameraDeactivateEvent>(monitor, ev, true);
			}
			component.ActiveMonitors.Clear();
			base.RaiseLocalEvent<SurveillanceCameraDeactivateEvent>(ev);
			this.UpdateVisuals(camera, component, null);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0001E054 File Offset: 0x0001C254
		[NullableContext(2)]
		public void SetActive(EntityUid camera, bool setting, SurveillanceCameraComponent component = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true))
			{
				return;
			}
			if (setting)
			{
				component.Active = setting;
			}
			else
			{
				this.Deactivate(camera, component);
			}
			this.UpdateVisuals(camera, component, null);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0001E080 File Offset: 0x0001C280
		[NullableContext(2)]
		public void AddActiveViewer(EntityUid camera, EntityUid player, EntityUid? monitor = null, SurveillanceCameraComponent component = null, ActorComponent actor = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true) || !component.Active || !base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			this._viewSubscriberSystem.AddViewSubscriber(camera, actor.PlayerSession);
			component.ActiveViewers.Add(player);
			if (monitor != null)
			{
				component.ActiveMonitors.Add(monitor.Value);
			}
			this.UpdateVisuals(camera, component, null);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0001E0F8 File Offset: 0x0001C2F8
		public void AddActiveViewers(EntityUid camera, HashSet<EntityUid> players, EntityUid? monitor = null, [Nullable(2)] SurveillanceCameraComponent component = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true) || !component.Active)
			{
				return;
			}
			foreach (EntityUid player in players)
			{
				this.AddActiveViewer(camera, player, monitor, component, null);
			}
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0001E164 File Offset: 0x0001C364
		[NullableContext(2)]
		public void SwitchActiveViewers(EntityUid oldCamera, EntityUid newCamera, [Nullable(1)] HashSet<EntityUid> players, EntityUid? monitor = null, SurveillanceCameraComponent oldCameraComponent = null, SurveillanceCameraComponent newCameraComponent = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(oldCamera, ref oldCameraComponent, true) || !base.Resolve<SurveillanceCameraComponent>(newCamera, ref newCameraComponent, true) || !oldCameraComponent.Active || !newCameraComponent.Active)
			{
				return;
			}
			if (monitor != null)
			{
				oldCameraComponent.ActiveMonitors.Remove(monitor.Value);
				newCameraComponent.ActiveMonitors.Add(monitor.Value);
			}
			foreach (EntityUid player in players)
			{
				this.RemoveActiveViewer(oldCamera, player, null, oldCameraComponent, null);
				this.AddActiveViewer(newCamera, player, null, newCameraComponent, null);
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0001E230 File Offset: 0x0001C430
		[NullableContext(2)]
		public void RemoveActiveViewer(EntityUid camera, EntityUid player, EntityUid? monitor = null, SurveillanceCameraComponent component = null, ActorComponent actor = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true) || !base.Resolve<ActorComponent>(player, ref actor, true))
			{
				return;
			}
			this._viewSubscriberSystem.RemoveViewSubscriber(camera, actor.PlayerSession);
			component.ActiveViewers.Remove(player);
			if (monitor != null)
			{
				component.ActiveMonitors.Remove(monitor.Value);
			}
			this.UpdateVisuals(camera, component, null);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0001E2A0 File Offset: 0x0001C4A0
		public void RemoveActiveViewers(EntityUid camera, HashSet<EntityUid> players, EntityUid? monitor = null, [Nullable(2)] SurveillanceCameraComponent component = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent>(camera, ref component, true))
			{
				return;
			}
			foreach (EntityUid player in players)
			{
				this.RemoveActiveViewer(camera, player, monitor, component, null);
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0001E300 File Offset: 0x0001C500
		[NullableContext(2)]
		private void UpdateVisuals(EntityUid uid, SurveillanceCameraComponent component = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<SurveillanceCameraComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			SurveillanceCameraVisuals key = SurveillanceCameraVisuals.Disabled;
			if (component.Active)
			{
				key = SurveillanceCameraVisuals.Active;
			}
			if (component.ActiveViewers.Count > 0 || component.ActiveMonitors.Count > 0)
			{
				key = SurveillanceCameraVisuals.InUse;
			}
			this._appearance.SetData(uid, SurveillanceCameraVisualsKey.Key, key, appearance);
		}

		// Token: 0x04000397 RID: 919
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000398 RID: 920
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04000399 RID: 921
		[Dependency]
		private readonly ViewSubscriberSystem _viewSubscriberSystem;

		// Token: 0x0400039A RID: 922
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x0400039B RID: 923
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x0400039C RID: 924
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400039D RID: 925
		public const string CameraPingSubnetMessage = "surveillance_camera_ping_subnet";

		// Token: 0x0400039E RID: 926
		public const string CameraPingMessage = "surveillance_camera_ping";

		// Token: 0x0400039F RID: 927
		public const string CameraHeartbeatMessage = "surveillance_camera_heartbeat";

		// Token: 0x040003A0 RID: 928
		public const string CameraDataMessage = "surveillance_camera_data";

		// Token: 0x040003A1 RID: 929
		public const string CameraConnectMessage = "surveillance_camera_connect";

		// Token: 0x040003A2 RID: 930
		public const string CameraSubnetConnectMessage = "surveillance_camera_subnet_connect";

		// Token: 0x040003A3 RID: 931
		public const string CameraSubnetDisconnectMessage = "surveillance_camera_subnet_disconnect";

		// Token: 0x040003A4 RID: 932
		public const string CameraAddressData = "surveillance_camera_data_origin";

		// Token: 0x040003A5 RID: 933
		public const string CameraNameData = "surveillance_camera_data_name";

		// Token: 0x040003A6 RID: 934
		public const string CameraSubnetData = "surveillance_camera_data_subnet";

		// Token: 0x040003A7 RID: 935
		public const int CameraNameLimit = 32;
	}
}
