using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Pathfinding;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000351 RID: 849
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PickAccessibleOperator : HTNOperator
	{
		// Token: 0x060011BE RID: 4542 RVA: 0x0005DA1F File Offset: 0x0005BC1F
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._pathfinding = sysManager.GetEntitySystem<PathfindingSystem>();
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x0005DA34 File Offset: 0x0005BC34
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
			PickAccessibleOperator.<Plan>d__6 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.cancelToken = cancelToken;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<PickAccessibleOperator.<Plan>d__6>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000AB9 RID: 2745
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000ABA RID: 2746
		private PathfindingSystem _pathfinding;

		// Token: 0x04000ABB RID: 2747
		[DataField("rangeKey", false, 1, true, false, null)]
		public string RangeKey = string.Empty;

		// Token: 0x04000ABC RID: 2748
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;

		// Token: 0x04000ABD RID: 2749
		[DataField("pathfindKey", false, 1, false, false, null)]
		public string PathfindKey = "MovementPathfind";
	}
}
