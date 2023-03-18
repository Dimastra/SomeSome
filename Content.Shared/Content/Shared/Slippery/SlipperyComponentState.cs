using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Slippery
{
	// Token: 0x02000197 RID: 407
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SlipperyComponentState : ComponentState
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001264E File Offset: 0x0001084E
		public float ParalyzeTime { get; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x00012656 File Offset: 0x00010856
		public float LaunchForwardsMultiplier { get; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0001265E File Offset: 0x0001085E
		public string SlipSound { get; }

		// Token: 0x060004C9 RID: 1225 RVA: 0x00012666 File Offset: 0x00010866
		public SlipperyComponentState(float paralyzeTime, float launchForwardsMultiplier, string slipSound)
		{
			this.ParalyzeTime = paralyzeTime;
			this.LaunchForwardsMultiplier = launchForwardsMultiplier;
			this.SlipSound = slipSound;
		}
	}
}
