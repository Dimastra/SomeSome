using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000584 RID: 1412
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedAnchorableSystem)
	})]
	public sealed class AnchorableComponent : Component
	{
		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001159 RID: 4441 RVA: 0x00039087 File Offset: 0x00037287
		// (set) Token: 0x0600115A RID: 4442 RVA: 0x0003908F File Offset: 0x0003728F
		[DataField("tool", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string Tool { get; private set; } = "Anchoring";

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x0600115B RID: 4443 RVA: 0x00039098 File Offset: 0x00037298
		// (set) Token: 0x0600115C RID: 4444 RVA: 0x000390A0 File Offset: 0x000372A0
		[DataField("snap", false, 1, false, false, null)]
		[ViewVariables]
		public bool Snap { get; private set; } = true;

		// Token: 0x04001009 RID: 4105
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 1f;
	}
}
