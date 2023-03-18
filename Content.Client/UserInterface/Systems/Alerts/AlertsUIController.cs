using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Alerts;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Shared.Alert;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems.Alerts
{
	// Token: 0x020000B9 RID: 185
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AlertsUIController : UIController, IOnStateEntered<GameplayState>, IOnSystemChanged<ClientAlertsSystem>, IOnSystemLoaded<ClientAlertsSystem>, IOnSystemUnloaded<ClientAlertsSystem>
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x0001B911 File Offset: 0x00019B11
		[Nullable(2)]
		private AlertsUI UI
		{
			[NullableContext(2)]
			get
			{
				return this.UIManager.GetActiveUIWidgetOrNull<AlertsUI>();
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001B91E File Offset: 0x00019B1E
		[NullableContext(2)]
		private void OnAlertPressed(object sender, AlertType e)
		{
			ClientAlertsSystem alertsSystem = this._alertsSystem;
			if (alertsSystem == null)
			{
				return;
			}
			alertsSystem.AlertClicked(e);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001B931 File Offset: 0x00019B31
		private void SystemOnClearAlerts([Nullable(2)] object sender, EventArgs e)
		{
			AlertsUI ui = this.UI;
			if (ui == null)
			{
				return;
			}
			ui.ClearAllControls();
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001B944 File Offset: 0x00019B44
		private void SystemOnSyncAlerts([Nullable(2)] object sender, IReadOnlyDictionary<AlertKey, AlertState> e)
		{
			ClientAlertsSystem clientAlertsSystem = sender as ClientAlertsSystem;
			if (clientAlertsSystem != null)
			{
				AlertsUI ui = this.UI;
				if (ui != null)
				{
					ui.SyncControls(clientAlertsSystem, clientAlertsSystem.AlertOrder, e);
				}
			}
			if (this.UI != null)
			{
				this.UI.AlertPressed -= this.OnAlertPressed;
				this.UI.AlertPressed += this.OnAlertPressed;
			}
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001B9AA File Offset: 0x00019BAA
		public void OnSystemLoaded(ClientAlertsSystem system)
		{
			system.SyncAlerts += this.SystemOnSyncAlerts;
			system.ClearAlerts += this.SystemOnClearAlerts;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001B9D0 File Offset: 0x00019BD0
		public void OnSystemUnloaded(ClientAlertsSystem system)
		{
			system.SyncAlerts -= this.SystemOnSyncAlerts;
			system.ClearAlerts -= this.SystemOnClearAlerts;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001B9F6 File Offset: 0x00019BF6
		public void OnStateEntered(GameplayState state)
		{
			if (this.UI != null)
			{
				this.UI.AlertPressed += this.OnAlertPressed;
			}
			this.SyncAlerts();
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001BA20 File Offset: 0x00019C20
		public void SyncAlerts()
		{
			ClientAlertsSystem alertsSystem = this._alertsSystem;
			IReadOnlyDictionary<AlertKey, AlertState> readOnlyDictionary = (alertsSystem != null) ? alertsSystem.ActiveAlerts : null;
			if (readOnlyDictionary != null)
			{
				this.SystemOnSyncAlerts(this._alertsSystem, readOnlyDictionary);
			}
		}

		// Token: 0x04000253 RID: 595
		[Nullable(2)]
		[UISystemDependency]
		private readonly ClientAlertsSystem _alertsSystem;
	}
}
