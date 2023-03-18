using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Clothing.Components
{
	// Token: 0x0200063B RID: 1595
	[RegisterComponent]
	public sealed class LoadoutComponent : Component
	{
		// Token: 0x040014CB RID: 5323
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("prototypes", false, 1, true, false, typeof(PrototypeIdListSerializer<StartingGearPrototype>))]
		public List<string> Prototypes;
	}
}
