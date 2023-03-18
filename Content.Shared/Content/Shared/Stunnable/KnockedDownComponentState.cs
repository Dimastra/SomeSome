using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stunnable
{
	// Token: 0x0200010D RID: 269
	[NetSerializable]
	[Serializable]
	public sealed class KnockedDownComponentState : ComponentState
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0000D84B File Offset: 0x0000BA4B
		// (set) Token: 0x06000303 RID: 771 RVA: 0x0000D853 File Offset: 0x0000BA53
		public float HelpInterval { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000D85C File Offset: 0x0000BA5C
		// (set) Token: 0x06000305 RID: 773 RVA: 0x0000D864 File Offset: 0x0000BA64
		public float HelpTimer { get; set; }

		// Token: 0x06000306 RID: 774 RVA: 0x0000D86D File Offset: 0x0000BA6D
		public KnockedDownComponentState(float helpInterval, float helpTimer)
		{
			this.HelpInterval = helpInterval;
			this.HelpTimer = helpTimer;
		}
	}
}
