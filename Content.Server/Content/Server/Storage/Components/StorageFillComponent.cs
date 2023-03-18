using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Storage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Storage.Components
{
	// Token: 0x02000170 RID: 368
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StorageSystem)
	})]
	public sealed class StorageFillComponent : Component
	{
		// Token: 0x0400045D RID: 1117
		[Nullable(1)]
		[DataField("contents", false, 1, false, false, null)]
		public List<EntitySpawnEntry> Contents = new List<EntitySpawnEntry>();
	}
}
