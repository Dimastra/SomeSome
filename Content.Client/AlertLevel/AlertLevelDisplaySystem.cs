using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.AlertLevel;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.AlertLevel
{
	// Token: 0x0200047D RID: 1149
	public sealed class AlertLevelDisplaySystem : EntitySystem
	{
		// Token: 0x06001C64 RID: 7268 RVA: 0x000A4BAC File Offset: 0x000A2DAC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AlertLevelDisplayComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<AlertLevelDisplayComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06001C65 RID: 7269 RVA: 0x000A4BC8 File Offset: 0x000A2DC8
		[NullableContext(1)]
		private void OnAppearanceChange(EntityUid uid, AlertLevelDisplayComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int num;
			if (!args.Sprite.LayerMapTryGet(AlertLevelDisplay.Layer, ref num, false))
			{
				int num2 = args.Sprite.AddLayer(new RSI.StateId(component.AlertVisuals.Values.First<string>()), null);
				args.Sprite.LayerMapSet(AlertLevelDisplay.Layer, num2);
			}
			object obj;
			if (!args.AppearanceData.TryGetValue(AlertLevelDisplay.CurrentLevel, out obj))
			{
				args.Sprite.LayerSetState(AlertLevelDisplay.Layer, new RSI.StateId(component.AlertVisuals.Values.First<string>()));
				return;
			}
			string text;
			if (component.AlertVisuals.TryGetValue((string)obj, out text))
			{
				args.Sprite.LayerSetState(AlertLevelDisplay.Layer, new RSI.StateId(text));
				return;
			}
			args.Sprite.LayerSetState(AlertLevelDisplay.Layer, new RSI.StateId(component.AlertVisuals.Values.First<string>()));
		}
	}
}
