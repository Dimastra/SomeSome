using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Mobs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.DamageState
{
	// Token: 0x02000362 RID: 866
	[RegisterComponent]
	public sealed class DamageStateVisualsComponent : Component
	{
		// Token: 0x04000B2A RID: 2858
		public int? OriginalDrawDepth;

		// Token: 0x04000B2B RID: 2859
		[Nullable(1)]
		[DataField("states", false, 1, false, false, null)]
		public Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> States = new Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>>();

		// Token: 0x04000B2C RID: 2860
		[DataField("rotate", false, 1, false, false, null)]
		public bool Rotate;
	}
}
