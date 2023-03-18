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
	// Token: 0x02000350 RID: 848
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PickAccessibleComponentOperator : HTNOperator
	{
		// Token: 0x060011BB RID: 4539 RVA: 0x0005D977 File Offset: 0x0005BB77
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._lookup = sysManager.GetEntitySystem<EntityLookupSystem>();
			this._pathfinding = sysManager.GetEntitySystem<PathfindingSystem>();
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x0005D998 File Offset: 0x0005BB98
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
			PickAccessibleComponentOperator.<Plan>d__9 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.cancelToken = cancelToken;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<PickAccessibleComponentOperator.<Plan>d__9>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000AB1 RID: 2737
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x04000AB2 RID: 2738
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AB3 RID: 2739
		private PathfindingSystem _pathfinding;

		// Token: 0x04000AB4 RID: 2740
		private EntityLookupSystem _lookup;

		// Token: 0x04000AB5 RID: 2741
		[DataField("rangeKey", false, 1, true, false, null)]
		public string RangeKey = string.Empty;

		// Token: 0x04000AB6 RID: 2742
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;

		// Token: 0x04000AB7 RID: 2743
		[DataField("component", false, 1, true, false, null)]
		public string Component = string.Empty;

		// Token: 0x04000AB8 RID: 2744
		[DataField("pathfindKey", false, 1, false, false, null)]
		public string PathfindKey = "MovementPathfind";
	}
}
