using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Shared.Camera
{
	// Token: 0x0200063B RID: 1595
	public abstract class SharedCameraRecoilSystem : EntitySystem
	{
		// Token: 0x06001333 RID: 4915 RVA: 0x0003FDBB File Offset: 0x0003DFBB
		public override void Initialize()
		{
			base.Initialize();
			this._log = Logger.GetSawmill("ecs.systems.SharedCameraRecoilSystem");
		}

		// Token: 0x06001334 RID: 4916
		[NullableContext(2)]
		public abstract void KickCamera(EntityUid euid, Vector2 kickback, CameraRecoilComponent component = null);

		// Token: 0x06001335 RID: 4917 RVA: 0x0003FDD4 File Offset: 0x0003DFD4
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			foreach (ValueTuple<SharedEyeComponent, CameraRecoilComponent> valueTuple in this.EntityManager.EntityQuery<SharedEyeComponent, CameraRecoilComponent>(true))
			{
				CameraRecoilComponent recoil = valueTuple.Item2;
				SharedEyeComponent eye = valueTuple.Item1;
				if (recoil.CurrentKick.Length <= 0.005f)
				{
					recoil.CurrentKick = Vector2.Zero;
					eye.Offset = recoil.BaseOffset + recoil.CurrentKick;
				}
				else
				{
					Vector2 normalized = recoil.CurrentKick.Normalized;
					recoil.LastKickTime += frameTime;
					float restoreRate = MathHelper.Lerp(0.1f, 30f, Math.Min(1f, recoil.LastKickTime / 4f));
					Vector2 restore = normalized * restoreRate * frameTime;
					float num;
					float num2;
					(recoil.CurrentKick - restore).Deconstruct(ref num, ref num2);
					float x = num;
					float y = num2;
					if (Math.Sign(x) != Math.Sign(recoil.CurrentKick.X))
					{
						x = 0f;
					}
					if (Math.Sign(y) != Math.Sign(recoil.CurrentKick.Y))
					{
						y = 0f;
					}
					recoil.CurrentKick = new ValueTuple<float, float>(x, y);
					eye.Offset = recoil.BaseOffset + recoil.CurrentKick;
				}
			}
		}

		// Token: 0x0400132F RID: 4911
		private const float RestoreRateMax = 30f;

		// Token: 0x04001330 RID: 4912
		private const float RestoreRateMin = 0.1f;

		// Token: 0x04001331 RID: 4913
		private const float RestoreRateRamp = 4f;

		// Token: 0x04001332 RID: 4914
		protected const float KickMagnitudeMax = 1f;

		// Token: 0x04001333 RID: 4915
		[Nullable(1)]
		private ISawmill _log;
	}
}
