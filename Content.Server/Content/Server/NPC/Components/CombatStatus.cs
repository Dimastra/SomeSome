using System;

namespace Content.Server.NPC.Components
{
	// Token: 0x0200036E RID: 878
	public enum CombatStatus : byte
	{
		// Token: 0x04000B09 RID: 2825
		NotInSight,
		// Token: 0x04000B0A RID: 2826
		Unspecified,
		// Token: 0x04000B0B RID: 2827
		TargetUnreachable,
		// Token: 0x04000B0C RID: 2828
		TargetOutOfRange,
		// Token: 0x04000B0D RID: 2829
		NoWeapon,
		// Token: 0x04000B0E RID: 2830
		Normal
	}
}
