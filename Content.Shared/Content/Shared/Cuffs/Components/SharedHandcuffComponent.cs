using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Cuffs.Components
{
	// Token: 0x0200054B RID: 1355
	[NetworkedComponent]
	public abstract class SharedHandcuffComponent : Component
	{
		// Token: 0x02000839 RID: 2105
		[NullableContext(2)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class HandcuffedComponentState : ComponentState
		{
			// Token: 0x1700051D RID: 1309
			// (get) Token: 0x06001923 RID: 6435 RVA: 0x0004F960 File Offset: 0x0004DB60
			public string IconState { get; }

			// Token: 0x06001924 RID: 6436 RVA: 0x0004F968 File Offset: 0x0004DB68
			public HandcuffedComponentState(string iconState)
			{
				this.IconState = iconState;
			}
		}
	}
}
