using System;
using System.Runtime.CompilerServices;
using Content.Shared.DeviceNetwork;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x0200021E RID: 542
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkConfiguratorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000E19 RID: 3609 RVA: 0x000556A0 File Offset: 0x000538A0
		public NetworkConfiguratorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<NetworkConfiguratorBoundUserInterface>(this);
			this._netConfig = this._entityManager.System<NetworkConfiguratorSystem>();
			this._deviceList = this._entityManager.System<DeviceListSystem>();
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x000556D3 File Offset: 0x000538D3
		public void OnRemoveButtonPressed(string address)
		{
			base.SendMessage(new NetworkConfiguratorRemoveDeviceMessage(address));
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x000556E4 File Offset: 0x000538E4
		protected override void Open()
		{
			base.Open();
			Enum uiKey = this.UiKey;
			if (uiKey is NetworkConfiguratorUiKey)
			{
				NetworkConfiguratorUiKey networkConfiguratorUiKey = (NetworkConfiguratorUiKey)uiKey;
				if (networkConfiguratorUiKey == NetworkConfiguratorUiKey.List)
				{
					this._listMenu = new NetworkConfiguratorListMenu(this);
					this._listMenu.OnClose += base.Close;
					this._listMenu.ClearButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
					{
						this.OnClearButtonPressed();
					};
					this._listMenu.OpenCentered();
					return;
				}
				if (networkConfiguratorUiKey != NetworkConfiguratorUiKey.Configure)
				{
					return;
				}
				this._configurationMenu = new NetworkConfiguratorConfigurationMenu();
				this._configurationMenu.OnClose += base.Close;
				this._configurationMenu.Set.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Set);
				};
				this._configurationMenu.Add.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Add);
				};
				this._configurationMenu.Clear.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Clear);
				};
				this._configurationMenu.Copy.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					this.OnConfigButtonPressed(NetworkConfiguratorButtonKey.Copy);
				};
				this._configurationMenu.Show.OnPressed += this.OnShowPressed;
				this._configurationMenu.Show.Pressed = this._netConfig.ConfiguredListIsTracked(base.Owner.Owner, null);
				this._configurationMenu.OpenCentered();
			}
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x00055843 File Offset: 0x00053A43
		private void OnShowPressed(BaseButton.ButtonEventArgs args)
		{
			this._netConfig.ToggleVisualization(base.Owner.Owner, args.Button.Pressed, null);
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x00055868 File Offset: 0x00053A68
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			NetworkConfiguratorUserInterfaceState networkConfiguratorUserInterfaceState = state as NetworkConfiguratorUserInterfaceState;
			if (networkConfiguratorUserInterfaceState == null)
			{
				DeviceListUserInterfaceState deviceListUserInterfaceState = state as DeviceListUserInterfaceState;
				if (deviceListUserInterfaceState == null)
				{
					return;
				}
				NetworkConfiguratorConfigurationMenu configurationMenu = this._configurationMenu;
				if (configurationMenu == null)
				{
					return;
				}
				configurationMenu.UpdateState(deviceListUserInterfaceState);
				return;
			}
			else
			{
				NetworkConfiguratorListMenu listMenu = this._listMenu;
				if (listMenu == null)
				{
					return;
				}
				listMenu.UpdateState(networkConfiguratorUserInterfaceState);
				return;
			}
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000558B4 File Offset: 0x00053AB4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			NetworkConfiguratorListMenu listMenu = this._listMenu;
			if (listMenu != null)
			{
				listMenu.Dispose();
			}
			NetworkConfiguratorConfigurationMenu configurationMenu = this._configurationMenu;
			if (configurationMenu == null)
			{
				return;
			}
			configurationMenu.Dispose();
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000558E2 File Offset: 0x00053AE2
		private void OnClearButtonPressed()
		{
			base.SendMessage(new NetworkConfiguratorClearDevicesMessage());
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x000558EF File Offset: 0x00053AEF
		private void OnConfigButtonPressed(NetworkConfiguratorButtonKey buttonKey)
		{
			base.SendMessage(new NetworkConfiguratorButtonPressedMessage(buttonKey));
		}

		// Token: 0x04000700 RID: 1792
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000701 RID: 1793
		[Nullable(2)]
		private NetworkConfiguratorListMenu _listMenu;

		// Token: 0x04000702 RID: 1794
		[Nullable(2)]
		private NetworkConfiguratorConfigurationMenu _configurationMenu;

		// Token: 0x04000703 RID: 1795
		private NetworkConfiguratorSystem _netConfig;

		// Token: 0x04000704 RID: 1796
		private DeviceListSystem _deviceList;
	}
}
