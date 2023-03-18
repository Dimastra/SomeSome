using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Medical.SuitSensors;
using Content.Server.UserInterface;
using Content.Shared.Medical.CrewMonitoring;
using Content.Shared.Medical.SuitSensor;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Server.Medical.CrewMonitoring
{
	// Token: 0x020003B8 RID: 952
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewMonitoringConsoleSystem : EntitySystem
	{
		// Token: 0x060013A3 RID: 5027 RVA: 0x00065B28 File Offset: 0x00063D28
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CrewMonitoringConsoleComponent, ComponentRemove>(new ComponentEventHandler<CrewMonitoringConsoleComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<CrewMonitoringConsoleComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<CrewMonitoringConsoleComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<CrewMonitoringConsoleComponent, BoundUIOpenedEvent>(new ComponentEventHandler<CrewMonitoringConsoleComponent, BoundUIOpenedEvent>(this.OnUIOpened), null, null);
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x00065B77 File Offset: 0x00063D77
		private void OnRemove(EntityUid uid, CrewMonitoringConsoleComponent component, ComponentRemove args)
		{
			component.ConnectedSensors.Clear();
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00065B84 File Offset: 0x00063D84
		private void OnPacketReceived(EntityUid uid, CrewMonitoringConsoleComponent component, DeviceNetworkPacketEvent args)
		{
			NetworkPayload payload = args.Data;
			string command;
			if (!payload.TryGetValue<string>("command", out command))
			{
				return;
			}
			if (command != "updated_state")
			{
				return;
			}
			Dictionary<string, SuitSensorStatus> sensorStatus;
			if (!payload.TryGetValue<Dictionary<string, SuitSensorStatus>>("suit-status-collection", out sensorStatus))
			{
				return;
			}
			component.ConnectedSensors = sensorStatus;
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00065BD5 File Offset: 0x00063DD5
		private void OnUIOpened(EntityUid uid, CrewMonitoringConsoleComponent component, BoundUIOpenedEvent args)
		{
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x00065BE0 File Offset: 0x00063DE0
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, CrewMonitoringConsoleComponent component = null)
		{
			if (!base.Resolve<CrewMonitoringConsoleComponent>(uid, ref component, true))
			{
				return;
			}
			BoundUserInterface ui = component.Owner.GetUIOrNull(CrewMonitoringUIKey.Key);
			if (ui == null)
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			CrewMonitoringState uiState = new CrewMonitoringState(component.ConnectedSensors.Values.ToList<SuitSensorStatus>(), xform.WorldPosition, component.Snap, component.Precision);
			ui.SetState(uiState, null, true);
		}

		// Token: 0x04000BFF RID: 3071
		[Dependency]
		private readonly SuitSensorSystem _sensors;

		// Token: 0x04000C00 RID: 3072
		[Dependency]
		private readonly SharedTransformSystem _xform;

		// Token: 0x04000C01 RID: 3073
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000C02 RID: 3074
		[Dependency]
		private readonly IMapManager _mapManager;
	}
}
