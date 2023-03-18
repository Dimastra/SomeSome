using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Camera
{
	// Token: 0x0200063C RID: 1596
	[NetSerializable]
	[Serializable]
	public sealed class CameraKickEvent : EntityEventArgs
	{
		// Token: 0x06001337 RID: 4919 RVA: 0x0003FF68 File Offset: 0x0003E168
		public CameraKickEvent(EntityUid euid, Vector2 recoil)
		{
			this.Recoil = recoil;
			this.Euid = euid;
		}

		// Token: 0x04001334 RID: 4916
		public readonly EntityUid Euid;

		// Token: 0x04001335 RID: 4917
		public readonly Vector2 Recoil;
	}
}
