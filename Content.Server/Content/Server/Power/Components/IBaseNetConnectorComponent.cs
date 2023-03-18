using System;
using System.Runtime.CompilerServices;

namespace Content.Server.Power.Components
{
	// Token: 0x020002A5 RID: 677
	[NullableContext(2)]
	public interface IBaseNetConnectorComponent<in TNetType>
	{
		// Token: 0x170001DE RID: 478
		// (set) Token: 0x06000DC0 RID: 3520
		TNetType Net { set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000DC1 RID: 3521
		Voltage Voltage { get; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000DC2 RID: 3522
		string NodeId { get; }
	}
}
