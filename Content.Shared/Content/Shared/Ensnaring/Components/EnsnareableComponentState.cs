using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004BC RID: 1212
	[NetSerializable]
	[Serializable]
	public sealed class EnsnareableComponentState : ComponentState
	{
		// Token: 0x06000EAD RID: 3757 RVA: 0x0002F465 File Offset: 0x0002D665
		public EnsnareableComponentState(bool isEnsnared)
		{
			this.IsEnsnared = isEnsnared;
		}

		// Token: 0x04000DCE RID: 3534
		public readonly bool IsEnsnared;
	}
}
