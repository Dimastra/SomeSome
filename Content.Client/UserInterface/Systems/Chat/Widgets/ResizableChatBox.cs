using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Chat.Widgets
{
	// Token: 0x020000A5 RID: 165
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResizableChatBox : ChatBox
	{
		// Token: 0x0600043E RID: 1086 RVA: 0x000188BB File Offset: 0x00016ABB
		public ResizableChatBox()
		{
			IoCManager.InjectDependencies<ResizableChatBox>(this);
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x000188CA File Offset: 0x00016ACA
		protected override void EnteredTree()
		{
			base.EnteredTree();
			this._clyde.OnWindowResized += this.ClydeOnOnWindowResized;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x000188E9 File Offset: 0x00016AE9
		protected override void ExitedTree()
		{
			base.ExitedTree();
			this._clyde.OnWindowResized -= this.ClydeOnOnWindowResized;
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00018908 File Offset: 0x00016B08
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			if (args.Function == EngineKeyFunctions.UIClick)
			{
				this._currentDrag = this.GetDragModeFor(args.RelativePosition);
				if (this._currentDrag != ResizableChatBox.DragMode.None)
				{
					this._dragOffsetTopLeft = args.PointerLocation.Position / this.UIScale - base.Position;
					this._dragOffsetBottomRight = base.Position + base.Size - args.PointerLocation.Position / this.UIScale;
				}
			}
			base.KeyBindDown(args);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x000189A4 File Offset: 0x00016BA4
		protected override void KeyBindUp(GUIBoundKeyEventArgs args)
		{
			if (args.Function != EngineKeyFunctions.UIClick)
			{
				return;
			}
			if (this._currentDrag != ResizableChatBox.DragMode.None)
			{
				this._dragOffsetTopLeft = (this._dragOffsetBottomRight = Vector2.Zero);
				this._currentDrag = ResizableChatBox.DragMode.None;
				Control keyboardFocused = base.UserInterfaceManager.KeyboardFocused;
				if (keyboardFocused != null)
				{
					keyboardFocused.ReleaseKeyboardFocus();
				}
			}
			base.KeyBindUp(args);
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00018A04 File Offset: 0x00016C04
		private ResizableChatBox.DragMode GetDragModeFor(Vector2 relativeMousePos)
		{
			ResizableChatBox.DragMode dragMode = ResizableChatBox.DragMode.None;
			if (relativeMousePos.Y > base.Size.Y - 7f)
			{
				dragMode = ResizableChatBox.DragMode.Bottom;
			}
			if (relativeMousePos.X < 7f)
			{
				dragMode |= ResizableChatBox.DragMode.Left;
			}
			return dragMode;
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x00018A40 File Offset: 0x00016C40
		protected override void MouseMove(GUIMouseMoveEventArgs args)
		{
			base.MouseMove(args);
			if (base.Parent == null)
			{
				return;
			}
			if (this._currentDrag == ResizableChatBox.DragMode.None)
			{
				Control.CursorShape defaultCursorShape = 0;
				switch (this.GetDragModeFor(args.RelativePosition))
				{
				case ResizableChatBox.DragMode.Bottom:
					defaultCursorShape = 5;
					break;
				case ResizableChatBox.DragMode.Left:
					defaultCursorShape = 4;
					break;
				case ResizableChatBox.DragMode.Bottom | ResizableChatBox.DragMode.Left:
					defaultCursorShape = 2;
					break;
				}
				base.DefaultCursorShape = defaultCursorShape;
				return;
			}
			float top = base.Rect.Top;
			float value = base.Rect.Bottom;
			float value2 = base.Rect.Left;
			float right = base.Rect.Right;
			float num;
			float num2;
			base.MinSize.Deconstruct(ref num, ref num2);
			float num3 = num;
			float num4 = num2;
			if ((this._currentDrag & ResizableChatBox.DragMode.Bottom) == ResizableChatBox.DragMode.Bottom)
			{
				value = Math.Max(args.GlobalPosition.Y + this._dragOffsetBottomRight.Y, top + num4);
			}
			if ((this._currentDrag & ResizableChatBox.DragMode.Left) == ResizableChatBox.DragMode.Left)
			{
				float val = right - num3;
				value2 = Math.Min(args.GlobalPosition.X - this._dragOffsetTopLeft.X, val);
			}
			this.ClampSize(new float?(value2), new float?(value));
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00018B62 File Offset: 0x00016D62
		protected override void UIScaleChanged()
		{
			base.UIScaleChanged();
			this.ClampAfterDelay();
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00018B70 File Offset: 0x00016D70
		private void ClydeOnOnWindowResized(WindowResizedEventArgs obj)
		{
			this.ClampAfterDelay();
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00018B78 File Offset: 0x00016D78
		private void ClampAfterDelay()
		{
			this._clampIn = 2;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00018B84 File Offset: 0x00016D84
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (this._clampIn <= 0)
			{
				return;
			}
			this._clampIn -= 1;
			if (this._clampIn == 0)
			{
				this.ClampSize(null, null);
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00018BD4 File Offset: 0x00016DD4
		private void ClampSize(float? desiredLeft = null, float? desiredBottom = null)
		{
			if (base.Parent == null)
			{
				return;
			}
			float right = base.Rect.Right;
			float num = desiredLeft ?? base.Rect.Left;
			float num2 = desiredBottom ?? base.Rect.Bottom;
			float num3 = base.Parent.Size.Y - 255f;
			if (num3 <= base.MinHeight)
			{
				num2 = base.MinHeight;
			}
			else
			{
				num2 = Math.Clamp(num2, base.MinHeight, num3);
			}
			float num4 = base.Parent.Size.X - base.MinWidth;
			if (num4 <= 500f)
			{
				num = num4;
			}
			else
			{
				num = Math.Clamp(num, 500f, num4);
			}
			LayoutContainer.SetMarginLeft(this, -(right + 10f - num));
			LayoutContainer.SetMarginBottom(this, num2);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00018CBB File Offset: 0x00016EBB
		protected override void MouseExited()
		{
			base.MouseExited();
			if (this._currentDrag == ResizableChatBox.DragMode.None)
			{
				base.DefaultCursorShape = 0;
			}
		}

		// Token: 0x04000203 RID: 515
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000204 RID: 516
		private const int DragMarginSize = 7;

		// Token: 0x04000205 RID: 517
		private const int MinDistanceFromBottom = 255;

		// Token: 0x04000206 RID: 518
		private const int MinLeft = 500;

		// Token: 0x04000207 RID: 519
		private ResizableChatBox.DragMode _currentDrag;

		// Token: 0x04000208 RID: 520
		private Vector2 _dragOffsetTopLeft;

		// Token: 0x04000209 RID: 521
		private Vector2 _dragOffsetBottomRight;

		// Token: 0x0400020A RID: 522
		private byte _clampIn;

		// Token: 0x020000A6 RID: 166
		[NullableContext(0)]
		[Flags]
		private enum DragMode : byte
		{
			// Token: 0x0400020C RID: 524
			None = 0,
			// Token: 0x0400020D RID: 525
			Bottom = 2,
			// Token: 0x0400020E RID: 526
			Left = 4
		}
	}
}
