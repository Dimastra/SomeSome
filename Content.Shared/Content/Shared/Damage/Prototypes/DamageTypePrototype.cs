using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Prototypes
{
	// Token: 0x0200053D RID: 1341
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("damageType", 1)]
	public sealed class DamageTypePrototype : IPrototype
	{
		// Token: 0x17000339 RID: 825
		// (get) Token: 0x0600105F RID: 4191 RVA: 0x00035C76 File Offset: 0x00033E76
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x00035C7E File Offset: 0x00033E7E
		// (set) Token: 0x06001061 RID: 4193 RVA: 0x00035C86 File Offset: 0x00033E86
		[DataField("armorCoefficientPrice", false, 1, false, false, null)]
		public double ArmorPriceCoefficient { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x00035C8F File Offset: 0x00033E8F
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x00035C97 File Offset: 0x00033E97
		[DataField("armorFlatPrice", false, 1, false, false, null)]
		public double ArmorPriceFlat { get; set; }
	}
}
