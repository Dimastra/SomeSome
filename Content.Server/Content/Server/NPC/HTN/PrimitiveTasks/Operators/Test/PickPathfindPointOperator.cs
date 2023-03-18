using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Test
{
	// Token: 0x02000358 RID: 856
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PickPathfindPointOperator : HTNOperator
	{
		// Token: 0x060011D1 RID: 4561 RVA: 0x0005DD78 File Offset: 0x0005BF78
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
			PickPathfindPointOperator.<Plan>d__2 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<PickPathfindPointOperator.<Plan>d__2>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000AD0 RID: 2768
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AD1 RID: 2769
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
