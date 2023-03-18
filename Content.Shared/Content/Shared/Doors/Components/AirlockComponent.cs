using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004EA RID: 1258
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access]
	public sealed class AirlockComponent : Component
	{
		// Token: 0x04000E47 RID: 3655
		[ViewVariables]
		[DataField("safety", false, 1, false, false, null)]
		public bool Safety = true;

		// Token: 0x04000E48 RID: 3656
		[ViewVariables]
		[DataField("emergencyAccess", false, 1, false, false, null)]
		public bool EmergencyAccess;

		// Token: 0x04000E49 RID: 3657
		[DataField("boltUpSound", false, 1, false, false, null)]
		public SoundSpecifier BoltUpSound = new SoundPathSpecifier("/Audio/Machines/boltsup.ogg", null);

		// Token: 0x04000E4A RID: 3658
		[DataField("boltDownSound", false, 1, false, false, null)]
		public SoundSpecifier BoltDownSound = new SoundPathSpecifier("/Audio/Machines/boltsdown.ogg", null);

		// Token: 0x04000E4B RID: 3659
		[DataField("poweredPryModifier", false, 1, false, false, null)]
		public readonly float PoweredPryModifier = 9f;

		// Token: 0x04000E4C RID: 3660
		[DataField("openPanelVisible", false, 1, false, false, null)]
		public bool OpenPanelVisible;

		// Token: 0x04000E4D RID: 3661
		[DataField("keepOpenIfClicked", false, 1, false, false, null)]
		public bool KeepOpenIfClicked;

		// Token: 0x04000E4E RID: 3662
		public bool BoltsDown;

		// Token: 0x04000E4F RID: 3663
		public bool BoltLightsEnabled = true;

		// Token: 0x04000E50 RID: 3664
		[ViewVariables]
		public bool BoltWireCut;

		// Token: 0x04000E51 RID: 3665
		[ViewVariables]
		public bool AutoClose = true;

		// Token: 0x04000E52 RID: 3666
		[DataField("autoCloseDelay", false, 1, false, false, null)]
		public TimeSpan AutoCloseDelay = TimeSpan.FromSeconds(5.0);

		// Token: 0x04000E53 RID: 3667
		[ViewVariables]
		public float AutoCloseDelayModifier = 1f;
	}
}
