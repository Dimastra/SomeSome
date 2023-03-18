using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001BA RID: 442
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RussianAccentSystem : EntitySystem
	{
		// Token: 0x060008A2 RID: 2210 RVA: 0x0002C41E File Offset: 0x0002A61E
		public override void Initialize()
		{
			base.SubscribeLocalEvent<RussianAccentComponent, AccentGetEvent>(new ComponentEventHandler<RussianAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0002C434 File Offset: 0x0002A634
		public static string Accentuate(string message)
		{
			StringBuilder accentedMessage = new StringBuilder(message);
			int i = 0;
			while (i < accentedMessage.Length)
			{
				char c = accentedMessage[i];
				StringBuilder stringBuilder = accentedMessage;
				int index = i;
				char value;
				if (c <= 'Y')
				{
					if (c <= 'N')
					{
						if (c != 'K')
						{
							if (c != 'N')
							{
								goto IL_10E;
							}
							value = 'И';
						}
						else
						{
							value = 'К';
						}
					}
					else if (c != 'R')
					{
						if (c != 'W')
						{
							if (c != 'Y')
							{
								goto IL_10E;
							}
							value = 'У';
						}
						else
						{
							value = 'Ш';
						}
					}
					else
					{
						value = 'Я';
					}
				}
				else if (c <= 'n')
				{
					if (c != 'b')
					{
						switch (c)
						{
						case 'h':
							value = 'н';
							break;
						case 'i':
						case 'j':
						case 'l':
							goto IL_10E;
						case 'k':
							value = 'к';
							break;
						case 'm':
							value = 'м';
							break;
						case 'n':
							value = 'и';
							break;
						default:
							goto IL_10E;
						}
					}
					else
					{
						value = 'в';
					}
				}
				else if (c != 'r')
				{
					if (c != 't')
					{
						if (c != 'w')
						{
							goto IL_10E;
						}
						value = 'ш';
					}
					else
					{
						value = 'т';
					}
				}
				else
				{
					value = 'я';
				}
				IL_117:
				stringBuilder[index] = value;
				i++;
				continue;
				IL_10E:
				value = accentedMessage[i];
				goto IL_117;
			}
			return accentedMessage.ToString();
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0002C578 File Offset: 0x0002A778
		private void OnAccent(EntityUid uid, RussianAccentComponent component, AccentGetEvent args)
		{
			args.Message = RussianAccentSystem.Accentuate(args.Message);
		}
	}
}
