using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.Components;
using Content.Server.MachineLinking.Events;
using Content.Server.Power.Components;
using Content.Server.Power.NodeGroups;
using Content.Server.Tools;
using Content.Shared.Interaction;
using Content.Shared.MachineLinking;
using Content.Shared.MachineLinking.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003F1 RID: 1009
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalLinkerSystem : EntitySystem
	{
		// Token: 0x06001497 RID: 5271 RVA: 0x0006AEEC File Offset: 0x000690EC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SignalTransmitterComponent, ComponentStartup>(new ComponentEventHandler<SignalTransmitterComponent, ComponentStartup>(this.OnTransmitterStartup), null, null);
			base.SubscribeLocalEvent<SignalTransmitterComponent, ComponentRemove>(new ComponentEventHandler<SignalTransmitterComponent, ComponentRemove>(this.OnTransmitterRemoved), null, null);
			base.SubscribeLocalEvent<SignalTransmitterComponent, InteractUsingEvent>(new ComponentEventHandler<SignalTransmitterComponent, InteractUsingEvent>(this.OnTransmitterInteractUsing), null, null);
			base.SubscribeLocalEvent<SignalTransmitterComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SignalTransmitterComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetTransmitterVerbs), null, null);
			base.SubscribeLocalEvent<SignalReceiverComponent, ComponentStartup>(new ComponentEventHandler<SignalReceiverComponent, ComponentStartup>(this.OnReceiverStartup), null, null);
			base.SubscribeLocalEvent<SignalReceiverComponent, ComponentRemove>(new ComponentEventHandler<SignalReceiverComponent, ComponentRemove>(this.OnReceiverRemoved), null, null);
			base.SubscribeLocalEvent<SignalReceiverComponent, InteractUsingEvent>(new ComponentEventHandler<SignalReceiverComponent, InteractUsingEvent>(this.OnReceiverInteractUsing), null, null);
			base.SubscribeLocalEvent<SignalReceiverComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<SignalReceiverComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetReceiverVerbs), null, null);
			base.SubscribeLocalEvent<SignalLinkerComponent, SignalPortSelected>(new ComponentEventHandler<SignalLinkerComponent, SignalPortSelected>(this.OnSignalPortSelected), null, null);
			base.SubscribeLocalEvent<SignalLinkerComponent, LinkerClearSelected>(new ComponentEventHandler<SignalLinkerComponent, LinkerClearSelected>(this.OnLinkerClearSelected), null, null);
			base.SubscribeLocalEvent<SignalLinkerComponent, LinkerLinkDefaultSelected>(new ComponentEventHandler<SignalLinkerComponent, LinkerLinkDefaultSelected>(this.OnLinkerLinkDefaultSelected), null, null);
			base.SubscribeLocalEvent<SignalLinkerComponent, BoundUIClosedEvent>(new ComponentEventHandler<SignalLinkerComponent, BoundUIClosedEvent>(this.OnLinkerUIClosed), null, null);
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x0006AFF0 File Offset: 0x000691F0
		public void EnsureReceiverPorts(EntityUid uid, params string[] ports)
		{
			SignalReceiverComponent comp = base.EnsureComp<SignalReceiverComponent>(uid);
			foreach (string port in ports)
			{
				comp.Inputs.TryAdd(port, new List<PortIdentifier>());
			}
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x0006B02C File Offset: 0x0006922C
		public void EnsureTransmitterPorts(EntityUid uid, params string[] ports)
		{
			SignalTransmitterComponent comp = base.EnsureComp<SignalTransmitterComponent>(uid);
			foreach (string port in ports)
			{
				comp.Outputs.TryAdd(port, new List<PortIdentifier>());
			}
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x0006B068 File Offset: 0x00069268
		private void OnGetReceiverVerbs(EntityUid uid, SignalReceiverComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			SignalLinkerComponent linker;
			if (!base.TryComp<SignalLinkerComponent>(args.Using, ref linker) || !this.IsLinkerInteractable(args.Using.Value, linker))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = Loc.GetString("signal-linking-verb-text-link-default"),
				IconEntity = args.Using
			};
			args.Verbs.Add(verb);
			if (linker.SavedTransmitter != null)
			{
				verb.Act = delegate()
				{
					string msg = this.TryLinkDefaults(uid, linker.SavedTransmitter.Value, new EntityUid?(args.User), component, null) ? Loc.GetString("signal-linking-verb-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", linker.SavedTransmitter.Value)
					}) : Loc.GetString("signal-linking-verb-fail", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", linker.SavedTransmitter.Value)
					});
					this._popupSystem.PopupEntity(msg, uid, args.User, PopupType.Small);
				};
				return;
			}
			verb.Disabled = true;
			verb.Message = Loc.GetString("signal-linking-verb-disabled-no-transmitter");
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x0006B160 File Offset: 0x00069360
		private void OnGetTransmitterVerbs(EntityUid uid, SignalTransmitterComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			SignalLinkerComponent linker;
			if (!base.TryComp<SignalLinkerComponent>(args.Using, ref linker) || !this.IsLinkerInteractable(args.Using.Value, linker))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Text = Loc.GetString("signal-linking-verb-text-link-default"),
				IconEntity = args.Using
			};
			args.Verbs.Add(verb);
			if (linker.SavedReceiver != null)
			{
				verb.Act = delegate()
				{
					string msg = this.TryLinkDefaults(linker.SavedReceiver.Value, uid, new EntityUid?(args.User), null, component) ? Loc.GetString("signal-linking-verb-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", linker.SavedReceiver.Value)
					}) : Loc.GetString("signal-linking-verb-fail", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", linker.SavedReceiver.Value)
					});
					this._popupSystem.PopupEntity(msg, uid, args.User, PopupType.Small);
				};
				return;
			}
			verb.Disabled = true;
			verb.Message = Loc.GetString("signal-linking-verb-disabled-no-receiver");
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x0006B258 File Offset: 0x00069458
		public void InvokePort(EntityUid uid, string port, [Nullable(2)] SignalTransmitterComponent component = null)
		{
			if (!base.Resolve<SignalTransmitterComponent>(uid, ref component, true))
			{
				return;
			}
			List<PortIdentifier> receivers;
			if (!component.Outputs.TryGetValue(port, out receivers))
			{
				return;
			}
			foreach (PortIdentifier receiver in receivers)
			{
				base.RaiseLocalEvent<SignalReceivedEvent>(receiver.Uid, new SignalReceivedEvent(receiver.Port, new EntityUid?(uid)), false);
			}
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x0006B2DC File Offset: 0x000694DC
		private void OnTransmitterStartup(EntityUid uid, SignalTransmitterComponent transmitter, ComponentStartup args)
		{
			Dictionary<EntityUid, SignalReceiverComponent> uidCache = new Dictionary<EntityUid, SignalReceiverComponent>();
			foreach (KeyValuePair<string, List<PortIdentifier>> tport in transmitter.Outputs)
			{
				foreach (PortIdentifier rport in tport.Value.ToList<PortIdentifier>())
				{
					SignalReceiverComponent receiver;
					if (!uidCache.TryGetValue(rport.Uid, out receiver))
					{
						uidCache.Add(rport.Uid, receiver = base.CompOrNull<SignalReceiverComponent>(rport.Uid));
					}
					List<PortIdentifier> rpv;
					if (receiver == null || !receiver.Inputs.TryGetValue(rport.Port, out rpv))
					{
						tport.Value.Remove(rport);
					}
					else if (!rpv.Contains(new PortIdentifier(uid, tport.Key)))
					{
						rpv.Add(new PortIdentifier(uid, tport.Key));
					}
				}
			}
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x0006B400 File Offset: 0x00069600
		private void OnReceiverStartup(EntityUid uid, SignalReceiverComponent receiver, ComponentStartup args)
		{
			Dictionary<EntityUid, SignalTransmitterComponent> uidCache = new Dictionary<EntityUid, SignalTransmitterComponent>();
			using (Dictionary<string, List<PortIdentifier>>.Enumerator enumerator = receiver.Inputs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, List<PortIdentifier>> rport = enumerator.Current;
					List<PortIdentifier> toRemove = new List<PortIdentifier>();
					foreach (PortIdentifier tport2 in rport.Value)
					{
						SignalTransmitterComponent transmitter;
						if (!uidCache.TryGetValue(tport2.Uid, out transmitter))
						{
							uidCache.Add(tport2.Uid, transmitter = base.CompOrNull<SignalTransmitterComponent>(tport2.Uid));
						}
						List<PortIdentifier> tpv;
						if (transmitter == null || !transmitter.Outputs.TryGetValue(tport2.Port, out tpv))
						{
							toRemove.Add(tport2);
						}
						else if (!tpv.Contains(new PortIdentifier(uid, rport.Key)))
						{
							tpv.Add(new PortIdentifier(uid, rport.Key));
						}
					}
					toRemove.ForEach(delegate(PortIdentifier tport)
					{
						rport.Value.Remove(tport);
					});
				}
			}
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x0006B548 File Offset: 0x00069748
		private void OnTransmitterRemoved(EntityUid uid, SignalTransmitterComponent transmitter, ComponentRemove args)
		{
			Dictionary<EntityUid, SignalReceiverComponent> uidCache = new Dictionary<EntityUid, SignalReceiverComponent>();
			foreach (KeyValuePair<string, List<PortIdentifier>> tport in transmitter.Outputs)
			{
				foreach (PortIdentifier rport in tport.Value)
				{
					SignalReceiverComponent receiver;
					if (!uidCache.TryGetValue(rport.Uid, out receiver))
					{
						uidCache.Add(rport.Uid, receiver = base.CompOrNull<SignalReceiverComponent>(rport.Uid));
					}
					List<PortIdentifier> rpv;
					if (receiver != null && receiver.Inputs.TryGetValue(rport.Port, out rpv))
					{
						rpv.Remove(new PortIdentifier(uid, tport.Key));
					}
				}
			}
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x0006B63C File Offset: 0x0006983C
		private void OnReceiverRemoved(EntityUid uid, SignalReceiverComponent component, ComponentRemove args)
		{
			Dictionary<EntityUid, SignalTransmitterComponent> uidCache = new Dictionary<EntityUid, SignalTransmitterComponent>();
			foreach (KeyValuePair<string, List<PortIdentifier>> rport in component.Inputs)
			{
				foreach (PortIdentifier tport in rport.Value)
				{
					SignalTransmitterComponent transmitter;
					if (!uidCache.TryGetValue(tport.Uid, out transmitter))
					{
						uidCache.Add(tport.Uid, transmitter = base.CompOrNull<SignalTransmitterComponent>(tport.Uid));
					}
					List<PortIdentifier> receivers;
					if (transmitter != null && transmitter.Outputs.TryGetValue(tport.Port, out receivers))
					{
						receivers.Remove(new PortIdentifier(uid, rport.Key));
					}
				}
			}
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x0006B730 File Offset: 0x00069930
		private void OnTransmitterInteractUsing(EntityUid uid, SignalTransmitterComponent transmitter, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SignalLinkerComponent linker;
			ActorComponent actor;
			if (!base.TryComp<SignalLinkerComponent>(args.Used, ref linker) || !this.IsLinkerInteractable(args.Used, linker) || !base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			linker.SavedTransmitter = new EntityUid?(uid);
			SignalReceiverComponent receiver;
			if (!base.TryComp<SignalReceiverComponent>(linker.SavedReceiver, ref receiver))
			{
				this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-saved", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("machine", uid)
				}), args.User, PopupType.Medium);
				args.Handled = true;
				return;
			}
			BoundUserInterface bui;
			if (this.TryGetOrOpenUI(actor, linker, out bui))
			{
				this.TryUpdateUI(linker, transmitter, receiver, bui);
				args.Handled = true;
			}
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x0006B7F4 File Offset: 0x000699F4
		private void OnReceiverInteractUsing(EntityUid uid, SignalReceiverComponent receiver, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SignalLinkerComponent linker;
			ActorComponent actor;
			if (!base.TryComp<SignalLinkerComponent>(args.Used, ref linker) || !this.IsLinkerInteractable(args.Used, linker) || !base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			linker.SavedReceiver = new EntityUid?(uid);
			SignalTransmitterComponent transmitter;
			if (!base.TryComp<SignalTransmitterComponent>(linker.SavedTransmitter, ref transmitter))
			{
				this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-saved", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("machine", uid)
				}), args.User, PopupType.Medium);
				args.Handled = true;
				return;
			}
			BoundUserInterface bui;
			if (this.TryGetOrOpenUI(actor, linker, out bui))
			{
				this.TryUpdateUI(linker, transmitter, receiver, bui);
				args.Handled = true;
			}
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x0006B8B5 File Offset: 0x00069AB5
		private bool TryGetOrOpenUI(ActorComponent actor, SignalLinkerComponent linker, [Nullable(2)] [NotNullWhen(true)] out BoundUserInterface bui)
		{
			if (this._userInterfaceSystem.TryGetUi(linker.Owner, SignalLinkerUiKey.Key, ref bui, null))
			{
				bui.Open(actor.PlayerSession);
				return true;
			}
			return false;
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x0006B8E4 File Offset: 0x00069AE4
		private bool TryUpdateUI(SignalLinkerComponent linker, SignalTransmitterComponent transmitter, SignalReceiverComponent receiver, [Nullable(2)] BoundUserInterface bui = null)
		{
			if (bui == null && !this._userInterfaceSystem.TryGetUi(linker.Owner, SignalLinkerUiKey.Key, ref bui, null))
			{
				return false;
			}
			List<string> outKeys = transmitter.Outputs.Keys.ToList<string>();
			List<string> inKeys = receiver.Inputs.Keys.ToList<string>();
			List<ValueTuple<int, int>> links = new List<ValueTuple<int, int>>();
			for (int i = 0; i < outKeys.Count; i++)
			{
				foreach (PortIdentifier re in transmitter.Outputs[outKeys[i]])
				{
					if (re.Uid == receiver.Owner)
					{
						links.Add(new ValueTuple<int, int>(i, inKeys.IndexOf(re.Port)));
					}
				}
			}
			BoundUserInterface boundUserInterface = bui;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(base.Name(transmitter.Owner, null));
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(transmitter.Owner);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			string transmitterName = defaultInterpolatedStringHandler.ToStringAndClear();
			List<string> transmitterPorts = outKeys;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
			defaultInterpolatedStringHandler.AppendFormatted(base.Name(receiver.Owner, null));
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(receiver.Owner);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			boundUserInterface.SetState(new SignalPortsState(transmitterName, transmitterPorts, defaultInterpolatedStringHandler.ToStringAndClear(), inKeys, links), null, true);
			return true;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x0006BA6C File Offset: 0x00069C6C
		private bool TryLink(SignalTransmitterComponent transmitter, SignalReceiverComponent receiver, SignalPortSelected args, EntityUid? user, bool quiet = false, bool checkRange = true)
		{
			List<PortIdentifier> linkedReceivers;
			List<PortIdentifier> linkedTransmitters;
			if (!transmitter.Outputs.TryGetValue(args.TransmitterPort, out linkedReceivers) || !receiver.Inputs.TryGetValue(args.ReceiverPort, out linkedTransmitters))
			{
				return false;
			}
			quiet |= (user == null);
			foreach (PortIdentifier identifier in linkedTransmitters)
			{
				if (identifier.Uid == transmitter.Owner && identifier.Port == args.TransmitterPort)
				{
					return true;
				}
			}
			if (checkRange && !this.IsInRange(transmitter, receiver))
			{
				if (!quiet)
				{
					this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-out-of-range"), user.Value, PopupType.Small);
				}
				return false;
			}
			LinkAttemptEvent linkAttempt = new LinkAttemptEvent(user, transmitter.Owner, args.TransmitterPort, receiver.Owner, args.ReceiverPort);
			base.RaiseLocalEvent<LinkAttemptEvent>(transmitter.Owner, linkAttempt, true);
			if (linkAttempt.Cancelled)
			{
				if (!quiet)
				{
					this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-connection-refused", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", transmitter.Owner)
					}), user.Value, PopupType.Small);
				}
				return false;
			}
			base.RaiseLocalEvent<LinkAttemptEvent>(receiver.Owner, linkAttempt, true);
			if (linkAttempt.Cancelled)
			{
				if (!quiet)
				{
					this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-connection-refused", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("machine", receiver.Owner)
					}), user.Value, PopupType.Small);
				}
				return false;
			}
			linkedReceivers.Add(new PortIdentifier(receiver.Owner, args.ReceiverPort));
			linkedTransmitters.Add(new PortIdentifier(transmitter.Owner, args.TransmitterPort));
			if (!quiet)
			{
				this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-linked-port", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("machine1", transmitter.Owner),
					new ValueTuple<string, object>("port1", this.PortName<TransmitterPortPrototype>(args.TransmitterPort)),
					new ValueTuple<string, object>("machine2", receiver.Owner),
					new ValueTuple<string, object>("port2", this.PortName<ReceiverPortPrototype>(args.ReceiverPort))
				}), user.Value, PopupType.Medium);
			}
			NewLinkEvent newLink = new NewLinkEvent(user, transmitter.Owner, args.TransmitterPort, receiver.Owner, args.ReceiverPort);
			base.RaiseLocalEvent<NewLinkEvent>(receiver.Owner, newLink, false);
			base.RaiseLocalEvent<NewLinkEvent>(transmitter.Owner, newLink, false);
			return true;
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x0006BD34 File Offset: 0x00069F34
		private void OnSignalPortSelected(EntityUid uid, SignalLinkerComponent linker, SignalPortSelected args)
		{
			SignalTransmitterComponent transmitter;
			SignalReceiverComponent receiver;
			List<PortIdentifier> receivers;
			List<PortIdentifier> transmitters;
			if (!base.TryComp<SignalTransmitterComponent>(linker.SavedTransmitter, ref transmitter) || !base.TryComp<SignalReceiverComponent>(linker.SavedReceiver, ref receiver) || !transmitter.Outputs.TryGetValue(args.TransmitterPort, out receivers) || !receiver.Inputs.TryGetValue(args.ReceiverPort, out transmitters))
			{
				return;
			}
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid attached = attachedEntity.GetValueOrDefault();
				if (attached.Valid)
				{
					if (receivers.Contains(new PortIdentifier(receiver.Owner, args.ReceiverPort)) || transmitters.Contains(new PortIdentifier(transmitter.Owner, args.TransmitterPort)))
					{
						if (receivers.Remove(new PortIdentifier(receiver.Owner, args.ReceiverPort)) && transmitters.Remove(new PortIdentifier(transmitter.Owner, args.TransmitterPort)))
						{
							base.RaiseLocalEvent<PortDisconnectedEvent>(receiver.Owner, new PortDisconnectedEvent(args.ReceiverPort), true);
							base.RaiseLocalEvent<PortDisconnectedEvent>(transmitter.Owner, new PortDisconnectedEvent(args.TransmitterPort), true);
							this._popupSystem.PopupCursor(Loc.GetString("signal-linker-component-unlinked-port", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("machine1", transmitter.Owner),
								new ValueTuple<string, object>("port1", this.PortName<TransmitterPortPrototype>(args.TransmitterPort)),
								new ValueTuple<string, object>("machine2", receiver.Owner),
								new ValueTuple<string, object>("port2", this.PortName<ReceiverPortPrototype>(args.ReceiverPort))
							}), attached, PopupType.Medium);
						}
					}
					else
					{
						this.TryLink(transmitter, receiver, args, new EntityUid?(attached), false, true);
					}
					this.TryUpdateUI(linker, transmitter, receiver, null);
					return;
				}
			}
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x0006BF04 File Offset: 0x0006A104
		public string PortName<[Nullable(0)] TPort>(string port) where TPort : MachinePortPrototype, IPrototype
		{
			TPort proto;
			if (!this._protoMan.TryIndex<TPort>(port, ref proto))
			{
				return port;
			}
			return Loc.GetString(proto.Name);
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x0006BF34 File Offset: 0x0006A134
		private void OnLinkerClearSelected(EntityUid uid, SignalLinkerComponent linker, LinkerClearSelected args)
		{
			SignalReceiverComponent receiver;
			SignalTransmitterComponent transmitter;
			if (!base.TryComp<SignalTransmitterComponent>(linker.SavedTransmitter, ref transmitter) || !base.TryComp<SignalReceiverComponent>(linker.SavedReceiver, ref receiver))
			{
				return;
			}
			Predicate<PortIdentifier> <>9__0;
			foreach (KeyValuePair<string, List<PortIdentifier>> keyValuePair in transmitter.Outputs)
			{
				string text;
				List<PortIdentifier> list;
				keyValuePair.Deconstruct(out text, out list);
				string port = text;
				List<PortIdentifier> list2 = list;
				Predicate<PortIdentifier> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((PortIdentifier id) => id.Uid == receiver.Owner));
				}
				if (list2.RemoveAll(match) > 0)
				{
					base.RaiseLocalEvent<PortDisconnectedEvent>(transmitter.Owner, new PortDisconnectedEvent(port), true);
				}
			}
			Predicate<PortIdentifier> <>9__1;
			foreach (KeyValuePair<string, List<PortIdentifier>> keyValuePair in receiver.Inputs)
			{
				string text;
				List<PortIdentifier> list;
				keyValuePair.Deconstruct(out text, out list);
				string port2 = text;
				List<PortIdentifier> list3 = list;
				Predicate<PortIdentifier> match2;
				if ((match2 = <>9__1) == null)
				{
					match2 = (<>9__1 = ((PortIdentifier id) => id.Uid == transmitter.Owner));
				}
				if (list3.RemoveAll(match2) > 0)
				{
					base.RaiseLocalEvent<PortDisconnectedEvent>(receiver.Owner, new PortDisconnectedEvent(port2), true);
				}
			}
			this.TryUpdateUI(linker, transmitter, receiver, null);
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x0006C0B0 File Offset: 0x0006A2B0
		private void OnLinkerLinkDefaultSelected(EntityUid uid, SignalLinkerComponent linker, LinkerLinkDefaultSelected args)
		{
			SignalTransmitterComponent transmitter;
			SignalReceiverComponent receiver;
			if (!base.TryComp<SignalTransmitterComponent>(linker.SavedTransmitter, ref transmitter) || !base.TryComp<SignalReceiverComponent>(linker.SavedReceiver, ref receiver))
			{
				return;
			}
			EntityUid? attachedEntity = args.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid user = attachedEntity.GetValueOrDefault();
				if (user.Valid)
				{
					this.TryLinkDefaults(linker.SavedReceiver.Value, linker.SavedTransmitter.Value, new EntityUid?(user), receiver, transmitter);
					this.TryUpdateUI(linker, transmitter, receiver, null);
					return;
				}
			}
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x0006C138 File Offset: 0x0006A338
		[NullableContext(2)]
		public bool TryLinkDefaults(EntityUid receiverUid, EntityUid transmitterUid, EntityUid? user, SignalReceiverComponent receiver = null, SignalTransmitterComponent transmitter = null)
		{
			if (!base.Resolve<SignalReceiverComponent>(receiverUid, ref receiver, false) || !base.Resolve<SignalTransmitterComponent>(transmitterUid, ref transmitter, false))
			{
				return false;
			}
			if (!this.IsInRange(transmitter, receiver))
			{
				return false;
			}
			bool allLinksSucceeded = true;
			Predicate<PortIdentifier> <>9__0;
			foreach (KeyValuePair<string, List<PortIdentifier>> keyValuePair in transmitter.Outputs)
			{
				string text;
				List<PortIdentifier> list;
				keyValuePair.Deconstruct(out text, out list);
				string port = text;
				List<PortIdentifier> list2 = list;
				Predicate<PortIdentifier> match;
				if ((match = <>9__0) == null)
				{
					match = (<>9__0 = ((PortIdentifier id) => id.Uid == receiver.Owner));
				}
				if (list2.RemoveAll(match) > 0)
				{
					base.RaiseLocalEvent<PortDisconnectedEvent>(transmitter.Owner, new PortDisconnectedEvent(port), true);
				}
			}
			Predicate<PortIdentifier> <>9__1;
			foreach (KeyValuePair<string, List<PortIdentifier>> keyValuePair in receiver.Inputs)
			{
				string text;
				List<PortIdentifier> list;
				keyValuePair.Deconstruct(out text, out list);
				string port2 = text;
				List<PortIdentifier> list3 = list;
				Predicate<PortIdentifier> match2;
				if ((match2 = <>9__1) == null)
				{
					match2 = (<>9__1 = ((PortIdentifier id) => id.Uid == transmitter.Owner));
				}
				if (list3.RemoveAll(match2) > 0)
				{
					base.RaiseLocalEvent<PortDisconnectedEvent>(receiver.Owner, new PortDisconnectedEvent(port2), true);
				}
			}
			foreach (string outPort in transmitter.Outputs.Keys)
			{
				TransmitterPortPrototype prototype = this._protoMan.Index<TransmitterPortPrototype>(outPort);
				if (prototype.DefaultLinks != null)
				{
					foreach (string inPort in prototype.DefaultLinks)
					{
						if (receiver.Inputs.ContainsKey(inPort))
						{
							allLinksSucceeded &= this.TryLink(transmitter, receiver, new SignalPortSelected(outPort, inPort), user, true, false);
						}
					}
				}
			}
			return allLinksSucceeded;
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x0006C398 File Offset: 0x0006A598
		private void OnLinkerUIClosed(EntityUid uid, SignalLinkerComponent component, BoundUIClosedEvent args)
		{
			component.SavedTransmitter = null;
			component.SavedReceiver = null;
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x0006C3B4 File Offset: 0x0006A5B4
		private bool IsInRange(SignalTransmitterComponent transmitterComponent, SignalReceiverComponent receiverComponent)
		{
			ApcPowerReceiverComponent transmitterPower;
			ApcPowerReceiverComponent receiverPower;
			if (base.TryComp<ApcPowerReceiverComponent>(transmitterComponent.Owner, ref transmitterPower) && base.TryComp<ApcPowerReceiverComponent>(receiverComponent.Owner, ref receiverPower))
			{
				ApcPowerProviderComponent provider = transmitterPower.Provider;
				IApcNet apcNet = (provider != null) ? provider.Net : null;
				ApcPowerProviderComponent provider2 = receiverPower.Provider;
				if (apcNet == ((provider2 != null) ? provider2.Net : null))
				{
					return true;
				}
			}
			return base.Comp<TransformComponent>(transmitterComponent.Owner).MapPosition.InRange(base.Comp<TransformComponent>(receiverComponent.Owner).MapPosition, transmitterComponent.TransmissionRange);
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x0006C43C File Offset: 0x0006A63C
		private bool IsLinkerInteractable(EntityUid uid, SignalLinkerComponent linkerComponent)
		{
			string quality = linkerComponent.RequiredQuality;
			return quality == null || this._tools.HasQuality(uid, quality, null);
		}

		// Token: 0x04000CC4 RID: 3268
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04000CC5 RID: 3269
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000CC6 RID: 3270
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000CC7 RID: 3271
		[Dependency]
		private readonly ToolSystem _tools;
	}
}
