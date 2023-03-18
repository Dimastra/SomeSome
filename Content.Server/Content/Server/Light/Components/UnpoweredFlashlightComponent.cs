using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x02000420 RID: 1056
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class UnpoweredFlashlightComponent : Component
	{
		// Token: 0x04000D4C RID: 3404
		[DataField("toggleFlashlightSound", false, 1, false, false, null)]
		public SoundSpecifier ToggleSound = new SoundPathSpecifier("/Audio/Items/flashlight_pda.ogg", null);

		// Token: 0x04000D4D RID: 3405
		[ViewVariables]
		public bool LightOn;

		// Token: 0x04000D4E RID: 3406
		[DataField("toggleAction", false, 1, true, false, null)]
		public InstantAction ToggleAction = new InstantAction();
	}
}
