using System;

namespace Content.Shared.Rounding
{
	// Token: 0x020001DD RID: 477
	public static class ContentHelpers
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x00013AE8 File Offset: 0x00011CE8
		public static int RoundToLevels(double actual, double max, int levels)
		{
			if (levels <= 0)
			{
				throw new ArgumentException("Levels must be greater than 0.", "levels");
			}
			if (actual >= max)
			{
				return levels - 1;
			}
			if (actual <= 0.0)
			{
				return 0;
			}
			double num = actual / max;
			double threshold;
			if (levels % 2 == 0)
			{
				threshold = (double)(((float)levels / 2f - 1f) / (float)(levels - 1));
			}
			else
			{
				threshold = 0.5;
			}
			double preround = num * (double)(levels - 1);
			if (num <= threshold || levels <= 2)
			{
				return (int)Math.Ceiling(preround);
			}
			return (int)Math.Floor(preround);
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00013B65 File Offset: 0x00011D65
		public static int RoundToNearestLevels(double actual, double max, int levels)
		{
			if (levels <= 1)
			{
				throw new ArgumentException("Levels must be greater than 1.", "levels");
			}
			if (actual >= max)
			{
				return levels;
			}
			if (actual <= 0.0)
			{
				return 0;
			}
			return (int)Math.Round(actual / max * (double)levels, MidpointRounding.AwayFromZero);
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x00013B9C File Offset: 0x00011D9C
		public static int RoundToEqualLevels(double actual, double max, int levels)
		{
			if (levels <= 1)
			{
				throw new ArgumentException("Levels must be greater than 1.", "levels");
			}
			if (actual >= max)
			{
				return levels - 1;
			}
			if (actual <= 0.0)
			{
				return 0;
			}
			return (int)Math.Round(actual / max * (double)levels, MidpointRounding.ToZero);
		}
	}
}
