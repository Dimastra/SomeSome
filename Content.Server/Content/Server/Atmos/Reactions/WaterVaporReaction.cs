using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x02000745 RID: 1861
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class WaterVaporReaction : IGasReactionEffect
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06002704 RID: 9988 RVA: 0x000CD37E File Offset: 0x000CB57E
		[DataField("reagent", false, 1, false, false, null)]
		public string Reagent { get; }

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06002705 RID: 9989 RVA: 0x000CD386 File Offset: 0x000CB586
		[DataField("gas", false, 1, false, false, null)]
		public int GasId { get; }

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06002706 RID: 9990 RVA: 0x000CD38E File Offset: 0x000CB58E
		[DataField("molesPerUnit", false, 1, false, false, null)]
		public float MolesPerUnit { get; } = 1f;

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06002707 RID: 9991 RVA: 0x000CD396 File Offset: 0x000CB596
		[DataField("puddlePrototype", false, 1, false, false, null)]
		public string PuddlePrototype { get; } = "PuddleSmear";

		// Token: 0x06002708 RID: 9992 RVA: 0x000CD3A0 File Offset: 0x000CB5A0
		[NullableContext(1)]
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder, AtmosphereSystem atmosphereSystem)
		{
			if (string.IsNullOrEmpty(this.Reagent) || string.IsNullOrEmpty(this.PuddlePrototype))
			{
				return ReactionResult.NoReaction;
			}
			TileAtmosphere tile = holder as TileAtmosphere;
			if (tile == null)
			{
				return ReactionResult.NoReaction;
			}
			if (mixture.GetMoles(this.GasId) < this.MolesPerUnit)
			{
				return ReactionResult.NoReaction;
			}
			mixture.AdjustMoles(this.GasId, -this.MolesPerUnit);
			TileRef tileRef = tile.GridIndices.GetTileRef(tile.GridIndex, null);
			EntitySystem.Get<SpillableSystem>().SpillAt(tileRef, new Solution(this.Reagent, FixedPoint2.New(this.MolesPerUnit)), this.PuddlePrototype, true, false, false, true);
			return ReactionResult.Reacting;
		}
	}
}
