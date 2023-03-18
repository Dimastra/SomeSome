using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B0 RID: 432
	[NetSerializable]
	[Serializable]
	public sealed class ParticleAcceleratorSetEnableMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000509 RID: 1289 RVA: 0x00013548 File Offset: 0x00011748
		public ParticleAcceleratorSetEnableMessage(bool enabled)
		{
			this.Enabled = enabled;
		}

		// Token: 0x040004F6 RID: 1270
		public readonly bool Enabled;
	}
}
