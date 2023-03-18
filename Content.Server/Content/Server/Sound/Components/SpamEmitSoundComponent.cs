using System;
using System.Runtime.CompilerServices;
using Content.Shared.Sound.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Sound.Components
{
	// Token: 0x020001DD RID: 477
	[RegisterComponent]
	public sealed class SpamEmitSoundComponent : BaseEmitSoundComponent
	{
		// Token: 0x0400057A RID: 1402
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x0400057B RID: 1403
		[DataField("rollInterval", false, 1, false, false, null)]
		public float RollInterval = 2f;

		// Token: 0x0400057C RID: 1404
		[DataField("playChance", false, 1, false, false, null)]
		public float PlayChance = 0.5f;

		// Token: 0x0400057D RID: 1405
		[Nullable(2)]
		[DataField("popUp", false, 1, false, false, null)]
		public string PopUp;

		// Token: 0x0400057E RID: 1406
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;
	}
}
