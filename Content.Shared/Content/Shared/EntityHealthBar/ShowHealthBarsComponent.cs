using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityHealthBar
{
	// Token: 0x0200003D RID: 61
	[RegisterComponent]
	public sealed class ShowHealthBarsComponent : Component
	{
		// Token: 0x040000A0 RID: 160
		[Nullable(2)]
		[DataField("damageContainer", false, 1, false, false, typeof(PrototypeIdSerializer<DamageContainerPrototype>))]
		public string DamageContainer;
	}
}
