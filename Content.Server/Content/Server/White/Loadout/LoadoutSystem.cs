using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Hands.Systems;
using Content.Server.Storage.EntitySystems;
using Content.Server.White.Sponsors;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.White.Sponsors;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.White.Loadout
{
	// Token: 0x02000098 RID: 152
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LoadoutSystem : EntitySystem
	{
		// Token: 0x0600025D RID: 605 RVA: 0x0000CC2A File Offset: 0x0000AE2A
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawned), null, null);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000CC40 File Offset: 0x0000AE40
		private void OnPlayerSpawned(PlayerSpawnCompleteEvent ev)
		{
			SponsorInfo sponsor;
			if (this._sponsorsManager.TryGetInfo(ev.Player.UserId, out sponsor))
			{
				foreach (string loadoutId in sponsor.AllowedMarkings)
				{
					LoadoutItemPrototype loadout;
					if (this._prototypeManager.TryIndex<LoadoutItemPrototype>(loadoutId, ref loadout))
					{
						bool flag = loadout.SponsorOnly && !sponsor.AllowedMarkings.Contains(loadoutId);
						bool isWhitelisted = ev.JobId != null && loadout.WhitelistJobs != null && !loadout.WhitelistJobs.Contains(ev.JobId);
						bool isBlacklisted = ev.JobId != null && loadout.BlacklistJobs != null && loadout.BlacklistJobs.Contains(ev.JobId);
						bool isSpeciesRestricted = loadout.SpeciesRestrictions != null && loadout.SpeciesRestrictions.Contains(ev.Profile.Species);
						if (!flag && !isWhitelisted && !isBlacklisted && !isSpeciesRestricted)
						{
							EntityUid entity = base.Spawn(loadout.EntityId, base.Transform(ev.Mob).Coordinates);
							ClothingComponent clothing;
							if (!base.TryComp<ClothingComponent>(entity, ref clothing))
							{
								this._handsSystem.TryPickup(ev.Mob, entity, null, true, false, null, null);
							}
							else
							{
								string firstSlotName = null;
								bool isEquiped = false;
								foreach (SlotDefinition slot in this._inventorySystem.GetSlots(ev.Mob, null))
								{
									if (clothing.Slots.HasFlag(slot.SlotFlags))
									{
										if (firstSlotName == null)
										{
											firstSlotName = slot.Name;
										}
										EntityUid? entityUid;
										if (!this._inventorySystem.TryGetSlotEntity(ev.Mob, slot.Name, out entityUid, null, null) && this._inventorySystem.TryEquip(ev.Mob, entity, slot.Name, true, false, false, null, null))
										{
											isEquiped = true;
											break;
										}
									}
								}
								if (!isEquiped && firstSlotName != null)
								{
									EntityUid? slotEntity;
									EntityUid? backEntity;
									string text;
									if (this._inventorySystem.TryGetSlotEntity(ev.Mob, firstSlotName, out slotEntity, null, null) && this._inventorySystem.TryGetSlotEntity(ev.Mob, "back", out backEntity, null, null) && this._storageSystem.CanInsert(backEntity.Value, slotEntity.Value, out text, null))
									{
										this._storageSystem.Insert(backEntity.Value, slotEntity.Value, null, true);
									}
									this._inventorySystem.TryEquip(ev.Mob, entity, firstSlotName, true, false, false, null, null);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x040001B9 RID: 441
		private const string BackpackSlotId = "back";

		// Token: 0x040001BA RID: 442
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040001BB RID: 443
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040001BC RID: 444
		[Dependency]
		private readonly HandsSystem _handsSystem;

		// Token: 0x040001BD RID: 445
		[Dependency]
		private readonly StorageSystem _storageSystem;

		// Token: 0x040001BE RID: 446
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;
	}
}
