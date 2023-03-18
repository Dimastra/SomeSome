using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Explosion;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x02000518 RID: 1304
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ExplosionSystem)
	})]
	public sealed class ExplosionResistanceComponent : Component
	{
		// Token: 0x0400116A RID: 4458
		[DataField("damageCoefficient", false, 1, false, false, null)]
		public float DamageCoefficient = 1f;

		// Token: 0x0400116B RID: 4459
		[Nullable(1)]
		[ViewVariables]
		[DataField("resistances", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, ExplosionPrototype>))]
		public Dictionary<string, float> Resistances = new Dictionary<string, float>();
	}
}
