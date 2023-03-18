using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Ghost;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Ghost
{
	// Token: 0x02000301 RID: 769
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedGhostComponent))]
	public sealed class GhostComponent : SharedGhostComponent
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06001340 RID: 4928 RVA: 0x000728F9 File Offset: 0x00070AF9
		// (set) Token: 0x06001341 RID: 4929 RVA: 0x00072901 File Offset: 0x00070B01
		public bool IsAttached { get; set; }

		// Token: 0x040009A3 RID: 2467
		public InstantAction ToggleLightingAction = new InstantAction
		{
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/VerbIcons/light.svg.192dpi.png", "/")),
			DisplayName = "ghost-gui-toggle-lighting-manager-name",
			Description = "ghost-gui-toggle-lighting-manager-desc",
			UserPopup = "ghost-gui-toggle-lighting-manager-popup",
			ClientExclusive = true,
			CheckCanInteract = false,
			Event = new ToggleLightingActionEvent()
		};

		// Token: 0x040009A4 RID: 2468
		public InstantAction ToggleFoVAction = new InstantAction
		{
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/VerbIcons/vv.svg.192dpi.png", "/")),
			DisplayName = "ghost-gui-toggle-fov-name",
			Description = "ghost-gui-toggle-fov-desc",
			UserPopup = "ghost-gui-toggle-fov-popup",
			ClientExclusive = true,
			CheckCanInteract = false,
			Event = new ToggleFoVActionEvent()
		};

		// Token: 0x040009A5 RID: 2469
		public InstantAction ToggleGhostsAction = new InstantAction
		{
			Icon = new SpriteSpecifier.Rsi(new ResourcePath("Mobs/Ghosts/ghost_human.rsi", "/"), "icon"),
			DisplayName = "ghost-gui-toggle-ghost-visibility-name",
			Description = "ghost-gui-toggle-ghost-visibility-desc",
			UserPopup = "ghost-gui-toggle-ghost-visibility-popup",
			ClientExclusive = true,
			CheckCanInteract = false,
			Event = new ToggleGhostsActionEvent()
		};
	}
}
