using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Content.Server.Chat.Systems;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.White.TTS;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.White.TTS
{
	// Token: 0x02000085 RID: 133
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TTSSystem : EntitySystem
	{
		// Token: 0x060001F1 RID: 497 RVA: 0x0000AE04 File Offset: 0x00009004
		public override void Initialize()
		{
			this._cfg.OnValueChanged<bool>(CCVars.TTSEnabled, delegate(bool v)
			{
				this._isEnabled = v;
			}, true);
			this._cfg.OnValueChanged<string>(CCVars.TTSApiUrl, delegate(string url)
			{
				this._apiUrl = url;
			}, true);
			base.SubscribeLocalEvent<TransformSpeechEvent>(new EntityEventHandler<TransformSpeechEvent>(this.OnTransformSpeech), null, null);
			base.SubscribeLocalEvent<TTSComponent, EntitySpokeEvent>(new ComponentEventHandler<TTSComponent, EntitySpokeEvent>(this.OnEntitySpoke), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup), null, null);
			base.SubscribeNetworkEvent<RequestTTSEvent>(new EntityEventHandler<RequestTTSEvent>(this.OnRequestTTS), null, null);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000AE9B File Offset: 0x0000909B
		private void OnRequestTTS(RequestTTSEvent ev)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000AEA4 File Offset: 0x000090A4
		private void OnEntitySpoke(EntityUid uid, TTSComponent component, EntitySpokeEvent args)
		{
			TTSSystem.<OnEntitySpoke>d__10 <OnEntitySpoke>d__;
			<OnEntitySpoke>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnEntitySpoke>d__.<>4__this = this;
			<OnEntitySpoke>d__.uid = uid;
			<OnEntitySpoke>d__.component = component;
			<OnEntitySpoke>d__.args = args;
			<OnEntitySpoke>d__.<>1__state = -1;
			<OnEntitySpoke>d__.<>t__builder.Start<TTSSystem.<OnEntitySpoke>d__10>(ref <OnEntitySpoke>d__);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000AEF3 File Offset: 0x000090F3
		private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
		{
			this._ttsManager.ResetCache();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000AF00 File Offset: 0x00009100
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		private Task<byte[]> GenerateTTS(EntityUid uid, string text, string speaker)
		{
			TTSSystem.<GenerateTTS>d__12 <GenerateTTS>d__;
			<GenerateTTS>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<GenerateTTS>d__.<>4__this = this;
			<GenerateTTS>d__.uid = uid;
			<GenerateTTS>d__.text = text;
			<GenerateTTS>d__.speaker = speaker;
			<GenerateTTS>d__.<>1__state = -1;
			<GenerateTTS>d__.<>t__builder.Start<TTSSystem.<GenerateTTS>d__12>(ref <GenerateTTS>d__);
			return <GenerateTTS>d__.<>t__builder.Task;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000AF5B File Offset: 0x0000915B
		private void OnTransformSpeech(TransformSpeechEvent args)
		{
			if (!this._isEnabled)
			{
				return;
			}
			args.Message = args.Message.Replace("+", "");
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000AF84 File Offset: 0x00009184
		private string Sanitize(string text)
		{
			text = text.Trim();
			text = Regex.Replace(text, "Ё", "Е");
			text = Regex.Replace(text, "[^a-zA-Zа-яА-ЯёЁ0-9,\\-,+, ,?,!]", "");
			text = Regex.Replace(text, "[a-zA-Z]", new MatchEvaluator(this.ReplaceLat2Cyr), RegexOptions.IgnoreCase | RegexOptions.Multiline);
			text = Regex.Replace(text, "(?<![a-zA-Zа-яёА-ЯЁ])[a-zA-Zа-яёА-ЯЁ]+?(?![a-zA-Zа-яёА-ЯЁ])", new MatchEvaluator(this.ReplaceMatchedWord), RegexOptions.IgnoreCase | RegexOptions.Multiline);
			text = Regex.Replace(text, "(?<=[1-90])(\\.|,)(?=[1-90])", " целых ");
			text = Regex.Replace(text, "\\d+", new MatchEvaluator(this.ReplaceWord2Num));
			text = text.Trim();
			return text;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000B028 File Offset: 0x00009228
		private string ReplaceLat2Cyr(Match oneChar)
		{
			string replace;
			if (TTSSystem.ReverseTranslit.TryGetValue(oneChar.Value.ToLower(), out replace))
			{
				return replace;
			}
			return oneChar.Value;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B058 File Offset: 0x00009258
		private string ReplaceMatchedWord(Match word)
		{
			string replace;
			if (TTSSystem.WordReplacement.TryGetValue(word.Value.ToLower(), out replace))
			{
				return replace;
			}
			return word.Value;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B088 File Offset: 0x00009288
		private string ReplaceWord2Num(Match word)
		{
			long number;
			if (!long.TryParse(word.Value, out number))
			{
				return word.Value;
			}
			return NumberConverter.NumberToText(number, true);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B0B4 File Offset: 0x000092B4
		private string ToSsmlText(string text, TTSSystem.SpeechRate rate = TTSSystem.SpeechRate.Medium)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler.AppendLiteral("<speak><prosody rate=\"");
			defaultInterpolatedStringHandler.AppendFormatted(TTSSystem.SpeechRateMap[rate]);
			defaultInterpolatedStringHandler.AppendLiteral("\">");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral("</prosody></speak>");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x0400016C RID: 364
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400016D RID: 365
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400016E RID: 366
		[Dependency]
		private readonly TTSManager _ttsManager;

		// Token: 0x0400016F RID: 367
		[Dependency]
		private readonly SharedTransformSystem _xforms;

		// Token: 0x04000170 RID: 368
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000171 RID: 369
		private const int MaxMessageChars = 200;

		// Token: 0x04000172 RID: 370
		private bool _isEnabled;

		// Token: 0x04000173 RID: 371
		private string _apiUrl = string.Empty;

		// Token: 0x04000174 RID: 372
		private static readonly IReadOnlyDictionary<string, string> WordReplacement = new Dictionary<string, string>
		{
			{
				"нт",
				"Эн Тэ"
			},
			{
				"смо",
				"Эс Мэ О"
			},
			{
				"гп",
				"Гэ Пэ"
			},
			{
				"рд",
				"Эр Дэ"
			},
			{
				"гсб",
				"Гэ Эс Бэ"
			},
			{
				"гв",
				"Гэ Вэ"
			},
			{
				"нр",
				"Эн Эр"
			},
			{
				"срп",
				"Эс Эр Пэ"
			},
			{
				"цк",
				"Цэ Каа"
			},
			{
				"рнд",
				"Эр Эн Дэ"
			},
			{
				"сб",
				"Эс Бэ"
			},
			{
				"рцд",
				"Эр Цэ Дэ"
			},
			{
				"брпд",
				"Бэ Эр Пэ Дэ"
			},
			{
				"рпд",
				"Эр Пэ Дэ"
			},
			{
				"рпед",
				"Эр Пед"
			},
			{
				"тсф",
				"Тэ Эс Эф"
			},
			{
				"срт",
				"Эс Эр Тэ"
			},
			{
				"обр",
				"О Бэ Эр"
			},
			{
				"кпк",
				"Кэ Пэ Каа"
			},
			{
				"пда",
				"Пэ Дэ А"
			},
			{
				"id",
				"Ай Ди"
			},
			{
				"мщ",
				"Эм Ще"
			},
			{
				"вт",
				"Вэ Тэ"
			},
			{
				"ерп",
				"Йе Эр Пэ"
			},
			{
				"се",
				"Эс Йе"
			},
			{
				"апц",
				"А Пэ Цэ"
			},
			{
				"лкп",
				"Эл Ка Пэ"
			},
			{
				"см",
				"Эс Эм"
			},
			{
				"ека",
				"Йе Ка"
			},
			{
				"ка",
				"Кэ А"
			},
			{
				"бса",
				"Бэ Эс Аа"
			},
			{
				"тк",
				"Тэ Ка"
			},
			{
				"бфл",
				"Бэ Эф Эл"
			},
			{
				"бщ",
				"Бэ Щэ"
			},
			{
				"кк",
				"Кэ Ка"
			},
			{
				"ск",
				"Эс Ка"
			},
			{
				"зк",
				"Зэ Ка"
			},
			{
				"ерт",
				"Йе Эр Тэ"
			},
			{
				"вкд",
				"Вэ Ка Дэ"
			},
			{
				"нтр",
				"Эн Тэ Эр"
			},
			{
				"пнт",
				"Пэ Эн Тэ"
			},
			{
				"авд",
				"А Вэ Дэ"
			},
			{
				"пнв",
				"Пэ Эн Вэ"
			},
			{
				"ссд",
				"Эс Эс Дэ"
			},
			{
				"кпб",
				"Кэ Пэ Бэ"
			},
			{
				"сссп",
				"Эс Эс Эс Пэ"
			},
			{
				"крб",
				"Ка Эр Бэ"
			},
			{
				"бд",
				"Бэ Дэ"
			},
			{
				"сст",
				"Эс Эс Тэ"
			},
			{
				"скс",
				"Эс Ка Эс"
			},
			{
				"икн",
				"И Ка Эн"
			},
			{
				"нсс",
				"Эн Эс Эс"
			},
			{
				"емп",
				"Йе Эм Пэ"
			},
			{
				"бс",
				"Бэ Эс"
			},
			{
				"цкс",
				"Цэ Ка Эс"
			},
			{
				"срд",
				"Эс Эр Дэ"
			},
			{
				"жпс",
				"Джи Пи Эс"
			},
			{
				"gps",
				"Джи Пи Эс"
			},
			{
				"ннксс",
				"Эн Эн Ка Эс Эс"
			},
			{
				"ss",
				"Эс Эс"
			},
			{
				"сс",
				"Эс Эс"
			},
			{
				"тесла",
				"тэсла"
			},
			{
				"трейзен",
				"трэйзэн"
			},
			{
				"нанотрейзен",
				"нанотрэйзэн"
			},
			{
				"рпзд",
				"Эр Пэ Зэ Дэ"
			},
			{
				"кз",
				"Кэ Зэ"
			}
		};

		// Token: 0x04000175 RID: 373
		private static readonly IReadOnlyDictionary<string, string> ReverseTranslit = new Dictionary<string, string>
		{
			{
				"a",
				"а"
			},
			{
				"b",
				"б"
			},
			{
				"v",
				"в"
			},
			{
				"g",
				"г"
			},
			{
				"d",
				"д"
			},
			{
				"e",
				"е"
			},
			{
				"je",
				"ё"
			},
			{
				"zh",
				"ж"
			},
			{
				"z",
				"з"
			},
			{
				"i",
				"и"
			},
			{
				"y",
				"й"
			},
			{
				"k",
				"к"
			},
			{
				"l",
				"л"
			},
			{
				"m",
				"м"
			},
			{
				"n",
				"н"
			},
			{
				"o",
				"о"
			},
			{
				"p",
				"п"
			},
			{
				"r",
				"р"
			},
			{
				"s",
				"с"
			},
			{
				"t",
				"т"
			},
			{
				"u",
				"у"
			},
			{
				"f",
				"ф"
			},
			{
				"h",
				"х"
			},
			{
				"c",
				"ц"
			},
			{
				"x",
				"кс"
			},
			{
				"ch",
				"ч"
			},
			{
				"sh",
				"ш"
			},
			{
				"jsh",
				"щ"
			},
			{
				"hh",
				"ъ"
			},
			{
				"ih",
				"ы"
			},
			{
				"jh",
				"ь"
			},
			{
				"eh",
				"э"
			},
			{
				"ju",
				"ю"
			},
			{
				"ja",
				"я"
			}
		};

		// Token: 0x04000176 RID: 374
		private static readonly IReadOnlyDictionary<TTSSystem.SpeechRate, string> SpeechRateMap = new Dictionary<TTSSystem.SpeechRate, string>
		{
			{
				TTSSystem.SpeechRate.VerySlow,
				"x-slow"
			},
			{
				TTSSystem.SpeechRate.Slow,
				"slow"
			},
			{
				TTSSystem.SpeechRate.Medium,
				"medium"
			},
			{
				TTSSystem.SpeechRate.Fast,
				"fast"
			},
			{
				TTSSystem.SpeechRate.VeryFast,
				"x-fast"
			}
		};

		// Token: 0x020008A4 RID: 2212
		[NullableContext(0)]
		private enum SpeechRate : byte
		{
			// Token: 0x04001D0F RID: 7439
			VerySlow,
			// Token: 0x04001D10 RID: 7440
			Slow,
			// Token: 0x04001D11 RID: 7441
			Medium,
			// Token: 0x04001D12 RID: 7442
			Fast,
			// Token: 0x04001D13 RID: 7443
			VeryFast
		}
	}
}
