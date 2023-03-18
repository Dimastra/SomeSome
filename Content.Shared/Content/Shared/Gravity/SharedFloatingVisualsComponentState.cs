using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Gravity
{
	// Token: 0x02000444 RID: 1092
	[NetSerializable]
	[Serializable]
	public sealed class SharedFloatingVisualsComponentState : ComponentState
	{
		// Token: 0x06000D3A RID: 3386 RVA: 0x0002BD14 File Offset: 0x00029F14
		public SharedFloatingVisualsComponentState(float animationTime, Vector2 offset, bool hasGravity)
		{
			this.AnimationTime = animationTime;
			this.Offset = offset;
			this.HasGravity = hasGravity;
		}

		// Token: 0x04000CC9 RID: 3273
		public float AnimationTime;

		// Token: 0x04000CCA RID: 3274
		public Vector2 Offset;

		// Token: 0x04000CCB RID: 3275
		public bool HasGravity;
	}
}
