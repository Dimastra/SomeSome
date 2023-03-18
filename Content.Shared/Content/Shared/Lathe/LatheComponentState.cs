using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe
{
	// Token: 0x02000381 RID: 897
	[NetSerializable]
	[Serializable]
	public sealed class LatheComponentState : ComponentState
	{
		// Token: 0x06000A70 RID: 2672 RVA: 0x000226C5 File Offset: 0x000208C5
		public LatheComponentState(float materialUseMultiplier)
		{
			this.MaterialUseMultiplier = materialUseMultiplier;
		}

		// Token: 0x04000A5A RID: 2650
		public float MaterialUseMultiplier;
	}
}
