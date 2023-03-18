using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole
{
	// Token: 0x020005BE RID: 1470
	[NetSerializable]
	[Serializable]
	public enum ClonerStatus : byte
	{
		// Token: 0x04001099 RID: 4249
		Ready,
		// Token: 0x0400109A RID: 4250
		ScannerEmpty,
		// Token: 0x0400109B RID: 4251
		ScannerOccupantAlive,
		// Token: 0x0400109C RID: 4252
		OccupantMetaphyiscal,
		// Token: 0x0400109D RID: 4253
		ClonerOccupied,
		// Token: 0x0400109E RID: 4254
		NoClonerDetected,
		// Token: 0x0400109F RID: 4255
		NoMindDetected
	}
}
