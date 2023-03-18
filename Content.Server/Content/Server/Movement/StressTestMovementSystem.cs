using System;
using Content.Server.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Movement
{
	// Token: 0x0200038E RID: 910
	internal sealed class StressTestMovementSystem : EntitySystem
	{
		// Token: 0x060012AD RID: 4781 RVA: 0x00060BDC File Offset: 0x0005EDDC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (StressTestMovementComponent stressTest in this.EntityManager.EntityQuery<StressTestMovementComponent>(true))
			{
				TransformComponent component = this.EntityManager.GetComponent<TransformComponent>(stressTest.Owner);
				stressTest.Progress += frameTime;
				if (stressTest.Progress > 1f)
				{
					stressTest.Progress -= 1f;
				}
				float x = MathF.Sin(stressTest.Progress * 6.2831855f);
				float y = MathF.Cos(stressTest.Progress * 6.2831855f);
				component.WorldPosition = stressTest.Origin + new Vector2(x, y) * 5f;
			}
		}
	}
}
