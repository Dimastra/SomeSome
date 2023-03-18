using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls.FancyTree
{
	// Token: 0x020000EA RID: 234
	public sealed class TreeLine : Control
	{
		// Token: 0x060006C5 RID: 1733 RVA: 0x00023948 File Offset: 0x00021B48
		[NullableContext(1)]
		protected override void Draw(DrawingHandleScreen handle)
		{
			base.Draw(handle);
			TreeItem treeItem = base.Parent as TreeItem;
			if (treeItem == null)
			{
				return;
			}
			if (!treeItem.Expanded || !treeItem.Tree.DrawLines || treeItem.Body.ChildCount == 0)
			{
				return;
			}
			int num = Math.Max(1, (int)((float)treeItem.Tree.LineWidth * this.UIScale));
			int num2 = num / 2;
			int num3 = num - num2;
			Vector2i globalPixelPosition = treeItem.GlobalPixelPosition;
			Vector2i vector2i = treeItem.Icon.GlobalPixelPosition - globalPixelPosition;
			Vector2i pixelSize = treeItem.Icon.PixelSize;
			int num4 = vector2i.X + pixelSize.X / 2;
			Vector2i vector2i2 = treeItem.Button.GlobalPixelPosition - globalPixelPosition;
			Vector2i pixelSize2 = treeItem.Button.PixelSize;
			int item = vector2i2.Y + pixelSize2.Y;
			TreeItem treeItem2 = (TreeItem)treeItem.Body.GetChild(treeItem.Body.ChildCount - 1);
			int item2 = (treeItem2.Button.GlobalPixelPosition - globalPixelPosition).Y + treeItem2.Button.PixelSize.Y / 2;
			UIBox2i uibox2i;
			uibox2i..ctor(new ValueTuple<int, int>(num4 - num2, item), new ValueTuple<int, int>(num4 + num3, item2));
			handle.DrawRect(uibox2i, treeItem.Tree.LineColor, true);
			int num5 = Math.Max(1, (int)(16f * this.UIScale / 2f));
			foreach (Control control in treeItem.Body.Children)
			{
				TreeItem treeItem3 = (TreeItem)control;
				int num6 = (treeItem3.Button.GlobalPixelPosition - globalPixelPosition).Y + treeItem3.Button.PixelSize.Y / 2;
				uibox2i = new UIBox2i(new ValueTuple<int, int>(num4 - num2, num6 - num2), new ValueTuple<int, int>(num4 + num5, num6 + num3));
				handle.DrawRect(uibox2i, treeItem.Tree.LineColor, true);
			}
		}
	}
}
