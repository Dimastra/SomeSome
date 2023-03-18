﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Cargo.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Cargo.UI
{
	// Token: 0x02000408 RID: 1032
	[GenerateTypedNameReferences]
	public sealed class CargoProductRow : PanelContainer
	{
		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06001966 RID: 6502 RVA: 0x00091D6A File Offset: 0x0008FF6A
		// (set) Token: 0x06001967 RID: 6503 RVA: 0x00091D72 File Offset: 0x0008FF72
		[Nullable(2)]
		public CargoProductPrototype Product { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x06001968 RID: 6504 RVA: 0x00091D7B File Offset: 0x0008FF7B
		public CargoProductRow()
		{
			CargoProductRow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x00091D89 File Offset: 0x0008FF89
		public Button MainButton
		{
			get
			{
				return base.FindControl<Button>("MainButton");
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x0600196A RID: 6506 RVA: 0x00023682 File Offset: 0x00021882
		public TextureRect Icon
		{
			get
			{
				return base.FindControl<TextureRect>("Icon");
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x0600196B RID: 6507 RVA: 0x000914D3 File Offset: 0x0008F6D3
		public Label ProductName
		{
			get
			{
				return base.FindControl<Label>("ProductName");
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x0600196C RID: 6508 RVA: 0x000914E0 File Offset: 0x0008F6E0
		public Label PointCost
		{
			get
			{
				return base.FindControl<Label>("PointCost");
			}
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x00091D98 File Offset: 0x0008FF98
		static void xaml(IServiceProvider A_0, PanelContainer A_1)
		{
			XamlIlContext.Context<PanelContainer> context = new XamlIlContext.Context<PanelContainer>(A_0, null, "resm:Content.Client.Cargo.UI.CargoProductRow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.HorizontalExpand = true;
			Button button = new Button();
			button.Name = "MainButton";
			Control control = button;
			context.RobustNameScope.Register("MainButton", control);
			button.ToolTip = "";
			button.Access = new AccessLevel?(0);
			button.HorizontalExpand = true;
			button.VerticalExpand = true;
			control = button;
			A_1.XamlChildren.Add(control);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.HorizontalExpand = true;
			TextureRect textureRect = new TextureRect();
			textureRect.Name = "Icon";
			control = textureRect;
			context.RobustNameScope.Register("Icon", control);
			textureRect.Access = new AccessLevel?(0);
			textureRect.MinSize = new Vector2(32f, 32f);
			textureRect.RectClipContent = true;
			control = textureRect;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "ProductName";
			control = label;
			context.RobustNameScope.Register("ProductName", control);
			label.Access = new AccessLevel?(0);
			label.HorizontalExpand = true;
			control = label;
			boxContainer.XamlChildren.Add(control);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromXaml("#252525")
			};
			Label label2 = new Label();
			label2.Name = "PointCost";
			control = label2;
			context.RobustNameScope.Register("PointCost", control);
			label2.Access = new AccessLevel?(0);
			label2.MinSize = new Vector2(52f, 32f);
			label2.Align = 2;
			control = label2;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
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

		// Token: 0x0600196E RID: 6510 RVA: 0x00092024 File Offset: 0x00090224
		private static void !XamlIlPopulateTrampoline(CargoProductRow A_0)
		{
			CargoProductRow.Populate:Content.Client.Cargo.UI.CargoProductRow.xaml(null, A_0);
		}
	}
}
