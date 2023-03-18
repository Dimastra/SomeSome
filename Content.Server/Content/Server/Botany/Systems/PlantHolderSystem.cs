using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Botany.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Hands.Components;
using Content.Server.Kitchen.Components;
using Content.Server.Popups;
using Content.Shared.Atmos;
using Content.Shared.Botany;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Botany.Systems
{
	// Token: 0x020006FE RID: 1790
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class PlantHolderSystem : EntitySystem
	{
		// Token: 0x0600257A RID: 9594 RVA: 0x000C4AFC File Offset: 0x000C2CFC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlantHolderComponent, ExaminedEvent>(new ComponentEventHandler<PlantHolderComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<PlantHolderComponent, InteractUsingEvent>(new ComponentEventHandler<PlantHolderComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<PlantHolderComponent, InteractHandEvent>(new ComponentEventHandler<PlantHolderComponent, InteractHandEvent>(this.OnInteractHand), null, null);
		}

		// Token: 0x0600257B RID: 9595 RVA: 0x000C4B4C File Offset: 0x000C2D4C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (PlantHolderComponent plantHolder in base.EntityQuery<PlantHolderComponent>(false))
			{
				if (!(plantHolder.NextUpdate > this._gameTiming.CurTime))
				{
					plantHolder.NextUpdate = this._gameTiming.CurTime + plantHolder.UpdateDelay;
					this.Update(plantHolder.Owner, plantHolder);
				}
			}
		}

		// Token: 0x0600257C RID: 9596 RVA: 0x000C4BDC File Offset: 0x000C2DDC
		[NullableContext(1)]
		private void OnExamine(EntityUid uid, PlantHolderComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			if (component.Seed == null)
			{
				args.PushMarkup(Loc.GetString("plant-holder-component-nothing-planted-message"));
			}
			else if (!component.Dead)
			{
				string displayName = Loc.GetString(component.Seed.DisplayName);
				args.PushMarkup(Loc.GetString("plant-holder-component-something-already-growing-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seedName", displayName),
					new ValueTuple<string, object>("toBeForm", displayName.EndsWith('s') ? "are" : "is")
				}));
				if (component.Health <= component.Seed.Endurance / 2f)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-something-already-growing-low-health-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("healthState", Loc.GetString(((float)component.Age > component.Seed.Lifespan) ? "plant-holder-component-plant-old-adjective" : "plant-holder-component-plant-unhealthy-adjective"))
					}));
				}
			}
			else
			{
				args.PushMarkup(Loc.GetString("plant-holder-component-dead-plant-matter-message"));
			}
			if (component.WeedLevel >= 5f)
			{
				args.PushMarkup(Loc.GetString("plant-holder-component-weed-high-level-message"));
			}
			if (component.PestLevel >= 5f)
			{
				args.PushMarkup(Loc.GetString("plant-holder-component-pest-high-level-message"));
			}
			args.PushMarkup(Loc.GetString("plant-holder-component-water-level-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("waterLevel", (int)component.WaterLevel)
			}));
			args.PushMarkup(Loc.GetString("plant-holder-component-nutrient-level-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("nutritionLevel", (int)component.NutritionLevel)
			}));
			if (component.DrawWarnings)
			{
				if (component.Toxins > 40f)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-toxins-high-warning"));
				}
				if (component.ImproperLight)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-light-improper-warning"));
				}
				if (component.ImproperHeat)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-heat-improper-warning"));
				}
				if (component.ImproperPressure)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-pressure-improper-warning"));
				}
				if (component.MissingGas > 0)
				{
					args.PushMarkup(Loc.GetString("plant-holder-component-gas-missing-warning"));
				}
			}
		}

		// Token: 0x0600257D RID: 9597 RVA: 0x000C4E18 File Offset: 0x000C3018
		[NullableContext(1)]
		private void OnInteractUsing(EntityUid uid, PlantHolderComponent component, InteractUsingEvent args)
		{
			SeedComponent seeds;
			Solution solution;
			Solution targetSolution;
			SprayComponent spray;
			if (base.TryComp<SeedComponent>(args.Used, ref seeds))
			{
				if (component.Seed != null)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-already-seeded-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", base.Comp<MetaDataComponent>(uid).EntityName)
					}), args.User, PopupType.Medium);
					return;
				}
				SeedData seed;
				if (!this._botanySystem.TryGetSeed(seeds, out seed))
				{
					return;
				}
				string name = Loc.GetString(seed.Name);
				string noun = Loc.GetString(seed.Noun);
				this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-plant-success-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seedName", name),
					new ValueTuple<string, object>("seedNoun", noun)
				}), args.User, PopupType.Medium);
				component.Seed = seed;
				component.Dead = false;
				component.Age = 1;
				component.Health = component.Seed.Endurance;
				component.LastCycle = this._gameTiming.CurTime;
				this.EntityManager.QueueDeleteEntity(args.Used);
				this.CheckLevelSanity(uid, component);
				this.UpdateSprite(uid, component);
				return;
			}
			else if (this._tagSystem.HasTag(args.Used, "Hoe"))
			{
				if (component.WeedLevel > 0f)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-remove-weeds-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", base.Comp<MetaDataComponent>(uid).EntityName)
					}), args.User, PopupType.Medium);
					this._popupSystem.PopupEntity(Loc.GetString("plant-holder-component-remove-weeds-others-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("otherName", base.Comp<MetaDataComponent>(args.User).EntityName)
					}), uid, Filter.PvsExcept(args.User, 2f, null), true, PopupType.Small);
					component.WeedLevel = 0f;
					this.UpdateSprite(uid, component);
					return;
				}
				this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-no-weeds-message"), args.User, PopupType.Small);
				return;
			}
			else if (this._tagSystem.HasTag(args.Used, "Shovel"))
			{
				if (component.Seed != null)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-remove-plant-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", base.Comp<MetaDataComponent>(uid).EntityName)
					}), args.User, PopupType.Medium);
					this._popupSystem.PopupEntity(Loc.GetString("plant-holder-component-remove-plant-others-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("name", base.Comp<MetaDataComponent>(args.User).EntityName)
					}), uid, Filter.PvsExcept(args.User, 2f, null), true, PopupType.Small);
					this.RemovePlant(uid, component);
					return;
				}
				this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-no-plant-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", base.Comp<MetaDataComponent>(uid).EntityName)
				}), args.User, PopupType.Small);
				return;
			}
			else if (this._solutionSystem.TryGetDrainableSolution(args.Used, out solution, null, null) && this._solutionSystem.TryGetSolution(uid, component.SoilSolutionName, out targetSolution, null) && base.TryComp<SprayComponent>(args.Used, ref spray))
			{
				FixedPoint2 amount = FixedPoint2.New(1);
				EntityUid solutionEntity = args.Used;
				this._audio.PlayPvs(spray.SpraySound, args.Used, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.125f))));
				Solution split = this._solutionSystem.Drain(solutionEntity, solution, amount, null);
				if (split.Volume == 0)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-no-plant-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("owner", args.Used)
					}), args.User, PopupType.Small);
					return;
				}
				this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-spray-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", uid),
					new ValueTuple<string, object>("amount", split.Volume)
				}), args.User, PopupType.Medium);
				this._solutionSystem.TryAddSolution(uid, targetSolution, split);
				this.ForceUpdateByExternalCause(uid, component);
				return;
			}
			else
			{
				if (!this._tagSystem.HasTag(args.Used, "PlantSampleTaker"))
				{
					if (base.HasComp<SharpComponent>(args.Used))
					{
						this.DoHarvest(uid, args.User, component);
					}
					ProduceComponent produce;
					if (base.TryComp<ProduceComponent>(args.Used, ref produce))
					{
						this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-compost-message", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("owner", uid),
							new ValueTuple<string, object>("usingItem", args.Used)
						}), args.User, PopupType.Medium);
						this._popupSystem.PopupEntity(Loc.GetString("plant-holder-component-compost-others-message", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("user", Identity.Entity(args.User, this.EntityManager)),
							new ValueTuple<string, object>("usingItem", args.Used),
							new ValueTuple<string, object>("owner", uid)
						}), uid, Filter.PvsExcept(args.User, 2f, null), true, PopupType.Small);
						Solution solution2;
						if (this._solutionSystem.TryGetSolution(args.Used, produce.SolutionName, out solution2, null))
						{
							this._solutionSystem.TryAddSolution(args.Used, solution2, this._solutionSystem.SplitSolution(args.Used, solution2, solution2.Volume));
							this.ForceUpdateByExternalCause(uid, component);
						}
						this.EntityManager.QueueDeleteEntity(args.Used);
					}
					return;
				}
				if (component.Seed == null)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-nothing-to-sample-message"), args.User, PopupType.Small);
					return;
				}
				if (component.Sampled)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-already-sampled-message"), args.User, PopupType.Small);
					return;
				}
				if (component.Dead)
				{
					this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-dead-plant-message"), args.User, PopupType.Small);
					return;
				}
				component.Seed.Unique = false;
				this._botanySystem.SpawnSeedPacket(component.Seed, base.Transform(args.User).Coordinates).RandomOffset(0.25f);
				string displayName = Loc.GetString(component.Seed.DisplayName);
				this._popupSystem.PopupCursor(Loc.GetString("plant-holder-component-take-sample-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seedName", displayName)
				}), args.User, PopupType.Small);
				component.Health -= (float)(this._random.Next(3, 5) * 10);
				if (RandomExtensions.Prob(this._random, 0.3f))
				{
					component.Sampled = true;
				}
				this.CheckLevelSanity(uid, component);
				this.ForceUpdateByExternalCause(uid, component);
				return;
			}
		}

		// Token: 0x0600257E RID: 9598 RVA: 0x000C555C File Offset: 0x000C375C
		[NullableContext(1)]
		private void OnInteractHand(EntityUid uid, PlantHolderComponent component, InteractHandEvent args)
		{
			this.DoHarvest(uid, args.User, component);
		}

		// Token: 0x0600257F RID: 9599 RVA: 0x000C556D File Offset: 0x000C376D
		public void WeedInvasion()
		{
		}

		// Token: 0x06002580 RID: 9600 RVA: 0x000C5570 File Offset: 0x000C3770
		public void Update(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			this.UpdateReagents(uid, component);
			TimeSpan curTime = this._gameTiming.CurTime;
			if (component.ForceUpdate)
			{
				component.ForceUpdate = false;
			}
			else if (curTime < component.LastCycle + component.CycleDelay)
			{
				if (component.UpdateSpriteAfterUpdate)
				{
					this.UpdateSprite(uid, component);
				}
				return;
			}
			component.LastCycle = curTime;
			if (component.MutationLevel > 0f)
			{
				this.Mutate(uid, Math.Min(component.MutationLevel, 25f), component);
				component.MutationLevel = 0f;
			}
			if (component.WaterLevel > 10f && component.NutritionLevel > 2f && RandomExtensions.Prob(this._random, (component.Seed == null) ? 0.05f : 0.01f))
			{
				component.WeedLevel += 1f * component.WeedCoefficient;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (component.WeedLevel >= 10f && RandomExtensions.Prob(this._random, 0.1f) && (component.Seed == null || component.WeedLevel >= component.Seed.WeedTolerance + 2f))
			{
				this.WeedInvasion();
			}
			if (component.Seed == null || component.Dead)
			{
				if (component.UpdateSpriteAfterUpdate)
				{
					this.UpdateSprite(uid, component);
				}
				return;
			}
			if (RandomExtensions.Prob(this._random, 0.01f))
			{
				component.PestLevel += 0.5f;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (component.SkipAging > 0)
			{
				PlantHolderComponent plantHolderComponent = component;
				int skipAging = plantHolderComponent.SkipAging;
				plantHolderComponent.SkipAging = skipAging - 1;
			}
			else
			{
				if (RandomExtensions.Prob(this._random, 0.8f))
				{
					component.Age++;
				}
				component.UpdateSpriteAfterUpdate = true;
			}
			if (component.Seed.NutrientConsumption > 0f && component.NutritionLevel > 0f && RandomExtensions.Prob(this._random, 0.75f))
			{
				component.NutritionLevel -= MathF.Max(0f, component.Seed.NutrientConsumption * 1f);
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (component.Seed.WaterConsumption > 0f && component.WaterLevel > 0f && RandomExtensions.Prob(this._random, 0.75f))
			{
				component.WaterLevel -= MathF.Max(0f, component.Seed.NutrientConsumption * 4f * 1f);
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			float healthMod = (float)this._random.Next(1, 3) * 1f;
			if (!component.Seed.Viable)
			{
				this.AffectGrowth(uid, -1, component);
				component.Health -= 6f * healthMod;
			}
			if (RandomExtensions.Prob(this._random, 0.35f))
			{
				if (component.NutritionLevel > 2f)
				{
					component.Health += healthMod;
				}
				else
				{
					this.AffectGrowth(uid, -1, component);
					component.Health -= healthMod;
				}
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (RandomExtensions.Prob(this._random, 0.35f))
			{
				if (component.WaterLevel > 10f)
				{
					component.Health += healthMod;
				}
				else
				{
					this.AffectGrowth(uid, -1, component);
					component.Health -= healthMod;
				}
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			GasMixture environment = this._atmosphere.GetContainingMixture(uid, true, true, null) ?? GasMixture.SpaceGas;
			if (component.Seed.ConsumeGasses.Count > 0)
			{
				component.MissingGas = 0;
				foreach (KeyValuePair<Gas, float> keyValuePair in component.Seed.ConsumeGasses)
				{
					Gas gas3;
					float num;
					keyValuePair.Deconstruct(out gas3, out num);
					Gas gas = gas3;
					float amount = num;
					if (environment.GetMoles(gas) < amount)
					{
						component.MissingGas++;
					}
					else
					{
						environment.AdjustMoles(gas, -amount);
					}
				}
				if (component.MissingGas > 0)
				{
					component.Health -= (float)component.MissingGas * 1f;
					if (component.DrawWarnings)
					{
						component.UpdateSpriteAfterUpdate = true;
					}
				}
			}
			float pressure = environment.Pressure;
			if (pressure < component.Seed.LowPressureTolerance || pressure > component.Seed.HighPressureTolerance)
			{
				component.Health -= healthMod;
				component.ImproperPressure = true;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			else
			{
				component.ImproperPressure = false;
			}
			if (MathF.Abs(environment.Temperature - component.Seed.IdealHeat) > component.Seed.HeatTolerance)
			{
				component.Health -= healthMod;
				component.ImproperHeat = true;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			else
			{
				component.ImproperHeat = false;
			}
			int exudeCount = component.Seed.ExudeGasses.Count;
			if (exudeCount > 0)
			{
				foreach (KeyValuePair<Gas, float> keyValuePair in component.Seed.ExudeGasses)
				{
					Gas gas3;
					float num;
					keyValuePair.Deconstruct(out gas3, out num);
					Gas gas2 = gas3;
					float amount2 = num;
					environment.AdjustMoles(gas2, MathF.Max(1f, MathF.Round(amount2 * MathF.Round(component.Seed.Potency) / (float)exudeCount)));
				}
			}
			if (component.Toxins > 0f)
			{
				float toxinUptake = MathF.Max(1f, MathF.Round(component.Toxins / 10f));
				if (component.Toxins > component.Seed.ToxinsTolerance)
				{
					component.Health -= toxinUptake;
				}
				component.Toxins -= toxinUptake;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (component.PestLevel > 0f)
			{
				if (component.PestLevel > component.Seed.PestTolerance)
				{
					component.Health -= 1f;
				}
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if (component.WeedLevel > 0f)
			{
				if (component.WeedLevel >= component.Seed.WeedTolerance)
				{
					component.Health -= 1f;
				}
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			if ((float)component.Age > component.Seed.Lifespan)
			{
				component.Health -= (float)this._random.Next(3, 5) * 1f;
				if (component.DrawWarnings)
				{
					component.UpdateSpriteAfterUpdate = true;
				}
			}
			else if (component.Age < 0)
			{
				this._botanySystem.SpawnSeedPacket(component.Seed, base.Transform(uid).Coordinates);
				this.RemovePlant(uid, component);
				component.ForceUpdate = true;
				this.Update(uid, component);
			}
			this.CheckHealth(uid, component);
			if (component.Harvest && component.Seed.HarvestRepeat == HarvestType.SelfHarvest)
			{
				this.AutoHarvest(uid, component);
			}
			if (!component.Dead && component.Seed.ProductPrototypes.Count > 0)
			{
				if ((float)component.Age > component.Seed.Production)
				{
					if ((float)(component.Age - component.LastProduce) > component.Seed.Production && !component.Harvest)
					{
						component.Harvest = true;
						component.LastProduce = component.Age;
					}
				}
				else if (component.Harvest)
				{
					component.Harvest = false;
					component.LastProduce = component.Age;
				}
			}
			this.CheckLevelSanity(uid, component);
			if (component.Seed.Sentient)
			{
				GhostTakeoverAvailableComponent comp = base.EnsureComp<GhostTakeoverAvailableComponent>(uid);
				comp.RoleName = base.MetaData(uid).EntityName;
				comp.RoleDescription = Loc.GetString("station-event-random-sentience-role-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", comp.RoleName)
				});
			}
			if (component.UpdateSpriteAfterUpdate)
			{
				this.UpdateSprite(uid, component);
			}
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x000C5DBC File Offset: 0x000C3FBC
		public void CheckLevelSanity(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Seed != null)
			{
				component.Health = MathHelper.Clamp(component.Health, 0f, component.Seed.Endurance);
			}
			else
			{
				component.Health = 0f;
				component.Dead = false;
			}
			component.MutationLevel = MathHelper.Clamp(component.MutationLevel, 0f, 100f);
			component.NutritionLevel = MathHelper.Clamp(component.NutritionLevel, 0f, 100f);
			component.WaterLevel = MathHelper.Clamp(component.WaterLevel, 0f, 100f);
			component.PestLevel = MathHelper.Clamp(component.PestLevel, 0f, 10f);
			component.WeedLevel = MathHelper.Clamp(component.WeedLevel, 0f, 10f);
			component.Toxins = MathHelper.Clamp(component.Toxins, 0f, 100f);
			component.YieldMod = MathHelper.Clamp(component.YieldMod, 0, 2);
			component.MutationMod = MathHelper.Clamp(component.MutationMod, 0f, 3f);
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x000C5EE4 File Offset: 0x000C40E4
		public bool DoHarvest(EntityUid plantholder, EntityUid user, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(plantholder, ref component, true))
			{
				return false;
			}
			if (component.Seed == null || base.Deleted(user, null))
			{
				return false;
			}
			if (component.Harvest && !component.Dead)
			{
				HandsComponent hands;
				if (base.TryComp<HandsComponent>(user, ref hands))
				{
					if (!this._botanySystem.CanHarvest(component.Seed, hands.ActiveHandEntity))
					{
						return false;
					}
				}
				else if (!this._botanySystem.CanHarvest(component.Seed, null))
				{
					return false;
				}
				this._botanySystem.Harvest(component.Seed, user, component.YieldMod);
				this.AfterHarvest(plantholder, component);
				return true;
			}
			if (!component.Dead)
			{
				return false;
			}
			this.RemovePlant(plantholder, component);
			this.AfterHarvest(plantholder, component);
			return true;
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x000C5FA8 File Offset: 0x000C41A8
		public void AutoHarvest(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Seed == null || !component.Harvest)
			{
				return;
			}
			this._botanySystem.AutoHarvest(component.Seed, base.Transform(uid).Coordinates, 1);
			this.AfterHarvest(uid, component);
		}

		// Token: 0x06002584 RID: 9604 RVA: 0x000C5FFC File Offset: 0x000C41FC
		private void AfterHarvest(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.Harvest = false;
			component.LastProduce = component.Age;
			SeedData seed = component.Seed;
			if (seed != null && seed.HarvestRepeat == HarvestType.NoRepeat)
			{
				this.RemovePlant(uid, component);
			}
			this.CheckLevelSanity(uid, component);
			this.UpdateSprite(uid, component);
		}

		// Token: 0x06002585 RID: 9605 RVA: 0x000C6058 File Offset: 0x000C4258
		public void CheckHealth(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Health <= 0f)
			{
				this.Die(uid, component);
			}
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x000C607C File Offset: 0x000C427C
		public void Die(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.Dead = true;
			component.Harvest = false;
			component.MutationLevel = 0f;
			component.YieldMod = 1;
			component.MutationMod = 1f;
			component.ImproperLight = false;
			component.ImproperHeat = false;
			component.ImproperPressure = false;
			component.WeedLevel += 1f;
			component.PestLevel = 0f;
			this.UpdateSprite(uid, component);
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x000C60FC File Offset: 0x000C42FC
		public void RemovePlant(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.YieldMod = 1;
			component.MutationMod = 1f;
			component.PestLevel = 0f;
			component.Seed = null;
			component.Dead = false;
			component.Age = 0;
			component.Sampled = false;
			component.Harvest = false;
			component.ImproperLight = false;
			component.ImproperPressure = false;
			component.ImproperHeat = false;
			this.UpdateSprite(uid, component);
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x000C6174 File Offset: 0x000C4374
		public void AffectGrowth(EntityUid uid, int amount, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Seed == null)
			{
				return;
			}
			if (amount > 0)
			{
				if ((float)component.Age < component.Seed.Maturation)
				{
					component.Age += amount;
					return;
				}
				if (!component.Harvest && (float)component.Seed.Yield <= 0f)
				{
					component.LastProduce -= amount;
					return;
				}
			}
			else
			{
				if ((float)component.Age < component.Seed.Maturation)
				{
					PlantHolderComponent plantHolderComponent = component;
					int skipAging = plantHolderComponent.SkipAging;
					plantHolderComponent.SkipAging = skipAging + 1;
					return;
				}
				if (!component.Harvest && (float)component.Seed.Yield <= 0f)
				{
					component.LastProduce += amount;
				}
			}
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x000C6236 File Offset: 0x000C4436
		public void AdjustNutrient(EntityUid uid, float amount, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.NutritionLevel += amount;
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000C6253 File Offset: 0x000C4453
		public void AdjustWater(EntityUid uid, float amount, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.WaterLevel += amount;
			if (amount > 0f)
			{
				component.Toxins -= amount * 4f;
			}
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x000C628C File Offset: 0x000C448C
		public void UpdateReagents(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			Solution solution;
			if (!this._solutionSystem.TryGetSolution(uid, component.SoilSolutionName, out solution, null))
			{
				return;
			}
			if (solution.Volume > 0 && component.MutationLevel < 25f)
			{
				FixedPoint2 amt = FixedPoint2.New(1);
				foreach (Solution.ReagentQuantity reagentQuantity in this._solutionSystem.RemoveEachReagent(uid, solution, amt))
				{
					string text;
					FixedPoint2 fixedPoint;
					reagentQuantity.Deconstruct(out text, out fixedPoint);
					string reagentId = text;
					FixedPoint2 quantity = fixedPoint;
					this._prototype.Index<ReagentPrototype>(reagentId).ReactionPlant(new EntityUid?(uid), new Solution.ReagentQuantity(reagentId, quantity), solution);
				}
			}
			this.CheckLevelSanity(uid, component);
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000C635C File Offset: 0x000C455C
		private void Mutate(EntityUid uid, float severity, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.Seed != null)
			{
				this.EnsureUniqueSeed(uid, component);
				this._mutation.MutateSeed(component.Seed, severity);
			}
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000C6390 File Offset: 0x000C4590
		public void UpdateSprite(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			component.UpdateSpriteAfterUpdate = false;
			if (component.Seed != null && component.Seed.Bioluminescent)
			{
				PointLightComponent light = base.EnsureComp<PointLightComponent>(uid);
				light.Radius = component.Seed.BioluminescentRadius;
				light.Color = component.Seed.BioluminescentColor;
				light.CastShadows = false;
				base.Dirty(light, null);
			}
			else
			{
				base.RemComp<PointLightComponent>(uid);
			}
			AppearanceComponent app;
			if (!base.TryComp<AppearanceComponent>(uid, ref app))
			{
				return;
			}
			if (component.Seed != null)
			{
				if (component.DrawWarnings)
				{
					this._appearance.SetData(uid, PlantHolderVisuals.HealthLight, component.Health <= component.Seed.Endurance / 2f, null);
				}
				if (component.Dead)
				{
					this._appearance.SetData(uid, PlantHolderVisuals.PlantRsi, component.Seed.PlantRsi.ToString(), app);
					this._appearance.SetData(uid, PlantHolderVisuals.PlantState, "dead", app);
				}
				else if (component.Harvest)
				{
					this._appearance.SetData(uid, PlantHolderVisuals.PlantRsi, component.Seed.PlantRsi.ToString(), app);
					this._appearance.SetData(uid, PlantHolderVisuals.PlantState, "harvest", app);
				}
				else if ((float)component.Age < component.Seed.Maturation)
				{
					int growthStage = Math.Max(1, (int)((float)(component.Age * component.Seed.GrowthStages) / component.Seed.Maturation));
					this._appearance.SetData(uid, PlantHolderVisuals.PlantRsi, component.Seed.PlantRsi.ToString(), app);
					SharedAppearanceSystem appearance = this._appearance;
					Enum @enum = PlantHolderVisuals.PlantState;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler.AppendLiteral("stage-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(growthStage);
					appearance.SetData(uid, @enum, defaultInterpolatedStringHandler.ToStringAndClear(), app);
					component.LastProduce = component.Age;
				}
				else
				{
					this._appearance.SetData(uid, PlantHolderVisuals.PlantRsi, component.Seed.PlantRsi.ToString(), app);
					SharedAppearanceSystem appearance2 = this._appearance;
					Enum enum2 = PlantHolderVisuals.PlantState;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
					defaultInterpolatedStringHandler.AppendLiteral("stage-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(component.Seed.GrowthStages);
					appearance2.SetData(uid, enum2, defaultInterpolatedStringHandler.ToStringAndClear(), app);
				}
			}
			else
			{
				this._appearance.SetData(uid, PlantHolderVisuals.PlantState, "", app);
				this._appearance.SetData(uid, PlantHolderVisuals.HealthLight, false, app);
			}
			if (!component.DrawWarnings)
			{
				return;
			}
			this._appearance.SetData(uid, PlantHolderVisuals.WaterLight, component.WaterLevel <= 10f, app);
			this._appearance.SetData(uid, PlantHolderVisuals.NutritionLight, component.NutritionLevel <= 2f, app);
			this._appearance.SetData(uid, PlantHolderVisuals.AlertLight, component.WeedLevel >= 5f || component.PestLevel >= 5f || component.Toxins >= 40f || component.ImproperHeat || component.ImproperLight || component.ImproperPressure || component.MissingGas > 0, app);
			this._appearance.SetData(uid, PlantHolderVisuals.HarvestLight, component.Harvest, app);
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x000C6708 File Offset: 0x000C4908
		public void EnsureUniqueSeed(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			SeedData seed = component.Seed;
			if (seed != null && !seed.Unique)
			{
				component.Seed = component.Seed.Clone();
			}
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x000C6748 File Offset: 0x000C4948
		public void ForceUpdateByExternalCause(EntityUid uid, PlantHolderComponent component = null)
		{
			if (!base.Resolve<PlantHolderComponent>(uid, ref component, true))
			{
				return;
			}
			PlantHolderComponent plantHolderComponent = component;
			int skipAging = plantHolderComponent.SkipAging;
			plantHolderComponent.SkipAging = skipAging + 1;
			component.ForceUpdate = true;
			this.Update(uid, component);
		}

		// Token: 0x0400171D RID: 5917
		[Nullable(1)]
		[Dependency]
		private readonly BotanySystem _botanySystem;

		// Token: 0x0400171E RID: 5918
		[Nullable(1)]
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400171F RID: 5919
		[Nullable(1)]
		[Dependency]
		private readonly MutationSystem _mutation;

		// Token: 0x04001720 RID: 5920
		[Nullable(1)]
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x04001721 RID: 5921
		[Nullable(1)]
		[Dependency]
		private readonly AudioSystem _audio;

		// Token: 0x04001722 RID: 5922
		[Nullable(1)]
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001723 RID: 5923
		[Nullable(1)]
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001724 RID: 5924
		[Nullable(1)]
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x04001725 RID: 5925
		[Nullable(1)]
		[Dependency]
		private readonly SolutionContainerSystem _solutionSystem;

		// Token: 0x04001726 RID: 5926
		[Nullable(1)]
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001727 RID: 5927
		[Nullable(1)]
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x04001728 RID: 5928
		public const float HydroponicsSpeedMultiplier = 1f;

		// Token: 0x04001729 RID: 5929
		public const float HydroponicsConsumptionMultiplier = 4f;
	}
}
