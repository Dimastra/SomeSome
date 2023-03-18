using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.GameTicking;
using Content.Server.Ghost;
using Content.Server.Ghost.Components;
using Content.Server.Mind.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server.Mind
{
	// Token: 0x020003A1 RID: 929
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MindSystem : EntitySystem
	{
		// Token: 0x0600130F RID: 4879 RVA: 0x00062970 File Offset: 0x00060B70
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MindComponent, ComponentShutdown>(new ComponentEventHandler<MindComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<MindComponent, ExaminedEvent>(new ComponentEventHandler<MindComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<MindComponent, SuicideEvent>(new ComponentEventHandler<MindComponent, SuicideEvent>(this.OnSuicide), null, null);
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x000629BF File Offset: 0x00060BBF
		[NullableContext(2)]
		public void SetGhostOnShutdown(EntityUid uid, bool value, MindComponent mind = null)
		{
			if (!base.Resolve<MindComponent>(uid, ref mind, true))
			{
				return;
			}
			mind.GhostOnShutdown = value;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x000629D8 File Offset: 0x00060BD8
		public void LetsALogMindChange(EntityUid target)
		{
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.GhostRoleTaken;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(18, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "'target'", "ToPrettyString(target)");
			logStringHandler.AppendLiteral(" transfer mind out");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x00062A23 File Offset: 0x00060C23
		public void InternalAssignMind(EntityUid uid, Mind value, [Nullable(2)] MindComponent mind = null)
		{
			if (!base.Resolve<MindComponent>(uid, ref mind, true))
			{
				return;
			}
			mind.Mind = value;
			base.RaiseLocalEvent<MindAddedMessage>(uid, new MindAddedMessage(), true);
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x00062A46 File Offset: 0x00060C46
		[NullableContext(2)]
		public void InternalEjectMind(EntityUid uid, MindComponent mind = null)
		{
			if (!base.Resolve<MindComponent>(uid, ref mind, true))
			{
				return;
			}
			if (!base.Deleted(uid, null))
			{
				base.RaiseLocalEvent<MindRemovedMessage>(uid, new MindRemovedMessage(), true);
			}
			mind.Mind = null;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x00062A74 File Offset: 0x00060C74
		public void TransferMindProperties(MindComponent source, MindComponent target)
		{
			IEnumerable<PropertyInfo> props = from x in typeof(MindComponent).GetProperties().ToList<PropertyInfo>()
			where x.CanWrite
			select x;
			string[] includedProps = new string[]
			{
				"ShowExamineInfo",
				"GhostOnShutdown",
				"PreventGhosting"
			};
			foreach (PropertyInfo prop in props)
			{
				if (includedProps.Contains(prop.Name))
				{
					prop.SetValue(target, prop.GetValue(source));
				}
			}
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x00062B2C File Offset: 0x00060D2C
		private void OnShutdown(EntityUid uid, MindComponent mind, ComponentShutdown args)
		{
			if (this._gameTicker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			if (mind.HasMind)
			{
				Mind mind2 = mind.Mind;
				EntityUid? entityUid = (mind2 != null) ? mind2.VisitingEntity : null;
				if (entityUid != null)
				{
					EntityUid visiting = entityUid.GetValueOrDefault();
					if (visiting.Valid)
					{
						GhostComponent ghost;
						if (base.TryComp<GhostComponent>(visiting, ref ghost))
						{
							this._ghostSystem.SetCanReturnToBody(ghost, false);
						}
						mind.Mind.TransferTo(new EntityUid?(visiting), false, false);
						return;
					}
				}
				if (mind.GhostOnShutdown)
				{
					base.Transform(uid).AttachToGridOrMap();
					EntityCoordinates spawnPosition = base.Transform(uid).Coordinates;
					Timer.Spawn(0, delegate()
					{
						if (this._gameTicker.RunLevel != GameRunLevel.InRound)
						{
							return;
						}
						EntityUid? gridId = spawnPosition.GetGridUid(this.EntityManager);
						if (spawnPosition.IsValid(this.EntityManager))
						{
							EntityUid? entityUid2 = gridId;
							EntityUid invalid = EntityUid.Invalid;
							if ((entityUid2 == null || (entityUid2 != null && !(entityUid2.GetValueOrDefault() == invalid))) && this._mapManager.GridExists(gridId))
							{
								goto IL_9E;
							}
						}
						spawnPosition = this._gameTicker.GetObserverSpawnPoint();
						IL_9E:
						if (!spawnPosition.IsValid(this.EntityManager))
						{
							string text = "mind";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(74, 2);
							defaultInterpolatedStringHandler.AppendLiteral("Entity \"");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(uid));
							defaultInterpolatedStringHandler.AppendLiteral("\" for ");
							Mind mind3 = mind.Mind;
							defaultInterpolatedStringHandler.AppendFormatted((mind3 != null) ? mind3.CharacterName : null);
							defaultInterpolatedStringHandler.AppendLiteral(" was deleted, and no applicable spawn location is available.");
							Logger.WarningS(text, defaultInterpolatedStringHandler.ToStringAndClear());
							Mind mind4 = mind.Mind;
							if (mind4 == null)
							{
								return;
							}
							mind4.TransferTo(null, false, false);
							return;
						}
						else
						{
							EntityUid ghost2 = this.Spawn("MobObserver", spawnPosition);
							GhostComponent ghostComponent = this.Comp<GhostComponent>(ghost2);
							this._ghostSystem.SetCanReturnToBody(ghostComponent, false);
							string text2 = "mind";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 3);
							defaultInterpolatedStringHandler.AppendLiteral("Entity \"");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(uid));
							defaultInterpolatedStringHandler.AppendLiteral("\" for ");
							Mind mind5 = mind.Mind;
							defaultInterpolatedStringHandler.AppendFormatted((mind5 != null) ? mind5.CharacterName : null);
							defaultInterpolatedStringHandler.AppendLiteral(" was deleted, spawned \"");
							defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(ghost2));
							defaultInterpolatedStringHandler.AppendLiteral("\".");
							Logger.DebugS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
							if (mind.Mind == null)
							{
								return;
							}
							string val = mind.Mind.CharacterName ?? string.Empty;
							this.MetaData(ghost2).EntityName = val;
							mind.Mind.TransferTo(new EntityUid?(ghost2), false, false);
							return;
						}
					}, default(CancellationToken));
				}
			}
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x00062C30 File Offset: 0x00060E30
		private void OnExamined(EntityUid uid, MindComponent mind, ExaminedEvent args)
		{
			if (!mind.ShowExamineInfo || !args.IsInDetailsRange)
			{
				return;
			}
			MobStateComponent state;
			if (base.TryComp<MobStateComponent>(uid, ref state) && this._mobStateSystem.IsDead(uid, state))
			{
				Mind mind2 = mind.Mind;
				if (((mind2 != null) ? mind2.Session : null) == null)
				{
					args.PushMarkup("[color=yellow]" + Loc.GetString("mind-component-no-mind-and-dead-text", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("ent", uid)
					}) + "[/color]");
					return;
				}
				args.PushMarkup("[color=red]" + Loc.GetString("comp-mind-examined-dead", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ent", uid)
				}) + "[/color]");
				return;
			}
			else
			{
				if (!mind.HasMind)
				{
					args.PushMarkup("[color=mediumpurple]" + Loc.GetString("comp-mind-examined-catatonic", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("ent", uid)
					}) + "[/color]");
					return;
				}
				Mind mind3 = mind.Mind;
				if (((mind3 != null) ? mind3.Session : null) == null)
				{
					args.PushMarkup("[color=yellow]" + Loc.GetString("comp-mind-examined-ssd", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("ent", uid)
					}) + "[/color]");
				}
				return;
			}
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x00062D91 File Offset: 0x00060F91
		private void OnSuicide(EntityUid uid, MindComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (component.HasMind && component.Mind.PreventSuicide)
			{
				args.BlockSuicideAttempt(true);
			}
		}

		// Token: 0x04000BA7 RID: 2983
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000BA8 RID: 2984
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000BA9 RID: 2985
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000BAA RID: 2986
		[Dependency]
		private readonly GhostSystem _ghostSystem;

		// Token: 0x04000BAB RID: 2987
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;
	}
}
