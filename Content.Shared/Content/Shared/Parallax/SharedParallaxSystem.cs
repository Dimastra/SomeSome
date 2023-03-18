using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Parallax
{
	// Token: 0x0200029B RID: 667
	public abstract class SharedParallaxSystem : EntitySystem
	{
		// Token: 0x020007BD RID: 1981
		[NetSerializable]
		[Serializable]
		protected sealed class ParallaxComponentState : ComponentState
		{
			// Token: 0x040017F2 RID: 6130
			[Nullable(1)]
			public string Parallax = string.Empty;
		}
	}
}
