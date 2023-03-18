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
	// Token: 0x020001BC RID: 444
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SkeletonAccentSystem : EntitySystem
	{
		// Token: 0x060008AB RID: 2219 RVA: 0x0002C68B File Offset: 0x0002A88B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SkeletonAccentComponent, AccentGetEvent>(new ComponentEventHandler<SkeletonAccentComponent, AccentGetEvent>(this.OnAccentGet), null, null);
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x0002C6A8 File Offset: 0x0002A8A8
		public string Accentuate(string message, SkeletonAccentComponent component)
		{
			string msg = message;
			msg = Regex.Replace(msg, "(?<!\\w)[^aeiou]one", "bone", RegexOptions.IgnoreCase);
			foreach (KeyValuePair<string, string> keyValuePair in SkeletonAccentSystem.DirectReplacements)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string first = text;
				string replace = text2;
				msg = Regex.Replace(msg, "(?<!\\w)" + first + "(?!\\w)", replace, RegexOptions.IgnoreCase);
			}
			if (RandomExtensions.Prob(this._random, component.ackChance))
			{
				msg = msg + " " + Loc.GetString("skeleton-suffix");
			}
			return msg;
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0002C75C File Offset: 0x0002A95C
		private void OnAccentGet(EntityUid uid, SkeletonAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message, component);
		}

		// Token: 0x04000541 RID: 1345
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000542 RID: 1346
		private static readonly Dictionary<string, string> DirectReplacements = new Dictionary<string, string>
		{
			{
				"fuck you",
				"I've got a BONE to pick with you"
			},
			{
				"fucked",
				"boned"
			},
			{
				"fuck",
				"RATTLE RATTLE"
			},
			{
				"fck",
				"RATTLE RATTLE"
			},
			{
				"shit",
				"RATTLE RATTLE"
			},
			{
				"definitely",
				"make no bones about it"
			},
			{
				"absolutely",
				"make no bones about it"
			},
			{
				"afraid",
				"rattled"
			},
			{
				"scared",
				"rattled"
			},
			{
				"spooked",
				"rattled"
			},
			{
				"shocked",
				"rattled"
			},
			{
				"killed",
				"skeletonized"
			},
			{
				"humorous",
				"humerus"
			},
			{
				"to be a",
				"tibia"
			},
			{
				"under",
				"ulna"
			}
		};
	}
}
