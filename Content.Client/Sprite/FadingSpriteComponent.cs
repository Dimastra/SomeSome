using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Sprite
{
	// Token: 0x02000136 RID: 310
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SpriteFadeSystem)
	})]
	public sealed class FadingSpriteComponent : Component
	{
		// Token: 0x04000431 RID: 1073
		[ViewVariables]
		public float OriginalAlpha;
	}
}
