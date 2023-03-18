using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;

namespace Content.Client.Light.Components
{
	// Token: 0x02000270 RID: 624
	public sealed class PulseBehaviour : LightBehaviourAnimationTrack
	{
		// Token: 0x06000FE8 RID: 4072 RVA: 0x0005F4E4 File Offset: 0x0005D6E4
		[return: TupleElementNames(new string[]
		{
			"KeyFrameIndex",
			"FramePlayingTime"
		})]
		public override ValueTuple<int, float> AdvancePlayback([Nullable(1)] object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
		{
			float num = prevPlayingTime + frameTime;
			float num2 = num / base.MaxTime;
			if (this.Property == "Enabled")
			{
				base.ApplyProperty(num2 < 0.5f);
				return new ValueTuple<int, float>(-1, num);
			}
			if (num2 < 0.5f)
			{
				switch (base.InterpolateMode)
				{
				case 0:
					base.ApplyProperty(AnimationTrackProperty.InterpolateLinear(base.StartValue, base.EndValue, num2 * 2f));
					goto IL_189;
				case 1:
					base.ApplyProperty(AnimationTrackProperty.InterpolateCubic(base.EndValue, base.StartValue, base.EndValue, base.StartValue, num2 * 2f));
					goto IL_189;
				}
				base.ApplyProperty(base.StartValue);
			}
			else
			{
				switch (base.InterpolateMode)
				{
				case 0:
					base.ApplyProperty(AnimationTrackProperty.InterpolateLinear(base.EndValue, base.StartValue, (num2 - 0.5f) * 2f));
					goto IL_189;
				case 1:
					base.ApplyProperty(AnimationTrackProperty.InterpolateCubic(base.StartValue, base.EndValue, base.StartValue, base.EndValue, (num2 - 0.5f) * 2f));
					goto IL_189;
				}
				base.ApplyProperty(base.EndValue);
			}
			IL_189:
			return new ValueTuple<int, float>(-1, num);
		}
	}
}
