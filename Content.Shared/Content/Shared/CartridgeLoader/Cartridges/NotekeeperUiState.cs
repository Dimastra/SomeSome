using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges
{
	// Token: 0x02000626 RID: 1574
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class NotekeeperUiState : BoundUserInterfaceState
	{
		// Token: 0x0600130E RID: 4878 RVA: 0x0003FA03 File Offset: 0x0003DC03
		public NotekeeperUiState(List<string> notes)
		{
			this.Notes = notes;
		}

		// Token: 0x040012F2 RID: 4850
		public List<string> Notes;
	}
}
