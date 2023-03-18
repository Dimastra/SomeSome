using System;
using System.Runtime.CompilerServices;
using Content.Shared.Paper;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Paper
{
	// Token: 0x020001EC RID: 492
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PaperSystem : VisualizerSystem<PaperVisualsComponent>
	{
		// Token: 0x06000C98 RID: 3224 RVA: 0x000495AC File Offset: 0x000477AC
		protected override void OnAppearanceChange(EntityUid uid, PaperVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			SharedPaperComponent.PaperStatus paperStatus;
			if (this.AppearanceSystem.TryGetData<SharedPaperComponent.PaperStatus>(uid, SharedPaperComponent.PaperVisuals.Status, ref paperStatus, args.Component))
			{
				args.Sprite.LayerSetVisible(PaperVisualLayers.Writing, paperStatus == SharedPaperComponent.PaperStatus.Written);
			}
			string text;
			if (this.AppearanceSystem.TryGetData<string>(uid, SharedPaperComponent.PaperVisuals.Stamp, ref text, args.Component))
			{
				args.Sprite.LayerSetState(PaperVisualLayers.Stamp, text);
				args.Sprite.LayerSetVisible(PaperVisualLayers.Stamp, true);
			}
		}
	}
}
