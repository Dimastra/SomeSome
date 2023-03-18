using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001BB RID: 443
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ScrambledAccentSystem : EntitySystem
	{
		// Token: 0x060008A6 RID: 2214 RVA: 0x0002C593 File Offset: 0x0002A793
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ScrambledAccentComponent, AccentGetEvent>(new ComponentEventHandler<ScrambledAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008A7 RID: 2215 RVA: 0x0002C5AC File Offset: 0x0002A7AC
		public string Accentuate(string message)
		{
			string[] words = message.ToLower().Split(Array.Empty<char>());
			if (words.Length < 2)
			{
				int pick = this._random.Next(1, 8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("accent-scrambled-words-");
				defaultInterpolatedStringHandler.AppendFormatted<int>(pick);
				return Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			string[] scrambled = (from x in words
			orderby this._random.Next()
			select x).ToArray<string>();
			string msg = string.Join(" ", scrambled);
			msg = msg[0].ToString().ToUpper() + msg.Remove(0, 1);
			return Regex.Replace(msg, "(?<=\\ )i(?=[\\ \\.\\?]|$)", "I");
		}

		// Token: 0x060008A8 RID: 2216 RVA: 0x0002C662 File Offset: 0x0002A862
		private void OnAccent(EntityUid uid, ScrambledAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}

		// Token: 0x04000540 RID: 1344
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
