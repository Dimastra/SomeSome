using System;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000354 RID: 852
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RotateToTargetOperator : HTNOperator
	{
		// Token: 0x060011C5 RID: 4549 RVA: 0x0005DB7C File Offset: 0x0005BD7C
		public override void Initialize(IEntitySystemManager sysManager)
		{
			base.Initialize(sysManager);
			this._rotate = sysManager.GetEntitySystem<RotateToFaceSystem>();
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x0005DB91 File Offset: 0x0005BD91
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			blackboard.Remove<Angle>(this.TargetKey);
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x0005DBA8 File Offset: 0x0005BDA8
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			Angle rotateTarget;
			if (!blackboard.TryGetValue<Angle>(this.TargetKey, out rotateTarget, this._entityManager))
			{
				return HTNOperatorStatus.Failed;
			}
			float rotateSpeed;
			if (!blackboard.TryGetValue<float>(this.RotationSpeedKey, out rotateSpeed, this._entityManager))
			{
				return HTNOperatorStatus.Failed;
			}
			EntityUid owner = blackboard.GetValue<EntityUid>("Owner");
			if (this._rotate.TryRotateTo(owner, rotateTarget, frameTime, this.Tolerance, (double)rotateSpeed, null))
			{
				return HTNOperatorStatus.Finished;
			}
			return HTNOperatorStatus.Continuing;
		}

		// Token: 0x04000AC5 RID: 2757
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000AC6 RID: 2758
		private RotateToFaceSystem _rotate;

		// Token: 0x04000AC7 RID: 2759
		[DataField("targetKey", false, 1, false, false, null)]
		public string TargetKey = "RotateTarget";

		// Token: 0x04000AC8 RID: 2760
		[DataField("rotateSpeedKey", false, 1, false, false, null)]
		public string RotationSpeedKey = "RotateSpeed";

		// Token: 0x04000AC9 RID: 2761
		[DataField("tolerance", false, 1, false, false, null)]
		public Angle Tolerance = Angle.FromDegrees(1.0);
	}
}
