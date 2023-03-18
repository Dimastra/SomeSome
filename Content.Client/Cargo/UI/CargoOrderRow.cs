﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Cargo;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Cargo.UI
{
	// Token: 0x02000407 RID: 1031
	[GenerateTypedNameReferences]
	public sealed class CargoOrderRow : PanelContainer
	{
		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x0600195C RID: 6492 RVA: 0x000919CD File Offset: 0x0008FBCD
		// (set) Token: 0x0600195D RID: 6493 RVA: 0x000919D5 File Offset: 0x0008FBD5
		[Nullable(2)]
		public CargoOrderData Order { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x0600195E RID: 6494 RVA: 0x000919DE File Offset: 0x0008FBDE
		public CargoOrderRow()
		{
			CargoOrderRow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x0600195F RID: 6495 RVA: 0x00023682 File Offset: 0x00021882
		public TextureRect Icon
		{
			get
			{
				return base.FindControl<TextureRect>("Icon");
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06001960 RID: 6496 RVA: 0x000914D3 File Offset: 0x0008F6D3
		public Label ProductName
		{
			get
			{
				return base.FindControl<Label>("ProductName");
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x0000F67B File Offset: 0x0000D87B
		public Label Description
		{
			get
			{
				return base.FindControl<Label>("Description");
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001962 RID: 6498 RVA: 0x000919EC File Offset: 0x0008FBEC
		public Button Approve
		{
			get
			{
				return base.FindControl<Button>("Approve");
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06001963 RID: 6499 RVA: 0x000919F9 File Offset: 0x0008FBF9
		public Button Cancel
		{
			get
			{
				return base.FindControl<Button>("Cancel");
			}
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x00091A08 File Offset: 0x0008FC08
		static void xaml(IServiceProvider A_0, PanelContainer A_1)
		{
			XamlIlContext.Context<PanelContainer> context = new XamlIlContext.Context<PanelContainer>(A_0, null, "resm:Content.Client.Cargo.UI.CargoOrderRow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.HorizontalExpand = true;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.HorizontalExpand = true;
			TextureRect textureRect = new TextureRect();
			textureRect.Name = "Icon";
			Control control = textureRect;
			context.RobustNameScope.Register("Icon", control);
			textureRect.Access = new AccessLevel?(0);
			textureRect.MinSize = new Vector2(32f, 32f);
			textureRect.RectClipContent = true;
			control = textureRect;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.HorizontalExpand = true;
			boxContainer2.VerticalExpand = true;
			Label label = new Label();
			label.Name = "ProductName";
			control = label;
			context.RobustNameScope.Register("ProductName", control);
			label.Access = new AccessLevel?(0);
			label.HorizontalExpand = true;
			string item = "LabelSubText";
			label.StyleClasses.Add(item);
			label.ClipText = true;
			control = label;
			boxContainer2.XamlChildren.Add(control);
			Label label2 = new Label();
			label2.Name = "Description";
			control = label2;
			context.RobustNameScope.Register("Description", control);
			label2.Access = new AccessLevel?(0);
			label2.HorizontalExpand = true;
			item = "LabelSubText";
			label2.StyleClasses.Add(item);
			label2.ClipText = true;
			control = label2;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "Approve";
			control = button;
			context.RobustNameScope.Register("Approve", control);
			button.Access = new AccessLevel?(0);
			button.Text = (string)new LocExtension("cargo-console-menu-cargo-order-row-approve-button").ProvideValue();
			item = "LabelSubText";
			button.StyleClasses.Add(item);
			control = button;
			boxContainer.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "Cancel";
			control = button2;
			context.RobustNameScope.Register("Cancel", control);
			button2.Access = new AccessLevel?(0);
			button2.Text = (string)new LocExtension("cargo-console-menu-cargo-order-row-cancel-button").ProvideValue();
			item = "LabelSubText";
			button2.StyleClasses.Add(item);
			control = button2;
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

		// Token: 0x06001965 RID: 6501 RVA: 0x00091D61 File Offset: 0x0008FF61
		private static void !XamlIlPopulateTrampoline(CargoOrderRow A_0)
		{
			CargoOrderRow.Populate:Content.Client.Cargo.UI.CargoOrderRow.xaml(null, A_0);
		}
	}
}
