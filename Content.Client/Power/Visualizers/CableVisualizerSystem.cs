using System;
using System.Runtime.CompilerServices;
using Content.Client.SubFloor;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Power.Visualizers
{
	// Token: 0x020001A2 RID: 418
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class CableVisualizerSystem : VisualizerSystem<CableVisualizerComponent>
	{
		// Token: 0x06000B00 RID: 2816 RVA: 0x00040199 File Offset: 0x0003E399
		public override void Initialize()
		{
			base.SubscribeLocalEvent<CableVisualizerComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CableVisualizerComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, new Type[]
			{
				typeof(SubFloorHideSystem)
			});
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x000401C4 File Offset: 0x0003E3C4
		protected override void OnAppearanceChange(EntityUid uid, CableVisualizerComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			if (!args.Sprite.Visible)
			{
				return;
			}
			WireVisDirFlags value;
			if (!this.AppearanceSystem.TryGetData<WireVisDirFlags>(uid, WireVisVisuals.ConnectedMask, ref value, args.Component))
			{
				value = WireVisDirFlags.None;
			}
			SpriteComponent sprite = args.Sprite;
			int num = 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(component.StatePrefix);
			defaultInterpolatedStringHandler.AppendFormatted<int>((int)value);
			sprite.LayerSetState(num, defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
