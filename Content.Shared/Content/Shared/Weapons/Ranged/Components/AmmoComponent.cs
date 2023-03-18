using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged.Components
{
	// Token: 0x02000059 RID: 89
	[RegisterComponent]
	[Virtual]
	public class AmmoComponent : Component, IShootable
	{
		// Token: 0x0400010B RID: 267
		[Nullable(2)]
		[ViewVariables]
		[DataField("muzzleFlash", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string MuzzleFlash = "MuzzleFlashEffect";
	}
}
