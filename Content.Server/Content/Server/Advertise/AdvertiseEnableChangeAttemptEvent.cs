using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Advertise
{
	// Token: 0x020007FB RID: 2043
	public sealed class AdvertiseEnableChangeAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06002C2F RID: 11311 RVA: 0x000E706C File Offset: 0x000E526C
		public bool NewState { get; }

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06002C30 RID: 11312 RVA: 0x000E7074 File Offset: 0x000E5274
		public bool OldState { get; }

		// Token: 0x06002C31 RID: 11313 RVA: 0x000E707C File Offset: 0x000E527C
		public AdvertiseEnableChangeAttemptEvent(bool newState, bool oldEnabledState)
		{
			this.NewState = newState;
			this.OldState = oldEnabledState;
		}
	}
}
