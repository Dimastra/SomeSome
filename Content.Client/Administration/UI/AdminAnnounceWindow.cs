﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Administration.UI
{
	// Token: 0x02000488 RID: 1160
	[GenerateTypedNameReferences]
	public sealed class AdminAnnounceWindow : DefaultWindow
	{
		// Token: 0x06001C8E RID: 7310 RVA: 0x000A58B8 File Offset: 0x000A3AB8
		public AdminAnnounceWindow()
		{
			AdminAnnounceWindow.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<AdminAnnounceWindow>(this);
			this.AnnounceMethod.AddItem(this._localization.GetString("admin-announce-type-station"), null);
			this.AnnounceMethod.SetItemMetadata(0, AdminAnnounceType.Station);
			this.AnnounceMethod.AddItem(this._localization.GetString("admin-announce-type-server"), null);
			this.AnnounceMethod.SetItemMetadata(1, AdminAnnounceType.Server);
			this.AnnounceMethod.OnItemSelected += this.AnnounceMethodOnOnItemSelected;
			this.Announcement.OnTextChanged += this.AnnouncementOnOnTextChanged;
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x000A5972 File Offset: 0x000A3B72
		[NullableContext(1)]
		private void AnnouncementOnOnTextChanged(LineEdit.LineEditEventArgs args)
		{
			this.AnnounceButton.Disabled = (args.Text.TrimStart() == "");
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x000A5994 File Offset: 0x000A3B94
		[NullableContext(1)]
		private void AnnounceMethodOnOnItemSelected(OptionButton.ItemSelectedEventArgs args)
		{
			this.AnnounceMethod.SelectId(args.Id);
			this.Announcer.Editable = (((AdminAnnounceType?)args.Button.SelectedMetadata).GetValueOrDefault() == AdminAnnounceType.Station);
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001C91 RID: 7313 RVA: 0x000A59D8 File Offset: 0x000A3BD8
		public LineEdit Announcer
		{
			get
			{
				return base.FindControl<LineEdit>("Announcer");
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001C92 RID: 7314 RVA: 0x000A59E5 File Offset: 0x000A3BE5
		public OptionButton AnnounceMethod
		{
			get
			{
				return base.FindControl<OptionButton>("AnnounceMethod");
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06001C93 RID: 7315 RVA: 0x000A59F2 File Offset: 0x000A3BF2
		public LineEdit Announcement
		{
			get
			{
				return base.FindControl<LineEdit>("Announcement");
			}
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06001C94 RID: 7316 RVA: 0x000A59FF File Offset: 0x000A3BFF
		public CheckBox KeepWindowOpen
		{
			get
			{
				return base.FindControl<CheckBox>("KeepWindowOpen");
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06001C95 RID: 7317 RVA: 0x000A5A0C File Offset: 0x000A3C0C
		public Button AnnounceButton
		{
			get
			{
				return base.FindControl<Button>("AnnounceButton");
			}
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x000A5A1C File Offset: 0x000A3C1C
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.AdminAnnounceWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("admin-announce-title").ProvideValue();
			A_1.MinWidth = 500f;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 1;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.HorizontalExpand = true;
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "Announcer";
			Control control = lineEdit;
			context.RobustNameScope.Register("Announcer", control);
			lineEdit.Access = new AccessLevel?(0);
			lineEdit.PlaceHolder = (string)new LocExtension("admin-announce-announcer-placeholder").ProvideValue();
			lineEdit.Text = (string)new LocExtension("admin-announce-announcer-default").ProvideValue();
			lineEdit.HorizontalExpand = true;
			lineEdit.SizeFlagsStretchRatio = 2f;
			control = lineEdit;
			boxContainer.XamlChildren.Add(control);
			control = new Control
			{
				HorizontalExpand = true,
				SizeFlagsStretchRatio = 1f
			};
			boxContainer.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "AnnounceMethod";
			control = optionButton;
			context.RobustNameScope.Register("AnnounceMethod", control);
			optionButton.Access = new AccessLevel?(0);
			optionButton.HorizontalExpand = true;
			optionButton.SizeFlagsStretchRatio = 2f;
			control = optionButton;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			gridContainer.XamlChildren.Add(control);
			LineEdit lineEdit2 = new LineEdit();
			lineEdit2.Name = "Announcement";
			control = lineEdit2;
			context.RobustNameScope.Register("Announcement", control);
			lineEdit2.Access = new AccessLevel?(0);
			lineEdit2.PlaceHolder = (string)new LocExtension("admin-announce-announcement-placeholder").ProvideValue();
			control = lineEdit2;
			gridContainer.XamlChildren.Add(control);
			GridContainer gridContainer2 = new GridContainer();
			gridContainer2.Rows = 1;
			CheckBox checkBox = new CheckBox();
			checkBox.Name = "KeepWindowOpen";
			control = checkBox;
			context.RobustNameScope.Register("KeepWindowOpen", control);
			checkBox.Access = new AccessLevel?(0);
			checkBox.Text = (string)new LocExtension("admin-announce-keep-open").ProvideValue();
			control = checkBox;
			gridContainer2.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "AnnounceButton";
			control = button;
			context.RobustNameScope.Register("AnnounceButton", control);
			button.Access = new AccessLevel?(0);
			button.Disabled = true;
			button.Text = (string)new LocExtension("admin-announce-button").ProvideValue();
			button.HorizontalAlignment = 2;
			control = button;
			gridContainer2.XamlChildren.Add(control);
			control = gridContainer2;
			gridContainer.XamlChildren.Add(control);
			control = gridContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x000A5DB6 File Offset: 0x000A3FB6
		private static void !XamlIlPopulateTrampoline(AdminAnnounceWindow A_0)
		{
			AdminAnnounceWindow.Populate:Content.Client.Administration.UI.AdminAnnounceWindow.xaml(null, A_0);
		}

		// Token: 0x04000E4E RID: 3662
		[Nullable(1)]
		[Dependency]
		private readonly ILocalizationManager _localization;
	}
}
