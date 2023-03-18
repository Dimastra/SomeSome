using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Pathfinding;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Specific
{
	// Token: 0x0200035A RID: 858
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PickNearbyInjectableOperator : HTNOperator
	{
		// Token: 0x060011D7 RID: 4567 RVA: 0x0005E025 File Offset: 0x0005C225
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._lookup = sysManager.GetEntitySystem<EntityLookupSystem>();
			this._pathfinding = sysManager.GetEntitySystem<PathfindingSystem>();
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x0005E048 File Offset: 0x0005C248
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
			PickNearbyInjectableOperator.<Plan>d__7 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.cancelToken = cancelToken;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<PickNearbyInjectableOperator.<Plan>d__7>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x04000AD8 RID: 2776
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AD9 RID: 2777
		private EntityLookupSystem _lookup;

		// Token: 0x04000ADA RID: 2778
		private PathfindingSystem _pathfinding;

		// Token: 0x04000ADB RID: 2779
		[DataField("rangeKey", false, 1, false, false, null)]
		public string RangeKey = "MedibotInjectRange";

		// Token: 0x04000ADC RID: 2780
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey = string.Empty;

		// Token: 0x04000ADD RID: 2781
		[DataField("targetMoveKey", false, 1, true, false, null)]
		public string TargetMoveKey = string.Empty;
	}
}
