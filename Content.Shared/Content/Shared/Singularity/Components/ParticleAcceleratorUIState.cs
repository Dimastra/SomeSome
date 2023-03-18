using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001AF RID: 431
	[NetSerializable]
	[Serializable]
	public sealed class ParticleAcceleratorUIState : BoundUserInterfaceState
	{
		// Token: 0x06000508 RID: 1288 RVA: 0x000134C8 File Offset: 0x000116C8
		public ParticleAcceleratorUIState(bool assembled, bool enabled, ParticleAcceleratorPowerState state, int powerReceive, int powerDraw, bool emitterLeftExists, bool emitterCenterExists, bool emitterRightExists, bool powerBoxExists, bool fuelChamberExists, bool endCapExists, bool interfaceBlock, ParticleAcceleratorPowerState maxLevel, bool wirePowerBlock)
		{
			this.Assembled = assembled;
			this.Enabled = enabled;
			this.State = state;
			this.PowerDraw = powerDraw;
			this.PowerReceive = powerReceive;
			this.EmitterLeftExists = emitterLeftExists;
			this.EmitterCenterExists = emitterCenterExists;
			this.EmitterRightExists = emitterRightExists;
			this.PowerBoxExists = powerBoxExists;
			this.FuelChamberExists = fuelChamberExists;
			this.EndCapExists = endCapExists;
			this.InterfaceBlock = interfaceBlock;
			this.MaxLevel = maxLevel;
			this.WirePowerBlock = wirePowerBlock;
		}

		// Token: 0x040004E8 RID: 1256
		public bool Assembled;

		// Token: 0x040004E9 RID: 1257
		public bool Enabled;

		// Token: 0x040004EA RID: 1258
		public ParticleAcceleratorPowerState State;

		// Token: 0x040004EB RID: 1259
		public int PowerDraw;

		// Token: 0x040004EC RID: 1260
		public int PowerReceive;

		// Token: 0x040004ED RID: 1261
		public bool EmitterLeftExists;

		// Token: 0x040004EE RID: 1262
		public bool EmitterCenterExists;

		// Token: 0x040004EF RID: 1263
		public bool EmitterRightExists;

		// Token: 0x040004F0 RID: 1264
		public bool PowerBoxExists;

		// Token: 0x040004F1 RID: 1265
		public bool FuelChamberExists;

		// Token: 0x040004F2 RID: 1266
		public bool EndCapExists;

		// Token: 0x040004F3 RID: 1267
		public bool InterfaceBlock;

		// Token: 0x040004F4 RID: 1268
		public ParticleAcceleratorPowerState MaxLevel;

		// Token: 0x040004F5 RID: 1269
		public bool WirePowerBlock;
	}
}
