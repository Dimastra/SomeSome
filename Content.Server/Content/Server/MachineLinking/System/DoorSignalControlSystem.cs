using System;
using System.Runtime.CompilerServices;
using Content.Server.Doors.Systems;
using Content.Server.MachineLinking.Components;
using Content.Server.MachineLinking.Events;
using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.MachineLinking.System
{
	// Token: 0x020003EF RID: 1007
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoorSignalControlSystem : EntitySystem
	{
		// Token: 0x0600148F RID: 5263 RVA: 0x0006AD5C File Offset: 0x00068F5C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DoorSignalControlComponent, ComponentInit>(new ComponentEventHandler<DoorSignalControlComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<DoorSignalControlComponent, SignalReceivedEvent>(new ComponentEventHandler<DoorSignalControlComponent, SignalReceivedEvent>(this.OnSignalReceived), null, null);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0006AD8C File Offset: 0x00068F8C
		private void OnInit(EntityUid uid, DoorSignalControlComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				component.OpenPort,
				component.ClosePort,
				component.TogglePort
			});
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x0006ADBC File Offset: 0x00068FBC
		private void OnSignalReceived(EntityUid uid, DoorSignalControlComponent component, SignalReceivedEvent args)
		{
			DoorComponent door;
			if (!base.TryComp<DoorComponent>(uid, ref door))
			{
				return;
			}
			if (args.Port == component.OpenPort)
			{
				if (door.State != DoorState.Open)
				{
					this._doorSystem.TryOpen(uid, door, null, false, false);
					return;
				}
			}
			else if (args.Port == component.ClosePort)
			{
				if (door.State != DoorState.Closed)
				{
					this._doorSystem.TryClose(uid, door, null, false);
					return;
				}
			}
			else if (args.Port == component.TogglePort)
			{
				this._doorSystem.TryToggleDoor(uid, door, null, false);
			}
		}

		// Token: 0x04000CC1 RID: 3265
		[Dependency]
		private readonly DoorSystem _doorSystem;

		// Token: 0x04000CC2 RID: 3266
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;
	}
}
