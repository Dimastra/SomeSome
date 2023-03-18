using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B6 RID: 1462
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ClothingComponentState : ComponentState
	{
		// Token: 0x060011D6 RID: 4566 RVA: 0x0003A8B9 File Offset: 0x00038AB9
		public ClothingComponentState(string equippedPrefix)
		{
			this.EquippedPrefix = equippedPrefix;
		}

		// Token: 0x04001077 RID: 4215
		public string EquippedPrefix;
	}
}
