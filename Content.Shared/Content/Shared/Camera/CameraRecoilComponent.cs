using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;

namespace Content.Shared.Camera
{
	// Token: 0x0200063A RID: 1594
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class CameraRecoilComponent : Component
	{
		// Token: 0x170003DD RID: 989
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x0003FD80 File Offset: 0x0003DF80
		// (set) Token: 0x0600132D RID: 4909 RVA: 0x0003FD88 File Offset: 0x0003DF88
		public Vector2 CurrentKick { get; set; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x0600132E RID: 4910 RVA: 0x0003FD91 File Offset: 0x0003DF91
		// (set) Token: 0x0600132F RID: 4911 RVA: 0x0003FD99 File Offset: 0x0003DF99
		public float LastKickTime { get; set; }

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06001330 RID: 4912 RVA: 0x0003FDA2 File Offset: 0x0003DFA2
		// (set) Token: 0x06001331 RID: 4913 RVA: 0x0003FDAA File Offset: 0x0003DFAA
		public Vector2 BaseOffset { get; set; }
	}
}
