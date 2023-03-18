using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Maps
{
	// Token: 0x020003D4 RID: 980
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gameMapPool", 1)]
	public sealed class GameMapPoolPrototype : IPrototype
	{
		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06001421 RID: 5153 RVA: 0x00068975 File Offset: 0x00066B75
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000C78 RID: 3192
		[DataField("maps", false, 1, true, false, typeof(PrototypeIdHashSetSerializer<GameMapPrototype>))]
		public readonly HashSet<string> Maps = new HashSet<string>(0);
	}
}
