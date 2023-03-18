using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.Utility;

namespace Content.Server.Objectives.Interfaces
{
	// Token: 0x020002F9 RID: 761
	[NullableContext(1)]
	public interface IObjectiveCondition : IEquatable<IObjectiveCondition>
	{
		// Token: 0x06000F9B RID: 3995
		IObjectiveCondition GetAssigned(Mind mind);

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000F9C RID: 3996
		string Title { get; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000F9D RID: 3997
		string Description { get; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000F9E RID: 3998
		SpriteSpecifier Icon { get; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000F9F RID: 3999
		float Progress { get; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000FA0 RID: 4000
		float Difficulty { get; }
	}
}
