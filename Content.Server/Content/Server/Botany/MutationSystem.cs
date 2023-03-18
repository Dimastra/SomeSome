using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Botany
{
	// Token: 0x020006FA RID: 1786
	[NullableContext(1)]
	[Nullable(0)]
	public class MutationSystem : EntitySystem
	{
		// Token: 0x0600255C RID: 9564 RVA: 0x000C3BC4 File Offset: 0x000C1DC4
		public void MutateSeed(SeedData seed, float severity)
		{
			this.MutateFloat(ref seed.NutrientConsumption, 0.05f, 1.2f, 5, 215, severity);
			this.MutateFloat(ref seed.WaterConsumption, 3f, 9f, 5, 215, severity);
			this.MutateFloat(ref seed.IdealHeat, 263f, 323f, 5, 215, severity);
			this.MutateFloat(ref seed.HeatTolerance, 2f, 25f, 5, 215, severity);
			this.MutateFloat(ref seed.IdealLight, 0f, 14f, 5, 215, severity);
			this.MutateFloat(ref seed.LightTolerance, 1f, 5f, 5, 215, severity);
			this.MutateFloat(ref seed.ToxinsTolerance, 1f, 10f, 5, 215, severity);
			this.MutateFloat(ref seed.LowPressureTolerance, 60f, 100f, 5, 215, severity);
			this.MutateFloat(ref seed.HighPressureTolerance, 100f, 140f, 5, 215, severity);
			this.MutateFloat(ref seed.PestTolerance, 0f, 15f, 5, 215, severity);
			this.MutateFloat(ref seed.WeedTolerance, 0f, 15f, 5, 215, severity);
			this.MutateFloat(ref seed.Endurance, 50f, 150f, 5, 215, 2f * severity);
			this.MutateInt(ref seed.Yield, 3, 10, 5, 215, 2f * severity);
			this.MutateFloat(ref seed.Lifespan, 10f, 80f, 5, 215, 2f * severity);
			this.MutateFloat(ref seed.Maturation, 3f, 8f, 5, 215, 2f * severity);
			this.MutateFloat(ref seed.Production, 1f, 10f, 5, 215, 2f * severity);
			this.MutateFloat(ref seed.Potency, 30f, 100f, 5, 215, 2f * severity);
			this.MutateBool(ref seed.Viable, false, 30, 215, severity);
			this.MutateBool(ref seed.Seedless, true, 10, 215, severity);
			this.MutateBool(ref seed.Slip, true, 10, 215, severity);
			this.MutateBool(ref seed.Sentient, true, 10, 215, severity);
			this.MutateBool(ref seed.Ligneous, true, 10, 215, severity);
			this.MutateBool(ref seed.Bioluminescent, true, 10, 215, severity);
			seed.BioluminescentColor = this.RandomColor(seed.BioluminescentColor, 10, 215, severity);
		}

		// Token: 0x0600255D RID: 9565 RVA: 0x000C3E74 File Offset: 0x000C2074
		public SeedData Cross(SeedData a, SeedData b)
		{
			SeedData result = b.Clone();
			result.Chemicals = (this.random(0.5f) ? a.Chemicals : result.Chemicals);
			this.CrossFloat(ref result.NutrientConsumption, a.NutrientConsumption);
			this.CrossFloat(ref result.WaterConsumption, a.WaterConsumption);
			this.CrossFloat(ref result.IdealHeat, a.IdealHeat);
			this.CrossFloat(ref result.HeatTolerance, a.HeatTolerance);
			this.CrossFloat(ref result.IdealLight, a.IdealLight);
			this.CrossFloat(ref result.LightTolerance, a.LightTolerance);
			this.CrossFloat(ref result.ToxinsTolerance, a.ToxinsTolerance);
			this.CrossFloat(ref result.LowPressureTolerance, a.LowPressureTolerance);
			this.CrossFloat(ref result.HighPressureTolerance, a.HighPressureTolerance);
			this.CrossFloat(ref result.PestTolerance, a.PestTolerance);
			this.CrossFloat(ref result.WeedTolerance, a.WeedTolerance);
			this.CrossFloat(ref result.Endurance, a.Endurance);
			this.CrossInt(ref result.Yield, a.Yield);
			this.CrossFloat(ref result.Lifespan, a.Lifespan);
			this.CrossFloat(ref result.Maturation, a.Maturation);
			this.CrossFloat(ref result.Production, a.Production);
			this.CrossFloat(ref result.Potency, a.Potency);
			this.CrossBool(ref result.Seedless, a.Seedless);
			this.CrossBool(ref result.Viable, a.Viable);
			this.CrossBool(ref result.Slip, a.Slip);
			this.CrossBool(ref result.Sentient, a.Sentient);
			this.CrossBool(ref result.Ligneous, a.Ligneous);
			this.CrossBool(ref result.Bioluminescent, a.Bioluminescent);
			result.BioluminescentColor = (this.random(0.5f) ? a.BioluminescentColor : result.BioluminescentColor);
			if (a.Name == result.Name && this.random(0.7f))
			{
				result.Seedless = true;
			}
			return result;
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x000C4090 File Offset: 0x000C2290
		private void MutateFloat(ref float val, float min, float max, int bits, int totalbits, float mult)
		{
			float p = mult * (float)bits / (float)totalbits;
			if (!this.random(p))
			{
				return;
			}
			int i = (int)Math.Round((double)((val - min) / (max - min) * (float)bits));
			i = Math.Clamp(i, 0, bits);
			float p_increase = 1f - (float)i / (float)bits;
			int np;
			if (this.random(p_increase))
			{
				np = i + 1;
			}
			else
			{
				np = i - 1;
			}
			float nval = MathF.Min(MathF.Max((float)np / (float)bits * (max - min) + min, min), max);
			val = nval;
		}

		// Token: 0x0600255F RID: 9567 RVA: 0x000C4110 File Offset: 0x000C2310
		private void MutateInt(ref int n, int min, int max, int bits, int totalbits, float mult)
		{
			float p = mult * (float)bits / (float)totalbits;
			if (!this.random(p))
			{
				return;
			}
			float p_increase = 1f - (float)n / (float)bits;
			int np;
			if (this.random(p_increase))
			{
				np = n + 1;
			}
			else
			{
				np = n - 1;
			}
			np = Math.Min(Math.Max(np, min), max);
			n = np;
		}

		// Token: 0x06002560 RID: 9568 RVA: 0x000C4168 File Offset: 0x000C2368
		private void MutateBool(ref bool val, bool polarity, int bits, int totalbits, float mult)
		{
			float p = mult * (float)bits / (float)totalbits;
			if (!this.random(p))
			{
				return;
			}
			val = polarity;
		}

		// Token: 0x06002561 RID: 9569 RVA: 0x000C418C File Offset: 0x000C238C
		private Color RandomColor(Color color, int bits, int totalbits, float mult)
		{
			float p = mult * (float)bits / (float)totalbits;
			if (this.random(p))
			{
				List<Color> colors = new List<Color>
				{
					Color.White,
					Color.Red,
					Color.Yellow,
					Color.Green,
					Color.Blue,
					Color.Purple,
					Color.Pink
				};
				return RandomExtensions.Pick<Color>(IoCManager.Resolve<IRobustRandom>(), colors);
			}
			return color;
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x000C420B File Offset: 0x000C240B
		private void CrossFloat(ref float val, float other)
		{
			val = (this.random(0.5f) ? val : other);
		}

		// Token: 0x06002563 RID: 9571 RVA: 0x000C4221 File Offset: 0x000C2421
		private void CrossInt(ref int val, int other)
		{
			val = (this.random(0.5f) ? val : other);
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x000C4237 File Offset: 0x000C2437
		private void CrossBool(ref bool val, bool other)
		{
			val = (this.random(0.5f) ? val : other);
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x000C424D File Offset: 0x000C244D
		private bool random(float p)
		{
			return RandomExtensions.Prob(this._robustRandom, p);
		}

		// Token: 0x04001712 RID: 5906
		[Dependency]
		private readonly IRobustRandom _robustRandom;
	}
}
