using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Components;
using Robust.Shared.Analyzers;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000606 RID: 1542
	[NullableContext(1)]
	[RequiresExplicitImplementation]
	public interface IRefreshParts
	{
		// Token: 0x06002127 RID: 8487
		void RefreshParts(IEnumerable<MachinePartComponent> parts);
	}
}
