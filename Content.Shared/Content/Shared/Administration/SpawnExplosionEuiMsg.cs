using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Content.Shared.Explosion;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000747 RID: 1863
	public static class SpawnExplosionEuiMsg
	{
		// Token: 0x0200089C RID: 2204
		[NetSerializable]
		[Serializable]
		public sealed class Close : EuiMessageBase
		{
		}

		// Token: 0x0200089D RID: 2205
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class PreviewRequest : EuiMessageBase
		{
			// Token: 0x06001A0D RID: 6669 RVA: 0x00051C98 File Offset: 0x0004FE98
			public PreviewRequest(MapCoordinates epicenter, string typeId, float totalIntensity, float intensitySlope, float maxIntensity)
			{
				this.Epicenter = epicenter;
				this.TypeId = typeId;
				this.TotalIntensity = totalIntensity;
				this.IntensitySlope = intensitySlope;
				this.MaxIntensity = maxIntensity;
			}

			// Token: 0x04001A8D RID: 6797
			public readonly MapCoordinates Epicenter;

			// Token: 0x04001A8E RID: 6798
			public readonly string TypeId;

			// Token: 0x04001A8F RID: 6799
			public readonly float TotalIntensity;

			// Token: 0x04001A90 RID: 6800
			public readonly float IntensitySlope;

			// Token: 0x04001A91 RID: 6801
			public readonly float MaxIntensity;
		}

		// Token: 0x0200089E RID: 2206
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class PreviewData : EuiMessageBase
		{
			// Token: 0x06001A0E RID: 6670 RVA: 0x00051CC5 File Offset: 0x0004FEC5
			public PreviewData(ExplosionVisualsState explosion, float slope, float totalIntensity)
			{
				this.Slope = slope;
				this.TotalIntensity = totalIntensity;
				this.Explosion = explosion;
			}

			// Token: 0x04001A92 RID: 6802
			public readonly float Slope;

			// Token: 0x04001A93 RID: 6803
			public readonly float TotalIntensity;

			// Token: 0x04001A94 RID: 6804
			public readonly ExplosionVisualsState Explosion;
		}
	}
}
