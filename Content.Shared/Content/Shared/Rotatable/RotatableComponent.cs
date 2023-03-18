using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Rotatable
{
	// Token: 0x020001E0 RID: 480
	[RegisterComponent]
	public sealed class RotatableComponent : Component
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x00013BD5 File Offset: 0x00011DD5
		// (set) Token: 0x06000550 RID: 1360 RVA: 0x00013BDD File Offset: 0x00011DDD
		[ViewVariables]
		[DataField("rotateWhileAnchored", false, 1, false, false, null)]
		public bool RotateWhileAnchored { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000551 RID: 1361 RVA: 0x00013BE6 File Offset: 0x00011DE6
		// (set) Token: 0x06000552 RID: 1362 RVA: 0x00013BEE File Offset: 0x00011DEE
		[ViewVariables]
		[DataField("rotateWhilePulling", false, 1, false, false, null)]
		public bool RotateWhilePulling { get; private set; } = true;

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000553 RID: 1363 RVA: 0x00013BF7 File Offset: 0x00011DF7
		// (set) Token: 0x06000554 RID: 1364 RVA: 0x00013BFF File Offset: 0x00011DFF
		[ViewVariables]
		[DataField("increment", false, 1, false, false, null)]
		public Angle Increment { get; private set; } = Angle.FromDegrees(90.0);
	}
}
