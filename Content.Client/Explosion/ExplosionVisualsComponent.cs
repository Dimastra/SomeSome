using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Explosion
{
	// Token: 0x02000325 RID: 805
	[RegisterComponent]
	[ComponentReference(typeof(SharedExplosionVisualsComponent))]
	public sealed class ExplosionVisualsComponent : SharedExplosionVisualsComponent
	{
		// Token: 0x04000A30 RID: 2608
		public EntityUid LightEntity;

		// Token: 0x04000A31 RID: 2609
		public float IntensityPerState;

		// Token: 0x04000A32 RID: 2610
		[Nullable(1)]
		public List<Texture[]> FireFrames = new List<Texture[]>();

		// Token: 0x04000A33 RID: 2611
		public Color? FireColor;
	}
}
