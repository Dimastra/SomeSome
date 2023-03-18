using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Monitor.Components
{
	// Token: 0x02000784 RID: 1924
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AirAlarmComponent : Component
	{
		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x060028F4 RID: 10484 RVA: 0x000D5A60 File Offset: 0x000D3C60
		// (set) Token: 0x060028F5 RID: 10485 RVA: 0x000D5A68 File Offset: 0x000D3C68
		[ViewVariables]
		public AirAlarmMode CurrentMode { get; set; } = AirAlarmMode.Filtering;

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x060028F6 RID: 10486 RVA: 0x000D5A71 File Offset: 0x000D3C71
		// (set) Token: 0x060028F7 RID: 10487 RVA: 0x000D5A79 File Offset: 0x000D3C79
		[Nullable(2)]
		[ViewVariables]
		public IAirAlarmModeUpdate CurrentModeUpdater { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x060028F8 RID: 10488 RVA: 0x000D5A82 File Offset: 0x000D3C82
		// (set) Token: 0x060028F9 RID: 10489 RVA: 0x000D5A8A File Offset: 0x000D3C8A
		[ViewVariables]
		public AirAlarmTab CurrentTab { get; set; }

		// Token: 0x04001967 RID: 6503
		public readonly HashSet<string> KnownDevices = new HashSet<string>();

		// Token: 0x04001968 RID: 6504
		public readonly Dictionary<string, GasVentPumpData> VentData = new Dictionary<string, GasVentPumpData>();

		// Token: 0x04001969 RID: 6505
		public readonly Dictionary<string, GasVentScrubberData> ScrubberData = new Dictionary<string, GasVentScrubberData>();

		// Token: 0x0400196A RID: 6506
		public readonly Dictionary<string, AtmosSensorData> SensorData = new Dictionary<string, AtmosSensorData>();

		// Token: 0x0400196B RID: 6507
		public HashSet<NetUserId> ActivePlayers = new HashSet<NetUserId>();

		// Token: 0x0400196C RID: 6508
		public bool CanSync = true;
	}
}
