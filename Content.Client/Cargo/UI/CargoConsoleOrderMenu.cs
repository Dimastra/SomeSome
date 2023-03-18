﻿using System;
using System.Collections.Generic;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Cargo.UI
{
	// Token: 0x02000405 RID: 1029
	[GenerateTypedNameReferences]
	internal sealed class CargoConsoleOrderMenu : DefaultWindow
	{
		// Token: 0x0600194F RID: 6479 RVA: 0x00091448 File Offset: 0x0008F648
		public CargoConsoleOrderMenu()
		{
			CargoConsoleOrderMenu.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<CargoConsoleOrderMenu>(this);
			this.Amount.SetButtons(new List<int>
			{
				-3,
				-2,
				-1
			}, new List<int>
			{
				1,
				2,
				3
			});
			this.Amount.IsValid = ((int n) => n > 0);
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06001950 RID: 6480 RVA: 0x000914D3 File Offset: 0x0008F6D3
		public Label ProductName
		{
			get
			{
				return base.FindControl<Label>("ProductName");
			}
		}

		// Token: 0x17000534 RID: 1332
		// (get) Token: 0x06001951 RID: 6481 RVA: 0x00014240 File Offset: 0x00012440
		public RichTextLabel Description
		{
			get
			{
				return base.FindControl<RichTextLabel>("Description");
			}
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06001952 RID: 6482 RVA: 0x000914E0 File Offset: 0x0008F6E0
		public Label PointCost
		{
			get
			{
				return base.FindControl<Label>("PointCost");
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06001953 RID: 6483 RVA: 0x000914ED File Offset: 0x0008F6ED
		public LineEdit Requester
		{
			get
			{
				return base.FindControl<LineEdit>("Requester");
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06001954 RID: 6484 RVA: 0x000914FA File Offset: 0x0008F6FA
		public LineEdit Reason
		{
			get
			{
				return base.FindControl<LineEdit>("Reason");
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06001955 RID: 6485 RVA: 0x00091507 File Offset: 0x0008F707
		public SpinBox Amount
		{
			get
			{
				return base.FindControl<SpinBox>("Amount");
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001956 RID: 6486 RVA: 0x00091514 File Offset: 0x0008F714
		public Button SubmitButton
		{
			get
			{
				return base.FindControl<Button>("SubmitButton");
			}
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x00091524 File Offset: 0x0008F724
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Cargo.UI.CargoConsoleOrderMenu.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("cargo-console-order-menu-title").ProvideValue();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 2;
			Control control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-product-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "ProductName";
			control = label;
			context.RobustNameScope.Register("ProductName", control);
			label.Access = new AccessLevel?(0);
			control = label;
			gridContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-description-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.Name = "Description";
			control = richTextLabel;
			context.RobustNameScope.Register("Description", control);
			richTextLabel.Access = new AccessLevel?(0);
			richTextLabel.VerticalExpand = true;
			richTextLabel.SetWidth = 350f;
			control = richTextLabel;
			gridContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-cost-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "PointCost";
			control = label2;
			context.RobustNameScope.Register("PointCost", control);
			label2.Access = new AccessLevel?(0);
			control = label2;
			gridContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-requester-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "Requester";
			control = lineEdit;
			context.RobustNameScope.Register("Requester", control);
			lineEdit.Access = new AccessLevel?(0);
			control = lineEdit;
			gridContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-reason-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			LineEdit lineEdit2 = new LineEdit();
			lineEdit2.Name = "Reason";
			control = lineEdit2;
			context.RobustNameScope.Register("Reason", control);
			lineEdit2.Access = new AccessLevel?(0);
			control = lineEdit2;
			gridContainer.XamlChildren.Add(control);
			control = new Label
			{
				Text = (string)new LocExtension("cargo-console-order-menu-amount-label").ProvideValue()
			};
			gridContainer.XamlChildren.Add(control);
			SpinBox spinBox = new SpinBox();
			spinBox.Name = "Amount";
			control = spinBox;
			context.RobustNameScope.Register("Amount", control);
			spinBox.Access = new AccessLevel?(0);
			spinBox.HorizontalExpand = true;
			spinBox.Value = 1;
			control = spinBox;
			gridContainer.XamlChildren.Add(control);
			control = gridContainer;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "SubmitButton";
			control = button;
			context.RobustNameScope.Register("SubmitButton", control);
			button.Access = new AccessLevel?(0);
			button.Text = (string)new LocExtension("cargo-console-order-menu-submit-button").ProvideValue();
			button.TextAlign = 1;
			control = button;
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

		// Token: 0x06001958 RID: 6488 RVA: 0x000919B2 File Offset: 0x0008FBB2
		private static void !XamlIlPopulateTrampoline(CargoConsoleOrderMenu A_0)
		{
			CargoConsoleOrderMenu.Populate:Content.Client.Cargo.UI.CargoConsoleOrderMenu.xaml(null, A_0);
		}
	}
}
