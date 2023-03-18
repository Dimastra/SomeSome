using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000D9 RID: 217
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ListContainer : Control
	{
		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00020C2F File Offset: 0x0001EE2F
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x00020C37 File Offset: 0x0001EE37
		public int? SeparationOverride { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00020C40 File Offset: 0x0001EE40
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00020C4B File Offset: 0x0001EE4B
		public bool Group
		{
			get
			{
				return this._buttonGroup != null;
			}
			set
			{
				this._buttonGroup = (value ? new ButtonGroup() : null);
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00020C5E File Offset: 0x0001EE5E
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x00020C66 File Offset: 0x0001EE66
		public bool Toggle { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00020C6F File Offset: 0x0001EE6F
		public IReadOnlyList<ListData> Data
		{
			get
			{
				return this._data;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x00020C77 File Offset: 0x0001EE77
		// (set) Token: 0x06000610 RID: 1552 RVA: 0x00020C7F File Offset: 0x0001EE7F
		public int ScrollSpeedY { get; set; } = 50;

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x00020C88 File Offset: 0x0001EE88
		private int ActualSeparation
		{
			get
			{
				int result;
				if (base.TryGetStyleProperty<int>("separation", ref result))
				{
					return result;
				}
				int? separationOverride = this.SeparationOverride;
				if (separationOverride == null)
				{
					return 3;
				}
				return separationOverride.GetValueOrDefault();
			}
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00020CC0 File Offset: 0x0001EEC0
		public ListContainer()
		{
			base.HorizontalExpand = true;
			base.VerticalExpand = true;
			base.RectClipContent = true;
			base.MouseFilter = 1;
			this._vScrollBar = new VScrollBar
			{
				HorizontalExpand = false,
				HorizontalAlignment = 3
			};
			base.AddChild(this._vScrollBar);
			this._vScrollBar.OnValueChanged += this.ScrollValueChanged;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00020D4C File Offset: 0x0001EF4C
		public void PopulateList(IReadOnlyList<ListData> data)
		{
			if (this._itemHeight != 0f)
			{
				List<ListData> data2 = this._data;
				if (data2 == null || data2.Count != 0)
				{
					goto IL_70;
				}
			}
			if (data.Count > 0)
			{
				ListContainerButton listContainerButton = new ListContainerButton(data[0]);
				Action<ListData, ListContainerButton> generateItem = this.GenerateItem;
				if (generateItem != null)
				{
					generateItem(data[0], listContainerButton);
				}
				listContainerButton.Measure(Vector2.Infinity);
				this._itemHeight = listContainerButton.DesiredSize.Y;
				listContainerButton.Dispose();
			}
			IL_70:
			foreach (ListContainerButton listContainerButton2 in this._buttons.Values)
			{
				listContainerButton2.Dispose();
			}
			this._buttons.Clear();
			this._data = data.ToList<ListData>();
			this._updateChildren = true;
			base.InvalidateArrange();
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00020E38 File Offset: 0x0001F038
		public void DirtyList()
		{
			this._updateChildren = true;
			base.InvalidateArrange();
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00020E48 File Offset: 0x0001F048
		public void Select(ListData data)
		{
			if (!this._data.Contains(data))
			{
				return;
			}
			ListContainerButton listContainerButton;
			if (this._buttons.TryGetValue(data, out listContainerButton) && this.Toggle)
			{
				listContainerButton.Pressed = true;
			}
			this._selected = data;
			if (listContainerButton == null)
			{
				listContainerButton = new ListContainerButton(data);
			}
			this.OnItemPressed(new BaseButton.ButtonEventArgs(listContainerButton, new GUIBoundKeyEventArgs(EngineKeyFunctions.UIClick, 0, new ScreenCoordinates(0f, 0f, WindowId.Main), true, Vector2.Zero, Vector2.Zero)));
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00020ECC File Offset: 0x0001F0CC
		private void OnItemPressed(BaseButton.ButtonEventArgs args)
		{
			ListContainerButton listContainerButton = args.Button as ListContainerButton;
			if (listContainerButton == null)
			{
				return;
			}
			this._selected = listContainerButton.Data;
			Action<BaseButton.ButtonEventArgs, ListData> itemPressed = this.ItemPressed;
			if (itemPressed == null)
			{
				return;
			}
			itemPressed(args, listContainerButton.Data);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00020F0C File Offset: 0x0001F10C
		private Vector2 GetScrollValue()
		{
			float num = this._vScrollBar.Value;
			if (!this._vScrollBar.Visible)
			{
				num = 0f;
			}
			return new Vector2(0f, num);
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00020F44 File Offset: 0x0001F144
		protected override Vector2 ArrangeOverride(Vector2 finalSize)
		{
			float totalHeight = this._totalHeight;
			float x = this._vScrollBar.DesiredSize.X;
			Vector2 vector = finalSize;
			float num;
			float num2;
			vector.Deconstruct(ref num, ref num2);
			float num3 = num;
			float num4 = num2;
			try
			{
				this._suppressScrollValueChanged = true;
				if (num4 < totalHeight)
				{
					num3 -= x;
				}
				if (num4 < totalHeight)
				{
					this._vScrollBar.Visible = true;
					this._vScrollBar.Page = num4;
					this._vScrollBar.MaxValue = totalHeight;
				}
				else
				{
					this._vScrollBar.Visible = false;
				}
			}
			finally
			{
				this._suppressScrollValueChanged = false;
			}
			if (this._vScrollBar.Visible)
			{
				this._vScrollBar.Arrange(UIBox2.FromDimensions(Vector2.Zero, finalSize));
			}
			Vector2 scrollValue = this.GetScrollValue();
			int topIndex = this._topIndex;
			this._topIndex = (int)((scrollValue.Y + (float)this.ActualSeparation) / (this._itemHeight + (float)this.ActualSeparation));
			if (this._topIndex != topIndex)
			{
				this._updateChildren = true;
			}
			int bottomIndex = this._bottomIndex;
			this._bottomIndex = (int)Math.Ceiling((double)((scrollValue.Y + num4) / (this._itemHeight + (float)this.ActualSeparation)));
			this._bottomIndex = Math.Min(this._bottomIndex, this._data.Count);
			if (this._bottomIndex != bottomIndex)
			{
				this._updateChildren = true;
			}
			if (this._updateChildren)
			{
				this._updateChildren = false;
				Dictionary<ListData, ListContainerButton> dictionary = new Dictionary<ListData, ListContainerButton>(this._buttons);
				foreach (Control control in base.Children.ToArray<Control>())
				{
					if (control != this._vScrollBar)
					{
						base.RemoveChild(control);
					}
				}
				if (this._data.Count > 0)
				{
					for (int j = this._topIndex; j < this._bottomIndex; j++)
					{
						ListData listData = this._data[j];
						ListContainerButton listContainerButton;
						if (this._buttons.TryGetValue(listData, out listContainerButton))
						{
							dictionary.Remove(listData);
						}
						else
						{
							listContainerButton = new ListContainerButton(listData);
							listContainerButton.OnPressed += this.OnItemPressed;
							listContainerButton.ToggleMode = this.Toggle;
							listContainerButton.Group = this._buttonGroup;
							Action<ListData, ListContainerButton> generateItem = this.GenerateItem;
							if (generateItem != null)
							{
								generateItem(listData, listContainerButton);
							}
							this._buttons.Add(listData, listContainerButton);
							if (this.Toggle && listData == this._selected)
							{
								listContainerButton.Pressed = true;
							}
						}
						base.AddChild(listContainerButton);
						listContainerButton.Measure(finalSize);
					}
				}
				foreach (KeyValuePair<ListData, ListContainerButton> keyValuePair in dictionary)
				{
					ListData listData2;
					ListContainerButton listContainerButton2;
					keyValuePair.Deconstruct(out listData2, out listContainerButton2);
					ListData key = listData2;
					Control control2 = listContainerButton2;
					this._buttons.Remove(key);
					control2.Dispose();
				}
				this._vScrollBar.SetPositionLast();
			}
			int num5 = (int)(num3 * this.UIScale);
			int num6 = (int)((float)this.ActualSeparation * this.UIScale);
			int num7 = (int)(-(int)((scrollValue.Y - (float)this._topIndex * (this._itemHeight + (float)this.ActualSeparation)) * this.UIScale));
			bool flag = true;
			foreach (Control control3 in base.Children)
			{
				if (control3 != this._vScrollBar)
				{
					if (!flag)
					{
						num7 += num6;
					}
					flag = false;
					int y = control3.DesiredPixelSize.Y;
					UIBox2i uibox2i;
					uibox2i..ctor(0, num7, num5, num7 + y);
					control3.ArrangePixel(uibox2i);
					num7 += y;
				}
			}
			return finalSize;
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00021314 File Offset: 0x0001F514
		protected override Vector2 MeasureOverride(Vector2 availableSize)
		{
			this._vScrollBar.Measure(availableSize);
			availableSize.X -= this._vScrollBar.DesiredSize.X;
			Vector2 vector;
			vector..ctor(availableSize.X, float.PositiveInfinity);
			Vector2 vector2 = Vector2.Zero;
			foreach (Control control in base.Children)
			{
				control.Measure(vector);
				if (control != this._vScrollBar)
				{
					vector2 = Vector2.ComponentMax(vector2, control.DesiredSize);
				}
			}
			if (this._itemHeight == 0f && vector2.Y != 0f)
			{
				this._itemHeight = vector2.Y;
			}
			this._totalHeight = this._itemHeight * (float)this._data.Count + (float)(this.ActualSeparation * (this._data.Count - 1));
			return new Vector2(vector2.X, 0f);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00021424 File Offset: 0x0001F624
		private void ScrollValueChanged(Range _)
		{
			if (this._suppressScrollValueChanged)
			{
				return;
			}
			base.InvalidateArrange();
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00021435 File Offset: 0x0001F635
		protected override void MouseWheel(GUIMouseWheelEventArgs args)
		{
			base.MouseWheel(args);
			this._vScrollBar.ValueTarget -= args.Delta.Y * (float)this.ScrollSpeedY;
			args.Handle();
		}

		// Token: 0x040002AF RID: 687
		public const string StylePropertySeparation = "separation";

		// Token: 0x040002B0 RID: 688
		public const string StyleClassListContainerButton = "list-container-button";

		// Token: 0x040002B3 RID: 691
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public Action<ListData, ListContainerButton> GenerateItem;

		// Token: 0x040002B4 RID: 692
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public Action<BaseButton.ButtonEventArgs, ListData> ItemPressed;

		// Token: 0x040002B5 RID: 693
		private const int DefaultSeparation = 3;

		// Token: 0x040002B6 RID: 694
		private readonly VScrollBar _vScrollBar;

		// Token: 0x040002B7 RID: 695
		private readonly Dictionary<ListData, ListContainerButton> _buttons = new Dictionary<ListData, ListContainerButton>();

		// Token: 0x040002B8 RID: 696
		private List<ListData> _data = new List<ListData>();

		// Token: 0x040002B9 RID: 697
		[Nullable(2)]
		private ListData _selected;

		// Token: 0x040002BA RID: 698
		private float _itemHeight;

		// Token: 0x040002BB RID: 699
		private float _totalHeight;

		// Token: 0x040002BC RID: 700
		private int _topIndex;

		// Token: 0x040002BD RID: 701
		private int _bottomIndex;

		// Token: 0x040002BE RID: 702
		private bool _updateChildren;

		// Token: 0x040002BF RID: 703
		private bool _suppressScrollValueChanged;

		// Token: 0x040002C0 RID: 704
		[Nullable(2)]
		private ButtonGroup _buttonGroup;
	}
}
