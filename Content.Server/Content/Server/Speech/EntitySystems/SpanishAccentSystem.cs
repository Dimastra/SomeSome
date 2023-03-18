using System;
using System.Runtime.CompilerServices;
using Content.Server.Speech.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech.EntitySystems
{
	// Token: 0x020001BE RID: 446
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpanishAccentSystem : EntitySystem
	{
		// Token: 0x060008B6 RID: 2230 RVA: 0x0002CB4B File Offset: 0x0002AD4B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SpanishAccentComponent, AccentGetEvent>(new ComponentEventHandler<SpanishAccentComponent, AccentGetEvent>(this.OnAccent), null, null);
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x0002CB61 File Offset: 0x0002AD61
		public string Accentuate(string message)
		{
			message = this.InsertS(message);
			message = this.ReplaceQuestionMark(message);
			return message;
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x0002CB78 File Offset: 0x0002AD78
		private string InsertS(string message)
		{
			string msg = message.Replace(" s", " es").Replace(" S", " Es");
			if (msg.StartsWith("s", StringComparison.Ordinal))
			{
				return msg.Remove(0, 1).Insert(0, "es");
			}
			if (msg.StartsWith("S", StringComparison.Ordinal))
			{
				return msg.Remove(0, 1).Insert(0, "Es");
			}
			return msg;
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x0002CBEC File Offset: 0x0002ADEC
		private string ReplaceQuestionMark(string message)
		{
			string[] array = AccentSystem.SentenceRegex.Split(message);
			string msg = "";
			foreach (string s in array)
			{
				if (s.EndsWith("?", StringComparison.Ordinal))
				{
					msg += s.Insert(s.Length - s.TrimStart().Length, "¿");
				}
				else
				{
					msg += s;
				}
			}
			return msg;
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x0002CC5A File Offset: 0x0002AE5A
		private void OnAccent(EntityUid uid, SpanishAccentComponent component, AccentGetEvent args)
		{
			args.Message = this.Accentuate(args.Message);
		}
	}
}
