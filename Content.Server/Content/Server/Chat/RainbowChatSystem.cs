using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Systems;
using Content.Shared.Humanoid;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Chat
{
	// Token: 0x020006BB RID: 1723
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RainbowChatSystem : EntitySystem
	{
		// Token: 0x060023D1 RID: 9169 RVA: 0x000BA751 File Offset: 0x000B8951
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HumanoidAppearanceComponent, SetSpeakerColorEvent>(new ComponentEventHandler<HumanoidAppearanceComponent, SetSpeakerColorEvent>(this.OnHumanoidSpeak), null, null);
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000BA770 File Offset: 0x000B8970
		private void OnHumanoidSpeak(EntityUid uid, HumanoidAppearanceComponent component, SetSpeakerColorEvent args)
		{
			string color = this.GetColor(args.Name);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[color=");
			defaultInterpolatedStringHandler.AppendFormatted(color);
			defaultInterpolatedStringHandler.AppendLiteral("]");
			defaultInterpolatedStringHandler.AppendFormatted(args.Name);
			defaultInterpolatedStringHandler.AppendLiteral("[/color]");
			args.Name = defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x000BA7DC File Offset: 0x000B89DC
		private string GetColor(string characterName)
		{
			string hexColor;
			if (this.NameToColor.TryGetValue(characterName, out hexColor))
			{
				return hexColor;
			}
			return this.CreateCharacterHexColor(characterName);
		}

		// Token: 0x060023D4 RID: 9172 RVA: 0x000BA804 File Offset: 0x000B8A04
		private string CreateCharacterHexColor(string characterName)
		{
			Random random = new Random(characterName.GetHashCode());
			float randomFloat = this.GetRandomFloat(ref random, 0f, 1f);
			float s = this.GetRandomFloat(ref random, RainbowChatSystem.MinSaturation, RainbowChatSystem.MaxSaturation);
			float v = this.GetRandomFloat(ref random, RainbowChatSystem.MinValue, RainbowChatSystem.MaxValue);
			string color = Color.FromHsv(new Vector4(randomFloat, s, v, 1f)).ToHex();
			this.NameToColor[characterName] = color;
			return color;
		}

		// Token: 0x060023D5 RID: 9173 RVA: 0x000BA87E File Offset: 0x000B8A7E
		private float GetRandomFloat(ref Random random, float minimum, float maximum)
		{
			return (float)(random.NextDouble() * (double)(maximum - minimum) + (double)minimum);
		}

		// Token: 0x04001627 RID: 5671
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04001628 RID: 5672
		public Dictionary<string, string> NameToColor = new Dictionary<string, string>();

		// Token: 0x04001629 RID: 5673
		private static float MinSaturation = 0.22f;

		// Token: 0x0400162A RID: 5674
		private static float MaxSaturation = 0.3f;

		// Token: 0x0400162B RID: 5675
		private static float MinValue = 0.7f;

		// Token: 0x0400162C RID: 5676
		private static float MaxValue = 0.8f;
	}
}
