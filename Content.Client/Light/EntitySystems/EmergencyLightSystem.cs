using System;
using System.Runtime.CompilerServices;
using Content.Client.Light.Components;
using Content.Shared.Light.Component;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Light.EntitySystems
{
	// Token: 0x02000269 RID: 617
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class EmergencyLightSystem : VisualizerSystem<EmergencyLightComponent>
	{
		// Token: 0x06000FC0 RID: 4032 RVA: 0x0005EF8C File Offset: 0x0005D18C
		protected override void OnAppearanceChange(EntityUid uid, EmergencyLightComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			if (!this.AppearanceSystem.TryGetData<bool>(uid, EmergencyLightVisuals.On, ref flag, args.Component))
			{
				flag = false;
			}
			args.Sprite.LayerSetVisible(EmergencyLightVisualLayers.LightOff, !flag);
			args.Sprite.LayerSetVisible(EmergencyLightVisualLayers.LightOn, flag);
			Color color;
			if (this.AppearanceSystem.TryGetData<Color>(uid, EmergencyLightVisuals.Color, ref color, args.Component))
			{
				args.Sprite.LayerSetColor(EmergencyLightVisualLayers.LightOn, color);
				args.Sprite.LayerSetColor(EmergencyLightVisualLayers.LightOff, color);
			}
		}
	}
}
