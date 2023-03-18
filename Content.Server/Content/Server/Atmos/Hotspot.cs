using System;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos
{
	// Token: 0x02000733 RID: 1843
	public struct Hotspot
	{
		// Token: 0x04001808 RID: 6152
		[ViewVariables]
		public bool Valid;

		// Token: 0x04001809 RID: 6153
		[ViewVariables]
		public bool SkippedFirstProcess;

		// Token: 0x0400180A RID: 6154
		[ViewVariables]
		public bool Bypassing;

		// Token: 0x0400180B RID: 6155
		[ViewVariables]
		public float Temperature;

		// Token: 0x0400180C RID: 6156
		[ViewVariables]
		public float Volume;

		// Token: 0x0400180D RID: 6157
		[ViewVariables]
		public byte State;
	}
}
