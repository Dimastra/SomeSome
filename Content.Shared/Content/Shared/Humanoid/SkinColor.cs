using System;
using Robust.Shared.Maths;

namespace Content.Shared.Humanoid
{
	// Token: 0x02000412 RID: 1042
	public static class SkinColor
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000C48 RID: 3144 RVA: 0x0002896F File Offset: 0x00026B6F
		public static Color ValidHumanSkinTone
		{
			get
			{
				return Color.FromHsv(new Vector4(0.07f, 0.2f, 1f, 1f));
			}
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x0002898F File Offset: 0x00026B8F
		public static Color ValidTintedHuesSkinTone(Color color)
		{
			return SkinColor.TintedHues(color);
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x00028998 File Offset: 0x00026B98
		public static Color HumanSkinTone(int tone)
		{
			tone = Math.Clamp(tone, 0, 100);
			int rangeOffset = tone - 20;
			float hue = 25f;
			float sat = 20f;
			float val = 100f;
			if (rangeOffset <= 0)
			{
				hue += (float)Math.Abs(rangeOffset);
			}
			else
			{
				sat += (float)rangeOffset;
				val -= (float)rangeOffset;
			}
			return Color.FromHsv(new Vector4(hue / 360f, sat / 100f, val / 100f, 1f));
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x00028A08 File Offset: 0x00026C08
		public static float HumanSkinToneFromColor(Color color)
		{
			Vector4 hsv = Color.ToHsv(color);
			if (Math.Clamp(hsv.X, 0.06944445f, 1f) > 0.06944445f && (double)hsv.Z == 1.0)
			{
				return Math.Abs(45f - hsv.X * 360f);
			}
			return hsv.Y * 100f;
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00028A70 File Offset: 0x00026C70
		public static bool VerifyHumanSkinTone(Color color)
		{
			Vector4 vector = Color.ToHsv(color);
			double hue = Math.Round((double)(vector.X * 360f));
			double sat = Math.Round((double)(vector.Y * 100f));
			double val = Math.Round((double)(vector.Z * 100f));
			return hue >= 25.0 && hue <= 45.0 && sat >= 20.0 && val >= 20.0;
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x00028AF0 File Offset: 0x00026CF0
		public static Color TintedHues(Color color)
		{
			Vector4 newColor = Color.ToHsv(color);
			newColor.Y = 0.1f;
			return Color.FromHsv(newColor);
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x00028B16 File Offset: 0x00026D16
		public static bool VerifyTintedHues(Color color)
		{
			return Color.ToHsv(color).Y != 0.1f;
		}
	}
}
