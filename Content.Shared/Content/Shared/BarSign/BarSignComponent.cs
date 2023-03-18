using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.BarSign
{
	// Token: 0x0200067C RID: 1660
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BarSignComponent : Component
	{
		// Token: 0x040013F8 RID: 5112
		[Nullable(2)]
		[DataField("current", false, 1, false, false, typeof(PrototypeIdSerializer<BarSignPrototype>))]
		public string CurrentSign;
	}
}
