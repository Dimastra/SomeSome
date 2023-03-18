using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Guardian
{
	// Token: 0x02000483 RID: 1155
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GuardianCreatorComponent : Component
	{
		// Token: 0x1700031A RID: 794
		// (get) Token: 0x0600170D RID: 5901 RVA: 0x000793F5 File Offset: 0x000775F5
		// (set) Token: 0x0600170E RID: 5902 RVA: 0x000793FD File Offset: 0x000775FD
		[DataField("guardianProto", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string GuardianProto { get; set; }

		// Token: 0x04000E75 RID: 3701
		public bool Used;

		// Token: 0x04000E77 RID: 3703
		[DataField("delay", false, 1, false, false, null)]
		public float InjectionDelay = 5f;

		// Token: 0x04000E78 RID: 3704
		public bool Injecting;
	}
}
