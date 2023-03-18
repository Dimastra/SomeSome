using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Guardian;
using Content.Server.Popups;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.IdentityManagement;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Implants
{
	// Token: 0x02000451 RID: 1105
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ImplanterSystem : SharedImplanterSystem
	{
		// Token: 0x06001646 RID: 5702 RVA: 0x0007581B File Offset: 0x00073A1B
		public void InitializeImplanted()
		{
			base.SubscribeLocalEvent<ImplantedComponent, ComponentInit>(new ComponentEventHandler<ImplantedComponent, ComponentInit>(this.OnImplantedInit), null, null);
			base.SubscribeLocalEvent<ImplantedComponent, ComponentShutdown>(new ComponentEventHandler<ImplantedComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x00075845 File Offset: 0x00073A45
		private void OnImplantedInit(EntityUid uid, ImplantedComponent component, ComponentInit args)
		{
			component.ImplantContainer = this._container.EnsureContainer<Container>(uid, "implant", null);
			component.ImplantContainer.OccludesLight = false;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0007586B File Offset: 0x00073A6B
		private void OnShutdown(EntityUid uid, ImplantedComponent component, ComponentShutdown args)
		{
			this._container.CleanContainer(component.ImplantContainer);
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x00075880 File Offset: 0x00073A80
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeImplanted();
			base.SubscribeLocalEvent<ImplanterComponent, HandDeselectedEvent>(new ComponentEventHandler<ImplanterComponent, HandDeselectedEvent>(this.OnHandDeselect), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, AfterInteractEvent>(new ComponentEventHandler<ImplanterComponent, AfterInteractEvent>(this.OnImplanterAfterInteract), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, ComponentGetState>(new ComponentEventRefHandler<ImplanterComponent, ComponentGetState>(this.OnImplanterGetState), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, DoAfterEvent<ImplanterSystem.ImplantEvent>>(new ComponentEventHandler<ImplanterComponent, DoAfterEvent<ImplanterSystem.ImplantEvent>>(this.OnImplant), null, null);
			base.SubscribeLocalEvent<ImplanterComponent, DoAfterEvent<ImplanterSystem.DrawEvent>>(new ComponentEventHandler<ImplanterComponent, DoAfterEvent<ImplanterSystem.DrawEvent>>(this.OnDraw), null, null);
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x00075900 File Offset: 0x00073B00
		private void OnImplanterAfterInteract(EntityUid uid, ImplanterComponent component, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach || args.Handled)
			{
				return;
			}
			if (!base.HasComp<MobStateComponent>(args.Target.Value) || base.HasComp<GuardianComponent>(args.Target.Value))
			{
				return;
			}
			if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly)
			{
				this.TryDraw(component, args.User, args.Target.Value, uid);
			}
			else if (args.User == args.Target)
			{
				base.Implant(uid, args.Target.Value, component);
			}
			else
			{
				this.TryImplant(component, args.User, args.Target.Value, uid);
			}
			args.Handled = true;
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x000759EE File Offset: 0x00073BEE
		private void OnHandDeselect(EntityUid uid, ImplanterComponent component, HandDeselectedEvent args)
		{
			CancellationTokenSource cancelToken = component.CancelToken;
			if (cancelToken != null)
			{
				cancelToken.Cancel();
			}
			component.CancelToken = null;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x00075A08 File Offset: 0x00073C08
		public void TryImplant(ImplanterComponent component, EntityUid user, EntityUid target, EntityUid implanter)
		{
			if (component.CancelToken != null)
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("injector-component-injecting-user"), target, user, PopupType.Small);
			EntityUid userName = Identity.Entity(user, this.EntityManager);
			this._popup.PopupEntity(Loc.GetString("implanter-component-implanting-target", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("user", userName)
			}), user, target, PopupType.LargeCaution);
			CancellationTokenSource cancelToken = component.CancelToken;
			if (cancelToken != null)
			{
				cancelToken.Cancel();
			}
			component.CancelToken = new CancellationTokenSource();
			ImplanterSystem.ImplantEvent implantEvent = new ImplanterSystem.ImplantEvent();
			this._doAfter.DoAfter<ImplanterSystem.ImplantEvent>(new DoAfterEventArgs(user, component.ImplantTime, component.CancelToken.Token, new EntityUid?(target), new EntityUid?(implanter))
			{
				BreakOnUserMove = true,
				BreakOnTargetMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = true
			}, implantEvent);
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x00075AF0 File Offset: 0x00073CF0
		public void TryDraw(ImplanterComponent component, EntityUid user, EntityUid target, EntityUid implanter)
		{
			this._popup.PopupEntity(Loc.GetString("injector-component-injecting-user"), target, user, PopupType.Small);
			CancellationTokenSource cancelToken = component.CancelToken;
			if (cancelToken != null)
			{
				cancelToken.Cancel();
			}
			component.CancelToken = new CancellationTokenSource();
			ImplanterSystem.DrawEvent drawEvent = new ImplanterSystem.DrawEvent();
			SharedDoAfterSystem doAfter = this._doAfter;
			float drawTime = component.DrawTime;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(implanter);
			doAfter.DoAfter<ImplanterSystem.DrawEvent>(new DoAfterEventArgs(user, drawTime, default(CancellationToken), target2, used)
			{
				BreakOnUserMove = true,
				BreakOnTargetMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				NeedHand = true
			}, drawEvent);
		}

		// Token: 0x0600164E RID: 5710 RVA: 0x00075B8E File Offset: 0x00073D8E
		private void OnImplanterGetState(EntityUid uid, ImplanterComponent component, ref ComponentGetState args)
		{
			args.State = new ImplanterComponentState(component.CurrentMode, component.ImplantOnly);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x00075BA8 File Offset: 0x00073DA8
		private void OnImplant(EntityUid uid, ImplanterComponent component, DoAfterEvent<ImplanterSystem.ImplantEvent> args)
		{
			if (args.Cancelled)
			{
				component.CancelToken = null;
				return;
			}
			if (args.Handled || args.Args.Target == null || args.Args.Used == null)
			{
				return;
			}
			base.Implant(args.Args.Used.Value, args.Args.Target.Value, component);
			args.Handled = true;
			component.CancelToken = null;
		}

		// Token: 0x06001650 RID: 5712 RVA: 0x00075C28 File Offset: 0x00073E28
		private void OnDraw(EntityUid uid, ImplanterComponent component, DoAfterEvent<ImplanterSystem.DrawEvent> args)
		{
			if (args.Cancelled)
			{
				component.CancelToken = null;
				return;
			}
			if (args.Handled || args.Args.Used == null || args.Args.Target == null)
			{
				return;
			}
			base.Draw(args.Args.Used.Value, args.Args.User, args.Args.Target.Value, component);
			args.Handled = true;
			component.CancelToken = null;
		}

		// Token: 0x04000DF2 RID: 3570
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04000DF3 RID: 3571
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000DF4 RID: 3572
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x020009CE RID: 2510
		[NullableContext(0)]
		private sealed class ImplantEvent : EntityEventArgs
		{
		}

		// Token: 0x020009CF RID: 2511
		[NullableContext(0)]
		private sealed class DrawEvent : EntityEventArgs
		{
		}
	}
}
