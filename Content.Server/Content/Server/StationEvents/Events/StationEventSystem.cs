using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Database;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events
{
	// Token: 0x02000191 RID: 401
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class StationEventSystem : GameRuleSystem
	{
		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x00027734 File Offset: 0x00025934
		// (set) Token: 0x060007EB RID: 2027 RVA: 0x0002773C File Offset: 0x0002593C
		protected float Elapsed { get; set; }

		// Token: 0x060007EC RID: 2028 RVA: 0x00027745 File Offset: 0x00025945
		public override void Initialize()
		{
			base.Initialize();
			this.Sawmill = Logger.GetSawmill("stationevents");
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00027760 File Offset: 0x00025960
		public override void Started()
		{
			ISharedAdminLogManager adminLogManager = this.AdminLogManager;
			LogType type = LogType.EventStarted;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(15, 1);
			logStringHandler.AppendLiteral("Event started: ");
			logStringHandler.AppendFormatted(this.Configuration.Id);
			adminLogManager.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x000277A4 File Offset: 0x000259A4
		public override void Added()
		{
			ISharedAdminLogManager adminLogManager = this.AdminLogManager;
			LogType type = LogType.EventAnnounced;
			LogStringHandler logStringHandler = new LogStringHandler(25, 1);
			logStringHandler.AppendLiteral("Event added / announced: ");
			logStringHandler.AppendFormatted(this.Configuration.Id);
			adminLogManager.Add(type, ref logStringHandler);
			StationEventRuleConfiguration ev = this.Configuration as StationEventRuleConfiguration;
			if (ev == null)
			{
				return;
			}
			if (ev.StartAnnouncement != null)
			{
				this.ChatSystem.DispatchGlobalAnnouncement(Loc.GetString(ev.StartAnnouncement), "Central Command", false, null, new Color?(Color.Gold));
			}
			if (ev.StartAudio != null)
			{
				SoundSystem.Play(ev.StartAudio.GetSound(null, null), Filter.Broadcast(), new AudioParams?(ev.StartAudio.Params));
			}
			this.Elapsed = 0f;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00027864 File Offset: 0x00025A64
		public override void Ended()
		{
			ISharedAdminLogManager adminLogManager = this.AdminLogManager;
			LogType type = LogType.EventStopped;
			LogStringHandler logStringHandler = new LogStringHandler(13, 1);
			logStringHandler.AppendLiteral("Event ended: ");
			logStringHandler.AppendFormatted(this.Configuration.Id);
			adminLogManager.Add(type, ref logStringHandler);
			StationEventRuleConfiguration ev = this.Configuration as StationEventRuleConfiguration;
			if (ev == null)
			{
				return;
			}
			if (ev.EndAnnouncement != null)
			{
				this.ChatSystem.DispatchGlobalAnnouncement(Loc.GetString(ev.EndAnnouncement), "Central Command", false, null, new Color?(Color.Gold));
			}
			if (ev.EndAudio != null)
			{
				SoundSystem.Play(ev.EndAudio.GetSound(null, null), Filter.Broadcast(), new AudioParams?(ev.EndAudio.Params));
			}
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00027918 File Offset: 0x00025B18
		public override void Update(float frameTime)
		{
			if (base.RuleAdded)
			{
				StationEventRuleConfiguration data = this.Configuration as StationEventRuleConfiguration;
				if (data != null)
				{
					this.Elapsed += frameTime;
					if (!base.RuleStarted && this.Elapsed >= data.StartAfter)
					{
						this.GameTicker.StartGameRule(this.PrototypeManager.Index<GameRulePrototype>(this.Prototype));
					}
					if (base.RuleStarted && this.Elapsed >= data.EndAfter)
					{
						this.GameTicker.EndGameRule(this.PrototypeManager.Index<GameRulePrototype>(this.Prototype));
					}
					return;
				}
			}
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x000279AF File Offset: 0x00025BAF
		protected void ForceEndSelf()
		{
			this.GameTicker.EndGameRule(this.PrototypeManager.Index<GameRulePrototype>(this.Prototype));
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x000279D0 File Offset: 0x00025BD0
		protected bool TryFindRandomTile(out Vector2i tile, out EntityUid targetStation, out EntityUid targetGrid, out EntityCoordinates targetCoords)
		{
			tile = default(Vector2i);
			targetCoords = EntityCoordinates.Invalid;
			if (this.StationSystem.Stations.Count == 0)
			{
				targetStation = EntityUid.Invalid;
				targetGrid = EntityUid.Invalid;
				return false;
			}
			targetStation = RandomExtensions.Pick<EntityUid>(this.RobustRandom, this.StationSystem.Stations);
			HashSet<EntityUid> possibleTargets = base.Comp<StationDataComponent>(targetStation).Grids;
			if (possibleTargets.Count == 0)
			{
				targetGrid = EntityUid.Invalid;
				return false;
			}
			targetGrid = RandomExtensions.Pick<EntityUid>(this.RobustRandom, possibleTargets);
			MapGridComponent gridComp;
			if (!base.TryComp<MapGridComponent>(targetGrid, ref gridComp))
			{
				return false;
			}
			bool found = false;
			ValueTuple<Vector2, Angle, Matrix3> worldPositionRotationMatrix = base.Transform(targetGrid).GetWorldPositionRotationMatrix();
			Vector2 gridPos = worldPositionRotationMatrix.Item1;
			Matrix3 gridMatrix = worldPositionRotationMatrix.Item3;
			Box2 localAABB = gridComp.LocalAABB;
			Box2 gridBounds = gridMatrix.TransformBox(ref localAABB);
			for (int i = 0; i < 10; i++)
			{
				int randomX = this.RobustRandom.Next((int)gridBounds.Left, (int)gridBounds.Right);
				int randomY = this.RobustRandom.Next((int)gridBounds.Bottom, (int)gridBounds.Top);
				tile = new Vector2i(randomX - (int)gridPos.X, randomY - (int)gridPos.Y);
				if (!this._atmosphere.IsTileSpace(new EntityUid?(gridComp.Owner), base.Transform(targetGrid).MapUid, tile, gridComp) && !this._atmosphere.IsTileAirBlocked(gridComp.Owner, tile, AtmosDirection.All, gridComp))
				{
					found = true;
					targetCoords = gridComp.GridTileToLocal(tile);
					break;
				}
			}
			return found;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00027B8E File Offset: 0x00025D8E
		[NullableContext(2)]
		[return: Nullable(1)]
		public static GameRulePrototype GetRandomEventUnweighted(IPrototypeManager prototypeManager = null, IRobustRandom random = null)
		{
			IoCManager.Resolve<IPrototypeManager, IRobustRandom>(ref prototypeManager, ref random);
			return RandomExtensions.Pick<GameRulePrototype>(random, (from p in prototypeManager.EnumeratePrototypes<GameRulePrototype>()
			where p.Configuration is StationEventRuleConfiguration
			select p).ToArray<GameRulePrototype>());
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00027BD0 File Offset: 0x00025DD0
		public float GetSeverityModifier()
		{
			GetSeverityModifierEvent ev = new GetSeverityModifierEvent();
			base.RaiseLocalEvent<GetSeverityModifierEvent>(ev);
			return ev.Modifier;
		}

		// Token: 0x040004D6 RID: 1238
		[Dependency]
		protected readonly IRobustRandom RobustRandom;

		// Token: 0x040004D7 RID: 1239
		[Dependency]
		protected readonly IAdminLogManager AdminLogManager;

		// Token: 0x040004D8 RID: 1240
		[Dependency]
		protected readonly IPrototypeManager PrototypeManager;

		// Token: 0x040004D9 RID: 1241
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x040004DA RID: 1242
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x040004DB RID: 1243
		[Dependency]
		protected readonly ChatSystem ChatSystem;

		// Token: 0x040004DC RID: 1244
		[Dependency]
		protected readonly StationSystem StationSystem;

		// Token: 0x040004DD RID: 1245
		protected ISawmill Sawmill;
	}
}
