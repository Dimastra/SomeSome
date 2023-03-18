using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Station;
using Robust.Shared.GameObjects;

namespace Content.Client.Station
{
	// Token: 0x02000132 RID: 306
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationSystem : EntitySystem
	{
		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000840 RID: 2112 RVA: 0x000300A4 File Offset: 0x0002E2A4
		public IReadOnlySet<EntityUid> Stations
		{
			get
			{
				return this._stations;
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x000300AC File Offset: 0x0002E2AC
		public override void Initialize()
		{
			base.SubscribeNetworkEvent<StationsUpdatedEvent>(new EntityEventHandler<StationsUpdatedEvent>(this.StationsUpdated), null, null);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x000300C2 File Offset: 0x0002E2C2
		private void StationsUpdated(StationsUpdatedEvent ev)
		{
			this._stations.Clear();
			this._stations.UnionWith(ev.Stations);
		}

		// Token: 0x0400042A RID: 1066
		private readonly HashSet<EntityUid> _stations = new HashSet<EntityUid>();
	}
}
