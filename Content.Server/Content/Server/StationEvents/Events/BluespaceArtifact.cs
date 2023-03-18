using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000182 RID: 386
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BluespaceArtifact : StationEventSystem
	{
		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060007A9 RID: 1961 RVA: 0x00025C0B File Offset: 0x00023E0B
		public override string Prototype
		{
			get
			{
				return "BluespaceArtifact";
			}
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00025C14 File Offset: 0x00023E14
		public override void Added()
		{
			base.Added();
			string str = Loc.GetString("bluespace-artifact-event-announcement", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("sighting", Loc.GetString(RandomExtensions.Pick<string>(this._random, this.PossibleSighting)))
			});
			this.ChatSystem.DispatchGlobalAnnouncement(str, "Central Command", true, null, new Color?(Color.FromHex("#18abf5", null)));
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00025C90 File Offset: 0x00023E90
		public override void Started()
		{
			base.Started();
			int amountToSpawn = Math.Max(1, (int)MathF.Round(base.GetSeverityModifier() / 1.5f));
			for (int i = 0; i < amountToSpawn; i++)
			{
				Vector2i vector2i;
				EntityUid entityUid;
				EntityUid entityUid2;
				EntityCoordinates coords;
				if (!base.TryFindRandomTile(out vector2i, out entityUid, out entityUid2, out coords))
				{
					return;
				}
				this.EntityManager.SpawnEntity(this.ArtifactSpawnerPrototype, coords);
				this.EntityManager.SpawnEntity(this.ArtifactFlashPrototype, coords);
				ISawmill sawmill = this.Sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Spawning random artifact at ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coords);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0400049F RID: 1183
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040004A0 RID: 1184
		public readonly string ArtifactSpawnerPrototype = "RandomArtifactSpawner";

		// Token: 0x040004A1 RID: 1185
		public readonly string ArtifactFlashPrototype = "EffectFlashBluespace";

		// Token: 0x040004A2 RID: 1186
		public readonly List<string> PossibleSighting = new List<string>
		{
			"bluespace-artifact-sighting-1",
			"bluespace-artifact-sighting-2",
			"bluespace-artifact-sighting-3",
			"bluespace-artifact-sighting-4",
			"bluespace-artifact-sighting-5",
			"bluespace-artifact-sighting-6",
			"bluespace-artifact-sighting-7"
		};
	}
}
