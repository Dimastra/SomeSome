using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Ranged
{
	// Token: 0x0200035C RID: 860
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RangedOperator : HTNOperator
	{
		// Token: 0x060011DD RID: 4573 RVA: 0x0005E108 File Offset: 0x0005C308
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
			RangedOperator.<Plan>d__3 <Plan>d__;
			<Plan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Dictionary<string, object>>>.Create();
			<Plan>d__.<>4__this = this;
			<Plan>d__.blackboard = blackboard;
			<Plan>d__.<>1__state = -1;
			<Plan>d__.<>t__builder.Start<RangedOperator.<Plan>d__3>(ref <Plan>d__);
			return <Plan>d__.<>t__builder.Task;
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x0005E154 File Offset: 0x0005C354
		public override void Startup(NPCBlackboard blackboard)
		{
			base.Startup(blackboard);
			NPCRangedCombatComponent ranged = this._entManager.EnsureComponent<NPCRangedCombatComponent>(blackboard.GetValue<EntityUid>("Owner"));
			ranged.Target = blackboard.GetValue<EntityUid>(this.TargetKey);
			float rotSpeed;
			if (blackboard.TryGetValue<float>("RotateSpeed", out rotSpeed, this._entManager))
			{
				ranged.RotationSpeed = new Angle?(new Angle((double)rotSpeed));
			}
			SoundSpecifier losSound;
			if (blackboard.TryGetValue<SoundSpecifier>("SoundTargetInLOS", out losSound, this._entManager))
			{
				ranged.SoundTargetInLOS = losSound;
			}
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			this._entManager.RemoveComponent<NPCRangedCombatComponent>(blackboard.GetValue<EntityUid>("Owner"));
			blackboard.Remove<EntityUid>(this.TargetKey);
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x0005E204 File Offset: 0x0005C404
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			base.Update(blackboard, frameTime);
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			HTNOperatorStatus status = HTNOperatorStatus.Continuing;
			NPCRangedCombatComponent combat;
			if (this._entManager.TryGetComponent<NPCRangedCombatComponent>(owner, ref combat))
			{
				MobStateComponent mobState;
				if (this._entManager.TryGetComponent<MobStateComponent>(combat.Target, ref mobState))
				{
					MobState currentState = mobState.CurrentState;
					if (mobState.CurrentState > this.TargetState)
					{
						status = HTNOperatorStatus.Finished;
						goto IL_77;
					}
				}
				CombatStatus status2 = combat.Status;
				if (status2 != CombatStatus.NotInSight && status2 != CombatStatus.TargetUnreachable)
				{
					if (status2 != CombatStatus.Normal)
					{
						status = HTNOperatorStatus.Failed;
					}
					else
					{
						status = HTNOperatorStatus.Continuing;
					}
				}
				else
				{
					status = HTNOperatorStatus.Failed;
				}
			}
			IL_77:
			if (status != HTNOperatorStatus.Continuing)
			{
				this._entManager.RemoveComponent<NPCRangedCombatComponent>(owner);
			}
			return status;
		}

		// Token: 0x04000ADE RID: 2782
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000ADF RID: 2783
		[DataField("targetKey", false, 1, true, false, null)]
		public string TargetKey;

		// Token: 0x04000AE0 RID: 2784
		[DataField("targetState", false, 1, false, false, null)]
		public MobState TargetState = MobState.Alive;
	}
}
