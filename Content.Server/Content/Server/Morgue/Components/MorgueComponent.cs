using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Morgue.Components
{
	// Token: 0x0200039C RID: 924
	[RegisterComponent]
	public sealed class MorgueComponent : Component
	{
		// Token: 0x04000B8B RID: 2955
		[ViewVariables]
		[DataField("doSoulBeep", false, 1, false, false, null)]
		public bool DoSoulBeep = true;

		// Token: 0x04000B8C RID: 2956
		[ViewVariables]
		public float AccumulatedFrameTime;

		// Token: 0x04000B8D RID: 2957
		[ViewVariables]
		public float BeepTime = 10f;

		// Token: 0x04000B8E RID: 2958
		[Nullable(1)]
		[DataField("occupantHasSoulAlarmSound", false, 1, false, false, null)]
		public SoundSpecifier OccupantHasSoulAlarmSound = new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg", null);
	}
}
