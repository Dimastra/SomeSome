using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.Atmos;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Miasma
{
	// Token: 0x0200078B RID: 1931
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MiasmaSystem : EntitySystem
	{
		// Token: 0x0600290A RID: 10506 RVA: 0x000D5BD4 File Offset: 0x000D3DD4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this._timing.CurTime >= this._diseaseTime)
			{
				this._diseaseTime = this._timing.CurTime + this._poolRepickTime;
				this._poolDisease = RandomExtensions.Pick<string>(this._random, this.MiasmaDiseasePool);
			}
			foreach (ValueTuple<RottingComponent, PerishableComponent, MetaDataComponent> valueTuple in base.EntityQuery<RottingComponent, PerishableComponent, MetaDataComponent>(false))
			{
				RottingComponent rotting = valueTuple.Item1;
				PerishableComponent perishable = valueTuple.Item2;
				MetaDataComponent metadata = valueTuple.Item3;
				if (perishable.Progressing && this.IsRotting(perishable, metadata) && !(this._timing.CurTime < perishable.RotNextUpdate))
				{
					perishable.RotNextUpdate = this._timing.CurTime + TimeSpan.FromSeconds((double)this._rotUpdateRate);
					base.EnsureComp<FliesComponent>(perishable.Owner);
					if (rotting.DealDamage)
					{
						DamageSpecifier damage = new DamageSpecifier();
						damage.DamageDict.Add("Blunt", 0.3);
						damage.DamageDict.Add("Cellular", 0.3);
						this._damageableSystem.TryChangeDamage(new EntityUid?(perishable.Owner), damage, true, true, null, new EntityUid?(perishable.Owner));
					}
					PhysicsComponent physics;
					if (base.TryComp<PhysicsComponent>(perishable.Owner, ref physics))
					{
						float molRate = perishable.MolsPerSecondPerUnitMass * this._rotUpdateRate;
						TransformComponent transform = base.Transform(perishable.Owner);
						Vector2i indices = this._transformSystem.GetGridOrMapTilePosition(perishable.Owner, null);
						GasMixture tileMixture = this._atmosphereSystem.GetTileMixture(transform.GridUid, null, indices, true);
						if (tileMixture != null)
						{
							tileMixture.AdjustMoles(Gas.Miasma, molRate * physics.FixturesMass);
						}
					}
				}
			}
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x000D5DE0 File Offset: 0x000D3FE0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RottingComponent, ComponentShutdown>(new ComponentEventHandler<RottingComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<RottingComponent, OnTemperatureChangeEvent>(new ComponentEventHandler<RottingComponent, OnTemperatureChangeEvent>(this.OnTempChange), null, null);
			base.SubscribeLocalEvent<PerishableComponent, MobStateChangedEvent>(new ComponentEventHandler<PerishableComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<PerishableComponent, BeingGibbedEvent>(new ComponentEventHandler<PerishableComponent, BeingGibbedEvent>(this.OnGibbed), null, null);
			base.SubscribeLocalEvent<PerishableComponent, ExaminedEvent>(new ComponentEventHandler<PerishableComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<RottingComponent, RejuvenateEvent>(new ComponentEventHandler<RottingComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
			base.SubscribeLocalEvent<AntiRottingContainerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<AntiRottingContainerComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
			base.SubscribeLocalEvent<AntiRottingContainerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<AntiRottingContainerComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved), null, null);
			base.SubscribeLocalEvent<FliesComponent, ComponentInit>(new ComponentEventHandler<FliesComponent, ComponentInit>(this.OnFliesInit), null, null);
			base.SubscribeLocalEvent<FliesComponent, ComponentShutdown>(new ComponentEventHandler<FliesComponent, ComponentShutdown>(this.OnFliesShutdown), null, null);
			this._poolDisease = RandomExtensions.Pick<string>(this._random, this.MiasmaDiseasePool);
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x000D5ED4 File Offset: 0x000D40D4
		private void OnShutdown(EntityUid uid, RottingComponent component, ComponentShutdown args)
		{
			base.RemComp<FliesComponent>(uid);
			PerishableComponent perishable;
			if (base.TryComp<PerishableComponent>(uid, ref perishable))
			{
				perishable.TimeOfDeath = TimeSpan.Zero;
				perishable.RotNextUpdate = TimeSpan.Zero;
			}
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x000D5F0C File Offset: 0x000D410C
		private void OnTempChange(EntityUid uid, RottingComponent component, OnTemperatureChangeEvent args)
		{
			if (base.HasComp<BodyPreservedComponent>(uid))
			{
				return;
			}
			bool decompose = args.CurrentTemperature > 274f;
			this.ToggleDecomposition(uid, decompose, null);
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x000D5F3A File Offset: 0x000D413A
		private void OnMobStateChanged(EntityUid uid, PerishableComponent component, MobStateChangedEvent args)
		{
			if (this._mobState.IsDead(uid, null))
			{
				base.EnsureComp<RottingComponent>(uid);
				component.TimeOfDeath = this._timing.CurTime;
			}
		}

		// Token: 0x0600290F RID: 10511 RVA: 0x000D5F64 File Offset: 0x000D4164
		private bool IsRotting(PerishableComponent perishable, [Nullable(2)] MetaDataComponent metadata = null)
		{
			return !(perishable.TimeOfDeath == TimeSpan.Zero) && this._timing.CurTime >= perishable.TimeOfDeath + perishable.RotAfter + this._metaDataSystem.GetPauseTime(perishable.Owner, metadata);
		}

		// Token: 0x06002910 RID: 10512 RVA: 0x000D5FC4 File Offset: 0x000D41C4
		private void OnGibbed(EntityUid uid, PerishableComponent component, BeingGibbedEvent args)
		{
			PhysicsComponent physics;
			if (!base.TryComp<PhysicsComponent>(uid, ref physics))
			{
				return;
			}
			if (!this.IsRotting(component, null))
			{
				return;
			}
			float molsToDump = component.MolsPerSecondPerUnitMass * physics.FixturesMass * (float)(this._timing.CurTime - component.TimeOfDeath).TotalSeconds;
			TransformComponent transform = base.Transform(uid);
			Vector2i indices = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
			GasMixture tileMixture = this._atmosphereSystem.GetTileMixture(transform.GridUid, null, indices, true);
			if (tileMixture != null)
			{
				tileMixture.AdjustMoles(Gas.Miasma, molsToDump);
			}
			foreach (EntityUid part in args.GibbedParts)
			{
				this.EntityManager.DeleteEntity(part);
			}
		}

		// Token: 0x06002911 RID: 10513 RVA: 0x000D60A8 File Offset: 0x000D42A8
		private void OnExamined(EntityUid uid, PerishableComponent component, ExaminedEvent args)
		{
			if (!this.IsRotting(component, null))
			{
				return;
			}
			double stage = (this._timing.CurTime - component.TimeOfDeath).TotalSeconds / component.RotAfter.TotalSeconds;
			string text;
			if (stage < 3.0)
			{
				if (stage < 2.0)
				{
					text = "miasma-rotting";
				}
				else
				{
					text = "miasma-bloated";
				}
			}
			else
			{
				text = "miasma-extremely-bloated";
			}
			string description = text;
			args.PushMarkup(Loc.GetString(description));
		}

		// Token: 0x06002912 RID: 10514 RVA: 0x000D6129 File Offset: 0x000D4329
		private void OnRejuvenate(EntityUid uid, RottingComponent component, RejuvenateEvent args)
		{
			this.EntityManager.RemoveComponentDeferred<RottingComponent>(uid);
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x000D6138 File Offset: 0x000D4338
		private void OnEntInserted(EntityUid uid, AntiRottingContainerComponent component, EntInsertedIntoContainerMessage args)
		{
			PerishableComponent perishable;
			if (base.TryComp<PerishableComponent>(args.Entity, ref perishable))
			{
				this.ModifyPreservationSource(args.Entity, true);
				this.ToggleDecomposition(args.Entity, false, perishable);
			}
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x000D6170 File Offset: 0x000D4370
		private void OnEntRemoved(EntityUid uid, AntiRottingContainerComponent component, EntRemovedFromContainerMessage args)
		{
			PerishableComponent perishable;
			MetaDataComponent metadata;
			if (base.TryComp<PerishableComponent>(args.Entity, ref perishable) && base.TryComp<MetaDataComponent>(uid, ref metadata) && metadata.EntityLifeStage < 4)
			{
				this.ModifyPreservationSource(args.Entity, false);
				this.ToggleDecomposition(args.Entity, true, perishable);
			}
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x000D61BC File Offset: 0x000D43BC
		private void OnFliesInit(EntityUid uid, FliesComponent component, ComponentInit args)
		{
			component.VirtFlies = this.EntityManager.SpawnEntity("AmbientSoundSourceFlies", base.Transform(uid).Coordinates);
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x000D61E0 File Offset: 0x000D43E0
		private void OnFliesShutdown(EntityUid uid, FliesComponent component, ComponentShutdown args)
		{
			if (!base.Terminating(uid, null) && !base.Deleted(uid, null))
			{
				base.Del(component.VirtFlies);
			}
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x000D6204 File Offset: 0x000D4404
		[NullableContext(2)]
		public void ToggleDecomposition(EntityUid uid, bool decompose, PerishableComponent perishable = null)
		{
			if (base.Terminating(uid, null) || !base.Resolve<PerishableComponent>(uid, ref perishable, false))
			{
				return;
			}
			if (decompose == perishable.Progressing)
			{
				return;
			}
			perishable.Progressing = decompose;
			if (!this.IsRotting(perishable, null))
			{
				return;
			}
			if (decompose)
			{
				base.EnsureComp<FliesComponent>(uid);
				return;
			}
			base.RemComp<FliesComponent>(uid);
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x000D6258 File Offset: 0x000D4458
		public void ModifyPreservationSource(EntityUid uid, bool add)
		{
			BodyPreservedComponent component = base.EnsureComp<BodyPreservedComponent>(uid);
			if (add)
			{
				component.PreservationSources++;
				return;
			}
			component.PreservationSources--;
			if (component.PreservationSources == 0)
			{
				base.RemCompDeferred(uid, component);
			}
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x000D629D File Offset: 0x000D449D
		public string RequestPoolDisease()
		{
			this._diseaseTime = this._timing.CurTime + this._poolRepickTime;
			return this._poolDisease;
		}

		// Token: 0x04001983 RID: 6531
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04001984 RID: 6532
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001985 RID: 6533
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001986 RID: 6534
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04001987 RID: 6535
		[Dependency]
		private readonly MetaDataSystem _metaDataSystem;

		// Token: 0x04001988 RID: 6536
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001989 RID: 6537
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400198A RID: 6538
		private float _rotUpdateRate = 5f;

		// Token: 0x0400198B RID: 6539
		public readonly IReadOnlyList<string> MiasmaDiseasePool = new string[]
		{
			"VentCough",
			"AMIV",
			"SpaceCold",
			"SpaceFlu",
			"BirdFlew",
			"VanAusdallsRobovirus",
			"BleedersBite",
			"Plague",
			"TongueTwister",
			"MemeticAmirmir"
		};

		// Token: 0x0400198C RID: 6540
		private string _poolDisease = "";

		// Token: 0x0400198D RID: 6541
		private TimeSpan _diseaseTime = TimeSpan.FromMinutes(5.0);

		// Token: 0x0400198E RID: 6542
		private TimeSpan _poolRepickTime = TimeSpan.FromMinutes(5.0);
	}
}
