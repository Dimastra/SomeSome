using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Light.Component
{
	// Token: 0x02000372 RID: 882
	[NetworkedComponent]
	public abstract class SharedEmergencyLightComponent : Component
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000A48 RID: 2632 RVA: 0x00022329 File Offset: 0x00020529
		// (set) Token: 0x06000A49 RID: 2633 RVA: 0x00022331 File Offset: 0x00020531
		public bool Enabled { get; set; }
	}
}
