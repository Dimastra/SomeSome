using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Shared.Audio
{
	// Token: 0x02000681 RID: 1665
	[NullableContext(2)]
	[Nullable(0)]
	public static class AudioHelpers
	{
		// Token: 0x0600146E RID: 5230 RVA: 0x00044198 File Offset: 0x00042398
		[Obsolete("Use variation datafield.")]
		public static AudioParams WithVariation(float amplitude)
		{
			return AudioHelpers.WithVariation(amplitude, null);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000441A4 File Offset: 0x000423A4
		public static AudioParams WithVariation(float amplitude, IRobustRandom rand)
		{
			IoCManager.Resolve<IRobustRandom>(ref rand);
			float scale = (float)RandomExtensions.NextGaussian(rand, 1.0, (double)amplitude);
			return AudioParams.Default.WithPitchScale(scale);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x000441D8 File Offset: 0x000423D8
		public static AudioParams ShiftSemitone(int shift)
		{
			shift = MathHelper.Clamp(shift, -12, 12);
			float pitchMult = AudioHelpers.SemitoneMultipliers[shift + 12];
			return AudioParams.Default.WithPitchScale(pitchMult);
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x00044207 File Offset: 0x00042407
		public static AudioParams WithSemitoneVariation(int variation, IRobustRandom rand)
		{
			IoCManager.Resolve<IRobustRandom>(ref rand);
			variation = Math.Clamp(variation, 0, 12);
			return AudioHelpers.ShiftSemitone(rand.Next(-variation, variation));
		}

		// Token: 0x0400140A RID: 5130
		[Nullable(1)]
		private static readonly float[] SemitoneMultipliers = new float[]
		{
			0.5f,
			0.5297273f,
			0.56122726f,
			0.5946137f,
			0.6299545f,
			0.6674091f,
			0.7071136f,
			0.7491591f,
			0.79370457f,
			0.84088635f,
			0.8909091f,
			0.94386363f,
			1f,
			1.0594546f,
			1.1224545f,
			1.1892046f,
			1.2599318f,
			1.3348409f,
			1.4142046f,
			1.4983182f,
			1.5874091f,
			1.6817955f,
			1.7817954f,
			1.8877499f,
			2f
		};
	}
}
