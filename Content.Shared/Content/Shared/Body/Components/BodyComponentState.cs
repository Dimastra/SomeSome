using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Part;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Components
{
	// Token: 0x0200066B RID: 1643
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BodyComponentState : ComponentState
	{
		// Token: 0x06001420 RID: 5152 RVA: 0x00043480 File Offset: 0x00041680
		public BodyComponentState([Nullable(2)] BodyPartSlot root, SoundSpecifier gibSound)
		{
			this.Root = root;
			this.GibSound = gibSound;
		}

		// Token: 0x040013C6 RID: 5062
		[Nullable(2)]
		public readonly BodyPartSlot Root;

		// Token: 0x040013C7 RID: 5063
		public readonly SoundSpecifier GibSound;
	}
}
