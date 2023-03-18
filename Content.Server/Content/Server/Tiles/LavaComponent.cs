using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Tiles
{
	// Token: 0x02000122 RID: 290
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(LavaSystem)
	})]
	public sealed class LavaComponent : Component
	{
		// Token: 0x04000328 RID: 808
		[Nullable(1)]
		[ViewVariables]
		[DataField("soundDisintegration", false, 1, false, false, null)]
		public SoundSpecifier DisintegrationSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", null);

		// Token: 0x04000329 RID: 809
		[ViewVariables]
		[DataField("fireStacks", false, 1, false, false, null)]
		public float FireStacks = 2f;
	}
}
