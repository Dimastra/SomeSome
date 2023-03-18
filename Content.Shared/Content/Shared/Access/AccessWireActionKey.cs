using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Access
{
	// Token: 0x0200076F RID: 1903
	[NetSerializable]
	[Serializable]
	public enum AccessWireActionKey : byte
	{
		// Token: 0x04001746 RID: 5958
		Key,
		// Token: 0x04001747 RID: 5959
		Status,
		// Token: 0x04001748 RID: 5960
		Pulsed,
		// Token: 0x04001749 RID: 5961
		PulseCancel
	}
}
