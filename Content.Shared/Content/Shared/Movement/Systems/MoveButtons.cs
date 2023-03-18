using System;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002DD RID: 733
	[Flags]
	public enum MoveButtons : byte
	{
		// Token: 0x04000852 RID: 2130
		None = 0,
		// Token: 0x04000853 RID: 2131
		Up = 1,
		// Token: 0x04000854 RID: 2132
		Down = 2,
		// Token: 0x04000855 RID: 2133
		Left = 4,
		// Token: 0x04000856 RID: 2134
		Right = 8,
		// Token: 0x04000857 RID: 2135
		Walk = 16
	}
}
