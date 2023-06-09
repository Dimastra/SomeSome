﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Content.Shared.Localizations;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Administration.UI.Tabs.AdminTab
{
	// Token: 0x020004A8 RID: 1192
	[GenerateTypedNameReferences]
	public sealed class AdminShuttleWindow : DefaultWindow
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x000AB45D File Offset: 0x000A965D
		public AdminShuttleWindow()
		{
			AdminShuttleWindow.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<AdminShuttleWindow>(this);
			this._callShuttleTime.OnTextChanged += this.CallShuttleTimeOnOnTextChanged;
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x000AB48C File Offset: 0x000A968C
		[NullableContext(1)]
		private void CallShuttleTimeOnOnTextChanged(LineEdit.LineEditEventArgs obj)
		{
			ILocalizationManager localizationManager = IoCManager.Resolve<ILocalizationManager>();
			TimeSpan timeSpan;
			this._callShuttleButton.Disabled = !TimeSpan.TryParseExact(obj.Text, ContentLocalizationManager.TimeSpanMinutesFormats, localizationManager.DefaultCulture, out timeSpan);
			this._callShuttleButton.Command = "callshuttle " + obj.Text;
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06001D59 RID: 7513 RVA: 0x000AB4E0 File Offset: 0x000A96E0
		private LineEdit _callShuttleTime
		{
			get
			{
				return base.FindControl<LineEdit>("_callShuttleTime");
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06001D5A RID: 7514 RVA: 0x000AB4ED File Offset: 0x000A96ED
		private CommandButton _callShuttleButton
		{
			get
			{
				return base.FindControl<CommandButton>("_callShuttleButton");
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001D5B RID: 7515 RVA: 0x000AB4FA File Offset: 0x000A96FA
		private CommandButton _recallShuttleButton
		{
			get
			{
				return base.FindControl<CommandButton>("_recallShuttleButton");
			}
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x000AB508 File Offset: 0x000A9708
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("admin-shuttle-title").ProvideValue();
			A_1.MinWidth = 300f;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			boxContainer2.HorizontalExpand = true;
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "_callShuttleTime";
			Control control = lineEdit;
			context.RobustNameScope.Register("_callShuttleTime", control);
			lineEdit.Text = "4:00";
			lineEdit.PlaceHolder = "m:ss";
			lineEdit.HorizontalExpand = true;
			lineEdit.SizeFlagsStretchRatio = 2f;
			control = lineEdit;
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 1f
			};
			boxContainer2.XamlChildren.Add(control);
			CommandButton commandButton = new CommandButton();
			commandButton.Command = "callshuttle 4:00";
			commandButton.Name = "_callShuttleButton";
			control = commandButton;
			context.RobustNameScope.Register("_callShuttleButton", control);
			commandButton.Text = (string)new LocExtension("comms-console-menu-call-shuttle").ProvideValue();
			commandButton.HorizontalExpand = true;
			commandButton.SizeFlagsStretchRatio = 2f;
			control = commandButton;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			CommandButton commandButton2 = new CommandButton();
			commandButton2.Command = "recallshuttle";
			commandButton2.Name = "_recallShuttleButton";
			control = commandButton2;
			context.RobustNameScope.Register("_recallShuttleButton", control);
			commandButton2.Text = (string)new LocExtension("comms-console-menu-recall-shuttle").ProvideValue();
			commandButton2.HorizontalAlignment = 2;
			control = commandButton2;
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

		// Token: 0x06001D5D RID: 7517 RVA: 0x000AB77E File Offset: 0x000A997E
		private static void !XamlIlPopulateTrampoline(AdminShuttleWindow A_0)
		{
			AdminShuttleWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleWindow.xaml(null, A_0);
		}
	}
}
