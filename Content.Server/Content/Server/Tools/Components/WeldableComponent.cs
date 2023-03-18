using System;
using System.Runtime.CompilerServices;
using Content.Server.Tools.Systems;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Tools.Components
{
	// Token: 0x0200011A RID: 282
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(WeldableSystem)
	})]
	public sealed class WeldableComponent : SharedWeldableComponent
	{
		// Token: 0x040002FC RID: 764
		[Nullable(1)]
		[DataField("weldingQuality", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		[ViewVariables]
		public string WeldingQuality = "Welding";

		// Token: 0x040002FD RID: 765
		[DataField("weldable", false, 1, false, false, null)]
		[ViewVariables]
		public bool Weldable = true;

		// Token: 0x040002FE RID: 766
		[DataField("fuel", false, 1, false, false, null)]
		[ViewVariables]
		public float FuelConsumption = 1f;

		// Token: 0x040002FF RID: 767
		[DataField("time", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan WeldingTime = TimeSpan.FromSeconds(1.0);

		// Token: 0x04000300 RID: 768
		[Nullable(2)]
		[DataField("weldedExamineMessage", false, 1, false, false, null)]
		[ViewVariables]
		public string WeldedExamineMessage = "weldable-component-examine-is-welded";

		// Token: 0x04000301 RID: 769
		[ViewVariables]
		public bool BeingWelded;

		// Token: 0x04000302 RID: 770
		[ViewVariables]
		public bool IsWelded;
	}
}
