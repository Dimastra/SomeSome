using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B6 RID: 438
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MobsterAccentSystem : EntitySystem
	{
		// Token: 0x06000891 RID: 2193 RVA: 0x0002BBAC File Offset: 0x00029DAC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MobsterAccentComponent, AccentGetEvent>(new ComponentEventHandler<MobsterAccentComponent, AccentGetEvent>(this.OnAccentGet), null, null);
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0002BBC8 File Offset: 0x00029DC8
		public string Accentuate(string message, MobsterAccentComponent component)
		{
			string msg = message;
			foreach (KeyValuePair<string, string> keyValuePair in MobsterAccentSystem.DirectReplacements)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string first = text;
				string replace = text2;
				msg = Regex.Replace(msg, "(?<!\\w)" + first + "(?!\\w)", replace, RegexOptions.IgnoreCase);
			}
			msg = Regex.Replace(msg, "(?<=\\w\\w)ing(?!\\w)", "in'", RegexOptions.IgnoreCase);
			msg = Regex.Replace(msg, "(?<=\\w)or(?=\\w)", "uh", RegexOptions.IgnoreCase);
			msg = Regex.Replace(msg, "(?<=\\w)ar(?=\\w)", "ah", RegexOptions.IgnoreCase);
			if (RandomExtensions.Prob(this._random, 0.15f))
			{
				int pick = this._random.Next(1, 2);
				msg = msg[0].ToString().ToLower() + msg.Remove(0, 1);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
				defaultInterpolatedStringHandler.AppendLiteral("accent-mobster-prefix-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(pick);
				msg = Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear()) + " " + msg;
			}
			msg = msg[0].ToString().ToUpper() + msg.Remove(0, 1);
			if (RandomExtensions.Prob(this._random, 0.4f))
			{
				if (component.IsBoss)
				{
					int pick2 = this._random.Next(1, 4);
					string str = msg;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler.AppendLiteral("accent-mobster-suffix-boss-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(pick2);
					msg = str + Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					int pick3 = this._random.Next(1, 3);
					string str2 = msg;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler.AppendLiteral("accent-mobster-suffix-minion-");
					defaultInterpolatedStringHandler.AppendFormatted<int>(pick3);
					msg = str2 + Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			return msg;
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x0002BDB8 File Offset: 0x00029FB8
		private void OnAccentGet(EntityUid uid, MobsterAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message, component);
		}

		// Token: 0x04000538 RID: 1336
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000539 RID: 1337
		private static readonly Dictionary<string, string> DirectReplacements = new Dictionary<string, string>
		{
			{
				"let me",
				"lemme"
			},
			{
				"should",
				"oughta"
			},
			{
				"the",
				"da"
			},
			{
				"them",
				"dem"
			},
			{
				"attack",
				"whack"
			},
			{
				"kill",
				"whack"
			},
			{
				"murder",
				"whack"
			},
			{
				"dead",
				"sleepin' with da fishies"
			},
			{
				"hey",
				"ey'o"
			},
			{
				"hi",
				"ey'o"
			},
			{
				"hello",
				"ey'o"
			},
			{
				"rules",
				"roolz"
			},
			{
				"you",
				"yous"
			},
			{
				"have to",
				"gotta"
			},
			{
				"going to",
				"boutta"
			},
			{
				"about to",
				"boutta"
			},
			{
				"here",
				"'ere"
			},
			{
				"утащил",
				"сдёрнул"
			},
			{
				"принеси",
				"надыбай"
			},
			{
				"принесите",
				"надыбайте"
			},
			{
				"сб",
				"мусора"
			},
			{
				"враг",
				"шелупонь"
			},
			{
				"враги",
				"шелупонь"
			},
			{
				"тревога",
				"шухер"
			},
			{
				"заметили",
				"спалили"
			},
			{
				"оружие",
				"валына"
			},
			{
				"убийство",
				"мокруха"
			},
			{
				"убить",
				"замочить"
			},
			{
				"убей",
				"вальни"
			},
			{
				"убейте",
				"завалите"
			},
			{
				"еда",
				"жратва"
			},
			{
				"еды",
				"жратвы"
			},
			{
				"убили",
				"замаслили"
			},
			{
				"ранен",
				"словил маслину"
			},
			{
				"мертв",
				"спит с рыбами"
			},
			{
				"мёртв",
				"спит с рыбами"
			},
			{
				"мертва",
				"спит с рыбами"
			},
			{
				"хэй",
				"йоу"
			},
			{
				"хей",
				"йоу"
			},
			{
				"здесь",
				"здеся"
			},
			{
				"тут",
				"тута"
			},
			{
				"привет",
				"аве"
			},
			{
				"плохо",
				"ацтой"
			},
			{
				"хорошо",
				"агонь"
			}
		};
	}
}
