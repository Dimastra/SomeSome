using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disposal.Components;
using Robust.Client.Animations;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Disposal.Components
{
	// Token: 0x02000355 RID: 853
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedDisposalUnitComponent))]
	public sealed class DisposalUnitComponent : SharedDisposalUnitComponent
	{
		// Token: 0x06001525 RID: 5413 RVA: 0x0007C54C File Offset: 0x0007A74C
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			SharedDisposalUnitComponent.DisposalUnitComponentState disposalUnitComponentState = curState as SharedDisposalUnitComponent.DisposalUnitComponentState;
			if (disposalUnitComponentState == null)
			{
				return;
			}
			this.RecentlyEjected = disposalUnitComponentState.RecentlyEjected;
		}

		// Token: 0x04000AF5 RID: 2805
		[DataField("flushSound", false, 1, false, false, null)]
		public readonly SoundSpecifier FlushSound;

		// Token: 0x04000AF6 RID: 2806
		[Nullable(1)]
		public Animation FlushAnimation;

		// Token: 0x04000AF7 RID: 2807
		public SharedDisposalUnitComponent.DisposalUnitBoundUserInterfaceState UiState;
	}
}
