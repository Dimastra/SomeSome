using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalScanner
{
	// Token: 0x02000309 RID: 777
	public abstract class SharedMedicalScannerComponent : Component
	{
		// Token: 0x020007D8 RID: 2008
		[NetSerializable]
		[Serializable]
		public enum MedicalScannerVisuals : byte
		{
			// Token: 0x0400182F RID: 6191
			Status
		}

		// Token: 0x020007D9 RID: 2009
		[NetSerializable]
		[Serializable]
		public enum MedicalScannerStatus : byte
		{
			// Token: 0x04001831 RID: 6193
			Off,
			// Token: 0x04001832 RID: 6194
			Open,
			// Token: 0x04001833 RID: 6195
			Red,
			// Token: 0x04001834 RID: 6196
			Death,
			// Token: 0x04001835 RID: 6197
			Green,
			// Token: 0x04001836 RID: 6198
			Yellow
		}
	}
}
