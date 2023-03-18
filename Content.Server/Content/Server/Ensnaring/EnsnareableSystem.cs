using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Content.Shared.Ensnaring;
using Content.Shared.Ensnaring.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Throwing;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Ensnaring
{
	// Token: 0x0200052A RID: 1322
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EnsnareableSystem : SharedEnsnareableSystem
	{
		// Token: 0x06001B84 RID: 7044 RVA: 0x000934AC File Offset: 0x000916AC
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeEnsnaring();
			base.SubscribeLocalEvent<EnsnareableComponent, ComponentInit>(new ComponentEventHandler<EnsnareableComponent, ComponentInit>(this.OnEnsnareableInit), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, DoAfterEvent>(new ComponentEventHandler<EnsnareableComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x000934E2 File Offset: 0x000916E2
		private void OnEnsnareableInit(EntityUid uid, EnsnareableComponent component, ComponentInit args)
		{
			component.Container = this._container.EnsureContainer<Container>(uid, "ensnare", null);
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x000934FC File Offset: 0x000916FC
		private void OnDoAfter(EntityUid uid, EnsnareableComponent component, DoAfterEvent args)
		{
			EnsnaringComponent ensnaring;
			if (args.Handled || !base.TryComp<EnsnaringComponent>(args.Args.Used, ref ensnaring))
			{
				return;
			}
			if (args.Cancelled)
			{
				this._popup.PopupEntity(Loc.GetString("ensnare-component-try-free-fail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ensnare", args.Args.Used)
				}), uid, uid, PopupType.Large);
				return;
			}
			component.Container.Remove(args.Args.Used.Value, null, null, null, true, false, null, null);
			component.IsEnsnared = false;
			base.Dirty(component, null);
			ensnaring.Ensnared = null;
			this._popup.PopupEntity(Loc.GetString("ensnare-component-try-free-complete", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("ensnare", args.Args.Used)
			}), uid, uid, PopupType.Large);
			this.UpdateAlert(args.Args.Used.Value, component);
			EnsnareRemoveEvent ev = new EnsnareRemoveEvent();
			base.RaiseLocalEvent<EnsnareRemoveEvent>(uid, ev, false);
			args.Handled = true;
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x0009362C File Offset: 0x0009182C
		public void InitializeEnsnaring()
		{
			base.SubscribeLocalEvent<EnsnaringComponent, ComponentRemove>(new ComponentEventHandler<EnsnaringComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<EnsnaringComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<EnsnaringComponent, StepTriggerAttemptEvent>(this.AttemptStepTrigger), null, null);
			base.SubscribeLocalEvent<EnsnaringComponent, StepTriggeredEvent>(new ComponentEventRefHandler<EnsnaringComponent, StepTriggeredEvent>(this.OnStepTrigger), null, null);
			base.SubscribeLocalEvent<EnsnaringComponent, ThrowDoHitEvent>(new ComponentEventHandler<EnsnaringComponent, ThrowDoHitEvent>(this.OnThrowHit), null, null);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x0009368C File Offset: 0x0009188C
		private void OnComponentRemove(EntityUid uid, EnsnaringComponent component, ComponentRemove args)
		{
			EnsnareableComponent ensnared;
			if (!base.TryComp<EnsnareableComponent>(component.Ensnared, ref ensnared))
			{
				return;
			}
			if (ensnared.IsEnsnared)
			{
				this.ForceFree(uid, component);
			}
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x000936BA File Offset: 0x000918BA
		private void AttemptStepTrigger(EntityUid uid, EnsnaringComponent component, ref StepTriggerAttemptEvent args)
		{
			args.Continue = true;
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x000936C3 File Offset: 0x000918C3
		private void OnStepTrigger(EntityUid uid, EnsnaringComponent component, ref StepTriggeredEvent args)
		{
			this.TryEnsnare(args.Tripper, uid, component);
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x000936D3 File Offset: 0x000918D3
		private void OnThrowHit(EntityUid uid, EnsnaringComponent component, ThrowDoHitEvent args)
		{
			if (!component.CanThrowTrigger)
			{
				return;
			}
			this.TryEnsnare(args.Target, uid, component);
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x000936EC File Offset: 0x000918EC
		public void TryEnsnare(EntityUid target, EntityUid ensnare, EnsnaringComponent component)
		{
			EnsnareableComponent ensnareable;
			if (!base.TryComp<EnsnareableComponent>(target, ref ensnareable))
			{
				return;
			}
			component.Ensnared = new EntityUid?(target);
			ensnareable.Container.Insert(ensnare, null, null, null, null, null);
			ensnareable.IsEnsnared = true;
			base.Dirty(ensnareable, null);
			this.UpdateAlert(ensnare, ensnareable);
			EnsnareEvent ev = new EnsnareEvent(component.WalkSpeed, component.SprintSpeed);
			base.RaiseLocalEvent<EnsnareEvent>(target, ev, false);
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00093758 File Offset: 0x00091958
		public void TryFree(EntityUid target, EntityUid ensnare, EnsnaringComponent component, EntityUid? user = null)
		{
			if (!base.HasComp<EnsnareableComponent>(target))
			{
				return;
			}
			EntityUid? entityUid;
			bool flag;
			if (user != null)
			{
				entityUid = user;
				flag = !(target != entityUid);
			}
			else
			{
				flag = true;
			}
			bool isOwner = flag;
			float freeTime = isOwner ? component.BreakoutTime : component.FreeTime;
			bool breakOnMove = !isOwner || !component.CanMoveBreakout;
			float delay = freeTime;
			entityUid = new EntityUid?(target);
			EntityUid? used = new EntityUid?(ensnare);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(target, delay, default(CancellationToken), entityUid, used)
			{
				BreakOnUserMove = breakOnMove,
				BreakOnTargetMove = breakOnMove,
				BreakOnDamage = false,
				BreakOnStun = true,
				NeedHand = true
			};
			this._doAfter.DoAfter(doAfterEventArgs);
			if (isOwner)
			{
				this._popup.PopupEntity(Loc.GetString("ensnare-component-try-free", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ensnare", ensnare)
				}), target, target, PopupType.Small);
			}
			if (!isOwner && user != null)
			{
				this._popup.PopupEntity(Loc.GetString("ensnare-component-try-free-other", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ensnare", ensnare),
					new ValueTuple<string, object>("user", Identity.Entity(target, this.EntityManager))
				}), user.Value, user.Value, PopupType.Small);
			}
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x000938C0 File Offset: 0x00091AC0
		public void ForceFree(EntityUid ensnare, EnsnaringComponent component)
		{
			EnsnareableComponent ensnareable;
			if (!base.TryComp<EnsnareableComponent>(component.Ensnared, ref ensnareable))
			{
				return;
			}
			ensnareable.Container.ForceRemove(ensnare, null, null);
			ensnareable.IsEnsnared = false;
			base.Dirty(ensnareable, null);
			component.Ensnared = null;
			this.UpdateAlert(ensnare, ensnareable);
			EnsnareRemoveEvent ev = new EnsnareRemoveEvent();
			base.RaiseLocalEvent<EnsnareRemoveEvent>(ensnare, ev, false);
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x00093920 File Offset: 0x00091B20
		public void UpdateAlert(EntityUid ensnare, EnsnareableComponent component)
		{
			if (!component.IsEnsnared)
			{
				this._alerts.ClearAlert(ensnare, AlertType.Ensnared);
				return;
			}
			this._alerts.ShowAlert(ensnare, AlertType.Ensnared, null, null);
		}

		// Token: 0x040011A7 RID: 4519
		[Dependency]
		private readonly ContainerSystem _container;

		// Token: 0x040011A8 RID: 4520
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x040011A9 RID: 4521
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x040011AA RID: 4522
		[Dependency]
		private readonly AlertsSystem _alerts;
	}
}
