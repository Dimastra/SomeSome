using System;
using System.Runtime.CompilerServices;
using Content.Shared.Item;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands.Components
{
	// Token: 0x0200043E RID: 1086
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class HandVirtualItemComponent : Component
	{
		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000D27 RID: 3367 RVA: 0x0002BB23 File Offset: 0x00029D23
		// (set) Token: 0x06000D28 RID: 3368 RVA: 0x0002BB2B File Offset: 0x00029D2B
		public EntityUid BlockingEntity
		{
			get
			{
				return this._blockingEntity;
			}
			set
			{
				this._blockingEntity = value;
				base.Dirty(null);
			}
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x0002BB3B File Offset: 0x00029D3B
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new HandVirtualItemComponent.VirtualItemComponentState(this.BlockingEntity);
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x0002BB48 File Offset: 0x00029D48
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			HandVirtualItemComponent.VirtualItemComponentState pullState = curState as HandVirtualItemComponent.VirtualItemComponentState;
			if (pullState == null)
			{
				return;
			}
			this._blockingEntity = pullState.BlockingEntity;
			IContainer container;
			if (ContainerHelpers.TryGetContainer(base.Owner, ref container, null))
			{
				EntitySystem.Get<SharedItemSystem>().VisualsChanged(base.Owner);
			}
		}

		// Token: 0x04000CB5 RID: 3253
		private EntityUid _blockingEntity;

		// Token: 0x02000802 RID: 2050
		[NetSerializable]
		[Serializable]
		public sealed class VirtualItemComponentState : ComponentState
		{
			// Token: 0x060018CA RID: 6346 RVA: 0x0004EDE2 File Offset: 0x0004CFE2
			public VirtualItemComponentState(EntityUid blockingEntity)
			{
				this.BlockingEntity = blockingEntity;
			}

			// Token: 0x0400189F RID: 6303
			public readonly EntityUid BlockingEntity;
		}
	}
}
