using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Audio;
using Content.Server.Construction;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Shuttles.Components;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001FB RID: 507
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThrusterSystem : EntitySystem
	{
		// Token: 0x06000A3F RID: 2623 RVA: 0x000360E0 File Offset: 0x000342E0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ThrusterComponent, ActivateInWorldEvent>(new ComponentEventHandler<ThrusterComponent, ActivateInWorldEvent>(this.OnActivateThruster), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, ComponentInit>(new ComponentEventHandler<ThrusterComponent, ComponentInit>(this.OnThrusterInit), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, ComponentShutdown>(new ComponentEventHandler<ThrusterComponent, ComponentShutdown>(this.OnThrusterShutdown), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, PowerChangedEvent>(new ComponentEventRefHandler<ThrusterComponent, PowerChangedEvent>(this.OnPowerChange), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<ThrusterComponent, AnchorStateChangedEvent>(this.OnAnchorChange), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, ReAnchorEvent>(new ComponentEventRefHandler<ThrusterComponent, ReAnchorEvent>(this.OnThrusterReAnchor), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, MoveEvent>(new ComponentEventRefHandler<ThrusterComponent, MoveEvent>(this.OnRotate), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, IsHotEvent>(new ComponentEventHandler<ThrusterComponent, IsHotEvent>(this.OnIsHotEvent), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, StartCollideEvent>(new ComponentEventRefHandler<ThrusterComponent, StartCollideEvent>(this.OnStartCollide), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, EndCollideEvent>(new ComponentEventRefHandler<ThrusterComponent, EndCollideEvent>(this.OnEndCollide), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, ExaminedEvent>(new ComponentEventHandler<ThrusterComponent, ExaminedEvent>(this.OnThrusterExamine), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, RefreshPartsEvent>(new ComponentEventHandler<ThrusterComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<ThrusterComponent, UpgradeExamineEvent>(new ComponentEventHandler<ThrusterComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<ShuttleComponent, TileChangedEvent>(new ComponentEventRefHandler<ShuttleComponent, TileChangedEvent>(this.OnShuttleTileChange), null, null);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0003620C File Offset: 0x0003440C
		private void OnThrusterExamine(EntityUid uid, ThrusterComponent component, ExaminedEvent args)
		{
			string enabled = Loc.GetString(component.Enabled ? "thruster-comp-enabled" : "thruster-comp-disabled");
			args.PushMarkup(enabled);
			TransformComponent xform;
			if (component.Type == ThrusterType.Linear && this.EntityManager.TryGetComponent<TransformComponent>(uid, ref xform) && xform.Anchored)
			{
				string nozzleDir = Loc.GetString("thruster-comp-nozzle-direction", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("direction", DirectionExtensions.GetDir(xform.LocalRotation.Opposite().ToWorldVec()).ToString().ToLowerInvariant())
				});
				args.PushMarkup(nozzleDir);
				string nozzleText = Loc.GetString(this.NozzleExposed(xform) ? "thruster-comp-nozzle-exposed" : "thruster-comp-nozzle-not-exposed");
				args.PushMarkup(nozzleText);
			}
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x000362DC File Offset: 0x000344DC
		private void OnIsHotEvent(EntityUid uid, ThrusterComponent component, IsHotEvent args)
		{
			args.IsHot = (component.Type != ThrusterType.Angular && component.IsOn);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000362F8 File Offset: 0x000344F8
		private void OnShuttleTileChange(EntityUid uid, ShuttleComponent component, ref TileChangedEvent args)
		{
			if (args.NewTile.IsSpace(this._tileDefManager) || !args.OldTile.IsSpace(this._tileDefManager))
			{
				return;
			}
			Vector2i tilePos = args.NewTile.GridIndices;
			MapGridComponent grid = this._mapManager.GetGrid(uid);
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<ThrusterComponent> thrusterQuery = base.GetEntityQuery<ThrusterComponent>();
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 || y == 0)
					{
						Vector2i checkPos = tilePos + new Vector2i(x, y);
						EntityUid? ent;
						while (grid.GetAnchoredEntitiesEnumerator(checkPos).MoveNext(ref ent))
						{
							ThrusterComponent thruster;
							if (thrusterQuery.TryGetComponent(ent.Value, ref thruster) && thruster.RequireSpace)
							{
								TransformComponent xform = xformQuery.GetComponent(ent.Value);
								Vector2 direction = xform.LocalRotation.ToWorldVec();
								if (!(new Vector2i((int)direction.X, (int)direction.Y) != new Vector2i(x, y)))
								{
									this.DisableThruster(ent.Value, thruster, xform.GridUid, null, null);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0003643B File Offset: 0x0003463B
		private void OnActivateThruster(EntityUid uid, ThrusterComponent component, ActivateInWorldEvent args)
		{
			component.Enabled = !component.Enabled;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0003644C File Offset: 0x0003464C
		private void OnRotate(EntityUid uid, ThrusterComponent component, ref MoveEvent args)
		{
			TransformComponent xform;
			MapGridComponent grid;
			ShuttleComponent shuttleComponent;
			if (!component.Enabled || component.Type != ThrusterType.Linear || !this.EntityManager.TryGetComponent<TransformComponent>(uid, ref xform) || !this._mapManager.TryGetGrid(xform.GridUid, ref grid) || !this.EntityManager.TryGetComponent<ShuttleComponent>(grid.Owner, ref shuttleComponent))
			{
				return;
			}
			bool canEnable = this.CanEnable(uid, component);
			if (!canEnable && !component.IsOn)
			{
				return;
			}
			if (!component.IsOn && canEnable)
			{
				this.EnableThruster(uid, component, null);
				return;
			}
			if (component.IsOn && !canEnable)
			{
				this.DisableThruster(uid, component, xform, new Angle?(args.OldRotation));
				return;
			}
			int oldDirection = args.OldRotation.GetCardinalDir() / 2;
			int direction = args.NewRotation.GetCardinalDir() / 2;
			shuttleComponent.LinearThrust[oldDirection] -= component.Thrust;
			shuttleComponent.LinearThrusters[oldDirection].Remove(component);
			shuttleComponent.LinearThrust[direction] += component.Thrust;
			shuttleComponent.LinearThrusters[direction].Add(component);
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0003655C File Offset: 0x0003475C
		private void OnAnchorChange(EntityUid uid, ThrusterComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored && this.CanEnable(uid, component))
			{
				this.EnableThruster(uid, component, null);
				return;
			}
			this.DisableThruster(uid, component, null, null);
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x00036598 File Offset: 0x00034798
		private void OnThrusterReAnchor(EntityUid uid, ThrusterComponent component, ref ReAnchorEvent args)
		{
			this.DisableThruster(uid, component, new EntityUid?(args.OldGrid), null, null);
			if (this.CanEnable(uid, component))
			{
				this.EnableThruster(uid, component, null);
			}
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x000365D5 File Offset: 0x000347D5
		private void OnThrusterInit(EntityUid uid, ThrusterComponent component, ComponentInit args)
		{
			this._ambient.SetAmbience(uid, false, null);
			if (!component.Enabled)
			{
				return;
			}
			if (this.CanEnable(uid, component))
			{
				this.EnableThruster(uid, component, null);
			}
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00036604 File Offset: 0x00034804
		private void OnThrusterShutdown(EntityUid uid, ThrusterComponent component, ComponentShutdown args)
		{
			this.DisableThruster(uid, component, null, null);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00036624 File Offset: 0x00034824
		private void OnPowerChange(EntityUid uid, ThrusterComponent component, ref PowerChangedEvent args)
		{
			if (args.Powered && this.CanEnable(uid, component))
			{
				this.EnableThruster(uid, component, null);
				return;
			}
			this.DisableThruster(uid, component, null, null);
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x00036660 File Offset: 0x00034860
		public void EnableThruster(EntityUid uid, ThrusterComponent component, [Nullable(2)] TransformComponent xform = null)
		{
			MapGridComponent grid;
			if (component.IsOn || !base.Resolve<TransformComponent>(uid, ref xform, true) || !this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			component.IsOn = true;
			ShuttleComponent shuttleComponent;
			if (!this.EntityManager.TryGetComponent<ShuttleComponent>(grid.Owner, ref shuttleComponent))
			{
				return;
			}
			ThrusterType type = component.Type;
			if (type != ThrusterType.Linear)
			{
				if (type != ThrusterType.Angular)
				{
					throw new ArgumentOutOfRangeException();
				}
				shuttleComponent.AngularThrust += component.Thrust;
				shuttleComponent.AngularThrusters.Add(component);
			}
			else
			{
				int direction = xform.LocalRotation.GetCardinalDir() / 2;
				shuttleComponent.LinearThrust[direction] += component.Thrust;
				shuttleComponent.LinearThrusters[direction].Add(component);
				PhysicsComponent physicsComponent;
				if (this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref physicsComponent) && component.BurnPoly.Count > 0)
				{
					PolygonShape shape = new PolygonShape();
					shape.SetVertices(component.BurnPoly);
					this._fixtureSystem.TryCreateFixture(uid, shape, "thruster-burn", 1f, false, 158, 0, 0.4f, 0f, true, null, null, null);
				}
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, ThrusterVisualState.State, true, appearance);
			}
			PointLightComponent pointLightComponent;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref pointLightComponent))
			{
				pointLightComponent.Enabled = true;
			}
			this._ambient.SetAmbience(uid, true, null);
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x000367E0 File Offset: 0x000349E0
		public void DisableThruster(EntityUid uid, ThrusterComponent component, [Nullable(2)] TransformComponent xform = null, Angle? angle = null)
		{
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return;
			}
			this.DisableThruster(uid, component, xform.GridUid, xform, null);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00036814 File Offset: 0x00034A14
		public void DisableThruster(EntityUid uid, ThrusterComponent component, EntityUid? gridId, [Nullable(2)] TransformComponent xform = null, Angle? angle = null)
		{
			MapGridComponent grid;
			if (!component.IsOn || !base.Resolve<TransformComponent>(uid, ref xform, true) || !this._mapManager.TryGetGrid(gridId, ref grid))
			{
				return;
			}
			component.IsOn = false;
			ShuttleComponent shuttleComponent;
			if (!this.EntityManager.TryGetComponent<ShuttleComponent>(grid.Owner, ref shuttleComponent))
			{
				return;
			}
			ThrusterType type = component.Type;
			if (type != ThrusterType.Linear)
			{
				if (type != ThrusterType.Angular)
				{
					throw new ArgumentOutOfRangeException();
				}
				shuttleComponent.AngularThrust -= component.Thrust;
				shuttleComponent.AngularThrusters.Remove(component);
			}
			else
			{
				Angle value = angle.GetValueOrDefault();
				if (angle == null)
				{
					value = xform.LocalRotation;
					angle = new Angle?(value);
				}
				int direction = angle.Value.GetCardinalDir() / 2;
				shuttleComponent.LinearThrust[direction] -= component.Thrust;
				shuttleComponent.LinearThrusters[direction].Remove(component);
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearance.SetData(uid, ThrusterVisualState.State, false, appearance);
			}
			PointLightComponent pointLightComponent;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref pointLightComponent))
			{
				pointLightComponent.Enabled = false;
			}
			this._ambient.SetAmbience(uid, false, null);
			PhysicsComponent physicsComponent;
			if (this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref physicsComponent))
			{
				this._fixtureSystem.DestroyFixture(uid, "thruster-burn", true, physicsComponent, null, null);
			}
			this._activeThrusters.Remove(component);
			component.Colliding.Clear();
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x00036990 File Offset: 0x00034B90
		public bool CanEnable(EntityUid uid, ThrusterComponent component)
		{
			if (!component.Enabled)
			{
				return false;
			}
			if (component.LifeStage > 6)
			{
				return false;
			}
			TransformComponent xform = base.Transform(uid);
			return xform.Anchored && this.IsPowered(uid, this.EntityManager, null) && (!component.RequireSpace || this.NozzleExposed(xform));
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x000369E8 File Offset: 0x00034BE8
		private bool NozzleExposed(TransformComponent xform)
		{
			if (xform.GridUid == null)
			{
				return true;
			}
			float num;
			float num2;
			(xform.LocalPosition + xform.LocalRotation.Opposite().ToWorldVec()).Deconstruct(ref num, ref num2);
			float x = num;
			float y = num2;
			return this._mapManager.GetGrid(xform.GridUid.Value).GetTileRef(new Vector2i((int)Math.Floor((double)x), (int)Math.Floor((double)y))).Tile.IsSpace(null);
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x00036A7C File Offset: 0x00034C7C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._accumulator += frameTime;
			if (this._accumulator < 1f)
			{
				return;
			}
			this._accumulator -= 1f;
			foreach (ThrusterComponent comp in this._activeThrusters.ToArray<ThrusterComponent>())
			{
				MetaDataComponent metaData = null;
				if (comp.Firing && comp.Damage != null && !base.Paused(comp.Owner, metaData) && !base.Deleted(comp.Owner, metaData))
				{
					foreach (EntityUid uid in comp.Colliding.ToArray())
					{
						this._damageable.TryChangeDamage(new EntityUid?(uid), comp.Damage, false, true, null, null);
					}
				}
			}
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00036B64 File Offset: 0x00034D64
		private void OnStartCollide(EntityUid uid, ThrusterComponent component, ref StartCollideEvent args)
		{
			if (args.OurFixture.ID != "thruster-burn")
			{
				return;
			}
			this._activeThrusters.Add(component);
			component.Colliding.Add(args.OtherFixture.Body.Owner);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00036BB4 File Offset: 0x00034DB4
		private void OnEndCollide(EntityUid uid, ThrusterComponent component, ref EndCollideEvent args)
		{
			if (args.OurFixture.ID != "thruster-burn")
			{
				return;
			}
			component.Colliding.Remove(args.OtherFixture.Body.Owner);
			if (component.Colliding.Count == 0)
			{
				this._activeThrusters.Remove(component);
			}
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00036C10 File Offset: 0x00034E10
		public void EnableLinearThrustDirection(ShuttleComponent component, DirectionFlag direction)
		{
			if ((component.ThrustDirections & direction) != null)
			{
				return;
			}
			component.ThrustDirections |= direction;
			int index = this.GetFlagIndex(direction);
			foreach (ThrusterComponent comp in component.LinearThrusters[index])
			{
				comp.Firing = true;
				this._appearance.SetData(comp.Owner, ThrusterVisualState.Thrusting, true, null);
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00036CA4 File Offset: 0x00034EA4
		public void DisableLinearThrustDirection(ShuttleComponent component, DirectionFlag direction)
		{
			if ((component.ThrustDirections & direction) == null)
			{
				return;
			}
			component.ThrustDirections &= ~direction;
			int index = this.GetFlagIndex(direction);
			foreach (ThrusterComponent comp in component.LinearThrusters[index])
			{
				comp.Firing = false;
				this._appearance.SetData(comp.Owner, ThrusterVisualState.Thrusting, false, null);
			}
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00036D3C File Offset: 0x00034F3C
		public void DisableLinearThrusters(ShuttleComponent component)
		{
			foreach (object obj in Enum.GetValues(typeof(DirectionFlag)))
			{
				DirectionFlag dir = (DirectionFlag)obj;
				this.DisableLinearThrustDirection(component, dir);
			}
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00036DA0 File Offset: 0x00034FA0
		public void SetAngularThrust(ShuttleComponent component, bool on)
		{
			if (on)
			{
				using (List<ThrusterComponent>.Enumerator enumerator = component.AngularThrusters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ThrusterComponent comp = enumerator.Current;
						comp.Firing = true;
						this._appearance.SetData(comp.Owner, ThrusterVisualState.Thrusting, true, null);
					}
					return;
				}
			}
			foreach (ThrusterComponent comp2 in component.AngularThrusters)
			{
				comp2.Firing = false;
				this._appearance.SetData(comp2.Owner, ThrusterVisualState.Thrusting, false, null);
			}
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00036E74 File Offset: 0x00035074
		private void OnRefreshParts(EntityUid uid, ThrusterComponent component, RefreshPartsEvent args)
		{
			float thrustRating = args.PartRatings[component.MachinePartThrust];
			component.Thrust = component.BaseThrust * MathF.Pow(component.PartRatingThrustMultiplier, thrustRating - 1f);
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00036EB2 File Offset: 0x000350B2
		private void OnUpgradeExamine(EntityUid uid, ThrusterComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("thruster-comp-upgrade-thrust", component.Thrust / component.BaseThrust);
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x00036ECC File Offset: 0x000350CC
		private int GetFlagIndex(DirectionFlag flag)
		{
			return (int)Math.Log2(flag);
		}

		// Token: 0x04000628 RID: 1576
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000629 RID: 1577
		[Dependency]
		private readonly ITileDefinitionManager _tileDefManager;

		// Token: 0x0400062A RID: 1578
		[Dependency]
		private readonly AmbientSoundSystem _ambient;

		// Token: 0x0400062B RID: 1579
		[Dependency]
		private readonly FixtureSystem _fixtureSystem;

		// Token: 0x0400062C RID: 1580
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x0400062D RID: 1581
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400062E RID: 1582
		public const string BurnFixture = "thruster-burn";

		// Token: 0x0400062F RID: 1583
		private readonly HashSet<ThrusterComponent> _activeThrusters = new HashSet<ThrusterComponent>();

		// Token: 0x04000630 RID: 1584
		private float _accumulator;
	}
}
