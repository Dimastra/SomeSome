using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Client.Animations;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Light.Components
{
	// Token: 0x02000273 RID: 627
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ColorCycleBehaviour : LightBehaviourAnimationTrack, ISerializationHooks
	{
		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0005F9E6 File Offset: 0x0005DBE6
		// (set) Token: 0x06000FF4 RID: 4084 RVA: 0x0005F9EE File Offset: 0x0005DBEE
		[DataField("property", false, 1, false, false, null)]
		public override string Property { get; protected set; } = "Color";

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0005F9F7 File Offset: 0x0005DBF7
		// (set) Token: 0x06000FF6 RID: 4086 RVA: 0x0005F9FF File Offset: 0x0005DBFF
		[DataField("colors", false, 1, false, false, null)]
		public List<Color> ColorsToCycle { get; set; } = new List<Color>();

		// Token: 0x06000FF7 RID: 4087 RVA: 0x0005FA08 File Offset: 0x0005DC08
		public override void OnStart()
		{
			this._colorIndex++;
			if (this._colorIndex > this.ColorsToCycle.Count - 1)
			{
				this._colorIndex = 0;
			}
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x0005FA34 File Offset: 0x0005DC34
		[NullableContext(0)]
		[return: TupleElementNames(new string[]
		{
			"KeyFrameIndex",
			"FramePlayingTime"
		})]
		public override ValueTuple<int, float> AdvancePlayback([Nullable(1)] object context, int prevKeyFrameIndex, float prevPlayingTime, float frameTime)
		{
			float num = prevPlayingTime + frameTime;
			float num2 = num / base.MaxTime;
			switch (base.InterpolateMode)
			{
			case 0:
				base.ApplyProperty(AnimationTrackProperty.InterpolateLinear(this.ColorsToCycle[(this._colorIndex - 1) % this.ColorsToCycle.Count], this.ColorsToCycle[this._colorIndex], num2));
				goto IL_123;
			case 1:
				base.ApplyProperty(AnimationTrackProperty.InterpolateCubic(this.ColorsToCycle[this._colorIndex], this.ColorsToCycle[(this._colorIndex + 1) % this.ColorsToCycle.Count], this.ColorsToCycle[(this._colorIndex + 2) % this.ColorsToCycle.Count], this.ColorsToCycle[(this._colorIndex + 3) % this.ColorsToCycle.Count], num2));
				goto IL_123;
			}
			base.ApplyProperty(this.ColorsToCycle[this._colorIndex]);
			IL_123:
			return new ValueTuple<int, float>(-1, num);
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x0005FB6B File Offset: 0x0005DD6B
		void ISerializationHooks.AfterDeserialization()
		{
			if (this.ColorsToCycle.Count < 2)
			{
				throw new InvalidOperationException("ColorCycleBehaviour has less than 2 colors to cycle");
			}
		}

		// Token: 0x040007E5 RID: 2021
		private int _colorIndex;
	}
}
