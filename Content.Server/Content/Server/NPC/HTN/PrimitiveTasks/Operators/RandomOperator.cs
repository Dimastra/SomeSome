using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000353 RID: 851
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RandomOperator : HTNOperator
	{
		// Token: 0x060011C3 RID: 4547 RVA: 0x0005DB08 File Offset: 0x0005BD08
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
			RandomOperator.<Plan>d__5 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<RandomOperator.<Plan>d__5>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000AC0 RID: 2752
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AC1 RID: 2753
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000AC2 RID: 2754
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;

		// Token: 0x04000AC3 RID: 2755
		[DataField("minKey", false, 1, true, false, null)]
		public string MinKey = string.Empty;

		// Token: 0x04000AC4 RID: 2756
		[DataField("maxKey", false, 1, true, false, null)]
		public string MaxKey = string.Empty;
	}
}
