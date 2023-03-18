using System;
using Content.Shared.Explosion;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Explosion
{
	// Token: 0x02000326 RID: 806
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(TriggerSystem)
	})]
	public sealed class TriggerOnProximityComponent : SharedTriggerOnProximityComponent
	{
	}
}
