using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
using Content.Server.Power.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Wires;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.Wires
{
	// Token: 0x02000074 RID: 116
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WiresSystem : EntitySystem
	{
		// Token: 0x06000186 RID: 390 RVA: 0x00008664 File Offset: 0x00006864
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<WiresSystem.WireToolFinishedEvent>(new EntityEventHandler<WiresSystem.WireToolFinishedEvent>(this.OnToolFinished), null, null);
			base.SubscribeLocalEvent<WiresSystem.WireToolCanceledEvent>(new EntityEventHandler<WiresSystem.WireToolCanceledEvent>(this.OnToolCanceled), null, null);
			base.SubscribeLocalEvent<WiresComponent, ComponentStartup>(new ComponentEventHandler<WiresComponent, ComponentStartup>(this.OnWiresStartup), null, null);
			base.SubscribeLocalEvent<WiresComponent, WiresActionMessage>(new ComponentEventHandler<WiresComponent, WiresActionMessage>(this.OnWiresActionMessage), null, null);
			base.SubscribeLocalEvent<WiresComponent, InteractUsingEvent>(new ComponentEventHandler<WiresComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<WiresComponent, ExaminedEvent>(new ComponentEventHandler<WiresComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<WiresComponent, MapInitEvent>(new ComponentEventHandler<WiresComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<WiresComponent, TimedWireEvent>(new ComponentEventHandler<WiresComponent, TimedWireEvent>(this.OnTimedWire), null, null);
			base.SubscribeLocalEvent<WiresComponent, PowerChangedEvent>(new ComponentEventRefHandler<WiresComponent, PowerChangedEvent>(this.OnWiresPowered), null, null);
			base.SubscribeLocalEvent<WiresComponent, DoAfterEvent<WiresSystem.WireExtraData>>(new ComponentEventHandler<WiresComponent, DoAfterEvent<WiresSystem.WireExtraData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008750 File Offset: 0x00006950
		[NullableContext(2)]
		private void SetOrCreateWireLayout(EntityUid uid, WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return;
			}
			WireLayout layout = null;
			List<Wire> wireSet = null;
			if (!wires.AlwaysRandomize)
			{
				this.TryGetLayout(wires.LayoutId, out layout);
			}
			List<IWireAction> wireActions = new List<IWireAction>();
			int dummyWires = 0;
			WireLayoutPrototype layoutPrototype;
			if (!this._protoMan.TryIndex<WireLayoutPrototype>(wires.LayoutId, ref layoutPrototype))
			{
				return;
			}
			dummyWires += layoutPrototype.DummyWires;
			if (layoutPrototype.Wires != null)
			{
				wireActions.AddRange(layoutPrototype.Wires);
			}
			foreach (WireLayoutPrototype parentLayout in this._protoMan.EnumerateParents<WireLayoutPrototype>(wires.LayoutId, false))
			{
				if (parentLayout.Wires != null)
				{
					wireActions.AddRange(parentLayout.Wires);
				}
				dummyWires += parentLayout.DummyWires;
			}
			if (wireActions.Count > 0)
			{
				foreach (IWireAction wireAction in wireActions)
				{
					wireAction.Initialize();
				}
				wireSet = this.CreateWireSet(uid, layout, wireActions, dummyWires);
			}
			if (wireSet == null || wireSet.Count == 0)
			{
				return;
			}
			wires.WiresList.AddRange(wireSet);
			Dictionary<object, int> types = new Dictionary<object, int>();
			if (layout != null)
			{
				for (int i = 0; i < wireSet.Count; i++)
				{
					wires.WiresList[layout.Specifications[i].Position] = wireSet[i];
				}
				int id = 0;
				using (List<Wire>.Enumerator enumerator3 = wires.WiresList.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						Wire wire = enumerator3.Current;
						wire.Id = id++;
						if (wire.Action != null)
						{
							Type wireType = wire.Action.GetType();
							if (types.ContainsKey(wireType))
							{
								Dictionary<object, int> dictionary = types;
								object key = wireType;
								dictionary[key]++;
							}
							else
							{
								types.Add(wireType, 1);
							}
							wire.Action.AddWire(wire, types[wireType]);
						}
					}
					return;
				}
			}
			List<ValueTuple<int, Wire>> enumeratedList = new List<ValueTuple<int, Wire>>();
			Dictionary<int, WireLayout.WireData> data = new Dictionary<int, WireLayout.WireData>();
			for (int j = 0; j < wireSet.Count; j++)
			{
				enumeratedList.Add(new ValueTuple<int, Wire>(j, wireSet[j]));
			}
			this._random.Shuffle<ValueTuple<int, Wire>>(enumeratedList);
			for (int k = 0; k < enumeratedList.Count; k++)
			{
				ValueTuple<int, Wire> valueTuple = enumeratedList[k];
				int id2 = valueTuple.Item1;
				Wire d = valueTuple.Item2;
				if (d.Action != null)
				{
					Type actionType = d.Action.GetType();
					if (types.ContainsKey(actionType))
					{
						Dictionary<object, int> dictionary = types;
						object key = actionType;
						dictionary[key]++;
					}
					else
					{
						types.Add(actionType, 1);
					}
					d.Id = k;
					if (!d.Action.AddWire(d, types[actionType]))
					{
						d.Action = null;
					}
				}
				data.Add(id2, new WireLayout.WireData(d.Letter, d.Color, k));
				wires.WiresList[k] = wireSet[id2];
			}
			if (!wires.AlwaysRandomize && !string.IsNullOrEmpty(wires.LayoutId))
			{
				this.AddLayout(wires.LayoutId, new WireLayout(data));
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008ADC File Offset: 0x00006CDC
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		private List<Wire> CreateWireSet(EntityUid uid, [Nullable(2)] WireLayout layout, List<IWireAction> wires, int dummyWires)
		{
			if (wires.Count == 0)
			{
				return null;
			}
			List<WireColor> colors = new List<WireColor>((WireColor[])Enum.GetValues(typeof(WireColor)));
			List<WireLetter> letters = new List<WireLetter>((WireLetter[])Enum.GetValues(typeof(WireLetter)));
			List<Wire> wireSet = new List<Wire>();
			for (int i = 0; i < wires.Count; i++)
			{
				wireSet.Add(this.CreateWire(uid, wires[i], i, layout, colors, letters));
			}
			for (int j = 1; j <= dummyWires; j++)
			{
				wireSet.Add(this.CreateWire(uid, null, wires.Count + j, layout, colors, letters));
			}
			return wireSet;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00008B84 File Offset: 0x00006D84
		private Wire CreateWire(EntityUid uid, [Nullable(2)] IWireAction action, int position, [Nullable(2)] WireLayout layout, List<WireColor> colors, List<WireLetter> letters)
		{
			WireLayout.WireData spec;
			WireColor color;
			WireLetter letter;
			if (layout != null && layout.Specifications.TryGetValue(position, out spec))
			{
				color = spec.Color;
				letter = spec.Letter;
				colors.Remove(color);
				letters.Remove(letter);
			}
			else
			{
				color = ((colors.Count == 0) ? WireColor.Red : RandomExtensions.PickAndTake<WireColor>(this._random, colors));
				letter = ((letters.Count == 0) ? WireLetter.α : RandomExtensions.PickAndTake<WireLetter>(this._random, letters));
			}
			return new Wire(uid, false, color, letter, position, action);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00008C07 File Offset: 0x00006E07
		private void OnWiresStartup(EntityUid uid, WiresComponent component, ComponentStartup args)
		{
			if (!string.IsNullOrEmpty(component.LayoutId))
			{
				this.SetOrCreateWireLayout(uid, component);
			}
			this.UpdateUserInterface(uid, null, null);
			this.UpdateAppearance(uid, null, null);
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008C30 File Offset: 0x00006E30
		private void OnTimedWire(EntityUid uid, WiresComponent component, TimedWireEvent args)
		{
			args.Delegate(args.Wire);
			this.UpdateUserInterface(uid, null, null);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008C4C File Offset: 0x00006E4C
		public bool TryCancelWireAction(EntityUid owner, object key)
		{
			CancellationTokenSource token;
			if (this.TryGetData<CancellationTokenSource>(owner, key, out token, null))
			{
				token.Cancel();
				return true;
			}
			return false;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008C70 File Offset: 0x00006E70
		public void StartWireAction(EntityUid owner, float delay, object key, TimedWireEvent onFinish)
		{
			if (!base.HasComp<WiresComponent>(owner))
			{
				return;
			}
			if (!this._activeWires.ContainsKey(owner))
			{
				this._activeWires.Add(owner, new List<WiresSystem.ActiveWireAction>());
			}
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			if (this.HasData(owner, key, null))
			{
				return;
			}
			this.SetData(owner, key, tokenSource, null);
			this._activeWires[owner].Add(new WiresSystem.ActiveWireAction(key, delay, tokenSource.Token, onFinish));
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00008CE4 File Offset: 0x00006EE4
		public override void Update(float frameTime)
		{
			foreach (KeyValuePair<EntityUid, List<WiresSystem.ActiveWireAction>> keyValuePair in this._activeWires)
			{
				EntityUid entityUid;
				List<WiresSystem.ActiveWireAction> list;
				keyValuePair.Deconstruct(out entityUid, out list);
				EntityUid owner = entityUid;
				List<WiresSystem.ActiveWireAction> list2 = list;
				if (!base.HasComp<WiresComponent>(owner))
				{
					this._activeWires.Remove(owner);
				}
				foreach (WiresSystem.ActiveWireAction wire in list2)
				{
					if (wire.CancelToken.IsCancellationRequested)
					{
						base.RaiseLocalEvent<TimedWireEvent>(owner, wire.OnFinish, true);
						this._finishedWires.Add(new ValueTuple<EntityUid, WiresSystem.ActiveWireAction>(owner, wire));
					}
					else
					{
						wire.TimeLeft -= frameTime;
						if (wire.TimeLeft <= 0f)
						{
							base.RaiseLocalEvent<TimedWireEvent>(owner, wire.OnFinish, true);
							this._finishedWires.Add(new ValueTuple<EntityUid, WiresSystem.ActiveWireAction>(owner, wire));
						}
					}
				}
			}
			if (this._finishedWires.Count != 0)
			{
				foreach (ValueTuple<EntityUid, WiresSystem.ActiveWireAction> valueTuple in this._finishedWires)
				{
					EntityUid owner2 = valueTuple.Item1;
					WiresSystem.ActiveWireAction wireAction = valueTuple.Item2;
					this._activeWires[owner2].RemoveAll((WiresSystem.ActiveWireAction action) => action.CancelToken == wireAction.CancelToken);
					if (this._activeWires[owner2].Count == 0)
					{
						this._activeWires.Remove(owner2);
					}
					this.RemoveData(owner2, wireAction.Id, null);
				}
				this._finishedWires.Clear();
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00008ED8 File Offset: 0x000070D8
		private void OnWiresPowered(EntityUid uid, WiresComponent component, ref PowerChangedEvent args)
		{
			this.UpdateUserInterface(uid, null, null);
			foreach (Wire wire in component.WiresList)
			{
				IWireAction action = wire.Action;
				if (action != null)
				{
					action.Update(wire);
				}
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00008F40 File Offset: 0x00007140
		private void OnWiresActionMessage(EntityUid uid, WiresComponent component, WiresActionMessage args)
		{
			if (args.Session.AttachedEntity == null)
			{
				return;
			}
			EntityUid player = args.Session.AttachedEntity.Value;
			HandsComponent handsComponent;
			if (!this.EntityManager.TryGetComponent<HandsComponent>(player, ref handsComponent))
			{
				this._popupSystem.PopupEntity(Loc.GetString("wires-component-ui-on-receive-message-no-hands"), uid, player, PopupType.Small);
				return;
			}
			if (!this._interactionSystem.InRangeUnobstructed(player, uid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				this._popupSystem.PopupEntity(Loc.GetString("wires-component-ui-on-receive-message-cannot-reach"), uid, player, PopupType.Small);
				return;
			}
			Hand activeHand = handsComponent.ActiveHand;
			if (activeHand == null)
			{
				return;
			}
			if (activeHand.HeldEntity == null)
			{
				return;
			}
			EntityUid activeHandEntity = activeHand.HeldEntity.Value;
			ToolComponent tool;
			if (!this.EntityManager.TryGetComponent<ToolComponent>(activeHandEntity, ref tool))
			{
				return;
			}
			this.TryDoWireAction(uid, player, activeHandEntity, args.Id, args.Action, component, tool);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000902C File Offset: 0x0000722C
		private void OnDoAfter(EntityUid uid, WiresComponent component, DoAfterEvent<WiresSystem.WireExtraData> args)
		{
			if (args.Cancelled)
			{
				component.WiresQueue.Remove(args.AdditionalData.Id);
				return;
			}
			if (args.Handled || args.Args.Target == null || args.Args.Used == null)
			{
				return;
			}
			this.UpdateWires(args.Args.Target.Value, args.Args.User, args.Args.Used.Value, args.AdditionalData.Id, args.AdditionalData.Action, component, null);
			args.Handled = true;
		}

		// Token: 0x06000192 RID: 402 RVA: 0x000090D8 File Offset: 0x000072D8
		private void OnInteractUsing(EntityUid uid, WiresComponent component, InteractUsingEvent args)
		{
			ToolComponent tool;
			if (!this.EntityManager.TryGetComponent<ToolComponent>(args.Used, ref tool))
			{
				return;
			}
			if (component.IsPanelOpen && (this._toolSystem.HasQuality(args.Used, "Cutting", tool) || this._toolSystem.HasQuality(args.Used, "Pulsing", tool)))
			{
				ActorComponent actor;
				if (this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
				{
					BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, WiresUiKey.Key, null);
					if (uiOrNull != null)
					{
						uiOrNull.Open(actor.PlayerSession);
					}
					args.Handled = true;
					return;
				}
			}
			else if (!component.IsScrewing && this._toolSystem.HasQuality(args.Used, "Screwing", tool))
			{
				ToolEventData toolEvData = new ToolEventData(new WiresSystem.WireToolFinishedEvent(uid, args.User), 0f, null, null);
				component.IsScrewing = this._toolSystem.UseTool(args.Used, args.User, new EntityUid?(uid), 1f, new string[]
				{
					"Screwing"
				}, toolEvData, 0f, tool, null, null);
				args.Handled = component.IsScrewing;
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(38, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
				logStringHandler.AppendLiteral(" is screwing ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "target", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral("'s ");
				logStringHandler.AppendFormatted(component.IsPanelOpen ? "open" : "closed");
				logStringHandler.AppendLiteral(" maintenance panel at ");
				logStringHandler.AppendFormatted<EntityCoordinates>(base.Transform(uid).Coordinates, "targetlocation", "Transform(uid).Coordinates");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x000092B8 File Offset: 0x000074B8
		private void OnToolFinished(WiresSystem.WireToolFinishedEvent args)
		{
			WiresComponent component;
			if (!this.EntityManager.TryGetComponent<WiresComponent>(args.Target, ref component))
			{
				return;
			}
			component.IsScrewing = false;
			component.IsPanelOpen = !component.IsPanelOpen;
			this.UpdateAppearance(args.Target, null, null);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(30, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "user", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" screwed ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
			logStringHandler.AppendLiteral("'s maintenance panel ");
			logStringHandler.AppendFormatted(component.IsPanelOpen ? "open" : "closed");
			adminLogger.Add(type, impact, ref logStringHandler);
			if (component.IsPanelOpen)
			{
				this._audio.PlayPvs(component.ScrewdriverOpenSound, args.Target, null);
				return;
			}
			this._audio.PlayPvs(component.ScrewdriverCloseSound, args.Target, null);
			BoundUserInterface ui = this._uiSystem.GetUiOrNull(args.Target, WiresUiKey.Key, null);
			if (ui != null)
			{
				this._uiSystem.CloseAll(ui);
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x000093FC File Offset: 0x000075FC
		private void OnToolCanceled(WiresSystem.WireToolCanceledEvent ev)
		{
			WiresComponent component;
			if (!base.TryComp<WiresComponent>(ev.Target, ref component))
			{
				return;
			}
			component.IsScrewing = false;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00009422 File Offset: 0x00007622
		private void OnExamine(EntityUid uid, WiresComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString(component.IsPanelOpen ? "wires-component-on-examine-panel-open" : "wires-component-on-examine-panel-closed"));
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00009443 File Offset: 0x00007643
		private void OnMapInit(EntityUid uid, WiresComponent component, MapInitEvent args)
		{
			if (component.SerialNumber == null)
			{
				this.GenerateSerialNumber(uid, component);
			}
			if (component.WireSeed == 0)
			{
				component.WireSeed = this._random.Next(1, int.MaxValue);
				this.UpdateUserInterface(uid, null, null);
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00009480 File Offset: 0x00007680
		[NullableContext(2)]
		private unsafe void GenerateSerialNumber(EntityUid uid, WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return;
			}
			Span<char> data = new Span<char>(stackalloc byte[(UIntPtr)18], 9);
			*data[4] = '-';
			if (RandomExtensions.Prob(this._random, 0.01f))
			{
				for (int i = 0; i < 4; i++)
				{
					*data[i] = (char)this._random.Next(1040, 1072);
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					*data[j] = (char)this._random.Next(65, 91);
				}
			}
			for (int k = 5; k < 9; k++)
			{
				*data[k] = (char)this._random.Next(48, 58);
			}
			wires.SerialNumber = new string(data);
			this.UpdateUserInterface(uid, null, null);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00009556 File Offset: 0x00007756
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, AppearanceComponent appearance = null, WiresComponent wires = null)
		{
			if (!base.Resolve<AppearanceComponent, WiresComponent>(uid, ref appearance, ref wires, false))
			{
				return;
			}
			this._appearance.SetData(uid, WiresVisuals.MaintenancePanelState, wires.IsPanelOpen && wires.IsPanelVisible, appearance);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00009590 File Offset: 0x00007790
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, WiresComponent wires = null, ServerUserInterfaceComponent ui = null)
		{
			if (!base.Resolve<WiresComponent, ServerUserInterfaceComponent>(uid, ref wires, ref ui, false))
			{
				return;
			}
			List<ClientWire> clientList = new List<ClientWire>();
			foreach (Wire entry in wires.WiresList)
			{
				clientList.Add(new ClientWire(entry.Id, entry.IsCut, entry.Color, entry.Letter));
				IWireAction action = entry.Action;
				StatusLightData? statusData = (action != null) ? action.GetStatusLightData(entry) : null;
				if (statusData != null)
				{
					IWireAction action2 = entry.Action;
					if (((action2 != null) ? action2.StatusKey : null) != null)
					{
						wires.Statuses[entry.Action.StatusKey] = new ValueTuple<int, StatusLightData?>(entry.OriginalPosition, statusData);
					}
				}
			}
			List<ValueTuple<int, object, object>> statuses = new List<ValueTuple<int, object, object>>();
			foreach (KeyValuePair<object, object> keyValuePair in wires.Statuses)
			{
				object obj;
				object obj2;
				keyValuePair.Deconstruct(out obj, out obj2);
				object key = obj;
				ValueTuple<int, StatusLightData?> valueCast = (ValueTuple<int, StatusLightData?>)obj2;
				statuses.Add(new ValueTuple<int, object, object>(valueCast.Item1, key, valueCast.Item2));
			}
			statuses.Sort(([TupleElementNames(new string[]
			{
				"position",
				"key",
				"value"
			})] [Nullable(new byte[]
			{
				0,
				1,
				1
			})] ValueTuple<int, object, object> a, [TupleElementNames(new string[]
			{
				"position",
				"key",
				"value"
			})] [Nullable(new byte[]
			{
				0,
				1,
				1
			})] ValueTuple<int, object, object> b) => a.Item1.CompareTo(b.Item1));
			this._uiSystem.TrySetUiState(uid, WiresUiKey.Key, new WiresBoundUserInterfaceState(clientList.ToArray(), (from p in statuses
			select new StatusEntry(p.Item2, p.Item3)).ToArray<StatusEntry>(), wires.BoardName, wires.SerialNumber, wires.WireSeed), null, ui, true);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000977C File Offset: 0x0000797C
		public void OpenUserInterface(EntityUid uid, IPlayerSession player)
		{
			BoundUserInterface ui;
			if (this._uiSystem.TryGetUi(uid, WiresUiKey.Key, ref ui, null))
			{
				this._uiSystem.OpenUi(ui, player);
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x000097AE File Offset: 0x000079AE
		[NullableContext(2)]
		public Wire TryGetWire(EntityUid uid, int id, WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return null;
			}
			if (id < 0 || id >= wires.WiresList.Count)
			{
				return null;
			}
			return wires.WiresList[id];
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000097DE File Offset: 0x000079DE
		[NullableContext(2)]
		[return: Nullable(1)]
		public IEnumerable<Wire> TryGetWires<T>(EntityUid uid, WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				yield break;
			}
			foreach (Wire wire in wires.WiresList)
			{
				if (wire.GetType() == typeof(T))
				{
					yield return wire;
				}
			}
			List<Wire>.Enumerator enumerator = default(List<Wire>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000097FC File Offset: 0x000079FC
		[NullableContext(2)]
		private void TryDoWireAction(EntityUid used, EntityUid user, EntityUid toolEntity, int id, WiresAction action, WiresComponent wires = null, ToolComponent tool = null)
		{
			if (!base.Resolve<WiresComponent>(used, ref wires, true) || !base.Resolve<ToolComponent>(toolEntity, ref tool, true))
			{
				return;
			}
			if (wires.WiresQueue.Contains(id))
			{
				return;
			}
			Wire wire = this.TryGetWire(used, id, wires);
			if (wire == null)
			{
				return;
			}
			switch (action)
			{
			case WiresAction.Mend:
				if (!this._toolSystem.HasQuality(toolEntity, "Cutting", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-wirecutters"), user, PopupType.Small);
					return;
				}
				if (!wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-mend-uncut-wire"), user, PopupType.Small);
					return;
				}
				break;
			case WiresAction.Cut:
				if (!this._toolSystem.HasQuality(toolEntity, "Cutting", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-wirecutters"), user, PopupType.Small);
					return;
				}
				if (wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-cut-cut-wire"), user, PopupType.Small);
					return;
				}
				break;
			case WiresAction.Pulse:
				if (!this._toolSystem.HasQuality(toolEntity, "Pulsing", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-multitool"), user, PopupType.Small);
					return;
				}
				if (wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-pulse-cut-wire"), user, PopupType.Small);
					return;
				}
				break;
			}
			wires.WiresQueue.Add(id);
			if (this._toolTime > 0f)
			{
				WiresSystem.WireExtraData data = new WiresSystem.WireExtraData(action, id);
				float toolTime = this._toolTime;
				EntityUid? target = new EntityUid?(used);
				EntityUid? used2 = new EntityUid?(toolEntity);
				DoAfterEventArgs args = new DoAfterEventArgs(user, toolTime, default(CancellationToken), target, used2)
				{
					NeedHand = true,
					BreakOnStun = true,
					BreakOnDamage = true,
					BreakOnUserMove = true
				};
				this._doAfter.DoAfter<WiresSystem.WireExtraData>(args, data);
				return;
			}
			this.UpdateWires(used, user, toolEntity, id, action, wires, null);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000099D0 File Offset: 0x00007BD0
		[NullableContext(2)]
		private void UpdateWires(EntityUid used, EntityUid user, EntityUid toolEntity, int id, WiresAction action, WiresComponent wires = null, ToolComponent tool = null)
		{
			if (!base.Resolve<WiresComponent>(used, ref wires, true))
			{
				return;
			}
			if (!wires.WiresQueue.Contains(id))
			{
				return;
			}
			if (!base.Resolve<ToolComponent>(toolEntity, ref tool, true))
			{
				wires.WiresQueue.Remove(id);
				return;
			}
			Wire wire = this.TryGetWire(used, id, wires);
			if (wire == null)
			{
				wires.WiresQueue.Remove(id);
				return;
			}
			switch (action)
			{
			case WiresAction.Mend:
				if (!this._toolSystem.HasQuality(toolEntity, "Cutting", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-wirecutters"), user, PopupType.Small);
				}
				else if (!wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-mend-uncut-wire"), user, PopupType.Small);
				}
				else
				{
					this._toolSystem.PlayToolSound(toolEntity, tool);
					if (wire.Action == null || wire.Action.Mend(user, wire))
					{
						wire.IsCut = false;
					}
					this.UpdateUserInterface(used, null, null);
				}
				break;
			case WiresAction.Cut:
				if (!this._toolSystem.HasQuality(toolEntity, "Cutting", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-wirecutters"), user, PopupType.Small);
				}
				else if (wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-cut-cut-wire"), user, PopupType.Small);
				}
				else
				{
					this._toolSystem.PlayToolSound(toolEntity, tool);
					if (wire.Action == null || wire.Action.Cut(user, wire))
					{
						wire.IsCut = true;
					}
					this.UpdateUserInterface(used, null, null);
				}
				break;
			case WiresAction.Pulse:
				if (!this._toolSystem.HasQuality(toolEntity, "Pulsing", tool))
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-need-multitool"), user, PopupType.Small);
				}
				else if (wire.IsCut)
				{
					this._popupSystem.PopupCursor(Loc.GetString("wires-component-ui-on-receive-message-cannot-pulse-cut-wire"), user, PopupType.Small);
				}
				else
				{
					IWireAction action2 = wire.Action;
					if (action2 != null)
					{
						action2.Pulse(user, wire);
					}
					this.UpdateUserInterface(used, null, null);
					this._audio.PlayPvs(wires.PulseSound, used, null);
				}
				break;
			}
			IWireAction action3 = wire.Action;
			if (action3 != null)
			{
				action3.Update(wire);
			}
			wires.WiresQueue.Remove(id);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00009C18 File Offset: 0x00007E18
		[NullableContext(2)]
		public bool TryGetData<T>(EntityUid uid, [Nullable(1)] object identifier, [NotNullWhen(true)] out T data, WiresComponent wires = null)
		{
			data = default(T);
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return false;
			}
			object result;
			wires.StateData.TryGetValue(identifier, out result);
			if (!(result is T))
			{
				return false;
			}
			data = (T)((object)result);
			return true;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00009C64 File Offset: 0x00007E64
		public void SetData(EntityUid uid, object identifier, object data, [Nullable(2)] WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return;
			}
			object storedMessage;
			if (wires.StateData.TryGetValue(identifier, out storedMessage) && storedMessage == data)
			{
				return;
			}
			wires.StateData[identifier] = data;
			this.UpdateUserInterface(uid, wires, null);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00009CAC File Offset: 0x00007EAC
		public bool HasData(EntityUid uid, object identifier, [Nullable(2)] WiresComponent wires = null)
		{
			return base.Resolve<WiresComponent>(uid, ref wires, true) && wires.StateData.ContainsKey(identifier);
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00009CC8 File Offset: 0x00007EC8
		public void RemoveData(EntityUid uid, object identifier, [Nullable(2)] WiresComponent wires = null)
		{
			if (!base.Resolve<WiresComponent>(uid, ref wires, true))
			{
				return;
			}
			wires.StateData.Remove(identifier);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00009CE4 File Offset: 0x00007EE4
		private bool TryGetLayout(string id, [Nullable(2)] [NotNullWhen(true)] out WireLayout layout)
		{
			return this._layouts.TryGetValue(id, out layout);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00009CF3 File Offset: 0x00007EF3
		private void AddLayout(string id, WireLayout layout)
		{
			this._layouts.Add(id, layout);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00009D02 File Offset: 0x00007F02
		private void Reset(RoundRestartCleanupEvent args)
		{
			this._layouts.Clear();
			this._random = new RobustRandom();
		}

		// Token: 0x0400012C RID: 300
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x0400012D RID: 301
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400012E RID: 302
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x0400012F RID: 303
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000130 RID: 304
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x04000131 RID: 305
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000132 RID: 306
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000133 RID: 307
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000134 RID: 308
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04000135 RID: 309
		private IRobustRandom _random = new RobustRandom();

		// Token: 0x04000136 RID: 310
		[ViewVariables]
		private readonly Dictionary<string, WireLayout> _layouts = new Dictionary<string, WireLayout>();

		// Token: 0x04000137 RID: 311
		private const float ScrewTime = 1f;

		// Token: 0x04000138 RID: 312
		private float _toolTime;

		// Token: 0x04000139 RID: 313
		private Dictionary<EntityUid, List<WiresSystem.ActiveWireAction>> _activeWires = new Dictionary<EntityUid, List<WiresSystem.ActiveWireAction>>();

		// Token: 0x0400013A RID: 314
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private List<ValueTuple<EntityUid, WiresSystem.ActiveWireAction>> _finishedWires = new List<ValueTuple<EntityUid, WiresSystem.ActiveWireAction>>();

		// Token: 0x02000892 RID: 2194
		[Nullable(0)]
		private class ActiveWireAction
		{
			// Token: 0x06002FB2 RID: 12210 RVA: 0x000F685C File Offset: 0x000F4A5C
			public ActiveWireAction(object identifier, float time, CancellationToken cancelToken, TimedWireEvent onFinish)
			{
				this.Id = identifier;
				this.TimeLeft = time;
				this.CancelToken = cancelToken;
				this.OnFinish = onFinish;
			}

			// Token: 0x04001CBE RID: 7358
			public object Id;

			// Token: 0x04001CBF RID: 7359
			public float TimeLeft;

			// Token: 0x04001CC0 RID: 7360
			public CancellationToken CancelToken;

			// Token: 0x04001CC1 RID: 7361
			public TimedWireEvent OnFinish;
		}

		// Token: 0x02000893 RID: 2195
		[NullableContext(0)]
		private struct WireExtraData : IEquatable<WiresSystem.WireExtraData>
		{
			// Token: 0x06002FB3 RID: 12211 RVA: 0x000F6881 File Offset: 0x000F4A81
			public WireExtraData(WiresAction Action, int Id)
			{
				this.Action = Action;
				this.Id = Id;
			}

			// Token: 0x06002FB4 RID: 12212 RVA: 0x000F6894 File Offset: 0x000F4A94
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("WireExtraData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06002FB5 RID: 12213 RVA: 0x000F68E0 File Offset: 0x000F4AE0
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Action = ");
				builder.Append(this.Action.ToString());
				builder.Append(", Id = ");
				builder.Append(this.Id.ToString());
				return true;
			}

			// Token: 0x06002FB6 RID: 12214 RVA: 0x000F6936 File Offset: 0x000F4B36
			[CompilerGenerated]
			public static bool operator !=(WiresSystem.WireExtraData left, WiresSystem.WireExtraData right)
			{
				return !(left == right);
			}

			// Token: 0x06002FB7 RID: 12215 RVA: 0x000F6942 File Offset: 0x000F4B42
			[CompilerGenerated]
			public static bool operator ==(WiresSystem.WireExtraData left, WiresSystem.WireExtraData right)
			{
				return left.Equals(right);
			}

			// Token: 0x06002FB8 RID: 12216 RVA: 0x000F694C File Offset: 0x000F4B4C
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<WiresAction>.Default.GetHashCode(this.Action) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Id);
			}

			// Token: 0x06002FB9 RID: 12217 RVA: 0x000F6975 File Offset: 0x000F4B75
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is WiresSystem.WireExtraData && this.Equals((WiresSystem.WireExtraData)obj);
			}

			// Token: 0x06002FBA RID: 12218 RVA: 0x000F698D File Offset: 0x000F4B8D
			[CompilerGenerated]
			public readonly bool Equals(WiresSystem.WireExtraData other)
			{
				return EqualityComparer<WiresAction>.Default.Equals(this.Action, other.Action) && EqualityComparer<int>.Default.Equals(this.Id, other.Id);
			}

			// Token: 0x06002FBB RID: 12219 RVA: 0x000F69BF File Offset: 0x000F4BBF
			[CompilerGenerated]
			public readonly void Deconstruct(out WiresAction Action, out int Id)
			{
				Action = this.Action;
				Id = this.Id;
			}

			// Token: 0x04001CC2 RID: 7362
			public WiresAction Action;

			// Token: 0x04001CC3 RID: 7363
			public int Id;
		}

		// Token: 0x02000894 RID: 2196
		[NullableContext(0)]
		private sealed class WireToolFinishedEvent : EntityEventArgs
		{
			// Token: 0x170007E3 RID: 2019
			// (get) Token: 0x06002FBC RID: 12220 RVA: 0x000F69D1 File Offset: 0x000F4BD1
			public EntityUid User { get; }

			// Token: 0x170007E4 RID: 2020
			// (get) Token: 0x06002FBD RID: 12221 RVA: 0x000F69D9 File Offset: 0x000F4BD9
			public EntityUid Target { get; }

			// Token: 0x06002FBE RID: 12222 RVA: 0x000F69E1 File Offset: 0x000F4BE1
			public WireToolFinishedEvent(EntityUid target, EntityUid user)
			{
				this.Target = target;
				this.User = user;
			}
		}

		// Token: 0x02000895 RID: 2197
		[NullableContext(0)]
		public struct WireToolCanceledEvent : IEquatable<WiresSystem.WireToolCanceledEvent>
		{
			// Token: 0x06002FBF RID: 12223 RVA: 0x000F69F7 File Offset: 0x000F4BF7
			public WireToolCanceledEvent(EntityUid Target)
			{
				this.Target = Target;
			}

			// Token: 0x170007E5 RID: 2021
			// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x000F6A00 File Offset: 0x000F4C00
			// (set) Token: 0x06002FC1 RID: 12225 RVA: 0x000F6A08 File Offset: 0x000F4C08
			public EntityUid Target { readonly get; set; }

			// Token: 0x06002FC2 RID: 12226 RVA: 0x000F6A14 File Offset: 0x000F4C14
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("WireToolCanceledEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06002FC3 RID: 12227 RVA: 0x000F6A60 File Offset: 0x000F4C60
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Target = ");
				builder.Append(this.Target.ToString());
				return true;
			}

			// Token: 0x06002FC4 RID: 12228 RVA: 0x000F6A95 File Offset: 0x000F4C95
			[CompilerGenerated]
			public static bool operator !=(WiresSystem.WireToolCanceledEvent left, WiresSystem.WireToolCanceledEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06002FC5 RID: 12229 RVA: 0x000F6AA1 File Offset: 0x000F4CA1
			[CompilerGenerated]
			public static bool operator ==(WiresSystem.WireToolCanceledEvent left, WiresSystem.WireToolCanceledEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06002FC6 RID: 12230 RVA: 0x000F6AAB File Offset: 0x000F4CAB
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<EntityUid>.Default.GetHashCode(this.<Target>k__BackingField);
			}

			// Token: 0x06002FC7 RID: 12231 RVA: 0x000F6ABD File Offset: 0x000F4CBD
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is WiresSystem.WireToolCanceledEvent && this.Equals((WiresSystem.WireToolCanceledEvent)obj);
			}

			// Token: 0x06002FC8 RID: 12232 RVA: 0x000F6AD5 File Offset: 0x000F4CD5
			[CompilerGenerated]
			public readonly bool Equals(WiresSystem.WireToolCanceledEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Target>k__BackingField, other.<Target>k__BackingField);
			}

			// Token: 0x06002FC9 RID: 12233 RVA: 0x000F6AED File Offset: 0x000F4CED
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Target)
			{
				Target = this.Target;
			}
		}
	}
}
