using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ghost
{
	// Token: 0x0200044F RID: 1103
	[NetworkedComponent]
	public abstract class SharedGhostComponent : Component
	{
		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x0002C70D File Offset: 0x0002A90D
		// (set) Token: 0x06000D6E RID: 3438 RVA: 0x0002C715 File Offset: 0x0002A915
		[ViewVariables]
		public bool CanGhostInteract
		{
			get
			{
				return this._canGhostInteract;
			}
			set
			{
				if (this._canGhostInteract == value)
				{
					return;
				}
				this._canGhostInteract = value;
				base.Dirty(null);
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000D6F RID: 3439 RVA: 0x0002C72F File Offset: 0x0002A92F
		// (set) Token: 0x06000D70 RID: 3440 RVA: 0x0002C737 File Offset: 0x0002A937
		[ViewVariables]
		public bool CanReturnToBody
		{
			get
			{
				return this._canReturnToBody;
			}
			set
			{
				if (this._canReturnToBody == value)
				{
					return;
				}
				this._canReturnToBody = value;
				base.Dirty(null);
			}
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0002C751 File Offset: 0x0002A951
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new GhostComponentState(this.CanReturnToBody, this.CanGhostInteract);
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x0002C764 File Offset: 0x0002A964
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			GhostComponentState state = curState as GhostComponentState;
			if (state == null)
			{
				return;
			}
			this.CanReturnToBody = state.CanReturnToBody;
			this.CanGhostInteract = state.CanGhostInteract;
		}

		// Token: 0x04000CE5 RID: 3301
		[DataField("canInteract", false, 1, false, false, null)]
		private bool _canGhostInteract;

		// Token: 0x04000CE6 RID: 3302
		[DataField("canReturnToBody", false, 1, false, false, null)]
		private bool _canReturnToBody;
	}
}
