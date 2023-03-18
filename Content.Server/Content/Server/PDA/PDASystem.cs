using System;
using System.Runtime.CompilerServices;
using Content.Server.CartridgeLoader;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Instruments;
using Content.Server.Light.Components;
using Content.Server.Light.EntitySystems;
using Content.Server.Light.Events;
using Content.Server.Mind.Components;
using Content.Server.PDA.Ringer;
using Content.Server.Station.Systems;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Server.Traitor;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.PDA;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.PDA
{
	// Token: 0x020002DE RID: 734
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PDASystem : SharedPDASystem
	{
		// Token: 0x06000EFA RID: 3834 RVA: 0x0004CAC8 File Offset: 0x0004ACC8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PDAComponent, LightToggleEvent>(new ComponentEventHandler<PDAComponent, LightToggleEvent>(this.OnLightToggle), null, null);
			base.SubscribeLocalEvent<PDAComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<PDAComponent, AfterActivatableUIOpenEvent>(this.AfterUIOpen), null, null);
			base.SubscribeLocalEvent<PDAComponent, StoreAddedEvent>(new ComponentEventRefHandler<PDAComponent, StoreAddedEvent>(this.OnUplinkInit), null, null);
			base.SubscribeLocalEvent<PDAComponent, StoreRemovedEvent>(new ComponentEventRefHandler<PDAComponent, StoreRemovedEvent>(this.OnUplinkRemoved), null, null);
			base.SubscribeLocalEvent<PDAComponent, GridModifiedEvent>(new ComponentEventHandler<PDAComponent, GridModifiedEvent>(this.OnGridChanged), null, null);
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x0004CB40 File Offset: 0x0004AD40
		protected override void OnComponentInit(EntityUid uid, PDAComponent pda, ComponentInit args)
		{
			base.OnComponentInit(uid, pda, args);
			ServerUserInterfaceComponent uiComponent;
			if (!base.TryComp<ServerUserInterfaceComponent>(uid, ref uiComponent))
			{
				return;
			}
			this.UpdateStationName(pda);
			BoundUserInterface ui;
			if (this._uiSystem.TryGetUi(uid, PDAUiKey.Key, ref ui, uiComponent))
			{
				ui.OnReceiveMessage += delegate(ServerBoundUserInterfaceMessage msg)
				{
					this.OnUIMessage(pda, msg);
				};
			}
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0004CBB0 File Offset: 0x0004ADB0
		protected override void OnItemInserted(EntityUid uid, PDAComponent pda, EntInsertedIntoContainerMessage args)
		{
			base.OnItemInserted(uid, pda, args);
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0004CBC2 File Offset: 0x0004ADC2
		protected override void OnItemRemoved(EntityUid uid, PDAComponent pda, EntRemovedFromContainerMessage args)
		{
			base.OnItemRemoved(uid, pda, args);
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0004CBD4 File Offset: 0x0004ADD4
		private void OnLightToggle(EntityUid uid, PDAComponent pda, LightToggleEvent args)
		{
			pda.FlashlightOn = args.IsOn;
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0004CBE9 File Offset: 0x0004ADE9
		public void SetOwner(PDAComponent pda, string ownerName)
		{
			pda.OwnerName = ownerName;
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0004CBF9 File Offset: 0x0004ADF9
		private void OnUplinkInit(EntityUid uid, PDAComponent pda, ref StoreAddedEvent args)
		{
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0004CC02 File Offset: 0x0004AE02
		private void OnUplinkRemoved(EntityUid uid, PDAComponent pda, ref StoreRemovedEvent args)
		{
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0004CC0B File Offset: 0x0004AE0B
		private void OnGridChanged(EntityUid uid, PDAComponent pda, GridModifiedEvent args)
		{
			this.UpdateStationName(pda);
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0004CC1C File Offset: 0x0004AE1C
		private void UpdatePDAUserInterface(PDAComponent pda)
		{
			PDAIdInfoText pdaidInfoText = default(PDAIdInfoText);
			pdaidInfoText.ActualOwnerName = pda.OwnerName;
			IdCardComponent containedID = pda.ContainedID;
			pdaidInfoText.IdOwner = ((containedID != null) ? containedID.FullName : null);
			IdCardComponent containedID2 = pda.ContainedID;
			pdaidInfoText.JobTitle = ((containedID2 != null) ? containedID2.JobTitle : null);
			PDAIdInfoText ownerInfo = pdaidInfoText;
			BoundUserInterface ui;
			if (!this._uiSystem.TryGetUi(pda.Owner, PDAUiKey.Key, ref ui, null))
			{
				return;
			}
			string address = this.GetDeviceNetAddress(pda.Owner);
			bool hasInstrument = base.HasComp<InstrumentComponent>(pda.Owner);
			PDAUpdateState state = new PDAUpdateState(pda.FlashlightOn, pda.PenSlot.HasItem, ownerInfo, pda.StationName, false, hasInstrument, address);
			CartridgeLoaderSystem cartridgeLoaderSystem = this._cartridgeLoaderSystem;
			if (cartridgeLoaderSystem != null)
			{
				cartridgeLoaderSystem.UpdateUiState(pda.Owner, state, null, null);
			}
			StoreComponent storeComponent;
			if (!base.TryComp<StoreComponent>(pda.Owner, ref storeComponent))
			{
				return;
			}
			PDAUpdateState uplinkState = new PDAUpdateState(pda.FlashlightOn, pda.PenSlot.HasItem, ownerInfo, pda.StationName, true, hasInstrument, address);
			foreach (IPlayerSession session in ui.SubscribedSessions)
			{
				EntityUid? entityUid = session.AttachedEntity;
				if (entityUid != null)
				{
					EntityUid user = entityUid.GetValueOrDefault();
					if (user.Valid)
					{
						entityUid = storeComponent.AccountOwner;
						EntityUid entityUid2 = user;
						MindComponent mindcomp;
						if ((entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == entityUid2)) || (base.TryComp<MindComponent>(session.AttachedEntity, ref mindcomp) && mindcomp.Mind != null && mindcomp.Mind.HasRole<TraitorRole>()))
						{
							CartridgeLoaderSystem cartridgeLoaderSystem2 = this._cartridgeLoaderSystem;
							if (cartridgeLoaderSystem2 != null)
							{
								cartridgeLoaderSystem2.UpdateUiState(pda.Owner, uplinkState, session, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0004CE00 File Offset: 0x0004B000
		private void OnUIMessage(PDAComponent pda, ServerBoundUserInterfaceMessage msg)
		{
			EntityUid pdaEnt = pda.Owner;
			BoundUserInterfaceMessage message = msg.Message;
			if (!(message is PDARequestUpdateInterfaceMessage))
			{
				UnpoweredFlashlightComponent flashlight;
				if (!(message is PDAToggleFlashlightMessage))
				{
					StoreComponent store;
					if (!(message is PDAShowUplinkMessage))
					{
						RingerComponent ringer;
						if (!(message is PDAShowRingtoneMessage))
						{
							if (!(message is PDAShowMusicMessage))
							{
								return;
							}
							InstrumentComponent instrument;
							if (base.TryComp<InstrumentComponent>(pdaEnt, ref instrument))
							{
								this._instrumentSystem.ToggleInstrumentUi(pdaEnt, msg.Session, instrument);
							}
						}
						else if (this.EntityManager.TryGetComponent<RingerComponent>(pdaEnt, ref ringer))
						{
							this._ringerSystem.ToggleRingerUI(ringer, msg.Session);
							return;
						}
					}
					else if (msg.Session.AttachedEntity != null && base.TryComp<StoreComponent>(pdaEnt, ref store))
					{
						this._storeSystem.ToggleUi(msg.Session.AttachedEntity.Value, pdaEnt, store);
						return;
					}
				}
				else if (this.EntityManager.TryGetComponent<UnpoweredFlashlightComponent>(pdaEnt, ref flashlight))
				{
					this._unpoweredFlashlight.ToggleLight(pdaEnt, flashlight);
					return;
				}
				return;
			}
			this.UpdatePDAUserInterface(pda);
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0004CEF8 File Offset: 0x0004B0F8
		private void UpdateStationName(PDAComponent pda)
		{
			EntityUid? station = this._stationSystem.GetOwningStation(pda.Owner, null);
			pda.StationName = ((station == null) ? null : base.Name(station.Value, null));
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x0004CF38 File Offset: 0x0004B138
		private void AfterUIOpen(EntityUid uid, PDAComponent pda, AfterActivatableUIOpenEvent args)
		{
			StoreComponent storeComp;
			if (!base.TryComp<StoreComponent>(pda.Owner, ref storeComp))
			{
				return;
			}
			EntityUid? accountOwner = storeComp.AccountOwner;
			EntityUid user = args.User;
			MindComponent mindcomp;
			if ((accountOwner == null || (accountOwner != null && accountOwner.GetValueOrDefault() != user)) && (!base.TryComp<MindComponent>(args.User, ref mindcomp) || mindcomp.Mind == null || !mindcomp.Mind.HasRole<TraitorRole>()))
			{
				return;
			}
			BoundUserInterface ui;
			if (!this._uiSystem.TryGetUi(pda.Owner, PDAUiKey.Key, ref ui, null))
			{
				return;
			}
			PDAIdInfoText pdaidInfoText = default(PDAIdInfoText);
			pdaidInfoText.ActualOwnerName = pda.OwnerName;
			IdCardComponent containedID = pda.ContainedID;
			pdaidInfoText.IdOwner = ((containedID != null) ? containedID.FullName : null);
			IdCardComponent containedID2 = pda.ContainedID;
			pdaidInfoText.JobTitle = ((containedID2 != null) ? containedID2.JobTitle : null);
			PDAIdInfoText ownerInfo = pdaidInfoText;
			PDAUpdateState state = new PDAUpdateState(pda.FlashlightOn, pda.PenSlot.HasItem, ownerInfo, pda.StationName, true, base.HasComp<InstrumentComponent>(pda.Owner), this.GetDeviceNetAddress(pda.Owner));
			CartridgeLoaderSystem cartridgeLoaderSystem = this._cartridgeLoaderSystem;
			if (cartridgeLoaderSystem == null)
			{
				return;
			}
			cartridgeLoaderSystem.UpdateUiState(uid, state, args.Session, null);
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x0004D070 File Offset: 0x0004B270
		[NullableContext(2)]
		private string GetDeviceNetAddress(EntityUid uid)
		{
			string address = null;
			DeviceNetworkComponent deviceNetworkComponent;
			if (base.TryComp<DeviceNetworkComponent>(uid, ref deviceNetworkComponent))
			{
				address = ((deviceNetworkComponent != null) ? deviceNetworkComponent.Address : null);
			}
			return address;
		}

		// Token: 0x040008CE RID: 2254
		[Dependency]
		private readonly UnpoweredFlashlightSystem _unpoweredFlashlight;

		// Token: 0x040008CF RID: 2255
		[Dependency]
		private readonly RingerSystem _ringerSystem;

		// Token: 0x040008D0 RID: 2256
		[Dependency]
		private readonly InstrumentSystem _instrumentSystem;

		// Token: 0x040008D1 RID: 2257
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x040008D2 RID: 2258
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040008D3 RID: 2259
		[Dependency]
		private readonly CartridgeLoaderSystem _cartridgeLoaderSystem;

		// Token: 0x040008D4 RID: 2260
		[Dependency]
		private readonly StoreSystem _storeSystem;
	}
}
