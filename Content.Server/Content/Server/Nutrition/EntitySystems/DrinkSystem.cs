using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Nutrition.Components;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030C RID: 780
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DrinkSystem : EntitySystem
	{
		// Token: 0x06001008 RID: 4104 RVA: 0x00051724 File Offset: 0x0004F924
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DrinkComponent, SolutionChangedEvent>(new ComponentEventHandler<DrinkComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
			base.SubscribeLocalEvent<DrinkComponent, ComponentInit>(new ComponentEventHandler<DrinkComponent, ComponentInit>(this.OnDrinkInit), null, null);
			base.SubscribeLocalEvent<DrinkComponent, LandEvent>(new ComponentEventRefHandler<DrinkComponent, LandEvent>(this.HandleLand), null, null);
			base.SubscribeLocalEvent<DrinkComponent, UseInHandEvent>(new ComponentEventHandler<DrinkComponent, UseInHandEvent>(this.OnUse), null, null);
			base.SubscribeLocalEvent<DrinkComponent, AfterInteractEvent>(new ComponentEventHandler<DrinkComponent, AfterInteractEvent>(this.AfterInteract), null, null);
			base.SubscribeLocalEvent<DrinkComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DrinkComponent, GetVerbsEvent<AlternativeVerb>>(this.AddDrinkVerb), null, null);
			base.SubscribeLocalEvent<DrinkComponent, ExaminedEvent>(new ComponentEventHandler<DrinkComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<DrinkComponent, SolutionTransferAttemptEvent>(new ComponentEventHandler<DrinkComponent, SolutionTransferAttemptEvent>(this.OnTransferAttempt), null, null);
			base.SubscribeLocalEvent<DrinkComponent, DoAfterEvent<DrinkSystem.DrinkData>>(new ComponentEventHandler<DrinkComponent, DoAfterEvent<DrinkSystem.DrinkData>>(this.OnDoAfter), null, null);
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x000517EB File Offset: 0x0004F9EB
		[NullableContext(2)]
		public bool IsEmpty(EntityUid uid, DrinkComponent component = null)
		{
			return !base.Resolve<DrinkComponent>(uid, ref component, true) || this._solutionContainerSystem.DrainAvailable(uid) <= 0;
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x00051810 File Offset: 0x0004FA10
		private void OnExamined(EntityUid uid, DrinkComponent component, ExaminedEvent args)
		{
			if (!component.Opened || !args.IsInDetailsRange)
			{
				return;
			}
			string color = this.IsEmpty(uid, component) ? "gray" : "yellow";
			string openedText = Loc.GetString(this.IsEmpty(uid, component) ? "drink-component-on-examine-is-empty" : "drink-component-on-examine-is-opened");
			args.Message.AddMarkup("\n" + Loc.GetString("drink-component-on-examine-details-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("colorName", color),
				new ValueTuple<string, object>("text", openedText)
			}));
			if (!this.IsEmpty(uid, component))
			{
				ExaminableSolutionComponent comp;
				if (base.TryComp<ExaminableSolutionComponent>(component.Owner, ref comp))
				{
					args.Message.AddMarkup(" - " + Loc.GetString("drink-component-on-examine-exact-volume", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("amount", this._solutionContainerSystem.DrainAvailable(uid))
					}));
					return;
				}
				int num = (int)this._solutionContainerSystem.PercentFull(uid);
				string text;
				if (num > 66)
				{
					if (num != 100)
					{
						text = "drink-component-on-examine-is-mostly-full";
					}
					else
					{
						text = "drink-component-on-examine-is-full";
					}
				}
				else if (num <= 33)
				{
					text = "drink-component-on-examine-is-mostly-empty";
				}
				else
				{
					text = this.HalfEmptyOrHalfFull(args);
				}
				string remainingString = text;
				args.Message.AddMarkup(" - " + Loc.GetString(remainingString));
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x00051978 File Offset: 0x0004FB78
		[NullableContext(2)]
		private void SetOpen(EntityUid uid, bool opened = false, DrinkComponent component = null)
		{
			if (!base.Resolve<DrinkComponent>(uid, ref component, true))
			{
				return;
			}
			if (opened == component.Opened)
			{
				return;
			}
			component.Opened = opened;
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, component.SolutionName, out solution, null))
			{
				return;
			}
			AppearanceComponent appearance;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				this._appearanceSystem.SetData(uid, DrinkCanStateVisual.Opened, opened, appearance);
			}
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x000519E4 File Offset: 0x0004FBE4
		private void AfterInteract(EntityUid uid, DrinkComponent component, AfterInteractEvent args)
		{
			if (args.Handled || args.Target == null || !args.CanReach)
			{
				return;
			}
			args.Handled = this.TryDrink(args.User, args.Target.Value, component, uid);
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x00051A34 File Offset: 0x0004FC34
		private void OnUse(EntityUid uid, DrinkComponent component, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.Opened)
			{
				this._audio.PlayPvs(this._audio.GetSound(component.OpenSounds), args.User, null);
				this.SetOpen(uid, true, component);
				return;
			}
			args.Handled = this.TryDrink(args.User, args.User, component, uid);
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x00051AA4 File Offset: 0x0004FCA4
		private void HandleLand(EntityUid uid, DrinkComponent component, ref LandEvent args)
		{
			Solution interactions;
			if (component.Pressurized && !component.Opened && RandomExtensions.Prob(this._random, 0.25f) && this._solutionContainerSystem.TryGetDrainableSolution(uid, out interactions, null, null))
			{
				component.Opened = true;
				this.UpdateAppearance(component);
				Solution solution = this._solutionContainerSystem.Drain(uid, interactions, interactions.Volume, null);
				this._spillableSystem.SpillAt(uid, solution, "PuddleSmear", true, true, null);
				this._audio.PlayPvs(this._audio.GetSound(component.BurstSound), uid, new AudioParams?(AudioParams.Default.WithVolume(-4f)));
			}
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x00051B58 File Offset: 0x0004FD58
		private void OnDrinkInit(EntityUid uid, DrinkComponent component, ComponentInit args)
		{
			this.SetOpen(uid, component.DefaultToOpened, component);
			DrainableSolutionComponent existingDrainable;
			if (this.EntityManager.TryGetComponent<DrainableSolutionComponent>(uid, ref existingDrainable))
			{
				component.SolutionName = existingDrainable.Solution;
			}
			else
			{
				this._solutionContainerSystem.EnsureSolution(uid, component.SolutionName, null);
			}
			this.UpdateAppearance(component);
			RefillableSolutionComponent refillComp;
			if (base.TryComp<RefillableSolutionComponent>(uid, ref refillComp))
			{
				refillComp.Solution = component.SolutionName;
			}
			DrainableSolutionComponent drainComp;
			if (base.TryComp<DrainableSolutionComponent>(uid, ref drainComp))
			{
				drainComp.Solution = component.SolutionName;
			}
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x00051BDA File Offset: 0x0004FDDA
		private void OnSolutionChange(EntityUid uid, DrinkComponent component, SolutionChangedEvent args)
		{
			this.UpdateAppearance(component);
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x00051BE4 File Offset: 0x0004FDE4
		public void UpdateAppearance(DrinkComponent component)
		{
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<AppearanceComponent>(component.Owner, ref appearance) || !this.EntityManager.HasComponent<SolutionContainerManagerComponent>(component.Owner))
			{
				return;
			}
			FixedPoint2 drainAvailable = this._solutionContainerSystem.DrainAvailable(component.Owner);
			this._appearanceSystem.SetData(component.Owner, FoodVisuals.Visual, drainAvailable.Float(), appearance);
			this._appearanceSystem.SetData(component.Owner, DrinkCanStateVisual.Opened, component.Opened, appearance);
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x00051C74 File Offset: 0x0004FE74
		private void OnTransferAttempt(EntityUid uid, DrinkComponent component, SolutionTransferAttemptEvent args)
		{
			if (!component.Opened)
			{
				args.Cancel(Loc.GetString("drink-component-try-use-drink-not-open", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", this.EntityManager.GetComponent<MetaDataComponent>(component.Owner).EntityName)
				}));
			}
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x00051CC8 File Offset: 0x0004FEC8
		private bool TryDrink(EntityUid user, EntityUid target, DrinkComponent drink, EntityUid item)
		{
			if (!this.EntityManager.HasComponent<BodyComponent>(target) || drink.ForceDrink)
			{
				return false;
			}
			if (!drink.Opened)
			{
				this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-not-open", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("owner", this.EntityManager.GetComponent<MetaDataComponent>(item).EntityName)
				}), item, user, PopupType.Small);
				return true;
			}
			Solution drinkSolution;
			if (!this._solutionContainerSystem.TryGetDrainableSolution(item, out drinkSolution, null, null) || drinkSolution.Volume <= 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-is-empty", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", this.EntityManager.GetComponent<MetaDataComponent>(item).EntityName)
				}), item, user, PopupType.Small);
				return true;
			}
			if (this._foodSystem.IsMouthBlocked(target, new EntityUid?(user)))
			{
				return true;
			}
			if (!this._interactionSystem.InRangeUnobstructed(user, item, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
			{
				return true;
			}
			drink.ForceDrink = (user != target);
			if (drink.ForceDrink)
			{
				EntityUid userName = Identity.Entity(user, this.EntityManager);
				this._popupSystem.PopupEntity(Loc.GetString("drink-component-force-feed", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", userName)
				}), user, target, PopupType.Small);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(23, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" is forcing ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				logStringHandler.AppendLiteral(" to drink ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "drink", "ToPrettyString(item)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(drinkSolution));
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Ingestion;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(14, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				logStringHandler.AppendLiteral(" is drinking ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "drink", "ToPrettyString(item)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(drinkSolution));
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			bool moveBreak = user != target;
			string flavors = this._flavorProfileSystem.GetLocalizedFlavorsMessage(user, drinkSolution);
			DrinkSystem.DrinkData drinkData = new DrinkSystem.DrinkData(drinkSolution, flavors);
			float delay = drink.ForceDrink ? drink.ForceFeedDelay : drink.Delay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(item);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used)
			{
				BreakOnUserMove = moveBreak,
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = moveBreak,
				MovementThreshold = 0.01f,
				DistanceThreshold = new float?(1f),
				NeedHand = true
			};
			this._doAfterSystem.DoAfter<DrinkSystem.DrinkData>(doAfterEventArgs, drinkData);
			return true;
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x00051FE0 File Offset: 0x000501E0
		private void OnDoAfter(EntityUid uid, DrinkComponent component, DoAfterEvent<DrinkSystem.DrinkData> args)
		{
			if (args.Cancelled && component.ForceDrink)
			{
				component.ForceDrink = false;
				return;
			}
			if (args.Handled || args.Cancelled || component.Deleted)
			{
				return;
			}
			BodyComponent body;
			if (!base.TryComp<BodyComponent>(args.Args.Target, ref body))
			{
				return;
			}
			FixedPoint2 transferAmount = FixedPoint2.Min(component.TransferAmount, args.AdditionalData.DrinkSolution.Volume);
			Solution drained = this._solutionContainerSystem.Drain(uid, args.AdditionalData.DrinkSolution, transferAmount, null);
			List<ValueTuple<StomachComponent, OrganComponent>> stomachs;
			if (!this._bodySystem.TryGetBodyOrganComponents<StomachComponent>(args.Args.Target.Value, out stomachs, body))
			{
				this._popupSystem.PopupEntity(component.ForceDrink ? Loc.GetString("drink-component-try-use-drink-cannot-drink-other") : Loc.GetString("drink-component-try-use-drink-had-enough"), args.Args.Target.Value, args.Args.User, PopupType.Small);
				if (base.HasComp<RefillableSolutionComponent>(args.Args.Target.Value))
				{
					this._spillableSystem.SpillAt(args.Args.User, drained, "PuddleSmear", true, true, null);
					args.Handled = true;
					return;
				}
				this._solutionContainerSystem.Refill(args.Args.Target.Value, args.AdditionalData.DrinkSolution, drained, null);
				args.Handled = true;
				return;
			}
			else
			{
				ValueTuple<StomachComponent, OrganComponent>? firstStomach = Extensions.FirstOrNull<ValueTuple<StomachComponent, OrganComponent>>(stomachs, ([TupleElementNames(new string[]
				{
					"Comp",
					"Organ"
				})] ValueTuple<StomachComponent, OrganComponent> stomach) => this._stomachSystem.CanTransferSolution(stomach.Item1.Owner, drained, null));
				if (firstStomach == null)
				{
					this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-had-enough"), args.Args.Target.Value, args.Args.Target.Value, PopupType.Small);
					if (component.ForceDrink)
					{
						this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-had-enough-other"), args.Args.Target.Value, args.Args.User, PopupType.Small);
						this._spillableSystem.SpillAt(args.Args.Target.Value, drained, "PuddleSmear", true, true, null);
					}
					else
					{
						this._solutionContainerSystem.TryAddSolution(uid, args.AdditionalData.DrinkSolution, drained);
					}
					args.Handled = true;
					return;
				}
				string flavors = args.AdditionalData.FlavorMessage;
				if (component.ForceDrink)
				{
					EntityUid targetName = Identity.Entity(args.Args.Target.Value, this.EntityManager);
					EntityUid userName = Identity.Entity(args.Args.User, this.EntityManager);
					this._popupSystem.PopupEntity(Loc.GetString("drink-component-force-feed-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("user", userName),
						new ValueTuple<string, object>("flavors", flavors)
					}), args.Args.Target.Value, args.Args.Target.Value, PopupType.Small);
					this._popupSystem.PopupEntity(Loc.GetString("drink-component-force-feed-success-user", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("target", targetName)
					}), args.Args.User, args.Args.User, PopupType.Small);
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.ForceFeed;
					LogImpact impact = LogImpact.Medium;
					LogStringHandler logStringHandler = new LogStringHandler(18, 3);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
					logStringHandler.AppendLiteral(" forced ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "target", "ToPrettyString(args.Args.User)");
					logStringHandler.AppendLiteral(" to drink ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner), "drink", "ToPrettyString(component.Owner)");
					adminLogger.Add(type, impact, ref logStringHandler);
				}
				else
				{
					this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-success-slurp-taste", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("flavors", flavors)
					}), args.Args.User, args.Args.User, PopupType.Small);
					this._popupSystem.PopupEntity(Loc.GetString("drink-component-try-use-drink-success-slurp"), args.Args.User, Filter.PvsExcept(args.Args.User, 2f, null), true, PopupType.Small);
					ISharedAdminLogManager adminLogger2 = this._adminLogger;
					LogType type2 = LogType.Ingestion;
					LogImpact impact2 = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(7, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "target", "ToPrettyString(args.Args.User)");
					logStringHandler.AppendLiteral(" drank ");
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "drink", "ToPrettyString(uid)");
					adminLogger2.Add(type2, impact2, ref logStringHandler);
				}
				this._audio.PlayPvs(this._audio.GetSound(component.UseSound), args.Args.Target.Value, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
				this._reaction.DoEntityReaction(args.Args.Target.Value, args.AdditionalData.DrinkSolution, ReactionMethod.Ingestion);
				this._stomachSystem.TryTransferSolution(firstStomach.Value.Item1.Owner, drained, firstStomach.Value.Item1, null);
				component.ForceDrink = false;
				args.Handled = true;
				return;
			}
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x00052550 File Offset: 0x00050750
		private void AddDrinkVerb(EntityUid uid, DrinkComponent component, GetVerbsEvent<AlternativeVerb> ev)
		{
			BodyComponent body;
			List<ValueTuple<StomachComponent, OrganComponent>> stomachs;
			if (uid == ev.User || !ev.CanInteract || !ev.CanAccess || !this.EntityManager.TryGetComponent<BodyComponent>(ev.User, ref body) || !this._bodySystem.TryGetBodyOrganComponents<StomachComponent>(ev.User, out stomachs, body))
			{
				return;
			}
			MobStateComponent mobState;
			if (this.EntityManager.TryGetComponent<MobStateComponent>(uid, ref mobState) && this._mobStateSystem.IsAlive(uid, mobState))
			{
				return;
			}
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.TryDrink(ev.User, ev.User, component, uid);
				},
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/drink.svg.192dpi.png", "/")),
				Text = Loc.GetString("drink-system-verb-drink"),
				Priority = 2
			};
			ev.Verbs.Add(verb);
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x00052670 File Offset: 0x00050870
		private string HalfEmptyOrHalfFull(ExaminedEvent args)
		{
			string remainingString = "drink-component-on-examine-is-half-full";
			MetaDataComponent examiner;
			if (base.TryComp<MetaDataComponent>(args.Examiner, ref examiner) && examiner.EntityName.Length > 0 && string.Compare(examiner.EntityName.Substring(0, 1), "m", StringComparison.InvariantCultureIgnoreCase) > 0)
			{
				remainingString = "drink-component-on-examine-is-half-empty";
			}
			return remainingString;
		}

		// Token: 0x0400093D RID: 2365
		[Dependency]
		private readonly FoodSystem _foodSystem;

		// Token: 0x0400093E RID: 2366
		[Dependency]
		private readonly FlavorProfileSystem _flavorProfileSystem;

		// Token: 0x0400093F RID: 2367
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000940 RID: 2368
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000941 RID: 2369
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000942 RID: 2370
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000943 RID: 2371
		[Dependency]
		private readonly StomachSystem _stomachSystem;

		// Token: 0x04000944 RID: 2372
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000945 RID: 2373
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000946 RID: 2374
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000947 RID: 2375
		[Dependency]
		private readonly SpillableSystem _spillableSystem;

		// Token: 0x04000948 RID: 2376
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000949 RID: 2377
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x0400094A RID: 2378
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x0400094B RID: 2379
		[Dependency]
		private readonly ReactiveSystem _reaction;

		// Token: 0x0200095C RID: 2396
		[NullableContext(0)]
		private struct DrinkData : IEquatable<DrinkSystem.DrinkData>
		{
			// Token: 0x06003210 RID: 12816 RVA: 0x00101089 File Offset: 0x000FF289
			[NullableContext(1)]
			public DrinkData(Solution DrinkSolution, string FlavorMessage)
			{
				this.DrinkSolution = DrinkSolution;
				this.FlavorMessage = FlavorMessage;
			}

			// Token: 0x06003211 RID: 12817 RVA: 0x0010109C File Offset: 0x000FF29C
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("DrinkData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003212 RID: 12818 RVA: 0x001010E8 File Offset: 0x000FF2E8
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("DrinkSolution = ");
				builder.Append(this.DrinkSolution);
				builder.Append(", FlavorMessage = ");
				builder.Append(this.FlavorMessage);
				return true;
			}

			// Token: 0x06003213 RID: 12819 RVA: 0x0010111D File Offset: 0x000FF31D
			[CompilerGenerated]
			public static bool operator !=(DrinkSystem.DrinkData left, DrinkSystem.DrinkData right)
			{
				return !(left == right);
			}

			// Token: 0x06003214 RID: 12820 RVA: 0x00101129 File Offset: 0x000FF329
			[CompilerGenerated]
			public static bool operator ==(DrinkSystem.DrinkData left, DrinkSystem.DrinkData right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003215 RID: 12821 RVA: 0x00101133 File Offset: 0x000FF333
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<Solution>.Default.GetHashCode(this.DrinkSolution) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.FlavorMessage);
			}

			// Token: 0x06003216 RID: 12822 RVA: 0x0010115C File Offset: 0x000FF35C
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is DrinkSystem.DrinkData && this.Equals((DrinkSystem.DrinkData)obj);
			}

			// Token: 0x06003217 RID: 12823 RVA: 0x00101174 File Offset: 0x000FF374
			[CompilerGenerated]
			public readonly bool Equals(DrinkSystem.DrinkData other)
			{
				return EqualityComparer<Solution>.Default.Equals(this.DrinkSolution, other.DrinkSolution) && EqualityComparer<string>.Default.Equals(this.FlavorMessage, other.FlavorMessage);
			}

			// Token: 0x06003218 RID: 12824 RVA: 0x001011A6 File Offset: 0x000FF3A6
			[NullableContext(1)]
			[CompilerGenerated]
			public readonly void Deconstruct(out Solution DrinkSolution, out string FlavorMessage)
			{
				DrinkSolution = this.DrinkSolution;
				FlavorMessage = this.FlavorMessage;
			}

			// Token: 0x04001FFD RID: 8189
			[Nullable(1)]
			public readonly Solution DrinkSolution;

			// Token: 0x04001FFE RID: 8190
			[Nullable(1)]
			public readonly string FlavorMessage;
		}
	}
}
