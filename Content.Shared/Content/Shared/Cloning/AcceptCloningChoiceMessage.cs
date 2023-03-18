using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning
{
	// Token: 0x020005BA RID: 1466
	[NetSerializable]
	[Serializable]
	public sealed class AcceptCloningChoiceMessage : EuiMessageBase
	{
		// Token: 0x060011D8 RID: 4568 RVA: 0x0003A8F9 File Offset: 0x00038AF9
		public AcceptCloningChoiceMessage(AcceptCloningUiButton button)
		{
			this.Button = button;
		}

		// Token: 0x04001088 RID: 4232
		public readonly AcceptCloningUiButton Button;
	}
}
