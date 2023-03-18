using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Melee
{
	// Token: 0x0200035D RID: 861
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeOperator : HTNOperator
	{
		// Token: 0x060011E2 RID: 4578 RVA: 0x0005E2A8 File Offset: 0x0005C4A8
		public override void Startup(NPCBlackboard blackboard)
		{
			base.Startup(blackboard);
			NPCMeleeCombatComponent npcmeleeCombatComponent = this._entManager.EnsureComponent<NPCMeleeCombatComponent>(blackboard.GetValue<EntityUid>("Owner"));
			npcmeleeCombatComponent.MissChance = blackboard.GetValueOrDefault<float>("MeleeMissChance", this._entManager);
			npcmeleeCombatComponent.Target = blackboard.GetValue<EntityUid>(this.TargetKey);
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x0005E2FC File Offset: 0x0005C4FC
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
			MeleeOperator.<Plan>d__4 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<MeleeOperator.<Plan>d__4>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x0005E347 File Offset: 0x0005C547
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			this._entManager.RemoveComponent<NPCMeleeCombatComponent>(blackboard.GetValue<EntityUid>("Owner"));
			blackboard.Remove<EntityUid>(this.TargetKey);
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x0005E378 File Offset: 0x0005C578
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			base.Update(blackboard, frameTime);
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			HTNOperatorStatus status = HTNOperatorStatus.Continuing;
			NPCMeleeCombatComponent combat;
			if (this._entManager.TryGetComponent<NPCMeleeCombatComponent>(owner, ref combat))
			{
				MobStateComponent mobState;
				if (this._entManager.TryGetComponent<MobStateComponent>(combat.Target, ref mobState))
				{
					MobState currentState = mobState.CurrentState;
					if (mobState.CurrentState > this.TargetState)
					{
						status = HTNOperatorStatus.Finished;
						goto IL_6D;
					}
				}
				CombatStatus status2 = combat.Status;
				if (status2 == CombatStatus.TargetOutOfRange || status2 == CombatStatus.Normal)
				{
					status = HTNOperatorStatus.Continuing;
				}
				else
				{
					status = HTNOperatorStatus.Failed;
				}
			}
			IL_6D:
			if (status != HTNOperatorStatus.Continuing)
			{
				this._entManager.RemoveComponent<NPCMeleeCombatComponent>(owner);
			}
			return status;
		}

		// Token: 0x04000AE1 RID: 2785
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000AE2 RID: 2786
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey;

		// Token: 0x04000AE3 RID: 2787
		[DataField("targetState", false, 1, false, false, null)]
		public MobState TargetState = MobState.Alive;
	}
}
