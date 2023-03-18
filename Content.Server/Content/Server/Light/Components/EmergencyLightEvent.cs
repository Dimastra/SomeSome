using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Light.Components
{
	// Token: 0x02000418 RID: 1048
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmergencyLightEvent : EntityEventArgs
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600154E RID: 5454 RVA: 0x0006FF97 File Offset: 0x0006E197
		public EmergencyLightComponent Component { get; }

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x0006FF9F File Offset: 0x0006E19F
		public EmergencyLightState State { get; }

		// Token: 0x06001550 RID: 5456 RVA: 0x0006FFA7 File Offset: 0x0006E1A7
		public EmergencyLightEvent(EmergencyLightComponent component, EmergencyLightState state)
		{
			this.Component = component;
			this.State = state;
		}
	}
}
