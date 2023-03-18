using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B3 RID: 1459
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChameleonPrototypeSelectedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060011D4 RID: 4564 RVA: 0x0003A889 File Offset: 0x00038A89
		public ChameleonPrototypeSelectedMessage(string selectedId)
		{
			this.SelectedId = selectedId;
		}

		// Token: 0x0400106B RID: 4203
		public readonly string SelectedId;
	}
}
