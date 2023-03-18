using System;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Components;
using Content.Server.Construction;
using Content.Server.Popups;
using Content.Server.Power.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Botany.Systems
{
	// Token: 0x020006FF RID: 1791
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SeedExtractorSystem : EntitySystem
	{
		// Token: 0x06002591 RID: 9617 RVA: 0x000C678C File Offset: 0x000C498C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SeedExtractorComponent, InteractUsingEvent>(new ComponentEventHandler<SeedExtractorComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<SeedExtractorComponent, RefreshPartsEvent>(new ComponentEventHandler<SeedExtractorComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<SeedExtractorComponent, UpgradeExamineEvent>(new ComponentEventHandler<SeedExtractorComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x000C67DC File Offset: 0x000C49DC
		private void OnInteractUsing(EntityUid uid, SeedExtractorComponent seedExtractor, InteractUsingEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			ProduceComponent produce;
			if (!base.TryComp<ProduceComponent>(args.Used, ref produce))
			{
				return;
			}
			SeedData seed;
			if (!this._botanySystem.TryGetSeed(produce, out seed) || seed.Seedless)
			{
				this._popupSystem.PopupCursor(Loc.GetString("seed-extractor-component-no-seeds", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", args.Used)
				}), args.User, PopupType.MediumCaution);
				return;
			}
			this._popupSystem.PopupCursor(Loc.GetString("seed-extractor-component-interact-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", args.Used)
			}), args.User, PopupType.Medium);
			base.QueueDel(args.Used);
			float amount = (float)((int)this._random.NextFloat((float)seedExtractor.BaseMinSeeds, (float)(seedExtractor.BaseMaxSeeds + 1))) * seedExtractor.SeedAmountMultiplier;
			EntityCoordinates coords = base.Transform(uid).Coordinates;
			if (amount > 1f)
			{
				seed.Unique = false;
			}
			int i = 0;
			while ((float)i < amount)
			{
				this._botanySystem.SpawnSeedPacket(seed, coords);
				i++;
			}
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x000C690C File Offset: 0x000C4B0C
		private void OnRefreshParts(EntityUid uid, SeedExtractorComponent seedExtractor, RefreshPartsEvent args)
		{
			float manipulatorQuality = args.PartRatings[seedExtractor.MachinePartSeedAmount];
			seedExtractor.SeedAmountMultiplier = MathF.Pow(seedExtractor.PartRatingSeedAmountMultiplier, manipulatorQuality - 1f);
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x000C6943 File Offset: 0x000C4B43
		private void OnUpgradeExamine(EntityUid uid, SeedExtractorComponent seedExtractor, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("seed-extractor-component-upgrade-seed-yield", seedExtractor.SeedAmountMultiplier);
		}

		// Token: 0x0400172A RID: 5930
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400172B RID: 5931
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400172C RID: 5932
		[Dependency]
		private readonly BotanySystem _botanySystem;
	}
}
