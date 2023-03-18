using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Sound.Components
{
	// Token: 0x02000187 RID: 391
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EmitSoundOnActivateComponent : BaseEmitSoundComponent
	{
		// Token: 0x0400045A RID: 1114
		[DataField("handle", false, 1, false, false, null)]
		public bool Handle = true;
	}
}
