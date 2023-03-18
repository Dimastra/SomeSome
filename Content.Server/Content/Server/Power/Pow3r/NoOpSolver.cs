using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Power.Pow3r
{
	// Token: 0x0200027A RID: 634
	public sealed class NoOpSolver : IPowerSolver
	{
		// Token: 0x06000CB8 RID: 3256 RVA: 0x00042EC8 File Offset: 0x000410C8
		[NullableContext(1)]
		public void Tick(float frameTime, PowerState state, int parallel)
		{
		}
	}
}
