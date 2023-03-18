using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000741 RID: 1857
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class QuickDialogEntry
	{
		// Token: 0x06001691 RID: 5777 RVA: 0x00049ACC File Offset: 0x00047CCC
		public QuickDialogEntry(string fieldId, QuickDialogEntryType type, string prompt)
		{
			this.FieldId = fieldId;
			this.Type = type;
			this.Prompt = prompt;
		}

		// Token: 0x040016C6 RID: 5830
		public string FieldId;

		// Token: 0x040016C7 RID: 5831
		public QuickDialogEntryType Type;

		// Token: 0x040016C8 RID: 5832
		public string Prompt;
	}
}
