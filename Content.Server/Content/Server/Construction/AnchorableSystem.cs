using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Coordinates.Helpers;
using Content.Server.Popups;
using Content.Server.Pulling;
using Content.Shared.Administration.Logs;
using Content.Shared.Construction.Components;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Popups;
using Content.Shared.Pulling.Components;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Server.Construction
{
	// Token: 0x020005ED RID: 1517
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnchorableSystem : SharedAnchorableSystem
	{
		// Token: 0x06002052 RID: 8274 RVA: 0x000A87D4 File Offset: 0x000A69D4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AnchorableComponent, AnchorableSystem.TryAnchorCompletedEvent>(new ComponentEventHandler<AnchorableComponent, AnchorableSystem.TryAnchorCompletedEvent>(this.OnAnchorComplete), null, null);
			base.SubscribeLocalEvent<AnchorableComponent, AnchorableSystem.TryUnanchorCompletedEvent>(new ComponentEventHandler<AnchorableComponent, AnchorableSystem.TryUnanchorCompletedEvent>(this.OnUnanchorComplete), null, null);
			base.SubscribeLocalEvent<AnchorableComponent, ExaminedEvent>(new ComponentEventHandler<AnchorableComponent, ExaminedEvent>(this.OnAnchoredExamine), null, null);
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x000A8824 File Offset: 0x000A6A24
		private void OnAnchoredExamine(EntityUid uid, AnchorableComponent component, ExaminedEvent args)
		{
			string messageId = base.Comp<TransformComponent>(uid).Anchored ? "examinable-anchored" : "examinable-unanchored";
			args.PushMarkup(Loc.GetString(messageId, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", uid)
			}));
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x000A8878 File Offset: 0x000A6A78
		private void OnUnanchorComplete(EntityUid uid, AnchorableComponent component, AnchorableSystem.TryUnanchorCompletedEvent args)
		{
			TransformComponent transformComponent = base.Transform(uid);
			base.RaiseLocalEvent<BeforeUnanchoredEvent>(uid, new BeforeUnanchoredEvent(args.User, args.Using), false);
			transformComponent.Anchored = false;
			base.RaiseLocalEvent<UserUnanchoredEvent>(uid, new UserUnanchoredEvent(args.User, args.Using), false);
			this._popup.PopupEntity(Loc.GetString("anchorable-unanchored"), uid, PopupType.Small);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Unanchor;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(19, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.User), "user", "EntityManager.ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" unanchored ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(uid), "anchored", "EntityManager.ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" using ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.Using), "using", "EntityManager.ToPrettyString(args.Using)");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000A8970 File Offset: 0x000A6B70
		private void OnAnchorComplete(EntityUid uid, AnchorableComponent component, AnchorableSystem.TryAnchorCompletedEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			PhysicsComponent anchorBody;
			if (base.TryComp<PhysicsComponent>(uid, ref anchorBody) && !this.TileFree(xform.Coordinates, anchorBody))
			{
				this._popup.PopupEntity(Loc.GetString("anchorable-occupied"), uid, args.User, PopupType.Small);
				return;
			}
			Angle rot = xform.LocalRotation;
			xform.LocalRotation = Math.Round(rot / 1.5707963267948966) * 1.5707963267948966;
			SharedPullableComponent pullable;
			if (base.TryComp<SharedPullableComponent>(uid, ref pullable) && pullable.Puller != null)
			{
				this._pulling.TryStopPull(pullable, null);
			}
			if (component.Snap)
			{
				xform.Coordinates = xform.Coordinates.SnapToGrid(this.EntityManager, this._mapManager);
			}
			base.RaiseLocalEvent<BeforeAnchoredEvent>(uid, new BeforeAnchoredEvent(args.User, args.Using), false);
			xform.Anchored = true;
			base.RaiseLocalEvent<UserAnchoredEvent>(uid, new UserAnchoredEvent(args.User, args.Using), false);
			this._popup.PopupEntity(Loc.GetString("anchorable-anchored"), uid, PopupType.Small);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Anchor;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.User), "user", "EntityManager.ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" anchored ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(uid), "anchored", "EntityManager.ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" using ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.Using), "using", "EntityManager.ToPrettyString(args.Using)");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x000A8B2C File Offset: 0x000A6D2C
		private bool TileFree(EntityCoordinates coordinates, PhysicsComponent anchorBody)
		{
			EntityUid? gridUid = coordinates.GetGridUid(this.EntityManager);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(gridUid, ref grid))
			{
				return false;
			}
			Vector2i tileIndices = grid.TileIndicesFor(coordinates);
			AnchoredEntitiesEnumerator enumerator = grid.GetAnchoredEntitiesEnumerator(tileIndices);
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			EntityUid? ent;
			while (enumerator.MoveNext(ref ent))
			{
				PhysicsComponent body;
				if (bodyQuery.TryGetComponent(ent, ref body) && body.CanCollide && body.Hard && ((body.CollisionMask & anchorBody.CollisionLayer) != 0 || (body.CollisionLayer & anchorBody.CollisionMask) != 0))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x000A8BC0 File Offset: 0x000A6DC0
		[NullableContext(2)]
		private bool Valid(EntityUid uid, EntityUid userUid, EntityUid usingUid, bool anchoring, AnchorableComponent anchorable = null, ToolComponent usingTool = null)
		{
			if (!base.Resolve<AnchorableComponent>(uid, ref anchorable, true))
			{
				return false;
			}
			if (!base.Resolve<ToolComponent>(usingUid, ref usingTool, true))
			{
				return false;
			}
			BaseAnchoredAttemptEvent attempt = anchoring ? new AnchorAttemptEvent(userUid, usingUid) : new UnanchorAttemptEvent(userUid, usingUid);
			if (anchoring)
			{
				base.RaiseLocalEvent<AnchorAttemptEvent>(uid, (AnchorAttemptEvent)attempt, false);
			}
			else
			{
				base.RaiseLocalEvent<UnanchorAttemptEvent>(uid, (UnanchorAttemptEvent)attempt, false);
			}
			anchorable.Delay += attempt.Delay;
			return !attempt.Cancelled;
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x000A8C40 File Offset: 0x000A6E40
		[NullableContext(2)]
		private void TryAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent anchorable = null, TransformComponent transform = null, SharedPullableComponent pullable = null, ToolComponent usingTool = null)
		{
			if (!base.Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true))
			{
				return;
			}
			base.Resolve<SharedPullableComponent>(uid, ref pullable, false);
			if (!base.Resolve<ToolComponent>(usingUid, ref usingTool, true))
			{
				return;
			}
			if (!this.Valid(uid, userUid, usingUid, true, anchorable, usingTool))
			{
				return;
			}
			PhysicsComponent anchorBody;
			if (base.TryComp<PhysicsComponent>(uid, ref anchorBody) && !this.TileFree(transform.Coordinates, anchorBody))
			{
				this._popup.PopupEntity(Loc.GetString("anchorable-occupied"), uid, userUid, PopupType.Small);
				return;
			}
			ToolEventData toolEvData = new ToolEventData(new AnchorableSystem.TryAnchorCompletedEvent(userUid, usingUid), 0f, null, new EntityUid?(uid));
			this._tool.UseTool(usingUid, userUid, new EntityUid?(uid), anchorable.Delay, usingTool.Qualities, toolEvData, 0f, null, null, null);
		}

		// Token: 0x06002059 RID: 8281 RVA: 0x000A8D00 File Offset: 0x000A6F00
		[NullableContext(2)]
		private void TryUnAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent anchorable = null, TransformComponent transform = null, ToolComponent usingTool = null)
		{
			if (!base.Resolve<AnchorableComponent, TransformComponent>(uid, ref anchorable, ref transform, true))
			{
				return;
			}
			if (!base.Resolve<ToolComponent>(usingUid, ref usingTool, true))
			{
				return;
			}
			if (!this.Valid(uid, userUid, usingUid, false, null, null))
			{
				return;
			}
			ToolEventData toolEvData = new ToolEventData(new AnchorableSystem.TryUnanchorCompletedEvent(userUid, usingUid), 0f, null, new EntityUid?(uid));
			this._tool.UseTool(usingUid, userUid, new EntityUid?(uid), anchorable.Delay, usingTool.Qualities, toolEvData, 0f, null, null, null);
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x000A8D7C File Offset: 0x000A6F7C
		[NullableContext(2)]
		public override void TryToggleAnchor(EntityUid uid, EntityUid userUid, EntityUid usingUid, AnchorableComponent anchorable = null, TransformComponent transform = null, SharedPullableComponent pullable = null, ToolComponent usingTool = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref transform, true))
			{
				return;
			}
			LogStringHandler logStringHandler;
			if (transform.Anchored)
			{
				this.TryUnAnchor(uid, userUid, usingUid, anchorable, transform, usingTool);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Anchor;
				LogImpact impact = LogImpact.Low;
				logStringHandler = new LogStringHandler(29, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(userUid), "user", "ToPrettyString(userUid)");
				logStringHandler.AppendLiteral(" is trying to unanchor ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" from ");
				logStringHandler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
				adminLogger.Add(type, impact, ref logStringHandler);
				return;
			}
			this.TryAnchor(uid, userUid, usingUid, anchorable, transform, pullable, usingTool);
			ISharedAdminLogManager adminLogger2 = this._adminLogger;
			LogType type2 = LogType.Anchor;
			LogImpact impact2 = LogImpact.Low;
			logStringHandler = new LogStringHandler(25, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(userUid), "user", "ToPrettyString(userUid)");
			logStringHandler.AppendLiteral(" is trying to anchor ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<EntityCoordinates>(transform.Coordinates, "targetlocation", "transform.Coordinates");
			adminLogger2.Add(type2, impact2, ref logStringHandler);
		}

		// Token: 0x04001403 RID: 5123
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001404 RID: 5124
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001405 RID: 5125
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04001406 RID: 5126
		[Dependency]
		private readonly SharedToolSystem _tool;

		// Token: 0x04001407 RID: 5127
		[Dependency]
		private readonly PullingSystem _pulling;

		// Token: 0x02000ABF RID: 2751
		[NullableContext(0)]
		private abstract class AnchorEvent : EntityEventArgs
		{
			// Token: 0x060035B8 RID: 13752 RVA: 0x0011DA54 File Offset: 0x0011BC54
			protected AnchorEvent(EntityUid userUid, EntityUid usingUid)
			{
				this.User = userUid;
				this.Using = usingUid;
			}

			// Token: 0x040027B6 RID: 10166
			public EntityUid User;

			// Token: 0x040027B7 RID: 10167
			public EntityUid Using;
		}

		// Token: 0x02000AC0 RID: 2752
		[NullableContext(0)]
		private sealed class TryUnanchorCompletedEvent : AnchorableSystem.AnchorEvent
		{
			// Token: 0x060035B9 RID: 13753 RVA: 0x0011DA6A File Offset: 0x0011BC6A
			public TryUnanchorCompletedEvent(EntityUid userUid, EntityUid usingUid) : base(userUid, usingUid)
			{
			}
		}

		// Token: 0x02000AC1 RID: 2753
		[NullableContext(0)]
		private sealed class TryAnchorCompletedEvent : AnchorableSystem.AnchorEvent
		{
			// Token: 0x060035BA RID: 13754 RVA: 0x0011DA74 File Offset: 0x0011BC74
			public TryAnchorCompletedEvent(EntityUid userUid, EntityUid usingUid) : base(userUid, usingUid)
			{
			}
		}
	}
}
