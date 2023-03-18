using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Cleanable;
using Content.Server.Decals;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Decals;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.TileReactions
{
	// Token: 0x0200064D RID: 1613
	[DataDefinition]
	public sealed class CleanTileReaction : ITileReaction
	{
		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06002236 RID: 8758 RVA: 0x000B3134 File Offset: 0x000B1334
		// (set) Token: 0x06002237 RID: 8759 RVA: 0x000B313C File Offset: 0x000B133C
		[DataField("cleanAmountMultiplier", false, 1, false, false, null)]
		public float CleanAmountMultiplier { get; private set; } = 1f;

		// Token: 0x06002238 RID: 8760 RVA: 0x000B3148 File Offset: 0x000B1348
		[NullableContext(1)]
		FixedPoint2 ITileReaction.TileReact(TileRef tile, ReagentPrototype reagent, FixedPoint2 reactVolume)
		{
			EntityUid[] array = EntitySystem.Get<EntityLookupSystem>().GetEntitiesIntersecting(tile, 46).ToArray<EntityUid>();
			FixedPoint2 amount = FixedPoint2.Zero;
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			foreach (EntityUid entity in array)
			{
				CleanableComponent cleanable;
				if (entMan.TryGetComponent<CleanableComponent>(entity, ref cleanable))
				{
					FixedPoint2 next = amount + cleanable.CleanAmount * this.CleanAmountMultiplier;
					if (reactVolume < next)
					{
						break;
					}
					amount = next;
					entMan.QueueDeleteEntity(entity);
				}
			}
			DecalSystem decalSystem = EntitySystem.Get<DecalSystem>();
			foreach (ValueTuple<uint, Decal> valueTuple in decalSystem.GetDecalsInRange(tile.GridUid, tile.GridIndices + new Vector2(0.5f, 0.5f), 0.75f, (Decal x) => x.Cleanable))
			{
				uint uid = valueTuple.Item1;
				decalSystem.RemoveDecal(tile.GridUid, uid, null);
			}
			return amount;
		}
	}
}
