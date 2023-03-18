using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Pointing
{
	// Token: 0x02000265 RID: 613
	[NetSerializable]
	[Serializable]
	public sealed class PointingAttemptEvent : EntityEventArgs
	{
		// Token: 0x06000711 RID: 1809 RVA: 0x00018515 File Offset: 0x00016715
		public PointingAttemptEvent(EntityUid target)
		{
			this.Target = target;
		}

		// Token: 0x040006F0 RID: 1776
		public EntityUid Target;
	}
}
