using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B2 RID: 434
	[NetSerializable]
	[Serializable]
	public sealed class ParticleAcceleratorSetPowerStateMessage : BoundUserInterfaceMessage
	{
		// Token: 0x0600050B RID: 1291 RVA: 0x0001355F File Offset: 0x0001175F
		public ParticleAcceleratorSetPowerStateMessage(ParticleAcceleratorPowerState state)
		{
			this.State = state;
		}

		// Token: 0x040004F7 RID: 1271
		public readonly ParticleAcceleratorPowerState State;
	}
}
