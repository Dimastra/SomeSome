using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.StationEvents.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000190 RID: 400
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpiderSpawn : StationEventSystem
	{
		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00027624 File Offset: 0x00025824
		public override string Prototype
		{
			get
			{
				return "SpiderSpawn";
			}
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0002762C File Offset: 0x0002582C
		public override void Started()
		{
			base.Started();
			List<VentCritterSpawnLocationComponent> spawnLocations = this.EntityManager.EntityQuery<VentCritterSpawnLocationComponent>(false).ToList<VentCritterSpawnLocationComponent>();
			this.RobustRandom.Shuffle<VentCritterSpawnLocationComponent>(spawnLocations);
			double mod = Math.Sqrt((double)base.GetSeverityModifier());
			int spawnAmount = (int)((double)this.RobustRandom.Next(4, 8) * mod);
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Spawning ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(spawnAmount);
			defaultInterpolatedStringHandler.AppendLiteral(" of spiders");
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (VentCritterSpawnLocationComponent location in spawnLocations)
			{
				if (spawnAmount-- == 0)
				{
					break;
				}
				TransformComponent coords = this.EntityManager.GetComponent<TransformComponent>(location.Owner);
				this.EntityManager.SpawnEntity("MobGiantSpiderAngry", coords.Coordinates);
			}
		}
	}
}
