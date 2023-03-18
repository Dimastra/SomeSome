using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.DeviceNetwork;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.NetworkConfigurator
{
	// Token: 0x02000225 RID: 549
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetworkConfiguratorSystem : SharedNetworkConfiguratorSystem
	{
		// Token: 0x06000E45 RID: 3653 RVA: 0x000566F7 File Offset: 0x000548F7
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ClearAllOverlaysEvent>(delegate(ClearAllOverlaysEvent _)
			{
				this.ClearAllOverlays();
			}, null, null);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x00056713 File Offset: 0x00054913
		[NullableContext(2)]
		public bool ConfiguredListIsTracked(EntityUid uid, NetworkConfiguratorComponent component = null)
		{
			return base.Resolve<NetworkConfiguratorComponent>(uid, ref component, true) && component.ActiveDeviceList != null && base.HasComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00056744 File Offset: 0x00054944
		[NullableContext(2)]
		public void ToggleVisualization(EntityUid uid, bool toggle, NetworkConfiguratorComponent component = null)
		{
			if (this._playerManager.LocalPlayer == null || this._playerManager.LocalPlayer.ControlledEntity == null || !base.Resolve<NetworkConfiguratorComponent>(uid, ref component, true) || component.ActiveDeviceList == null)
			{
				return;
			}
			if (!toggle)
			{
				if (this._overlay.HasOverlay<NetworkConfiguratorLinkOverlay>())
				{
					this._overlay.GetOverlay<NetworkConfiguratorLinkOverlay>().ClearEntity(component.ActiveDeviceList.Value);
				}
				base.RemComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
				if (!base.EntityQuery<NetworkConfiguratorActiveLinkOverlayComponent>(false).Any<NetworkConfiguratorActiveLinkOverlayComponent>())
				{
					this._overlay.RemoveOverlay<NetworkConfiguratorLinkOverlay>();
					this._actions.RemoveAction(this._playerManager.LocalPlayer.ControlledEntity.Value, this._prototypeManager.Index<InstantActionPrototype>("ClearNetworkLinkOverlays"), null);
				}
				return;
			}
			if (!this._overlay.HasOverlay<NetworkConfiguratorLinkOverlay>())
			{
				this._overlay.AddOverlay(new NetworkConfiguratorLinkOverlay());
				this._actions.AddAction(this._playerManager.LocalPlayer.ControlledEntity.Value, new InstantAction(this._prototypeManager.Index<InstantActionPrototype>("ClearNetworkLinkOverlays")), null, null, true);
			}
			base.EnsureComp<NetworkConfiguratorActiveLinkOverlayComponent>(component.ActiveDeviceList.Value);
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00056894 File Offset: 0x00054A94
		public void ClearAllOverlays()
		{
			if (!this._overlay.HasOverlay<NetworkConfiguratorLinkOverlay>())
			{
				return;
			}
			foreach (NetworkConfiguratorActiveLinkOverlayComponent networkConfiguratorActiveLinkOverlayComponent in base.EntityQuery<NetworkConfiguratorActiveLinkOverlayComponent>(false))
			{
				base.RemCompDeferred<NetworkConfiguratorActiveLinkOverlayComponent>(networkConfiguratorActiveLinkOverlayComponent.Owner);
			}
			this._overlay.RemoveOverlay<NetworkConfiguratorLinkOverlay>();
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer != null && localPlayer.ControlledEntity != null)
			{
				this._actions.RemoveAction(this._playerManager.LocalPlayer.ControlledEntity.Value, this._prototypeManager.Index<InstantActionPrototype>("ClearNetworkLinkOverlays"), null);
			}
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x00056958 File Offset: 0x00054B58
		[NullableContext(2)]
		public void SetActiveDeviceList(EntityUid tool, EntityUid list, NetworkConfiguratorComponent component = null)
		{
			if (!base.Resolve<NetworkConfiguratorComponent>(tool, ref component, true))
			{
				return;
			}
			component.ActiveDeviceList = new EntityUid?(list);
		}

		// Token: 0x0400070C RID: 1804
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400070D RID: 1805
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x0400070E RID: 1806
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400070F RID: 1807
		[Dependency]
		private readonly ActionsSystem _actions;

		// Token: 0x04000710 RID: 1808
		private const string Action = "ClearNetworkLinkOverlays";
	}
}
