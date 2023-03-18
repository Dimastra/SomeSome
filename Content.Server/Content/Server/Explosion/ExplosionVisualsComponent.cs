using System;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;

namespace Content.Server.Explosion
{
	// Token: 0x02000508 RID: 1288
	[RegisterComponent]
	[ComponentReference(typeof(SharedExplosionVisualsComponent))]
	public sealed class ExplosionVisualsComponent : SharedExplosionVisualsComponent
	{
	}
}
