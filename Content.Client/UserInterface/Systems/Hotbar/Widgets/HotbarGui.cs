﻿using System;
using CompiledRobustXaml;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.UserInterface.Systems.Hotbar.Widgets
{
	// Token: 0x02000080 RID: 128
	[GenerateTypedNameReferences]
	public sealed class HotbarGui : UIWidget
	{
		// Token: 0x060002B6 RID: 694 RVA: 0x00011ABC File Offset: 0x0000FCBC
		public HotbarGui()
		{
			HotbarGui.!XamlIlPopulateTrampoline(this);
			this.StatusPanel.Update(null);
			base.UserInterfaceManager.GetUIController<HotbarUIController>().Setup(this.HandContainer, this.InventoryHotbar, this.StatusPanel);
			LayoutContainer.SetGrowVertical(this, 1);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00011B12 File Offset: 0x0000FD12
		public void UpdatePanelEntity(EntityUid? entity)
		{
			this.StatusPanel.Update(entity);
			if (entity == null)
			{
				this.StatusPanel.Visible = false;
				return;
			}
			this.StatusPanel.Visible = true;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002B8 RID: 696 RVA: 0x00011B42 File Offset: 0x0000FD42
		private HotbarGui HotbarInterface
		{
			get
			{
				return base.FindControl<HotbarGui>("HotbarInterface");
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x00011B4F File Offset: 0x0000FD4F
		private ItemStatusPanel StatusPanel
		{
			get
			{
				return base.FindControl<ItemStatusPanel>("StatusPanel");
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002BA RID: 698 RVA: 0x00011B5C File Offset: 0x0000FD5C
		public ItemSlotButtonContainer InventoryHotbar
		{
			get
			{
				return base.FindControl<ItemSlotButtonContainer>("InventoryHotbar");
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002BB RID: 699 RVA: 0x00011B69 File Offset: 0x0000FD69
		private BoxContainer Hotbar
		{
			get
			{
				return base.FindControl<BoxContainer>("Hotbar");
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00011B76 File Offset: 0x0000FD76
		private ItemSlotButtonContainer SecondHotbar
		{
			get
			{
				return base.FindControl<ItemSlotButtonContainer>("SecondHotbar");
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002BD RID: 701 RVA: 0x00011B83 File Offset: 0x0000FD83
		public HandsContainer HandContainer
		{
			get
			{
				return base.FindControl<HandsContainer>("HandContainer");
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00011B90 File Offset: 0x0000FD90
		private ItemSlotButtonContainer MainHotbar
		{
			get
			{
				return base.FindControl<ItemSlotButtonContainer>("MainHotbar");
			}
		}

		// Token: 0x060002BF RID: 703 RVA: 0x00011BA0 File Offset: 0x0000FDA0
		static void xaml(IServiceProvider A_0, HotbarGui A_1)
		{
			XamlIlContext.Context<HotbarGui> context = new XamlIlContext.Context<HotbarGui>(A_0, null, "resm:Content.Client.UserInterface.Systems.Hotbar.Widgets.HotbarGui.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Name = "HotbarInterface";
			context.RobustNameScope.Register("HotbarInterface", A_1);
			A_1.VerticalExpand = false;
			A_1.VerticalAlignment = 3;
			A_1.Orientation = 1;
			A_1.HorizontalAlignment = 2;
			ItemStatusPanel itemStatusPanel = new ItemStatusPanel();
			itemStatusPanel.Name = "StatusPanel";
			Control control = itemStatusPanel;
			context.RobustNameScope.Register("StatusPanel", control);
			itemStatusPanel.Visible = false;
			itemStatusPanel.HorizontalAlignment = 2;
			control = itemStatusPanel;
			A_1.XamlChildren.Add(control);
			ItemSlotButtonContainer itemSlotButtonContainer = new ItemSlotButtonContainer();
			itemSlotButtonContainer.Name = "InventoryHotbar";
			control = itemSlotButtonContainer;
			context.RobustNameScope.Register("InventoryHotbar", control);
			itemSlotButtonContainer.Access = new AccessLevel?(0);
			itemSlotButtonContainer.Visible = false;
			itemSlotButtonContainer.Columns = 10;
			itemSlotButtonContainer.SlotGroup = "Default";
			itemSlotButtonContainer.ExpandBackwards = true;
			itemSlotButtonContainer.VerticalExpand = true;
			itemSlotButtonContainer.HorizontalAlignment = 2;
			itemSlotButtonContainer.VerticalAlignment = 3;
			control = itemSlotButtonContainer;
			A_1.XamlChildren.Add(control);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 0;
			boxContainer.Name = "Hotbar";
			control = boxContainer;
			context.RobustNameScope.Register("Hotbar", control);
			ItemSlotButtonContainer itemSlotButtonContainer2 = new ItemSlotButtonContainer();
			itemSlotButtonContainer2.Name = "SecondHotbar";
			control = itemSlotButtonContainer2;
			context.RobustNameScope.Register("SecondHotbar", control);
			itemSlotButtonContainer2.SlotGroup = "SecondHotbar";
			itemSlotButtonContainer2.VerticalAlignment = 3;
			itemSlotButtonContainer2.HorizontalAlignment = 3;
			itemSlotButtonContainer2.VerticalExpand = false;
			itemSlotButtonContainer2.Columns = 6;
			itemSlotButtonContainer2.HorizontalExpand = true;
			control = itemSlotButtonContainer2;
			boxContainer.XamlChildren.Add(control);
			HandsContainer handsContainer = new HandsContainer();
			handsContainer.Name = "HandContainer";
			control = handsContainer;
			context.RobustNameScope.Register("HandContainer", control);
			handsContainer.Access = new AccessLevel?(0);
			handsContainer.HorizontalAlignment = 2;
			handsContainer.ColumnLimit = 6;
			control = handsContainer;
			boxContainer.XamlChildren.Add(control);
			ItemSlotButtonContainer itemSlotButtonContainer3 = new ItemSlotButtonContainer();
			itemSlotButtonContainer3.Name = "MainHotbar";
			control = itemSlotButtonContainer3;
			context.RobustNameScope.Register("MainHotbar", control);
			itemSlotButtonContainer3.SlotGroup = "MainHotbar";
			itemSlotButtonContainer3.VerticalExpand = false;
			itemSlotButtonContainer3.HorizontalAlignment = 1;
			itemSlotButtonContainer3.VerticalAlignment = 3;
			itemSlotButtonContainer3.HorizontalExpand = true;
			itemSlotButtonContainer3.Columns = 6;
			control = itemSlotButtonContainer3;
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

		// Token: 0x060002C0 RID: 704 RVA: 0x00011F0D File Offset: 0x0001010D
		private static void !XamlIlPopulateTrampoline(HotbarGui A_0)
		{
			HotbarGui.Populate:Content.Client.UserInterface.Systems.Hotbar.Widgets.HotbarGui.xaml(null, A_0);
		}
	}
}