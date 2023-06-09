﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Resources;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Controls.FancyTree
{
	// Token: 0x020000E7 RID: 231
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public sealed class FancyTree : Control
	{
		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06000698 RID: 1688 RVA: 0x00022A24 File Offset: 0x00020C24
		// (remove) Token: 0x06000699 RID: 1689 RVA: 0x00022A5C File Offset: 0x00020C5C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<TreeItem> OnSelectedItemChanged;

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x00022A91 File Offset: 0x00020C91
		// (set) Token: 0x0600069B RID: 1691 RVA: 0x00022A99 File Offset: 0x00020C99
		public int? SelectedIndex { get; private set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600069C RID: 1692 RVA: 0x00022AA2 File Offset: 0x00020CA2
		// (set) Token: 0x0600069D RID: 1693 RVA: 0x00022AAA File Offset: 0x00020CAA
		public bool HideEmptyIcon
		{
			get
			{
				return this._hideEmptyIcon;
			}
			set
			{
				this.SetHideEmptyIcon(value);
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x00022AB4 File Offset: 0x00020CB4
		[Nullable(2)]
		public TreeItem SelectedItem
		{
			[NullableContext(2)]
			get
			{
				if (this.SelectedIndex != null)
				{
					return this.Items[this.SelectedIndex.Value];
				}
				return null;
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00022AEC File Offset: 0x00020CEC
		public FancyTree()
		{
			FancyTree.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<FancyTree>(this);
			this.LoadIcons();
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00022B50 File Offset: 0x00020D50
		private void LoadIcons()
		{
			Color color;
			this.IconColor = (base.TryGetStyleProperty<Color>("IconColor", ref color) ? color : Color.White);
			if (!base.TryGetStyleProperty<Texture>("IconExpanded", ref this.IconExpanded))
			{
				this.IconExpanded = this._resCache.GetTexture("/Textures/Interface/Nano/inverted_triangle.svg.png");
			}
			if (!base.TryGetStyleProperty<Texture>("IconCollapsed", ref this.IconCollapsed))
			{
				this.IconCollapsed = this._resCache.GetTexture("/Textures/Interface/Nano/triangle_right.png");
			}
			if (!base.TryGetStyleProperty<Texture>("IconNoChildren", ref this.IconNoChildren))
			{
				this.IconNoChildren = this._resCache.GetTexture("/Textures/Interface/Nano/triangle_right_hollow.svg.png");
			}
			foreach (Control control in this.Body.Children)
			{
				this.RecursiveUpdateIcon((TreeItem)control);
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x00022C48 File Offset: 0x00020E48
		public TreeItem AddItem([Nullable(2)] TreeItem parent = null)
		{
			if (parent != null && parent.Tree != this)
			{
				throw new ArgumentException("Parent must be owned by this tree.", "parent");
			}
			TreeItem item = new TreeItem
			{
				Tree = this,
				Index = this.Items.Count
			};
			this.Items.Add(item);
			item.Icon.SetSize = new ValueTuple<float, float>(16f, 16f);
			item.Button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.OnPressed(item);
			};
			if (parent == null)
			{
				this.Body.AddChild(item);
			}
			else
			{
				item.Padding.MinWidth = parent.Padding.MinWidth + 16f;
				parent.Body.AddChild(item);
			}
			item.UpdateIcon();
			this.QueueRowStyleUpdate();
			return item;
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x00022D54 File Offset: 0x00020F54
		private void OnPressed(TreeItem item)
		{
			int? selectedIndex = this.SelectedIndex;
			int index = item.Index;
			if (selectedIndex.GetValueOrDefault() == index & selectedIndex != null)
			{
				item.SetExpanded(!item.Expanded);
				return;
			}
			this.SetSelectedIndex(new int?(item.Index));
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00022DA4 File Offset: 0x00020FA4
		public void SetSelectedIndex(int? value)
		{
			int? num;
			if (value != null)
			{
				num = value;
				int num2 = 0;
				if (!(num.GetValueOrDefault() < num2 & num != null))
				{
					num = value;
					num2 = this.Items.Count;
					if (!(num.GetValueOrDefault() >= num2 & num != null))
					{
						goto IL_4E;
					}
				}
			}
			value = null;
			IL_4E:
			num = this.SelectedIndex;
			int? num3 = value;
			if (num.GetValueOrDefault() == num3.GetValueOrDefault() & num != null == (num3 != null))
			{
				return;
			}
			TreeItem selectedItem = this.SelectedItem;
			if (selectedItem != null)
			{
				selectedItem.SetSelected(false);
			}
			this.SelectedIndex = value;
			TreeItem selectedItem2 = this.SelectedItem;
			if (selectedItem2 != null)
			{
				selectedItem2.SetSelected(true);
				if (this.AutoExpand && !selectedItem2.Expanded)
				{
					selectedItem2.SetExpanded(true);
				}
			}
			Action<TreeItem> onSelectedItemChanged = this.OnSelectedItemChanged;
			if (onSelectedItemChanged == null)
			{
				return;
			}
			onSelectedItemChanged(selectedItem2);
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00022E80 File Offset: 0x00021080
		public void SetAllExpanded(bool value)
		{
			foreach (Control control in this.Body.Children)
			{
				this.RecursiveSetExpanded((TreeItem)control, value);
			}
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00022EE0 File Offset: 0x000210E0
		public void RecursiveSetExpanded(TreeItem item, bool value)
		{
			item.SetExpanded(value);
			foreach (Control control in item.Body.Children)
			{
				this.RecursiveSetExpanded((TreeItem)control, value);
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x00022F48 File Offset: 0x00021148
		public void Clear()
		{
			foreach (TreeItem treeItem in this.Items)
			{
				treeItem.Dispose();
			}
			this.Items.Clear();
			this.Body.Children.Clear();
			this.SelectedIndex = null;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00022FC4 File Offset: 0x000211C4
		public void QueueRowStyleUpdate()
		{
			this._rowStyleUpdateQueued = true;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00022FD0 File Offset: 0x000211D0
		protected override void FrameUpdate(FrameEventArgs args)
		{
			if (!this._rowStyleUpdateQueued)
			{
				return;
			}
			this._rowStyleUpdateQueued = false;
			int num = 0;
			foreach (Control control in this.Body.Children)
			{
				this.RecursivelyUpdateRowStyle((TreeItem)control, ref num);
			}
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x00023044 File Offset: 0x00021244
		private void RecursivelyUpdateRowStyle(TreeItem item, ref int index)
		{
			if (int.IsOddInteger(index))
			{
				item.Button.RemoveStyleClass("even-row");
				item.Button.AddStyleClass("odd-row");
			}
			else
			{
				item.Button.AddStyleClass("even-row");
				item.Button.RemoveStyleClass("odd-row");
			}
			index++;
			if (!item.Expanded)
			{
				return;
			}
			foreach (Control control in item.Body.Children)
			{
				this.RecursivelyUpdateRowStyle((TreeItem)control, ref index);
			}
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000230FC File Offset: 0x000212FC
		private void SetHideEmptyIcon(bool value)
		{
			if (value == this._hideEmptyIcon)
			{
				return;
			}
			this._hideEmptyIcon = value;
			foreach (Control control in this.Body.Children)
			{
				this.RecursiveUpdateIcon((TreeItem)control);
			}
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0002316C File Offset: 0x0002136C
		private void RecursiveUpdateIcon(TreeItem item)
		{
			item.UpdateIcon();
			foreach (Control control in item.Body.Children)
			{
				this.RecursiveUpdateIcon((TreeItem)control);
			}
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x000231D0 File Offset: 0x000213D0
		protected override void StylePropertiesChanged()
		{
			this.LoadIcons();
			Color color;
			this.LineColor = (base.TryGetStyleProperty<Color>("LineColor", ref color) ? color : Color.White);
			int num;
			this.LineWidth = (base.TryGetStyleProperty<int>("LineWidth", ref num) ? num : 2);
			base.StylePropertiesChanged();
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0002321F File Offset: 0x0002141F
		[Nullable(0)]
		public BoxContainer Body
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<BoxContainer>("Body");
			}
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0002322C File Offset: 0x0002142C
		static void xaml(IServiceProvider A_0, FancyTree A_1)
		{
			XamlIlContext.Context<FancyTree> context = new XamlIlContext.Context<FancyTree>(A_0, null, "resm:Content.Client.UserInterface.Controls.FancyTree.FancyTree.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.ReturnMeasure = true;
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.Name = "Body";
			Control control = boxContainer;
			context.RobustNameScope.Register("Body", control);
			boxContainer.Access = new AccessLevel?(0);
			boxContainer.Margin = new Thickness(2f, 2f, 2f, 2f);
			control = boxContainer;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00023340 File Offset: 0x00021540
		private static void !XamlIlPopulateTrampoline(FancyTree A_0)
		{
			FancyTree.Populate:Content.Client.UserInterface.Controls.FancyTree.FancyTree.xaml(null, A_0);
		}

		// Token: 0x040002F7 RID: 759
		[Dependency]
		private readonly IResourceCache _resCache;

		// Token: 0x040002F8 RID: 760
		public const string StylePropertyLineWidth = "LineWidth";

		// Token: 0x040002F9 RID: 761
		public const string StylePropertyLineColor = "LineColor";

		// Token: 0x040002FA RID: 762
		public const string StylePropertyIconColor = "IconColor";

		// Token: 0x040002FB RID: 763
		public const string StylePropertyIconExpanded = "IconExpanded";

		// Token: 0x040002FC RID: 764
		public const string StylePropertyIconCollapsed = "IconCollapsed";

		// Token: 0x040002FD RID: 765
		public const string StylePropertyIconNoChildren = "IconNoChildren";

		// Token: 0x040002FE RID: 766
		public readonly List<TreeItem> Items = new List<TreeItem>();

		// Token: 0x04000301 RID: 769
		private bool _rowStyleUpdateQueued = true;

		// Token: 0x04000302 RID: 770
		public bool DrawLines = true;

		// Token: 0x04000303 RID: 771
		public Color LineColor = Color.White;

		// Token: 0x04000304 RID: 772
		public Color IconColor = Color.White;

		// Token: 0x04000305 RID: 773
		public int LineWidth = 2;

		// Token: 0x04000306 RID: 774
		public const int Indentation = 16;

		// Token: 0x04000307 RID: 775
		public const string DefaultIconExpanded = "/Textures/Interface/Nano/inverted_triangle.svg.png";

		// Token: 0x04000308 RID: 776
		public const string DefaultIconCollapsed = "/Textures/Interface/Nano/triangle_right.png";

		// Token: 0x04000309 RID: 777
		public const string DefaultIconNoChildren = "/Textures/Interface/Nano/triangle_right_hollow.svg.png";

		// Token: 0x0400030A RID: 778
		[Nullable(2)]
		public Texture IconExpanded;

		// Token: 0x0400030B RID: 779
		[Nullable(2)]
		public Texture IconCollapsed;

		// Token: 0x0400030C RID: 780
		[Nullable(2)]
		public Texture IconNoChildren;

		// Token: 0x0400030D RID: 781
		private bool _hideEmptyIcon;

		// Token: 0x0400030E RID: 782
		public bool AutoExpand = true;
	}
}
