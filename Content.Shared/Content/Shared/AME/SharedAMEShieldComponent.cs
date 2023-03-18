using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.AME
{
	// Token: 0x02000717 RID: 1815
	[Virtual]
	public class SharedAMEShieldComponent : Component
	{
		// Token: 0x02000889 RID: 2185
		[NetSerializable]
		[Serializable]
		public enum AMEShieldVisuals
		{
			// Token: 0x04001A5C RID: 6748
			Core,
			// Token: 0x04001A5D RID: 6749
			CoreState
		}

		// Token: 0x0200088A RID: 2186
		public enum AMECoreState
		{
			// Token: 0x04001A5F RID: 6751
			Off,
			// Token: 0x04001A60 RID: 6752
			Weak,
			// Token: 0x04001A61 RID: 6753
			Strong
		}
	}
}
