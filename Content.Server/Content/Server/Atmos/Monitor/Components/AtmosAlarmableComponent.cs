using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Monitor.Components
{
	// Token: 0x02000785 RID: 1925
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AtmosAlarmableComponent : Component
	{
		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x060028FB RID: 10491 RVA: 0x000D5AEC File Offset: 0x000D3CEC
		// (set) Token: 0x060028FC RID: 10492 RVA: 0x000D5AF4 File Offset: 0x000D3CF4
		[ViewVariables]
		public bool IgnoreAlarms { get; set; }

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x060028FD RID: 10493 RVA: 0x000D5AFD File Offset: 0x000D3CFD
		// (set) Token: 0x060028FE RID: 10494 RVA: 0x000D5B05 File Offset: 0x000D3D05
		[DataField("alarmSound", false, 1, false, false, null)]
		public SoundSpecifier AlarmSound { get; set; } = new SoundPathSpecifier("/Audio/Machines/alarm.ogg", null);

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x060028FF RID: 10495 RVA: 0x000D5B0E File Offset: 0x000D3D0E
		// (set) Token: 0x06002900 RID: 10496 RVA: 0x000D5B16 File Offset: 0x000D3D16
		[DataField("alarmVolume", false, 1, false, false, null)]
		public float AlarmVolume { get; set; } = -10f;

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06002901 RID: 10497 RVA: 0x000D5B1F File Offset: 0x000D3D1F
		[DataField("syncWith", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<TagPrototype>))]
		public HashSet<string> SyncWithTags { get; } = new HashSet<string>();

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06002902 RID: 10498 RVA: 0x000D5B27 File Offset: 0x000D3D27
		[Nullable(2)]
		[DataField("monitorAlertTypes", false, 1, false, false, null)]
		public HashSet<AtmosMonitorThresholdType> MonitorAlertTypes { [NullableContext(2)] get; }

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06002903 RID: 10499 RVA: 0x000D5B2F File Offset: 0x000D3D2F
		[DataField("receiveOnly", false, 1, false, false, null)]
		public bool ReceiveOnly { get; }

		// Token: 0x0400196D RID: 6509
		[ViewVariables]
		public readonly Dictionary<string, AtmosAlarmType> NetworkAlarmStates = new Dictionary<string, AtmosAlarmType>();

		// Token: 0x0400196E RID: 6510
		[ViewVariables]
		public AtmosAlarmType LastAlarmState;
	}
}
