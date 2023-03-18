using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Emag.Systems
{
	// Token: 0x020004C6 RID: 1222
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmagSystem : EntitySystem
	{
		// Token: 0x06000EBD RID: 3773 RVA: 0x0002F608 File Offset: 0x0002D808
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmagComponent, ExaminedEvent>(new ComponentEventHandler<EmagComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<EmagComponent, AfterInteractEvent>(new ComponentEventHandler<EmagComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<EmagComponent, ComponentGetState>(new ComponentEventRefHandler<EmagComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<EmagComponent, ComponentHandleState>(new ComponentEventRefHandler<EmagComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<EmagComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<EmagComponent, EntityUnpausedEvent>(this.OnUnpaused), null, null);
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0002F67F File Offset: 0x0002D87F
		private void OnGetState(EntityUid uid, EmagComponent component, ref ComponentGetState args)
		{
			args.State = new EmagComponentState(component.MaxCharges, component.Charges, component.RechargeDuration, component.NextChargeTime, component.EmagImmuneTag, component.AutoRecharge);
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0002F6B0 File Offset: 0x0002D8B0
		private void OnHandleState(EntityUid uid, EmagComponent component, ref ComponentHandleState args)
		{
			EmagComponentState state = args.Current as EmagComponentState;
			if (state == null)
			{
				return;
			}
			component.MaxCharges = state.MaxCharges;
			component.Charges = state.Charges;
			component.RechargeDuration = state.RechargeTime;
			component.NextChargeTime = state.NextChargeTime;
			component.EmagImmuneTag = state.EmagImmuneTag;
			component.AutoRecharge = state.AutoRecharge;
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x0002F715 File Offset: 0x0002D915
		private void OnUnpaused(EntityUid uid, EmagComponent component, ref EntityUnpausedEvent args)
		{
			component.NextChargeTime += args.PausedTime;
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0002F730 File Offset: 0x0002D930
		private void OnExamine(EntityUid uid, EmagComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("emag-charges-remaining", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("charges", component.Charges)
			}));
			if (component.Charges == component.MaxCharges)
			{
				args.PushMarkup(Loc.GetString("emag-max-charges"));
				return;
			}
			double timeRemaining = Math.Round((component.NextChargeTime - this._timing.CurTime).TotalSeconds);
			args.PushMarkup(Loc.GetString("emag-recharging", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seconds", timeRemaining)
			}));
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0002F7E0 File Offset: 0x0002D9E0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (EmagComponent emag in base.EntityQuery<EmagComponent>(false))
			{
				if (emag.AutoRecharge && emag.Charges != emag.MaxCharges && !(this._timing.CurTime < emag.NextChargeTime))
				{
					this.ChangeEmagCharge(emag.Owner, 1, true, emag);
				}
			}
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0002F86C File Offset: 0x0002DA6C
		private void OnAfterInteract(EntityUid uid, EmagComponent component, AfterInteractEvent args)
		{
			if (args.CanReach)
			{
				EntityUid? target2 = args.Target;
				if (target2 != null)
				{
					EntityUid target = target2.GetValueOrDefault();
					args.Handled = this.TryUseEmag(uid, args.User, target, component);
					return;
				}
			}
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0002F8B4 File Offset: 0x0002DAB4
		[NullableContext(2)]
		public bool ChangeEmagCharge(EntityUid uid, int change, bool resetTimer, EmagComponent component = null)
		{
			if (!base.Resolve<EmagComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.Charges + change < 0 || component.Charges + change > component.MaxCharges)
			{
				return false;
			}
			if (resetTimer || component.Charges == component.MaxCharges)
			{
				component.NextChargeTime = this._timing.CurTime + component.RechargeDuration;
			}
			component.Charges += change;
			base.Dirty(component, null);
			return true;
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0002F93C File Offset: 0x0002DB3C
		[NullableContext(2)]
		public bool TryUseEmag(EntityUid emag, EntityUid user, EntityUid target, EmagComponent component = null)
		{
			if (!base.Resolve<EmagComponent>(emag, ref component, false))
			{
				return false;
			}
			if (this._tagSystem.HasTag(target, component.EmagImmuneTag))
			{
				return false;
			}
			if (component.Charges <= 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("emag-no-charges"), user, user, PopupType.Small);
				return false;
			}
			if (!this.DoEmagEffect(user, target))
			{
				return false;
			}
			if (this._net.IsClient && this._timing.IsFirstTimePredicted)
			{
				this._popupSystem.PopupEntity(Loc.GetString("emag-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
				}), user, user, PopupType.Medium);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Emag;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(9, 2);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "player", "ToPrettyString(user)");
			logStringHandler.AppendLiteral(" emagged ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.ChangeEmagCharge(emag, -1, false, component);
			return true;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0002FA5C File Offset: 0x0002DC5C
		public bool DoEmagEffect(EntityUid user, EntityUid target)
		{
			if (base.HasComp<EmaggedComponent>(target))
			{
				return false;
			}
			GotEmaggedEvent emaggedEvent = new GotEmaggedEvent(user, false, false);
			base.RaiseLocalEvent<GotEmaggedEvent>(target, ref emaggedEvent, false);
			if (!emaggedEvent.Repeatable)
			{
				base.EnsureComp<EmaggedComponent>(target);
			}
			return emaggedEvent.Handled;
		}

		// Token: 0x04000DDD RID: 3549
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000DDE RID: 3550
		[Dependency]
		private readonly INetManager _net;

		// Token: 0x04000DDF RID: 3551
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000DE0 RID: 3552
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000DE1 RID: 3553
		[Dependency]
		private readonly TagSystem _tagSystem;
	}
}
