using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Examine
{
	// Token: 0x020004AE RID: 1198
	[RegisterComponent]
	public sealed class GroupExamineComponent : Component
	{
		// Token: 0x04000DAD RID: 3501
		[Nullable(1)]
		[DataField("group", false, 1, false, false, null)]
		public List<ExamineGroup> ExamineGroups = new List<ExamineGroup>
		{
			new ExamineGroup
			{
				Components = new List<string>
				{
					"Armor",
					"ClothingSpeedModifier"
				}
			}
		};
	}
}
