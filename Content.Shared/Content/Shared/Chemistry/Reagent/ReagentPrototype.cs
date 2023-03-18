using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E7 RID: 1511
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("reagent", 1)]
	[DataDefinition]
	public sealed class ReagentPrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x1700039A RID: 922
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x0003B96B File Offset: 0x00039B6B
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001230 RID: 4656 RVA: 0x0003B973 File Offset: 0x00039B73
		[DataField("name", false, 1, true, false, null)]
		private string Name { get; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x0003B97B File Offset: 0x00039B7B
		[ViewVariables]
		public string LocalizedName
		{
			get
			{
				return Loc.GetString(this.Name);
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06001232 RID: 4658 RVA: 0x0003B988 File Offset: 0x00039B88
		[DataField("group", false, 1, false, false, null)]
		public string Group { get; } = "Unknown";

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001233 RID: 4659 RVA: 0x0003B990 File Offset: 0x00039B90
		// (set) Token: 0x06001234 RID: 4660 RVA: 0x0003B998 File Offset: 0x00039B98
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ReagentPrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] private set; }

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x0003B9A1 File Offset: 0x00039BA1
		// (set) Token: 0x06001236 RID: 4662 RVA: 0x0003B9A9 File Offset: 0x00039BA9
		[NeverPushInheritance]
		[AbstractDataField(1)]
		public bool Abstract { get; private set; }

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x0003B9B2 File Offset: 0x00039BB2
		[DataField("desc", false, 1, true, false, null)]
		private string Description { get; }

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x0003B9BA File Offset: 0x00039BBA
		[ViewVariables]
		public string LocalizedDescription
		{
			get
			{
				return Loc.GetString(this.Description);
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x0003B9C7 File Offset: 0x00039BC7
		[DataField("physicalDesc", false, 1, true, false, null)]
		private string PhysicalDescription { get; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0003B9CF File Offset: 0x00039BCF
		[ViewVariables]
		public string LocalizedPhysicalDescription
		{
			get
			{
				return Loc.GetString(this.PhysicalDescription);
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0003B9DC File Offset: 0x00039BDC
		[DataField("flavor", false, 1, false, false, null)]
		public string Flavor { get; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x0003B9E4 File Offset: 0x00039BE4
		[DataField("color", false, 1, false, false, null)]
		public Color SubstanceColor { get; } = Color.White;

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0003B9EC File Offset: 0x00039BEC
		[DataField("specificHeat", false, 1, false, false, null)]
		public float SpecificHeat { get; } = 1f;

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0003B9F4 File Offset: 0x00039BF4
		[DataField("boilingPoint", false, 1, false, false, null)]
		public float? BoilingPoint { get; }

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x0003B9FC File Offset: 0x00039BFC
		[DataField("meltingPoint", false, 1, false, false, null)]
		public float? MeltingPoint { get; }

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x0003BA04 File Offset: 0x00039C04
		[Nullable(2)]
		[DataField("metamorphicSprite", false, 1, false, false, null)]
		public SpriteSpecifier MetamorphicSprite { [NullableContext(2)] get; }

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x0003BA0C File Offset: 0x00039C0C
		[DataField("pricePerUnit", false, 1, false, false, null)]
		public float PricePerUnit { get; }

		// Token: 0x06001242 RID: 4674 RVA: 0x0003BA14 File Offset: 0x00039C14
		public Color GetSubstanceTextColor()
		{
			float highestValue = MathF.Max(this.SubstanceColor.R, MathF.Max(this.SubstanceColor.G, this.SubstanceColor.B));
			float difference = 0.5f - highestValue;
			if (difference > 0f)
			{
				return new Color(this.SubstanceColor.R + difference, this.SubstanceColor.G + difference, this.SubstanceColor.B + difference, 1f);
			}
			return this.SubstanceColor;
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0003BA98 File Offset: 0x00039C98
		public FixedPoint2 ReactionTile(TileRef tile, FixedPoint2 reactVolume)
		{
			FixedPoint2 removed = FixedPoint2.Zero;
			if (tile.Tile.IsEmpty)
			{
				return removed;
			}
			foreach (ITileReaction reaction in this.TileReactions)
			{
				removed += reaction.TileReact(tile, this, reactVolume - removed);
				if (removed > reactVolume)
				{
					throw new Exception("Removed more than we have!");
				}
				if (removed == reactVolume)
				{
					break;
				}
			}
			return removed;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x0003BB30 File Offset: 0x00039D30
		public void ReactionPlant(EntityUid? plantHolder, Solution.ReagentQuantity amount, Solution solution)
		{
			if (plantHolder == null)
			{
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			ReagentEffectArgs args = new ReagentEffectArgs(plantHolder.Value, null, solution, this, amount.Quantity, entMan, null, 1f);
			foreach (ReagentEffect plantMetabolizable in this.PlantMetabolisms)
			{
				if (plantMetabolizable.ShouldApply(args, random))
				{
					if (plantMetabolizable.ShouldLog)
					{
						EntityUid entity = args.SolutionEntity;
						SharedAdminLogSystem sharedAdminLogSystem = EntitySystem.Get<SharedAdminLogSystem>();
						LogType type = LogType.ReagentEffect;
						LogImpact logImpact = plantMetabolizable.LogImpact;
						LogStringHandler logStringHandler = new LogStringHandler(59, 4);
						logStringHandler.AppendLiteral("Plant metabolism effect ");
						logStringHandler.AppendFormatted(plantMetabolizable.GetType().Name, 0, "effect");
						logStringHandler.AppendLiteral(" of reagent ");
						logStringHandler.AppendFormatted(this.ID, 0, "reagent");
						logStringHandler.AppendLiteral(" applied on entity ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(entMan.ToPrettyString(entity), "entity", "entMan.ToPrettyString(entity)");
						logStringHandler.AppendLiteral(" at ");
						logStringHandler.AppendFormatted<EntityCoordinates>(entMan.GetComponent<TransformComponent>(entity).Coordinates, "coordinates", "entMan.GetComponent<TransformComponent>(entity).Coordinates");
						sharedAdminLogSystem.Add(type, logImpact, ref logStringHandler);
					}
					plantMetabolizable.Effect(args);
				}
			}
		}

		// Token: 0x04001121 RID: 4385
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("metabolisms", false, 1, false, true, typeof(PrototypeIdDictionarySerializer<ReagentEffectsEntry, MetabolismGroupPrototype>))]
		public Dictionary<string, ReagentEffectsEntry> Metabolisms;

		// Token: 0x04001122 RID: 4386
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("reactiveEffects", false, 1, false, true, typeof(PrototypeIdDictionarySerializer<ReactiveReagentEffectEntry, ReactiveGroupPrototype>))]
		public Dictionary<string, ReactiveReagentEffectEntry> ReactiveEffects;

		// Token: 0x04001123 RID: 4387
		[DataField("tileReactions", false, 1, false, true, null)]
		public readonly List<ITileReaction> TileReactions = new List<ITileReaction>(0);

		// Token: 0x04001124 RID: 4388
		[DataField("plantMetabolism", false, 1, false, true, null)]
		public readonly List<ReagentEffect> PlantMetabolisms = new List<ReagentEffect>(0);
	}
}
