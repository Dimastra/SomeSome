using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003BF RID: 959
	[RegisterComponent]
	public sealed class StethoscopeComponent : Component
	{
		// Token: 0x04000C20 RID: 3104
		public bool IsActive;

		// Token: 0x04000C21 RID: 3105
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 2.5f;

		// Token: 0x04000C22 RID: 3106
		[Nullable(1)]
		public EntityTargetAction Action = new EntityTargetAction
		{
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Clothing/Neck/Misc/stethoscope.rsi/icon.png", "/")),
			DisplayName = "stethoscope-verb",
			Priority = -1,
			Event = new StethoscopeActionEvent()
		};
	}
}
