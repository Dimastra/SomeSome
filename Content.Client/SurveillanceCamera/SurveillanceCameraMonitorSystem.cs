using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Client.SurveillanceCamera
{
	// Token: 0x02000107 RID: 263
	public sealed class SurveillanceCameraMonitorSystem : EntitySystem
	{
		// Token: 0x0600074B RID: 1867 RVA: 0x000263D0 File Offset: 0x000245D0
		public override void Update(float frameTime)
		{
			foreach (ActiveSurveillanceCameraMonitorVisualsComponent activeSurveillanceCameraMonitorVisualsComponent in base.EntityQuery<ActiveSurveillanceCameraMonitorVisualsComponent>(false))
			{
				if (!base.Paused(activeSurveillanceCameraMonitorVisualsComponent.Owner, null))
				{
					activeSurveillanceCameraMonitorVisualsComponent.TimeLeft -= frameTime;
					if (activeSurveillanceCameraMonitorVisualsComponent.TimeLeft <= 0f || base.Deleted(activeSurveillanceCameraMonitorVisualsComponent.Owner, null))
					{
						if (activeSurveillanceCameraMonitorVisualsComponent.OnFinish != null)
						{
							activeSurveillanceCameraMonitorVisualsComponent.OnFinish();
						}
						this.EntityManager.RemoveComponentDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(activeSurveillanceCameraMonitorVisualsComponent.Owner);
					}
				}
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00026478 File Offset: 0x00024678
		[NullableContext(1)]
		public void AddTimer(EntityUid uid, Action onFinish)
		{
			base.EnsureComp<ActiveSurveillanceCameraMonitorVisualsComponent>(uid).OnFinish = onFinish;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00026487 File Offset: 0x00024687
		public void RemoveTimer(EntityUid uid)
		{
			this.EntityManager.RemoveComponentDeferred<ActiveSurveillanceCameraMonitorVisualsComponent>(uid);
		}
	}
}
