using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks
{
	// Token: 0x0200034C RID: 844
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class HTNOperator
	{
		// Token: 0x060011A7 RID: 4519 RVA: 0x0005D530 File Offset: 0x0005B730
		public virtual void Initialize(IEntitySystemManager sysManager)
		{
			IoCManager.InjectDependencies<HTNOperator>(this);
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0005D53C File Offset: 0x0005B73C
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
		public virtual Task<ValueTuple<bool, Dictionary<string, object>>> Plan(NPCBlackboard blackboard, CancellationToken cancelToken)
		{
			HTNOperator.<Plan>d__1 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<HTNOperator.<Plan>d__1>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x0005D577 File Offset: 0x0005B777
		public virtual HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			return HTNOperatorStatus.Finished;
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x0005D57A File Offset: 0x0005B77A
		public virtual void Startup(NPCBlackboard blackboard)
		{
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0005D57C File Offset: 0x0005B77C
		public virtual void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
		}
	}
}
