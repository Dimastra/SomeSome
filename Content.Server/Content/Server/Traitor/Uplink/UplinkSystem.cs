using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Traitor.Uplink
{
	// Token: 0x0200010D RID: 269
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UplinkSystem : EntitySystem
	{
		// Token: 0x060004D1 RID: 1233 RVA: 0x00016FA4 File Offset: 0x000151A4
		public int GetTCBalance(StoreComponent component)
		{
			FixedPoint2? tcBalance = new FixedPoint2?(component.Balance.GetValueOrDefault("Telecrystal"));
			if (tcBalance == null)
			{
				return 0;
			}
			return tcBalance.GetValueOrDefault().Int();
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00016FE4 File Offset: 0x000151E4
		public bool AddUplink(EntityUid user, FixedPoint2? balance, string uplinkPresetId = "StorePresetUplink", EntityUid? uplinkEntity = null)
		{
			if (uplinkEntity == null)
			{
				uplinkEntity = this.FindUplinkTarget(user);
				if (uplinkEntity == null)
				{
					return false;
				}
			}
			StoreComponent store = base.EnsureComp<StoreComponent>(uplinkEntity.Value);
			this._store.InitializeFromPreset(uplinkPresetId, uplinkEntity.Value, store);
			store.AccountOwner = new EntityUid?(user);
			store.Balance.Clear();
			if (balance != null)
			{
				store.Balance.Clear();
				this._store.TryAddCurrency(new Dictionary<string, FixedPoint2>
				{
					{
						"Telecrystal",
						balance.Value
					}
				}, uplinkEntity.Value, store);
			}
			return true;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00017088 File Offset: 0x00015288
		private EntityUid? FindUplinkTarget(EntityUid user)
		{
			InventorySystem.ContainerSlotEnumerator containerSlotEnumerator;
			if (this._inventorySystem.TryGetContainerSlotEnumerator(user, out containerSlotEnumerator, null))
			{
				ContainerSlot pdaUid;
				while (containerSlotEnumerator.MoveNext(out pdaUid))
				{
					EntityUid? containedEntity = pdaUid.ContainedEntity;
					if (containedEntity != null)
					{
						containedEntity = pdaUid.ContainedEntity;
						if (!base.HasComp<PDAComponent>(containedEntity.Value))
						{
							containedEntity = pdaUid.ContainedEntity;
							if (!base.HasComp<StoreComponent>(containedEntity.Value))
							{
								continue;
							}
						}
						containedEntity = pdaUid.ContainedEntity;
						return new EntityUid?(containedEntity.Value);
					}
				}
			}
			foreach (EntityUid item in this._handsSystem.EnumerateHeld(user, null))
			{
				if (base.HasComp<PDAComponent>(item) || base.HasComp<StoreComponent>(item))
				{
					return new EntityUid?(item);
				}
			}
			return null;
		}

		// Token: 0x040002CE RID: 718
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040002CF RID: 719
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040002D0 RID: 720
		[Dependency]
		private readonly StoreSystem _store;

		// Token: 0x040002D1 RID: 721
		public const string TelecrystalCurrencyPrototype = "Telecrystal";
	}
}
