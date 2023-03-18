using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Station.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000184 RID: 388
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BreakerFlip : StationEventSystem
	{
		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x00025F24 File Offset: 0x00024124
		public override string Prototype
		{
			get
			{
				return "BreakerFlip";
			}
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x00025F2C File Offset: 0x0002412C
		public override void Added()
		{
			base.Added();
			string text = "station-event-breaker-flip-announcement";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "data";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler.AppendLiteral("random-sentience-event-data-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.RobustRandom.Next(1, 6));
			array[num] = new ValueTuple<string, object>(item, Loc.GetString(Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear())));
			string str = Loc.GetString(text, array);
			this.ChatSystem.DispatchGlobalAnnouncement(str, "Central Command", false, null, new Color?(Color.Gold));
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x00025FBC File Offset: 0x000241BC
		public override void Started()
		{
			base.Started();
			if (this.StationSystem.Stations.Count == 0)
			{
				return;
			}
			EntityUid chosenStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations.ToList<EntityUid>());
			List<ApcComponent> stationApcs = new List<ApcComponent>();
			foreach (ValueTuple<ApcComponent, TransformComponent> valueTuple in base.EntityQuery<ApcComponent, TransformComponent>(false))
			{
				ApcComponent apc = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				if (apc.MainBreakerEnabled)
				{
					StationMemberComponent stationMemberComponent = base.CompOrNull<StationMemberComponent>(transform.GridUid);
					if (stationMemberComponent != null && stationMemberComponent.Station == chosenStation)
					{
						stationApcs.Add(apc);
					}
				}
			}
			int toDisable = Math.Min(this.RobustRandom.Next(3, 7), stationApcs.Count);
			if (toDisable == 0)
			{
				return;
			}
			this.RobustRandom.Shuffle<ApcComponent>(stationApcs);
			for (int i = 0; i < toDisable; i++)
			{
				this._apcSystem.ApcToggleBreaker(stationApcs[i].Owner, stationApcs[i], null);
			}
		}

		// Token: 0x040004A5 RID: 1189
		[Dependency]
		private readonly ApcSystem _apcSystem;
	}
}
