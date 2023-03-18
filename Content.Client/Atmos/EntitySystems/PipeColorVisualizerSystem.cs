using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.Components;
using Content.Shared.Atmos.Piping;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.EntitySystems
{
	// Token: 0x0200045E RID: 1118
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PipeColorVisualizerSystem : VisualizerSystem<PipeColorVisualsComponent>
	{
		// Token: 0x06001BCF RID: 7119 RVA: 0x000A0E9C File Offset: 0x0009F09C
		protected override void OnAppearanceChange(EntityUid uid, PipeColorVisualsComponent component, ref AppearanceChangeEvent args)
		{
			SpriteComponent spriteComponent;
			Color color;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent) && this.AppearanceSystem.TryGetData<Color>(uid, PipeColorVisuals.Color, ref color, args.Component))
			{
				ISpriteLayer spriteLayer = spriteComponent[PipeVisualLayers.Pipe];
				spriteLayer.Color = color.WithAlpha(spriteLayer.Color.A);
			}
		}
	}
}
