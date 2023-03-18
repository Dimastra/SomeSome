using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Borgs;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Chemistry.ReactionEffects;
using Content.Server.Fluids.EntitySystems;
using Content.Server.HealthExaminable;
using Content.Server.Popups;
using Content.Shared.Alert;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Body.Systems
{
	// Token: 0x02000705 RID: 1797
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BloodstreamSystem : EntitySystem
	{
		// Token: 0x060025C3 RID: 9667 RVA: 0x000C6BA4 File Offset: 0x000C4DA4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BloodstreamComponent, ComponentInit>(new ComponentEventHandler<BloodstreamComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, DamageChangedEvent>(new ComponentEventHandler<BloodstreamComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, HealthBeingExaminedEvent>(new ComponentEventHandler<BloodstreamComponent, HealthBeingExaminedEvent>(this.OnHealthBeingExamined), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, BeingGibbedEvent>(new ComponentEventHandler<BloodstreamComponent, BeingGibbedEvent>(this.OnBeingGibbed), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<BloodstreamComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, ReactionAttemptEvent>(new ComponentEventHandler<BloodstreamComponent, ReactionAttemptEvent>(this.OnReactionAttempt), null, null);
			base.SubscribeLocalEvent<BloodstreamComponent, RejuvenateEvent>(new ComponentEventHandler<BloodstreamComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x060025C4 RID: 9668 RVA: 0x000C6C44 File Offset: 0x000C4E44
		private void OnReactionAttempt(EntityUid uid, BloodstreamComponent component, ReactionAttemptEvent args)
		{
			if (args.Solution.Name != BloodstreamComponent.DefaultBloodSolutionName && args.Solution.Name != BloodstreamComponent.DefaultChemicalsSolutionName && args.Solution.Name != BloodstreamComponent.DefaultBloodTemporarySolutionName)
			{
				return;
			}
			foreach (ReagentEffect effect in args.Reaction.Effects)
			{
				if (effect is CreateEntityReactionEffect || effect is AreaReactionEffect)
				{
					args.Cancel();
					break;
				}
			}
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x000C6CF4 File Offset: 0x000C4EF4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (BloodstreamComponent bloodstream in this.EntityManager.EntityQuery<BloodstreamComponent>(false))
			{
				bloodstream.AccumulatedFrametime += frameTime;
				if (bloodstream.AccumulatedFrametime >= bloodstream.UpdateInterval)
				{
					bloodstream.AccumulatedFrametime -= bloodstream.UpdateInterval;
					EntityUid uid = bloodstream.Owner;
					MobStateComponent state;
					if (!base.TryComp<MobStateComponent>(uid, ref state) || !this._mobStateSystem.IsDead(uid, state))
					{
						if (bloodstream.IsBleeding && !base.HasComp<BorgComponent>(bloodstream.Owner))
						{
							this._alertsSystem.ShowAlert(bloodstream.Owner, AlertType.Bleeding, null, null);
						}
						else
						{
							this._alertsSystem.ClearAlert(bloodstream.Owner, AlertType.Bleeding);
						}
						if (bloodstream.BloodSolution.Volume < bloodstream.BloodSolution.MaxVolume)
						{
							this.TryModifyBloodLevel(uid, bloodstream.BloodRefreshAmount, bloodstream);
						}
						if (bloodstream.BleedAmount > 0f)
						{
							this.TryModifyBloodLevel(uid, -bloodstream.BleedAmount / 20f, bloodstream);
							this.TryModifyBleedAmount(uid, -bloodstream.BleedReductionAmount, bloodstream);
						}
						float bloodPercentage = this.GetBloodLevelPercentage(uid, bloodstream);
						if (bloodPercentage < bloodstream.BloodlossThreshold)
						{
							DamageSpecifier amt = bloodstream.BloodlossDamage / (0.1f + bloodPercentage);
							this._damageableSystem.TryChangeDamage(new EntityUid?(uid), amt, true, false, null, null);
							this._drunkSystem.TryApplyDrunkenness(uid, bloodstream.UpdateInterval * (1f + (bloodstream.BloodlossThreshold - bloodPercentage)), false, null);
						}
						else
						{
							this._damageableSystem.TryChangeDamage(new EntityUid?(uid), bloodstream.BloodlossHealDamage * bloodPercentage, true, false, null, null);
						}
					}
				}
			}
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x000C6F08 File Offset: 0x000C5108
		private void OnComponentInit(EntityUid uid, BloodstreamComponent component, ComponentInit args)
		{
			component.ChemicalSolution = this._solutionContainerSystem.EnsureSolution(uid, BloodstreamComponent.DefaultChemicalsSolutionName, null);
			component.BloodSolution = this._solutionContainerSystem.EnsureSolution(uid, BloodstreamComponent.DefaultBloodSolutionName, null);
			component.BloodTemporarySolution = this._solutionContainerSystem.EnsureSolution(uid, BloodstreamComponent.DefaultBloodTemporarySolutionName, null);
			component.ChemicalSolution.MaxVolume = component.ChemicalMaxVolume;
			component.BloodSolution.MaxVolume = component.BloodMaxVolume;
			component.BloodTemporarySolution.MaxVolume = component.BleedPuddleThreshold * 4;
			FixedPoint2 fixedPoint;
			this._solutionContainerSystem.TryAddReagent(uid, component.BloodSolution, component.BloodReagent, component.BloodMaxVolume, out fixedPoint, null);
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x000C6FC0 File Offset: 0x000C51C0
		private void OnDamageChanged(EntityUid uid, BloodstreamComponent component, DamageChangedEvent args)
		{
			if (args.DamageDelta == null)
			{
				return;
			}
			DamageModifierSetPrototype modifiers;
			if (!this._prototypeManager.TryIndex<DamageModifierSetPrototype>(component.DamageBleedModifiers, ref modifiers))
			{
				return;
			}
			DamageSpecifier bloodloss = DamageSpecifier.ApplyModifierSet(args.DamageDelta, modifiers);
			if (bloodloss.Empty)
			{
				return;
			}
			float oldBleedAmount = component.BleedAmount;
			FixedPoint2 total = bloodloss.Total;
			float totalFloat = total.Float();
			this.TryModifyBleedAmount(uid, totalFloat, component);
			float prob = Math.Clamp(totalFloat / 50f, 0f, 1f);
			float healPopupProb = Math.Clamp(Math.Abs(totalFloat) / 25f, 0f, 1f);
			if (totalFloat > 0f && RandomExtensions.Prob(this._robustRandom, prob))
			{
				this.TryModifyBloodLevel(uid, -total / 5f, component);
				SoundSystem.Play(component.InstantBloodSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioParams.Default));
				return;
			}
			if (totalFloat < 0f && oldBleedAmount > 0f && RandomExtensions.Prob(this._robustRandom, healPopupProb))
			{
				SoundSystem.Play(component.BloodHealedSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(AudioParams.Default));
				this._popupSystem.PopupEntity(Loc.GetString("bloodstream-component-wounds-cauterized"), uid, uid, PopupType.Medium);
			}
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x000C7120 File Offset: 0x000C5320
		private void OnHealthBeingExamined(EntityUid uid, BloodstreamComponent component, HealthBeingExaminedEvent args)
		{
			if (component.BleedAmount > 10f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkup(Loc.GetString("bloodstream-component-profusely-bleeding", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}));
			}
			else if (component.BleedAmount > 0f)
			{
				args.Message.PushNewline();
				args.Message.AddMarkup(Loc.GetString("bloodstream-component-bleeding", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}));
			}
			if (this.GetBloodLevelPercentage(uid, component) < component.BloodlossThreshold)
			{
				args.Message.PushNewline();
				args.Message.AddMarkup(Loc.GetString("bloodstream-component-looks-pale", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(uid, this.EntityManager))
				}));
			}
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x000C7231 File Offset: 0x000C5431
		private void OnBeingGibbed(EntityUid uid, BloodstreamComponent component, BeingGibbedEvent args)
		{
			this.SpillAllSolutions(uid, component);
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x000C723C File Offset: 0x000C543C
		private void OnApplyMetabolicMultiplier(EntityUid uid, BloodstreamComponent component, ApplyMetabolicMultiplierEvent args)
		{
			if (args.Apply)
			{
				component.UpdateInterval *= args.Multiplier;
				return;
			}
			component.UpdateInterval /= args.Multiplier;
			if (component.AccumulatedFrametime >= component.UpdateInterval)
			{
				component.AccumulatedFrametime = component.UpdateInterval;
			}
		}

		// Token: 0x060025CB RID: 9675 RVA: 0x000C7292 File Offset: 0x000C5492
		private void OnRejuvenate(EntityUid uid, BloodstreamComponent component, RejuvenateEvent args)
		{
			this.TryModifyBleedAmount(uid, -component.BleedAmount, component);
			this.TryModifyBloodLevel(uid, component.BloodSolution.AvailableVolume, component);
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x000C72B8 File Offset: 0x000C54B8
		public bool TryAddToChemicals(EntityUid uid, Solution solution, [Nullable(2)] BloodstreamComponent component = null)
		{
			return base.Resolve<BloodstreamComponent>(uid, ref component, false) && this._solutionContainerSystem.TryAddSolution(uid, component.ChemicalSolution, solution);
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x000C72DC File Offset: 0x000C54DC
		public bool FlushChemicals(EntityUid uid, string excludedReagentID, FixedPoint2 quantity, [Nullable(2)] BloodstreamComponent component = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref component, false))
			{
				return false;
			}
			for (int i = component.ChemicalSolution.Contents.Count - 1; i >= 0; i--)
			{
				string text;
				FixedPoint2 fixedPoint;
				component.ChemicalSolution.Contents[i].Deconstruct(out text, out fixedPoint);
				string reagentId = text;
				if (reagentId != excludedReagentID)
				{
					this._solutionContainerSystem.TryRemoveReagent(uid, component.ChemicalSolution, reagentId, quantity);
				}
			}
			return true;
		}

		// Token: 0x060025CE RID: 9678 RVA: 0x000C7355 File Offset: 0x000C5555
		[NullableContext(2)]
		public float GetBloodLevelPercentage(EntityUid uid, BloodstreamComponent component = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref component, true))
			{
				return 0f;
			}
			return component.BloodSolution.FillFraction;
		}

		// Token: 0x060025CF RID: 9679 RVA: 0x000C7374 File Offset: 0x000C5574
		[NullableContext(2)]
		public void SetBloodLossThreshold(EntityUid uid, float threshold, BloodstreamComponent comp = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref comp, true))
			{
				return;
			}
			comp.BloodlossThreshold = threshold;
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x000C738C File Offset: 0x000C558C
		[NullableContext(2)]
		public bool TryModifyBloodLevel(EntityUid uid, FixedPoint2 amount, BloodstreamComponent component = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref component, false))
			{
				return false;
			}
			if (amount >= 0)
			{
				FixedPoint2 fixedPoint;
				return this._solutionContainerSystem.TryAddReagent(uid, component.BloodSolution, component.BloodReagent, amount, out fixedPoint, null);
			}
			Solution newSol = component.BloodSolution.SplitSolution(-amount);
			component.BloodTemporarySolution.AddSolution(newSol, this._prototypeManager);
			if (component.BloodTemporarySolution.Volume > component.BleedPuddleThreshold)
			{
				Solution temp = component.ChemicalSolution.SplitSolution(component.BloodTemporarySolution.Volume / 10f);
				component.BloodTemporarySolution.AddSolution(temp, this._prototypeManager);
				this._spillableSystem.SpillAt(uid, component.BloodTemporarySolution, "PuddleBlood", false, true, null);
				component.BloodTemporarySolution.RemoveAllSolution();
			}
			return true;
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x000C746B File Offset: 0x000C566B
		[NullableContext(2)]
		public bool TryModifyBleedAmount(EntityUid uid, float amount, BloodstreamComponent component = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref component, false))
			{
				return false;
			}
			component.BleedAmount += amount;
			component.BleedAmount = Math.Clamp(component.BleedAmount, 0f, component.MaxBleedAmount);
			return true;
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x000C74A8 File Offset: 0x000C56A8
		[NullableContext(2)]
		public void SpillAllSolutions(EntityUid uid, BloodstreamComponent component = null)
		{
			if (!base.Resolve<BloodstreamComponent>(uid, ref component, true))
			{
				return;
			}
			FixedPoint2 max = component.BloodSolution.MaxVolume + component.BloodTemporarySolution.MaxVolume + component.ChemicalSolution.MaxVolume;
			Solution tempSol = new Solution
			{
				MaxVolume = max
			};
			tempSol.AddSolution(component.BloodSolution, this._prototypeManager);
			component.BloodSolution.RemoveAllSolution();
			tempSol.AddSolution(component.BloodTemporarySolution, this._prototypeManager);
			component.BloodTemporarySolution.RemoveAllSolution();
			tempSol.AddSolution(component.ChemicalSolution, this._prototypeManager);
			component.ChemicalSolution.RemoveAllSolution();
			this._spillableSystem.SpillAt(uid, tempSol, "PuddleBlood", true, true, null);
		}

		// Token: 0x04001756 RID: 5974
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x04001757 RID: 5975
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04001758 RID: 5976
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04001759 RID: 5977
		[Dependency]
		private readonly SpillableSystem _spillableSystem;

		// Token: 0x0400175A RID: 5978
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x0400175B RID: 5979
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x0400175C RID: 5980
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400175D RID: 5981
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400175E RID: 5982
		[Dependency]
		private readonly SharedDrunkSystem _drunkSystem;
	}
}
