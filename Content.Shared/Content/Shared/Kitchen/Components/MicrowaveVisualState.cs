using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x0200039B RID: 923
	[NetSerializable]
	[Serializable]
	public enum MicrowaveVisualState
	{
		// Token: 0x04000A8A RID: 2698
		Idle,
		// Token: 0x04000A8B RID: 2699
		Cooking,
		// Token: 0x04000A8C RID: 2700
		Broken
	}
}
