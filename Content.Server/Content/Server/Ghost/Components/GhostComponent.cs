using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Ghost;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Ghost.Components
{
	// Token: 0x0200049E RID: 1182
	[RegisterComponent]
	[ComponentReference(typeof(SharedGhostComponent))]
	public sealed class GhostComponent : SharedGhostComponent
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x0007C316 File Offset: 0x0007A516
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x0007C31E File Offset: 0x0007A51E
		public TimeSpan TimeOfDeath { get; set; } = TimeSpan.Zero;

		// Token: 0x04000EBF RID: 3775
		[DataField("booRadius", false, 1, false, false, null)]
		public float BooRadius = 3f;

		// Token: 0x04000EC0 RID: 3776
		[DataField("booMaxTargets", false, 1, false, false, null)]
		public int BooMaxTargets = 3;

		// Token: 0x04000EC1 RID: 3777
		[Nullable(1)]
		[DataField("action", false, 1, false, false, null)]
		public InstantAction Action = new InstantAction
		{
			UseDelay = new TimeSpan?(TimeSpan.FromSeconds(120.0)),
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Interface/Actions/scream.png", "/")),
			DisplayName = "action-name-boo",
			Description = "action-description-boo",
			CheckCanInteract = false,
			Event = new BooActionEvent()
		};
	}
}
