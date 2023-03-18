using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B8 RID: 1720
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GasMixerBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x060014EB RID: 5355 RVA: 0x00045213 File Offset: 0x00043413
		public string MixerLabel { get; }

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x060014EC RID: 5356 RVA: 0x0004521B File Offset: 0x0004341B
		public float OutputPressure { get; }

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x060014ED RID: 5357 RVA: 0x00045223 File Offset: 0x00043423
		public bool Enabled { get; }

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x060014EE RID: 5358 RVA: 0x0004522B File Offset: 0x0004342B
		public float NodeOne { get; }

		// Token: 0x060014EF RID: 5359 RVA: 0x00045233 File Offset: 0x00043433
		public GasMixerBoundUserInterfaceState(string mixerLabel, float outputPressure, bool enabled, float nodeOne)
		{
			this.MixerLabel = mixerLabel;
			this.OutputPressure = outputPressure;
			this.Enabled = enabled;
			this.NodeOne = nodeOne;
		}
	}
}
