using System;
using System.Runtime.CompilerServices;
using Content.Shared.Vehicle;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Vehicle
{
	// Token: 0x02000069 RID: 105
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class VehicleVisualsSystem : VisualizerSystem<VehicleVisualsComponent>
	{
		// Token: 0x060001F3 RID: 499 RVA: 0x0000DC28 File Offset: 0x0000BE28
		protected override void OnAppearanceChange(EntityUid uid, VehicleVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int drawDepth;
			if (this.AppearanceSystem.TryGetData<int>(uid, VehicleVisuals.DrawDepth, ref drawDepth, args.Component))
			{
				args.Sprite.DrawDepth = drawDepth;
			}
			bool flag;
			if (this.AppearanceSystem.TryGetData<bool>(uid, VehicleVisuals.AutoAnimate, ref flag, args.Component))
			{
				args.Sprite.LayerSetAutoAnimated(VehicleVisualLayers.AutoAnimate, flag);
			}
		}
	}
}
