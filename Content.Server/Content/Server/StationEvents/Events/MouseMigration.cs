using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.StationEvents.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018B RID: 395
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MouseMigration : StationEventSystem
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x00026C18 File Offset: 0x00024E18
		public override string Prototype
		{
			get
			{
				return "MouseMigration";
			}
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00026C20 File Offset: 0x00024E20
		public override void Started()
		{
			base.Started();
			float modifier = base.GetSeverityModifier();
			List<ValueTuple<VentCritterSpawnLocationComponent, TransformComponent>> spawnLocations = this.EntityManager.EntityQuery<VentCritterSpawnLocationComponent, TransformComponent>(false).ToList<ValueTuple<VentCritterSpawnLocationComponent, TransformComponent>>();
			this.RobustRandom.Shuffle<ValueTuple<VentCritterSpawnLocationComponent, TransformComponent>>(spawnLocations);
			int spawnAmount = (int)((double)this.RobustRandom.Next(7, 15) * Math.Sqrt((double)modifier));
			int i = 0;
			while (i < spawnAmount && i < spawnLocations.Count - 1)
			{
				string spawnChoice = RandomExtensions.Pick<string>(this.RobustRandom, MouseMigration.SpawnedPrototypeChoices);
				if (RandomExtensions.Prob(this.RobustRandom, Math.Min(0.01f * modifier, 1f)) || i == 0)
				{
					spawnChoice = "SpawnPointGhostRatKing";
				}
				EntityCoordinates coords = spawnLocations[i].Item2.Coordinates;
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Spawning mouse ");
				defaultInterpolatedStringHandler.AppendFormatted(spawnChoice);
				defaultInterpolatedStringHandler.AppendLiteral(" at ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coords);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				this.EntityManager.SpawnEntity(spawnChoice, coords);
				i++;
			}
		}

		// Token: 0x040004C9 RID: 1225
		public static List<string> SpawnedPrototypeChoices = new List<string>
		{
			"MobMouse",
			"MobMouse1",
			"MobMouse2",
			"MobRatServant"
		};
	}
}
