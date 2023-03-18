using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000320 RID: 800
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ThirstComponent : Component
	{
		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0005538D File Offset: 0x0005358D
		[DataField("thresholds", false, 1, false, false, null)]
		public Dictionary<ThirstThreshold, float> ThirstThresholds { get; } = new Dictionary<ThirstThreshold, float>
		{
			{
				ThirstThreshold.OverHydrated,
				600f
			},
			{
				ThirstThreshold.Okay,
				450f
			},
			{
				ThirstThreshold.Thirsty,
				300f
			},
			{
				ThirstThreshold.Parched,
				150f
			},
			{
				ThirstThreshold.Dead,
				0f
			}
		};

		// Token: 0x040009B1 RID: 2481
		[ViewVariables]
		[DataField("baseDecayRate", false, 1, false, false, null)]
		public float BaseDecayRate = 0.1f;

		// Token: 0x040009B2 RID: 2482
		[ViewVariables]
		public float ActualDecayRate;

		// Token: 0x040009B3 RID: 2483
		[ViewVariables]
		public ThirstThreshold CurrentThirstThreshold;

		// Token: 0x040009B4 RID: 2484
		public ThirstThreshold LastThirstThreshold;

		// Token: 0x040009B5 RID: 2485
		[ViewVariables]
		[DataField("startingThirst", false, 1, false, false, null)]
		public float CurrentThirst = -1f;

		// Token: 0x040009B7 RID: 2487
		public static readonly Dictionary<ThirstThreshold, AlertType> ThirstThresholdAlertTypes = new Dictionary<ThirstThreshold, AlertType>
		{
			{
				ThirstThreshold.Thirsty,
				AlertType.Thirsty
			},
			{
				ThirstThreshold.Parched,
				AlertType.Parched
			},
			{
				ThirstThreshold.Dead,
				AlertType.Parched
			}
		};
	}
}
