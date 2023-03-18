using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x0200064E RID: 1614
	[DataDefinition]
	public sealed class CreateEntityTileReaction : ITileReaction
	{
		// Token: 0x0600223A RID: 8762 RVA: 0x000B3288 File Offset: 0x000B1488
		[NullableContext(1)]
		public FixedPoint2 TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			if (reactVolume >= this.Usage)
			{
				IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
				if (this.Whitelist != null)
				{
					int acc = 0;
					foreach (EntityUid ent in tile.GetEntitiesInTile(4, null))
					{
						if (this.Whitelist.IsValid(ent, null))
						{
							acc++;
						}
						if (acc >= this.MaxOnTile)
						{
							return FixedPoint2.Zero;
						}
					}
				}
				IRobustRandom robustRandom = IoCManager.Resolve<IRobustRandom>();
				float xoffs = robustRandom.NextFloat(-this.RandomOffsetMax, this.RandomOffsetMax);
				float yoffs = robustRandom.NextFloat(-this.RandomOffsetMax, this.RandomOffsetMax);
				EntityCoordinates pos = tile.GridPosition(null).Offset(new Vector2(0.5f + xoffs, 0.5f + yoffs));
				entMan.SpawnEntity(this.Entity, pos);
				return this.Usage;
			}
			return FixedPoint2.Zero;
		}

		// Token: 0x04001525 RID: 5413
		[Nullable(1)]
		[DataField("entity", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Entity;

		// Token: 0x04001526 RID: 5414
		[DataField("usage", false, 1, false, false, null)]
		public FixedPoint2 Usage = FixedPoint2.New(1);

		// Token: 0x04001527 RID: 5415
		[DataField("maxOnTile", false, 1, false, false, null)]
		public int MaxOnTile = 1;

		// Token: 0x04001528 RID: 5416
		[Nullable(2)]
		[DataField("maxOnTileWhitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04001529 RID: 5417
		[DataField("randomOffsetMax", false, 1, false, false, null)]
		public float RandomOffsetMax;
	}
}
