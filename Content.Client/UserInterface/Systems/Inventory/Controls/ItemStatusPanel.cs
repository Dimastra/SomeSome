﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.IoC;
using Content.Client.Items;
using Content.Client.Resources;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Systems.Inventory.Controls
{
	// Token: 0x0200007D RID: 125
	[GenerateTypedNameReferences]
	public sealed class ItemStatusPanel : BoxContainer
	{
		// Token: 0x060002A3 RID: 675 RVA: 0x000113EC File Offset: 0x0000F5EC
		public ItemStatusPanel()
		{
			ItemStatusPanel.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<ItemStatusPanel>(this);
			this.SetSide(HandLocation.Middle);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x00011408 File Offset: 0x0000F608
		public void SetSide(HandLocation location)
		{
			string path;
			StyleBox.Margin margin;
			StyleBox.Margin margin2;
			Label.AlignMode align;
			switch (location)
			{
			case HandLocation.Left:
				path = "/Textures/Interface/Nano/item_status_right.svg.96dpi.png";
				margin = 9;
				margin2 = 6;
				align = 2;
				break;
			case HandLocation.Middle:
				path = "/Textures/Interface/Nano/item_status_middle.svg.96dpi.png";
				margin = 5;
				margin2 = 10;
				align = 0;
				break;
			case HandLocation.Right:
				path = "/Textures/Interface/Nano/item_status_left.svg.96dpi.png";
				margin = 5;
				margin2 = 10;
				align = 0;
				break;
			default:
				throw new ArgumentOutOfRangeException("location", location, null);
			}
			StyleBoxTexture styleBoxTexture = (StyleBoxTexture)this.Panel.PanelOverride;
			styleBoxTexture.Texture = StaticIoC.ResC.GetTexture(path);
			styleBoxTexture.SetPatchMargin(margin2, 2f);
			styleBoxTexture.SetPatchMargin(margin, 13f);
			this.ItemNameLabel.Align = align;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x000114AC File Offset: 0x0000F6AC
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this.UpdateItemName();
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x000114BC File Offset: 0x0000F6BC
		public void Update(EntityUid? entity)
		{
			if (entity == null)
			{
				this.ClearOldStatus();
				this._entity = null;
				this.Panel.Visible = false;
				return;
			}
			if (entity != this._entity)
			{
				this._entity = new EntityUid?(entity.Value);
				this.BuildNewEntityStatus();
				this.UpdateItemName();
			}
			this.Panel.Visible = true;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00011558 File Offset: 0x0000F758
		private void UpdateItemName()
		{
			if (this._entity == null)
			{
				return;
			}
			MetaDataComponent metaDataComponent;
			if (!this._entityManager.TryGetComponent<MetaDataComponent>(this._entity, ref metaDataComponent) || metaDataComponent.Deleted)
			{
				this.Update(null);
				return;
			}
			HandVirtualItemComponent handVirtualItemComponent;
			if (this._entityManager.TryGetComponent<HandVirtualItemComponent>(this._entity, ref handVirtualItemComponent) && this._entityManager.EntityExists(handVirtualItemComponent.BlockingEntity))
			{
				this.ItemNameLabel.Text = Identity.Name(handVirtualItemComponent.BlockingEntity, this._entityManager, null);
				return;
			}
			this.ItemNameLabel.Text = Identity.Name(this._entity.Value, this._entityManager, null);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00011618 File Offset: 0x0000F818
		private void ClearOldStatus()
		{
			this.StatusContents.RemoveAllChildren();
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00011628 File Offset: 0x0000F828
		private void BuildNewEntityStatus()
		{
			this.ClearOldStatus();
			ItemStatusCollectMessage itemStatusCollectMessage = new ItemStatusCollectMessage();
			this._entityManager.EventBus.RaiseLocalEvent<ItemStatusCollectMessage>(this._entity.Value, itemStatusCollectMessage, true);
			foreach (Control control in itemStatusCollectMessage.Controls)
			{
				this.StatusContents.AddChild(control);
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002AA RID: 682 RVA: 0x000116AC File Offset: 0x0000F8AC
		private PanelContainer Panel
		{
			get
			{
				return base.FindControl<PanelContainer>("Panel");
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002AB RID: 683 RVA: 0x000116B9 File Offset: 0x0000F8B9
		private BoxContainer StatusContents
		{
			get
			{
				return base.FindControl<BoxContainer>("StatusContents");
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002AC RID: 684 RVA: 0x000116C6 File Offset: 0x0000F8C6
		private Label ItemNameLabel
		{
			get
			{
				return base.FindControl<Label>("ItemNameLabel");
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x000116D4 File Offset: 0x0000F8D4
		static void xaml(IServiceProvider A_0, ItemStatusPanel A_1)
		{
			XamlIlContext.Context<ItemStatusPanel> context = new XamlIlContext.Context<ItemStatusPanel>(A_0, null, "resm:Content.Client.UserInterface.Systems.Inventory.Controls.ItemStatusPanel.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.VerticalAlignment = 3;
			A_1.HorizontalAlignment = 2;
			A_1.MinSize = new Vector2(150f, 0f);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.Name = "Panel";
			Control control = panelContainer;
			context.RobustNameScope.Register("Panel", control);
			panelContainer.ModulateSelfOverride = new Color?(Color.FromXaml("#FFFFFFE6"));
			panelContainer.HorizontalExpand = true;
			panelContainer.PanelOverride = new StyleBoxTexture
			{
				ContentMarginLeftOverride = new float?(6f),
				ContentMarginRightOverride = new float?(6f),
				ContentMarginTopOverride = new float?(4f),
				ContentMarginBottomOverride = new float?(4f)
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(0);
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Name = "StatusContents";
			control = boxContainer2;
			context.RobustNameScope.Register("StatusContents", control);
			boxContainer2.Orientation = 1;
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "ItemNameLabel";
			control = label;
			context.RobustNameScope.Register("ItemNameLabel", control);
			label.ClipText = true;
			string item = "ItemStatus";
			label.StyleClasses.Add(item);
			control = label;
			boxContainer.XamlChildren.Add(control);
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

		// Token: 0x060002AE RID: 686 RVA: 0x00011900 File Offset: 0x0000FB00
		private static void !XamlIlPopulateTrampoline(ItemStatusPanel A_0)
		{
			ItemStatusPanel.Populate:Content.Client.UserInterface.Systems.Inventory.Controls.ItemStatusPanel.xaml(null, A_0);
		}

		// Token: 0x04000171 RID: 369
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000172 RID: 370
		[ViewVariables]
		private EntityUid? _entity;
	}
}
