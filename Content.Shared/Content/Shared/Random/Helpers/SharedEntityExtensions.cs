using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Random.Helpers
{
	// Token: 0x0200021A RID: 538
	public static class SharedEntityExtensions
	{
		// Token: 0x060005FD RID: 1533 RVA: 0x00015134 File Offset: 0x00013334
		public static void RandomOffset(this EntityUid entity, float minX, float maxX, float minY, float maxY)
		{
			IRobustRandom robustRandom = IoCManager.Resolve<IRobustRandom>();
			float randomX = robustRandom.NextFloat() * (maxX - minX) + minX;
			float randomY = robustRandom.NextFloat() * (maxY - minY) + minY;
			Vector2 offset;
			offset..ctor(randomX, randomY);
			IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(entity).LocalPosition += offset;
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00015185 File Offset: 0x00013385
		public static void RandomOffset(this EntityUid entity, float min, float max)
		{
			entity.RandomOffset(min, max, min, max);
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00015191 File Offset: 0x00013391
		public static void RandomOffset(this EntityUid entity, float value)
		{
			value = Math.Abs(value);
			entity.RandomOffset(-value, value);
		}
	}
}
