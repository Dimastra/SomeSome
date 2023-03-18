using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Paper;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Tools;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Fax;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Fax
{
	// Token: 0x02000502 RID: 1282
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FaxSystem : EntitySystem
	{
		// Token: 0x06001A64 RID: 6756 RVA: 0x0008AD24 File Offset: 0x00088F24
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FaxMachineComponent, ComponentInit>(new ComponentEventHandler<FaxMachineComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, MapInitEvent>(new ComponentEventHandler<FaxMachineComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, ComponentRemove>(new ComponentEventHandler<FaxMachineComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<FaxMachineComponent, EntInsertedIntoContainerMessage>(this.OnItemSlotChanged), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<FaxMachineComponent, EntRemovedFromContainerMessage>(this.OnItemSlotChanged), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, PowerChangedEvent>(new ComponentEventRefHandler<FaxMachineComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<FaxMachineComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, InteractUsingEvent>(new ComponentEventHandler<FaxMachineComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, GotEmaggedEvent>(new ComponentEventRefHandler<FaxMachineComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<FaxMachineComponent, AfterActivatableUIOpenEvent>(this.OnToggleInterface), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, FaxSendMessage>(new ComponentEventHandler<FaxMachineComponent, FaxSendMessage>(this.OnSendButtonPressed), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, FaxRefreshMessage>(new ComponentEventHandler<FaxMachineComponent, FaxRefreshMessage>(this.OnRefreshButtonPressed), null, null);
			base.SubscribeLocalEvent<FaxMachineComponent, FaxDestinationMessage>(new ComponentEventHandler<FaxMachineComponent, FaxDestinationMessage>(this.OnDestinationSelected), null, null);
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0008AE3C File Offset: 0x0008903C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<FaxMachineComponent, ApcPowerReceiverComponent> valueTuple in base.EntityQuery<FaxMachineComponent, ApcPowerReceiverComponent>(false))
			{
				FaxMachineComponent comp = valueTuple.Item1;
				if (valueTuple.Item2.Powered)
				{
					this.ProcessPrintingAnimation(frameTime, comp);
					this.ProcessInsertingAnimation(frameTime, comp);
					this.ProcessSendingTimeout(frameTime, comp);
				}
			}
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0008AEB4 File Offset: 0x000890B4
		private void ProcessPrintingAnimation(float frameTime, FaxMachineComponent comp)
		{
			if (comp.PrintingTimeRemaining > 0f)
			{
				comp.PrintingTimeRemaining -= frameTime;
				this.UpdateAppearance(comp.Owner, comp);
				if (comp.PrintingTimeRemaining <= 0f)
				{
					this.SpawnPaperFromQueue(comp.Owner, comp);
					this.UpdateUserInterface(comp.Owner, comp);
				}
				return;
			}
			if (comp.PrintingQueue.Count > 0)
			{
				comp.PrintingTimeRemaining = comp.PrintingTime;
				this._audioSystem.PlayPvs(comp.PrintSound, comp.Owner, null);
			}
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x0008AF54 File Offset: 0x00089154
		private void ProcessInsertingAnimation(float frameTime, FaxMachineComponent comp)
		{
			if (comp.InsertingTimeRemaining <= 0f)
			{
				return;
			}
			comp.InsertingTimeRemaining -= frameTime;
			this.UpdateAppearance(comp.Owner, comp);
			if (comp.InsertingTimeRemaining <= 0f)
			{
				this._itemSlotsSystem.SetLock(comp.Owner, comp.PaperSlot, false, null);
				this.UpdateUserInterface(comp.Owner, comp);
			}
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x0008AFC2 File Offset: 0x000891C2
		private void ProcessSendingTimeout(float frameTime, FaxMachineComponent comp)
		{
			if (comp.SendTimeoutRemaining > 0f)
			{
				comp.SendTimeoutRemaining -= frameTime;
				if (comp.SendTimeoutRemaining <= 0f)
				{
					this.UpdateUserInterface(comp.Owner, comp);
				}
			}
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x0008AFF9 File Offset: 0x000891F9
		private void OnComponentInit(EntityUid uid, FaxMachineComponent component, ComponentInit args)
		{
			this._itemSlotsSystem.AddItemSlot(uid, "Paper", component.PaperSlot, null);
			this.UpdateAppearance(uid, component);
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x0008B01B File Offset: 0x0008921B
		private void OnComponentRemove(EntityUid uid, FaxMachineComponent component, ComponentRemove args)
		{
			this._itemSlotsSystem.RemoveItemSlot(uid, component.PaperSlot, null);
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x0008B030 File Offset: 0x00089230
		private void OnMapInit(EntityUid uid, FaxMachineComponent component, MapInitEvent args)
		{
			this.Refresh(uid, component);
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x0008B03C File Offset: 0x0008923C
		private void OnItemSlotChanged(EntityUid uid, FaxMachineComponent component, ContainerModifiedMessage args)
		{
			if (!component.Initialized)
			{
				return;
			}
			if (args.Container.ID != component.PaperSlot.ID)
			{
				return;
			}
			if (component.PaperSlot.Item != null)
			{
				component.InsertingTimeRemaining = component.InsertionTime;
				this._itemSlotsSystem.SetLock(uid, component.PaperSlot, true, null);
			}
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0008B0B0 File Offset: 0x000892B0
		private void OnPowerChanged(EntityUid uid, FaxMachineComponent component, ref PowerChangedEvent args)
		{
			bool flag = !args.Powered && component.InsertingTimeRemaining > 0f;
			if (flag)
			{
				component.InsertingTimeRemaining = 0f;
				this._itemSlotsSystem.SetLock(uid, component.PaperSlot, false, null);
				EntityUid? entityUid;
				this._itemSlotsSystem.TryEject(uid, component.PaperSlot, null, out entityUid, true);
			}
			bool isPrintInterrupted = !args.Powered && component.PrintingTimeRemaining > 0f;
			if (isPrintInterrupted)
			{
				component.PrintingTimeRemaining = 0f;
			}
			if (flag || isPrintInterrupted)
			{
				this.UpdateAppearance(component.Owner, component);
			}
			this._itemSlotsSystem.SetLock(uid, component.PaperSlot, !args.Powered, null);
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0008B16C File Offset: 0x0008936C
		private void OnInteractUsing(EntityUid uid, FaxMachineComponent component, InteractUsingEvent args)
		{
			ActorComponent actor;
			if (args.Handled || !base.TryComp<ActorComponent>(args.User, ref actor) || !this._toolSystem.HasQuality(args.Used, "Screwing", null))
			{
				return;
			}
			this._quickDialog.OpenDialog<string>(actor.PlayerSession, Loc.GetString("fax-machine-dialog-rename"), Loc.GetString("fax-machine-dialog-field-name"), delegate(string newName)
			{
				if (component.FaxName == newName)
				{
					return;
				}
				if (newName.Length > 20)
				{
					this._popupSystem.PopupEntity(Loc.GetString("fax-machine-popup-name-long"), uid, PopupType.Small);
					return;
				}
				if (component.KnownFaxes.ContainsValue(newName) && !this.HasComp<EmaggedComponent>(uid))
				{
					this._popupSystem.PopupEntity(Loc.GetString("fax-machine-popup-name-exist"), uid, PopupType.Small);
					return;
				}
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(23, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
				logStringHandler.AppendLiteral(" renamed ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(uid), "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" from \"");
				logStringHandler.AppendFormatted(component.FaxName);
				logStringHandler.AppendLiteral("\" to \"");
				logStringHandler.AppendFormatted(newName);
				logStringHandler.AppendLiteral("\"");
				adminLogger.Add(type, impact, ref logStringHandler);
				component.FaxName = newName;
				this._popupSystem.PopupEntity(Loc.GetString("fax-machine-popup-name-set"), uid, PopupType.Small);
				this.UpdateUserInterface(uid, component);
			}, null);
			args.Handled = true;
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0008B21C File Offset: 0x0008941C
		private void OnEmagged(EntityUid uid, FaxMachineComponent component, ref GotEmaggedEvent args)
		{
			this._audioSystem.PlayPvs(component.EmagSound, uid, null);
			args.Handled = true;
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0008B24C File Offset: 0x0008944C
		private void OnPacketReceived(EntityUid uid, FaxMachineComponent component, DeviceNetworkPacketEvent args)
		{
			if (!base.HasComp<DeviceNetworkComponent>(uid) || string.IsNullOrEmpty(args.SenderAddress))
			{
				return;
			}
			string command;
			if (args.Data.TryGetValue<string>("command", out command))
			{
				if (!(command == "fax_ping"))
				{
					if (!(command == "fax_pong"))
					{
						if (!(command == "fax_print"))
						{
							return;
						}
						string name;
						string content;
						if (!args.Data.TryGetValue<string>("fax_data_title", out name) || !args.Data.TryGetValue<string>("fax_data_content", out content))
						{
							return;
						}
						string stampState;
						args.Data.TryGetValue<string>("fax_data_stamp_state", out stampState);
						List<string> stampedBy;
						args.Data.TryGetValue<List<string>>("fax_data_stamped_by", out stampedBy);
						string prototypeId;
						args.Data.TryGetValue<string>("fax_data_prototype", out prototypeId);
						FaxPrintout printout = new FaxPrintout(content, name, prototypeId, stampState, stampedBy);
						this.Receive(uid, printout, args.SenderAddress, null);
					}
					else
					{
						string faxName;
						if (!args.Data.TryGetValue<string>("fax_data_name", out faxName))
						{
							return;
						}
						component.KnownFaxes[args.SenderAddress] = faxName;
						this.UpdateUserInterface(uid, component);
						return;
					}
				}
				else
				{
					if ((!base.HasComp<EmaggedComponent>(uid) || !args.Data.ContainsKey("fax_data_i_am_syndicate")) && !component.ResponsePings)
					{
						return;
					}
					NetworkPayload payload = new NetworkPayload
					{
						{
							"command",
							"fax_pong"
						},
						{
							"fax_data_name",
							component.FaxName
						}
					};
					this._deviceNetworkSystem.QueuePacket(uid, args.SenderAddress, payload, null, null);
					return;
				}
			}
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0008B3D2 File Offset: 0x000895D2
		private void OnToggleInterface(EntityUid uid, FaxMachineComponent component, AfterActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0008B3DC File Offset: 0x000895DC
		private void OnSendButtonPressed(EntityUid uid, FaxMachineComponent component, FaxSendMessage args)
		{
			this.Send(uid, component, args.Session.AttachedEntity);
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x0008B3F1 File Offset: 0x000895F1
		private void OnRefreshButtonPressed(EntityUid uid, FaxMachineComponent component, FaxRefreshMessage args)
		{
			this.Refresh(uid, component);
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x0008B3FB File Offset: 0x000895FB
		private void OnDestinationSelected(EntityUid uid, FaxMachineComponent component, FaxDestinationMessage args)
		{
			this.SetDestination(uid, args.Address, component);
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0008B40C File Offset: 0x0008960C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.InsertingTimeRemaining > 0f)
			{
				this._appearanceSystem.SetData(uid, FaxMachineVisuals.VisualState, FaxMachineVisualState.Inserting, null);
				return;
			}
			if (component.PrintingTimeRemaining > 0f)
			{
				this._appearanceSystem.SetData(uid, FaxMachineVisuals.VisualState, FaxMachineVisualState.Printing, null);
				return;
			}
			this._appearanceSystem.SetData(uid, FaxMachineVisuals.VisualState, FaxMachineVisualState.Normal, null);
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0008B490 File Offset: 0x00089690
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			bool isPaperInserted = component.PaperSlot.Item != null;
			bool canSend = isPaperInserted && component.DestinationFaxAddress != null && component.SendTimeoutRemaining <= 0f && component.InsertingTimeRemaining <= 0f;
			FaxUiState state = new FaxUiState(component.FaxName, component.KnownFaxes, canSend, isPaperInserted, component.DestinationFaxAddress);
			this._userInterface.TrySetUiState(uid, FaxUiKey.Key, state, null, null, true);
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0008B51B File Offset: 0x0008971B
		public void SetDestination(EntityUid uid, string destAddress, [Nullable(2)] FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			component.DestinationFaxAddress = destAddress;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0008B53C File Offset: 0x0008973C
		[NullableContext(2)]
		public void Refresh(EntityUid uid, FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			component.DestinationFaxAddress = null;
			component.KnownFaxes.Clear();
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"fax_ping"
				}
			};
			if (base.HasComp<EmaggedComponent>(uid))
			{
				payload.Add("fax_data_i_am_syndicate", true);
			}
			this._deviceNetworkSystem.QueuePacket(uid, null, payload, null, null);
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x0008B5B4 File Offset: 0x000897B4
		[NullableContext(2)]
		public void Send(EntityUid uid, FaxMachineComponent component = null, EntityUid? sender = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			EntityUid? sendEntity = component.PaperSlot.Item;
			if (sendEntity == null)
			{
				return;
			}
			if (component.DestinationFaxAddress == null)
			{
				return;
			}
			string faxName;
			if (!component.KnownFaxes.TryGetValue(component.DestinationFaxAddress, out faxName))
			{
				return;
			}
			MetaDataComponent metadata;
			PaperComponent paper;
			if (!base.TryComp<MetaDataComponent>(sendEntity, ref metadata) || !base.TryComp<PaperComponent>(sendEntity, ref paper))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload
			{
				{
					"command",
					"fax_print"
				},
				{
					"fax_data_title",
					metadata.EntityName
				},
				{
					"fax_data_content",
					paper.Content
				}
			};
			if (metadata.EntityPrototype != null)
			{
				payload["fax_data_prototype"] = metadata.EntityPrototype.ID;
			}
			if (paper.StampState != null)
			{
				payload["fax_data_stamp_state"] = paper.StampState;
				payload["fax_data_stamped_by"] = paper.StampedBy;
			}
			this._deviceNetworkSystem.QueuePacket(uid, component.DestinationFaxAddress, payload, null, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(27, 6);
			logStringHandler.AppendFormatted((sender != null) ? base.ToPrettyString(sender.Value) : "Unknown", 0, "user");
			logStringHandler.AppendLiteral(" sent fax from \"");
			logStringHandler.AppendFormatted(component.FaxName);
			logStringHandler.AppendLiteral("\" ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted(faxName);
			logStringHandler.AppendLiteral(" (");
			logStringHandler.AppendFormatted(component.DestinationFaxAddress);
			logStringHandler.AppendLiteral("): ");
			logStringHandler.AppendFormatted(paper.Content);
			adminLogger.Add(type, impact, ref logStringHandler);
			component.SendTimeoutRemaining += component.SendTimeout;
			this._audioSystem.PlayPvs(component.SendSound, uid, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0008B7C0 File Offset: 0x000899C0
		[NullableContext(2)]
		public void Receive(EntityUid uid, [Nullable(1)] FaxPrintout printout, string fromAddress, FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true))
			{
				return;
			}
			string faxName = Loc.GetString("fax-machine-popup-source-unknown");
			if (fromAddress != null && component.KnownFaxes.ContainsKey(fromAddress))
			{
				faxName = component.KnownFaxes[fromAddress];
			}
			this._popupSystem.PopupEntity(Loc.GetString("fax-machine-popup-received", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("from", faxName)
			}), uid, PopupType.Small);
			this._appearanceSystem.SetData(uid, FaxMachineVisuals.VisualState, FaxMachineVisualState.Printing, null);
			if (component.NotifyAdmins)
			{
				this.NotifyAdmins(faxName);
			}
			component.PrintingQueue.Enqueue(printout);
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0008B86C File Offset: 0x00089A6C
		[NullableContext(2)]
		private void SpawnPaperFromQueue(EntityUid uid, FaxMachineComponent component = null)
		{
			if (!base.Resolve<FaxMachineComponent>(uid, ref component, true) || component.PrintingQueue.Count == 0)
			{
				return;
			}
			FaxPrintout printout = component.PrintingQueue.Dequeue();
			string entityToSpawn = (printout.PrototypeId.Length == 0) ? "Paper" : printout.PrototypeId;
			EntityUid printed = this.EntityManager.SpawnEntity(entityToSpawn, base.Transform(uid).Coordinates);
			PaperComponent paper;
			if (base.TryComp<PaperComponent>(printed, ref paper))
			{
				this._paperSystem.SetContent(printed, printout.Content, null);
				if (printout.StampState != null)
				{
					foreach (string stampedBy in printout.StampedBy)
					{
						this._paperSystem.TryStamp(printed, stampedBy, printout.StampState, null);
					}
				}
			}
			MetaDataComponent metadata;
			if (base.TryComp<MetaDataComponent>(printed, ref metadata))
			{
				metadata.EntityName = printout.Name;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(14, 4);
			logStringHandler.AppendLiteral("\"");
			logStringHandler.AppendFormatted(component.FaxName);
			logStringHandler.AppendLiteral("\" ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" printed ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(printed), "ToPrettyString(printed)");
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(printout.Content);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x0008B9F4 File Offset: 0x00089BF4
		private void NotifyAdmins(string faxName)
		{
			this._chat.SendAdminAnnouncement(Loc.GetString("fax-machine-chat-notify", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("fax", faxName)
			}));
			this._audioSystem.PlayGlobal("/Audio/Machines/high_tech_confirm.ogg", Filter.Empty().AddPlayers(this._adminManager.ActiveAdmins), false, null);
		}

		// Token: 0x040010D3 RID: 4307
		[Dependency]
		private readonly IChatManager _chat;

		// Token: 0x040010D4 RID: 4308
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x040010D5 RID: 4309
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040010D6 RID: 4310
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x040010D7 RID: 4311
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040010D8 RID: 4312
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x040010D9 RID: 4313
		[Dependency]
		private readonly PaperSystem _paperSystem;

		// Token: 0x040010DA RID: 4314
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x040010DB RID: 4315
		[Dependency]
		private readonly ToolSystem _toolSystem;

		// Token: 0x040010DC RID: 4316
		[Dependency]
		private readonly QuickDialogSystem _quickDialog;

		// Token: 0x040010DD RID: 4317
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x040010DE RID: 4318
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040010DF RID: 4319
		public const string PaperSlotId = "Paper";
	}
}
