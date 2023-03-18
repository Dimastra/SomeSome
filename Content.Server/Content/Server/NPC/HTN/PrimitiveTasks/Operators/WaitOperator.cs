using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators
{
	// Token: 0x02000357 RID: 855
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WaitOperator : HTNOperator
	{
		// Token: 0x060011CE RID: 4558 RVA: 0x0005DD04 File Offset: 0x0005BF04
		public override HTNOperatorStatus Update(NPCBlackboard blackboard, float frameTime)
		{
			float timer;
			if (!blackboard.TryGetValue<float>(this.Key, out timer, this._entManager))
			{
				return HTNOperatorStatus.Finished;
			}
			timer -= frameTime;
			blackboard.SetValue(this.Key, timer);
			if (timer > 0f)
			{
				return HTNOperatorStatus.Continuing;
			}
			return HTNOperatorStatus.Finished;
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x0005DD4A File Offset: 0x0005BF4A
		public override void Shutdown(NPCBlackboard blackboard, HTNOperatorStatus status)
		{
			base.Shutdown(blackboard, status);
			if (status != HTNOperatorStatus.BetterPlan)
			{
				blackboard.Remove<float>(this.Key);
			}
		}

		// Token: 0x04000ACE RID: 2766
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000ACF RID: 2767
		[DataField("key", false, 1, true, false, null)]
		public string Key = string.Empty;
	}
}
