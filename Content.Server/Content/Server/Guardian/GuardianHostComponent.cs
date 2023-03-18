using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Guardian
{
	// Token: 0x02000484 RID: 1156
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GuardianHostComponent : Component
	{
		// Token: 0x04000E79 RID: 3705
		public EntityUid? HostedGuardian;

		// Token: 0x04000E7A RID: 3706
		[ViewVariables]
		public ContainerSlot GuardianContainer;

		// Token: 0x04000E7B RID: 3707
		[DataField("action", false, 1, false, false, null)]
		public InstantAction Action = new InstantAction
		{
			DisplayName = "action-name-guardian",
			Description = "action-description-guardian",
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/Actions/manifest.png", "/")),
			UseDelay = new TimeSpan?(TimeSpan.FromSeconds(2.0)),
			CheckCanInteract = false,
			Event = new GuardianToggleActionEvent()
		};
	}
}
