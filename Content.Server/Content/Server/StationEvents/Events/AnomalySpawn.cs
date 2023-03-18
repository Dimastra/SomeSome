using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Anomaly;
using Content.Server.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000181 RID: 385
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnomalySpawn : StationEventSystem
	{
		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00025A57 File Offset: 0x00023C57
		public override string Prototype
		{
			get
			{
				return "AnomalySpawn";
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x00025A60 File Offset: 0x00023C60
		public override void Added()
		{
			base.Added();
			string text = "anomaly-spawn-event-announcement";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "sighting";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler.AppendLiteral("anomaly-spawn-sighting-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._random.Next(1, 6));
			array[num] = new ValueTuple<string, object>(item, Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()));
			string str = Loc.GetString(text, array);
			this.ChatSystem.DispatchGlobalAnnouncement(str, "Central Command", true, null, new Color?(Color.FromHex("#18abf5", null)));
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00025AFC File Offset: 0x00023CFC
		public override void Started()
		{
			base.Started();
			if (this.StationSystem.Stations.Count == 0)
			{
				return;
			}
			EntityUid chosenStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations.ToList<EntityUid>());
			StationDataComponent stationData;
			if (!base.TryComp<StationDataComponent>(chosenStation, ref stationData))
			{
				return;
			}
			EntityUid? grid = null;
			foreach (EntityUid g in stationData.Grids.Where(new Func<EntityUid, bool>(base.HasComp<BecomesStationComponent>)))
			{
				grid = new EntityUid?(g);
			}
			if (grid == null)
			{
				return;
			}
			int amountToSpawn = Math.Max(1, (int)MathF.Round(base.GetSeverityModifier() / 2f));
			for (int i = 0; i < amountToSpawn; i++)
			{
				this._anomaly.SpawnOnRandomGridLocation(grid.Value, this.AnomalySpawnerPrototype);
			}
		}

		// Token: 0x0400049C RID: 1180
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400049D RID: 1181
		[Dependency]
		private readonly AnomalySystem _anomaly;

		// Token: 0x0400049E RID: 1182
		public readonly string AnomalySpawnerPrototype = "RandomAnomalySpawner";
	}
}
