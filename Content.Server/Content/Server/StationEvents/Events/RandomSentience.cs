using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x0200018D RID: 397
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomSentience : StationEventSystem
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060007D8 RID: 2008 RVA: 0x00027093 File Offset: 0x00025293
		public override string Prototype
		{
			get
			{
				return "RandomSentience";
			}
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0002709C File Offset: 0x0002529C
		public override void Started()
		{
			base.Started();
			HashSet<EntityUid> stationsToNotify = new HashSet<EntityUid>();
			float mod = base.GetSeverityModifier();
			List<SentienceTargetComponent> targetList = this.EntityManager.EntityQuery<SentienceTargetComponent>(false).ToList<SentienceTargetComponent>();
			this.RobustRandom.Shuffle<SentienceTargetComponent>(targetList);
			int toMakeSentient = (int)((double)this.RobustRandom.Next(2, 5) * Math.Sqrt((double)mod));
			HashSet<string> groups = new HashSet<string>();
			foreach (SentienceTargetComponent target in targetList)
			{
				if (toMakeSentient-- == 0)
				{
					break;
				}
				this.EntityManager.RemoveComponent<SentienceTargetComponent>(target.Owner);
				GhostTakeoverAvailableComponent comp = this.EntityManager.AddComponent<GhostTakeoverAvailableComponent>(target.Owner);
				comp.RoleName = this.EntityManager.GetComponent<MetaDataComponent>(target.Owner).EntityName;
				comp.RoleDescription = Loc.GetString("station-event-random-sentience-role-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", comp.RoleName)
				});
				groups.Add(Loc.GetString(target.FlavorKind));
			}
			if (groups.Count == 0)
			{
				return;
			}
			List<string> groupList = groups.ToList<string>();
			string kind = (groupList.Count > 0) ? groupList[0] : "???";
			string kind2 = (groupList.Count > 1) ? groupList[1] : "???";
			string kind3 = (groupList.Count > 2) ? groupList[2] : "???";
			IEntitySystemManager entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
			StationSystem stationSystem = entitySystemManager.GetEntitySystem<StationSystem>();
			ChatSystem chatSystem = entitySystemManager.GetEntitySystem<ChatSystem>();
			foreach (SentienceTargetComponent target2 in targetList)
			{
				EntityUid? station = stationSystem.GetOwningStation(target2.Owner, null);
				if (station != null)
				{
					stationsToNotify.Add(station.Value);
				}
			}
			foreach (EntityUid station2 in stationsToNotify)
			{
				ChatSystem chatSystem2 = chatSystem;
				EntityUid source = station2;
				string text = "station-event-random-sentience-announcement";
				ValueTuple<string, object>[] array = new ValueTuple<string, object>[6];
				array[0] = new ValueTuple<string, object>("kind1", kind);
				array[1] = new ValueTuple<string, object>("kind2", kind2);
				array[2] = new ValueTuple<string, object>("kind3", kind3);
				array[3] = new ValueTuple<string, object>("amount", groupList.Count);
				int num = 4;
				string item = "data";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("random-sentience-event-data-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.RobustRandom.Next(1, 6));
				array[num] = new ValueTuple<string, object>(item, Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()));
				int num2 = 5;
				string item2 = "strength";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("random-sentience-event-strength-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.RobustRandom.Next(1, 8));
				array[num2] = new ValueTuple<string, object>(item2, Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()));
				chatSystem2.DispatchStationAnnouncement(source, Loc.GetString(text, array), "Central Command", false, null, new Color?(Color.Gold));
			}
		}
	}
}
