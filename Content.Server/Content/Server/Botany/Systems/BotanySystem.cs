using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Kitchen.Components;
using Content.Server.Popups;
using Content.Shared.Botany;
using Content.Shared.Chemistry.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Botany.Systems
{
	// Token: 0x020006FC RID: 1788
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BotanySystem : EntitySystem
	{
		// Token: 0x0600256C RID: 9580 RVA: 0x000C4474 File Offset: 0x000C2674
		public void ProduceGrown(EntityUid uid, ProduceComponent produce)
		{
			SeedData seed;
			if (!this.TryGetSeed(produce, out seed))
			{
				return;
			}
			SpriteComponent sprite;
			if (base.TryComp<SpriteComponent>(uid, ref sprite))
			{
				sprite.LayerSetRSI(0, seed.PlantRsi);
				sprite.LayerSetState(0, seed.PlantIconState);
			}
			Solution.ReagentQuantity[] reagents = new Solution.ReagentQuantity[seed.Chemicals.Count];
			int i = 0;
			foreach (KeyValuePair<string, SeedChemQuantity> keyValuePair in seed.Chemicals)
			{
				string text;
				SeedChemQuantity seedChemQuantity;
				keyValuePair.Deconstruct(out text, out seedChemQuantity);
				string chem = text;
				SeedChemQuantity quantity = seedChemQuantity;
				FixedPoint2 amount = FixedPoint2.New(quantity.Min);
				if (quantity.PotencyDivisor > 0 && seed.Potency > 0f)
				{
					amount += FixedPoint2.New(seed.Potency / (float)quantity.PotencyDivisor);
				}
				amount = FixedPoint2.New((int)MathHelper.Clamp(amount.Float(), (float)quantity.Min, (float)quantity.Max));
				reagents[i++] = new Solution.ReagentQuantity(chem, amount);
			}
			this._solutionContainerSystem.EnsureSolution(uid, produce.SolutionName, reagents, true, null);
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x000C45B0 File Offset: 0x000C27B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SeedComponent, ExaminedEvent>(new ComponentEventHandler<SeedComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x000C45CC File Offset: 0x000C27CC
		public bool TryGetSeed(SeedComponent comp, [Nullable(2)] [NotNullWhen(true)] out SeedData seed)
		{
			if (comp.Seed != null)
			{
				seed = comp.Seed;
				return true;
			}
			SeedPrototype protoSeed;
			if (comp.SeedId != null && this._prototypeManager.TryIndex<SeedPrototype>(comp.SeedId, ref protoSeed))
			{
				seed = protoSeed;
				return true;
			}
			seed = null;
			return false;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x000C4614 File Offset: 0x000C2814
		public bool TryGetSeed(ProduceComponent comp, [Nullable(2)] [NotNullWhen(true)] out SeedData seed)
		{
			if (comp.Seed != null)
			{
				seed = comp.Seed;
				return true;
			}
			SeedPrototype protoSeed;
			if (comp.SeedId != null && this._prototypeManager.TryIndex<SeedPrototype>(comp.SeedId, ref protoSeed))
			{
				seed = protoSeed;
				return true;
			}
			seed = null;
			return false;
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x000C465C File Offset: 0x000C285C
		private void OnExamined(EntityUid uid, SeedComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			SeedData seed;
			if (!this.TryGetSeed(component, out seed))
			{
				return;
			}
			string name = Loc.GetString(seed.DisplayName);
			args.PushMarkup(Loc.GetString("seed-component-description", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seedName", name)
			}));
			args.PushMarkup(Loc.GetString("seed-component-plant-yield-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seedYield", seed.Yield)
			}));
			args.PushMarkup(Loc.GetString("seed-component-plant-potency-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seedPotency", seed.Potency)
			}));
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x000C4718 File Offset: 0x000C2918
		public EntityUid SpawnSeedPacket(SeedData proto, EntityCoordinates transformCoordinates)
		{
			EntityUid seed = base.Spawn(proto.PacketPrototype, transformCoordinates);
			base.EnsureComp<SeedComponent>(seed).Seed = proto;
			SpriteComponent sprite;
			if (base.TryComp<SpriteComponent>(seed, ref sprite))
			{
				sprite.LayerSetSprite(0, new SpriteSpecifier.Rsi(proto.PlantRsi, "seed"));
			}
			string name = Loc.GetString(proto.Name);
			string noun = Loc.GetString(proto.Noun);
			string val = Loc.GetString("botany-seed-packet-name", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seedName", name),
				new ValueTuple<string, object>("seedNoun", noun)
			});
			base.MetaData(seed).EntityName = val;
			return seed;
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x000C47BF File Offset: 0x000C29BF
		public IEnumerable<EntityUid> AutoHarvest(SeedData proto, EntityCoordinates position, int yieldMod = 1)
		{
			if (position.IsValid(this.EntityManager) && proto.ProductPrototypes.Count > 0)
			{
				return this.GenerateProduct(proto, position, yieldMod);
			}
			return Enumerable.Empty<EntityUid>();
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x000C47F0 File Offset: 0x000C29F0
		public IEnumerable<EntityUid> Harvest(SeedData proto, EntityUid user, int yieldMod = 1)
		{
			if (proto.ProductPrototypes.Count == 0 || proto.Yield <= 0)
			{
				this._popupSystem.PopupCursor(Loc.GetString("botany-harvest-fail-message"), user, PopupType.Medium);
				return Enumerable.Empty<EntityUid>();
			}
			string name = Loc.GetString(proto.DisplayName);
			this._popupSystem.PopupCursor(Loc.GetString("botany-harvest-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", name)
			}), user, PopupType.Medium);
			return this.GenerateProduct(proto, base.Transform(user).Coordinates, yieldMod);
		}

		// Token: 0x06002574 RID: 9588 RVA: 0x000C4880 File Offset: 0x000C2A80
		public IEnumerable<EntityUid> GenerateProduct(SeedData proto, EntityCoordinates position, int yieldMod = 1)
		{
			int totalYield = 0;
			if (proto.Yield > -1)
			{
				if (yieldMod < 0)
				{
					totalYield = proto.Yield;
				}
				else
				{
					totalYield = proto.Yield * yieldMod;
				}
				totalYield = Math.Max(1, totalYield);
			}
			List<EntityUid> products = new List<EntityUid>();
			if (totalYield > 1 || proto.HarvestRepeat != HarvestType.NoRepeat)
			{
				proto.Unique = false;
			}
			for (int i = 0; i < totalYield; i++)
			{
				string product = RandomExtensions.Pick<string>(this._robustRandom, proto.ProductPrototypes);
				EntityUid entity = base.Spawn(product, position);
				entity.RandomOffset(0.25f);
				products.Add(entity);
				ProduceComponent produce = base.EnsureComp<ProduceComponent>(entity);
				produce.Seed = proto;
				this.ProduceGrown(entity, produce);
				this._appearance.SetData(entity, ProduceVisuals.Potency, proto.Potency, null);
				if (proto.Mysterious)
				{
					MetaDataComponent metaDataComponent = base.MetaData(entity);
					metaDataComponent.EntityName += "?";
					metaDataComponent.EntityDescription = metaDataComponent.EntityDescription + " " + Loc.GetString("botany-mysterious-description-addon");
				}
				if (proto.Bioluminescent)
				{
					PointLightComponent light = base.EnsureComp<PointLightComponent>(entity);
					light.Radius = proto.BioluminescentRadius;
					light.Color = proto.BioluminescentColor;
					light.CastShadows = false;
					base.Dirty(light, null);
				}
				if (proto.Slip)
				{
					SlipperyComponent slippery = base.EnsureComp<SlipperyComponent>(entity);
					this.EntityManager.Dirty(slippery, null);
					base.EnsureComp<StepTriggerComponent>(entity);
				}
			}
			return products;
		}

		// Token: 0x06002575 RID: 9589 RVA: 0x000C49F8 File Offset: 0x000C2BF8
		public bool CanHarvest(SeedData proto, EntityUid? held = null)
		{
			return !proto.Ligneous || (proto.Ligneous && held != null && base.HasComp<SharpComponent>(held));
		}

		// Token: 0x04001716 RID: 5910
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x04001717 RID: 5911
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001718 RID: 5912
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001719 RID: 5913
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400171A RID: 5914
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;
	}
}
