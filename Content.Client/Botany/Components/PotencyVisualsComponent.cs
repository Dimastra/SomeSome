using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Botany.Components
{
	// Token: 0x02000418 RID: 1048
	[RegisterComponent]
	public sealed class PotencyVisualsComponent : Component
	{
		// Token: 0x04000D0D RID: 3341
		[DataField("minimumScale", false, 1, false, false, null)]
		public float MinimumScale = 0.5f;

		// Token: 0x04000D0E RID: 3342
		[DataField("maximumScale", false, 1, false, false, null)]
		public float MaximumScale = 1.5f;
	}
}
