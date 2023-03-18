using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Client.Alerts
{
	// Token: 0x0200047C RID: 1148
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ClientAlertsSystem : AlertsSystem
	{
		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06001C53 RID: 7251 RVA: 0x000A477A File Offset: 0x000A297A
		// (set) Token: 0x06001C54 RID: 7252 RVA: 0x000A4782 File Offset: 0x000A2982
		public AlertOrderPrototype AlertOrder { get; set; }

		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06001C55 RID: 7253 RVA: 0x000A478C File Offset: 0x000A298C
		// (remove) Token: 0x06001C56 RID: 7254 RVA: 0x000A47C4 File Offset: 0x000A29C4
		public event EventHandler ClearAlerts;

		// Token: 0x140000B2 RID: 178
		// (add) Token: 0x06001C57 RID: 7255 RVA: 0x000A47FC File Offset: 0x000A29FC
		// (remove) Token: 0x06001C58 RID: 7256 RVA: 0x000A4834 File Offset: 0x000A2A34
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> SyncAlerts;

		// Token: 0x06001C59 RID: 7257 RVA: 0x000A486C File Offset: 0x000A2A6C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AlertsComponent, PlayerAttachedEvent>(new ComponentEventHandler<AlertsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<AlertsComponent, PlayerDetachedEvent>(new ComponentEventHandler<AlertsComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<AlertsComponent, ComponentHandleState>(new ComponentEventRefHandler<AlertsComponent, ComponentHandleState>(this.ClientAlertsHandleState), null, null);
		}

		// Token: 0x06001C5A RID: 7258 RVA: 0x000A48BB File Offset: 0x000A2ABB
		protected override void LoadPrototypes()
		{
			base.LoadPrototypes();
			this.AlertOrder = this._prototypeManager.EnumeratePrototypes<AlertOrderPrototype>().FirstOrDefault<AlertOrderPrototype>();
			if (this.AlertOrder == null)
			{
				Logger.ErrorS("alert", "no alertOrder prototype found, alerts will be in random order");
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06001C5B RID: 7259 RVA: 0x000A48F0 File Offset: 0x000A2AF0
		public IReadOnlyDictionary<AlertKey, AlertState> ActiveAlerts
		{
			get
			{
				LocalPlayer localPlayer = this._playerManager.LocalPlayer;
				EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
				if (entityUid == null)
				{
					return null;
				}
				return base.GetActiveAlerts(entityUid.Value);
			}
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x000A4938 File Offset: 0x000A2B38
		[NullableContext(1)]
		protected override void AfterShowAlert(AlertsComponent alertsComponent)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			EntityUid owner = alertsComponent.Owner;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != owner))
			{
				return;
			}
			EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
			if (syncAlerts == null)
			{
				return;
			}
			syncAlerts(this, alertsComponent.Alerts);
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x000A49AC File Offset: 0x000A2BAC
		[NullableContext(1)]
		protected override void AfterClearAlert(AlertsComponent alertsComponent)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			EntityUid owner = alertsComponent.Owner;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != owner))
			{
				return;
			}
			EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
			if (syncAlerts == null)
			{
				return;
			}
			syncAlerts(this, alertsComponent.Alerts);
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x000A4A20 File Offset: 0x000A2C20
		[NullableContext(1)]
		private void ClientAlertsHandleState(EntityUid uid, AlertsComponent component, ref ComponentHandleState args)
		{
			AlertsComponentState alertsComponentState = args.Current as AlertsComponentState;
			Dictionary<AlertKey, AlertState> dictionary = (alertsComponentState != null) ? alertsComponentState.Alerts : null;
			if (dictionary == null)
			{
				return;
			}
			component.Alerts = new Dictionary<AlertKey, AlertState>(dictionary);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == uid));
			}
			if (flag)
			{
				EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
				if (syncAlerts == null)
				{
					return;
				}
				syncAlerts(this, dictionary);
			}
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000A4AA8 File Offset: 0x000A2CA8
		[NullableContext(1)]
		private void OnPlayerAttached(EntityUid uid, AlertsComponent component, PlayerAttachedEvent args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			EventHandler<IReadOnlyDictionary<AlertKey, AlertState>> syncAlerts = this.SyncAlerts;
			if (syncAlerts == null)
			{
				return;
			}
			syncAlerts(this, component.Alerts);
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x000A4B10 File Offset: 0x000A2D10
		[NullableContext(1)]
		protected override void HandleComponentShutdown(EntityUid uid, AlertsComponent component, ComponentShutdown args)
		{
			base.HandleComponentShutdown(uid, component, args);
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = true;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity == null || (controlledEntity != null && controlledEntity.GetValueOrDefault() != uid));
			}
			if (flag)
			{
				return;
			}
			EventHandler clearAlerts = this.ClearAlerts;
			if (clearAlerts == null)
			{
				return;
			}
			clearAlerts(this, EventArgs.Empty);
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x000A4B7E File Offset: 0x000A2D7E
		[NullableContext(1)]
		private void OnPlayerDetached(EntityUid uid, AlertsComponent component, PlayerDetachedEvent args)
		{
			EventHandler clearAlerts = this.ClearAlerts;
			if (clearAlerts == null)
			{
				return;
			}
			clearAlerts(this, EventArgs.Empty);
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000A4B96 File Offset: 0x000A2D96
		public void AlertClicked(AlertType alertType)
		{
			base.RaiseNetworkEvent(new ClickAlertEvent(alertType));
		}

		// Token: 0x04000E30 RID: 3632
		[Nullable(1)]
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000E31 RID: 3633
		[Nullable(1)]
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
