using System;
using System.Runtime.CompilerServices;
using Content.Shared.Light.Component;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Light.Visualizers
{
	// Token: 0x02000265 RID: 613
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class LightBulbSystem : VisualizerSystem<LightBulbComponent>
	{
		// Token: 0x06000FB1 RID: 4017 RVA: 0x0005EA1C File Offset: 0x0005CC1C
		protected override void OnAppearanceChange(EntityUid uid, LightBulbComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			LightBulbState lightBulbState;
			if (this.AppearanceSystem.TryGetData<LightBulbState>(uid, LightBulbVisuals.State, ref lightBulbState, args.Component))
			{
				switch (lightBulbState)
				{
				case LightBulbState.Normal:
					args.Sprite.LayerSetState(LightBulbVisualLayers.Base, comp.NormalSpriteState);
					break;
				case LightBulbState.Broken:
					args.Sprite.LayerSetState(LightBulbVisualLayers.Base, comp.BrokenSpriteState);
					break;
				case LightBulbState.Burned:
					args.Sprite.LayerSetState(LightBulbVisualLayers.Base, comp.BurnedSpriteState);
					break;
				}
			}
			Color color;
			if (this.AppearanceSystem.TryGetData<Color>(uid, LightBulbVisuals.Color, ref color, args.Component))
			{
				args.Sprite.Color = color;
			}
		}
	}
}
