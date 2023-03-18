using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000618 RID: 1560
	[Virtual]
	[NetSerializable]
	[Serializable]
	public class CartridgeLoaderUiState : BoundUserInterfaceState
	{
		// Token: 0x040012DB RID: 4827
		public EntityUid? ActiveUI;

		// Token: 0x040012DC RID: 4828
		[Nullable(1)]
		public List<EntityUid> Programs = new List<EntityUid>();
	}
}
