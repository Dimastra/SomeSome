using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Storage.Components
{
	// Token: 0x0200016F RID: 367
	[RegisterComponent]
	public sealed class SpawnItemsOnUseComponent : Component
	{
		// Token: 0x0400045A RID: 1114
		[Nullable(1)]
		[DataField("items", false, 1, true, false, null)]
		public List<EntitySpawnEntry> Items = new List<EntitySpawnEntry>();

		// Token: 0x0400045B RID: 1115
		[Nullable(2)]
		[DataField("sound", false, 1, true, false, null)]
		public SoundSpecifier Sound;

		// Token: 0x0400045C RID: 1116
		[DataField("uses", false, 1, false, false, null)]
		public int Uses = 1;
	}
}
