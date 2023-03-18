using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Store.Components
{
	// Token: 0x02000159 RID: 345
	[RegisterComponent]
	public sealed class CurrencyComponent : Component
	{
		// Token: 0x040003D0 RID: 976
		[Nullable(1)]
		[ViewVariables]
		[DataField("price", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<FixedPoint2, CurrencyPrototype>))]
		public Dictionary<string, FixedPoint2> Price = new Dictionary<string, FixedPoint2>();
	}
}
