using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Explosion.Components
{
	// Token: 0x0200051D RID: 1309
	[RegisterComponent]
	public sealed class TriggerOnCollideComponent : Component
	{
		// Token: 0x0400117E RID: 4478
		[Nullable(1)]
		[DataField("fixtureID", false, 1, true, false, null)]
		public string FixtureID = string.Empty;

		// Token: 0x0400117F RID: 4479
		[DataField("ignoreOtherNonHard", false, 1, false, false, null)]
		public bool IgnoreOtherNonHard = true;
	}
}
