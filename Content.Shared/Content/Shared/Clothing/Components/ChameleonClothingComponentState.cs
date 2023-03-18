using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing.Components
{
	// Token: 0x020005B1 RID: 1457
	[NetSerializable]
	[Serializable]
	public sealed class ChameleonClothingComponentState : ComponentState
	{
		// Token: 0x04001068 RID: 4200
		[Nullable(2)]
		public string SelectedId;
	}
}
