using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CB RID: 971
	public interface ITargetedInteractEventArgs
	{
		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000B27 RID: 2855
		EntityUid User { get; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000B28 RID: 2856
		EntityUid Target { get; }
	}
}
