using System;
using System.Runtime.CompilerServices;
using Content.Shared.Xenoarchaeology.XenoArtifacts;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class RandomArtifactSpriteSystem : VisualizerSystem<RandomArtifactSpriteComponent>
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000020B8 File Offset: 0x000002B8
		protected override void OnAppearanceChange(EntityUid uid, RandomArtifactSpriteComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int num;
			if (!this.AppearanceSystem.TryGetData<int>(uid, SharedArtifactsVisuals.SpriteIndex, ref num, args.Component))
			{
				return;
			}
			bool flag;
			if (!this.AppearanceSystem.TryGetData<bool>(uid, SharedArtifactsVisuals.IsActivated, ref flag, args.Component))
			{
				flag = false;
			}
			string str = num.ToString("D2");
			string str2 = flag ? "_on" : "";
			int num2;
			if (args.Sprite.LayerMapTryGet(ArtifactsVisualLayers.Effect, ref num2, false))
			{
				string text = "ano" + str;
				args.Sprite.LayerSetState(ArtifactsVisualLayers.Base, text);
				args.Sprite.LayerSetState(num2, text + "_on");
				args.Sprite.LayerSetVisible(num2, flag);
				return;
			}
			string text2 = "ano" + str + str2;
			args.Sprite.LayerSetState(ArtifactsVisualLayers.Base, text2);
		}
	}
}
