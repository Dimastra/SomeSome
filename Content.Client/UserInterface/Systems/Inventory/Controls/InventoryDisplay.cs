using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Inventory.Controls
{
	// Token: 0x02000079 RID: 121
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InventoryDisplay : LayoutContainer
	{
		// Token: 0x06000288 RID: 648 RVA: 0x00010E38 File Offset: 0x0000F038
		public InventoryDisplay()
		{
			this.resizer = new Control();
			base.AddChild(this.resizer);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00010E64 File Offset: 0x0000F064
		public SlotControl AddButton(SlotControl newButton, Vector2i buttonOffset)
		{
			base.AddChild(newButton);
			base.HorizontalExpand = true;
			base.VerticalExpand = true;
			base.InheritChildMeasure = true;
			if (!this._buttons.TryAdd(newButton.SlotName, new ValueTuple<SlotControl, Vector2i>(newButton, buttonOffset)))
			{
				Logger.Warning("Tried to add button without a slot!");
			}
			LayoutContainer.SetPosition(newButton, buttonOffset * 75 + new Vector2(5f, 5f));
			this.UpdateSizeData(buttonOffset);
			return newButton;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00010EE0 File Offset: 0x0000F0E0
		[return: Nullable(2)]
		public SlotControl GetButton(string slotName)
		{
			ValueTuple<SlotControl, Vector2i> valueTuple;
			if (this._buttons.TryGetValue(slotName, out valueTuple))
			{
				return valueTuple.Item1;
			}
			return null;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00010F08 File Offset: 0x0000F108
		private void UpdateSizeData(Vector2i buttonOffset)
		{
			Vector2i vector2i = buttonOffset;
			int num;
			int num2;
			vector2i.Deconstruct(ref num, ref num2);
			int num3 = num;
			if (num3 > this.Columns)
			{
				this.Columns = num3;
			}
			vector2i = buttonOffset;
			vector2i.Deconstruct(ref num2, ref num);
			int num4 = num;
			if (num4 > this.Rows)
			{
				this.Rows = num4;
			}
			this.resizer.SetHeight = (float)((this.Rows + 1) * 80);
			this.resizer.SetWidth = (float)((this.Columns + 1) * 80);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00010F84 File Offset: 0x0000F184
		public bool TryGetButton(string slotName, [Nullable(2)] out SlotControl button)
		{
			ValueTuple<SlotControl, Vector2i> valueTuple;
			bool result = this._buttons.TryGetValue(slotName, out valueTuple);
			button = valueTuple.Item1;
			return result;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00010FA8 File Offset: 0x0000F1A8
		public void RemoveButton(string slotName)
		{
			if (!this._buttons.Remove(slotName))
			{
				return;
			}
			this.Columns = 0;
			this.Rows = 0;
			foreach (KeyValuePair<string, ValueTuple<SlotControl, Vector2i>> keyValuePair in this._buttons)
			{
				string text;
				ValueTuple<SlotControl, Vector2i> valueTuple;
				keyValuePair.Deconstruct(out text, out valueTuple);
				Vector2i item = valueTuple.Item2;
				this.UpdateSizeData(item);
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0001102C File Offset: 0x0000F22C
		public void ClearButtons()
		{
			base.Children.Clear();
		}

		// Token: 0x04000167 RID: 359
		private int Columns;

		// Token: 0x04000168 RID: 360
		private int Rows;

		// Token: 0x04000169 RID: 361
		private const int MarginThickness = 10;

		// Token: 0x0400016A RID: 362
		private const int ButtonSpacing = 5;

		// Token: 0x0400016B RID: 363
		private const int ButtonSize = 75;

		// Token: 0x0400016C RID: 364
		private readonly Control resizer;

		// Token: 0x0400016D RID: 365
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1
		})]
		private readonly Dictionary<string, ValueTuple<SlotControl, Vector2i>> _buttons = new Dictionary<string, ValueTuple<SlotControl, Vector2i>>();
	}
}
