using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NPC.HTN.PrimitiveTasks;

namespace Content.Server.NPC.HTN
{
	// Token: 0x02000346 RID: 838
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HTNPlan
	{
		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06001188 RID: 4488 RVA: 0x0005C5AF File Offset: 0x0005A7AF
		public HTNPrimitiveTask CurrentTask
		{
			get
			{
				return this.Tasks[this.Index];
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06001189 RID: 4489 RVA: 0x0005C5C2 File Offset: 0x0005A7C2
		public HTNOperator CurrentOperator
		{
			get
			{
				return this.CurrentTask.Operator;
			}
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x0005C5CF File Offset: 0x0005A7CF
		public HTNPlan(List<HTNPrimitiveTask> tasks, List<int> branchTraversalRecord, [Nullable(new byte[]
		{
			1,
			2,
			1,
			1
		})] List<Dictionary<string, object>> effects)
		{
			this.Tasks = tasks;
			this.BranchTraversalRecord = branchTraversalRecord;
			this.Effects = effects;
		}

		// Token: 0x04000A88 RID: 2696
		[Nullable(new byte[]
		{
			1,
			2,
			1,
			1
		})]
		public readonly List<Dictionary<string, object>> Effects;

		// Token: 0x04000A89 RID: 2697
		public List<int> BranchTraversalRecord;

		// Token: 0x04000A8A RID: 2698
		public List<HTNPrimitiveTask> Tasks;

		// Token: 0x04000A8B RID: 2699
		public int Index;
	}
}
