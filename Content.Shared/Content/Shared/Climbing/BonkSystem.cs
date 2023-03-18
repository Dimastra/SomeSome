using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.DragDrop;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Shared.Climbing
{
	// Token: 0x020005C3 RID: 1475
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BonkSystem : EntitySystem
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x0003A985 File Offset: 0x00038B85
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BonkableComponent, DragDropTargetEvent>(new ComponentEventRefHandler<BonkableComponent, DragDropTargetEvent>(this.OnDragDrop), null, null);
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x0003A9A4 File Offset: 0x00038BA4
		[NullableContext(2)]
		public bool TryBonk(EntityUid user, EntityUid bonkableUid, BonkableComponent bonkableComponent = null)
		{
			if (!base.Resolve<BonkableComponent>(bonkableUid, ref bonkableComponent, false))
			{
				return false;
			}
			if (!this._cfg.GetCVar<bool>(CCVars.GameTableBonk) && !this._interactionSystem.TryRollClumsy(user, bonkableComponent.BonkClumsyChance, null))
			{
				return false;
			}
			EntityUid userName = Identity.Entity(user, this.EntityManager);
			EntityUid bonkableName = Identity.Entity(bonkableUid, this.EntityManager);
			this._popupSystem.PopupEntity(Loc.GetString("bonkable-success-message-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", userName),
				new ValueTuple<string, object>("bonkable", bonkableName)
			}), user, Filter.PvsExcept(user, 2f, null), true, PopupType.Small);
			this._popupSystem.PopupEntity(Loc.GetString("bonkable-success-message-user", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", userName),
				new ValueTuple<string, object>("bonkable", bonkableName)
			}), user, user, PopupType.Small);
			this._audioSystem.PlayPvs(bonkableComponent.BonkSound, bonkableUid, null);
			this._stunSystem.TryParalyze(user, TimeSpan.FromSeconds((double)bonkableComponent.BonkTime), true, null);
			DamageSpecifier bonkDmg = bonkableComponent.BonkDamage;
			if (bonkDmg != null)
			{
				this._damageableSystem.TryChangeDamage(new EntityUid?(user), bonkDmg, true, true, null, new EntityUid?(user));
			}
			return true;
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x0003AB04 File Offset: 0x00038D04
		private void OnDragDrop(EntityUid uid, BonkableComponent bonkableComponent, ref DragDropTargetEvent args)
		{
			this.TryBonk(args.Dragged, uid, null);
		}

		// Token: 0x040010AA RID: 4266
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040010AB RID: 4267
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040010AC RID: 4268
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x040010AD RID: 4269
		[Dependency]
		private readonly SharedStunSystem _stunSystem;

		// Token: 0x040010AE RID: 4270
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x040010AF RID: 4271
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;
	}
}
