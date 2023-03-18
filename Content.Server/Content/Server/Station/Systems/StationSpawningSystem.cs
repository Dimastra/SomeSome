using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.DetailExaminable;
using Content.Server.Hands.Components;
using Content.Server.Hands.Systems;
using Content.Server.Humanoid;
using Content.Server.IdentityManagement;
using Content.Server.Mind.Commands;
using Content.Server.PDA;
using Content.Server.Roles;
using Content.Server.Station.Components;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Station.Systems
{
	// Token: 0x02000199 RID: 409
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationSpawningSystem : EntitySystem
	{
		// Token: 0x0600082A RID: 2090 RVA: 0x000296C4 File Offset: 0x000278C4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StationInitializedEvent>(new EntityEventHandler<StationInitializedEvent>(this.OnStationInitialized), null, null);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x000296DA File Offset: 0x000278DA
		private void OnStationInitialized(StationInitializedEvent ev)
		{
			base.AddComp<StationSpawningComponent>(ev.Station);
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x000296EC File Offset: 0x000278EC
		[NullableContext(2)]
		public EntityUid? SpawnPlayerCharacterOnStation(EntityUid? station, Job job, HumanoidCharacterProfile profile, StationSpawningComponent stationSpawning = null)
		{
			if (station != null && !base.Resolve<StationSpawningComponent>(station.Value, ref stationSpawning, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			PlayerSpawningEvent ev = new PlayerSpawningEvent(job, profile, station);
			base.RaiseLocalEvent<PlayerSpawningEvent>(ev);
			return ev.SpawnResult;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0002973C File Offset: 0x0002793C
		[NullableContext(2)]
		public EntityUid SpawnPlayerMob(EntityCoordinates coordinates, Job job, HumanoidCharacterProfile profile, EntityUid? station)
		{
			if (job != null && job.JobEntity != null)
			{
				EntityUid jobEntity = this.EntityManager.SpawnEntity(job.JobEntity, coordinates);
				MakeSentientCommand.MakeSentient(jobEntity, this.EntityManager, true, true);
				this.DoJobSpecials(job, jobEntity);
				this._identity.QueueIdentityUpdate(jobEntity);
				return jobEntity;
			}
			EntityUid entity = this.EntityManager.SpawnEntity(this._prototypeManager.Index<SpeciesPrototype>(((profile != null) ? profile.Species : null) ?? "Human").Prototype, coordinates);
			if (job != null && job.StartingGear != null)
			{
				StartingGearPrototype startingGear = this._prototypeManager.Index<StartingGearPrototype>(job.StartingGear);
				this.EquipStartingGear(entity, startingGear, profile);
				if (profile != null)
				{
					this.EquipIdCard(entity, profile.Name, job.Prototype, station);
				}
			}
			if (profile != null)
			{
				this._humanoidSystem.LoadProfile(entity, profile, null);
				this.EntityManager.GetComponent<MetaDataComponent>(entity).EntityName = profile.Name;
				if (profile.FlavorText != "" && this._configurationManager.GetCVar<bool>(CCVars.FlavorText))
				{
					this.EntityManager.AddComponent<DetailExaminableComponent>(entity).Content = profile.FlavorText;
				}
			}
			this.DoJobSpecials(job, entity);
			this._identity.QueueIdentityUpdate(entity);
			return entity;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00029874 File Offset: 0x00027A74
		[NullableContext(2)]
		private void DoJobSpecials(Job job, EntityUid entity)
		{
			JobSpecial[] array = ((job != null) ? job.Prototype.Special : null) ?? Array.Empty<JobSpecial>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AfterEquip(entity);
			}
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x000298B4 File Offset: 0x00027AB4
		public void EquipStartingGear(EntityUid entity, StartingGearPrototype startingGear, [Nullable(2)] HumanoidCharacterProfile profile)
		{
			SlotDefinition[] slotDefinitions;
			if (this._inventorySystem.TryGetSlots(entity, out slotDefinitions, null))
			{
				foreach (SlotDefinition slot in slotDefinitions)
				{
					string equipmentStr = startingGear.GetGear(slot.Name, profile);
					if (!string.IsNullOrEmpty(equipmentStr))
					{
						EntityUid equipmentEntity = this.EntityManager.SpawnEntity(equipmentStr, this.EntityManager.GetComponent<TransformComponent>(entity).Coordinates);
						this._inventorySystem.TryEquip(entity, equipmentEntity, slot.Name, true, false, false, null, null);
					}
				}
			}
			HandsComponent handsComponent;
			if (!base.TryComp<HandsComponent>(entity, ref handsComponent))
			{
				return;
			}
			IEnumerable<KeyValuePair<string, string>> inhand = startingGear.Inhand;
			EntityCoordinates coords = this.EntityManager.GetComponent<TransformComponent>(entity).Coordinates;
			foreach (KeyValuePair<string, string> keyValuePair in inhand)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string hand = text;
				string prototype = text2;
				EntityUid inhandEntity = this.EntityManager.SpawnEntity(prototype, coords);
				this._handsSystem.TryPickup(entity, inhandEntity, hand, false, false, handsComponent, null);
			}
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x000299D8 File Offset: 0x00027BD8
		public void EquipIdCard(EntityUid entity, string characterName, JobPrototype jobPrototype, EntityUid? station)
		{
			EntityUid? idUid;
			if (!this._inventorySystem.TryGetSlotEntity(entity, "id", out idUid, null, null))
			{
				return;
			}
			PDAComponent pdaComponent;
			if (!this.EntityManager.TryGetComponent<PDAComponent>(idUid, ref pdaComponent) || pdaComponent.ContainedID == null)
			{
				return;
			}
			IdCardComponent card = pdaComponent.ContainedID;
			EntityUid cardId = card.Owner;
			this._cardSystem.TryChangeFullName(cardId, characterName, card, null);
			this._cardSystem.TryChangeJobTitle(cardId, jobPrototype.LocalizedName, card, null);
			bool extendedAccess = false;
			if (station != null)
			{
				extendedAccess = base.Comp<StationJobsComponent>(station.Value).ExtendedAccess;
			}
			this._accessSystem.SetAccessToJob(cardId, jobPrototype, extendedAccess, null);
			this._pdaSystem.SetOwner(pdaComponent, characterName);
		}

		// Token: 0x040004F5 RID: 1269
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040004F6 RID: 1270
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040004F7 RID: 1271
		[Dependency]
		private readonly HandsSystem _handsSystem;

		// Token: 0x040004F8 RID: 1272
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoidSystem;

		// Token: 0x040004F9 RID: 1273
		[Dependency]
		private readonly IdCardSystem _cardSystem;

		// Token: 0x040004FA RID: 1274
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040004FB RID: 1275
		[Dependency]
		private readonly PDASystem _pdaSystem;

		// Token: 0x040004FC RID: 1276
		[Dependency]
		private readonly SharedAccessSystem _accessSystem;

		// Token: 0x040004FD RID: 1277
		[Dependency]
		private readonly IdentitySystem _identity;
	}
}
