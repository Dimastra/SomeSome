﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Actions.Windows
{
	// Token: 0x020000C5 RID: 197
	[GenerateTypedNameReferences]
	public sealed class ActionsWindow : DefaultWindow
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x0001E12B File Offset: 0x0001C32B
		// (set) Token: 0x0600057A RID: 1402 RVA: 0x0001E133 File Offset: 0x0001C333
		[Nullable(1)]
		public MultiselectOptionButton<ActionsWindow.Filters> FilterButton { [NullableContext(1)] get; [NullableContext(1)] private set; }

		// Token: 0x0600057B RID: 1403 RVA: 0x0001E13C File Offset: 0x0001C33C
		public ActionsWindow()
		{
			ActionsWindow.!XamlIlPopulateTrampoline(this);
			Control searchContainer = this.SearchContainer;
			MultiselectOptionButton<ActionsWindow.Filters> multiselectOptionButton = new MultiselectOptionButton<ActionsWindow.Filters>();
			multiselectOptionButton.Label = Loc.GetString("ui-actionmenu-filter-button");
			MultiselectOptionButton<ActionsWindow.Filters> multiselectOptionButton2 = multiselectOptionButton;
			this.FilterButton = multiselectOptionButton;
			searchContainer.AddChild(multiselectOptionButton2);
			foreach (ActionsWindow.Filters filters in Enum.GetValues<ActionsWindow.Filters>())
			{
				this.FilterButton.AddItem(filters.ToString(), filters);
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x0001E1AF File Offset: 0x0001C3AF
		private ActionsWindow ActionsList
		{
			get
			{
				return base.FindControl<ActionsWindow>("ActionsList");
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600057D RID: 1405 RVA: 0x0001E1BC File Offset: 0x0001C3BC
		private BoxContainer SearchContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("SearchContainer");
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x0001E1C9 File Offset: 0x0001C3C9
		public LineEdit SearchBar
		{
			get
			{
				return base.FindControl<LineEdit>("SearchBar");
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600057F RID: 1407 RVA: 0x0001E1D6 File Offset: 0x0001C3D6
		public Button ClearButton
		{
			get
			{
				return base.FindControl<Button>("ClearButton");
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x0001E1E3 File Offset: 0x0001C3E3
		public Label FilterLabel
		{
			get
			{
				return base.FindControl<Label>("FilterLabel");
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x0001E1F0 File Offset: 0x0001C3F0
		public GridContainer ResultsGrid
		{
			get
			{
				return base.FindControl<GridContainer>("ResultsGrid");
			}
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0001E200 File Offset: 0x0001C400
		static void xaml(IServiceProvider A_0, ActionsWindow A_1)
		{
			XamlIlContext.Context<ActionsWindow> context = new XamlIlContext.Context<ActionsWindow>(A_0, null, "resm:Content.Client.UserInterface.Systems.Actions.Windows.ActionsWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Name = "ActionsList";
			context.RobustNameScope.Register("ActionsList", A_1);
			A_1.HorizontalExpand = true;
			A_1.Title = "Actions";
			A_1.VerticalExpand = true;
			A_1.Resizable = true;
			A_1.MinHeight = 300f;
			A_1.MinWidth = 300f;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Name = "SearchContainer";
			Control control = boxContainer2;
			context.RobustNameScope.Register("SearchContainer", control);
			boxContainer2.Orientation = 0;
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "SearchBar";
			control = lineEdit;
			context.RobustNameScope.Register("SearchBar", control);
			lineEdit.Access = new AccessLevel?(0);
			string item = "actionSearchBox";
			lineEdit.StyleClasses.Add(item);
			lineEdit.HorizontalExpand = true;
			lineEdit.PlaceHolder = (string)new LocExtension("ui-actionmenu-search-bar-placeholder-text").ProvideValue();
			control = lineEdit;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "ClearButton";
			control = button;
			context.RobustNameScope.Register("ClearButton", control);
			button.Access = new AccessLevel?(0);
			button.Text = (string)new LocExtension("ui-actionmenu-clear-button").ProvideValue();
			control = button;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "FilterLabel";
			control = label;
			context.RobustNameScope.Register("FilterLabel", control);
			label.Access = new AccessLevel?(0);
			control = label;
			boxContainer.XamlChildren.Add(control);
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.VerticalExpand = true;
			scrollContainer.HorizontalExpand = true;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Name = "ResultsGrid";
			control = gridContainer;
			context.RobustNameScope.Register("ResultsGrid", control);
			gridContainer.Access = new AccessLevel?(0);
			gridContainer.MaxGridWidth = 300f;
			control = gridContainer;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
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

		// Token: 0x06000583 RID: 1411 RVA: 0x0001E51E File Offset: 0x0001C71E
		private static void !XamlIlPopulateTrampoline(ActionsWindow A_0)
		{
			ActionsWindow.Populate:Content.Client.UserInterface.Systems.Actions.Windows.ActionsWindow.xaml(null, A_0);
		}

		// Token: 0x020000C6 RID: 198
		public enum Filters
		{
			// Token: 0x04000282 RID: 642
			Enabled,
			// Token: 0x04000283 RID: 643
			Item,
			// Token: 0x04000284 RID: 644
			Innate,
			// Token: 0x04000285 RID: 645
			Instant,
			// Token: 0x04000286 RID: 646
			Targeted
		}
	}
}
