using System;
using System.Runtime.CompilerServices;
using Robust.Client.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.DragDrop
{
	// Token: 0x0200033C RID: 828
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DragDropHelper<[Nullable(2)] T>
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x060014A0 RID: 5280 RVA: 0x00078C79 File Offset: 0x00076E79
		public ScreenCoordinates MouseScreenPosition
		{
			get
			{
				return this._inputManager.MouseScreenPosition;
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x00078C86 File Offset: 0x00076E86
		public bool IsDragging
		{
			get
			{
				return this._state == DragDropHelper<T>.DragState.Dragging;
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x060014A2 RID: 5282 RVA: 0x00078C91 File Offset: 0x00076E91
		// (set) Token: 0x060014A3 RID: 5283 RVA: 0x00078C99 File Offset: 0x00076E99
		[Nullable(2)]
		public T Dragged { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x060014A4 RID: 5284 RVA: 0x00078CA2 File Offset: 0x00076EA2
		public DragDropHelper(OnBeginDrag onBeginDrag, OnContinueDrag onContinueDrag, OnEndDrag onEndDrag)
		{
			this._inputManager = IoCManager.Resolve<IInputManager>();
			this._onBeginDrag = onBeginDrag;
			this._onEndDrag = onEndDrag;
			this._onContinueDrag = onContinueDrag;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x00078CD5 File Offset: 0x00076ED5
		public void MouseDown(T target)
		{
			if (this._state != DragDropHelper<T>.DragState.NotDragging)
			{
				this.EndDrag();
			}
			this.Dragged = target;
			this._state = DragDropHelper<T>.DragState.MouseDown;
			this._mouseDownScreenPos = this._inputManager.MouseScreenPosition;
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x00078D04 File Offset: 0x00076F04
		public void EndDrag()
		{
			this.Dragged = default(T);
			this._state = DragDropHelper<T>.DragState.NotDragging;
			this._onEndDrag();
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x00078D32 File Offset: 0x00076F32
		private void StartDragging()
		{
			if (this._onBeginDrag())
			{
				this._state = DragDropHelper<T>.DragState.Dragging;
				return;
			}
			this.EndDrag();
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x00078D50 File Offset: 0x00076F50
		public void Update(float frameTime)
		{
			DragDropHelper<T>.DragState state = this._state;
			if (state != DragDropHelper<T>.DragState.MouseDown)
			{
				if (state != DragDropHelper<T>.DragState.Dragging)
				{
					return;
				}
				if (!this._onContinueDrag(frameTime))
				{
					this.EndDrag();
				}
			}
			else
			{
				ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
				if ((this._mouseDownScreenPos.Position - mouseScreenPosition.Position).Length > this.Deadzone)
				{
					this.StartDragging();
					return;
				}
			}
		}

		// Token: 0x04000A94 RID: 2708
		private readonly IInputManager _inputManager;

		// Token: 0x04000A95 RID: 2709
		private readonly OnBeginDrag _onBeginDrag;

		// Token: 0x04000A96 RID: 2710
		private readonly OnEndDrag _onEndDrag;

		// Token: 0x04000A97 RID: 2711
		private readonly OnContinueDrag _onContinueDrag;

		// Token: 0x04000A98 RID: 2712
		public float Deadzone = 2f;

		// Token: 0x04000A9A RID: 2714
		private ScreenCoordinates _mouseDownScreenPos;

		// Token: 0x04000A9B RID: 2715
		[Nullable(0)]
		private DragDropHelper<T>.DragState _state;

		// Token: 0x0200033D RID: 829
		[NullableContext(0)]
		private enum DragState : byte
		{
			// Token: 0x04000A9D RID: 2717
			NotDragging,
			// Token: 0x04000A9E RID: 2718
			MouseDown,
			// Token: 0x04000A9F RID: 2719
			Dragging
		}
	}
}
