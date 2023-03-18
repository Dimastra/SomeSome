using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Examine
{
	// Token: 0x020004A9 RID: 1193
	[RegisterComponent]
	public sealed class ExaminerComponent : Component
	{
		// Token: 0x04000D9B RID: 3483
		[ViewVariables]
		[DataField("skipChecks", false, 1, false, false, null)]
		public bool SkipChecks;

		// Token: 0x04000D9C RID: 3484
		[ViewVariables]
		[DataField("checkInRangeUnOccluded", false, 1, false, false, null)]
		public bool CheckInRangeUnOccluded = true;
	}
}
