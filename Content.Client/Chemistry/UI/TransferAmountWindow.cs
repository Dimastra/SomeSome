﻿using System;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003DF RID: 991
	[GenerateTypedNameReferences]
	public sealed class TransferAmountWindow : DefaultWindow
	{
		// Token: 0x06001874 RID: 6260 RVA: 0x0008D402 File Offset: 0x0008B602
		public TransferAmountWindow()
		{
			TransferAmountWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06001875 RID: 6261 RVA: 0x00061C74 File Offset: 0x0005FE74
		public LineEdit AmountLineEdit
		{
			get
			{
				return base.FindControl<LineEdit>("AmountLineEdit");
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x0004BAAB File Offset: 0x00049CAB
		public Button ApplyButton
		{
			get
			{
				return base.FindControl<Button>("ApplyButton");
			}
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x0008D410 File Offset: 0x0008B610
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Chemistry.UI.TransferAmountWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("ui-transfer-amount-title").ProvideValue();
			A_1.Resizable = false;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(4);
			boxContainer.MinSize = new Vector2(240f, 80f);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "AmountLineEdit";
			Control control = lineEdit;
			context.RobustNameScope.Register("AmountLineEdit", control);
			lineEdit.Access = new AccessLevel?(0);
			lineEdit.HorizontalExpand = true;
			lineEdit.PlaceHolder = (string)new LocExtension("ui-transfer-amount-line-edit-placeholder").ProvideValue();
			control = lineEdit;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "ApplyButton";
			control = button;
			context.RobustNameScope.Register("ApplyButton", control);
			button.Access = new AccessLevel?(0);
			button.Text = (string)new LocExtension("ui-transfer-amount-apply").ProvideValue();
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

		// Token: 0x06001878 RID: 6264 RVA: 0x0008D5FB File Offset: 0x0008B7FB
		private static void !XamlIlPopulateTrampoline(TransferAmountWindow A_0)
		{
			TransferAmountWindow.Populate:Content.Client.Chemistry.UI.TransferAmountWindow.xaml(null, A_0);
		}
	}
}
