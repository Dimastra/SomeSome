﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Administration.UI.Tabs.AdminTab
{
	// Token: 0x020004AB RID: 1195
	[GenerateTypedNameReferences]
	public sealed class PlayerActionsWindow : DefaultWindow
	{
		// Token: 0x06001D7A RID: 7546 RVA: 0x000AC5A0 File Offset: 0x000AA7A0
		protected override void EnteredTree()
		{
			this.SubmitKickButton.OnPressed += this.SubmitKickButtonOnPressed;
			this.SubmitAHelpButton.OnPressed += this.SubmitAhelpButtonOnPressed;
			this.SubmitRespawnButton.OnPressed += this.SubmitRespawnButtonOnPressed;
			this.PlayerList.OnSelectionChanged += this.OnListOnOnSelectionChanged;
		}

		// Token: 0x06001D7B RID: 7547 RVA: 0x000AC60C File Offset: 0x000AA80C
		[NullableContext(2)]
		private void OnListOnOnSelectionChanged(PlayerInfo obj)
		{
			this._selectedPlayer = obj;
			bool disabled = this._selectedPlayer == null;
			this.SubmitKickButton.Disabled = disabled;
			this.SubmitAHelpButton.Disabled = disabled;
			this.SubmitRespawnButton.Disabled = disabled;
		}

		// Token: 0x06001D7C RID: 7548 RVA: 0x000AC654 File Offset: 0x000AA854
		[NullableContext(1)]
		private void SubmitKickButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this._selectedPlayer == null)
			{
				return;
			}
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
			defaultInterpolatedStringHandler.AppendLiteral("kick \"");
			defaultInterpolatedStringHandler.AppendFormatted(this._selectedPlayer.Username);
			defaultInterpolatedStringHandler.AppendLiteral("\" \"");
			defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(this.ReasonLine.Text));
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x000AC6D8 File Offset: 0x000AA8D8
		[NullableContext(1)]
		private void SubmitAhelpButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this._selectedPlayer == null)
			{
				return;
			}
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
			defaultInterpolatedStringHandler.AppendLiteral("openahelp \"");
			defaultInterpolatedStringHandler.AppendFormatted<NetUserId>(this._selectedPlayer.SessionId);
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x000AC739 File Offset: 0x000AA939
		[NullableContext(1)]
		private void SubmitRespawnButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this._selectedPlayer == null)
			{
				return;
			}
			IoCManager.Resolve<IClientConsoleHost>().ExecuteCommand("respawn \"" + this._selectedPlayer.Username + "\"");
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06001D7F RID: 7551 RVA: 0x000ABED6 File Offset: 0x000AA0D6
		private LineEdit ReasonLine
		{
			get
			{
				return base.FindControl<LineEdit>("ReasonLine");
			}
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x000ABF31 File Offset: 0x000AA131
		private PlayerListControl PlayerList
		{
			get
			{
				return base.FindControl<PlayerListControl>("PlayerList");
			}
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x000AC76E File Offset: 0x000AA96E
		private Button SubmitKickButton
		{
			get
			{
				return base.FindControl<Button>("SubmitKickButton");
			}
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06001D82 RID: 7554 RVA: 0x000AC77B File Offset: 0x000AA97B
		private Button SubmitAHelpButton
		{
			get
			{
				return base.FindControl<Button>("SubmitAHelpButton");
			}
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06001D83 RID: 7555 RVA: 0x000AC788 File Offset: 0x000AA988
		private Button SubmitRespawnButton
		{
			get
			{
				return base.FindControl<Button>("SubmitRespawnButton");
			}
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x000AC795 File Offset: 0x000AA995
		public PlayerActionsWindow()
		{
			PlayerActionsWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x000AC7A4 File Offset: 0x000AA9A4
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AdminTab.PlayerActionsWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("admin-player-actions-window-title").ProvideValue();
			A_1.MinSize = new Vector2(425f, 272f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Control control = new Label
			{
				Text = (string)new LocExtension("Reason").ProvideValue(),
				MinWidth = 100f
			};
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				MinWidth = 50f
			};
			boxContainer2.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "ReasonLine";
			control = lineEdit;
			context.RobustNameScope.Register("ReasonLine", control);
			lineEdit.MinWidth = 100f;
			lineEdit.HorizontalExpand = true;
			control = lineEdit;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			PlayerListControl playerListControl = new PlayerListControl();
			playerListControl.Name = "PlayerList";
			control = playerListControl;
			context.RobustNameScope.Register("PlayerList", control);
			playerListControl.VerticalExpand = true;
			control = playerListControl;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			Button button = new Button();
			button.Name = "SubmitKickButton";
			control = button;
			context.RobustNameScope.Register("SubmitKickButton", control);
			button.Text = (string)new LocExtension("admin-player-actions-kick").ProvideValue();
			button.Disabled = true;
			control = button;
			boxContainer3.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "SubmitAHelpButton";
			control = button2;
			context.RobustNameScope.Register("SubmitAHelpButton", control);
			button2.Text = (string)new LocExtension("admin-player-actions-ahelp").ProvideValue();
			button2.Disabled = true;
			control = button2;
			boxContainer3.XamlChildren.Add(control);
			Button button3 = new Button();
			button3.Name = "SubmitRespawnButton";
			control = button3;
			context.RobustNameScope.Register("SubmitRespawnButton", control);
			button3.Text = (string)new LocExtension("admin-player-actions-respawn").ProvideValue();
			button3.Disabled = true;
			control = button3;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x000ACAE9 File Offset: 0x000AACE9
		private static void !XamlIlPopulateTrampoline(PlayerActionsWindow A_0)
		{
			PlayerActionsWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.PlayerActionsWindow.xaml(null, A_0);
		}

		// Token: 0x04000E9C RID: 3740
		[Nullable(2)]
		private PlayerInfo _selectedPlayer;
	}
}
