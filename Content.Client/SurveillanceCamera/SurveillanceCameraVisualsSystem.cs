using System;
using System.Runtime.CompilerServices;
using Content.Shared.SurveillanceCamera;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.SurveillanceCamera
{
	// Token: 0x02000109 RID: 265
	public sealed class SurveillanceCameraVisualsSystem : EntitySystem
	{
		// Token: 0x06000750 RID: 1872 RVA: 0x000264A9 File Offset: 0x000246A9
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SurveillanceCameraVisualsComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000264C8 File Offset: 0x000246C8
		[NullableContext(1)]
		private void OnAppearanceChange(EntityUid uid, SurveillanceCameraVisualsComponent component, ref AppearanceChangeEvent args)
		{
			object obj;
			if (args.AppearanceData.TryGetValue(SurveillanceCameraVisualsKey.Key, out obj) && obj is SurveillanceCameraVisuals)
			{
				SurveillanceCameraVisuals key = (SurveillanceCameraVisuals)obj;
				int num;
				string text;
				if (args.Sprite != null && args.Sprite.LayerMapTryGet(SurveillanceCameraVisualsKey.Layer, ref num, false) && component.CameraSprites.TryGetValue(key, out text))
				{
					args.Sprite.LayerSetState(num, text);
					return;
				}
			}
		}
	}
}
