using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Pointing
{
	// Token: 0x02000266 RID: 614
	public abstract class SharedPointingSystem : EntitySystem
	{
		// Token: 0x020007BB RID: 1979
		[NetSerializable]
		[Serializable]
		protected sealed class PointingArrowComponentState : ComponentState
		{
			// Token: 0x06001816 RID: 6166 RVA: 0x0004D585 File Offset: 0x0004B785
			public PointingArrowComponentState(TimeSpan endTime)
			{
				this.EndTime = endTime;
			}

			// Token: 0x040017E2 RID: 6114
			public TimeSpan EndTime;
		}
	}
}
