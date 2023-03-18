using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D3 RID: 1747
	[NetSerializable]
	[Serializable]
	public enum AirAlarmMode
	{
		// Token: 0x04001555 RID: 5461
		None,
		// Token: 0x04001556 RID: 5462
		Filtering,
		// Token: 0x04001557 RID: 5463
		WideFiltering,
		// Token: 0x04001558 RID: 5464
		Fill,
		// Token: 0x04001559 RID: 5465
		Panic
	}
}
