using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Doors.Components;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Shuttles.Components;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Doors.Systems
{
	// Token: 0x02000548 RID: 1352
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FirelockSystem : EntitySystem
	{
		// Token: 0x06001C5A RID: 7258 RVA: 0x00096F1C File Offset: 0x0009511C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FirelockComponent, BeforeDoorOpenedEvent>(new ComponentEventHandler<FirelockComponent, BeforeDoorOpenedEvent>(this.OnBeforeDoorOpened), null, null);
			base.SubscribeLocalEvent<FirelockComponent, DoorGetPryTimeModifierEvent>(new ComponentEventHandler<FirelockComponent, DoorGetPryTimeModifierEvent>(this.OnDoorGetPryTimeModifier), null, null);
			base.SubscribeLocalEvent<FirelockComponent, DoorStateChangedEvent>(new ComponentEventHandler<FirelockComponent, DoorStateChangedEvent>(this.OnUpdateState), null, null);
			base.SubscribeLocalEvent<FirelockComponent, BeforeDoorAutoCloseEvent>(new ComponentEventHandler<FirelockComponent, BeforeDoorAutoCloseEvent>(this.OnBeforeDoorAutoclose), null, null);
			base.SubscribeLocalEvent<FirelockComponent, AtmosAlarmEvent>(new ComponentEventHandler<FirelockComponent, AtmosAlarmEvent>(this.OnAtmosAlarm), null, null);
			base.SubscribeLocalEvent<FirelockComponent, MapInitEvent>(new ComponentEventHandler<FirelockComponent, MapInitEvent>(this.UpdateVisuals), null, null);
			base.SubscribeLocalEvent<FirelockComponent, ComponentStartup>(new ComponentEventHandler<FirelockComponent, ComponentStartup>(this.UpdateVisuals), null, null);
			base.SubscribeLocalEvent<FirelockComponent, PowerChangedEvent>(new ComponentEventRefHandler<FirelockComponent, PowerChangedEvent>(this.PowerChanged), null, null);
		}

		// Token: 0x06001C5B RID: 7259 RVA: 0x00096FCF File Offset: 0x000951CF
		private void PowerChanged(EntityUid uid, FirelockComponent component, ref PowerChangedEvent args)
		{
			this._appearance.SetData(uid, DoorVisuals.Powered, args.Powered, null);
		}

		// Token: 0x06001C5C RID: 7260 RVA: 0x00096FEF File Offset: 0x000951EF
		private void UpdateVisuals(EntityUid uid, FirelockComponent component, EntityEventArgs args)
		{
			this.UpdateVisuals(uid, component, null, null, null, null);
		}

		// Token: 0x06001C5D RID: 7261 RVA: 0x00097000 File Offset: 0x00095200
		public override void Update(float frameTime)
		{
			this._accumulatedFrameTime += frameTime;
			if (this._accumulatedFrameTime < FirelockSystem._visualUpdateInterval)
			{
				return;
			}
			this._accumulatedFrameTime -= FirelockSystem._visualUpdateInterval;
			EntityQuery<AirtightComponent> airtightQuery = base.GetEntityQuery<AirtightComponent>();
			EntityQuery<AppearanceComponent> appearanceQuery = base.GetEntityQuery<AppearanceComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			foreach (ValueTuple<FirelockComponent, DoorComponent> valueTuple in base.EntityQuery<FirelockComponent, DoorComponent>(false))
			{
				FirelockComponent firelock = valueTuple.Item1;
				DoorComponent door = valueTuple.Item2;
				if (door.State == DoorState.Closed || door.State == DoorState.Welded || door.State == DoorState.Denying)
				{
					EntityUid uid = door.Owner;
					AirtightComponent airtight;
					TransformComponent xform;
					AppearanceComponent appearance;
					if (airtightQuery.TryGetComponent(uid, ref airtight) && xformQuery.TryGetComponent(uid, ref xform) && appearanceQuery.TryGetComponent(uid, ref appearance))
					{
						ValueTuple<bool, bool> valueTuple2 = this.CheckPressureAndFire(uid, firelock, xform, airtight, airtightQuery);
						bool fire = valueTuple2.Item1;
						bool pressure = valueTuple2.Item2;
						this._appearance.SetData(uid, DoorVisuals.ClosedLights, fire || pressure, appearance);
					}
				}
			}
		}

		// Token: 0x06001C5E RID: 7262 RVA: 0x00097128 File Offset: 0x00095328
		[NullableContext(2)]
		private void UpdateVisuals(EntityUid uid, FirelockComponent firelock = null, DoorComponent door = null, AirtightComponent airtight = null, AppearanceComponent appearance = null, TransformComponent xform = null)
		{
			if (!base.Resolve<DoorComponent, AppearanceComponent>(uid, ref door, ref appearance, false))
			{
				return;
			}
			if (door.State != DoorState.Closed && door.State != DoorState.Welded && door.State != DoorState.Denying)
			{
				this._appearance.SetData(uid, DoorVisuals.ClosedLights, false, appearance);
				return;
			}
			EntityQuery<AirtightComponent> query = base.GetEntityQuery<AirtightComponent>();
			if (!base.Resolve<FirelockComponent, AirtightComponent, AppearanceComponent, TransformComponent>(uid, ref firelock, ref airtight, ref appearance, ref xform, false) || !query.Resolve(uid, ref airtight, false))
			{
				return;
			}
			ValueTuple<bool, bool> valueTuple = this.CheckPressureAndFire(uid, firelock, xform, airtight, query);
			bool fire = valueTuple.Item1;
			bool pressure = valueTuple.Item2;
			this._appearance.SetData(uid, DoorVisuals.ClosedLights, fire || pressure, appearance);
		}

		// Token: 0x06001C5F RID: 7263 RVA: 0x000971D8 File Offset: 0x000953D8
		[NullableContext(2)]
		public bool EmergencyPressureStop(EntityUid uid, FirelockComponent firelock = null, DoorComponent door = null)
		{
			return base.Resolve<FirelockComponent, DoorComponent>(uid, ref firelock, ref door, true) && (door.State == DoorState.Open && this._doorSystem.TryClose(door.Owner, door, null, false)) && this._doorSystem.OnPartialClose(door.Owner, door, null);
		}

		// Token: 0x06001C60 RID: 7264 RVA: 0x00097231 File Offset: 0x00095431
		private void OnBeforeDoorOpened(EntityUid uid, FirelockComponent component, BeforeDoorOpenedEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null) || this.IsHoldingPressureOrFire(uid, component))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001C61 RID: 7265 RVA: 0x00097254 File Offset: 0x00095454
		private void OnDoorGetPryTimeModifier(EntityUid uid, FirelockComponent component, DoorGetPryTimeModifierEvent args)
		{
			ValueTuple<bool, bool> state = this.CheckPressureAndFire(uid, component);
			if (state.Item2)
			{
				this._popupSystem.PopupEntity(Loc.GetString("firelock-component-is-holding-fire-message"), uid, args.User, PopupType.MediumCaution);
			}
			else if (state.Item1)
			{
				this._popupSystem.PopupEntity(Loc.GetString("firelock-component-is-holding-pressure-message"), uid, args.User, PopupType.MediumCaution);
			}
			if (state.Item2 || state.Item1)
			{
				args.PryTimeModifier *= component.LockedPryTimeModifier;
			}
		}

		// Token: 0x06001C62 RID: 7266 RVA: 0x000972DC File Offset: 0x000954DC
		private void OnUpdateState(EntityUid uid, FirelockComponent component, DoorStateChangedEvent args)
		{
			BeforeDoorAutoCloseEvent ev = new BeforeDoorAutoCloseEvent();
			base.RaiseLocalEvent<BeforeDoorAutoCloseEvent>(uid, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			this._doorSystem.SetNextStateChange(uid, new TimeSpan?(component.AutocloseDelay), null);
			this.UpdateVisuals(uid, component, args);
		}

		// Token: 0x06001C63 RID: 7267 RVA: 0x00097324 File Offset: 0x00095524
		private void OnBeforeDoorAutoclose(EntityUid uid, FirelockComponent component, BeforeDoorAutoCloseEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				args.Cancel();
			}
			if (component.AlarmAutoClose)
			{
				AtmosAlarmType? alarm;
				if (this._atmosAlarmable.TryGetHighestAlert(uid, out alarm, null))
				{
					AtmosAlarmType? atmosAlarmType = alarm;
					AtmosAlarmType atmosAlarmType2 = AtmosAlarmType.Danger;
					if (!(atmosAlarmType.GetValueOrDefault() == atmosAlarmType2 & atmosAlarmType != null))
					{
						goto IL_50;
					}
				}
				if (alarm != null)
				{
					return;
				}
				IL_50:
				args.Cancel();
			}
		}

		// Token: 0x06001C64 RID: 7268 RVA: 0x00097388 File Offset: 0x00095588
		private void OnAtmosAlarm(EntityUid uid, FirelockComponent component, AtmosAlarmEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			DoorComponent doorComponent;
			if (!base.TryComp<DoorComponent>(uid, ref doorComponent))
			{
				return;
			}
			if (args.AlarmType == AtmosAlarmType.Normal || args.AlarmType == AtmosAlarmType.Warning)
			{
				if (doorComponent.State == DoorState.Closed)
				{
					this._doorSystem.TryOpen(uid, null, null, false, false);
					return;
				}
			}
			else if (args.AlarmType == AtmosAlarmType.Danger)
			{
				this.EmergencyPressureStop(uid, component, doorComponent);
			}
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x000973FC File Offset: 0x000955FC
		public bool IsHoldingPressureOrFire(EntityUid uid, FirelockComponent firelock)
		{
			ValueTuple<bool, bool> result = this.CheckPressureAndFire(uid, firelock);
			return result.Item1 || result.Item2;
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x00097424 File Offset: 0x00095624
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"Pressure",
			"Fire"
		})]
		public ValueTuple<bool, bool> CheckPressureAndFire(EntityUid uid, [Nullable(1)] FirelockComponent firelock)
		{
			EntityQuery<AirtightComponent> query = base.GetEntityQuery<AirtightComponent>();
			AirtightComponent airtight;
			if (query.TryGetComponent(uid, ref airtight))
			{
				return this.CheckPressureAndFire(uid, firelock, base.Transform(uid), airtight, query);
			}
			return new ValueTuple<bool, bool>(false, false);
		}

		// Token: 0x06001C67 RID: 7271 RVA: 0x00097460 File Offset: 0x00095660
		[return: TupleElementNames(new string[]
		{
			"Pressure",
			"Fire"
		})]
		[return: Nullable(0)]
		public ValueTuple<bool, bool> CheckPressureAndFire(EntityUid uid, FirelockComponent firelock, TransformComponent xform, AirtightComponent airtight, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AirtightComponent> airtightQuery)
		{
			if (!airtight.AirBlocked)
			{
				return new ValueTuple<bool, bool>(false, false);
			}
			DockingComponent dock;
			if (base.TryComp<DockingComponent>(uid, ref dock) && dock.Docked)
			{
				return new ValueTuple<bool, bool>(false, false);
			}
			GridAtmosphereComponent gridAtmosphere;
			if (!base.TryComp<GridAtmosphereComponent>(xform.ParentUid, ref gridAtmosphere))
			{
				return new ValueTuple<bool, bool>(false, false);
			}
			MapGridComponent grid = base.Comp<MapGridComponent>(xform.ParentUid);
			Vector2i pos = grid.CoordinatesToTile(xform.Coordinates);
			float minPressure = float.MaxValue;
			float maxPressure = float.MinValue;
			float minTemperature = float.MaxValue;
			float maxTemperature = float.MinValue;
			bool holdingFire = false;
			bool holdingPressure = false;
			List<Vector2i> tiles = new List<Vector2i>(4);
			List<AtmosDirection> directions = new List<AtmosDirection>(4);
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection dir = (AtmosDirection)(1 << i);
				if (airtight.AirBlockedDirection.HasFlag(dir))
				{
					directions.Add(dir);
					tiles.Add(pos.Offset(dir));
				}
			}
			int count = tiles.Count;
			if (airtight.AirBlockedDirection != AtmosDirection.All)
			{
				tiles.Add(pos);
			}
			GasMixture[] gasses = this._atmosSystem.GetTileMixtures(new EntityUid?(gridAtmosphere.Owner), xform.MapUid, tiles, false);
			if (gasses == null)
			{
				return new ValueTuple<bool, bool>(false, false);
			}
			int j = 0;
			while (j < count)
			{
				GasMixture gas = gasses[j];
				AtmosDirection dir2 = directions[j];
				Vector2i adjacentPos = tiles[j];
				if (gas == null)
				{
					goto IL_1A5;
				}
				if (!this.HasAirtightBlocker(grid.GetAnchoredEntities(adjacentPos), dir2.GetOpposite(), airtightQuery))
				{
					float p = gas.Pressure;
					minPressure = Math.Min(minPressure, p);
					maxPressure = Math.Max(maxPressure, p);
					minTemperature = Math.Min(minTemperature, gas.Temperature);
					maxTemperature = Math.Max(maxTemperature, gas.Temperature);
					goto IL_1A5;
				}
				IL_1DA:
				j++;
				continue;
				IL_1A5:
				holdingPressure |= (maxPressure - minPressure > firelock.PressureThreshold);
				holdingFire |= (maxTemperature - minTemperature > firelock.TemperatureThreshold);
				if (holdingPressure && holdingFire)
				{
					return new ValueTuple<bool, bool>(holdingPressure, holdingFire);
				}
				goto IL_1DA;
			}
			if (airtight.AirBlockedDirection == AtmosDirection.All)
			{
				return new ValueTuple<bool, bool>(holdingPressure, holdingFire);
			}
			GasMixture local = gasses[count];
			if (local != null)
			{
				float p2 = local.Pressure;
				minPressure = Math.Min(minPressure, p2);
				maxPressure = Math.Max(maxPressure, p2);
				minTemperature = Math.Min(minTemperature, local.Temperature);
				maxTemperature = Math.Max(maxTemperature, local.Temperature);
			}
			else
			{
				minPressure = Math.Min(minPressure, 0f);
				maxPressure = Math.Max(maxPressure, 0f);
				minTemperature = Math.Min(minTemperature, 0f);
				maxTemperature = Math.Max(maxTemperature, 0f);
			}
			holdingPressure |= (maxPressure - minPressure > firelock.PressureThreshold);
			holdingFire |= (maxTemperature - minTemperature > firelock.TemperatureThreshold);
			return new ValueTuple<bool, bool>(holdingPressure, holdingFire);
		}

		// Token: 0x06001C68 RID: 7272 RVA: 0x0009771C File Offset: 0x0009591C
		private bool HasAirtightBlocker(IEnumerable<EntityUid> enumerable, AtmosDirection dir, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AirtightComponent> airtightQuery)
		{
			foreach (EntityUid ent in enumerable)
			{
				AirtightComponent airtight;
				if (airtightQuery.TryGetComponent(ent, ref airtight) && airtight.AirBlocked && (airtight.AirBlockedDirection & dir) == dir)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04001239 RID: 4665
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400123A RID: 4666
		[Dependency]
		private readonly SharedDoorSystem _doorSystem;

		// Token: 0x0400123B RID: 4667
		[Dependency]
		private readonly AtmosAlarmableSystem _atmosAlarmable;

		// Token: 0x0400123C RID: 4668
		[Dependency]
		private readonly AtmosphereSystem _atmosSystem;

		// Token: 0x0400123D RID: 4669
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400123E RID: 4670
		private static float _visualUpdateInterval = 0.5f;

		// Token: 0x0400123F RID: 4671
		private float _accumulatedFrameTime;
	}
}
