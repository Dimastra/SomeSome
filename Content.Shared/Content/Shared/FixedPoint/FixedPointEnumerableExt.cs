using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Shared.FixedPoint
{
	// Token: 0x0200048D RID: 1165
	public static class FixedPointEnumerableExt
	{
		// Token: 0x06000E38 RID: 3640 RVA: 0x0002DC0C File Offset: 0x0002BE0C
		[NullableContext(1)]
		public static FixedPoint2 Sum(this IEnumerable<FixedPoint2> source)
		{
			FixedPoint2 acc = FixedPoint2.Zero;
			foreach (FixedPoint2 i in source)
			{
				acc += i;
			}
			return acc;
		}
	}
}
