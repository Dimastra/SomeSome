using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Nuke
{
	// Token: 0x020002BF RID: 703
	[NetSerializable]
	[Serializable]
	public sealed class NukeUiState : BoundUserInterfaceState
	{
		// Token: 0x040007E4 RID: 2020
		public bool DiskInserted;

		// Token: 0x040007E5 RID: 2021
		public NukeStatus Status;

		// Token: 0x040007E6 RID: 2022
		public int RemainingTime;

		// Token: 0x040007E7 RID: 2023
		public int CooldownTime;

		// Token: 0x040007E8 RID: 2024
		public bool IsAnchored;

		// Token: 0x040007E9 RID: 2025
		public int EnteredCodeLength;

		// Token: 0x040007EA RID: 2026
		public int MaxCodeLength;

		// Token: 0x040007EB RID: 2027
		public bool AllowArm;
	}
}
