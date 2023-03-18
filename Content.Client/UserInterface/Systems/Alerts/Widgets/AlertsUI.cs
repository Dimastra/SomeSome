﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.UserInterface.Systems.Alerts.Controls;
using Content.Shared.Alert;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Alerts.Widgets
{
	// Token: 0x020000BA RID: 186
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public sealed class AlertsUI : UIWidget
	{
		// Token: 0x06000505 RID: 1285 RVA: 0x0001BA50 File Offset: 0x00019C50
		public AlertsUI()
		{
			AlertsUI.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001BA69 File Offset: 0x00019C69
		public void SyncControls(AlertsSystem alertsSystem, [Nullable(2)] AlertOrderPrototype alertOrderPrototype, IReadOnlyDictionary<AlertKey, AlertState> alertStates)
		{
			if (this.SyncRemoveControls(alertStates))
			{
				return;
			}
			this.SyncUpdateControls(alertsSystem, alertOrderPrototype, alertStates);
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001BA80 File Offset: 0x00019C80
		public void ClearAllControls()
		{
			foreach (AlertControl alertControl in this._alertControls.Values)
			{
				alertControl.OnPressed -= this.AlertControlPressed;
				alertControl.Dispose();
			}
			this._alertControls.Clear();
		}

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06000508 RID: 1288 RVA: 0x0001BAF4 File Offset: 0x00019CF4
		// (remove) Token: 0x06000509 RID: 1289 RVA: 0x0001BB2C File Offset: 0x00019D2C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event EventHandler<AlertType> AlertPressed;

		// Token: 0x0600050A RID: 1290 RVA: 0x0001BB64 File Offset: 0x00019D64
		private bool SyncRemoveControls(IReadOnlyDictionary<AlertKey, AlertState> alertStates)
		{
			List<AlertKey> list = new List<AlertKey>();
			foreach (AlertKey alertKey in this._alertControls.Keys)
			{
				if (!alertStates.ContainsKey(alertKey))
				{
					list.Add(alertKey);
				}
			}
			foreach (AlertKey key in list)
			{
				AlertControl alertControl;
				this._alertControls.Remove(key, out alertControl);
				if (alertControl == null)
				{
					return true;
				}
				this.AlertContainer.Children.Remove(alertControl);
			}
			return false;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001BC34 File Offset: 0x00019E34
		private void SyncUpdateControls(AlertsSystem alertsSystem, [Nullable(2)] AlertOrderPrototype alertOrderPrototype, IReadOnlyDictionary<AlertKey, AlertState> alertStates)
		{
			foreach (KeyValuePair<AlertKey, AlertState> keyValuePair in alertStates)
			{
				AlertKey alertKey;
				AlertState alertState;
				keyValuePair.Deconstruct(out alertKey, out alertState);
				AlertKey alertKey2 = alertKey;
				AlertState alertState2 = alertState;
				if (alertKey2.AlertType == null)
				{
					Logger.WarningS("alert", "found alertkey without alerttype, alert keys should never be stored without an alerttype set: {0}", new object[]
					{
						alertKey2
					});
				}
				else
				{
					AlertType value = alertKey2.AlertType.Value;
					AlertPrototype alertPrototype;
					AlertControl alertControl;
					if (!alertsSystem.TryGet(value, out alertPrototype))
					{
						Logger.ErrorS("alert", "Unrecognized alertType {0}", new object[]
						{
							value
						});
					}
					else if (this._alertControls.TryGetValue(alertPrototype.AlertKey, out alertControl) && alertControl.Alert.AlertType == alertPrototype.AlertType)
					{
						alertControl.SetSeverity(alertState2.Severity);
						alertControl.Cooldown = alertState2.Cooldown;
					}
					else
					{
						if (alertControl != null)
						{
							this.AlertContainer.Children.Remove(alertControl);
						}
						AlertControl alertControl2 = this.CreateAlertControl(alertPrototype, alertState2);
						if (alertOrderPrototype != null)
						{
							bool flag = false;
							foreach (Control control in this.AlertContainer.Children)
							{
								if (alertOrderPrototype.Compare(alertPrototype, ((AlertControl)control).Alert) < 0)
								{
									int positionInParent = control.GetPositionInParent();
									this.AlertContainer.Children.Add(alertControl2);
									alertControl2.SetPositionInParent(positionInParent);
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								this.AlertContainer.Children.Add(alertControl2);
							}
						}
						else
						{
							this.AlertContainer.Children.Add(alertControl2);
						}
						this._alertControls[alertPrototype.AlertKey] = alertControl2;
					}
				}
			}
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001BE50 File Offset: 0x0001A050
		private AlertControl CreateAlertControl(AlertPrototype alert, AlertState alertState)
		{
			AlertControl alertControl = new AlertControl(alert, alertState.Severity);
			alertControl.Cooldown = alertState.Cooldown;
			alertControl.OnPressed += this.AlertControlPressed;
			return alertControl;
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0001BE7C File Offset: 0x0001A07C
		private void AlertControlPressed(BaseButton.ButtonEventArgs args)
		{
			AlertControl alertControl = args.Button as AlertControl;
			if (alertControl == null)
			{
				return;
			}
			if (args.Event.Function != EngineKeyFunctions.UIClick)
			{
				return;
			}
			EventHandler<AlertType> alertPressed = this.AlertPressed;
			if (alertPressed == null)
			{
				return;
			}
			alertPressed(this, alertControl.Alert.AlertType);
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x0001BECD File Offset: 0x0001A0CD
		[Nullable(0)]
		public BoxContainer AlertContainer
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<BoxContainer>("AlertContainer");
			}
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001BEDC File Offset: 0x0001A0DC
		static void xaml(IServiceProvider A_0, AlertsUI A_1)
		{
			XamlIlContext.Context<AlertsUI> context = new XamlIlContext.Context<AlertsUI>(A_0, null, "resm:Content.Client.UserInterface.Systems.Alerts.Widgets.AlertsUI.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.MinSize = new Vector2(64f, 64f);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.HorizontalAlignment = 3;
			panelContainer.VerticalAlignment = 1;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Name = "AlertContainer";
			Control control = boxContainer;
			context.RobustNameScope.Register("AlertContainer", control);
			boxContainer.Access = new AccessLevel?(0);
			boxContainer.Orientation = 1;
			control = boxContainer;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001BFF1 File Offset: 0x0001A1F1
		private static void !XamlIlPopulateTrampoline(AlertsUI A_0)
		{
			AlertsUI.Populate:Content.Client.UserInterface.Systems.Alerts.Widgets.AlertsUI.xaml(null, A_0);
		}

		// Token: 0x04000254 RID: 596
		private readonly Dictionary<AlertKey, AlertControl> _alertControls = new Dictionary<AlertKey, AlertControl>();
	}
}
