using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Content.Server.White.TTS
{
	// Token: 0x02000087 RID: 135
	[NullableContext(1)]
	[Nullable(0)]
	public static class NumberConverter
	{
		// Token: 0x06000201 RID: 513 RVA: 0x0000B7F4 File Offset: 0x000099F4
		public static string NumberToText(long value, bool male = true)
		{
			if (value >= (long)Math.Pow(10.0, 15.0))
			{
				return string.Empty;
			}
			if (value == 0L)
			{
				return "ноль";
			}
			StringBuilder str = new StringBuilder();
			if (value < 0L)
			{
				str.Append("минус");
				value = -value;
			}
			value = NumberConverter.AppendPeriod(value, 1000000000000L, str, "триллион", "триллиона", "триллионов", true);
			value = NumberConverter.AppendPeriod(value, 1000000000L, str, "миллиард", "миллиарда", "миллиардов", true);
			value = NumberConverter.AppendPeriod(value, 1000000L, str, "миллион", "миллиона", "миллионов", true);
			value = NumberConverter.AppendPeriod(value, 1000L, str, "тысяча", "тысячи", "тысяч", false);
			int hundreds = (int)(value / 100L);
			if (hundreds != 0)
			{
				NumberConverter.AppendWithSpace(str, NumberConverter.Hunds[hundreds]);
			}
			int less100 = (int)(value % 100L);
			string[] frac20 = male ? NumberConverter.Frac20Male : NumberConverter.Frac20Female;
			if (less100 < 20)
			{
				NumberConverter.AppendWithSpace(str, frac20[less100]);
			}
			else
			{
				int tens = less100 / 10;
				NumberConverter.AppendWithSpace(str, NumberConverter.Tens[tens]);
				if (less100 % 10 != 0)
				{
					str.Append(" " + frac20[less100 % 10]);
				}
			}
			return str.ToString();
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B93A File Offset: 0x00009B3A
		private static void AppendWithSpace(StringBuilder stringBuilder, string str)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(str);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000B95C File Offset: 0x00009B5C
		private static long AppendPeriod(long value, long power, StringBuilder str, string declension1, string declension2, string declension5, bool male)
		{
			int thousands = (int)(value / power);
			if (thousands > 0)
			{
				NumberConverter.AppendWithSpace(str, NumberConverter.NumberToText((long)thousands, male, declension1, declension2, declension5));
				return value % power;
			}
			return value;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B98B File Offset: 0x00009B8B
		private static string NumberToText(long value, bool male, string valueDeclensionFor1, string valueDeclensionFor2, string valueDeclensionFor5)
		{
			return NumberConverter.NumberToText(value, male) + " " + NumberConverter.GetDeclension((int)(value % 10L), valueDeclensionFor1, valueDeclensionFor2, valueDeclensionFor5);
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		private static string GetDeclension(int val, string one, string two, string five)
		{
			int t = (val % 100 > 20) ? (val % 10) : (val % 20);
			if (t == 1)
			{
				return one;
			}
			if (t - 2 > 2)
			{
				return five;
			}
			return two;
		}

		// Token: 0x04000179 RID: 377
		private static readonly string[] Frac20Male = new string[]
		{
			"",
			"один",
			"два",
			"три",
			"четыре",
			"пять",
			"шесть",
			"семь",
			"восемь",
			"девять",
			"десять",
			"одиннадцать",
			"двенадцать",
			"тринадцать",
			"четырнадцать",
			"пятнадцать",
			"шестнадцать",
			"семнадцать",
			"восемнадцать",
			"девятнадцать"
		};

		// Token: 0x0400017A RID: 378
		private static readonly string[] Frac20Female = new string[]
		{
			"",
			"одна",
			"две",
			"три",
			"четыре",
			"пять",
			"шесть",
			"семь",
			"восемь",
			"девять",
			"десять",
			"одиннадцать",
			"двенадцать",
			"тринадцать",
			"четырнадцать",
			"пятнадцать",
			"шестнадцать",
			"семнадцать",
			"восемнадцать",
			"девятнадцать"
		};

		// Token: 0x0400017B RID: 379
		private static readonly string[] Hunds = new string[]
		{
			"",
			"сто",
			"двести",
			"триста",
			"четыреста",
			"пятьсот",
			"шестьсот",
			"семьсот",
			"восемьсот",
			"девятьсот"
		};

		// Token: 0x0400017C RID: 380
		private static readonly string[] Tens = new string[]
		{
			"",
			"десять",
			"двадцать",
			"тридцать",
			"сорок",
			"пятьдесят",
			"шестьдесят",
			"семьдесят",
			"восемьдесят",
			"девяносто"
		};
	}
}
