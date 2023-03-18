using System;
using System.Runtime.CompilerServices;
using Content.Shared.Camera;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Camera
{
	// Token: 0x02000410 RID: 1040
	public sealed class CameraRecoilSystem : SharedCameraRecoilSystem
	{
		// Token: 0x060019A3 RID: 6563 RVA: 0x00093350 File Offset: 0x00091550
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<CameraKickEvent>(new EntityEventHandler<CameraKickEvent>(this.OnCameraKick), null, null);
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0009336C File Offset: 0x0009156C
		[NullableContext(1)]
		private void OnCameraKick(CameraKickEvent ev)
		{
			this.KickCamera(ev.Euid, ev.Recoil, null);
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x00093384 File Offset: 0x00091584
		[NullableContext(2)]
		public override void KickCamera(EntityUid uid, Vector2 recoil, CameraRecoilComponent component = null)
		{
			if (!base.Resolve<CameraRecoilComponent>(uid, ref component, false))
			{
				return;
			}
			float num = component.CurrentKick.Length / 1f;
			component.CurrentKick += recoil * (1f - num);
			if (component.CurrentKick.Length > 1f)
			{
				component.CurrentKick = component.CurrentKick.Normalized * 1f;
			}
			component.LastKickTime = 0f;
		}
	}
}
