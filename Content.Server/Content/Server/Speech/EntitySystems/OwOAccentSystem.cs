using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B8 RID: 440
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class OwOAccentSystem : EntitySystem
	{
		// Token: 0x0600089A RID: 2202 RVA: 0x0002C207 File Offset: 0x0002A407
		public override void Initialize()
		{
			base.SubscribeLocalEvent<OwOAccentComponent, AccentGetEvent>(new ComponentEventHandler<OwOAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x0002C220 File Offset: 0x0002A420
		public string Accentuate(string message)
		{
			foreach (KeyValuePair<string, string> keyValuePair in OwOAccentSystem.SpecialWords)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string word = text;
				string repl = text2;
				message = message.Replace(word, repl);
			}
			return message.Replace("!", RandomExtensions.Pick<string>(this._random, OwOAccentSystem.Faces)).Replace("r", "w").Replace("R", "W").Replace("l", "w").Replace("L", "W").Replace("р", "в").Replace("Р", "В").Replace("л", "в").Replace("Л", "В");
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0002C314 File Offset: 0x0002A514
		private void OnAccent(EntityUid uid, OwOAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}

		// Token: 0x0400053B RID: 1339
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400053C RID: 1340
		private static readonly IReadOnlyList<string> Faces = new List<string>
		{
			" (・`ω´・)",
			" ;;w;;",
			" owo",
			" UwU",
			" >w<",
			" ^w^"
		}.AsReadOnly();

		// Token: 0x0400053D RID: 1341
		private static readonly IReadOnlyDictionary<string, string> SpecialWords = new Dictionary<string, string>
		{
			{
				"you",
				"wu"
			},
			{
				"ты",
				"ти"
			}
		};
	}
}
