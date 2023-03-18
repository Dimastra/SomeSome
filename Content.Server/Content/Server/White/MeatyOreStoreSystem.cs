using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Roles;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Server.White.Sponsors;
using Content.Shared.CCVar;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Verbs;
using Content.Shared.White.MeatyOre;
using Content.Shared.White.Sponsors;
using Robust.Server.GameObjects;
using Robust.Server.GameStates;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Random;

namespace Content.Server.White
{
	// Token: 0x02000082 RID: 130
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeatyOreStoreSystem : EntitySystem
	{
		// Token: 0x060001DD RID: 477 RVA: 0x0000A6D8 File Offset: 0x000088D8
		public override void Initialize()
		{
			base.Initialize();
			this._configurationManager.OnValueChanged<bool>(CCVars.MeatyOrePanelEnabled, new Action<bool>(this.OnPanelEnableChanged), true);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnPostRoundCleanup), null, null);
			base.SubscribeNetworkEvent<MeatyOreShopRequestEvent>(new EntitySessionEventHandler<MeatyOreShopRequestEvent>(this.OnShopRequested), null, null);
			base.SubscribeLocalEvent<MindComponent, MeatyTraitorRequestActionEvent>(new ComponentEventHandler<MindComponent, MeatyTraitorRequestActionEvent>(this.OnAntagPurchase), null, null);
			base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.MeatyOreVerbs), null, null);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000A758 File Offset: 0x00008958
		private void MeatyOreVerbs(GetVerbsEvent<Verb> ev)
		{
			if (ev.User == ev.Target)
			{
				return;
			}
			ActorComponent actorComponent;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(ev.User, ref actorComponent))
			{
				return;
			}
			SponsorInfo sponsorInfo;
			if (!this._sponsorsManager.TryGetInfo(actorComponent.PlayerSession.UserId, out sponsorInfo))
			{
				return;
			}
			if (!base.HasComp<HumanoidAppearanceComponent>(ev.Target))
			{
				return;
			}
			MobStateComponent state;
			if (!base.TryComp<MobStateComponent>(ev.Target, ref state) || (state == null || state.CurrentState != MobState.Alive))
			{
				return;
			}
			StoreComponent store;
			if (!this.TryGetStore(actorComponent.PlayerSession, out store))
			{
				return;
			}
			MindComponent targetMind;
			if (!base.TryComp<MindComponent>(ev.Target, ref targetMind) || !targetMind.HasMind)
			{
				return;
			}
			if (targetMind.Mind.AllRoles.Any((Role x) => x.Antagonist))
			{
				return;
			}
			Job currentJob = targetMind.Mind.CurrentJob;
			if (currentJob == null || !currentJob.CanBeAntag)
			{
				return;
			}
			if (targetMind.Mind.Session == null)
			{
				return;
			}
			FixedPoint2 currency;
			if (!store.Balance.TryGetValue("MeatyOreCoin", out currency))
			{
				return;
			}
			if (currency - 10 < 0)
			{
				return;
			}
			Verb verb = new Verb
			{
				Text = "Выдать роль.",
				ConfirmationPopup = true,
				Message = "Цена - " + MeatyOreStoreSystem.MeatyOreCurrensyPrototype + ":10",
				Act = delegate()
				{
					this._storeSystem.TryAddCurrency(new Dictionary<string, FixedPoint2>
					{
						{
							MeatyOreStoreSystem.MeatyOreCurrensyPrototype,
							-10
						}
					}, store.Owner, store);
					this._traitorRuleSystem.MakeTraitor(targetMind.Mind.Session);
				},
				Category = VerbCategory.MeatyOre
			};
			ev.Verbs.Add(verb);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000A91C File Offset: 0x00008B1C
		private void OnPanelEnableChanged(bool newValue)
		{
			if (!newValue)
			{
				foreach (KeyValuePair<NetUserId, StoreComponent> meatyOreStoreData in this._meatyOreStores)
				{
					IPlayerSession session = this._playerManager.GetSessionByUserId(meatyOreStoreData.Key);
					if (session != null)
					{
						EntityUid? playerEntity = session.AttachedEntity;
						if (playerEntity != null)
						{
							this._storeSystem.CloseUi(playerEntity.Value, meatyOreStoreData.Value);
						}
					}
				}
			}
			MeatyOreStoreSystem.MeatyOrePanelEnabled = newValue;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A9B0 File Offset: 0x00008BB0
		private void OnAntagPurchase(EntityUid uid, MindComponent component, MeatyTraitorRequestActionEvent args)
		{
			if (component.Mind == null)
			{
				return;
			}
			if (component.Mind.Session == null)
			{
				return;
			}
			TraitorRuleSystem traitorRuleSystem = this._traitorRuleSystem;
			Mind mind = component.Mind;
			traitorRuleSystem.MakeTraitor((mind != null) ? mind.Session : null);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A9E8 File Offset: 0x00008BE8
		private void OnShopRequested(MeatyOreShopRequestEvent msg, EntitySessionEventArgs args)
		{
			IPlayerSession playerSession = args.SenderSession as IPlayerSession;
			if (!MeatyOreStoreSystem.MeatyOrePanelEnabled)
			{
				this._chatManager.DispatchServerMessage(playerSession, "Мясная панель отключена на данном сервере! Приятной игры!", false);
				return;
			}
			EntityUid? playerEntity = args.SenderSession.AttachedEntity;
			if (playerEntity == null)
			{
				return;
			}
			if (!base.HasComp<HumanoidAppearanceComponent>(playerEntity.Value))
			{
				return;
			}
			StoreComponent storeComponent;
			if (!this.TryGetStore(playerSession, out storeComponent))
			{
				return;
			}
			this._storeSystem.ToggleUi(playerEntity.Value, storeComponent.Owner, storeComponent);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000AA68 File Offset: 0x00008C68
		private bool TryGetStore(IPlayerSession session, out StoreComponent store)
		{
			store = null;
			SponsorInfo sponsorInfo;
			if (!this._sponsorsManager.TryGetInfo(session.UserId, out sponsorInfo))
			{
				return false;
			}
			if (this._meatyOreStores.TryGetValue(session.UserId, out store))
			{
				return true;
			}
			if (sponsorInfo.MeatyOreCoin == 0)
			{
				return false;
			}
			store = this.CreateStore(session.UserId, sponsorInfo.MeatyOreCoin);
			return true;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000AAC4 File Offset: 0x00008CC4
		private void OnPostRoundCleanup(RoundRestartCleanupEvent ev)
		{
			foreach (StoreComponent store in this._meatyOreStores.Values)
			{
				base.Del(store.Owner);
			}
			this._meatyOreStores.Clear();
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000AB2C File Offset: 0x00008D2C
		private StoreComponent CreateStore(NetUserId userId, int balance)
		{
			IPlayerSession session = this._playerManager.GetSessionByUserId(userId);
			EntityUid shopEntity = this._entityManager.SpawnEntity("StoreMeatyOreEntity", MapCoordinates.Nullspace);
			StoreComponent storeComponent = base.Comp<StoreComponent>(shopEntity);
			this._storeSystem.InitializeFromPreset("StorePresetMeatyOre", shopEntity, storeComponent);
			storeComponent.Balance.Clear();
			this._storeSystem.TryAddCurrency(new Dictionary<string, FixedPoint2>
			{
				{
					MeatyOreStoreSystem.MeatyOreCurrensyPrototype,
					balance
				}
			}, storeComponent.Owner, storeComponent);
			this._meatyOreStores[userId] = storeComponent;
			this._pvsOverrideSystem.AddSessionOverride(shopEntity, session);
			return storeComponent;
		}

		// Token: 0x04000155 RID: 341
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000156 RID: 342
		[Dependency]
		private readonly StoreSystem _storeSystem;

		// Token: 0x04000157 RID: 343
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04000158 RID: 344
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000159 RID: 345
		[Dependency]
		private readonly TraitorRuleSystem _traitorRuleSystem;

		// Token: 0x0400015A RID: 346
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400015B RID: 347
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x0400015C RID: 348
		[Dependency]
		private readonly SponsorsManager _sponsorsManager;

		// Token: 0x0400015D RID: 349
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400015E RID: 350
		[Dependency]
		private readonly PVSOverrideSystem _pvsOverrideSystem;

		// Token: 0x0400015F RID: 351
		private static readonly string StorePresetPrototype = "StorePresetMeatyOre";

		// Token: 0x04000160 RID: 352
		private static readonly string MeatyOreCurrensyPrototype = "MeatyOreCoin";

		// Token: 0x04000161 RID: 353
		private static bool MeatyOrePanelEnabled;

		// Token: 0x04000162 RID: 354
		private readonly Dictionary<NetUserId, StoreComponent> _meatyOreStores = new Dictionary<NetUserId, StoreComponent>();
	}
}
