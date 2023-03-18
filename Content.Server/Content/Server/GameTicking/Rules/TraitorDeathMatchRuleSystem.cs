using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Managers;
using Content.Server.Hands.Components;
using Content.Server.Mind;
using Content.Server.PDA;
using Content.Server.Players;
using Content.Server.Spawners.Components;
using Content.Server.Store.Components;
using Content.Server.Traitor;
using Content.Server.Traitor.Uplink;
using Content.Server.TraitorDeathMatch.Components;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004C2 RID: 1218
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitorDeathMatchRuleSystem : GameRuleSystem
	{
		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001926 RID: 6438 RVA: 0x0008409E File Offset: 0x0008229E
		public override string Prototype
		{
			get
			{
				return "TraitorDeathMatch";
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001927 RID: 6439 RVA: 0x000840A5 File Offset: 0x000822A5
		public string PDAPrototypeName
		{
			get
			{
				return "CaptainPDA";
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001928 RID: 6440 RVA: 0x000840AC File Offset: 0x000822AC
		public string BeltPrototypeName
		{
			get
			{
				return "ClothingBeltJanitorFilled";
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x000840B3 File Offset: 0x000822B3
		public string BackpackPrototypeName
		{
			get
			{
				return "ClothingBackpackFilled";
			}
		}

		// Token: 0x0600192A RID: 6442 RVA: 0x000840BC File Offset: 0x000822BC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndText), null, null);
			base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawned), null, null);
			base.SubscribeLocalEvent<GhostAttemptHandleEvent>(new EntityEventHandler<GhostAttemptHandleEvent>(this.OnGhostAttempt), null, null);
		}

		// Token: 0x0600192B RID: 6443 RVA: 0x0008410C File Offset: 0x0008230C
		private void OnPlayerSpawned(PlayerSpawnCompleteEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			IPlayerSession player = ev.Player;
			int startingBalance = this._cfg.GetCVar<int>(CCVars.TraitorDeathMatchStartingBalance);
			PlayerData playerData = player.Data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				Logger.ErrorS("preset", "Failed getting mind for TDM player.");
				return;
			}
			AntagPrototype antagPrototype = this._prototypeManager.Index<AntagPrototype>("Traitor");
			TraitorRole traitorRole = new TraitorRole(mind, antagPrototype);
			mind.AddRole(traitorRole);
			EntityUid? ownedEntity = mind.OwnedEntity;
			if (ownedEntity != null)
			{
				EntityUid owned = ownedEntity.GetValueOrDefault();
				if (owned.Valid)
				{
					foreach (string slot in new string[]
					{
						"id",
						"belt",
						"back"
					})
					{
						EntityUid? entityUid;
						if (this._inventory.TryUnequip(owned, slot, out entityUid, true, true, false, null, null))
						{
							base.Del(entityUid.Value);
						}
					}
					EntityCoordinates ownedCoords = base.Transform(owned).Coordinates;
					EntityUid newPDA = base.Spawn(this.PDAPrototypeName, ownedCoords);
					this._inventory.TryEquip(owned, newPDA, "id", true, false, false, null, null);
					EntityUid newTmp = base.Spawn(this.BeltPrototypeName, ownedCoords);
					this._inventory.TryEquip(owned, newTmp, "belt", true, false, false, null, null);
					newTmp = base.Spawn(this.BackpackPrototypeName, ownedCoords);
					this._inventory.TryEquip(owned, newTmp, "back", true, false, false, null, null);
					if (!this._uplink.AddUplink(owned, new FixedPoint2?(startingBalance), "StorePresetUplink", null))
					{
						return;
					}
					this._allOriginalNames[owned] = base.Name(owned, null);
					PDAComponent pda = base.Comp<PDAComponent>(newPDA);
					this.EntityManager.EntitySysManager.GetEntitySystem<PDASystem>().SetOwner(pda, base.Name(owned, null));
					this.EntityManager.AddComponent<TraitorDeathMatchReliableOwnerTagComponent>(newPDA).UserId = mind.UserId;
				}
			}
			EntityCoordinates bestTarget;
			if (mind.OwnedEntity != null && this.FindAnyIsolatedSpawnLocation(mind, out bestTarget))
			{
				base.Transform(mind.OwnedEntity.Value).Coordinates = bestTarget;
				return;
			}
			if (this._safeToEndRound)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("traitor-death-match-station-is-too-unsafe-announcement"), null);
				this._restarter.RoundMaxTime = TimeSpan.FromMinutes(1.0);
				this._restarter.RestartTimer();
				this._safeToEndRound = false;
			}
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x000843A8 File Offset: 0x000825A8
		private void OnGhostAttempt(GhostAttemptHandleEvent ev)
		{
			if (!base.RuleAdded || ev.Handled)
			{
				return;
			}
			ev.Handled = true;
			Mind mind = ev.Mind;
			EntityUid? ownedEntity = mind.OwnedEntity;
			if (ownedEntity != null)
			{
				EntityUid entity = ownedEntity.GetValueOrDefault();
				MobStateComponent mobState;
				if (entity.Valid && base.TryComp<MobStateComponent>(entity, ref mobState))
				{
					if (this._mobStateSystem.IsCritical(entity, mobState))
					{
						DamageSpecifier damage = new DamageSpecifier(this._prototypeManager.Index<DamageTypePrototype>("Asphyxiation"), 100);
						EntitySystem.Get<DamageableSystem>().TryChangeDamage(new EntityUid?(entity), damage, true, true, null, null);
					}
					else if (!this._mobStateSystem.IsDead(entity, mobState) && base.HasComp<HandsComponent>(entity))
					{
						ev.Result = false;
						return;
					}
				}
			}
			IPlayerSession session = mind.Session;
			if (session == null)
			{
				ev.Result = false;
				return;
			}
			this.GameTicker.Respawn(session);
			ev.Result = true;
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x00084498 File Offset: 0x00082698
		private void OnRoundEndText(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			List<string> lines = new List<string>();
			lines.Add(Loc.GetString("traitor-death-match-end-round-description-first-line"));
			foreach (StoreComponent uplink in this.EntityManager.EntityQuery<StoreComponent>(true))
			{
				EntityUid? owner = uplink.AccountOwner;
				if (owner != null && this._allOriginalNames.ContainsKey(owner.Value))
				{
					int tcbalance = this._uplink.GetTCBalance(uplink);
					lines.Add(Loc.GetString("traitor-death-match-end-round-description-entry", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("originalName", this._allOriginalNames[owner.Value]),
						new ValueTuple<string, object>("tcBalance", tcbalance)
					}));
				}
			}
			ev.AddLine(string.Join<string>('\n', lines));
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0008459C File Offset: 0x0008279C
		public override void Started()
		{
			this._restarter.RoundMaxTime = TimeSpan.FromMinutes(30.0);
			this._restarter.RestartTimer();
			this._safeToEndRound = true;
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x000845C9 File Offset: 0x000827C9
		public override void Ended()
		{
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x000845CC File Offset: 0x000827CC
		private bool FindAnyIsolatedSpawnLocation(Mind ignoreMe, out EntityCoordinates bestTarget)
		{
			List<EntityCoordinates> existingPlayerPoints = new List<EntityCoordinates>();
			foreach (IPlayerSession playerSession in this._playerManager.ServerSessions)
			{
				PlayerData playerData = playerSession.Data.ContentData();
				Mind avoidMeMind = (playerData != null) ? playerData.Mind : null;
				if (avoidMeMind != null && avoidMeMind != ignoreMe)
				{
					EntityUid? avoidMeEntity = avoidMeMind.OwnedEntity;
					MobStateComponent mobState;
					if (avoidMeEntity != null && base.TryComp<MobStateComponent>(avoidMeEntity.Value, ref mobState) && !this._mobStateSystem.IsCritical(avoidMeEntity.Value, mobState) && !this._mobStateSystem.IsDead(avoidMeEntity.Value, mobState))
					{
						existingPlayerPoints.Add(base.Transform(avoidMeEntity.Value).Coordinates);
					}
				}
			}
			float bestTargetDistanceFromNearest = -1f;
			List<EntityUid> ents = (from x in this.EntityManager.EntityQuery<SpawnPointComponent>(false)
			select x.Owner).ToList<EntityUid>();
			this._robustRandom.Shuffle<EntityUid>(ents);
			bool foundATarget = false;
			bestTarget = EntityCoordinates.Invalid;
			foreach (EntityUid entity in ents)
			{
				TransformComponent transform = base.Transform(entity);
				if (transform.GridUid != null && transform.MapUid != null)
				{
					Vector2i position = this._transformSystem.GetGridOrMapTilePosition(entity, transform);
					if (this._atmosphereSystem.IsTileMixtureProbablySafe(new EntityUid?(transform.GridUid.Value), transform.MapUid.Value, position))
					{
						float distanceFromNearest = float.PositiveInfinity;
						foreach (EntityCoordinates existing in existingPlayerPoints)
						{
							float dist;
							if (base.Transform(entity).Coordinates.TryDistance(this.EntityManager, existing, ref dist))
							{
								distanceFromNearest = Math.Min(distanceFromNearest, dist);
							}
						}
						if (bestTargetDistanceFromNearest < distanceFromNearest)
						{
							bestTarget = base.Transform(entity).Coordinates;
							bestTargetDistanceFromNearest = distanceFromNearest;
							foundATarget = true;
						}
					}
				}
			}
			return foundATarget;
		}

		// Token: 0x04000FB6 RID: 4022
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000FB7 RID: 4023
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000FB8 RID: 4024
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000FB9 RID: 4025
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000FBA RID: 4026
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000FBB RID: 4027
		[Dependency]
		private readonly MaxTimeRestartRuleSystem _restarter;

		// Token: 0x04000FBC RID: 4028
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x04000FBD RID: 4029
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000FBE RID: 4030
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04000FBF RID: 4031
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04000FC0 RID: 4032
		[Dependency]
		private readonly UplinkSystem _uplink;

		// Token: 0x04000FC1 RID: 4033
		private bool _safeToEndRound;

		// Token: 0x04000FC2 RID: 4034
		private readonly Dictionary<EntityUid, string> _allOriginalNames = new Dictionary<EntityUid, string>();

		// Token: 0x04000FC3 RID: 4035
		private const string TraitorPrototypeID = "Traitor";
	}
}
