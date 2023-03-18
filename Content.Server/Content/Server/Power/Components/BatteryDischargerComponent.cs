using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.NodeGroups;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.Components
{
	// Token: 0x020002AD RID: 685
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BatteryDischargerComponent : BasePowerNetComponent
	{
		// Token: 0x06000DE5 RID: 3557 RVA: 0x00047AE3 File Offset: 0x00045CE3
		protected override void AddSelfToNet(IPowerNet net)
		{
			net.AddDischarger(this);
		}

		// Token: 0x06000DE6 RID: 3558 RVA: 0x00047AEC File Offset: 0x00045CEC
		protected override void RemoveSelfFromNet(IPowerNet net)
		{
			net.RemoveDischarger(this);
		}
	}
}
