using System;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002DE RID: 734
	[Flags]
	public enum ShuttleButtons : byte
	{
		// Token: 0x04000859 RID: 2137
		None = 0,
		// Token: 0x0400085A RID: 2138
		StrafeUp = 1,
		// Token: 0x0400085B RID: 2139
		StrafeDown = 2,
		// Token: 0x0400085C RID: 2140
		StrafeLeft = 4,
		// Token: 0x0400085D RID: 2141
		StrafeRight = 8,
		// Token: 0x0400085E RID: 2142
		RotateLeft = 16,
		// Token: 0x0400085F RID: 2143
		RotateRight = 32,
		// Token: 0x04000860 RID: 2144
		Brake = 64
	}
}
