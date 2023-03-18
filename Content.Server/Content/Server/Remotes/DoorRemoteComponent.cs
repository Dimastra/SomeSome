using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Remotes
{
	// Token: 0x02000246 RID: 582
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(DoorRemoteSystem)
	})]
	public sealed class DoorRemoteComponent : Component
	{
		// Token: 0x04000725 RID: 1829
		public DoorRemoteComponent.OperatingMode Mode;

		// Token: 0x02000920 RID: 2336
		public enum OperatingMode : byte
		{
			// Token: 0x04001ED8 RID: 7896
			OpenClose,
			// Token: 0x04001ED9 RID: 7897
			ToggleBolts,
			// Token: 0x04001EDA RID: 7898
			ToggleEmergencyAccess
		}
	}
}
