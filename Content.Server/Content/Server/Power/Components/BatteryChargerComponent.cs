using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.Components
{
	// Token: 0x020002AA RID: 682
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BatteryChargerComponent : BasePowerNetComponent
	{
		// Token: 0x06000DD6 RID: 3542 RVA: 0x0004795D File Offset: 0x00045B5D
		protected override void AddSelfToNet(IPowerNet net)
		{
			net.AddCharger(this);
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x00047966 File Offset: 0x00045B66
		protected override void RemoveSelfFromNet(IPowerNet net)
		{
			net.RemoveCharger(this);
		}
	}
}
