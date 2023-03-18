using System;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Humanoid
{
	// Token: 0x0200040F RID: 1039
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HumanoidMarkingModifierMarkingSetMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000C3C RID: 3132 RVA: 0x000288CF File Offset: 0x00026ACF
		public MarkingSet MarkingSet { get; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000C3D RID: 3133 RVA: 0x000288D7 File Offset: 0x00026AD7
		public bool ResendState { get; }

		// Token: 0x06000C3E RID: 3134 RVA: 0x000288DF File Offset: 0x00026ADF
		public HumanoidMarkingModifierMarkingSetMessage(MarkingSet set, bool resendState)
		{
			this.MarkingSet = set;
			this.ResendState = resendState;
		}
	}
}
