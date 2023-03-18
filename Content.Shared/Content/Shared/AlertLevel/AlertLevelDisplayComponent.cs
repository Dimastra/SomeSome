using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.AlertLevel
{
	// Token: 0x02000718 RID: 1816
	[RegisterComponent]
	public sealed class AlertLevelDisplayComponent : Component
	{
		// Token: 0x04001632 RID: 5682
		[Nullable(1)]
		[DataField("alertVisuals", false, 1, false, false, null)]
		public readonly Dictionary<string, string> AlertVisuals = new Dictionary<string, string>();
	}
}
