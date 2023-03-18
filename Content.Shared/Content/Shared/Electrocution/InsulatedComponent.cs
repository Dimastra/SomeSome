using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Electrocution
{
	// Token: 0x020004CE RID: 1230
	[Access(new Type[]
	{
		typeof(SharedElectrocutionSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class InsulatedComponent : Component
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0002FD94 File Offset: 0x0002DF94
		// (set) Token: 0x06000EDF RID: 3807 RVA: 0x0002FD9C File Offset: 0x0002DF9C
		[DataField("coefficient", false, 1, false, false, null)]
		public float SiemensCoefficient { get; set; }
	}
}
