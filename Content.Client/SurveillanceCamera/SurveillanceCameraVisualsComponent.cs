using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.SurveillanceCamera;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.SurveillanceCamera
{
	// Token: 0x02000108 RID: 264
	[RegisterComponent]
	public sealed class SurveillanceCameraVisualsComponent : Component
	{
		// Token: 0x04000366 RID: 870
		[Nullable(1)]
		[DataField("sprites", false, 1, false, false, null)]
		public readonly Dictionary<SurveillanceCameraVisuals, string> CameraSprites = new Dictionary<SurveillanceCameraVisuals, string>();
	}
}
