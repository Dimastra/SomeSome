using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Storage.EntitySystems;
using Content.Server.Store.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Traitor.Uplink.SurplusBundle
{
	// Token: 0x0200010F RID: 271
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SurplusBundleSystem : EntitySystem
	{
		// Token: 0x060004D6 RID: 1238 RVA: 0x0001718F File Offset: 0x0001538F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SurplusBundleComponent, MapInitEvent>(new ComponentEventHandler<SurplusBundleComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<SurplusBundleComponent, ComponentInit>(new ComponentEventHandler<SurplusBundleComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x000171C0 File Offset: 0x000153C0
		private void OnInit(EntityUid uid, SurplusBundleComponent component, ComponentInit args)
		{
			StorePresetPrototype storePreset = this._prototypeManager.Index<StorePresetPrototype>(component.StorePreset);
			this._listings = this._store.GetAvailableListings(uid, null, storePreset.Categories, null).ToArray<ListingData>();
			Array.Sort<ListingData>(this._listings, (ListingData a, ListingData b) => (int)(b.Cost.Values.Sum() - a.Cost.Values.Sum()));
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00017230 File Offset: 0x00015430
		private void OnMapInit(EntityUid uid, SurplusBundleComponent component, MapInitEvent args)
		{
			this.FillStorage(uid, component);
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001723C File Offset: 0x0001543C
		[NullableContext(2)]
		private void FillStorage(EntityUid uid, SurplusBundleComponent component = null)
		{
			if (!base.Resolve<SurplusBundleComponent>(uid, ref component, true))
			{
				return;
			}
			EntityCoordinates cords = base.Transform(uid).Coordinates;
			foreach (ListingData item in this.GetRandomContent(component.TotalPrice))
			{
				EntityUid ent = this.EntityManager.SpawnEntity(item.ProductEntity, cords);
				this._entityStorage.Insert(ent, component.Owner, null);
			}
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x000172D4 File Offset: 0x000154D4
		private List<ListingData> GetRandomContent(FixedPoint2 targetCost)
		{
			List<ListingData> ret = new List<ListingData>();
			if (this._listings.Length == 0)
			{
				return ret;
			}
			FixedPoint2 totalCost = FixedPoint2.Zero;
			int index = 0;
			while (totalCost < targetCost)
			{
				FixedPoint2 remainingBudget = targetCost - totalCost;
				while (this._listings[index].Cost.Values.Sum() > remainingBudget)
				{
					index++;
					if (index >= this._listings.Length)
					{
						return ret;
					}
				}
				int randomIndex = this._random.Next(index, this._listings.Length);
				ListingData randomItem = this._listings[randomIndex];
				ret.Add(randomItem);
				totalCost += randomItem.Cost.Values.Sum();
			}
			return ret;
		}

		// Token: 0x040002D4 RID: 724
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040002D5 RID: 725
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040002D6 RID: 726
		[Dependency]
		private readonly EntityStorageSystem _entityStorage;

		// Token: 0x040002D7 RID: 727
		[Dependency]
		private readonly StoreSystem _store;

		// Token: 0x040002D8 RID: 728
		private ListingData[] _listings;
	}
}
