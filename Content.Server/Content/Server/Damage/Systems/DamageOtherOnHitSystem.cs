using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Damage.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C3 RID: 1475
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageOtherOnHitSystem : EntitySystem
	{
		// Token: 0x06001F7D RID: 8061 RVA: 0x000A55FA File Offset: 0x000A37FA
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DamageOtherOnHitComponent, ThrowDoHitEvent>(new ComponentEventHandler<DamageOtherOnHitComponent, ThrowDoHitEvent>(this.OnDoHit), null, null);
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x000A5610 File Offset: 0x000A3810
		private void OnDoHit(EntityUid uid, DamageOtherOnHitComponent component, ThrowDoHitEvent args)
		{
			DamageSpecifier dmg = this._damageableSystem.TryChangeDamage(new EntityUid?(args.Target), component.Damage, component.IgnoreResistances, true, null, args.User);
			if (dmg != null && base.HasComp<MobStateComponent>(args.Target))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ThrowHit;
				LogStringHandler logStringHandler = new LogStringHandler(32, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Target), "target", "ToPrettyString(args.Target)");
				logStringHandler.AppendLiteral(" received ");
				logStringHandler.AppendFormatted<FixedPoint2>(dmg.Total, "damage", "dmg.Total");
				logStringHandler.AppendLiteral(" damage from collision");
				adminLogger.Add(type, ref logStringHandler);
			}
		}

		// Token: 0x04001390 RID: 5008
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001391 RID: 5009
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
