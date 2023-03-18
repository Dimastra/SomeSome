using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.EntitySystems;
using Content.Shared.Smoking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x0200041E RID: 1054
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(MatchstickSystem)
	})]
	public sealed class MatchstickComponent : Component
	{
		// Token: 0x04000D37 RID: 3383
		[DataField("state", false, 1, false, false, null)]
		public SmokableState CurrentState;

		// Token: 0x04000D38 RID: 3384
		[ViewVariables]
		[DataField("duration", false, 1, false, false, null)]
		public int Duration = 10;

		// Token: 0x04000D39 RID: 3385
		[Nullable(1)]
		[DataField("igniteSound", false, 1, true, false, null)]
		public SoundSpecifier IgniteSound;
	}
}
