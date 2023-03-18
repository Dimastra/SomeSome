using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Containers
{
	// Token: 0x020005E8 RID: 1512
	[RegisterComponent]
	public sealed class ContainerFillComponent : Component
	{
		// Token: 0x040013FF RID: 5119
		[Nullable(1)]
		[DataField("containers", false, 1, false, false, typeof(ContainerFillSerializer))]
		public readonly Dictionary<string, List<string>> Containers = new Dictionary<string, List<string>>();

		// Token: 0x04001400 RID: 5120
		[DataField("ignoreConstructionSpawn", false, 1, false, false, null)]
		public bool IgnoreConstructionSpawn = true;
	}
}
