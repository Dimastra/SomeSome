using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.PDA
{
	// Token: 0x020001BC RID: 444
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PDABorderColorComponent : Component
	{
		// Token: 0x04000591 RID: 1425
		[DataField("borderColor", false, 1, true, false, null)]
		public string BorderColor;

		// Token: 0x04000592 RID: 1426
		[DataField("accentHColor", false, 1, false, false, null)]
		public string AccentHColor;

		// Token: 0x04000593 RID: 1427
		[DataField("accentVColor", false, 1, false, false, null)]
		public string AccentVColor;
	}
}
