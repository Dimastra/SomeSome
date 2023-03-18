using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Beam.Components
{
	// Token: 0x0200067B RID: 1659
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class BeamVisualizerEvent : EntityEventArgs
	{
		// Token: 0x0600144E RID: 5198 RVA: 0x00044010 File Offset: 0x00042210
		public BeamVisualizerEvent(EntityUid beam, float distanceLength, Angle userAngle, [Nullable(2)] string bodyState = null, string shader = "unshaded")
		{
			this.Beam = beam;
			this.DistanceLength = distanceLength;
			this.UserAngle = userAngle;
			this.BodyState = bodyState;
			this.Shader = shader;
		}

		// Token: 0x040013F3 RID: 5107
		public readonly EntityUid Beam;

		// Token: 0x040013F4 RID: 5108
		public readonly float DistanceLength;

		// Token: 0x040013F5 RID: 5109
		public readonly Angle UserAngle;

		// Token: 0x040013F6 RID: 5110
		[Nullable(2)]
		public readonly string BodyState;

		// Token: 0x040013F7 RID: 5111
		public readonly string Shader = "unshaded";
	}
}
