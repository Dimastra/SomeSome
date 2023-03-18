using System;
using System.Runtime.CompilerServices;
using Content.Server.Mining.Components;
using Content.Shared.Destructible;
using Content.Shared.Mining;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Mining
{
	// Token: 0x0200039F RID: 927
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MiningSystem : EntitySystem
	{
		// Token: 0x060012E6 RID: 4838 RVA: 0x00061D66 File Offset: 0x0005FF66
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<OreVeinComponent, MapInitEvent>(new ComponentEventHandler<OreVeinComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<OreVeinComponent, DestructionEventArgs>(new ComponentEventHandler<OreVeinComponent, DestructionEventArgs>(this.OnDestruction), null, null);
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x00061D98 File Offset: 0x0005FF98
		private void OnDestruction(EntityUid uid, OreVeinComponent component, DestructionEventArgs args)
		{
			if (component.CurrentOre == null)
			{
				return;
			}
			OrePrototype proto = this._proto.Index<OrePrototype>(component.CurrentOre);
			if (proto.OreEntity == null)
			{
				return;
			}
			EntityCoordinates coords = base.Transform(uid).Coordinates;
			int toSpawn = this._random.Next(proto.MinOreYield, proto.MaxOreYield);
			for (int i = 0; i < toSpawn; i++)
			{
				base.Spawn(proto.OreEntity, coords.Offset(this._random.NextVector2(0.2f)));
			}
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x00061E20 File Offset: 0x00060020
		private void OnMapInit(EntityUid uid, OreVeinComponent component, MapInitEvent args)
		{
			if (component.CurrentOre != null || component.OreRarityPrototypeId == null || !RandomExtensions.Prob(this._random, component.OreChance))
			{
				return;
			}
			component.CurrentOre = this._proto.Index<WeightedRandomPrototype>(component.OreRarityPrototypeId).Pick(this._random);
		}

		// Token: 0x04000B94 RID: 2964
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04000B95 RID: 2965
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
