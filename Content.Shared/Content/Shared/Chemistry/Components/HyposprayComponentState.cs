using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005FA RID: 1530
	[NetSerializable]
	[Serializable]
	public sealed class HyposprayComponentState : ComponentState
	{
		// Token: 0x170003BE RID: 958
		// (get) Token: 0x0600129B RID: 4763 RVA: 0x0003CC23 File Offset: 0x0003AE23
		public FixedPoint2 CurVolume { get; }

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x0600129C RID: 4764 RVA: 0x0003CC2B File Offset: 0x0003AE2B
		public FixedPoint2 MaxVolume { get; }

		// Token: 0x0600129D RID: 4765 RVA: 0x0003CC33 File Offset: 0x0003AE33
		public HyposprayComponentState(FixedPoint2 curVolume, FixedPoint2 maxVolume)
		{
			this.CurVolume = curVolume;
			this.MaxVolume = maxVolume;
		}
	}
}
