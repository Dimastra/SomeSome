using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Components;
using Content.Server.NPC.Pathfinding;
using Content.Server.NPC.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x0200034E RID: 846
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MoveToOperator : HTNOperator
	{
		// Token: 0x060011AE RID: 4526 RVA: 0x0005D5A0 File Offset: 0x0005B7A0
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._pathfind = sysManager.GetEntitySystem<PathfindingSystem>();
			this._steering = sysManager.GetEntitySystem<NPCSteeringSystem>();
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0005D5C4 File Offset: 0x0005B7C4
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
			MoveToOperator.<Plan>d__11 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.cancelToken = cancelToken;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<MoveToOperator.<Plan>d__11>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0005D618 File Offset: 0x0005B818
		public override void Startup(NPCBlackboard blackboard)
		{
			base.Startup(blackboard);
			blackboard.Remove<EntityCoordinates>("OwnerCoordinates");
			EntityCoordinates targetCoordinates = blackboard.GetValue<EntityCoordinates>(this.TargetKey);
			NPCSteeringComponent comp = this._steering.Register(blackboard.GetValue<EntityUid>("Owner"), targetCoordinates, null);
			float range;
			if (blackboard.TryGetValue<float>(this.RangeKey, out range, this._entManager))
			{
				comp.Range = range;
			}
			PathResultEvent result;
			if (blackboard.TryGetValue<PathResultEvent>(this.PathfindKey, out result, this._entManager))
			{
				EntityCoordinates coordinates;
				if (blackboard.TryGetValue<EntityCoordinates>("OwnerCoordinates", out coordinates, this._entManager))
				{
					MapCoordinates mapCoords = coordinates.ToMap(this._entManager);
					this._steering.PrunePath(mapCoords, targetCoordinates.ToMapPos(this._entManager) - mapCoords.Position, result.Path);
				}
				comp.CurrentPath = result.Path;
			}
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0005D6F0 File Offset: 0x0005B8F0
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			CancellationTokenSource cancelToken;
			if (blackboard.TryGetValue<CancellationTokenSource>("MovementCancelToken", out cancelToken, this._entManager))
			{
				cancelToken.Cancel();
				blackboard.Remove<CancellationTokenSource>("MovementCancelToken");
			}
			blackboard.Remove<PathResultEvent>(this.PathfindKey);
			if (this.RemoveKeyOnFinish)
			{
				blackboard.Remove<EntityCoordinates>(this.TargetKey);
			}
			this._steering.Unregister(blackboard.GetValue<EntityUid>("Owner"), null);
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0005D768 File Offset: 0x0005B968
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			NPCSteeringComponent steering;
			if (!this._entManager.TryGetComponent<NPCSteeringComponent>(owner, ref steering))
			{
				return HTNOperatorStatus.Failed;
			}
			HTNOperatorStatus result;
			switch (steering.Status)
			{
			case SteeringStatus.NoPath:
				result = HTNOperatorStatus.Failed;
				break;
			case SteeringStatus.Moving:
				result = HTNOperatorStatus.Continuing;
				break;
			case SteeringStatus.InRange:
				result = HTNOperatorStatus.Finished;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		// Token: 0x04000AA0 RID: 2720
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AA1 RID: 2721
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000AA2 RID: 2722
		private NPCSteeringSystem _steering;

		// Token: 0x04000AA3 RID: 2723
		private PathfindingSystem _pathfind;

		// Token: 0x04000AA4 RID: 2724
		[DataField("pathfindInPlanning", false, 1, false, false, null)]
		public bool PathfindInPlanning = true;

		// Token: 0x04000AA5 RID: 2725
		[DataField("removeKeyOnFinish", false, 1, false, false, null)]
		public bool RemoveKeyOnFinish = true;

		// Token: 0x04000AA6 RID: 2726
		[DataField("targetKey", false, 1, false, false, null)]
		public string TargetKey = "MovementTarget";

		// Token: 0x04000AA7 RID: 2727
		[DataField("pathfindKey", false, 1, false, false, null)]
		public string PathfindKey = "MovementPathfind";

		// Token: 0x04000AA8 RID: 2728
		[DataField("rangeKey", false, 1, false, false, null)]
		public string RangeKey = "MovementRange";

		// Token: 0x04000AA9 RID: 2729
		private const string MovementCancelToken = "MovementCancelToken";
	}
}
