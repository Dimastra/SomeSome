using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Wires;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x0200077D RID: 1917
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AtmosMonitorDeviceNetWire : ComponentWireAction<AtmosAlarmableComponent>
	{
		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x060028A2 RID: 10402 RVA: 0x000D3C3E File Offset: 0x000D1E3E
		// (set) Token: 0x060028A3 RID: 10403 RVA: 0x000D3C46 File Offset: 0x000D1E46
		public override string Name { get; set; } = "wire-name-device-net";

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x060028A4 RID: 10404 RVA: 0x000D3C4F File Offset: 0x000D1E4F
		// (set) Token: 0x060028A5 RID: 10405 RVA: 0x000D3C57 File Offset: 0x000D1E57
		public override Color Color { get; set; } = Color.Orange;

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x060028A6 RID: 10406 RVA: 0x000D3C60 File Offset: 0x000D1E60
		public override object StatusKey { get; } = AtmosMonitorAlarmWireActionKeys.Network;

		// Token: 0x060028A7 RID: 10407 RVA: 0x000D3C68 File Offset: 0x000D1E68
		public override StatusLightState? GetLightState(Wire wire, AtmosAlarmableComponent comp)
		{
			AtmosAlarmType? alarm;
			if (!this._atmosAlarmableSystem.TryGetHighestAlert(wire.Owner, out alarm, comp))
			{
				alarm = new AtmosAlarmType?(AtmosAlarmType.Normal);
			}
			AtmosAlarmType? atmosAlarmType = alarm;
			AtmosAlarmType atmosAlarmType2 = AtmosAlarmType.Danger;
			return new StatusLightState?((atmosAlarmType.GetValueOrDefault() == atmosAlarmType2 & atmosAlarmType != null) ? StatusLightState.BlinkingFast : StatusLightState.On);
		}

		// Token: 0x060028A8 RID: 10408 RVA: 0x000D3CB4 File Offset: 0x000D1EB4
		public override void Initialize()
		{
			base.Initialize();
			this._atmosAlarmableSystem = this.EntityManager.System<AtmosAlarmableSystem>();
		}

		// Token: 0x060028A9 RID: 10409 RVA: 0x000D3CCD File Offset: 0x000D1ECD
		public override bool Cut(EntityUid user, Wire wire, AtmosAlarmableComponent comp)
		{
			comp.IgnoreAlarms = true;
			return true;
		}

		// Token: 0x060028AA RID: 10410 RVA: 0x000D3CD7 File Offset: 0x000D1ED7
		public override bool Mend(EntityUid user, Wire wire, AtmosAlarmableComponent comp)
		{
			comp.IgnoreAlarms = false;
			return true;
		}

		// Token: 0x060028AB RID: 10411 RVA: 0x000D3CE1 File Offset: 0x000D1EE1
		public override void Pulse(EntityUid user, Wire wire, AtmosAlarmableComponent comp)
		{
			if (this._alarmOnPulse)
			{
				this._atmosAlarmableSystem.ForceAlert(wire.Owner, AtmosAlarmType.Danger, comp, null, null);
			}
		}

		// Token: 0x04001939 RID: 6457
		[DataField("alarmOnPulse", false, 1, false, false, null)]
		private bool _alarmOnPulse;

		// Token: 0x0400193C RID: 6460
		private AtmosAlarmableSystem _atmosAlarmableSystem;
	}
}
