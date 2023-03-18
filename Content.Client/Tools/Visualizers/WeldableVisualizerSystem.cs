using System;
using System.Runtime.CompilerServices;
using Content.Client.Tools.Components;
using Content.Shared.Tools.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Tools.Visualizers
{
	// Token: 0x020000EE RID: 238
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class WeldableVisualizerSystem : VisualizerSystem<WeldableComponent>
	{
		// Token: 0x060006D1 RID: 1745 RVA: 0x00023DF4 File Offset: 0x00021FF4
		protected override void OnAppearanceChange(EntityUid uid, WeldableComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			this.AppearanceSystem.TryGetData<bool>(uid, WeldableVisuals.IsWelded, ref flag, args.Component);
			int num;
			if (args.Sprite.LayerMapTryGet(WeldableLayers.BaseWelded, ref num, false))
			{
				args.Sprite.LayerSetVisible(num, flag);
			}
		}
	}
}
