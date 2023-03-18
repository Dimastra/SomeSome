using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B5 RID: 437
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LizardAccentSystem : EntitySystem
	{
		// Token: 0x0600088E RID: 2190 RVA: 0x0002B988 File Offset: 0x00029B88
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LizardAccentComponent, AccentGetEvent>(new ComponentEventHandler<LizardAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x0002B9A4 File Offset: 0x00029BA4
		private void OnAccent(EntityUid uid, LizardAccentComponent component, AccentGetEvent args)
		{
			string message = args.Message;
			message = Regex.Replace(message, "s+", "sss");
			message = Regex.Replace(message, "S+", "SSS");
			message = Regex.Replace(message, "(\\w)x", "$1kss");
			message = Regex.Replace(message, "\\bx([\\-|r|R]|\\b)", "ecks$1");
			message = Regex.Replace(message, "\\bX([\\-|r|R]|\\b)", "ECKS$1");
			message = Regex.Replace(message, "с+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"сс",
				"ссс"
			}));
			message = Regex.Replace(message, "С+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"Сс",
				"Ссс"
			}));
			message = Regex.Replace(message, "з+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"сс",
				"ссс"
			}));
			message = Regex.Replace(message, "З+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"Сс",
				"Ссс"
			}));
			message = Regex.Replace(message, "ш+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"шш",
				"шшш"
			}));
			message = Regex.Replace(message, "Ш+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"Шш",
				"Шшш"
			}));
			message = Regex.Replace(message, "ч+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"щщ",
				"щщщ"
			}));
			message = Regex.Replace(message, "Ч+", RandomExtensions.Pick<string>(this._random, new List<string>
			{
				"Щщ",
				"Щщщ"
			}));
			args.Message = message;
		}

		// Token: 0x04000537 RID: 1335
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
