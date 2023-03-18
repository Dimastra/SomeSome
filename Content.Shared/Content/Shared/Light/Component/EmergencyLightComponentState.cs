using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000373 RID: 883
	[NetSerializable]
	[Serializable]
	public sealed class EmergencyLightComponentState : ComponentState
	{
		// Token: 0x06000A4B RID: 2635 RVA: 0x00022342 File Offset: 0x00020542
		public EmergencyLightComponentState(bool enabled)
		{
			this.Enabled = enabled;
		}

		// Token: 0x04000A26 RID: 2598
		public bool Enabled;
	}
}
