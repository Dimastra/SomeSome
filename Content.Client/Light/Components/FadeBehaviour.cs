using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Light.Components
{
	// Token: 0x02000271 RID: 625
	public sealed class FadeBehaviour : LightBehaviourAnimationTrack
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0005F689 File Offset: 0x0005D889
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0005F691 File Offset: 0x0005D891
		[DataField("reverseWhenFinished", false, 1, false, false, null)]
		public bool ReverseWhenFinished { get; set; }

		// Token: 0x06000FEC RID: 4076 RVA: 0x0005F69C File Offset: 0x0005D89C
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
				base.ApplyProperty(num2 < base.EndValue);
				return new ValueTuple<int, float>(-1, num);
			}
			if (this.ReverseWhenFinished)
			{
				if (num2 < 0.5f)
				{
					this.ApplyInterpolation(base.StartValue, base.EndValue, num2 * 2f);
				}
				else
				{
					this.ApplyInterpolation(base.EndValue, base.StartValue, (num2 - 0.5f) * 2f);
				}
			}
			else
			{
				this.ApplyInterpolation(base.StartValue, base.EndValue, num2);
			}
			return new ValueTuple<int, float>(-1, num);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x0005F74C File Offset: 0x0005D94C
		private void ApplyInterpolation(float start, float end, float interpolateValue)
		{
			switch (base.InterpolateMode)
			{
			case 0:
				base.ApplyProperty(AnimationTrackProperty.InterpolateLinear(start, end, interpolateValue));
				return;
			case 1:
				base.ApplyProperty(AnimationTrackProperty.InterpolateCubic(end, start, end, start, interpolateValue));
				return;
			}
			base.ApplyProperty((interpolateValue < 0.5f) ? start : end);
		}
	}
}
