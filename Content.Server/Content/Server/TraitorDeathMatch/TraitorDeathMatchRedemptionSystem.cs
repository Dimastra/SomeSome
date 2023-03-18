using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Server.Traitor.Uplink;
using Content.Server.TraitorDeathMatch.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.TraitorDeathMatch
{
	// Token: 0x02000109 RID: 265
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitorDeathMatchRedemptionSystem : EntitySystem
	{
		// Token: 0x060004C5 RID: 1221 RVA: 0x00016BCC File Offset: 0x00014DCC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TraitorDeathMatchRedemptionComponent, InteractUsingEvent>(new ComponentEventHandler<TraitorDeathMatchRedemptionComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00016BE8 File Offset: 0x00014DE8
		private void OnInteractUsing(EntityUid uid, TraitorDeathMatchRedemptionComponent component, InteractUsingEvent args)
		{
			MindComponent userMindComponent;
			if (!this.EntityManager.TryGetComponent<MindComponent>(args.User, ref userMindComponent))
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-no-mind-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			Mind userMind = userMindComponent.Mind;
			if (userMind == null)
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-no-user-mind-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			StoreComponent victimUplink;
			if (!this.EntityManager.TryGetComponent<StoreComponent>(args.Used, ref victimUplink))
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-no-pda-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			TraitorDeathMatchReliableOwnerTagComponent victimPDAuid;
			if (!this.EntityManager.TryGetComponent<TraitorDeathMatchReliableOwnerTagComponent>(args.Used, ref victimPDAuid))
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-no-pda-owner-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			if (victimPDAuid.UserId == userMind.UserId)
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-pda-different-user-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			StoreComponent userUplink = null;
			EntityUid? pdaUid;
			StoreComponent userUplinkComponent;
			if (this._inventory.TryGetSlotEntity(args.User, "id", out pdaUid, null, null) && this.EntityManager.TryGetComponent<StoreComponent>(pdaUid, ref userUplinkComponent))
			{
				userUplink = userUplinkComponent;
			}
			if (userUplink == null)
			{
				this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-main-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("secondMessage", Loc.GetString("traitor-death-match-redemption-component-interact-using-no-pda-in-pocket-message"))
				}), uid, args.User, PopupType.Small);
				return;
			}
			int transferAmount = this._uplink.GetTCBalance(victimUplink) + 4;
			victimUplink.Balance.Clear();
			this._store.TryAddCurrency(new Dictionary<string, FixedPoint2>
			{
				{
					"Telecrystal",
					FixedPoint2.New(transferAmount)
				}
			}, userUplink.Owner, userUplink);
			this.EntityManager.DeleteEntity(victimUplink.Owner);
			this._popup.PopupEntity(Loc.GetString("traitor-death-match-redemption-component-interact-using-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("tcAmount", transferAmount)
			}), uid, args.User, PopupType.Small);
			args.Handled = true;
		}

		// Token: 0x040002C6 RID: 710
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x040002C7 RID: 711
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040002C8 RID: 712
		[Dependency]
		private readonly UplinkSystem _uplink;

		// Token: 0x040002C9 RID: 713
		[Dependency]
		private readonly StoreSystem _store;
	}
}
