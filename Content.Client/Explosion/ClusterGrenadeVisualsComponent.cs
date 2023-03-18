using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Explosion
{
	// Token: 0x02000322 RID: 802
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ClusterGrenadeVisualizerSystem)
	})]
	public sealed class ClusterGrenadeVisualsComponent : Component
	{
		// Token: 0x04000A26 RID: 2598
		[Nullable(2)]
		[DataField("state", false, 1, false, false, null)]
		public string State;
	}
}
