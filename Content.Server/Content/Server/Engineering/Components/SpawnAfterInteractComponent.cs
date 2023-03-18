using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Engineering.Components
{
	// Token: 0x0200052E RID: 1326
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SpawnAfterInteractComponent : Component
	{
		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001B9B RID: 7067 RVA: 0x00093B0E File Offset: 0x00091D0E
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06001B9C RID: 7068 RVA: 0x00093B16 File Offset: 0x00091D16
		[DataField("ignoreDistance", false, 1, false, false, null)]
		public bool IgnoreDistance { get; }

		// Token: 0x040011B4 RID: 4532
		[DataField("doAfter", false, 1, false, false, null)]
		public float DoAfterTime;

		// Token: 0x040011B5 RID: 4533
		[DataField("removeOnInteract", false, 1, false, false, null)]
		public bool RemoveOnInteract;
	}
}
