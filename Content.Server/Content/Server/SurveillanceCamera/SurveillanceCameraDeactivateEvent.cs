using System;
using Robust.Shared.GameObjects;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x02000149 RID: 329
	public sealed class SurveillanceCameraDeactivateEvent : EntityEventArgs
	{
		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0001E377 File Offset: 0x0001C577
		public EntityUid Camera { get; }

		// Token: 0x06000641 RID: 1601 RVA: 0x0001E37F File Offset: 0x0001C57F
		public SurveillanceCameraDeactivateEvent(EntityUid camera)
		{
			this.Camera = camera;
		}
	}
}
