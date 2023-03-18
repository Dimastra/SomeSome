using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Storage;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003ED RID: 1005
	public sealed class WorldSpawnSpellEvent : WorldTargetActionEvent
	{
		// Token: 0x04000CBD RID: 3261
		[Nullable(1)]
		[DataField("prototypes", false, 1, false, false, null)]
		public List<EntitySpawnEntry> Contents = new List<EntitySpawnEntry>();

		// Token: 0x04000CBE RID: 3262
		[DataField("offset", false, 1, false, false, null)]
		public Vector2 Offset;

		// Token: 0x04000CBF RID: 3263
		[DataField("lifetime", false, 1, false, false, null)]
		public float? Lifetime;
	}
}
