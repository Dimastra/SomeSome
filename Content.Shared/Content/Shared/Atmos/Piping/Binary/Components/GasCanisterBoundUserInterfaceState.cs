using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006BE RID: 1726
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasCanisterBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x060014F6 RID: 5366 RVA: 0x0004529D File Offset: 0x0004349D
		public string CanisterLabel { get; }

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x060014F7 RID: 5367 RVA: 0x000452A5 File Offset: 0x000434A5
		public float CanisterPressure { get; }

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x060014F8 RID: 5368 RVA: 0x000452AD File Offset: 0x000434AD
		public bool PortStatus { get; }

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x000452B5 File Offset: 0x000434B5
		[Nullable(2)]
		public string TankLabel { [NullableContext(2)] get; }

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x060014FA RID: 5370 RVA: 0x000452BD File Offset: 0x000434BD
		public float TankPressure { get; }

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x060014FB RID: 5371 RVA: 0x000452C5 File Offset: 0x000434C5
		public float ReleasePressure { get; }

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x060014FC RID: 5372 RVA: 0x000452CD File Offset: 0x000434CD
		public bool ReleaseValve { get; }

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x000452D5 File Offset: 0x000434D5
		public float ReleasePressureMin { get; }

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x060014FE RID: 5374 RVA: 0x000452DD File Offset: 0x000434DD
		public float ReleasePressureMax { get; }

		// Token: 0x060014FF RID: 5375 RVA: 0x000452E8 File Offset: 0x000434E8
		public GasCanisterBoundUserInterfaceState(string canisterLabel, float canisterPressure, bool portStatus, [Nullable(2)] string tankLabel, float tankPressure, float releasePressure, bool releaseValve, float releaseValveMin, float releaseValveMax)
		{
			this.CanisterLabel = canisterLabel;
			this.CanisterPressure = canisterPressure;
			this.PortStatus = portStatus;
			this.TankLabel = tankLabel;
			this.TankPressure = tankPressure;
			this.ReleasePressure = releasePressure;
			this.ReleaseValve = releaseValve;
			this.ReleasePressureMin = releaseValveMin;
			this.ReleasePressureMax = releaseValveMax;
		}
	}
}
