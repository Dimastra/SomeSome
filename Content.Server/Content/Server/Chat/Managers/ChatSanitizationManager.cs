using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Server.Chat.Managers
{
	// Token: 0x020006CA RID: 1738
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChatSanitizationManager : IChatSanitizationManager
	{
		// Token: 0x06002440 RID: 9280 RVA: 0x000BD7DC File Offset: 0x000BB9DC
		public void Initialize()
		{
			this._configurationManager.OnValueChanged<bool>(CCVars.ChatSanitizerEnabled, delegate(bool x)
			{
				this._doSanitize = x;
			}, true);
			try
			{
				string filterData = this._resources.ContentFileReadAllText(new ResourcePath("/White/ChatFilters/slang.json", "/"));
				this._slangToNormal = JsonSerializer.Deserialize<Dictionary<string, string>>(filterData, null);
			}
			catch (Exception e)
			{
				Logger.ErrorS("chat", "Failed to load slang.json: {0}", new object[]
				{
					e
				});
			}
		}

		// Token: 0x06002441 RID: 9281 RVA: 0x000BD85C File Offset: 0x000BBA5C
		public bool TrySanitizeOutSmilies(string input, EntityUid speaker, out string sanitized, [Nullable(2)] [NotNullWhen(true)] out string emote)
		{
			if (!this._doSanitize)
			{
				sanitized = input;
				emote = null;
				return false;
			}
			input = input.TrimEnd();
			foreach (KeyValuePair<string, string> keyValuePair in ChatSanitizationManager.SmileyToEmote)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string smiley = text;
				string replacement = text2;
				if (input.EndsWith(smiley, true, CultureInfo.InvariantCulture))
				{
					sanitized = input.Remove(input.Length - smiley.Length).TrimEnd();
					emote = Loc.GetString(replacement, new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("ent", speaker)
					});
					return true;
				}
			}
			sanitized = input;
			emote = null;
			return false;
		}

		// Token: 0x06002442 RID: 9282 RVA: 0x000BD930 File Offset: 0x000BBB30
		public string SanitizeOutSlang(string input)
		{
			string pattern = "\\b(?<word>\\w+)\\b";
			return Regex.Replace(input, pattern, delegate(Match match)
			{
				if (!this._slangToNormal.ContainsKey(match.Groups[1].Value.ToLower()))
				{
					return match.Value;
				}
				return this._slangToNormal[match.Groups[1].Value.ToLower()];
			}, RegexOptions.IgnoreCase);
		}

		// Token: 0x0400167B RID: 5755
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400167C RID: 5756
		[Dependency]
		private readonly IResourceManager _resources;

		// Token: 0x0400167D RID: 5757
		private Dictionary<string, string> _slangToNormal = new Dictionary<string, string>();

		// Token: 0x0400167E RID: 5758
		private static readonly Dictionary<string, string> SmileyToEmote = new Dictionary<string, string>
		{
			{
				":)",
				"chatsan-smiles"
			},
			{
				":]",
				"chatsan-smiles"
			},
			{
				"=)",
				"chatsan-smiles"
			},
			{
				"=]",
				"chatsan-smiles"
			},
			{
				"(:",
				"chatsan-smiles"
			},
			{
				"[:",
				"chatsan-smiles"
			},
			{
				"(=",
				"chatsan-smiles"
			},
			{
				"[=",
				"chatsan-smiles"
			},
			{
				"^^",
				"chatsan-smiles"
			},
			{
				"^-^",
				"chatsan-smiles"
			},
			{
				":(",
				"chatsan-frowns"
			},
			{
				":[",
				"chatsan-frowns"
			},
			{
				"=(",
				"chatsan-frowns"
			},
			{
				"=[",
				"chatsan-frowns"
			},
			{
				"):",
				"chatsan-frowns"
			},
			{
				")=",
				"chatsan-frowns"
			},
			{
				"]:",
				"chatsan-frowns"
			},
			{
				"]=",
				"chatsan-frowns"
			},
			{
				":D",
				"chatsan-smiles-widely"
			},
			{
				"D:",
				"chatsan-frowns-deeply"
			},
			{
				":O",
				"chatsan-surprised"
			},
			{
				":3",
				"chatsan-smiles"
			},
			{
				":S",
				"chatsan-uncertain"
			},
			{
				":>",
				"chatsan-grins"
			},
			{
				":<",
				"chatsan-pouts"
			},
			{
				"xD",
				"chatsan-laughs"
			},
			{
				"хд",
				"chatsan-laughs"
			},
			{
				";-;",
				"chatsan-cries"
			},
			{
				";_;",
				"chatsan-cries"
			},
			{
				"qwq",
				"chatsan-cries"
			},
			{
				":u",
				"chatsan-smiles-smugly"
			},
			{
				":v",
				"chatsan-smiles-smugly"
			},
			{
				">:i",
				"chatsan-annoyed"
			},
			{
				":i",
				"chatsan-sighs"
			},
			{
				":|",
				"chatsan-sighs"
			},
			{
				":p",
				"chatsan-stick-out-tongue"
			},
			{
				":b",
				"chatsan-stick-out-tongue"
			},
			{
				"0-0",
				"chatsan-wide-eyed"
			},
			{
				"o-o",
				"chatsan-wide-eyed"
			},
			{
				"о-о",
				"chatsan-wide-eyed"
			},
			{
				"o.o",
				"chatsan-wide-eyed"
			},
			{
				"о.о",
				"chatsan-wide-eyed"
			},
			{
				"0_o",
				"chatsan-wide-eyed"
			},
			{
				"0_о",
				"chatsan-wide-eyed"
			},
			{
				"._.",
				"chatsan-surprised"
			},
			{
				".-.",
				"chatsan-confused"
			},
			{
				"-_-",
				"chatsan-unimpressed"
			},
			{
				"o/",
				"chatsan-waves"
			},
			{
				"о/",
				"chatsan-waves"
			},
			{
				"^^/",
				"chatsan-waves"
			},
			{
				":/",
				"chatsan-uncertain"
			},
			{
				":\\",
				"chatsan-uncertain"
			},
			{
				"lmao",
				"chatsan-laughs"
			},
			{
				"lmao.",
				"chatsan-laughs"
			},
			{
				"lol",
				"chatsan-laughs"
			},
			{
				"lol.",
				"chatsan-laughs"
			},
			{
				"лол",
				"chatsan-laughs"
			},
			{
				"lel",
				"chatsan-laughs"
			},
			{
				"lel.",
				"chatsan-laughs"
			},
			{
				"kek",
				"chatsan-laughs"
			},
			{
				"kek.",
				"chatsan-laughs"
			},
			{
				"o7",
				"chatsan-salutes"
			},
			{
				"о7",
				"chatsan-salutes"
			},
			{
				";_;7",
				"chatsan-tearfully-salutes"
			},
			{
				"idk",
				"chatsan-shrugs"
			}
		};

		// Token: 0x0400167F RID: 5759
		private bool _doSanitize;
	}
}
