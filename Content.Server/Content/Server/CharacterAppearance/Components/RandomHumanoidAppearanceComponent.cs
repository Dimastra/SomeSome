using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.CharacterAppearance.Components
{
	// Token: 0x0200045D RID: 1117
	[RegisterComponent]
	public sealed class RandomHumanoidAppearanceComponent : Component
	{
		// Token: 0x04000E16 RID: 3606
		[DataField("randomizeName", false, 1, false, false, null)]
		public bool RandomizeName = true;
	}
}
