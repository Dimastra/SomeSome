using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000185 RID: 389
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BureaucraticError : StationEventSystem
	{
		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x000260E0 File Offset: 0x000242E0
		public override string Prototype
		{
			get
			{
				return "BureaucraticError";
			}
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x000260E8 File Offset: 0x000242E8
		public override void Started()
		{
			base.Started();
			if (this.StationSystem.Stations.Count == 0)
			{
				return;
			}
			EntityUid chosenStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations.ToList<EntityUid>());
			List<string> jobList = this._stationJobs.GetJobs(chosenStation, null).Keys.ToList<string>();
			float mod = base.GetSeverityModifier();
			if (RandomExtensions.Prob(this.RobustRandom, Math.Min(0.25f * MathF.Sqrt(mod), 1f)))
			{
				string chosenJob = RandomExtensions.PickAndTake<string>(this.RobustRandom, jobList);
				this._stationJobs.MakeJobUnlimited(chosenStation, chosenJob, null);
				using (List<string>.Enumerator enumerator = jobList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string job = enumerator.Current;
						if (!this._stationJobs.IsJobUnlimited(chosenStation, job, null))
						{
							this._stationJobs.TrySetJobSlot(chosenStation, job, 0, false, null);
						}
					}
					return;
				}
			}
			int lower = (int)((double)jobList.Count * Math.Min(1.0, 0.2 * (double)mod));
			int upper = (int)((double)jobList.Count * Math.Min(1.0, 0.3 * (double)mod));
			for (int i = 0; i < this.RobustRandom.Next(lower, upper); i++)
			{
				string chosenJob2 = RandomExtensions.PickAndTake<string>(this.RobustRandom, jobList);
				if (!this._stationJobs.IsJobUnlimited(chosenStation, chosenJob2, null))
				{
					this._stationJobs.TryAdjustJobSlot(chosenStation, chosenJob2, this.RobustRandom.Next(-3, 6), false, false, null);
				}
			}
		}

		// Token: 0x040004A6 RID: 1190
		[Dependency]
		private readonly StationJobsSystem _stationJobs;
	}
}
