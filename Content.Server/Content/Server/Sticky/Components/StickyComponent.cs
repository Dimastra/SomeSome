using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Sticky.Components
{
	// Token: 0x02000174 RID: 372
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class StickyComponent : Component
	{
		// Token: 0x04000469 RID: 1129
		[DataField("whitelist", false, 1, false, false, null)]
		[ViewVariables]
		public EntityWhitelist Whitelist;

		// Token: 0x0400046A RID: 1130
		[DataField("blacklist", false, 1, false, false, null)]
		[ViewVariables]
		public EntityWhitelist Blacklist;

		// Token: 0x0400046B RID: 1131
		[DataField("stickDelay", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan StickDelay = TimeSpan.Zero;

		// Token: 0x0400046C RID: 1132
		[DataField("canUnstick", false, 1, false, false, null)]
		[ViewVariables]
		public bool CanUnstick = true;

		// Token: 0x0400046D RID: 1133
		[DataField("unstickDelay", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan UnstickDelay = TimeSpan.Zero;

		// Token: 0x0400046E RID: 1134
		[DataField("stickPopupStart", false, 1, false, false, null)]
		[ViewVariables]
		public string StickPopupStart;

		// Token: 0x0400046F RID: 1135
		[DataField("stickPopupSuccess", false, 1, false, false, null)]
		[ViewVariables]
		public string StickPopupSuccess;

		// Token: 0x04000470 RID: 1136
		[DataField("unstickPopupStart", false, 1, false, false, null)]
		[ViewVariables]
		public string UnstickPopupStart;

		// Token: 0x04000471 RID: 1137
		[DataField("unstickPopupSuccess", false, 1, false, false, null)]
		[ViewVariables]
		public string UnstickPopupSuccess;

		// Token: 0x04000472 RID: 1138
		[ViewVariables]
		public EntityUid? StuckTo;

		// Token: 0x04000473 RID: 1139
		public bool Stick;
	}
}
