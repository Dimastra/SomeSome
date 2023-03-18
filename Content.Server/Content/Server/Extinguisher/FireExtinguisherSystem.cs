using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Audio;
using Content.Shared.Chemistry.Components;
using Content.Shared.Extinguisher;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Extinguisher
{
	// Token: 0x02000507 RID: 1287
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FireExtinguisherSystem : EntitySystem
	{
		// Token: 0x06001A8A RID: 6794 RVA: 0x0008BDC8 File Offset: 0x00089FC8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FireExtinguisherComponent, ComponentInit>(new ComponentEventHandler<FireExtinguisherComponent, ComponentInit>(this.OnFireExtinguisherInit), null, null);
			base.SubscribeLocalEvent<FireExtinguisherComponent, UseInHandEvent>(new ComponentEventHandler<FireExtinguisherComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<FireExtinguisherComponent, AfterInteractEvent>(new ComponentEventHandler<FireExtinguisherComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<FireExtinguisherComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<FireExtinguisherComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetInteractionVerbs), null, null);
			base.SubscribeLocalEvent<FireExtinguisherComponent, SprayAttemptEvent>(new ComponentEventHandler<FireExtinguisherComponent, SprayAttemptEvent>(this.OnSprayAttempt), null, null);
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x0008BE3F File Offset: 0x0008A03F
		private void OnFireExtinguisherInit(EntityUid uid, FireExtinguisherComponent component, ComponentInit args)
		{
			if (component.HasSafety)
			{
				this.UpdateAppearance(uid, component, null);
			}
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x0008BE52 File Offset: 0x0008A052
		private void OnUseInHand(EntityUid uid, FireExtinguisherComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.ToggleSafety(uid, args.User, component);
			args.Handled = true;
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x0008BE74 File Offset: 0x0008A074
		private void OnAfterInteract(EntityUid uid, FireExtinguisherComponent component, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach)
			{
				return;
			}
			if (args.Handled)
			{
				return;
			}
			if (component.HasSafety && component.Safety)
			{
				this._popupSystem.PopupEntity(Loc.GetString("fire-extinguisher-component-safety-on-message"), uid, args.User, PopupType.Small);
				return;
			}
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				Solution targetSolution;
				Solution container;
				if (target.Valid && this._solutionContainerSystem.TryGetDrainableSolution(target, out targetSolution, null, null) && this._solutionContainerSystem.TryGetRefillableSolution(uid, out container, null, null))
				{
					args.Handled = true;
					FixedPoint2 transfer = container.AvailableVolume;
					SolutionTransferComponent solTrans;
					if (base.TryComp<SolutionTransferComponent>(uid, ref solTrans))
					{
						transfer = solTrans.TransferAmount;
					}
					transfer = FixedPoint2.Min(transfer, targetSolution.Volume);
					if (transfer > 0)
					{
						Solution drained = this._solutionContainerSystem.Drain(target, targetSolution, transfer, null);
						this._solutionContainerSystem.TryAddSolution(uid, container, drained);
						SoundSystem.Play(component.RefillSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, null);
						this._popupSystem.PopupEntity(Loc.GetString("fire-extinguisher-component-after-interact-refilled-message", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("owner", uid)
						}), uid, args.Target.Value, PopupType.Small);
					}
					return;
				}
			}
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x0008BFE4 File Offset: 0x0008A1E4
		private void OnGetInteractionVerbs(EntityUid uid, FireExtinguisherComponent component, GetVerbsEvent<InteractionVerb> args)
		{
			if (!args.CanInteract)
			{
				return;
			}
			InteractionVerb verb = new InteractionVerb
			{
				Act = delegate()
				{
					this.ToggleSafety(uid, args.User, component);
				},
				Text = Loc.GetString("fire-extinguisher-component-verb-text")
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x0008C05B File Offset: 0x0008A25B
		private void OnSprayAttempt(EntityUid uid, FireExtinguisherComponent component, SprayAttemptEvent args)
		{
			if (component.HasSafety && component.Safety)
			{
				this._popupSystem.PopupEntity(Loc.GetString("fire-extinguisher-component-safety-on-message"), uid, args.User, PopupType.Small);
				args.Cancel();
			}
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0008C090 File Offset: 0x0008A290
		private void UpdateAppearance(EntityUid uid, FireExtinguisherComponent comp, [Nullable(2)] AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				return;
			}
			if (comp.HasSafety)
			{
				this._appearance.SetData(uid, FireExtinguisherVisuals.Safety, comp.Safety, appearance);
			}
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x0008C0C8 File Offset: 0x0008A2C8
		[NullableContext(2)]
		public void ToggleSafety(EntityUid uid, EntityUid user, FireExtinguisherComponent extinguisher = null)
		{
			if (!base.Resolve<FireExtinguisherComponent>(uid, ref extinguisher, true))
			{
				return;
			}
			extinguisher.Safety = !extinguisher.Safety;
			SoundSystem.Play(extinguisher.SafetySound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.125f).WithVolume(-4f)));
			this.UpdateAppearance(uid, extinguisher, null);
		}

		// Token: 0x040010E5 RID: 4325
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040010E6 RID: 4326
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040010E7 RID: 4327
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
