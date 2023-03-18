using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Fax;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.White.StationGoal
{
	// Token: 0x0200008A RID: 138
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationGoalPaperSystem : EntitySystem
	{
		// Token: 0x0600020E RID: 526 RVA: 0x0000BD2A File Offset: 0x00009F2A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundStartedEvent>(new EntityEventHandler<RoundStartedEvent>(this.OnRoundStarted), null, null);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000BD46 File Offset: 0x00009F46
		private void OnRoundStarted(RoundStartedEvent ev)
		{
			this.SendRandomGoal();
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000BD50 File Offset: 0x00009F50
		public bool SendRandomGoal()
		{
			List<StationGoalPrototype> availableGoals = this._prototypeManager.EnumeratePrototypes<StationGoalPrototype>().ToList<StationGoalPrototype>();
			StationGoalPrototype goal = RandomExtensions.Pick<StationGoalPrototype>(this._random, availableGoals);
			return this.SendStationGoal(goal);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000BD84 File Offset: 0x00009F84
		public bool SendStationGoal(StationGoalPrototype goal)
		{
			IEnumerable<FaxMachineComponent> enumerable = this.EntityManager.EntityQuery<FaxMachineComponent>(false);
			bool wasSent = false;
			foreach (FaxMachineComponent fax in enumerable)
			{
				if (fax.ReceiveStationGoal)
				{
					FaxPrintout printout = new FaxPrintout(Loc.GetString(goal.Text), Loc.GetString("station-goal-fax-paper-name"), null, "paper_stamp-cent", new List<string>
					{
						Loc.GetString("stamp-component-stamped-name-centcom")
					});
					this._faxSystem.Receive(fax.Owner, printout, null, fax);
					wasSent = true;
				}
			}
			return wasSent;
		}

		// Token: 0x0400017D RID: 381
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400017E RID: 382
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400017F RID: 383
		[Dependency]
		private readonly FaxSystem _faxSystem;
	}
}
