using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Server.Polymorph.Systems
{
	// Token: 0x020002C4 RID: 708
	[NullableContext(1)]
	[Nullable(0)]
	internal struct PolymorphQueuedData
	{
		// Token: 0x06000E5A RID: 3674 RVA: 0x00048989 File Offset: 0x00046B89
		public PolymorphQueuedData(EntityUid ent, SoundSpecifier sound, string polymorph)
		{
			this.Ent = ent;
			this.Sound = sound;
			this.Polymorph = polymorph;
		}

		// Token: 0x04000865 RID: 2149
		public EntityUid Ent;

		// Token: 0x04000866 RID: 2150
		public SoundSpecifier Sound;

		// Token: 0x04000867 RID: 2151
		public string Polymorph;
	}
}
