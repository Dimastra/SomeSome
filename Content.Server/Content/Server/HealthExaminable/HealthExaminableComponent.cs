using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.HealthExaminable
{
	// Token: 0x02000475 RID: 1141
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(HealthExaminableSystem)
	})]
	public sealed class HealthExaminableComponent : Component
	{
		// Token: 0x04000E52 RID: 3666
		public List<FixedPoint2> Thresholds = new List<FixedPoint2>
		{
			FixedPoint2.New(10),
			FixedPoint2.New(25),
			FixedPoint2.New(50),
			FixedPoint2.New(75)
		};

		// Token: 0x04000E53 RID: 3667
		[DataField("examinableTypes", false, 1, true, false, typeof(PrototypeIdHashSetSerializer<DamageTypePrototype>))]
		public HashSet<string> ExaminableTypes;

		// Token: 0x04000E54 RID: 3668
		[DataField("locPrefix", false, 1, false, false, null)]
		public string LocPrefix = "carbon";
	}
}
