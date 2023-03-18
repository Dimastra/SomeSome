using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Ghost.Components;
using Content.Server.Pointing.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Bed.Sleep;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Helpers;
using Content.Shared.Mobs.Systems;
using Content.Shared.Pointing;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Replays;
using Robust.Shared.Timing;

namespace Content.Server.Pointing.EntitySystems
{
	// Token: 0x020002CB RID: 715
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PointingSystem : SharedPointingSystem
	{
		// Token: 0x06000E66 RID: 3686 RVA: 0x00048FC4 File Offset: 0x000471C4
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus != 4)
			{
				return;
			}
			this._pointers.Remove(e.Session);
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00048FE4 File Offset: 0x000471E4
		private void SendMessage(EntityUid source, IEnumerable<ICommonSession> viewers, EntityUid pointed, string selfMessage, string viewerMessage, [Nullable(2)] string viewerPointedAtMessage = null)
		{
			foreach (ICommonSession commonSession in viewers)
			{
				EntityUid? attachedEntity = commonSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid viewerEntity = attachedEntity.GetValueOrDefault();
					if (viewerEntity.Valid)
					{
						string message = (viewerEntity == source) ? selfMessage : ((viewerEntity == pointed && viewerPointedAtMessage != null) ? viewerPointedAtMessage : viewerMessage);
						base.RaiseNetworkEvent(new PopupEntityEvent(message, PopupType.Small, source), viewerEntity);
					}
				}
			}
			this._replay.QueueReplayMessage(new PopupEntityEvent(viewerMessage, PopupType.Small, source));
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x00049088 File Offset: 0x00047288
		public bool InRange(EntityUid pointer, EntityCoordinates coordinates)
		{
			if (base.HasComp<GhostComponent>(pointer))
			{
				return base.Transform(pointer).Coordinates.InRange(this.EntityManager, coordinates, 15f);
			}
			return pointer.InRangeUnOccluded(coordinates, 15f, (EntityUid e) => e == pointer, true);
		}

		// Token: 0x06000E69 RID: 3689 RVA: 0x000490F4 File Offset: 0x000472F4
		[NullableContext(2)]
		public bool TryPoint(ICommonSession session, EntityCoordinates coords, EntityUid pointed)
		{
			EntityUid? entityUid = (session != null) ? session.AttachedEntity : null;
			if (entityUid == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Player ");
				defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(" attempted to point without any attached entity");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			EntityUid player = entityUid.GetValueOrDefault();
			if (!coords.IsValid(this.EntityManager))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Player ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player));
				defaultInterpolatedStringHandler.AppendLiteral(" attempted to point at invalid coordinates: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coords);
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			TimeSpan lastTime;
			if (this._pointers.TryGetValue(session, out lastTime) && this._gameTiming.CurTime < lastTime + PointingSystem.PointDelay)
			{
				return false;
			}
			if (base.HasComp<PointingArrowComponent>(pointed))
			{
				return false;
			}
			if (this._mobState.IsIncapacitated(player, null))
			{
				return false;
			}
			if (base.HasComp<SleepingComponent>(player))
			{
				return false;
			}
			if (!this.InRange(player, coords))
			{
				this._popup.PopupEntity(Loc.GetString("pointing-system-try-point-cannot-reach"), player, player, PopupType.Small);
				return false;
			}
			MapCoordinates mapCoords = coords.ToMap(this.EntityManager);
			this._rotateToFaceSystem.TryFaceCoordinates(player, mapCoords.Position, null);
			EntityUid arrow = this.EntityManager.SpawnEntity("PointingArrow", coords);
			PointingArrowComponent pointing;
			if (base.TryComp<PointingArrowComponent>(arrow, ref pointing))
			{
				pointing.EndTime = this._gameTiming.CurTime + TimeSpan.FromSeconds(4.0);
			}
			PointingArrowComponent pointingArrowComponent;
			if (base.EntityQuery<PointingArrowAngeringComponent>(false).FirstOrDefault<PointingArrowAngeringComponent>() != null && base.TryComp<PointingArrowComponent>(arrow, ref pointingArrowComponent))
			{
				pointingArrowComponent.Rogue = true;
			}
			int layer = 1;
			VisibilityComponent playerVisibility;
			if (base.TryComp<VisibilityComponent>(player, ref playerVisibility))
			{
				VisibilityComponent arrowVisibility = this.EntityManager.EnsureComponent<VisibilityComponent>(arrow);
				layer = playerVisibility.Layer;
				this._visibilitySystem.SetLayer(arrowVisibility, layer, true);
			}
			IEnumerable<ICommonSession> viewers = Filter.Empty().AddWhere((ICommonSession session1) => base.<TryPoint>g__ViewerPredicate|0((IPlayerSession)session1), null).Recipients;
			string viewerPointedAtMessage = null;
			EntityUid playerName = Identity.Entity(player, this.EntityManager);
			string selfMessage;
			string viewerMessage;
			if (base.Exists(pointed))
			{
				EntityUid pointedName = Identity.Entity(pointed, this.EntityManager);
				selfMessage = ((player == pointed) ? Loc.GetString("pointing-system-point-at-self") : Loc.GetString("pointing-system-point-at-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("other", pointedName)
				}));
				viewerMessage = ((player == pointed) ? Loc.GetString("pointing-system-point-at-self-others", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("otherName", playerName),
					new ValueTuple<string, object>("other", playerName)
				}) : Loc.GetString("pointing-system-point-at-other-others", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("otherName", playerName),
					new ValueTuple<string, object>("other", pointedName)
				}));
				viewerPointedAtMessage = Loc.GetString("pointing-system-point-at-you-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("otherName", playerName)
				});
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "user", "ToPrettyString(player)");
				logStringHandler.AppendLiteral(" pointed at ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(pointed), "target", "ToPrettyString(pointed)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(pointed).Coordinates, "Transform(pointed).Coordinates");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				TileRef? tileRef = null;
				string position = null;
				MapGridComponent grid;
				if (this._mapManager.TryFindGridAt(mapCoords, ref grid))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendLiteral("EntId=");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(grid.Owner);
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted<Vector2i>(grid.WorldToTile(mapCoords.Position));
					position = defaultInterpolatedStringHandler.ToStringAndClear();
					tileRef = new TileRef?(grid.GetTileRef(grid.WorldToTile(mapCoords.Position)));
				}
				string name = Loc.GetString(this._tileDefinitionManager[(int)((tileRef != null) ? tileRef.GetValueOrDefault().Tile.TypeId : 0)].Name);
				selfMessage = Loc.GetString("pointing-system-point-at-tile", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("tileName", name)
				});
				viewerMessage = Loc.GetString("pointing-system-other-point-at-tile", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("otherName", playerName),
					new ValueTuple<string, object>("tileName", name)
				});
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Action;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "user", "ToPrettyString(player)");
				logStringHandler.AppendLiteral(" pointed at ");
				logStringHandler.AppendFormatted(name);
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted((position == null) ? mapCoords : position, 0, null);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			this._pointers[session] = this._gameTiming.CurTime;
			this.SendMessage(player, viewers, pointed, selfMessage, viewerMessage, viewerPointedAtMessage);
			return true;
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x000496B8 File Offset: 0x000478B8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<PointingAttemptEvent>(new EntitySessionEventHandler<PointingAttemptEvent>(this.OnPointAttempt), null, null);
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			CommandBinds.Builder.Bind(ContentKeyFunctions.Point, new PointerInputCmdHandler(new PointerInputCmdDelegate(this.TryPoint), true, false)).Register<PointingSystem>();
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x00049720 File Offset: 0x00047920
		private void OnPointAttempt(PointingAttemptEvent ev, EntitySessionEventArgs args)
		{
			TransformComponent xform;
			if (base.TryComp<TransformComponent>(ev.Target, ref xform))
			{
				this.TryPoint(args.SenderSession, xform.Coordinates, ev.Target);
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
			defaultInterpolatedStringHandler.AppendLiteral("User ");
			defaultInterpolatedStringHandler.AppendFormatted<ICommonSession>(args.SenderSession);
			defaultInterpolatedStringHandler.AppendLiteral(" attempted to point at a non-existent entity uid: ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(ev.Target);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x000497A1 File Offset: 0x000479A1
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
			this._pointers.Clear();
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x000497CC File Offset: 0x000479CC
		public override void Update(float frameTime)
		{
			TimeSpan currentTime = this._gameTiming.CurTime;
			foreach (PointingArrowComponent component in base.EntityQuery<PointingArrowComponent>(true))
			{
				this.Update(component, currentTime);
			}
		}

		// Token: 0x06000E6E RID: 3694 RVA: 0x00049828 File Offset: 0x00047A28
		private void Update(PointingArrowComponent component, TimeSpan currentTime)
		{
			if (component.EndTime > currentTime)
			{
				return;
			}
			if (component.Rogue)
			{
				base.RemComp<PointingArrowComponent>(component.Owner);
				base.EnsureComp<RoguePointingArrowComponent>(component.Owner);
				return;
			}
			base.Del(component.Owner);
		}

		// Token: 0x0400087B RID: 2171
		[Dependency]
		private readonly IReplayRecordingManager _replay;

		// Token: 0x0400087C RID: 2172
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400087D RID: 2173
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400087E RID: 2174
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x0400087F RID: 2175
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000880 RID: 2176
		[Dependency]
		private readonly RotateToFaceSystem _rotateToFaceSystem;

		// Token: 0x04000881 RID: 2177
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000882 RID: 2178
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000883 RID: 2179
		[Dependency]
		private readonly VisibilitySystem _visibilitySystem;

		// Token: 0x04000884 RID: 2180
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000885 RID: 2181
		private static readonly TimeSpan PointDelay = TimeSpan.FromSeconds(0.5);

		// Token: 0x04000886 RID: 2182
		private readonly Dictionary<ICommonSession, TimeSpan> _pointers = new Dictionary<ICommonSession, TimeSpan>();

		// Token: 0x04000887 RID: 2183
		private const float PointingRange = 15f;
	}
}
