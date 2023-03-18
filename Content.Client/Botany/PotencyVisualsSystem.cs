using System;
using System.Runtime.CompilerServices;
using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Botany
{
	// Token: 0x02000416 RID: 1046
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PotencyVisualsSystem : VisualizerSystem<PotencyVisualsComponent>
	{
		// Token: 0x060019B1 RID: 6577 RVA: 0x0009378C File Offset: 0x0009198C
		protected override void OnAppearanceChange(EntityUid uid, PotencyVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			float num;
			if (this.AppearanceSystem.TryGetData<float>(uid, ProduceVisuals.Potency, ref num, args.Component))
			{
				float num2 = MathHelper.Lerp(component.MinimumScale, component.MaximumScale, num / 100f);
				args.Sprite.Scale = new Vector2(num2, num2);
			}
		}
	}
}
