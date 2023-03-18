using System;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Explosion
{
	// Token: 0x02000321 RID: 801
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class ClusterGrenadeVisualizerSystem : VisualizerSystem<ClusterGrenadeVisualsComponent>
	{
		// Token: 0x06001434 RID: 5172 RVA: 0x000768C4 File Offset: 0x00074AC4
		protected override void OnAppearanceChange(EntityUid uid, ClusterGrenadeVisualsComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int value;
			if (this.AppearanceSystem.TryGetData<int>(uid, ClusterGrenadeVisuals.GrenadesCounter, ref value, args.Component))
			{
				SpriteComponent sprite = args.Sprite;
				int num = 0;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(comp.State);
				defaultInterpolatedStringHandler.AppendLiteral("-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(value);
				sprite.LayerSetState(num, defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}
}
