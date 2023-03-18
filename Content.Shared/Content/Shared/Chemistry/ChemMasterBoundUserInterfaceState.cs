using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005D4 RID: 1492
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x06001205 RID: 4613 RVA: 0x0003B3A4 File Offset: 0x000395A4
		public ChemMasterBoundUserInterfaceState(ChemMasterMode mode, ContainerInfo inputContainerInfo, ContainerInfo outputContainerInfo, [Nullable(1)] IReadOnlyList<Solution.ReagentQuantity> bufferReagents, FixedPoint2 bufferCurrentVolume, uint selectedPillType, uint pillDosageLimit, bool updateLabel)
		{
			this.InputContainerInfo = inputContainerInfo;
			this.OutputContainerInfo = outputContainerInfo;
			this.BufferReagents = bufferReagents;
			this.Mode = mode;
			this.BufferCurrentVolume = new FixedPoint2?(bufferCurrentVolume);
			this.SelectedPillType = selectedPillType;
			this.PillDosageLimit = pillDosageLimit;
			this.UpdateLabel = updateLabel;
		}

		// Token: 0x040010DB RID: 4315
		public readonly ContainerInfo InputContainerInfo;

		// Token: 0x040010DC RID: 4316
		public readonly ContainerInfo OutputContainerInfo;

		// Token: 0x040010DD RID: 4317
		[Nullable(1)]
		public readonly IReadOnlyList<Solution.ReagentQuantity> BufferReagents;

		// Token: 0x040010DE RID: 4318
		public readonly ChemMasterMode Mode;

		// Token: 0x040010DF RID: 4319
		public readonly FixedPoint2? BufferCurrentVolume;

		// Token: 0x040010E0 RID: 4320
		public readonly uint SelectedPillType;

		// Token: 0x040010E1 RID: 4321
		public readonly uint PillDosageLimit;

		// Token: 0x040010E2 RID: 4322
		public readonly bool UpdateLabel;
	}
}
