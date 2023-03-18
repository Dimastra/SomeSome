using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Server.Stunnable;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Temperature;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200079A RID: 1946
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlammableSystem : EntitySystem
	{
		// Token: 0x06002A10 RID: 10768 RVA: 0x000DD608 File Offset: 0x000DB808
		public override void Initialize()
		{
			base.UpdatesAfter.Add(typeof(AtmosphereSystem));
			base.SubscribeLocalEvent<FlammableComponent, MapInitEvent>(new ComponentEventHandler<FlammableComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<FlammableComponent, InteractUsingEvent>(new ComponentEventHandler<FlammableComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<FlammableComponent, StartCollideEvent>(new ComponentEventRefHandler<FlammableComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<FlammableComponent, IsHotEvent>(new ComponentEventHandler<FlammableComponent, IsHotEvent>(this.OnIsHot), null, null);
			base.SubscribeLocalEvent<FlammableComponent, TileFireEvent>(new ComponentEventRefHandler<FlammableComponent, TileFireEvent>(this.OnTileFire), null, null);
			base.SubscribeLocalEvent<FlammableComponent, RejuvenateEvent>(new ComponentEventHandler<FlammableComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
			base.SubscribeLocalEvent<IgniteOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<IgniteOnCollideComponent, StartCollideEvent>(this.IgniteOnCollide), null, null);
			base.SubscribeLocalEvent<IgniteOnMeleeHitComponent, MeleeHitEvent>(new ComponentEventHandler<IgniteOnMeleeHitComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x000DD6CC File Offset: 0x000DB8CC
		private void OnMeleeHit(EntityUid uid, IgniteOnMeleeHitComponent component, MeleeHitEvent args)
		{
			foreach (EntityUid entity in args.HitEntities)
			{
				FlammableComponent flammable;
				if (base.TryComp<FlammableComponent>(entity, ref flammable))
				{
					flammable.FireStacks += component.FireStacks;
					this.Ignite(entity, flammable);
				}
			}
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000DD738 File Offset: 0x000DB938
		private void IgniteOnCollide(EntityUid uid, IgniteOnCollideComponent component, ref StartCollideEvent args)
		{
			EntityUid otherFixture = args.OtherFixture.Body.Owner;
			FlammableComponent flammable;
			if (!this.EntityManager.TryGetComponent<FlammableComponent>(otherFixture, ref flammable))
			{
				return;
			}
			flammable.FireStacks += component.FireStacks;
			this.Ignite(otherFixture, flammable);
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000DD784 File Offset: 0x000DB984
		private void OnMapInit(EntityUid uid, FlammableComponent component, MapInitEvent args)
		{
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(uid, ref body))
			{
				return;
			}
			this._fixture.TryCreateFixture(uid, component.FlammableCollisionShape, "flammable", 1f, false, 0, 221, 0.4f, 0f, true, null, body, null);
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x000DD7D0 File Offset: 0x000DB9D0
		private void OnInteractUsing(EntityUid uid, FlammableComponent flammable, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			IsHotEvent isHotEvent = new IsHotEvent();
			base.RaiseLocalEvent<IsHotEvent>(args.Used, isHotEvent, false);
			if (!isHotEvent.IsHot)
			{
				return;
			}
			this.Ignite(uid, flammable);
			args.Handled = true;
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x000DD814 File Offset: 0x000DBA14
		private void OnCollide(EntityUid uid, FlammableComponent flammable, ref StartCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (args.OtherFixture.ID != "flammable" && args.OurFixture.ID != "flammable")
			{
				return;
			}
			FlammableComponent otherFlammable;
			if (!this.EntityManager.TryGetComponent<FlammableComponent>(otherUid, ref otherFlammable))
			{
				return;
			}
			if (!flammable.FireSpread || !otherFlammable.FireSpread)
			{
				return;
			}
			if (!flammable.OnFire)
			{
				if (otherFlammable.OnFire)
				{
					otherFlammable.FireStacks /= 2f;
					flammable.FireStacks += otherFlammable.FireStacks;
					this.Ignite(uid, flammable);
				}
				return;
			}
			if (otherFlammable.OnFire)
			{
				float fireSplit = (flammable.FireStacks + otherFlammable.FireStacks) / 2f;
				flammable.FireStacks = fireSplit;
				otherFlammable.FireStacks = fireSplit;
				return;
			}
			flammable.FireStacks /= 2f;
			otherFlammable.FireStacks += flammable.FireStacks;
			this.Ignite(otherUid, otherFlammable);
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x000DD919 File Offset: 0x000DBB19
		private void OnIsHot(EntityUid uid, FlammableComponent flammable, IsHotEvent args)
		{
			args.IsHot = flammable.OnFire;
		}

		// Token: 0x06002A17 RID: 10775 RVA: 0x000DD928 File Offset: 0x000DBB28
		private void OnTileFire(EntityUid uid, FlammableComponent flammable, ref TileFireEvent args)
		{
			float tempDelta = args.Temperature - 373.15f;
			float maxTemp = 0f;
			this._fireEvents.TryGetValue(flammable, out maxTemp);
			if (tempDelta > maxTemp)
			{
				this._fireEvents[flammable] = tempDelta;
			}
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x000DD968 File Offset: 0x000DBB68
		private void OnRejuvenate(EntityUid uid, FlammableComponent component, RejuvenateEvent args)
		{
			this.Extinguish(uid, component);
		}

		// Token: 0x06002A19 RID: 10777 RVA: 0x000DD974 File Offset: 0x000DBB74
		[NullableContext(2)]
		public void UpdateAppearance(EntityUid uid, FlammableComponent flammable = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<FlammableComponent, AppearanceComponent>(uid, ref flammable, ref appearance, true))
			{
				return;
			}
			this._appearance.SetData(uid, FireVisuals.OnFire, flammable.OnFire, appearance);
			this._appearance.SetData(uid, FireVisuals.FireStacks, flammable.FireStacks, appearance);
		}

		// Token: 0x06002A1A RID: 10778 RVA: 0x000DD9CC File Offset: 0x000DBBCC
		[NullableContext(2)]
		public void AdjustFireStacks(EntityUid uid, float relativeFireStacks, FlammableComponent flammable = null)
		{
			if (!base.Resolve<FlammableComponent>(uid, ref flammable, true))
			{
				return;
			}
			flammable.FireStacks = MathF.Min(MathF.Max(-10f, flammable.FireStacks + relativeFireStacks), 20f);
			if (flammable.OnFire && flammable.FireStacks <= 0f)
			{
				this.Extinguish(uid, flammable);
			}
			this.UpdateAppearance(uid, flammable, null);
		}

		// Token: 0x06002A1B RID: 10779 RVA: 0x000DDA30 File Offset: 0x000DBC30
		[NullableContext(2)]
		public void Extinguish(EntityUid uid, FlammableComponent flammable = null)
		{
			if (!base.Resolve<FlammableComponent>(uid, ref flammable, true))
			{
				return;
			}
			if (!flammable.OnFire)
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Flammable;
			LogStringHandler logStringHandler = new LogStringHandler(29, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(flammable.Owner), "entity", "ToPrettyString(flammable.Owner)");
			logStringHandler.AppendLiteral(" stopped being on fire damage");
			adminLogger.Add(type, ref logStringHandler);
			flammable.OnFire = false;
			flammable.FireStacks = 0f;
			flammable.Collided.Clear();
			this.UpdateAppearance(uid, flammable, null);
		}

		// Token: 0x06002A1C RID: 10780 RVA: 0x000DDABC File Offset: 0x000DBCBC
		[NullableContext(2)]
		public void Ignite(EntityUid uid, FlammableComponent flammable = null)
		{
			if (!base.Resolve<FlammableComponent>(uid, ref flammable, true))
			{
				return;
			}
			if (flammable.FireStacks > 0f && !flammable.OnFire)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Flammable;
				LogStringHandler logStringHandler = new LogStringHandler(11, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(flammable.Owner), "entity", "ToPrettyString(flammable.Owner)");
				logStringHandler.AppendLiteral(" is on fire");
				adminLogger.Add(type, ref logStringHandler);
				flammable.OnFire = true;
			}
			this.UpdateAppearance(uid, flammable, null);
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x000DDB40 File Offset: 0x000DBD40
		[NullableContext(2)]
		public void Resist(EntityUid uid, FlammableComponent flammable = null)
		{
			if (!base.Resolve<FlammableComponent>(uid, ref flammable, true))
			{
				return;
			}
			if (!flammable.OnFire || !this._actionBlockerSystem.CanInteract(flammable.Owner, null) || flammable.Resisting)
			{
				return;
			}
			flammable.Resisting = true;
			flammable.Owner.PopupMessage(Loc.GetString("flammable-component-resist-message"));
			this._stunSystem.TryParalyze(uid, TimeSpan.FromSeconds(2.0), true, null);
			TimerExtensions.SpawnTimer(flammable.Owner, 2000, delegate()
			{
				flammable.Resisting = false;
				flammable.FireStacks -= 1f;
				this.UpdateAppearance(uid, flammable, null);
			}, default(CancellationToken));
		}

		// Token: 0x06002A1E RID: 10782 RVA: 0x000DDC2C File Offset: 0x000DBE2C
		public override void Update(float frameTime)
		{
			foreach (KeyValuePair<FlammableComponent, float> keyValuePair in this._fireEvents)
			{
				FlammableComponent flammableComponent;
				float num;
				keyValuePair.Deconstruct(out flammableComponent, out num);
				FlammableComponent flammable = flammableComponent;
				float fireStackDelta = Math.Max(MathF.Log2(num / 100f) + 1f, 0f) - flammable.FireStacks;
				if (fireStackDelta > 0f)
				{
					this.AdjustFireStacks(flammable.Owner, fireStackDelta, flammable);
				}
				this.Ignite(flammable.Owner, flammable);
			}
			this._fireEvents.Clear();
			this._timer += frameTime;
			if (this._timer < 1f)
			{
				return;
			}
			this._timer -= 1f;
			foreach (ValueTuple<FlammableComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<FlammableComponent, TransformComponent>(false))
			{
				FlammableComponent flammable2 = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				EntityUid uid = flammable2.Owner;
				if (flammable2.FireStacks < 0f)
				{
					flammable2.FireStacks = MathF.Min(0f, flammable2.FireStacks + 1f);
				}
				if (!flammable2.OnFire)
				{
					this._alertsSystem.ClearAlert(uid, AlertType.Fire);
				}
				else
				{
					this._alertsSystem.ShowAlert(uid, AlertType.Fire, null, null);
					if (flammable2.FireStacks > 0f)
					{
						int damageScale = Math.Min((int)flammable2.FireStacks, 5);
						TemperatureComponent temp;
						if (base.TryComp<TemperatureComponent>(uid, ref temp))
						{
							this._temperatureSystem.ChangeHeat(uid, (float)(12500 * damageScale), false, temp);
						}
						this._damageableSystem.TryChangeDamage(new EntityUid?(uid), flammable2.Damage * (float)damageScale, false, true, null, null);
						this.AdjustFireStacks(uid, -0.1f * (flammable2.Resisting ? 10f : 1f), flammable2);
						GasMixture air = this._atmosphereSystem.GetContainingMixture(uid, false, false, null);
						if (air == null || air.GetMoles(Gas.Oxygen) < 1f)
						{
							this.Extinguish(uid, flammable2);
						}
						else
						{
							if (transform.GridUid != null)
							{
								this._atmosphereSystem.HotspotExpose(transform.GridUid.Value, this._transformSystem.GetGridOrMapTilePosition(uid, transform), 700f, 50f, true);
							}
							for (int i = flammable2.Collided.Count - 1; i >= 0; i--)
							{
								EntityUid otherUid = flammable2.Collided[i];
								if (!otherUid.IsValid() || !this.EntityManager.EntityExists(otherUid))
								{
									flammable2.Collided.RemoveAt(i);
								}
								else
								{
									Box2 worldAABB = this._lookup.GetWorldAABB(uid, transform);
									Box2 worldAABB2 = this._lookup.GetWorldAABB(otherUid, null);
									if (!worldAABB.Intersects(ref worldAABB2))
									{
										flammable2.Collided.RemoveAt(i);
									}
								}
							}
						}
					}
					else
					{
						this.Extinguish(uid, flammable2);
					}
				}
			}
		}

		// Token: 0x040019FD RID: 6653
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x040019FE RID: 6654
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040019FF RID: 6655
		[Dependency]
		private readonly StunSystem _stunSystem;

		// Token: 0x04001A00 RID: 6656
		[Dependency]
		private readonly TemperatureSystem _temperatureSystem;

		// Token: 0x04001A01 RID: 6657
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001A02 RID: 6658
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x04001A03 RID: 6659
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04001A04 RID: 6660
		[Dependency]
		private readonly FixtureSystem _fixture;

		// Token: 0x04001A05 RID: 6661
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04001A06 RID: 6662
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001A07 RID: 6663
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001A08 RID: 6664
		public const float MinimumFireStacks = -10f;

		// Token: 0x04001A09 RID: 6665
		public const float MaximumFireStacks = 20f;

		// Token: 0x04001A0A RID: 6666
		private const float UpdateTime = 1f;

		// Token: 0x04001A0B RID: 6667
		public const float MinIgnitionTemperature = 373.15f;

		// Token: 0x04001A0C RID: 6668
		public const string FlammableFixtureID = "flammable";

		// Token: 0x04001A0D RID: 6669
		private float _timer;

		// Token: 0x04001A0E RID: 6670
		private Dictionary<FlammableComponent, float> _fireEvents = new Dictionary<FlammableComponent, float>();
	}
}
