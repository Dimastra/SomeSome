using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x0200059F RID: 1439
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class DamageTypeTrigger : IThresholdTrigger
	{
		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001DF9 RID: 7673 RVA: 0x0009EEEB File Offset: 0x0009D0EB
		// (set) Token: 0x06001DFA RID: 7674 RVA: 0x0009EEF3 File Offset: 0x0009D0F3
		[DataField("damageType", false, 1, true, false, typeof(PrototypeIdSerializer<DamageTypePrototype>))]
		public string DamageType { get; set; }

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x06001DFB RID: 7675 RVA: 0x0009EEFC File Offset: 0x0009D0FC
		// (set) Token: 0x06001DFC RID: 7676 RVA: 0x0009EF04 File Offset: 0x0009D104
		[DataField("damage", false, 1, true, false, null)]
		public int Damage { get; set; }

		// Token: 0x06001DFD RID: 7677 RVA: 0x0009EF10 File Offset: 0x0009D110
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			FixedPoint2 damageReceived;
			return damageable.Damage.DamageDict.TryGetValue(this.DamageType, out damageReceived) && damageReceived >= this.Damage;
		}
	}
}
