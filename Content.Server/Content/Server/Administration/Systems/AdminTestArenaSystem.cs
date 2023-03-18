using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;

namespace Content.Server.Administration.Systems
{
	// Token: 0x0200080C RID: 2060
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminTestArenaSystem : EntitySystem
	{
		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06002CBA RID: 11450 RVA: 0x000E903B File Offset: 0x000E723B
		// (set) Token: 0x06002CBB RID: 11451 RVA: 0x000E9043 File Offset: 0x000E7243
		public Dictionary<NetUserId, EntityUid> ArenaMap { get; private set; } = new Dictionary<NetUserId, EntityUid>();

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06002CBC RID: 11452 RVA: 0x000E904C File Offset: 0x000E724C
		// (set) Token: 0x06002CBD RID: 11453 RVA: 0x000E9054 File Offset: 0x000E7254
		public Dictionary<NetUserId, EntityUid?> ArenaGrid { get; private set; } = new Dictionary<NetUserId, EntityUid?>();

		// Token: 0x06002CBE RID: 11454 RVA: 0x000E9060 File Offset: 0x000E7260
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Map",
			"Grid"
		})]
		public ValueTuple<EntityUid, EntityUid?> AssertArenaLoaded([Nullable(1)] IPlayerSession admin)
		{
			EntityUid arenaMap;
			if (!this.ArenaMap.TryGetValue(admin.UserId, out arenaMap) || base.Deleted(arenaMap, null) || base.Terminating(arenaMap, null))
			{
				this.ArenaMap[admin.UserId] = this._mapManager.GetMapEntityId(this._mapManager.CreateMap(null));
				IReadOnlyList<EntityUid> grids = this._map.LoadMap(base.Comp<MapComponent>(this.ArenaMap[admin.UserId]).WorldMap, "/Maps/Test/admin_test_arena.yml", null);
				this.ArenaGrid[admin.UserId] = ((grids.Count == 0) ? null : new EntityUid?(grids[0]));
				return new ValueTuple<EntityUid, EntityUid?>(this.ArenaMap[admin.UserId], this.ArenaGrid[admin.UserId]);
			}
			EntityUid? arenaGrid;
			if (this.ArenaGrid.TryGetValue(admin.UserId, out arenaGrid) && !base.Deleted(arenaGrid) && !base.Terminating(arenaGrid.Value, null))
			{
				return new ValueTuple<EntityUid, EntityUid?>(arenaMap, arenaGrid);
			}
			this.ArenaGrid[admin.UserId] = null;
			return new ValueTuple<EntityUid, EntityUid?>(arenaMap, null);
		}

		// Token: 0x04001B95 RID: 7061
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001B96 RID: 7062
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x04001B97 RID: 7063
		public const string ArenaMapPath = "/Maps/Test/admin_test_arena.yml";
	}
}
