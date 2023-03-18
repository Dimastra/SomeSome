using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Borgs;
using Content.Server.DoAfter;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.White.Borgs
{
	// Token: 0x0200009C RID: 156
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BorgRepairSystem : EntitySystem
	{
		// Token: 0x06000278 RID: 632 RVA: 0x0000D556 File Offset: 0x0000B756
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BorgRepairComponent, InteractUsingEvent>(new ComponentEventHandler<BorgRepairComponent, InteractUsingEvent>(this.Repair), null, null);
			base.SubscribeLocalEvent<BorgRepairComponent, DoAfterEvent>(new ComponentEventHandler<BorgRepairComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000D588 File Offset: 0x0000B788
		private void Repair(EntityUid uid, BorgRepairComponent component, InteractUsingEvent args)
		{
			float delay = 7f;
			if (!base.HasComp<BorgComponent>(args.Target))
			{
				return;
			}
			if (args.User == args.Target)
			{
				delay *= component.SelfRepairPenalty;
			}
			DamageableComponent damageable;
			if (!this.EntityManager.TryGetComponent<DamageableComponent>(component.Owner, ref damageable) || damageable.TotalDamage == 0)
			{
				return;
			}
			DoAfterEventArgs doAfterArgs = new DoAfterEventArgs(args.User, delay, default(CancellationToken), new EntityUid?(args.Target), null)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			};
			if (!this._toolSystem.HasQuality(args.Used, component.QualityNeeded, null))
			{
				return;
			}
			this._doAfterSystem.DoAfter(doAfterArgs);
			this._toolSystem.PlayToolSound(args.Used, null);
			component.Owner.PopupMessage(args.User, Loc.GetString("comp-repairable-repair", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", component.Owner),
				new ValueTuple<string, object>("tool", args.Used)
			}));
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000D6C4 File Offset: 0x0000B8C4
		private void OnDoAfter(EntityUid uid, BorgRepairComponent component, DoAfterEvent args)
		{
			if (args.Cancelled || args.Handled || args.Args.Target == null)
			{
				return;
			}
			this._damageableSystem.TryChangeDamage(new EntityUid?(uid), new DamageSpecifier(component.Damage), true, true, null, null);
		}

		// Token: 0x040001CC RID: 460
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x040001CD RID: 461
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040001CE RID: 462
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;
	}
}
