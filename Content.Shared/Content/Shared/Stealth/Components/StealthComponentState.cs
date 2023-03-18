using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Stealth.Components
{
	// Token: 0x02000153 RID: 339
	[NetSerializable]
	[Serializable]
	public sealed class StealthComponentState : ComponentState
	{
		// Token: 0x0600041A RID: 1050 RVA: 0x0001065D File Offset: 0x0000E85D
		public StealthComponentState(float stealthLevel, TimeSpan? lastUpdated, bool enabled)
		{
			this.Visibility = stealthLevel;
			this.LastUpdated = lastUpdated;
			this.Enabled = enabled;
		}

		// Token: 0x040003EC RID: 1004
		public readonly float Visibility;

		// Token: 0x040003ED RID: 1005
		public readonly TimeSpan? LastUpdated;

		// Token: 0x040003EE RID: 1006
		public readonly bool Enabled;
	}
}
