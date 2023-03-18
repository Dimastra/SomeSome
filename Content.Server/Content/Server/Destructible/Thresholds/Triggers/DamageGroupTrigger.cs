using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x0200059D RID: 1437
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class DamageGroupTrigger : IThresholdTrigger
	{
		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001DEF RID: 7663 RVA: 0x0009EE77 File Offset: 0x0009D077
		// (set) Token: 0x06001DF0 RID: 7664 RVA: 0x0009EE7F File Offset: 0x0009D07F
		[DataField("damageGroup", false, 1, true, false, typeof(PrototypeIdSerializer<DamageGroupPrototype>))]
		public string DamageGroup { get; set; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x0009EE88 File Offset: 0x0009D088
		// (set) Token: 0x06001DF2 RID: 7666 RVA: 0x0009EE90 File Offset: 0x0009D090
		[DataField("damage", false, 1, true, false, null)]
		public int Damage { get; set; }

		// Token: 0x06001DF3 RID: 7667 RVA: 0x0009EE99 File Offset: 0x0009D099
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			return damageable.DamagePerGroup[this.DamageGroup] >= this.Damage;
		}
	}
}
