using System;
using System.Runtime.CompilerServices;
using Content.Client.Tools.Components;
using Content.Shared.Tools.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Tools.Visualizers
{
	// Token: 0x020000EF RID: 239
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class WelderVisualizerSystem : VisualizerSystem<WelderComponent>
	{
		// Token: 0x060006D3 RID: 1747 RVA: 0x00023E50 File Offset: 0x00022050
		protected override void OnAppearanceChange(EntityUid uid, WelderComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			if (this.AppearanceSystem.TryGetData<bool>(uid, WelderVisuals.Lit, ref flag, args.Component))
			{
				args.Sprite.LayerSetVisible(WelderLayers.Flame, flag);
			}
		}
	}
}
