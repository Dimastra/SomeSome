using System;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Storage.Visualizers
{
	// Token: 0x02000126 RID: 294
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class StorageFillVisualizerSystem : VisualizerSystem<StorageFillVisualizerComponent>
	{
		// Token: 0x0600080D RID: 2061 RVA: 0x0002ED44 File Offset: 0x0002CF44
		protected override void OnAppearanceChange(EntityUid uid, StorageFillVisualizerComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			int value;
			if (!this.AppearanceSystem.TryGetData<int>(uid, StorageFillVisuals.FillLevel, ref value, args.Component))
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(component.FillBaseName);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted<int>(value);
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			args.Sprite.LayerSetState(StorageFillLayers.Fill, text);
		}
	}
}
