using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Enums;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StationRecords
{
	// Token: 0x0200015D RID: 349
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class GeneralStationRecord
	{
		// Token: 0x04000404 RID: 1028
		[ViewVariables]
		public string Name = string.Empty;

		// Token: 0x04000405 RID: 1029
		[ViewVariables]
		public int Age;

		// Token: 0x04000406 RID: 1030
		[ViewVariables]
		public string JobTitle = string.Empty;

		// Token: 0x04000407 RID: 1031
		[ViewVariables]
		public string JobIcon = string.Empty;

		// Token: 0x04000408 RID: 1032
		[ViewVariables]
		public string JobPrototype = string.Empty;

		// Token: 0x04000409 RID: 1033
		[ViewVariables]
		public string Species = string.Empty;

		// Token: 0x0400040A RID: 1034
		[ViewVariables]
		public Gender Gender = 1;

		// Token: 0x0400040B RID: 1035
		[ViewVariables]
		public int DisplayPriority;

		// Token: 0x0400040C RID: 1036
		[ViewVariables]
		public string Fingerprint = string.Empty;
	}
}
