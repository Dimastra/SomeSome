using System;
using System.Runtime.CompilerServices;
using Content.Shared.Bed;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Bed
{
	// Token: 0x02000421 RID: 1057
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class StasisBedSystem : VisualizerSystem<StasisBedVisualsComponent>
	{
		// Token: 0x060019DE RID: 6622 RVA: 0x0009449C File Offset: 0x0009269C
		protected override void OnAppearanceChange(EntityUid uid, StasisBedVisualsComponent component, ref AppearanceChangeEvent args)
		{
			bool flag;
			if (args.Sprite != null && this.AppearanceSystem.TryGetData<bool>(uid, StasisBedVisuals.IsOn, ref flag, args.Component))
			{
				args.Sprite.LayerSetVisible(StasisBedVisualLayers.IsOn, flag);
			}
		}
	}
}
