using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000352 RID: 850
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PickRandomRotationOperator : HTNOperator
	{
		// Token: 0x060011C1 RID: 4545 RVA: 0x0005DAB0 File Offset: 0x0005BCB0
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
			PickRandomRotationOperator.<Plan>d__2 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<PickRandomRotationOperator.<Plan>d__2>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000ABE RID: 2750
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000ABF RID: 2751
		[DataField("targetKey", false, 1, false, false, null)]
		public string TargetKey = "RotateTarget";
	}
}
