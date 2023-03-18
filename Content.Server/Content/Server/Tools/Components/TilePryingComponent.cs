using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Tools.Components
{
	// Token: 0x02000119 RID: 281
	[RegisterComponent]
	public sealed class TilePryingComponent : Component
	{
		// Token: 0x040002F8 RID: 760
		[DataField("toolComponentNeeded", false, 1, false, false, null)]
		public bool ToolComponentNeeded = true;

		// Token: 0x040002F9 RID: 761
		[Nullable(1)]
		[DataField("qualityNeeded", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string QualityNeeded = "Prying";

		// Token: 0x040002FA RID: 762
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;

		// Token: 0x040002FB RID: 763
		[Nullable(2)]
		[DataField("cancelToken", false, 1, false, false, null)]
		public CancellationTokenSource CancelToken;
	}
}
