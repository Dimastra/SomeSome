using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000B9 RID: 185
	[NetSerializable]
	[Serializable]
	public sealed class WelderComponentState : ComponentState
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000A9B7 File Offset: 0x00008BB7
		public float FuelCapacity { get; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000A9BF File Offset: 0x00008BBF
		public float Fuel { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000A9C7 File Offset: 0x00008BC7
		public bool Lit { get; }

		// Token: 0x0600020D RID: 525 RVA: 0x0000A9CF File Offset: 0x00008BCF
		public WelderComponentState(float fuelCapacity, float fuel, bool lit)
		{
			this.FuelCapacity = fuelCapacity;
			this.Fuel = fuel;
			this.Lit = lit;
		}
	}
}
