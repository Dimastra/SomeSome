using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Spawners.Components;
using Content.Server.Station.Systems;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;

namespace Content.Server.Spawners.EntitySystems
{
	// Token: 0x020001D3 RID: 467
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpawnPointSystem : EntitySystem
	{
		// Token: 0x060008E6 RID: 2278 RVA: 0x0002D505 File Offset: 0x0002B705
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PlayerSpawningEvent>(new EntityEventHandler<PlayerSpawningEvent>(this.OnSpawnPlayer), null, null);
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x0002D51C File Offset: 0x0002B71C
		private void OnSpawnPlayer(PlayerSpawningEvent args)
		{
			List<SpawnPointComponent> points = base.EntityQuery<SpawnPointComponent>(false).ToList<SpawnPointComponent>();
			this._random.Shuffle<SpawnPointComponent>(points);
			foreach (SpawnPointComponent spawnPoint in points)
			{
				TransformComponent xform = base.Transform(spawnPoint.Owner);
				if (args.Station == null || !(this._stationSystem.GetOwningStation(spawnPoint.Owner, xform) != args.Station))
				{
					if (this._gameTicker.RunLevel == GameRunLevel.InRound && spawnPoint.SpawnType == SpawnPointType.LateJoin)
					{
						args.SpawnResult = new EntityUid?(this._stationSpawning.SpawnPlayerMob(xform.Coordinates, args.Job, args.HumanoidCharacterProfile, args.Station));
						return;
					}
					if (this._gameTicker.RunLevel != GameRunLevel.InRound && spawnPoint.SpawnType == SpawnPointType.Job)
					{
						if (args.Job != null)
						{
							JobPrototype job = spawnPoint.Job;
							if (!(((job != null) ? job.ID : null) == args.Job.Prototype.ID))
							{
								continue;
							}
						}
						args.SpawnResult = new EntityUid?(this._stationSpawning.SpawnPlayerMob(xform.Coordinates, args.Job, args.HumanoidCharacterProfile, args.Station));
						return;
					}
				}
			}
			using (List<SpawnPointComponent>.Enumerator enumerator = points.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					SpawnPointComponent spawnPoint2 = enumerator.Current;
					TransformComponent xform2 = base.Transform(spawnPoint2.Owner);
					args.SpawnResult = new EntityUid?(this._stationSpawning.SpawnPlayerMob(xform2.Coordinates, args.Job, args.HumanoidCharacterProfile, args.Station));
					return;
				}
			}
			Logger.ErrorS("spawning", "No spawn points were available!");
		}

		// Token: 0x04000561 RID: 1377
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000562 RID: 1378
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000563 RID: 1379
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000564 RID: 1380
		[Dependency]
		private readonly StationSpawningSystem _stationSpawning;
	}
}
