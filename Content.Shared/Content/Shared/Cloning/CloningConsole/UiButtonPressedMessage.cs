using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole
{
	// Token: 0x020005C1 RID: 1473
	[NetSerializable]
	[Serializable]
	public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060011DA RID: 4570 RVA: 0x0003A958 File Offset: 0x00038B58
		public UiButtonPressedMessage(UiButton button)
		{
			this.Button = button;
		}

		// Token: 0x040010A5 RID: 4261
		public readonly UiButton Button;
	}
}
