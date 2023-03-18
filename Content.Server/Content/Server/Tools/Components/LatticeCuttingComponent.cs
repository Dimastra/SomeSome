using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tools.Components
{
	// Token: 0x02000118 RID: 280
	[RegisterComponent]
	public sealed class LatticeCuttingComponent : Component
	{
		// Token: 0x040002F4 RID: 756
		[DataField("toolComponentNeeded", false, 1, false, false, null)]
		public bool ToolComponentNeeded = true;

		// Token: 0x040002F5 RID: 757
		[Nullable(1)]
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Cutting";

		// Token: 0x040002F6 RID: 758
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 0.25f;

		// Token: 0x040002F7 RID: 759
		[DataField("vacuumDelay", false, 1, false, false, null)]
		public float VacuumDelay = 1.75f;
	}
}
