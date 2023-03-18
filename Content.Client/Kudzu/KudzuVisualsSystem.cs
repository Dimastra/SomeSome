using System;
using System.Runtime.CompilerServices;
using Content.Shared.Kudzu;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Kudzu
{
	// Token: 0x0200028E RID: 654
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class KudzuVisualsSystem : VisualizerSystem<KudzuVisualsComponent>
	{
		// Token: 0x060010A2 RID: 4258 RVA: 0x00063808 File Offset: 0x00061A08
		protected override void OnAppearanceChange(EntityUid uid, KudzuVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int value;
			int value2;
			if (this.AppearanceSystem.TryGetData<int>(uid, KudzuVisuals.Variant, ref value, args.Component) && this.AppearanceSystem.TryGetData<int>(uid, KudzuVisuals.GrowthLevel, ref value2, args.Component))
			{
				int num = args.Sprite.LayerMapReserveBlank(component.Layer);
				SpriteComponent sprite = args.Sprite;
				int num2 = num;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
				defaultInterpolatedStringHandler.AppendLiteral("kudzu_");
				defaultInterpolatedStringHandler.AppendFormatted<int>(value2);
				defaultInterpolatedStringHandler.AppendFormatted<int>(value);
				sprite.LayerSetState(num2, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}
}
