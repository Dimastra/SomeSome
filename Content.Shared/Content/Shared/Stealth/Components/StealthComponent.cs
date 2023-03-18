using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Stealth.Components
{
	// Token: 0x02000152 RID: 338
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedStealthSystem)
	})]
	public sealed class StealthComponent : Component
	{
		// Token: 0x040003E4 RID: 996
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x040003E5 RID: 997
		[DataField("hadOutline", false, 1, false, false, null)]
		public bool HadOutline;

		// Token: 0x040003E6 RID: 998
		[DataField("examineThreshold", false, 1, false, false, null)]
		public readonly float ExamineThreshold = 0.5f;

		// Token: 0x040003E7 RID: 999
		[DataField("lastVisibility", false, 1, false, false, null)]
		[Access]
		public float LastVisibility = 1f;

		// Token: 0x040003E8 RID: 1000
		[DataField("lastUpdate", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan? LastUpdated;

		// Token: 0x040003E9 RID: 1001
		[DataField("minVisibility", false, 1, false, false, null)]
		public readonly float MinVisibility = -1f;

		// Token: 0x040003EA RID: 1002
		[DataField("maxVisibility", false, 1, false, false, null)]
		public readonly float MaxVisibility = 1.5f;

		// Token: 0x040003EB RID: 1003
		[Nullable(1)]
		[DataField("examinedDesc", false, 1, false, false, null)]
		public string ExaminedDesc = "stealth-visual-effect";
	}
}
