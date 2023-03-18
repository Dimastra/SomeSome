using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Cooldown
{
	// Token: 0x0200055A RID: 1370
	public static class Cooldowns
	{
		// Token: 0x060010A1 RID: 4257 RVA: 0x0003630C File Offset: 0x0003450C
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		public static ValueTuple<TimeSpan, TimeSpan> FromNow(TimeSpan offset, [Nullable(2)] IGameTiming gameTiming = null)
		{
			TimeSpan curTime = (gameTiming ?? IoCManager.Resolve<IGameTiming>()).CurTime;
			return new ValueTuple<TimeSpan, TimeSpan>(curTime, curTime + offset);
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00036329 File Offset: 0x00034529
		[return: TupleElementNames(new string[]
		{
			"start",
			"end"
		})]
		public static ValueTuple<TimeSpan, TimeSpan> SecondsFromNow(double seconds, [Nullable(2)] IGameTiming gameTiming = null)
		{
			return Cooldowns.FromNow(TimeSpan.FromSeconds(seconds), gameTiming);
		}
	}
}
