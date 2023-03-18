using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C6 RID: 1478
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeforeDamageUserOnTriggerEvent : EntityEventArgs
	{
		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001F87 RID: 8071 RVA: 0x000A5865 File Offset: 0x000A3A65
		// (set) Token: 0x06001F88 RID: 8072 RVA: 0x000A586D File Offset: 0x000A3A6D
		public DamageSpecifier Damage { get; set; }

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06001F89 RID: 8073 RVA: 0x000A5876 File Offset: 0x000A3A76
		public EntityUid Tripper { get; }

		// Token: 0x06001F8A RID: 8074 RVA: 0x000A587E File Offset: 0x000A3A7E
		public BeforeDamageUserOnTriggerEvent(DamageSpecifier damage, EntityUid target)
		{
			this.Damage = damage;
			this.Tripper = target;
		}
	}
}
