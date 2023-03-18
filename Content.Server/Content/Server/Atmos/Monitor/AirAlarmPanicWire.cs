using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Wires;
using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x0200077C RID: 1916
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AirAlarmPanicWire : ComponentWireAction<AirAlarmComponent>
	{
		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06002897 RID: 10391 RVA: 0x000D3AF8 File Offset: 0x000D1CF8
		// (set) Token: 0x06002898 RID: 10392 RVA: 0x000D3B00 File Offset: 0x000D1D00
		public override string Name { get; set; } = "wire-name-air-alarm-panic";

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06002899 RID: 10393 RVA: 0x000D3B09 File Offset: 0x000D1D09
		// (set) Token: 0x0600289A RID: 10394 RVA: 0x000D3B11 File Offset: 0x000D1D11
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x0600289B RID: 10395 RVA: 0x000D3B1A File Offset: 0x000D1D1A
		public override object StatusKey { get; } = AirAlarmWireStatus.Panic;

		// Token: 0x0600289C RID: 10396 RVA: 0x000D3B22 File Offset: 0x000D1D22
		public override StatusLightState? GetLightState(Wire wire, AirAlarmComponent comp)
		{
			return new StatusLightState?((comp.CurrentMode == AirAlarmMode.Panic) ? StatusLightState.On : StatusLightState.Off);
		}

		// Token: 0x0600289D RID: 10397 RVA: 0x000D3B36 File Offset: 0x000D1D36
		public override void Initialize()
		{
			base.Initialize();
			this._airAlarmSystem = this.EntityManager.System<AirAlarmSystem>();
		}

		// Token: 0x0600289E RID: 10398 RVA: 0x000D3B50 File Offset: 0x000D1D50
		public override bool Cut(EntityUid user, Wire wire, AirAlarmComponent comp)
		{
			DeviceNetworkComponent devNet;
			if (this.EntityManager.TryGetComponent<DeviceNetworkComponent>(wire.Owner, ref devNet))
			{
				this._airAlarmSystem.SetMode(wire.Owner, devNet.Address, AirAlarmMode.Panic, false, null);
			}
			return true;
		}

		// Token: 0x0600289F RID: 10399 RVA: 0x000D3B90 File Offset: 0x000D1D90
		public override bool Mend(EntityUid user, Wire wire, AirAlarmComponent alarm)
		{
			DeviceNetworkComponent devNet;
			if (this.EntityManager.TryGetComponent<DeviceNetworkComponent>(wire.Owner, ref devNet) && alarm.CurrentMode == AirAlarmMode.Panic)
			{
				this._airAlarmSystem.SetMode(wire.Owner, devNet.Address, AirAlarmMode.Filtering, false, alarm);
			}
			return true;
		}

		// Token: 0x060028A0 RID: 10400 RVA: 0x000D3BD8 File Offset: 0x000D1DD8
		public override void Pulse(EntityUid user, Wire wire, AirAlarmComponent comp)
		{
			DeviceNetworkComponent devNet;
			if (this.EntityManager.TryGetComponent<DeviceNetworkComponent>(wire.Owner, ref devNet))
			{
				this._airAlarmSystem.SetMode(wire.Owner, devNet.Address, AirAlarmMode.Panic, false, null);
			}
		}

		// Token: 0x04001937 RID: 6455
		private AirAlarmSystem _airAlarmSystem;
	}
}
