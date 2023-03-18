using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor
{
	// Token: 0x020006CF RID: 1743
	[NetSerializable]
	[Serializable]
	public enum AtmosAlarmType : sbyte
	{
		// Token: 0x04001540 RID: 5440
		Invalid,
		// Token: 0x04001541 RID: 5441
		Normal,
		// Token: 0x04001542 RID: 5442
		Warning,
		// Token: 0x04001543 RID: 5443
		Danger,
		// Token: 0x04001544 RID: 5444
		Emagged
	}
}
