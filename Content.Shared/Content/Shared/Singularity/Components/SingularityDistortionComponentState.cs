using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001B8 RID: 440
	[NetSerializable]
	[Serializable]
	public sealed class SingularityDistortionComponentState : ComponentState
	{
		// Token: 0x06000515 RID: 1301 RVA: 0x000136CF File Offset: 0x000118CF
		public SingularityDistortionComponentState(float intensity, float falloff)
		{
			this.Intensity = intensity;
			this.Falloff = falloff;
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x000136E5 File Offset: 0x000118E5
		public float Intensity { get; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x000136ED File Offset: 0x000118ED
		public float Falloff { get; }
	}
}
