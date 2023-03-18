using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Atmos.Monitor
{
	// Token: 0x02000447 RID: 1095
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AtmosAlarmableVisualsComponent : Component
	{
		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x06001B07 RID: 6919 RVA: 0x0009BEF0 File Offset: 0x0009A0F0
		[DataField("layerMap", false, 1, false, false, null)]
		public string LayerMap { get; } = string.Empty;

		// Token: 0x04000D9A RID: 3482
		[DataField("alarmStates", false, 1, false, false, null)]
		public readonly Dictionary<AtmosAlarmType, string> AlarmStates = new Dictionary<AtmosAlarmType, string>();

		// Token: 0x04000D9B RID: 3483
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("hideOnDepowered", false, 1, false, false, null)]
		public readonly List<string> HideOnDepowered;

		// Token: 0x04000D9C RID: 3484
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("setOnDepowered", false, 1, false, false, null)]
		public readonly Dictionary<string, string> SetOnDepowered;
	}
}
