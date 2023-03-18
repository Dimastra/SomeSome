using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Holiday.Christmas
{
	// Token: 0x0200046F RID: 1135
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(LimitedItemGiverSystem)
	})]
	public sealed class LimitedItemGiverComponent : Component
	{
		// Token: 0x04000E3D RID: 3645
		public readonly HashSet<NetUserId> GrantedPlayers = new HashSet<NetUserId>();

		// Token: 0x04000E3E RID: 3646
		[DataField("spawnEntries", false, 1, true, false, null)]
		public List<EntitySpawnEntry> SpawnEntries;

		// Token: 0x04000E3F RID: 3647
		[DataField("receivedPopup", false, 1, true, false, null)]
		public string ReceivedPopup;

		// Token: 0x04000E40 RID: 3648
		[DataField("deniedPopup", false, 1, true, false, null)]
		public string DeniedPopup;

		// Token: 0x04000E41 RID: 3649
		[Nullable(2)]
		[DataField("requiredHoliday", false, 1, false, false, typeof(PrototypeIdSerializer<HolidayPrototype>))]
		[ViewVariables]
		public string RequiredHoliday;
	}
}
