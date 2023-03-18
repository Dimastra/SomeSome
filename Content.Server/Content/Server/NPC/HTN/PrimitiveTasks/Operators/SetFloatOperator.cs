using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000355 RID: 853
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SetFloatOperator : HTNOperator
	{
		// Token: 0x060011C9 RID: 4553 RVA: 0x0005DC40 File Offset: 0x0005BE40
		[return: TupleElementNames(new string[]
		{
			"Valid",
			"Effects"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			2,
			1,
			1
		})]
		public override Task<ValueTuple<bool, Dictionary<string, object>>> Plan(NPCBlackboard blackboard, CancellationToken cancelToken)
		{
			SetFloatOperator.<Plan>d__2 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<SetFloatOperator.<Plan>d__2>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000ACA RID: 2762
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;

		// Token: 0x04000ACB RID: 2763
		[ViewVariables]
		[DataField("amount", false, 1, false, false, null)]
		public float Amount;
	}
}
