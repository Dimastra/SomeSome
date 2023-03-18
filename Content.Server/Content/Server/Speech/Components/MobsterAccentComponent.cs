using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001C5 RID: 453
	[RegisterComponent]
	public sealed class MobsterAccentComponent : Component
	{
		// Token: 0x04000553 RID: 1363
		[DataField("isBoss", false, 1, false, false, null)]
		public bool IsBoss = true;
	}
}
