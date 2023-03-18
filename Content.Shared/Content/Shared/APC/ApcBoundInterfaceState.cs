using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.APC
{
	// Token: 0x020006F3 RID: 1779
	[NetSerializable]
	[Serializable]
	public sealed class ApcBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x0600157E RID: 5502 RVA: 0x0004632D File Offset: 0x0004452D
		public ApcBoundInterfaceState(bool mainBreaker, int power, ApcExternalPowerState apcExternalPower, float charge)
		{
			this.MainBreaker = mainBreaker;
			this.Power = power;
			this.ApcExternalPower = apcExternalPower;
			this.Charge = charge;
		}

		// Token: 0x040015AA RID: 5546
		public readonly bool MainBreaker;

		// Token: 0x040015AB RID: 5547
		public readonly int Power;

		// Token: 0x040015AC RID: 5548
		public readonly ApcExternalPowerState ApcExternalPower;

		// Token: 0x040015AD RID: 5549
		public readonly float Charge;
	}
}
