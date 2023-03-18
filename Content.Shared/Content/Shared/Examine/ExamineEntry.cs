using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Examine
{
	// Token: 0x020004B0 RID: 1200
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ExamineEntry
	{
		// Token: 0x06000E88 RID: 3720 RVA: 0x0002EF16 File Offset: 0x0002D116
		public ExamineEntry(string componentName, float priority, FormattedMessage message)
		{
			this.ComponentName = componentName;
			this.Priority = priority;
			this.Message = message;
		}

		// Token: 0x04000DB4 RID: 3508
		[DataField("component", false, 1, false, false, null)]
		public string ComponentName = string.Empty;

		// Token: 0x04000DB5 RID: 3509
		[DataField("priority", false, 1, false, false, null)]
		public float Priority;

		// Token: 0x04000DB6 RID: 3510
		[DataField("message", false, 1, false, false, null)]
		public FormattedMessage Message = new FormattedMessage();
	}
}
