using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.DoAfter;
using Content.Server.Medical.Components;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Server.Medical
{
	// Token: 0x020003B2 RID: 946
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StethoscopeSystem : EntitySystem
	{
		// Token: 0x06001383 RID: 4995 RVA: 0x00064CE0 File Offset: 0x00062EE0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StethoscopeComponent, GotEquippedEvent>(new ComponentEventHandler<StethoscopeComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<StethoscopeComponent, GotUnequippedEvent>(new ComponentEventHandler<StethoscopeComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<WearingStethoscopeComponent, GetVerbsEvent<InnateVerb>>(new ComponentEventHandler<WearingStethoscopeComponent, GetVerbsEvent<InnateVerb>>(this.AddStethoscopeVerb), null, null);
			base.SubscribeLocalEvent<StethoscopeComponent, GetItemActionsEvent>(new ComponentEventHandler<StethoscopeComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<StethoscopeComponent, StethoscopeActionEvent>(new ComponentEventHandler<StethoscopeComponent, StethoscopeActionEvent>(this.OnStethoscopeAction), null, null);
			base.SubscribeLocalEvent<StethoscopeComponent, DoAfterEvent>(new ComponentEventHandler<StethoscopeComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x00064D6C File Offset: 0x00062F6C
		private void OnEquipped(EntityUid uid, StethoscopeComponent component, GotEquippedEvent args)
		{
			ClothingComponent clothing;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing))
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			component.IsActive = true;
			base.EnsureComp<WearingStethoscopeComponent>(args.Equipee).Stethoscope = uid;
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x00064DBC File Offset: 0x00062FBC
		private void OnUnequipped(EntityUid uid, StethoscopeComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			base.RemComp<WearingStethoscopeComponent>(args.Equipee);
			component.IsActive = false;
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x00064DDC File Offset: 0x00062FDC
		private void AddStethoscopeVerb(EntityUid uid, WearingStethoscopeComponent component, GetVerbsEvent<InnateVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			if (!base.HasComp<MobStateComponent>(args.Target))
			{
				return;
			}
			if (component.CancelToken != null)
			{
				return;
			}
			StethoscopeComponent stetho;
			if (!base.TryComp<StethoscopeComponent>(component.Stethoscope, ref stetho))
			{
				return;
			}
			InnateVerb verb = new InnateVerb
			{
				Act = delegate()
				{
					this.StartListening(component.Stethoscope, uid, args.Target, stetho);
				},
				Text = Loc.GetString("stethoscope-verb"),
				Icon = new SpriteSpecifier.Rsi(new ResourcePath("Clothing/Neck/Misc/stethoscope.rsi", "/"), "icon"),
				Priority = 2
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x00064EC2 File Offset: 0x000630C2
		private void OnStethoscopeAction(EntityUid uid, StethoscopeComponent component, StethoscopeActionEvent args)
		{
			this.StartListening(uid, args.Performer, args.Target, component);
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x00064ED8 File Offset: 0x000630D8
		private void OnGetActions(EntityUid uid, StethoscopeComponent component, GetItemActionsEvent args)
		{
			args.Actions.Add(component.Action);
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x00064EEC File Offset: 0x000630EC
		private void StartListening(EntityUid scope, EntityUid user, EntityUid target, StethoscopeComponent comp)
		{
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			float delay = comp.Delay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(scope);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x00064F4C File Offset: 0x0006314C
		private void OnDoAfter(EntityUid uid, StethoscopeComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null)
			{
				return;
			}
			this.ExamineWithStethoscope(args.Args.User, args.Args.Target.Value);
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x00064FA0 File Offset: 0x000631A0
		public void ExamineWithStethoscope(EntityUid user, EntityUid target)
		{
			MobStateComponent mobState;
			if (!base.HasComp<RespiratorComponent>(target) || !base.TryComp<MobStateComponent>(target, ref mobState) || this._mobStateSystem.IsDead(target, mobState))
			{
				this._popupSystem.PopupEntity(Loc.GetString("stethoscope-dead"), target, user, PopupType.Small);
				return;
			}
			DamageableComponent damage;
			if (!base.TryComp<DamageableComponent>(target, ref damage))
			{
				return;
			}
			FixedPoint2 value;
			if (!damage.Damage.DamageDict.TryGetValue("Asphyxiation", out value))
			{
				return;
			}
			string message = this.GetDamageMessage(value);
			this._popupSystem.PopupEntity(Loc.GetString(message), target, user, PopupType.Small);
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x0006502C File Offset: 0x0006322C
		private string GetDamageMessage(FixedPoint2 totalOxyloss)
		{
			int num = (int)totalOxyloss;
			string result;
			if (num < 60)
			{
				if (num >= 20)
				{
					result = "stethoscope-hyper";
				}
				else
				{
					result = "stethoscope-normal";
				}
			}
			else if (num >= 80)
			{
				result = "stethoscope-fucked";
			}
			else
			{
				result = "stethoscope-irregular";
			}
			return result;
		}

		// Token: 0x04000BDE RID: 3038
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000BDF RID: 3039
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000BE0 RID: 3040
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;
	}
}
