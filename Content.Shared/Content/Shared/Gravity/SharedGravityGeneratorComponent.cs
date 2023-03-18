using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Gravity
{
	// Token: 0x02000449 RID: 1097
	[NetworkedComponent]
	[Virtual]
	public class SharedGravityGeneratorComponent : Component
	{
		// Token: 0x02000803 RID: 2051
		[NetSerializable]
		[Serializable]
		public sealed class SwitchGeneratorMessage : BoundUserInterfaceMessage
		{
			// Token: 0x060018CB RID: 6347 RVA: 0x0004EDF1 File Offset: 0x0004CFF1
			public SwitchGeneratorMessage(bool on)
			{
				this.On = on;
			}

			// Token: 0x040018A0 RID: 6304
			public bool On;
		}

		// Token: 0x02000804 RID: 2052
		[NetSerializable]
		[Serializable]
		public sealed class GeneratorState : BoundUserInterfaceState
		{
			// Token: 0x060018CC RID: 6348 RVA: 0x0004EE00 File Offset: 0x0004D000
			public GeneratorState(bool on, byte charge, GravityGeneratorPowerStatus powerStatus, short powerDraw, short powerDrawMax, short etaSeconds)
			{
				this.On = on;
				this.Charge = charge;
				this.PowerStatus = powerStatus;
				this.PowerDraw = powerDraw;
				this.PowerDrawMax = powerDrawMax;
				this.EtaSeconds = etaSeconds;
			}

			// Token: 0x040018A1 RID: 6305
			public bool On;

			// Token: 0x040018A2 RID: 6306
			public byte Charge;

			// Token: 0x040018A3 RID: 6307
			public GravityGeneratorPowerStatus PowerStatus;

			// Token: 0x040018A4 RID: 6308
			public short PowerDraw;

			// Token: 0x040018A5 RID: 6309
			public short PowerDrawMax;

			// Token: 0x040018A6 RID: 6310
			public short EtaSeconds;
		}

		// Token: 0x02000805 RID: 2053
		[NetSerializable]
		[Serializable]
		public enum GravityGeneratorUiKey
		{
			// Token: 0x040018A8 RID: 6312
			Key
		}
	}
}
