using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000410 RID: 1040
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidMarkingModifierBaseLayersSetMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000C3F RID: 3135 RVA: 0x000288F5 File Offset: 0x00026AF5
		public HumanoidMarkingModifierBaseLayersSetMessage(HumanoidVisualLayers layer, HumanoidAppearanceState.CustomBaseLayerInfo? info, bool resendState)
		{
			this.Layer = layer;
			this.Info = info;
			this.ResendState = resendState;
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000C40 RID: 3136 RVA: 0x00028912 File Offset: 0x00026B12
		public HumanoidVisualLayers Layer { get; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000C41 RID: 3137 RVA: 0x0002891A File Offset: 0x00026B1A
		public HumanoidAppearanceState.CustomBaseLayerInfo? Info { get; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x00028922 File Offset: 0x00026B22
		public bool ResendState { get; }
	}
}
