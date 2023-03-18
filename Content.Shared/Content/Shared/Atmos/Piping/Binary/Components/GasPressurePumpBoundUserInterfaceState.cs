using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components
{
	// Token: 0x020006C3 RID: 1731
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasPressurePumpBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x00045376 File Offset: 0x00043576
		public string PumpLabel { get; }

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001506 RID: 5382 RVA: 0x0004537E File Offset: 0x0004357E
		public float OutputPressure { get; }

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x00045386 File Offset: 0x00043586
		public bool Enabled { get; }

		// Token: 0x06001508 RID: 5384 RVA: 0x0004538E File Offset: 0x0004358E
		public GasPressurePumpBoundUserInterfaceState(string pumpLabel, float outputPressure, bool enabled)
		{
			this.PumpLabel = pumpLabel;
			this.OutputPressure = outputPressure;
			this.Enabled = enabled;
		}
	}
}
