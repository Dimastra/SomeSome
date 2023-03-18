using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Components;
using Content.Server.Singularity.Components;
using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001EB RID: 491
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravityWellSystem : SharedGravityWellSystem
	{
		// Token: 0x06000973 RID: 2419 RVA: 0x00030020 File Offset: 0x0002E220
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GravityWellComponent, ComponentStartup>(new ComponentEventHandler<GravityWellComponent, ComponentStartup>(this.OnGravityWellStartup), null, null);
			this._vvManager.GetTypeHandler<GravityWellComponent>().AddPath<TimeSpan>("TargetPulsePeriod", (EntityUid _, GravityWellComponent comp) => comp.TargetPulsePeriod, new ComponentPropertySetter<GravityWellComponent, TimeSpan>(this.SetPulsePeriod));
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x00030088 File Offset: 0x0002E288
		public override void Shutdown()
		{
			this._vvManager.GetTypeHandler<GravityWellComponent>().RemovePath("TargetPulsePeriod");
			base.Shutdown();
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x000300A8 File Offset: 0x0002E2A8
		public override void Update(float frameTime)
		{
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			foreach (ValueTuple<GravityWellComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<GravityWellComponent, TransformComponent>(false))
			{
				GravityWellComponent gravWell = valueTuple.Item1;
				TransformComponent xform = valueTuple.Item2;
				TimeSpan curTime = this._timing.CurTime;
				if (gravWell.NextPulseTime <= curTime)
				{
					this.Update(gravWell.Owner, curTime - gravWell.LastPulseTime, gravWell, xform);
				}
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x00030144 File Offset: 0x0002E344
		[NullableContext(2)]
		private void Update(EntityUid uid, GravityWellComponent gravWell = null, TransformComponent xform = null)
		{
			if (base.Resolve<GravityWellComponent>(uid, ref gravWell, true))
			{
				this.Update(uid, this._timing.CurTime - gravWell.LastPulseTime, gravWell, xform);
			}
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x00030174 File Offset: 0x0002E374
		[NullableContext(2)]
		private void Update(EntityUid uid, TimeSpan frameTime, GravityWellComponent gravWell = null, TransformComponent xform = null)
		{
			if (!base.Resolve<GravityWellComponent>(uid, ref gravWell, true))
			{
				return;
			}
			gravWell.LastPulseTime = this._timing.CurTime;
			gravWell.NextPulseTime = gravWell.LastPulseTime + gravWell.TargetPulsePeriod;
			if (gravWell.MaxRange < 0f || !base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return;
			}
			float scale = (float)frameTime.TotalSeconds;
			this.GravPulse(uid, gravWell.MaxRange, gravWell.MinRange, gravWell.BaseRadialAcceleration * scale, gravWell.BaseTangentialAcceleration * scale, xform);
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x000301FE File Offset: 0x0002E3FE
		private bool CanGravPulseAffect(EntityUid entity)
		{
			return !this.EntityManager.HasComponent<GhostComponent>(entity) && !this.EntityManager.HasComponent<MapGridComponent>(entity) && !this.EntityManager.HasComponent<MapComponent>(entity) && !this.EntityManager.HasComponent<GravityWellComponent>(entity);
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0003023B File Offset: 0x0002E43B
		[NullableContext(2)]
		public void GravPulse(EntityUid uid, float maxRange, float minRange, in Matrix3 baseMatrixDeltaV, TransformComponent xform = null)
		{
			if (base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				this.GravPulse(xform.Coordinates, maxRange, minRange, baseMatrixDeltaV);
			}
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0003025A File Offset: 0x0002E45A
		[NullableContext(2)]
		public void GravPulse(EntityUid uid, float maxRange, float minRange, float baseRadialDeltaV = 0f, float baseTangentialDeltaV = 0f, TransformComponent xform = null)
		{
			if (base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				this.GravPulse(xform.Coordinates, maxRange, minRange, baseRadialDeltaV, baseTangentialDeltaV);
			}
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0003027B File Offset: 0x0002E47B
		public void GravPulse(EntityCoordinates entityPos, float maxRange, float minRange, in Matrix3 baseMatrixDeltaV)
		{
			this.GravPulse(entityPos.ToMap(this.EntityManager), maxRange, minRange, baseMatrixDeltaV);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x00030294 File Offset: 0x0002E494
		public void GravPulse(EntityCoordinates entityPos, float maxRange, float minRange, float baseRadialDeltaV = 0f, float baseTangentialDeltaV = 0f)
		{
			this.GravPulse(entityPos.ToMap(this.EntityManager), maxRange, minRange, baseRadialDeltaV, baseTangentialDeltaV);
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x000302B0 File Offset: 0x0002E4B0
		public void GravPulse(MapCoordinates mapPos, float maxRange, float minRange, in Matrix3 baseMatrixDeltaV)
		{
			if (mapPos == MapCoordinates.Nullspace)
			{
				return;
			}
			Vector2 epicenter = mapPos.Position;
			float minRange2 = MathF.Max(minRange * minRange, 1E-05f);
			foreach (EntityUid entity in this._lookup.GetEntitiesInRange(mapPos.MapId, epicenter, maxRange, 10))
			{
				PhysicsComponent physics;
				if (base.TryComp<PhysicsComponent>(entity, ref physics) && physics.BodyType != 4 && this.CanGravPulseAffect(entity))
				{
					Vector2 displacement = epicenter - base.Transform(entity).WorldPosition;
					float distance2 = displacement.LengthSquared;
					if (distance2 >= minRange2)
					{
						float scaling = 1f / distance2 * physics.Mass;
						this._physics.ApplyLinearImpulse(entity, ref displacement * ref baseMatrixDeltaV * scaling, null, physics);
					}
				}
			}
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x000303A4 File Offset: 0x0002E5A4
		public void GravPulse(MapCoordinates mapPos, float maxRange, float minRange = 0f, float baseRadialDeltaV = 0f, float baseTangentialDeltaV = 0f)
		{
			Matrix3 matrix = new Matrix3(baseRadialDeltaV, baseTangentialDeltaV, 0f, -baseTangentialDeltaV, baseRadialDeltaV, 0f, 0f, 0f, 1f);
			this.GravPulse(mapPos, maxRange, minRange, matrix);
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x000303E4 File Offset: 0x0002E5E4
		[NullableContext(2)]
		public void SetPulsePeriod(EntityUid uid, TimeSpan value, GravityWellComponent gravWell = null)
		{
			if (!base.Resolve<GravityWellComponent>(uid, ref gravWell, true))
			{
				return;
			}
			if (MathHelper.CloseTo(gravWell.TargetPulsePeriod.TotalSeconds, value.TotalSeconds, 1E-07))
			{
				return;
			}
			gravWell.TargetPulsePeriod = value;
			gravWell.NextPulseTime = gravWell.LastPulseTime + gravWell.TargetPulsePeriod;
			TimeSpan curTime = this._timing.CurTime;
			if (gravWell.NextPulseTime <= curTime)
			{
				this.Update(uid, curTime - gravWell.LastPulseTime, gravWell, null);
			}
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x00030471 File Offset: 0x0002E671
		public void OnGravityWellStartup(EntityUid uid, GravityWellComponent comp, ComponentStartup args)
		{
			comp.LastPulseTime = this._timing.CurTime;
			comp.NextPulseTime = comp.LastPulseTime + comp.TargetPulsePeriod;
		}

		// Token: 0x040005B0 RID: 1456
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005B1 RID: 1457
		[Dependency]
		private readonly IViewVariablesManager _vvManager;

		// Token: 0x040005B2 RID: 1458
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040005B3 RID: 1459
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040005B4 RID: 1460
		public const float MinGravPulseRange = 1E-05f;
	}
}
