using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Recycling.Components
{
	// Token: 0x02000249 RID: 585
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RecyclerSystem)
	})]
	public sealed class RecyclableComponent : Component
	{
		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000BBC RID: 3004 RVA: 0x0003DD8A File Offset: 0x0003BF8A
		// (set) Token: 0x06000BBD RID: 3005 RVA: 0x0003DD92 File Offset: 0x0003BF92
		[DataField("safe", false, 1, false, false, null)]
		public bool Safe { get; set; } = true;

		// Token: 0x04000735 RID: 1845
		[Nullable(2)]
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype;

		// Token: 0x04000736 RID: 1846
		[DataField("amount", false, 1, false, false, null)]
		public int Amount = 1;
	}
}
