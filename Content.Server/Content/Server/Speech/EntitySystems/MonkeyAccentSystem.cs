using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001B7 RID: 439
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MonkeyAccentSystem : EntitySystem
	{
		// Token: 0x06000896 RID: 2198 RVA: 0x0002C0AF File Offset: 0x0002A2AF
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MonkeyAccentComponent, AccentGetEvent>(new ComponentEventHandler<MonkeyAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x0002C0C8 File Offset: 0x0002A2C8
		public string Accentuate(string message)
		{
			string[] words = message.Split(Array.Empty<char>());
			StringBuilder accentedMessage = new StringBuilder(message.Length + 2);
			for (int i = 0; i < words.Length; i++)
			{
				string word = words[i];
				if (this._random.NextDouble() >= 0.5)
				{
					if (word.Length > 1)
					{
						foreach (char c in word)
						{
							accentedMessage.Append('O');
						}
						if (this._random.NextDouble() >= 0.3)
						{
							accentedMessage.Append('K');
						}
					}
					else
					{
						accentedMessage.Append('O');
					}
				}
				else
				{
					foreach (char c2 in word)
					{
						if (this._random.NextDouble() >= 0.8)
						{
							accentedMessage.Append('H');
						}
						else
						{
							accentedMessage.Append('A');
						}
					}
				}
				if (i < words.Length - 1)
				{
					accentedMessage.Append(' ');
				}
			}
			accentedMessage.Append('!');
			return accentedMessage.ToString();
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x0002C1EB File Offset: 0x0002A3EB
		private void OnAccent(EntityUid uid, MonkeyAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}

		// Token: 0x0400053A RID: 1338
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
