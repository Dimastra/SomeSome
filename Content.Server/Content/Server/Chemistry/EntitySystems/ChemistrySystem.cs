using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Administration.Logs;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.DoAfter;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.Weapons.Melee;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.CombatMode;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000695 RID: 1685
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemistrySystem : EntitySystem
	{
		// Token: 0x060022DF RID: 8927 RVA: 0x000B5609 File Offset: 0x000B3809
		public override void Initialize()
		{
			this.InitializeHypospray();
			this.InitializeInjector();
			this.InitializeMixing();
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x000B5620 File Offset: 0x000B3820
		private void InitializeInjector()
		{
			base.SubscribeLocalEvent<InjectorComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<InjectorComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSetTransferVerbs), null, null);
			base.SubscribeLocalEvent<InjectorComponent, SolutionChangedEvent>(new ComponentEventHandler<InjectorComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
			base.SubscribeLocalEvent<InjectorComponent, DoAfterEvent>(new ComponentEventHandler<InjectorComponent, DoAfterEvent>(this.OnInjectDoAfter), null, null);
			base.SubscribeLocalEvent<InjectorComponent, ComponentStartup>(new ComponentEventHandler<InjectorComponent, ComponentStartup>(this.OnInjectorStartup), null, null);
			base.SubscribeLocalEvent<InjectorComponent, UseInHandEvent>(new ComponentEventHandler<InjectorComponent, UseInHandEvent>(this.OnInjectorUse), null, null);
			base.SubscribeLocalEvent<InjectorComponent, AfterInteractEvent>(new ComponentEventHandler<InjectorComponent, AfterInteractEvent>(this.OnInjectorAfterInteract), null, null);
			base.SubscribeLocalEvent<InjectorComponent, ComponentGetState>(new ComponentEventRefHandler<InjectorComponent, ComponentGetState>(this.OnInjectorGetState), null, null);
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x000B56BC File Offset: 0x000B38BC
		private void AddSetTransferVerbs(EntityUid uid, InjectorComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			int priority = 0;
			using (List<int>.Enumerator enumerator = ChemistrySystem.TransferAmounts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int amount = enumerator.Current;
					if (amount >= component.MinimumTransferAmount.Int() && amount <= component.MaximumTransferAmount.Int())
					{
						AlternativeVerb verb = new AlternativeVerb();
						verb.Text = Loc.GetString("comp-solution-transfer-verb-amount", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("amount", amount)
						});
						verb.Category = VerbCategory.SetTransferAmount;
						verb.Act = delegate()
						{
							component.TransferAmount = FixedPoint2.New(amount);
							this._popup.PopupEntity(Loc.GetString("comp-solution-transfer-set-amount", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("amount", amount)
							}), args.User, args.User, PopupType.Small);
						};
						verb.Priority = priority;
						priority--;
						args.Verbs.Add(verb);
					}
				}
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x000B5850 File Offset: 0x000B3A50
		private void UseInjector(EntityUid target, EntityUid user, EntityUid injector, InjectorComponent component)
		{
			if (component.ToggleState != SharedInjectorComponent.InjectorToggleMode.Inject)
			{
				if (component.ToggleState == SharedInjectorComponent.InjectorToggleMode.Draw)
				{
					BloodstreamComponent stream;
					if (base.TryComp<BloodstreamComponent>(target, ref stream))
					{
						this.TryDraw(component, injector, target, stream.BloodSolution, user, stream);
						return;
					}
					Solution drawableSolution;
					if (this._solutions.TryGetDrawableSolution(target, out drawableSolution, null, null))
					{
						this.TryDraw(component, injector, target, drawableSolution, user, null);
						return;
					}
					this._popup.PopupEntity(Loc.GetString("injector-component-cannot-draw-message", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
					}), injector, user, PopupType.Small);
				}
				return;
			}
			Solution injectableSolution;
			if (this._solutions.TryGetInjectableSolution(target, out injectableSolution, null, null))
			{
				this.TryInject(component, injector, target, injectableSolution, user, false);
				return;
			}
			Solution refillableSolution;
			if (this._solutions.TryGetRefillableSolution(target, out refillableSolution, null, null))
			{
				this.TryInject(component, injector, target, refillableSolution, user, true);
				return;
			}
			BloodstreamComponent bloodstream;
			if (base.TryComp<BloodstreamComponent>(target, ref bloodstream))
			{
				this.TryInjectIntoBloodstream(component, injector, target, bloodstream, user);
				return;
			}
			this._popup.PopupEntity(Loc.GetString("injector-component-cannot-transfer-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
			}), injector, user, PopupType.Small);
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x000B598B File Offset: 0x000B3B8B
		private void OnSolutionChange(EntityUid uid, InjectorComponent component, SolutionChangedEvent args)
		{
			base.Dirty(component, null);
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000B5998 File Offset: 0x000B3B98
		private void OnInjectorGetState(EntityUid uid, InjectorComponent component, ref ComponentGetState args)
		{
			Solution solution;
			this._solutions.TryGetSolution(uid, "injector", out solution, null);
			FixedPoint2 currentVolume = (solution != null) ? solution.Volume : FixedPoint2.Zero;
			FixedPoint2 maxVolume = (solution != null) ? solution.MaxVolume : FixedPoint2.Zero;
			args.State = new SharedInjectorComponent.InjectorComponentState(currentVolume, maxVolume, component.ToggleState);
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x000B59F0 File Offset: 0x000B3BF0
		private void OnInjectDoAfter(EntityUid uid, InjectorComponent component, DoAfterEvent args)
		{
			if (args.Cancelled)
			{
				component.IsInjecting = false;
				return;
			}
			if (args.Handled || args.Args.Target == null)
			{
				return;
			}
			this.UseInjector(args.Args.Target.Value, args.Args.User, uid, component);
			component.IsInjecting = false;
			args.Handled = true;
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x000B5A5C File Offset: 0x000B3C5C
		private void OnInjectorAfterInteract(EntityUid uid, InjectorComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				if (target.Valid && base.HasComp<SolutionContainerManagerComponent>(uid))
				{
					if (!base.HasComp<MobStateComponent>(target) && !base.HasComp<BloodstreamComponent>(target))
					{
						this.UseInjector(target, args.User, uid, component);
						args.Handled = true;
						return;
					}
					if (component.IgnoreMobs)
					{
						return;
					}
					this.InjectDoAfter(component, args.User, target, uid);
					args.Handled = true;
					return;
				}
			}
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000B5AED File Offset: 0x000B3CED
		private void OnInjectorStartup(EntityUid uid, InjectorComponent component, ComponentStartup args)
		{
			base.Dirty(component, null);
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x000B5AF7 File Offset: 0x000B3CF7
		private void OnInjectorUse(EntityUid uid, InjectorComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.Toggle(component, args.User, uid);
			args.Handled = true;
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x000B5B18 File Offset: 0x000B3D18
		private void Toggle(InjectorComponent component, EntityUid user, EntityUid injector)
		{
			if (component.InjectOnly)
			{
				return;
			}
			SharedInjectorComponent.InjectorToggleMode toggleState = component.ToggleState;
			string msg;
			if (toggleState != SharedInjectorComponent.InjectorToggleMode.Inject)
			{
				if (toggleState != SharedInjectorComponent.InjectorToggleMode.Draw)
				{
					throw new ArgumentOutOfRangeException();
				}
				component.ToggleState = SharedInjectorComponent.InjectorToggleMode.Inject;
				msg = "injector-component-injecting-text";
			}
			else
			{
				component.ToggleState = SharedInjectorComponent.InjectorToggleMode.Draw;
				msg = "injector-component-drawing-text";
			}
			this._popup.PopupEntity(Loc.GetString(msg), injector, user, PopupType.Small);
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x000B5B78 File Offset: 0x000B3D78
		private void InjectDoAfter(InjectorComponent component, EntityUid user, EntityUid target, EntityUid injector)
		{
			this._popup.PopupEntity(Loc.GetString("injector-component-injecting-user"), target, user, PopupType.Small);
			Solution solution;
			if (!this._solutions.TryGetSolution(injector, "injector", out solution, null))
			{
				return;
			}
			if (component.IsInjecting)
			{
				return;
			}
			float actualDelay = MathF.Max(component.Delay, 1f);
			actualDelay += (float)component.TransferAmount / component.Delay - 1f;
			bool isTarget = user != target;
			if (isTarget)
			{
				EntityUid userName = Identity.Entity(user, this.EntityManager);
				this._popup.PopupEntity(Loc.GetString("injector-component-injecting-target", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", userName)
				}), user, target, PopupType.Small);
				if (this._mobState.IsIncapacitated(target, null))
				{
					actualDelay /= 2f;
				}
				else if (this._combat.IsInCombatMode(new EntityUid?(target), null))
				{
					actualDelay += 1f;
				}
				if (component.ToggleState == SharedInjectorComponent.InjectorToggleMode.Inject)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.ForceFeed;
					LogStringHandler logStringHandler = new LogStringHandler(42, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(user), "user", "EntityManager.ToPrettyString(user)");
					logStringHandler.AppendLiteral(" is attempting to inject ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(target), "target", "EntityManager.ToPrettyString(target)");
					logStringHandler.AppendLiteral(" with a solution ");
					logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(solution), 0, "solution");
					adminLogger.Add(type, ref logStringHandler);
				}
			}
			else
			{
				actualDelay /= 2f;
				if (component.ToggleState == SharedInjectorComponent.InjectorToggleMode.Inject)
				{
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Ingestion;
					LogStringHandler logStringHandler = new LogStringHandler(53, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(user), "user", "EntityManager.ToPrettyString(user)");
					logStringHandler.AppendLiteral(" is attempting to inject themselves with a solution ");
					logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(solution), 0, "solution");
					logStringHandler.AppendLiteral(".");
					adminLogger2.Add(type2, ref logStringHandler);
				}
			}
			component.IsInjecting = true;
			SharedDoAfterSystem doAfter = this._doAfter;
			float delay = actualDelay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(injector);
			doAfter.DoAfter(new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used)
			{
				RaiseOnTarget = isTarget,
				RaiseOnUser = !isTarget,
				BreakOnUserMove = true,
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = true,
				MovementThreshold = 0.1f
			});
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x000B5DE0 File Offset: 0x000B3FE0
		private void TryInjectIntoBloodstream(InjectorComponent component, EntityUid injector, EntityUid target, BloodstreamComponent targetBloodstream, EntityUid user)
		{
			FixedPoint2 realTransferAmount = FixedPoint2.Min(component.TransferAmount, targetBloodstream.ChemicalSolution.AvailableVolume);
			if (realTransferAmount <= 0)
			{
				this._popup.PopupEntity(Loc.GetString("injector-component-cannot-inject-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
				}), injector, user, PopupType.Small);
				return;
			}
			Solution removedSolution = this._solutions.SplitSolution(user, targetBloodstream.ChemicalSolution, realTransferAmount);
			this._blood.TryAddToChemicals(target, removedSolution, targetBloodstream);
			this._reactiveSystem.DoEntityReaction(target, removedSolution, ReactionMethod.Injection);
			this._popup.PopupEntity(Loc.GetString("injector-component-inject-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", removedSolution.Volume),
				new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
			}), injector, user, PopupType.Small);
			base.Dirty(component, null);
			this.AfterInject(component, injector);
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000B5EF4 File Offset: 0x000B40F4
		private void TryInject(InjectorComponent component, EntityUid injector, EntityUid targetEntity, Solution targetSolution, EntityUid user, bool asRefill)
		{
			Solution solution;
			if (!this._solutions.TryGetSolution(injector, "injector", out solution, null) || solution.Volume == 0)
			{
				return;
			}
			FixedPoint2 realTransferAmount = FixedPoint2.Min(component.TransferAmount, targetSolution.AvailableVolume);
			if (realTransferAmount <= 0)
			{
				this._popup.PopupEntity(Loc.GetString("injector-component-target-already-full-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(targetEntity, this.EntityManager))
				}), injector, user, PopupType.Small);
				return;
			}
			Solution removedSolution = this._solutions.SplitSolution(injector, solution, realTransferAmount);
			this._reactiveSystem.DoEntityReaction(targetEntity, removedSolution, ReactionMethod.Injection);
			if (!asRefill)
			{
				this._solutions.Inject(targetEntity, targetSolution, removedSolution, null);
			}
			else
			{
				this._solutions.Refill(targetEntity, targetSolution, removedSolution, null);
			}
			this._popup.PopupEntity(Loc.GetString("injector-component-transfer-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", removedSolution.Volume),
				new ValueTuple<string, object>("target", Identity.Entity(targetEntity, this.EntityManager))
			}), injector, user, PopupType.Small);
			base.Dirty(component, null);
			this.AfterInject(component, injector);
		}

		// Token: 0x060022ED RID: 8941 RVA: 0x000B6034 File Offset: 0x000B4234
		private void AfterInject(InjectorComponent component, EntityUid injector)
		{
			Solution solution;
			if (this._solutions.TryGetSolution(injector, "injector", out solution, null) && solution.Volume == 0)
			{
				component.ToggleState = SharedInjectorComponent.InjectorToggleMode.Draw;
			}
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x000B606C File Offset: 0x000B426C
		private void AfterDraw(InjectorComponent component, EntityUid injector)
		{
			Solution solution;
			if (this._solutions.TryGetSolution(injector, "injector", out solution, null) && solution.AvailableVolume == 0)
			{
				component.ToggleState = SharedInjectorComponent.InjectorToggleMode.Inject;
			}
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x000B60A4 File Offset: 0x000B42A4
		private void TryDraw(InjectorComponent component, EntityUid injector, EntityUid targetEntity, Solution targetSolution, EntityUid user, [Nullable(2)] BloodstreamComponent stream = null)
		{
			Solution solution;
			if (!this._solutions.TryGetSolution(injector, "injector", out solution, null) || solution.AvailableVolume == 0)
			{
				return;
			}
			FixedPoint2 realTransferAmount = FixedPoint2.Min(new FixedPoint2[]
			{
				component.TransferAmount,
				targetSolution.Volume,
				solution.AvailableVolume
			});
			if (realTransferAmount <= 0)
			{
				this._popup.PopupEntity(Loc.GetString("injector-component-target-is-empty-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(targetEntity, this.EntityManager))
				}), injector, user, PopupType.Small);
				return;
			}
			if (stream != null)
			{
				this.DrawFromBlood(user, injector, targetEntity, component, solution, stream, realTransferAmount);
				return;
			}
			Solution removedSolution = this._solutions.Draw(targetEntity, targetSolution, realTransferAmount, null);
			if (!this._solutions.TryAddSolution(injector, solution, removedSolution))
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("injector-component-draw-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", removedSolution.Volume),
				new ValueTuple<string, object>("target", Identity.Entity(targetEntity, this.EntityManager))
			}), injector, user, PopupType.Small);
			base.Dirty(component, null);
			this.AfterDraw(component, injector);
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000B61F8 File Offset: 0x000B43F8
		private void DrawFromBlood(EntityUid user, EntityUid injector, EntityUid target, InjectorComponent component, Solution injectorSolution, BloodstreamComponent stream, FixedPoint2 transferAmount)
		{
			float drawAmount = (float)transferAmount;
			float bloodAmount = drawAmount;
			float chemAmount = 0f;
			if (stream.ChemicalSolution.Volume > 0f)
			{
				bloodAmount = drawAmount * 0.85f;
				chemAmount = drawAmount * 0.15f;
			}
			Solution bloodTemp = stream.BloodSolution.SplitSolution(bloodAmount);
			Solution chemTemp = stream.ChemicalSolution.SplitSolution(chemAmount);
			this._solutions.TryAddSolution(injector, injectorSolution, bloodTemp);
			this._solutions.TryAddSolution(injector, injectorSolution, chemTemp);
			this._popup.PopupEntity(Loc.GetString("injector-component-draw-success-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("amount", transferAmount),
				new ValueTuple<string, object>("target", Identity.Entity(target, this.EntityManager))
			}), injector, user, PopupType.Small);
			base.Dirty(component, null);
			this.AfterDraw(component, injector);
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x000B62F8 File Offset: 0x000B44F8
		private void InitializeHypospray()
		{
			base.SubscribeLocalEvent<HyposprayComponent, AfterInteractEvent>(new ComponentEventHandler<HyposprayComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<HyposprayComponent, MeleeHitEvent>(new ComponentEventHandler<HyposprayComponent, MeleeHitEvent>(this.OnAttack), null, null);
			base.SubscribeLocalEvent<HyposprayComponent, SolutionChangedEvent>(new ComponentEventHandler<HyposprayComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
			base.SubscribeLocalEvent<HyposprayComponent, UseInHandEvent>(new ComponentEventHandler<HyposprayComponent, UseInHandEvent>(this.OnUseInHand), null, null);
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x000B6355 File Offset: 0x000B4555
		private void OnUseInHand(EntityUid uid, HyposprayComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.TryDoInject(uid, new EntityUid?(args.User), args.User, null);
			args.Handled = true;
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x000B6381 File Offset: 0x000B4581
		private void OnSolutionChange(EntityUid uid, HyposprayComponent component, SolutionChangedEvent args)
		{
			base.Dirty(component, null);
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x000B638C File Offset: 0x000B458C
		public void OnAfterInteract(EntityUid uid, HyposprayComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach)
			{
				return;
			}
			EntityUid? target = args.Target;
			EntityUid user = args.User;
			this.TryDoInject(uid, target, user, null);
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000B63BB File Offset: 0x000B45BB
		public void OnAttack(EntityUid uid, HyposprayComponent component, MeleeHitEvent args)
		{
			if (!args.HitEntities.Any<EntityUid>())
			{
				return;
			}
			this.TryDoInject(uid, new EntityUid?(args.HitEntities.First<EntityUid>()), args.User, null);
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000B63EC File Offset: 0x000B45EC
		[NullableContext(2)]
		public bool TryDoInject(EntityUid uid, EntityUid? target, EntityUid user, HyposprayComponent component = null)
		{
			if (!base.Resolve<HyposprayComponent>(uid, ref component, true))
			{
				return false;
			}
			if (!ChemistrySystem.EligibleEntity(target, this._entMan))
			{
				return false;
			}
			string msgFormat = null;
			EntityUid? entityUid = target;
			if (entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == user))
			{
				msgFormat = "hypospray-component-inject-self-message";
			}
			else if (ChemistrySystem.EligibleEntity(new EntityUid?(user), this._entMan) && this._interaction.TryRollClumsy(user, component.ClumsyFailChance, null))
			{
				msgFormat = "hypospray-component-inject-self-clumsy-message";
				target = new EntityUid?(user);
			}
			Solution hypoSpraySolution;
			this._solutions.TryGetSolution(uid, component.SolutionName, out hypoSpraySolution, null);
			if (hypoSpraySolution == null || hypoSpraySolution.Volume == 0)
			{
				this._popup.PopupCursor(Loc.GetString("hypospray-component-empty-message"), user, PopupType.Small);
				return true;
			}
			Solution targetSolution;
			if (!this._solutions.TryGetInjectableSolution(target.Value, out targetSolution, null, null))
			{
				this._popup.PopupCursor(Loc.GetString("hypospray-cant-inject", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", Identity.Entity(target.Value, this._entMan))
				}), user, PopupType.Small);
				return false;
			}
			this._popup.PopupCursor(Loc.GetString(msgFormat ?? "hypospray-component-inject-other-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("other", target)
			}), user, PopupType.Small);
			entityUid = target;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != user))
			{
				this._popup.PopupCursor(Loc.GetString("hypospray-component-feel-prick-message"), target.Value, PopupType.Small);
				EntitySystem.Get<MeleeWeaponSystem>();
				Angle.FromWorldVec(this._entMan.GetComponent<TransformComponent>(target.Value).WorldPosition - this._entMan.GetComponent<TransformComponent>(user).WorldPosition);
			}
			this._audio.PlayPvs(component.InjectSound, user, null);
			FixedPoint2 realTransferAmount = FixedPoint2.Min(component.TransferAmount, targetSolution.AvailableVolume);
			if (realTransferAmount <= 0)
			{
				this._popup.PopupCursor(Loc.GetString("hypospray-component-transfer-already-full-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", target)
				}), user, PopupType.Small);
				return true;
			}
			Solution removedSolution = this._solutions.SplitSolution(uid, hypoSpraySolution, realTransferAmount);
			if (!targetSolution.CanAddSolution(removedSolution))
			{
				return true;
			}
			this._reactiveSystem.DoEntityReaction(target.Value, removedSolution, ReactionMethod.Injection);
			this._solutions.TryAddSolution(target.Value, targetSolution, removedSolution);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.ForceFeed;
			LogStringHandler logStringHandler = new LogStringHandler(36, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(user), "user", "_entMan.ToPrettyString(user)");
			logStringHandler.AppendLiteral(" injected ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(target.Value), "target", "_entMan.ToPrettyString(target.Value)");
			logStringHandler.AppendLiteral(" with a solution ");
			logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(removedSolution), 0, "removedSolution");
			logStringHandler.AppendLiteral(" using a ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(uid), "using", "_entMan.ToPrettyString(uid)");
			adminLogger.Add(type, ref logStringHandler);
			return true;
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x000B6749 File Offset: 0x000B4949
		private static bool EligibleEntity([NotNullWhen(true)] EntityUid? entity, IEntityManager entMan)
		{
			return entMan.HasComponent<SolutionContainerManagerComponent>(entity) && entMan.HasComponent<MobStateComponent>(entity);
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x000B675D File Offset: 0x000B495D
		public void InitializeMixing()
		{
			base.SubscribeLocalEvent<ReactionMixerComponent, AfterInteractEvent>(new ComponentEventHandler<ReactionMixerComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x000B6774 File Offset: 0x000B4974
		private void OnAfterInteract(EntityUid uid, ReactionMixerComponent component, AfterInteractEvent args)
		{
			if (args.Target == null || !args.CanReach)
			{
				return;
			}
			MixingAttemptEvent mixAttemptEvent = new MixingAttemptEvent(uid, false);
			base.RaiseLocalEvent<MixingAttemptEvent>(uid, ref mixAttemptEvent, false);
			if (mixAttemptEvent.Cancelled)
			{
				return;
			}
			Solution solution = null;
			if (!this._solutions.TryGetMixableSolution(args.Target.Value, out solution, null))
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString(component.MixMessage, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mixed", Identity.Entity(args.Target.Value, this.EntityManager)),
				new ValueTuple<string, object>("mixer", Identity.Entity(uid, this.EntityManager))
			}), args.User, args.User, PopupType.Small);
			this._solutions.UpdateChemicals(args.Target.Value, solution, true, component);
			AfterMixingEvent afterMixingEvent = new AfterMixingEvent(uid, args.Target.Value);
			base.RaiseLocalEvent<AfterMixingEvent>(uid, afterMixingEvent, false);
		}

		// Token: 0x0400159D RID: 5533
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400159E RID: 5534
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400159F RID: 5535
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x040015A0 RID: 5536
		[Dependency]
		private readonly BloodstreamSystem _blood;

		// Token: 0x040015A1 RID: 5537
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x040015A2 RID: 5538
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x040015A3 RID: 5539
		[Dependency]
		private readonly ReactiveSystem _reactiveSystem;

		// Token: 0x040015A4 RID: 5540
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040015A5 RID: 5541
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x040015A6 RID: 5542
		[Dependency]
		private readonly SharedCombatModeSystem _combat;

		// Token: 0x040015A7 RID: 5543
		[Dependency]
		private readonly SolutionContainerSystem _solutions;

		// Token: 0x040015A8 RID: 5544
		public static readonly List<int> TransferAmounts = new List<int>
		{
			1,
			5,
			10,
			15
		};
	}
}
