using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Shared.Alert
{
	// Token: 0x02000720 RID: 1824
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class AlertsSystem : EntitySystem
	{
		// Token: 0x06001619 RID: 5657 RVA: 0x000484DC File Offset: 0x000466DC
		[NullableContext(2)]
		public IReadOnlyDictionary<AlertKey, AlertState> GetActiveAlerts(EntityUid euid)
		{
			AlertsComponent comp;
			if (!this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref comp))
			{
				return null;
			}
			return comp.Alerts;
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x00048504 File Offset: 0x00046704
		public short GetSeverityRange(AlertType alertType)
		{
			short minSeverity = this._typeToAlert[alertType].MinSeverity;
			return (short)MathF.Max((float)minSeverity, (float)(this._typeToAlert[alertType].MaxSeverity - minSeverity));
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0004853F File Offset: 0x0004673F
		public short GetMaxSeverity(AlertType alertType)
		{
			return this._typeToAlert[alertType].MaxSeverity;
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x00048552 File Offset: 0x00046752
		public short GetMinSeverity(AlertType alertType)
		{
			return this._typeToAlert[alertType].MinSeverity;
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x00048568 File Offset: 0x00046768
		public bool IsShowingAlert(EntityUid euid, AlertType alertType)
		{
			AlertsComponent alertsComponent;
			if (!this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent))
			{
				return false;
			}
			AlertPrototype alert;
			if (this.TryGet(alertType, out alert))
			{
				return alertsComponent.Alerts.ContainsKey(alert.AlertKey);
			}
			Logger.DebugS("alert", "unknown alert type {0}", new object[]
			{
				alertType
			});
			return false;
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x000485C4 File Offset: 0x000467C4
		public bool IsShowingAlertCategory(EntityUid euid, AlertCategory alertCategory)
		{
			AlertsComponent alertsComponent;
			return this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent) && alertsComponent.Alerts.ContainsKey(AlertKey.ForCategory(alertCategory));
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x000485F4 File Offset: 0x000467F4
		public bool TryGetAlertState(EntityUid euid, AlertKey key, out AlertState alertState)
		{
			AlertsComponent alertsComponent;
			if (this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent))
			{
				return alertsComponent.Alerts.TryGetValue(key, out alertState);
			}
			alertState = default(AlertState);
			return false;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00048628 File Offset: 0x00046828
		[NullableContext(0)]
		public void ShowAlert(EntityUid euid, AlertType alertType, short? severity = null, ValueTuple<TimeSpan, TimeSpan>? cooldown = null)
		{
			AlertsComponent alertsComponent;
			if (!this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent))
			{
				return;
			}
			AlertPrototype alert;
			if (this.TryGet(alertType, out alert))
			{
				AlertState alertStateCallback;
				if (alertsComponent.Alerts.TryGetValue(alert.AlertKey, out alertStateCallback) && alertStateCallback.Type == alertType)
				{
					short? num = alertStateCallback.Severity;
					int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
					num = severity;
					int? num3 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
					if (num2.GetValueOrDefault() == num3.GetValueOrDefault() & num2 != null == (num3 != null))
					{
						ValueTuple<TimeSpan, TimeSpan>? cooldown2 = alertStateCallback.Cooldown;
						ValueTuple<TimeSpan, TimeSpan>? valueTuple = cooldown;
						bool flag = cooldown2 != null;
						bool flag2;
						if (flag != (valueTuple != null))
						{
							flag2 = false;
						}
						else if (!flag)
						{
							flag2 = true;
						}
						else
						{
							ValueTuple<TimeSpan, TimeSpan> valueOrDefault = cooldown2.GetValueOrDefault();
							ValueTuple<TimeSpan, TimeSpan> valueOrDefault2 = valueTuple.GetValueOrDefault();
							flag2 = (valueOrDefault.Item1 == valueOrDefault2.Item1 && valueOrDefault.Item2 == valueOrDefault2.Item2);
						}
						if (flag2)
						{
							return;
						}
					}
				}
				alertsComponent.Alerts.Remove(alert.AlertKey);
				alertsComponent.Alerts[alert.AlertKey] = new AlertState
				{
					Cooldown = cooldown,
					Severity = severity,
					Type = alertType
				};
				this.AfterShowAlert(alertsComponent);
				base.Dirty(alertsComponent, null);
				return;
			}
			Logger.ErrorS("alert", "Unable to show alert {0}, please ensure this alertType has a corresponding YML alert prototype", new object[]
			{
				alertType
			});
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x000487CC File Offset: 0x000469CC
		public void ClearAlertCategory(EntityUid euid, AlertCategory category)
		{
			AlertsComponent alertsComponent;
			if (!this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent))
			{
				return;
			}
			AlertKey key = AlertKey.ForCategory(category);
			if (!alertsComponent.Alerts.Remove(key))
			{
				return;
			}
			this.AfterClearAlert(alertsComponent);
			base.Dirty(alertsComponent, null);
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x00048810 File Offset: 0x00046A10
		public void ClearAlert(EntityUid euid, AlertType alertType)
		{
			AlertsComponent alertsComponent;
			if (!this.EntityManager.TryGetComponent<AlertsComponent>(euid, ref alertsComponent))
			{
				return;
			}
			AlertPrototype alert;
			if (!this.TryGet(alertType, out alert))
			{
				Logger.ErrorS("alert", "unable to clear alert, unknown alertType {0}", new object[]
				{
					alertType
				});
				return;
			}
			if (!alertsComponent.Alerts.Remove(alert.AlertKey))
			{
				return;
			}
			this.AfterClearAlert(alertsComponent);
			base.Dirty(alertsComponent, null);
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x0004887B File Offset: 0x00046A7B
		protected virtual void AfterShowAlert(AlertsComponent alertsComponent)
		{
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x0004887D File Offset: 0x00046A7D
		protected virtual void AfterClearAlert(AlertsComponent alertsComponent)
		{
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x00048880 File Offset: 0x00046A80
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AlertsComponent, ComponentStartup>(new ComponentEventHandler<AlertsComponent, ComponentStartup>(this.HandleComponentStartup), null, null);
			base.SubscribeLocalEvent<AlertsComponent, ComponentShutdown>(new ComponentEventHandler<AlertsComponent, ComponentShutdown>(this.HandleComponentShutdown), null, null);
			ComponentEventRefHandler<AlertsComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = AlertsSystem.<>O.<0>__ClientAlertsGetState) == null)
			{
				componentEventRefHandler = (AlertsSystem.<>O.<0>__ClientAlertsGetState = new ComponentEventRefHandler<AlertsComponent, ComponentGetState>(AlertsSystem.ClientAlertsGetState));
			}
			base.SubscribeLocalEvent<AlertsComponent, ComponentGetState>(componentEventRefHandler, null, null);
			base.SubscribeNetworkEvent<ClickAlertEvent>(new EntitySessionEventHandler<ClickAlertEvent>(this.HandleClickAlert), null, null);
			this.LoadPrototypes();
			this._prototypeManager.PrototypesReloaded += this.HandlePrototypesReloaded;
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x00048910 File Offset: 0x00046B10
		protected virtual void HandleComponentShutdown(EntityUid uid, AlertsComponent component, ComponentShutdown args)
		{
			base.RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x00048920 File Offset: 0x00046B20
		private void HandleComponentStartup(EntityUid uid, AlertsComponent component, ComponentStartup args)
		{
			base.RaiseLocalEvent<AlertSyncEvent>(uid, new AlertSyncEvent(uid), true);
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x00048930 File Offset: 0x00046B30
		public override void Shutdown()
		{
			this._prototypeManager.PrototypesReloaded -= this.HandlePrototypesReloaded;
			base.Shutdown();
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0004894F File Offset: 0x00046B4F
		private void HandlePrototypesReloaded(PrototypesReloadedEventArgs obj)
		{
			this.LoadPrototypes();
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x00048958 File Offset: 0x00046B58
		protected virtual void LoadPrototypes()
		{
			this._typeToAlert.Clear();
			foreach (AlertPrototype alert in this._prototypeManager.EnumeratePrototypes<AlertPrototype>())
			{
				if (!this._typeToAlert.TryAdd(alert.AlertType, alert))
				{
					Logger.ErrorS("alert", "Found alert with duplicate alertType {0} - all alerts must have a unique alerttype, this one will be skipped", new object[]
					{
						alert.AlertType
					});
				}
			}
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x000489E8 File Offset: 0x00046BE8
		[NullableContext(2)]
		public bool TryGet(AlertType alertType, [NotNullWhen(true)] out AlertPrototype alert)
		{
			return this._typeToAlert.TryGetValue(alertType, out alert);
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x000489F8 File Offset: 0x00046BF8
		private void HandleClickAlert(ClickAlertEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? player = args.SenderSession.AttachedEntity;
			if (player == null || !this.EntityManager.HasComponent<AlertsComponent>(player))
			{
				return;
			}
			if (!this.IsShowingAlert(player.Value, msg.Type))
			{
				Logger.DebugS("alert", "user {0} attempted to click alert {1} which is not currently showing for them", new object[]
				{
					this.EntityManager.GetComponent<MetaDataComponent>(player.Value).EntityName,
					msg.Type
				});
				return;
			}
			AlertPrototype alert;
			if (!this.TryGet(msg.Type, out alert))
			{
				Logger.WarningS("alert", "unrecognized encoded alert {0}", new object[]
				{
					msg.Type
				});
				return;
			}
			IAlertClick onClick = alert.OnClick;
			if (onClick == null)
			{
				return;
			}
			onClick.AlertClicked(player.Value);
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x00048AC8 File Offset: 0x00046CC8
		private static void ClientAlertsGetState(EntityUid uid, AlertsComponent component, ref ComponentGetState args)
		{
			args.State = new AlertsComponentState(component.Alerts);
		}

		// Token: 0x04001652 RID: 5714
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001653 RID: 5715
		private readonly Dictionary<AlertType, AlertPrototype> _typeToAlert = new Dictionary<AlertType, AlertPrototype>();

		// Token: 0x0200088B RID: 2187
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001A62 RID: 6754
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<AlertsComponent, ComponentGetState> <0>__ClientAlertsGetState;
		}
	}
}
