using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor.Components;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x02000775 RID: 1909
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirAlarmModeFactory
	{
		// Token: 0x06002888 RID: 10376 RVA: 0x000D35BC File Offset: 0x000D17BC
		[NullableContext(2)]
		public static IAirAlarmMode ModeToExecutor(AirAlarmMode mode)
		{
			IAirAlarmMode result;
			switch (mode)
			{
			case AirAlarmMode.None:
				result = AirAlarmModeFactory._noneMode;
				break;
			case AirAlarmMode.Filtering:
				result = AirAlarmModeFactory._filterMode;
				break;
			case AirAlarmMode.WideFiltering:
				result = AirAlarmModeFactory._wideFilterMode;
				break;
			case AirAlarmMode.Fill:
				result = AirAlarmModeFactory._fillMode;
				break;
			case AirAlarmMode.Panic:
				result = AirAlarmModeFactory._panicMode;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x0400192D RID: 6445
		private static IAirAlarmMode _filterMode = new AirAlarmFilterMode();

		// Token: 0x0400192E RID: 6446
		private static IAirAlarmMode _wideFilterMode = new AirAlarmWideFilterMode();

		// Token: 0x0400192F RID: 6447
		private static IAirAlarmMode _fillMode = new AirAlarmFillMode();

		// Token: 0x04001930 RID: 6448
		private static IAirAlarmMode _panicMode = new AirAlarmPanicMode();

		// Token: 0x04001931 RID: 6449
		private static IAirAlarmMode _noneMode = new AirAlarmNoneMode();
	}
}
