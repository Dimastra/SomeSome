using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Kudzu
{
	// Token: 0x0200042C RID: 1068
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SpreaderSystem)
	})]
	public sealed class SpreaderComponent : Component
	{
		// Token: 0x04000D6D RID: 3437
		[DataField("chance", false, 1, true, false, null)]
		public float Chance;

		// Token: 0x04000D6E RID: 3438
		[Nullable(1)]
		[DataField("growthResult", false, 1, true, false, null)]
		public string GrowthResult;

		// Token: 0x04000D6F RID: 3439
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;
	}
}
