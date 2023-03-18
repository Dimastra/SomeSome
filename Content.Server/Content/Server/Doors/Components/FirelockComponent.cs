using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Doors.Components
{
	// Token: 0x02000549 RID: 1353
	[RegisterComponent]
	public sealed class FirelockComponent : Component
	{
		// Token: 0x04001240 RID: 4672
		[DataField("lockedPryTimeModifier", false, 1, false, false, null)]
		[ViewVariables]
		public float LockedPryTimeModifier = 1.5f;

		// Token: 0x04001241 RID: 4673
		[DataField("autocloseDelay", false, 1, false, false, null)]
		public TimeSpan AutocloseDelay = TimeSpan.FromSeconds(3.0);

		// Token: 0x04001242 RID: 4674
		[DataField("pressureThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float PressureThreshold = 20f;

		// Token: 0x04001243 RID: 4675
		[DataField("temperatureThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float TemperatureThreshold = 330f;

		// Token: 0x04001244 RID: 4676
		[DataField("alarmAutoClose", false, 1, false, false, null)]
		[ViewVariables]
		public bool AlarmAutoClose = true;
	}
}
