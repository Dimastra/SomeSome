using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.UserInterface
{
	// Token: 0x020000A8 RID: 168
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StatValuesEuiMessage : EuiMessageBase
	{
		// Token: 0x04000253 RID: 595
		public string Title = string.Empty;

		// Token: 0x04000254 RID: 596
		public List<string> Headers = new List<string>();

		// Token: 0x04000255 RID: 597
		public List<string[]> Values = new List<string[]>();
	}
}
