using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat.TypingIndicator;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

namespace Content.Client.Chat.TypingIndicator
{
	// Token: 0x020003E8 RID: 1000
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class TypingIndicatorVisualizerSystem : VisualizerSystem<TypingIndicatorComponent>
	{
		// Token: 0x06001898 RID: 6296 RVA: 0x0008DCE0 File Offset: 0x0008BEE0
		protected override void OnAppearanceChange(EntityUid uid, TypingIndicatorComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			TypingIndicatorPrototype typingIndicatorPrototype;
			if (!this._prototypeManager.TryIndex<TypingIndicatorPrototype>(component.Prototype, ref typingIndicatorPrototype))
			{
				Logger.Error("Unknown typing indicator id: " + component.Prototype);
				return;
			}
			int num;
			if (!args.Sprite.LayerMapTryGet(TypingIndicatorLayers.Base, ref num, false))
			{
				num = args.Sprite.LayerMapReserveBlank(TypingIndicatorLayers.Base);
			}
			args.Sprite.LayerSetRSI(num, typingIndicatorPrototype.SpritePath);
			args.Sprite.LayerSetState(num, typingIndicatorPrototype.TypingState);
			args.Sprite.LayerSetShader(num, typingIndicatorPrototype.Shader);
			args.Sprite.LayerSetOffset(num, typingIndicatorPrototype.Offset);
			TypingIndicatorState typingIndicatorState;
			args.Component.TryGetData<TypingIndicatorState>(TypingIndicatorVisuals.State, ref typingIndicatorState);
			args.Sprite.LayerSetVisible(num, typingIndicatorState > TypingIndicatorState.None);
			if (typingIndicatorState == TypingIndicatorState.Idle)
			{
				args.Sprite.LayerSetState(num, typingIndicatorPrototype.IdleState);
				return;
			}
			if (typingIndicatorState != TypingIndicatorState.Typing)
			{
				return;
			}
			args.Sprite.LayerSetState(num, typingIndicatorPrototype.TypingState);
		}

		// Token: 0x04000C8F RID: 3215
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;
	}
}
