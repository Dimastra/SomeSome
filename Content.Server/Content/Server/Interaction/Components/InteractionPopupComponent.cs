using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Interaction.Components
{
	// Token: 0x02000448 RID: 1096
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(InteractionPopupSystem)
	})]
	public sealed class InteractionPopupComponent : Component
	{
		// Token: 0x04000DD4 RID: 3540
		[DataField("interactDelay", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan InteractDelay = TimeSpan.FromSeconds(1.0);

		// Token: 0x04000DD5 RID: 3541
		[DataField("interactSuccessString", false, 1, false, false, null)]
		public string InteractSuccessString;

		// Token: 0x04000DD6 RID: 3542
		[DataField("interactFailureString", false, 1, false, false, null)]
		public string InteractFailureString;

		// Token: 0x04000DD7 RID: 3543
		[DataField("interactSuccessSound", false, 1, false, false, null)]
		public SoundSpecifier InteractSuccessSound;

		// Token: 0x04000DD8 RID: 3544
		[DataField("interactFailureSound", false, 1, false, false, null)]
		public SoundSpecifier InteractFailureSound;

		// Token: 0x04000DD9 RID: 3545
		[DataField("successChance", false, 1, false, false, null)]
		public float SuccessChance = 1f;

		// Token: 0x04000DDA RID: 3546
		[DataField("messagePerceivedByOthers", false, 1, false, false, null)]
		public string MessagePerceivedByOthers;

		// Token: 0x04000DDB RID: 3547
		[DataField("soundPerceivedByOthers", false, 1, false, false, null)]
		public bool SoundPerceivedByOthers = true;

		// Token: 0x04000DDC RID: 3548
		[ViewVariables]
		public TimeSpan LastInteractTime;
	}
}
