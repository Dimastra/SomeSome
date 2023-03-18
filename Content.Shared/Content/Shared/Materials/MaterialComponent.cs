using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Materials
{
	// Token: 0x0200032F RID: 815
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MaterialComponent : Component
	{
		// Token: 0x04000947 RID: 2375
		[Nullable(1)]
		[DataField("materials", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MaterialPrototype>))]
		public readonly Dictionary<string, int> Materials = new Dictionary<string, int>();
	}
}
