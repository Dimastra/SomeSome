using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Mech.Components;
using Content.Server.Power.Components;
using Content.Server.Wires;
using Content.Shared.ActionBlocker;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Content.Shared.Movement.Events;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Mech.Systems
{
	// Token: 0x020003C6 RID: 966
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechSystem : SharedMechSystem
	{
		// Token: 0x060013D7 RID: 5079 RVA: 0x00066F30 File Offset: 0x00065130
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("mech");
			base.SubscribeLocalEvent<MechComponent, InteractUsingEvent>(new ComponentEventHandler<MechComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<MechComponent, MapInitEvent>(new ComponentEventHandler<MechComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<MechComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<MechComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAlternativeVerb), null, null);
			base.SubscribeLocalEvent<MechComponent, MechOpenUiEvent>(new ComponentEventHandler<MechComponent, MechOpenUiEvent>(this.OnOpenUi), null, null);
			base.SubscribeLocalEvent<MechComponent, DoAfterEvent<MechSystem.RemoveBatteryEvent>>(new ComponentEventHandler<MechComponent, DoAfterEvent<MechSystem.RemoveBatteryEvent>>(this.OnRemoveBattery), null, null);
			base.SubscribeLocalEvent<MechComponent, DoAfterEvent<MechSystem.MechEntryEvent>>(new ComponentEventHandler<MechComponent, DoAfterEvent<MechSystem.MechEntryEvent>>(this.OnMechEntry), null, null);
			base.SubscribeLocalEvent<MechComponent, DoAfterEvent<MechSystem.MechExitEvent>>(new ComponentEventHandler<MechComponent, DoAfterEvent<MechSystem.MechExitEvent>>(this.OnMechExit), null, null);
			base.SubscribeLocalEvent<MechComponent, DamageChangedEvent>(new ComponentEventHandler<MechComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
			base.SubscribeLocalEvent<MechComponent, MechEquipmentRemoveMessage>(new ComponentEventHandler<MechComponent, MechEquipmentRemoveMessage>(this.OnRemoveEquipmentMessage), null, null);
			base.SubscribeLocalEvent<MechComponent, UpdateCanMoveEvent>(new ComponentEventHandler<MechComponent, UpdateCanMoveEvent>(this.OnMechCanMoveEvent), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, ToolUserAttemptUseEvent>(new ComponentEventRefHandler<MechPilotComponent, ToolUserAttemptUseEvent>(this.OnToolUseAttempt), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, InhaleLocationEvent>(new ComponentEventHandler<MechPilotComponent, InhaleLocationEvent>(this.OnInhale), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, ExhaleLocationEvent>(new ComponentEventHandler<MechPilotComponent, ExhaleLocationEvent>(this.OnExhale), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, AtmosExposedGetAirEvent>(new ComponentEventRefHandler<MechPilotComponent, AtmosExposedGetAirEvent>(this.OnExpose), null, null);
			base.SubscribeLocalEvent<MechComponent, MechGrabberEjectMessage>(new ComponentEventHandler<MechComponent, MechGrabberEjectMessage>(this.RecieveEquipmentUiMesssages<MechGrabberEjectMessage>), null, null);
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0006707F File Offset: 0x0006527F
		private void OnMechCanMoveEvent(EntityUid uid, MechComponent component, UpdateCanMoveEvent args)
		{
			if (component.Broken || component.Integrity <= 0 || component.Energy <= 0)
			{
				args.Cancel();
			}
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x000670AC File Offset: 0x000652AC
		private void OnInteractUsing(EntityUid uid, MechComponent component, InteractUsingEvent args)
		{
			WiresComponent wires;
			if (base.TryComp<WiresComponent>(uid, ref wires) && !wires.IsPanelOpen)
			{
				return;
			}
			BatteryComponent battery;
			if (component.BatterySlot.ContainedEntity == null && base.TryComp<BatteryComponent>(args.Used, ref battery))
			{
				this.InsertBattery(uid, args.Used, component, battery);
				this._actionBlocker.UpdateCanMove(uid, null);
				return;
			}
			ToolComponent tool;
			if (base.TryComp<ToolComponent>(args.Used, ref tool) && tool.Qualities.Contains("Prying"))
			{
				EntityUid? containedEntity = component.BatterySlot.ContainedEntity;
				if (containedEntity != null)
				{
					MechSystem.RemoveBatteryEvent removeBattery = new MechSystem.RemoveBatteryEvent();
					EntityUid user = args.User;
					float batteryRemovalDelay = component.BatteryRemovalDelay;
					containedEntity = new EntityUid?(uid);
					EntityUid? used = new EntityUid?(args.Target);
					DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, batteryRemovalDelay, default(CancellationToken), containedEntity, used)
					{
						BreakOnTargetMove = true,
						BreakOnUserMove = true
					};
					this._doAfter.DoAfter<MechSystem.RemoveBatteryEvent>(doAfterEventArgs, removeBattery);
				}
			}
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x000671A3 File Offset: 0x000653A3
		private void OnRemoveBattery(EntityUid uid, MechComponent component, DoAfterEvent<MechSystem.RemoveBatteryEvent> args)
		{
			if (args.Cancelled || args.Handled)
			{
				return;
			}
			this.RemoveBattery(uid, component);
			this._actionBlocker.UpdateCanMove(uid, null);
			args.Handled = true;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x000671D4 File Offset: 0x000653D4
		private void OnMapInit(EntityUid uid, MechComponent component, MapInitEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			IEnumerable<string> startingEquipment = component.StartingEquipment;
			Func<string, EntityUid> <>9__0;
			Func<string, EntityUid> selector;
			if ((selector = <>9__0) == null)
			{
				selector = (<>9__0 = ((string equipment) => this.Spawn(equipment, xform.Coordinates)));
			}
			foreach (EntityUid ent in startingEquipment.Select(selector))
			{
				base.InsertEquipment(uid, ent, component, null);
			}
			component.Integrity = component.MaxIntegrity;
			component.Energy = component.MaxEnergy;
			if (component.StartingBattery != null)
			{
				EntityUid battery = base.Spawn(component.StartingBattery, base.Transform(uid).Coordinates);
				this.InsertBattery(uid, battery, component, null);
			}
			this._actionBlocker.UpdateCanMove(uid, null);
			base.Dirty(component, null);
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x000672C0 File Offset: 0x000654C0
		private void OnRemoveEquipmentMessage(EntityUid uid, SharedMechComponent component, MechEquipmentRemoveMessage args)
		{
			if (!base.Exists(args.Equipment) || base.Deleted(args.Equipment, null))
			{
				return;
			}
			if (!component.EquipmentContainer.ContainedEntities.Contains(args.Equipment))
			{
				return;
			}
			base.RemoveEquipment(uid, args.Equipment, component, null, false);
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x00067314 File Offset: 0x00065514
		private void OnOpenUi(EntityUid uid, MechComponent component, MechOpenUiEvent args)
		{
			args.Handled = true;
			this.ToggleMechUi(uid, component, null);
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x0006733C File Offset: 0x0006553C
		private void OnToolUseAttempt(EntityUid uid, MechPilotComponent component, ref ToolUserAttemptUseEvent args)
		{
			EntityUid? target = args.Target;
			EntityUid mech = component.Mech;
			if (target != null && (target == null || target.GetValueOrDefault() == mech))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x00067388 File Offset: 0x00065588
		private void OnAlternativeVerb(EntityUid uid, MechComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || component.Broken)
			{
				return;
			}
			if (base.CanInsert(uid, args.User, component))
			{
				AlternativeVerb enterVerb = new AlternativeVerb
				{
					Text = Loc.GetString("mech-verb-enter"),
					Act = delegate()
					{
						MechSystem.MechEntryEvent mechEntryEvent = new MechSystem.MechEntryEvent();
						EntityUid user = args.User;
						float entryDelay = component.EntryDelay;
						EntityUid? target = new EntityUid?(uid);
						DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, entryDelay, default(CancellationToken), target, null)
						{
							BreakOnUserMove = true,
							BreakOnStun = true
						};
						this._doAfter.DoAfter<MechSystem.MechEntryEvent>(doAfterEventArgs, mechEntryEvent);
					}
				};
				AlternativeVerb openUiVerb = new AlternativeVerb
				{
					Act = delegate()
					{
						this.ToggleMechUi(uid, component, new EntityUid?(args.User));
					},
					Text = Loc.GetString("mech-ui-open-verb")
				};
				args.Verbs.Add(enterVerb);
				args.Verbs.Add(openUiVerb);
				return;
			}
			if (!base.IsEmpty(component))
			{
				AlternativeVerb ejectVerb = new AlternativeVerb
				{
					Text = Loc.GetString("mech-verb-exit"),
					Priority = 1,
					Act = delegate()
					{
						EntityUid user = args.User;
						EntityUid? containedEntity = component.PilotSlot.ContainedEntity;
						if (user == containedEntity)
						{
							this.TryEject(uid, component);
							return;
						}
						MechSystem.MechExitEvent mechExitEvent = new MechSystem.MechExitEvent();
						EntityUid user2 = args.User;
						float exitDelay = component.ExitDelay;
						containedEntity = new EntityUid?(uid);
						DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user2, exitDelay, default(CancellationToken), containedEntity, null)
						{
							BreakOnUserMove = true,
							BreakOnTargetMove = true,
							BreakOnStun = true
						};
						this._doAfter.DoAfter<MechSystem.MechExitEvent>(doAfterEventArgs, mechExitEvent);
					}
				};
				args.Verbs.Add(ejectVerb);
			}
		}

		// Token: 0x060013E0 RID: 5088 RVA: 0x000674C4 File Offset: 0x000656C4
		private void OnMechEntry(EntityUid uid, MechComponent component, DoAfterEvent<MechSystem.MechEntryEvent> args)
		{
			if (args.Cancelled || args.Handled)
			{
				return;
			}
			this.TryInsert(uid, new EntityUid?(args.Args.User), component);
			this._actionBlocker.UpdateCanMove(uid, null);
			args.Handled = true;
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x00067510 File Offset: 0x00065710
		private void OnMechExit(EntityUid uid, MechComponent component, DoAfterEvent<MechSystem.MechExitEvent> args)
		{
			if (args.Cancelled || args.Handled)
			{
				return;
			}
			this.TryEject(uid, component);
			args.Handled = true;
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x00067534 File Offset: 0x00065734
		private void OnDamageChanged(EntityUid uid, SharedMechComponent component, DamageChangedEvent args)
		{
			FixedPoint2 integrity = component.MaxIntegrity - args.Damageable.TotalDamage;
			base.SetIntegrity(uid, integrity, component);
			if (args.DamageIncreased && args.DamageDelta != null && component.PilotSlot.ContainedEntity != null)
			{
				DamageSpecifier damage = args.DamageDelta * component.MechToPilotDamageMultiplier;
				this._damageable.TryChangeDamage(component.PilotSlot.ContainedEntity, damage, false, true, null, null);
			}
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x000675BC File Offset: 0x000657BC
		[NullableContext(2)]
		private void ToggleMechUi(EntityUid uid, MechComponent component = null, EntityUid? user = null)
		{
			if (!base.Resolve<MechComponent>(uid, ref component, true))
			{
				return;
			}
			EntityUid? entityUid = user;
			if (entityUid == null)
			{
				user = component.PilotSlot.ContainedEntity;
			}
			if (user == null)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(user, ref actor))
			{
				return;
			}
			this._ui.TryToggleUi(uid, MechUiKey.Key, actor.PlayerSession, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x00067628 File Offset: 0x00065828
		private void RecieveEquipmentUiMesssages<[Nullable(0)] T>(EntityUid uid, MechComponent component, T args) where T : MechEquipmentUiMessage
		{
			MechEquipmentUiMessageRelayEvent ev = new MechEquipmentUiMessageRelayEvent(args);
			foreach (EntityUid equipment in new List<EntityUid>(component.EquipmentContainer.ContainedEntities))
			{
				if (args.Equipment == equipment)
				{
					base.RaiseLocalEvent<MechEquipmentUiMessageRelayEvent>(equipment, ev, false);
				}
			}
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x000676A8 File Offset: 0x000658A8
		[NullableContext(2)]
		public override void UpdateUserInterface(EntityUid uid, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return;
			}
			base.UpdateUserInterface(uid, component);
			MechEquipmentUiStateReadyEvent ev = new MechEquipmentUiStateReadyEvent();
			foreach (EntityUid ent in component.EquipmentContainer.ContainedEntities)
			{
				base.RaiseLocalEvent<MechEquipmentUiStateReadyEvent>(ent, ev, false);
			}
			MechBoundUiState state = new MechBoundUiState
			{
				EquipmentStates = ev.States
			};
			BoundUserInterface ui = this._ui.GetUi(uid, MechUiKey.Key, null);
			this._ui.SetUiState(ui, state, null, true);
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x00067750 File Offset: 0x00065950
		[NullableContext(2)]
		public override bool TryInsert(EntityUid uid, EntityUid? toInsert, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.TryInsert(uid, toInsert, component))
			{
				return false;
			}
			MechComponent mech = (MechComponent)component;
			if (mech.Airtight)
			{
				MapCoordinates coordinates = base.Transform(uid).MapPosition;
				MapGridComponent grid;
				if (this._map.TryFindGridAt(coordinates, ref grid))
				{
					TileRef tile = grid.GetTileRef(coordinates);
					GasMixture environment = this._atmosphere.GetTileMixture(new EntityUid?(tile.GridUid), null, tile.GridIndices, true);
					if (environment != null)
					{
						this._atmosphere.Merge(mech.Air, environment.RemoveVolume(70f));
					}
				}
			}
			return true;
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x000677F8 File Offset: 0x000659F8
		[NullableContext(2)]
		public override bool TryEject(EntityUid uid, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.TryEject(uid, component))
			{
				return false;
			}
			MechComponent mech = (MechComponent)component;
			if (mech.Airtight)
			{
				MapCoordinates coordinates = base.Transform(uid).MapPosition;
				MapGridComponent grid;
				if (this._map.TryFindGridAt(coordinates, ref grid))
				{
					TileRef tile = grid.GetTileRef(coordinates);
					GasMixture environment = this._atmosphere.GetTileMixture(new EntityUid?(tile.GridUid), null, tile.GridIndices, true);
					if (environment != null)
					{
						this._atmosphere.Merge(environment, mech.Air);
						mech.Air.Clear();
					}
				}
			}
			return true;
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0006789F File Offset: 0x00065A9F
		[NullableContext(2)]
		public override void BreakMech(EntityUid uid, SharedMechComponent component = null)
		{
			base.BreakMech(uid, component);
			this._ui.TryCloseAll(uid, MechUiKey.Key, null);
			this._actionBlocker.UpdateCanMove(uid, null);
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x000678CC File Offset: 0x00065ACC
		[NullableContext(2)]
		public override bool TryChangeEnergy(EntityUid uid, FixedPoint2 delta, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!base.TryChangeEnergy(uid, delta, component))
			{
				return false;
			}
			EntityUid? battery = component.BatterySlot.ContainedEntity;
			if (battery == null)
			{
				return false;
			}
			BatteryComponent batteryComp;
			if (!base.TryComp<BatteryComponent>(battery, ref batteryComp))
			{
				return false;
			}
			batteryComp.CurrentCharge += delta.Float();
			if (batteryComp.CurrentCharge != component.Energy)
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Battery charge was not equal to mech charge. Battery ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(batteryComp.CurrentCharge);
				defaultInterpolatedStringHandler.AppendLiteral(". Mech ");
				defaultInterpolatedStringHandler.AppendFormatted<FixedPoint2>(component.Energy);
				sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				component.Energy = batteryComp.CurrentCharge;
				base.Dirty(component, null);
			}
			this._actionBlocker.UpdateCanMove(uid, null);
			return true;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x000679BC File Offset: 0x00065BBC
		[NullableContext(2)]
		public void InsertBattery(EntityUid uid, EntityUid toInsert, MechComponent component = null, BatteryComponent battery = null)
		{
			if (!base.Resolve<MechComponent>(uid, ref component, false))
			{
				return;
			}
			if (!base.Resolve<BatteryComponent>(toInsert, ref battery, false))
			{
				return;
			}
			component.BatterySlot.Insert(toInsert, null, null, null, null, null);
			component.Energy = battery.CurrentCharge;
			component.MaxEnergy = battery.MaxCharge;
			this._actionBlocker.UpdateCanMove(uid, null);
			base.Dirty(component, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x00067A38 File Offset: 0x00065C38
		[NullableContext(2)]
		public void RemoveBattery(EntityUid uid, MechComponent component = null)
		{
			if (!base.Resolve<MechComponent>(uid, ref component, true))
			{
				return;
			}
			this._container.EmptyContainer(component.BatterySlot, false, null, false, null);
			component.Energy = 0;
			component.MaxEnergy = 0;
			this._actionBlocker.UpdateCanMove(uid, null);
			base.Dirty(component, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x00067AA8 File Offset: 0x00065CA8
		private void OnInhale(EntityUid uid, MechPilotComponent component, InhaleLocationEvent args)
		{
			MechComponent mech;
			if (!base.TryComp<MechComponent>(component.Mech, ref mech))
			{
				return;
			}
			if (mech.Airtight)
			{
				args.Gas = mech.Air;
			}
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x00067ADC File Offset: 0x00065CDC
		private void OnExhale(EntityUid uid, MechPilotComponent component, ExhaleLocationEvent args)
		{
			MechComponent mech;
			if (!base.TryComp<MechComponent>(component.Mech, ref mech))
			{
				return;
			}
			if (mech.Airtight)
			{
				args.Gas = mech.Air;
			}
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x00067B10 File Offset: 0x00065D10
		private void OnExpose(EntityUid uid, MechPilotComponent component, ref AtmosExposedGetAirEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			MechComponent mech;
			if (!base.TryComp<MechComponent>(component.Mech, ref mech))
			{
				return;
			}
			args.Gas = (mech.Airtight ? mech.Air : this._atmosphere.GetContainingMixture(component.Mech, false, false, null));
			args.Handled = true;
		}

		// Token: 0x04000C46 RID: 3142
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x04000C47 RID: 3143
		[Dependency]
		private readonly ContainerSystem _container;

		// Token: 0x04000C48 RID: 3144
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x04000C49 RID: 3145
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000C4A RID: 3146
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x04000C4B RID: 3147
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x04000C4C RID: 3148
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04000C4D RID: 3149
		private ISawmill _sawmill;

		// Token: 0x020009A9 RID: 2473
		[NullableContext(0)]
		private sealed class RemoveBatteryEvent : EntityEventArgs
		{
		}

		// Token: 0x020009AA RID: 2474
		[NullableContext(0)]
		private sealed class MechEntryEvent : EntityEventArgs
		{
		}

		// Token: 0x020009AB RID: 2475
		[NullableContext(0)]
		private sealed class MechExitEvent : EntityEventArgs
		{
		}
	}
}
