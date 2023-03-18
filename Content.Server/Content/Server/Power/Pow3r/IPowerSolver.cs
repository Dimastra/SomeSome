using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Power.Pow3r
{
	// Token: 0x02000279 RID: 633
	[NullableContext(1)]
	public interface IPowerSolver
	{
		// Token: 0x06000CB7 RID: 3255
		void Tick(float frameTime, PowerState state, int parallel);
	}
}
