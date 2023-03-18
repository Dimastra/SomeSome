using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Damage.Systems
{
	// Token: 0x020005C5 RID: 1477
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageUserOnTriggerSystem : EntitySystem
	{
		// Token: 0x06001F83 RID: 8067 RVA: 0x000A57A2 File Offset: 0x000A39A2
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DamageUserOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<DamageUserOnTriggerComponent, TriggerEvent>(this.OnTrigger), null, null);
		}

		// Token: 0x06001F84 RID: 8068 RVA: 0x000A57B8 File Offset: 0x000A39B8
		private void OnTrigger(EntityUid uid, DamageUserOnTriggerComponent component, TriggerEvent args)
		{
			if (args.User == null)
			{
				return;
			}
			args.Handled |= this.OnDamageTrigger(uid, args.User.Value, component);
		}

		// Token: 0x06001F85 RID: 8069 RVA: 0x000A57FC File Offset: 0x000A39FC
		[NullableContext(2)]
		private bool OnDamageTrigger(EntityUid source, EntityUid target, DamageUserOnTriggerComponent component = null)
		{
			if (!base.Resolve<DamageUserOnTriggerComponent>(source, ref component, true))
			{
				return false;
			}
			BeforeDamageUserOnTriggerEvent ev = new BeforeDamageUserOnTriggerEvent(new DamageSpecifier(component.Damage), target);
			base.RaiseLocalEvent<BeforeDamageUserOnTriggerEvent>(source, ev, false);
			return this._damageableSystem.TryChangeDamage(new EntityUid?(target), ev.Damage, component.IgnoreResistances, true, null, new EntityUid?(source)) != null;
		}

		// Token: 0x04001393 RID: 5011
		[Dependency]
		private readonly DamageableSystem _damageableSystem;
	}
}
