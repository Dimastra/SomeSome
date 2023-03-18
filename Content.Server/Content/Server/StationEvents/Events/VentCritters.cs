using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.StationEvents.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000194 RID: 404
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VentCritters : StationEventSystem
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x00027EAE File Offset: 0x000260AE
		public override string Prototype
		{
			get
			{
				return "VentCritters";
			}
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00027EB8 File Offset: 0x000260B8
		public override void Started()
		{
			base.Started();
			string spawnChoice = RandomExtensions.Pick<string>(this.RobustRandom, VentCritters.SpawnedPrototypeChoices);
			List<VentCritterSpawnLocationComponent> spawnLocations = this.EntityManager.EntityQuery<VentCritterSpawnLocationComponent>(false).ToList<VentCritterSpawnLocationComponent>();
			this.RobustRandom.Shuffle<VentCritterSpawnLocationComponent>(spawnLocations);
			int spawnAmount = this.RobustRandom.Next(4, 12);
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Spawning ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(spawnAmount);
			defaultInterpolatedStringHandler.AppendLiteral(" of ");
			defaultInterpolatedStringHandler.AppendFormatted(spawnChoice);
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (VentCritterSpawnLocationComponent location in spawnLocations)
			{
				if (spawnAmount-- == 0)
				{
					break;
				}
				TransformComponent coords = this.EntityManager.GetComponent<TransformComponent>(location.Owner);
				this.EntityManager.SpawnEntity(spawnChoice, coords.Coordinates);
			}
		}

		// Token: 0x040004E1 RID: 1249
		public static List<string> SpawnedPrototypeChoices = new List<string>
		{
			"MobMouse",
			"MobMouse1",
			"MobMouse2"
		};
	}
}
