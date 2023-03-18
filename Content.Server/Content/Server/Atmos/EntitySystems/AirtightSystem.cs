using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200078E RID: 1934
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirtightSystem : EntitySystem
	{
		// Token: 0x0600291D RID: 10525 RVA: 0x000D63D4 File Offset: 0x000D45D4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AirtightComponent, ComponentInit>(new ComponentEventHandler<AirtightComponent, ComponentInit>(this.OnAirtightInit), null, null);
			base.SubscribeLocalEvent<AirtightComponent, ComponentShutdown>(new ComponentEventHandler<AirtightComponent, ComponentShutdown>(this.OnAirtightShutdown), null, null);
			base.SubscribeLocalEvent<AirtightComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<AirtightComponent, AnchorStateChangedEvent>(this.OnAirtightPositionChanged), null, null);
			base.SubscribeLocalEvent<AirtightComponent, ReAnchorEvent>(new ComponentEventRefHandler<AirtightComponent, ReAnchorEvent>(this.OnAirtightReAnchor), null, null);
			base.SubscribeLocalEvent<AirtightComponent, MoveEvent>(new ComponentEventRefHandler<AirtightComponent, MoveEvent>(this.OnAirtightRotated), null, null);
		}

		// Token: 0x0600291E RID: 10526 RVA: 0x000D6448 File Offset: 0x000D4648
		private void OnAirtightInit(EntityUid uid, AirtightComponent airtight, ComponentInit args)
		{
			TransformComponent xform = this.EntityManager.GetComponent<TransformComponent>(uid);
			if (airtight.FixAirBlockedDirectionInitialize)
			{
				MoveEvent moveEvent;
				moveEvent..ctor(uid, default(EntityCoordinates), default(EntityCoordinates), Angle.Zero, xform.LocalRotation, xform, false);
				if (this.AirtightRotate(uid, airtight, ref moveEvent))
				{
					return;
				}
			}
			this.UpdatePosition(airtight, null);
		}

		// Token: 0x0600291F RID: 10527 RVA: 0x000D64A8 File Offset: 0x000D46A8
		private void OnAirtightShutdown(EntityUid uid, AirtightComponent airtight, ComponentShutdown args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (this._mapManager.TryGetGrid(xform.GridUid, ref grid) && base.MetaData(grid.Owner).EntityLifeStage > 3)
			{
				return;
			}
			this.SetAirblocked(uid, airtight, false, xform);
		}

		// Token: 0x06002920 RID: 10528 RVA: 0x000D64F4 File Offset: 0x000D46F4
		private void OnAirtightPositionChanged(EntityUid uid, AirtightComponent airtight, ref AnchorStateChangedEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (!base.TryComp<MapGridComponent>(xform.GridUid, ref grid))
			{
				return;
			}
			EntityUid? gridId = xform.GridUid;
			EntityCoordinates coords = xform.Coordinates;
			Vector2i tilePos = grid.TileIndicesFor(coords);
			airtight.LastPosition = new ValueTuple<EntityUid, Vector2i>(gridId.Value, tilePos);
			this.InvalidatePosition(gridId.Value, tilePos, false);
		}

		// Token: 0x06002921 RID: 10529 RVA: 0x000D6558 File Offset: 0x000D4758
		private void OnAirtightReAnchor(EntityUid uid, AirtightComponent airtight, ref ReAnchorEvent args)
		{
			foreach (EntityUid gridId in new EntityUid[]
			{
				args.OldGrid,
				args.Grid
			})
			{
				airtight.LastPosition = new ValueTuple<EntityUid, Vector2i>(gridId, args.TilePos);
				this.InvalidatePosition(gridId, args.TilePos, false);
			}
		}

		// Token: 0x06002922 RID: 10530 RVA: 0x000D65BC File Offset: 0x000D47BC
		private void OnAirtightRotated(EntityUid uid, AirtightComponent airtight, ref MoveEvent ev)
		{
			this.AirtightRotate(uid, airtight, ref ev);
		}

		// Token: 0x06002923 RID: 10531 RVA: 0x000D65C8 File Offset: 0x000D47C8
		private bool AirtightRotate(EntityUid uid, AirtightComponent airtight, ref MoveEvent ev)
		{
			if (!airtight.RotateAirBlocked || airtight.InitialAirBlockedDirection == 0)
			{
				return false;
			}
			airtight.CurrentAirBlockedDirection = (int)this.Rotate((AtmosDirection)airtight.InitialAirBlockedDirection, ev.NewRotation);
			this.UpdatePosition(airtight, ev.Component);
			AirtightChanged airtightEv = new AirtightChanged(uid, airtight);
			base.RaiseLocalEvent<AirtightChanged>(uid, ref airtightEv, false);
			return true;
		}

		// Token: 0x06002924 RID: 10532 RVA: 0x000D6620 File Offset: 0x000D4820
		public void SetAirblocked(EntityUid uid, AirtightComponent airtight, bool airblocked, [Nullable(2)] TransformComponent xform = null)
		{
			if (airtight.AirBlocked == airblocked)
			{
				return;
			}
			if (!base.Resolve<TransformComponent>(airtight.Owner, ref xform, true))
			{
				return;
			}
			airtight.AirBlocked = airblocked;
			this.UpdatePosition(airtight, xform);
			AirtightChanged airtightEv = new AirtightChanged(uid, airtight);
			base.RaiseLocalEvent<AirtightChanged>(uid, ref airtightEv, false);
		}

		// Token: 0x06002925 RID: 10533 RVA: 0x000D666C File Offset: 0x000D486C
		public void UpdatePosition(AirtightComponent airtight, [Nullable(2)] TransformComponent xform = null)
		{
			if (!base.Resolve<TransformComponent>(airtight.Owner, ref xform, true))
			{
				return;
			}
			MapGridComponent grid;
			if (!xform.Anchored || !this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			airtight.LastPosition = new ValueTuple<EntityUid, Vector2i>(xform.GridUid.Value, grid.TileIndicesFor(xform.Coordinates));
			this.InvalidatePosition(airtight.LastPosition.Item1, airtight.LastPosition.Item2, airtight.FixVacuum && !airtight.AirBlocked);
		}

		// Token: 0x06002926 RID: 10534 RVA: 0x000D6700 File Offset: 0x000D4900
		public void InvalidatePosition(EntityUid gridId, Vector2i pos, bool fixVacuum = false)
		{
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(gridId), ref grid))
			{
				return;
			}
			EntityUid gridUid = grid.Owner;
			EntityQuery<AirtightComponent> query = this.EntityManager.GetEntityQuery<AirtightComponent>();
			this._explosionSystem.UpdateAirtightMap(gridId, pos, grid, new EntityQuery<AirtightComponent>?(query));
			this._atmosphereSystem.InvalidateTile(gridUid, pos);
		}

		// Token: 0x06002927 RID: 10535 RVA: 0x000D6758 File Offset: 0x000D4958
		private AtmosDirection Rotate(AtmosDirection myDirection, Angle myAngle)
		{
			AtmosDirection newAirBlockedDirs = AtmosDirection.Invalid;
			if (myAngle == Angle.Zero)
			{
				return myDirection;
			}
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (myDirection.IsFlagSet(direction))
				{
					Angle angle = direction.ToAngle();
					angle += myAngle;
					newAirBlockedDirs |= angle.ToAtmosDirectionCardinal();
				}
			}
			return newAirBlockedDirs;
		}

		// Token: 0x04001995 RID: 6549
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04001996 RID: 6550
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001997 RID: 6551
		[Dependency]
		private readonly ExplosionSystem _explosionSystem;
	}
}
