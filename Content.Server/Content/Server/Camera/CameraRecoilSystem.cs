using System;
using System.Runtime.CompilerServices;
using Content.Shared.Camera;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Camera
{
	// Token: 0x020006F1 RID: 1777
	public sealed class CameraRecoilSystem : SharedCameraRecoilSystem
	{
		// Token: 0x06002521 RID: 9505 RVA: 0x000C23A3 File Offset: 0x000C05A3
		[NullableContext(2)]
		public override void KickCamera(EntityUid euid, Vector2 kickback, CameraRecoilComponent component = null)
		{
			if (!base.Resolve<CameraRecoilComponent>(euid, ref component, false))
			{
				return;
			}
			base.RaiseNetworkEvent(new CameraKickEvent(euid, kickback), euid);
		}
	}
}
