using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Interaction;
using Content.Server.NPC.Pathfinding;
using Content.Server.NPC.Systems;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x0200034F RID: 847
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class NPCCombatOperator : HTNOperator
	{
		// Token: 0x1700027E RID: 638
		// (get) Token: 0x060011B4 RID: 4532 RVA: 0x0005D7F8 File Offset: 0x0005B9F8
		protected virtual bool IsRanged
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x0005D7FB File Offset: 0x0005B9FB
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			sysManager.GetEntitySystem<ExamineSystemShared>();
			this._factions = sysManager.GetEntitySystem<FactionSystem>();
			this.Interaction = sysManager.GetEntitySystem<InteractionSystem>();
			this._pathfinding = sysManager.GetEntitySystem<PathfindingSystem>();
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0005D830 File Offset: 0x0005BA30
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
			NPCCombatOperator.<Plan>d__10 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<NPCCombatOperator.<Plan>d__10>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0005D87C File Offset: 0x0005BA7C
		[return: TupleElementNames(new string[]
		{
			"Entity",
			"Rating",
			"Distance"
		})]
		[return: Nullable(new byte[]
		{
			1,
			1,
			0
		})]
		private Task<List<ValueTuple<EntityUid, float, float>>> GetTargets(NPCBlackboard blackboard)
		{
			NPCCombatOperator.<GetTargets>d__11 <GetTargets>d__;
			<GetTargets>d__.<>t__builder = AsyncTaskMethodBuilder<List<ValueTuple<EntityUid, float, float>>>.Create();
			<GetTargets>d__.<>4__this = this;
			<GetTargets>d__.blackboard = blackboard;
			<GetTargets>d__.<>1__state = -1;
			<GetTargets>d__.<>t__builder.Start<NPCCombatOperator.<GetTargets>d__11>(ref <GetTargets>d__);
			return <GetTargets>d__.<>t__builder.Task;
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0005D8C8 File Offset: 0x0005BAC8
		private Task UpdateTarget(EntityUid owner, EntityUid target, EntityUid existingTarget, EntityCoordinates ownerCoordinates, NPCBlackboard blackboard, float radius, bool canMove, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [TupleElementNames(new string[]
		{
			"Entity",
			"Rating",
			"Distance"
		})] [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<EntityUid, float, float>> targets)
		{
			NPCCombatOperator.<UpdateTarget>d__12 <UpdateTarget>d__;
			<UpdateTarget>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateTarget>d__.<>4__this = this;
			<UpdateTarget>d__.owner = owner;
			<UpdateTarget>d__.target = target;
			<UpdateTarget>d__.existingTarget = existingTarget;
			<UpdateTarget>d__.ownerCoordinates = ownerCoordinates;
			<UpdateTarget>d__.blackboard = blackboard;
			<UpdateTarget>d__.radius = radius;
			<UpdateTarget>d__.canMove = canMove;
			<UpdateTarget>d__.xformQuery = xformQuery;
			<UpdateTarget>d__.targets = targets;
			<UpdateTarget>d__.<>1__state = -1;
			<UpdateTarget>d__.<>t__builder.Start<NPCCombatOperator.<UpdateTarget>d__12>(ref <UpdateTarget>d__);
			return <UpdateTarget>d__.<>t__builder.Task;
		}

		// Token: 0x060011B9 RID: 4537
		protected abstract float GetRating(NPCBlackboard blackboard, EntityUid uid, EntityUid existingTarget, float distance, bool canMove, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery);

		// Token: 0x04000AAA RID: 2730
		[Dependency]
		protected readonly IEntityManager EntManager;

		// Token: 0x04000AAB RID: 2731
		private FactionSystem _factions;

		// Token: 0x04000AAC RID: 2732
		protected InteractionSystem Interaction;

		// Token: 0x04000AAD RID: 2733
		private PathfindingSystem _pathfinding;

		// Token: 0x04000AAE RID: 2734
		[DataField("key", false, 1, false, false, null)]
		public string Key = "CombatTarget";

		// Token: 0x04000AAF RID: 2735
		[DataField("keyCoordinates", false, 1, false, false, null)]
		public string KeyCoordinates = "CombatTargetCoordinates";

		// Token: 0x04000AB0 RID: 2736
		private const int MaxConsideredTargets = 10;
	}
}
