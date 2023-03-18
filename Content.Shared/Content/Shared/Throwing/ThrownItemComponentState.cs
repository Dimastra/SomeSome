using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D8 RID: 216
	[NetSerializable]
	[Serializable]
	public sealed class ThrownItemComponentState : ComponentState
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000B59B File Offset: 0x0000979B
		public EntityUid? Thrower { get; }

		// Token: 0x06000259 RID: 601 RVA: 0x0000B5A3 File Offset: 0x000097A3
		public ThrownItemComponentState(EntityUid? thrower)
		{
			this.Thrower = thrower;
		}
	}
}
