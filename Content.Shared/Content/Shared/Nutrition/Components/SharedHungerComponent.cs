using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components
{
	// Token: 0x020002B4 RID: 692
	[NetworkedComponent]
	public abstract class SharedHungerComponent : Component
	{
		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060007BD RID: 1981
		[ViewVariables]
		public abstract HungerThreshold CurrentHungerThreshold { get; }

		// Token: 0x020007C5 RID: 1989
		[NetSerializable]
		[Serializable]
		protected sealed class HungerComponentState : ComponentState
		{
			// Token: 0x170004F9 RID: 1273
			// (get) Token: 0x06001824 RID: 6180 RVA: 0x0004D8C0 File Offset: 0x0004BAC0
			public HungerThreshold CurrentThreshold { get; }

			// Token: 0x06001825 RID: 6181 RVA: 0x0004D8C8 File Offset: 0x0004BAC8
			public HungerComponentState(HungerThreshold currentThreshold)
			{
				this.CurrentThreshold = currentThreshold;
			}
		}
	}
}
