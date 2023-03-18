using System;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;

namespace Content.Client.Light.Components
{
	// Token: 0x02000272 RID: 626
	public sealed class RandomizeBehaviour : LightBehaviourAnimationTrack
	{
		// Token: 0x06000FEF RID: 4079 RVA: 0x0005F7CC File Offset: 0x0005D9CC
		public override void OnInitialize()
		{
			this._randomValue1 = (float)AnimationTrackProperty.InterpolateLinear(base.StartValue, base.EndValue, (float)this._random.NextDouble());
			this._randomValue2 = (float)AnimationTrackProperty.InterpolateLinear(base.StartValue, base.EndValue, (float)this._random.NextDouble());
			this._randomValue3 = (float)AnimationTrackProperty.InterpolateLinear(base.StartValue, base.EndValue, (float)this._random.NextDouble());
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x0005F870 File Offset: 0x0005DA70
		public override void OnStart()
		{
			if (this.Property == "Enabled")
			{
				base.ApplyProperty(this._random.NextDouble() < 0.5);
				return;
			}
			if (base.InterpolateMode == 1)
			{
				this._randomValue1 = this._randomValue2;
				this._randomValue2 = this._randomValue3;
			}
			this._randomValue3 = this._randomValue4;
			this._randomValue4 = (float)AnimationTrackProperty.InterpolateLinear(base.StartValue, base.EndValue, (float)this._random.NextDouble());
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x0005F910 File Offset: 0x0005DB10
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
				return new ValueTuple<int, float>(-1, num);
			}
			switch (base.InterpolateMode)
			{
			case 0:
				base.ApplyProperty(AnimationTrackProperty.InterpolateLinear(this._randomValue3, this._randomValue4, num2));
				goto IL_C2;
			case 1:
				base.ApplyProperty(AnimationTrackProperty.InterpolateCubic(this._randomValue1, this._randomValue2, this._randomValue3, this._randomValue4, num2));
				goto IL_C2;
			}
			base.ApplyProperty((num2 < 0.5f) ? this._randomValue3 : this._randomValue4);
			IL_C2:
			return new ValueTuple<int, float>(-1, num);
		}

		// Token: 0x040007DF RID: 2015
		private float _randomValue1;

		// Token: 0x040007E0 RID: 2016
		private float _randomValue2;

		// Token: 0x040007E1 RID: 2017
		private float _randomValue3;

		// Token: 0x040007E2 RID: 2018
		private float _randomValue4;
	}
}
