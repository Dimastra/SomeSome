using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Components
{
	// Token: 0x0200060C RID: 1548
	[RegisterComponent]
	public sealed class WelderRefinableComponent : Component
	{
		// Token: 0x0400146C RID: 5228
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("refineResult", false, 1, false, false, null)]
		public HashSet<string> RefineResult = new HashSet<string>();

		// Token: 0x0400146D RID: 5229
		[DataField("refineTime", false, 1, false, false, null)]
		public float RefineTime = 2f;

		// Token: 0x0400146E RID: 5230
		[DataField("refineFuel", false, 1, false, false, null)]
		public float RefineFuel;

		// Token: 0x0400146F RID: 5231
		[Nullable(1)]
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Welding";

		// Token: 0x04001470 RID: 5232
		public bool BeingWelded;
	}
}
