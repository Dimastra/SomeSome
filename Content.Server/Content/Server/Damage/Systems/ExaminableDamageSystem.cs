using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Components;
using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds;
using Content.Server.Destructible.Thresholds.Triggers;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Rounding;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C7 RID: 1479
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExaminableDamageSystem : EntitySystem
	{
		// Token: 0x06001F8B RID: 8075 RVA: 0x000A5894 File Offset: 0x000A3A94
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExaminableDamageComponent, ComponentInit>(new ComponentEventHandler<ExaminableDamageComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ExaminableDamageComponent, ExaminedEvent>(new ComponentEventHandler<ExaminableDamageComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x000A58C4 File Offset: 0x000A3AC4
		private void OnInit(EntityUid uid, ExaminableDamageComponent component, ComponentInit args)
		{
			if (component.MessagesProtoId == null)
			{
				return;
			}
			component.MessagesProto = this._prototype.Index<ExaminableDamagePrototype>(component.MessagesProtoId);
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x000A58E8 File Offset: 0x000A3AE8
		private void OnExamine(EntityUid uid, ExaminableDamageComponent component, ExaminedEvent args)
		{
			if (component.MessagesProto == null)
			{
				return;
			}
			string[] messages = component.MessagesProto.Messages;
			if (messages.Length == 0)
			{
				return;
			}
			int level = this.GetDamageLevel(uid, component, null, null);
			string msg = Loc.GetString(messages[level]);
			args.PushMarkup(msg);
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x000A592C File Offset: 0x000A3B2C
		[NullableContext(2)]
		private int GetDamageLevel(EntityUid uid, ExaminableDamageComponent component = null, DamageableComponent damageable = null, DestructibleComponent destructible = null)
		{
			if (!base.Resolve<ExaminableDamageComponent, DamageableComponent, DestructibleComponent>(uid, ref component, ref damageable, ref destructible, true))
			{
				return 0;
			}
			if (component.MessagesProto == null)
			{
				return 0;
			}
			int maxLevels = component.MessagesProto.Messages.Length - 1;
			if (maxLevels <= 0)
			{
				return 0;
			}
			DamageThreshold damageThreshold2 = destructible.Thresholds.LastOrDefault((DamageThreshold threshold) => threshold.Trigger is DamageTrigger);
			DamageTrigger trigger = (DamageTrigger)((damageThreshold2 != null) ? damageThreshold2.Trigger : null);
			if (trigger == null)
			{
				return 0;
			}
			FixedPoint2 damage = damageable.TotalDamage;
			int damageThreshold = trigger.Damage;
			return ContentHelpers.RoundToNearestLevels((double)((damageThreshold == 0) ? 0f : ((float)damage / (float)damageThreshold)), 1.0, maxLevels);
		}

		// Token: 0x04001396 RID: 5014
		[Dependency]
		private readonly IPrototypeManager _prototype;
	}
}
