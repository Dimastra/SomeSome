using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x02000399 RID: 921
	[NetSerializable]
	[Serializable]
	public sealed class MicrowaveSelectCookTimeMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A92 RID: 2706 RVA: 0x00022A05 File Offset: 0x00020C05
		public MicrowaveSelectCookTimeMessage(int buttonIndex, uint inputTime)
		{
			this.ButtonIndex = buttonIndex;
			this.NewCookTime = inputTime;
		}

		// Token: 0x04000A83 RID: 2691
		public int ButtonIndex;

		// Token: 0x04000A84 RID: 2692
		public uint NewCookTime;
	}
}
