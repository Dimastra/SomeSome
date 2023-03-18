using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Database;
using Content.Shared.DeviceNetwork;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000587 RID: 1415
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkConfiguratorSystem : SharedNetworkConfiguratorSystem
	{
		// Token: 0x06001DA3 RID: 7587 RVA: 0x0009DCB0 File Offset: 0x0009BEB0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, MapInitEvent>(new ComponentEventHandler<NetworkConfiguratorComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, AfterInteractEvent>(delegate(EntityUid uid, NetworkConfiguratorComponent component, AfterInteractEvent args)
			{
				this.OnUsed(uid, component, args.Target, args.User, args.CanReach);
			}, null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<NetworkConfiguratorComponent, GetVerbsEvent<UtilityVerb>>(this.OnAddInteractVerb), null, null);
			base.SubscribeLocalEvent<DeviceNetworkComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DeviceNetworkComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAddAlternativeSaveDeviceVerb), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, BoundUIClosedEvent>(new ComponentEventHandler<NetworkConfiguratorComponent, BoundUIClosedEvent>(this.OnUiClosed), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, NetworkConfiguratorRemoveDeviceMessage>(new ComponentEventHandler<NetworkConfiguratorComponent, NetworkConfiguratorRemoveDeviceMessage>(this.OnRemoveDevice), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, NetworkConfiguratorClearDevicesMessage>(new ComponentEventHandler<NetworkConfiguratorComponent, NetworkConfiguratorClearDevicesMessage>(this.OnClearDevice), null, null);
			base.SubscribeLocalEvent<NetworkConfiguratorComponent, NetworkConfiguratorButtonPressedMessage>(new ComponentEventHandler<NetworkConfiguratorComponent, NetworkConfiguratorButtonPressedMessage>(this.OnConfigButtonPressed), null, null);
			base.SubscribeLocalEvent<DeviceListComponent, ComponentRemove>(new ComponentEventHandler<DeviceListComponent, ComponentRemove>(this.OnComponentRemoved), null, null);
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x0009DD78 File Offset: 0x0009BF78
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (NetworkConfiguratorComponent component in this.EntityManager.EntityQuery<NetworkConfiguratorComponent>(false))
			{
				EntityUid uid = component.Owner;
				if (component.ActiveDeviceList == null || !this.EntityManager.EntityExists(component.ActiveDeviceList.Value) || !this._interactionSystem.InRangeUnobstructed(uid, component.ActiveDeviceList.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
				{
					BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, NetworkConfiguratorUiKey.Configure, null);
					if (uiOrNull != null)
					{
						uiOrNull.CloseAll();
					}
				}
			}
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x0009DE3C File Offset: 0x0009C03C
		private void OnMapInit(EntityUid uid, NetworkConfiguratorComponent component, MapInitEvent args)
		{
			component.Devices.Clear();
			this.UpdateUiState(uid, component);
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0009DE51 File Offset: 0x0009C051
		[NullableContext(2)]
		private void TryAddNetworkDevice(EntityUid? targetUid, EntityUid configuratorUid, EntityUid userUid, NetworkConfiguratorComponent configurator = null)
		{
			if (!base.Resolve<NetworkConfiguratorComponent>(configuratorUid, ref configurator, true))
			{
				return;
			}
			this.TryAddNetworkDevice(targetUid, userUid, configurator, null);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0009DE6C File Offset: 0x0009C06C
		private void TryAddNetworkDevice(EntityUid? targetUid, EntityUid userUid, NetworkConfiguratorComponent configurator, [Nullable(2)] DeviceNetworkComponent device = null)
		{
			if (targetUid == null || !base.Resolve<DeviceNetworkComponent>(targetUid.Value, ref device, false))
			{
				return;
			}
			string address = device.Address;
			if (string.IsNullOrEmpty(address))
			{
				if (base.MetaData(targetUid.Value).EntityLifeStage == 3)
				{
					this._popupSystem.PopupCursor(Loc.GetString("network-configurator-device-failed", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("device", targetUid)
					}), userUid, PopupType.Small);
					return;
				}
				address = "UID: " + targetUid.Value.ToString();
			}
			if (configurator.Devices.ContainsValue(targetUid.Value))
			{
				this._popupSystem.PopupCursor(Loc.GetString("network-configurator-device-already-saved", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("device", targetUid)
				}), userUid, PopupType.Small);
				return;
			}
			configurator.Devices.Add(address, targetUid.Value);
			this._popupSystem.PopupCursor(Loc.GetString("network-configurator-device-saved", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("address", device.Address),
				new ValueTuple<string, object>("device", targetUid)
			}), userUid, PopupType.Medium);
			this.UpdateUiState(configurator.Owner, configurator);
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x0009DFC4 File Offset: 0x0009C1C4
		private bool AccessCheck(EntityUid target, EntityUid? user, NetworkConfiguratorComponent component)
		{
			AccessReaderComponent reader;
			if (!base.TryComp<AccessReaderComponent>(target, ref reader) || user == null)
			{
				return false;
			}
			if (this._accessSystem.IsAllowed(user.Value, reader))
			{
				return true;
			}
			SoundSystem.Play(component.SoundNoAccess.GetSound(null, null), Filter.Pvs(user.Value, 2f, null, null, null), target, new AudioParams?(AudioParams.Default.WithVolume(-2f).WithPitchScale(1.2f)));
			this._popupSystem.PopupEntity(Loc.GetString("network-configurator-device-access-denied"), target, user.Value, PopupType.Small);
			return false;
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0009E066 File Offset: 0x0009C266
		private void OnComponentRemoved(EntityUid uid, DeviceListComponent component, ComponentRemove args)
		{
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, NetworkConfiguratorUiKey.Configure, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.CloseAll();
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x0009E085 File Offset: 0x0009C285
		private void OnUsed(EntityUid uid, NetworkConfiguratorComponent component, EntityUid? target, EntityUid user, bool canReach = true)
		{
			if (!canReach)
			{
				return;
			}
			if (!base.HasComp<DeviceListComponent>(target))
			{
				this.TryAddNetworkDevice(target, user, component, null);
				return;
			}
			this.OpenDeviceListUi(target, user, component);
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x0009E0AC File Offset: 0x0009C2AC
		private void OnAddInteractVerb(EntityUid uid, NetworkConfiguratorComponent component, GetVerbsEvent<UtilityVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Using == null || !base.HasComp<DeviceNetworkComponent>(args.Target))
			{
				return;
			}
			bool isDeviceList = base.HasComp<DeviceListComponent>(args.Target);
			UtilityVerb verb = new UtilityVerb
			{
				Text = Loc.GetString(isDeviceList ? "network-configurator-configure" : "network-configurator-save-device"),
				Icon = (isDeviceList ? new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png", "/")) : new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/in.svg.192dpi.png", "/"))),
				Act = delegate()
				{
					this.OnUsed(uid, component, new EntityUid?(args.Target), args.User, true);
				},
				Impact = LogImpact.Low
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x0009E1AC File Offset: 0x0009C3AC
		private void OnAddAlternativeSaveDeviceVerb(EntityUid uid, DeviceNetworkComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Using == null || !base.HasComp<NetworkConfiguratorComponent>(args.Using.Value) || !base.HasComp<DeviceListComponent>(args.Target))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = Loc.GetString("network-configurator-save-device"),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/in.svg.192dpi.png", "/")),
				Act = delegate()
				{
					this.TryAddNetworkDevice(new EntityUid?(args.Target), args.Using.Value, args.User, null);
				},
				Impact = LogImpact.Low
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0009E280 File Offset: 0x0009C480
		private void OpenDeviceListUi(EntityUid? targetUid, EntityUid userUid, NetworkConfiguratorComponent configurator)
		{
			ActorComponent actor;
			if (targetUid == null || !base.TryComp<ActorComponent>(userUid, ref actor) || !this.AccessCheck(targetUid.Value, new EntityUid?(userUid), configurator))
			{
				return;
			}
			configurator.ActiveDeviceList = targetUid;
			base.Dirty(configurator, null);
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(configurator.Owner, NetworkConfiguratorUiKey.Configure, null);
			if (uiOrNull != null)
			{
				uiOrNull.Open(actor.PlayerSession);
			}
			this._uiSystem.TrySetUiState(configurator.Owner, NetworkConfiguratorUiKey.Configure, new DeviceListUserInterfaceState((from v in this._deviceListSystem.GetDeviceList(configurator.ActiveDeviceList.Value, null)
			select new ValueTuple<string, string>(v.Key, base.MetaData(v.Value).EntityName)).ToHashSet<ValueTuple<string, string>>()), null, null, true);
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x0009E340 File Offset: 0x0009C540
		private void UpdateUiState(EntityUid uid, NetworkConfiguratorComponent component)
		{
			HashSet<ValueTuple<string, string>> devices = new HashSet<ValueTuple<string, string>>();
			HashSet<string> invalidDevices = new HashSet<string>();
			foreach (KeyValuePair<string, EntityUid> pair in component.Devices)
			{
				if (!base.Exists(pair.Value))
				{
					invalidDevices.Add(pair.Key);
				}
				else
				{
					devices.Add(new ValueTuple<string, string>(pair.Key, base.Name(pair.Value, null)));
				}
			}
			foreach (string invalidDevice in invalidDevices)
			{
				component.Devices.Remove(invalidDevice);
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, NetworkConfiguratorUiKey.List, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(new NetworkConfiguratorUserInterfaceState(devices), null, true);
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x0009E444 File Offset: 0x0009C644
		private void OnUiClosed(EntityUid uid, NetworkConfiguratorComponent component, BoundUIClosedEvent args)
		{
			component.ActiveDeviceList = null;
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x0009E452 File Offset: 0x0009C652
		private void OnRemoveDevice(EntityUid uid, NetworkConfiguratorComponent component, NetworkConfiguratorRemoveDeviceMessage args)
		{
			component.Devices.Remove(args.Address);
			this.UpdateUiState(uid, component);
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x0009E46E File Offset: 0x0009C66E
		private void OnClearDevice(EntityUid uid, NetworkConfiguratorComponent component, NetworkConfiguratorClearDevicesMessage _)
		{
			component.Devices.Clear();
			this.UpdateUiState(uid, component);
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x0009E484 File Offset: 0x0009C684
		private void OnConfigButtonPressed(EntityUid uid, NetworkConfiguratorComponent component, NetworkConfiguratorButtonPressedMessage args)
		{
			if (component.ActiveDeviceList == null)
			{
				return;
			}
			DeviceListUpdateResult result = DeviceListUpdateResult.NoComponent;
			switch (args.ButtonKey)
			{
			case NetworkConfiguratorButtonKey.Set:
				result = this._deviceListSystem.UpdateDeviceList(component.ActiveDeviceList.Value, new HashSet<EntityUid>(component.Devices.Values), false, null);
				break;
			case NetworkConfiguratorButtonKey.Add:
				result = this._deviceListSystem.UpdateDeviceList(component.ActiveDeviceList.Value, new HashSet<EntityUid>(component.Devices.Values), true, null);
				break;
			case NetworkConfiguratorButtonKey.Clear:
				result = this._deviceListSystem.UpdateDeviceList(component.ActiveDeviceList.Value, new HashSet<EntityUid>(), false, null);
				break;
			case NetworkConfiguratorButtonKey.Copy:
				component.Devices = this._deviceListSystem.GetDeviceList(component.ActiveDeviceList.Value, null);
				this.UpdateUiState(uid, component);
				return;
			}
			string text;
			if (result != DeviceListUpdateResult.TooManyDevices)
			{
				if (result != DeviceListUpdateResult.UpdateOk)
				{
					text = "error";
				}
				else
				{
					text = Loc.GetString("network-configurator-update-ok");
				}
			}
			else
			{
				text = Loc.GetString("network-configurator-too-many-devices");
			}
			string resultText = text;
			this._popupSystem.PopupCursor(Loc.GetString(resultText), args.Session, PopupType.Medium);
			this._uiSystem.TrySetUiState(component.Owner, NetworkConfiguratorUiKey.Configure, new DeviceListUserInterfaceState((from v in this._deviceListSystem.GetDeviceList(component.ActiveDeviceList.Value, null)
			select new ValueTuple<string, string>(v.Key, base.MetaData(v.Value).EntityName)).ToHashSet<ValueTuple<string, string>>()), null, null, true);
		}

		// Token: 0x04001304 RID: 4868
		[Dependency]
		private readonly DeviceListSystem _deviceListSystem;

		// Token: 0x04001305 RID: 4869
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04001306 RID: 4870
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04001307 RID: 4871
		[Dependency]
		private readonly AccessReaderSystem _accessSystem;

		// Token: 0x04001308 RID: 4872
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;
	}
}
