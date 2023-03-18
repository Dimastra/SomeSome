using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Solar
{
	// Token: 0x0200018E RID: 398
	[NetSerializable]
	[Serializable]
	public sealed class SolarControlConsoleBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060004C2 RID: 1218 RVA: 0x000125D6 File Offset: 0x000107D6
		public SolarControlConsoleBoundInterfaceState(Angle r, Angle vm, float p, Angle tw)
		{
			this.Rotation = r;
			this.AngularVelocity = vm;
			this.OutputPower = p;
			this.TowardsSun = tw;
		}

		// Token: 0x0400045C RID: 1116
		public Angle Rotation;

		// Token: 0x0400045D RID: 1117
		public Angle AngularVelocity;

		// Token: 0x0400045E RID: 1118
		public float OutputPower;

		// Token: 0x0400045F RID: 1119
		public Angle TowardsSun;
	}
}
