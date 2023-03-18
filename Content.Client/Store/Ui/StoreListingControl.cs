﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Store.Ui
{
	// Token: 0x0200011A RID: 282
	[GenerateTypedNameReferences]
	public sealed class StoreListingControl : Control
	{
		// Token: 0x060007C8 RID: 1992 RVA: 0x0002D1E0 File Offset: 0x0002B3E0
		[NullableContext(1)]
		public StoreListingControl(string itemName, string itemDescription, string price, bool canBuy, [Nullable(2)] Texture texture = null)
		{
			StoreListingControl.!XamlIlPopulateTrampoline(this);
			this.StoreItemName.Text = itemName;
			this.StoreItemDescription.SetMessage(itemDescription);
			this.StoreItemBuyButton.Text = price;
			this.StoreItemBuyButton.Disabled = !canBuy;
			this.StoreItemTexture.Texture = texture;
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060007C9 RID: 1993 RVA: 0x0002D23A File Offset: 0x0002B43A
		private Label StoreItemName
		{
			get
			{
				return base.FindControl<Label>("StoreItemName");
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x0002D247 File Offset: 0x0002B447
		public Button StoreItemBuyButton
		{
			get
			{
				return base.FindControl<Button>("StoreItemBuyButton");
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060007CB RID: 1995 RVA: 0x0002D254 File Offset: 0x0002B454
		private TextureRect StoreItemTexture
		{
			get
			{
				return base.FindControl<TextureRect>("StoreItemTexture");
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x0002D261 File Offset: 0x0002B461
		private RichTextLabel StoreItemDescription
		{
			get
			{
				return base.FindControl<RichTextLabel>("StoreItemDescription");
			}
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0002D270 File Offset: 0x0002B470
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Store.Ui.StoreListingControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Margin = new Thickness(8f, 8f, 8f, 8f);
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Label label = new Label();
			label.Name = "StoreItemName";
			Control control = label;
			context.RobustNameScope.Register("StoreItemName", control);
			label.HorizontalExpand = true;
			control = label;
			boxContainer2.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "StoreItemBuyButton";
			control = button;
			context.RobustNameScope.Register("StoreItemBuyButton", control);
			button.MinWidth = 64f;
			button.HorizontalAlignment = 3;
			button.Access = new AccessLevel?(0);
			control = button;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			PanelContainer panelContainer = new PanelContainer();
			string item = "HighDivider";
			panelContainer.StyleClasses.Add(item);
			control = panelContainer;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.HorizontalExpand = true;
			boxContainer3.Orientation = 0;
			TextureRect textureRect = new TextureRect();
			textureRect.Name = "StoreItemTexture";
			control = textureRect;
			context.RobustNameScope.Register("StoreItemTexture", control);
			textureRect.Margin = new Thickness(0f, 0f, 0f, 0f);
			textureRect.MinSize = new Vector2(48f, 48f);
			textureRect.Stretch = 7;
			control = textureRect;
			boxContainer3.XamlChildren.Add(control);
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.Name = "StoreItemDescription";
			control = richTextLabel;
			context.RobustNameScope.Register("StoreItemDescription", control);
			control = richTextLabel;
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

		// Token: 0x060007CE RID: 1998 RVA: 0x0002D517 File Offset: 0x0002B717
		private static void !XamlIlPopulateTrampoline(StoreListingControl A_0)
		{
			StoreListingControl.Populate:Content.Client.Store.Ui.StoreListingControl.xaml(null, A_0);
		}
	}
}
