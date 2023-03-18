using System;
using System.Runtime.CompilerServices;
using Content.Shared.Smoking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Chemistry.Visualizers
{
	// Token: 0x020003C9 RID: 969
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class SmokeVisualizerSystem : VisualizerSystem<SmokeVisualsComponent>
	{
		// Token: 0x060017F0 RID: 6128 RVA: 0x000899D4 File Offset: 0x00087BD4
		protected override void OnAppearanceChange(EntityUid uid, SmokeVisualsComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			Color color;
			if (!this.AppearanceSystem.TryGetData<Color>(uid, SmokeVisuals.Color, ref color, null))
			{
				return;
			}
			args.Sprite.Color = color;
		}
	}
}
