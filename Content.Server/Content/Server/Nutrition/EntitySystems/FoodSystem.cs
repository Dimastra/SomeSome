using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.DoAfter;
using Content.Server.Hands.Components;
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
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030F RID: 783
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FoodSystem : EntitySystem
	{
		// Token: 0x06001021 RID: 4129 RVA: 0x000529DC File Offset: 0x00050BDC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FoodComponent, UseInHandEvent>(new ComponentEventHandler<FoodComponent, UseInHandEvent>(this.OnUseFoodInHand), null, null);
			base.SubscribeLocalEvent<FoodComponent, AfterInteractEvent>(new ComponentEventHandler<FoodComponent, AfterInteractEvent>(this.OnFeedFood), null, null);
			base.SubscribeLocalEvent<FoodComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<FoodComponent, GetVerbsEvent<AlternativeVerb>>(this.AddEatVerb), null, null);
			base.SubscribeLocalEvent<FoodComponent, DoAfterEvent<FoodSystem.FoodData>>(new ComponentEventHandler<FoodComponent, DoAfterEvent<FoodSystem.FoodData>>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<InventoryComponent, IngestionAttemptEvent>(new ComponentEventHandler<InventoryComponent, IngestionAttemptEvent>(this.OnInventoryIngestAttempt), null, null);
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x00052A53 File Offset: 0x00050C53
		private void OnUseFoodInHand(EntityUid uid, FoodComponent foodComponent, UseInHandEvent ev)
		{
			if (ev.Handled)
			{
				return;
			}
			ev.Handled = this.TryFeed(ev.User, ev.User, uid, foodComponent);
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x00052A78 File Offset: 0x00050C78
		private void OnFeedFood(EntityUid uid, FoodComponent foodComponent, AfterInteractEvent args)
		{
			if (args.Handled || args.Target == null || !args.CanReach)
			{
				return;
			}
			args.Handled = this.TryFeed(args.User, args.Target.Value, uid, foodComponent);
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x00052AC8 File Offset: 0x00050CC8
		public bool TryFeed(EntityUid user, EntityUid target, EntityUid food, FoodComponent foodComp)
		{
			MobStateComponent mobState;
			if (food == user || (this.EntityManager.TryGetComponent<MobStateComponent>(food, ref mobState) && this._mobStateSystem.IsAlive(food, mobState)))
			{
				return false;
			}
			if (!this.EntityManager.HasComponent<BodyComponent>(target) || foodComp.ForceFeed)
			{
				return false;
			}
			Solution foodSolution;
			if (!this._solutionContainerSystem.TryGetSolution(food, foodComp.SolutionName, out foodSolution, null))
			{
				return false;
			}
			string flavors = this._flavorProfileSystem.GetLocalizedFlavorsMessage(food, user, foodSolution, null);
			if (foodComp.UsesRemaining <= 0)
			{
				this._popupSystem.PopupEntity(Loc.GetString("food-system-try-use-food-is-empty", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", food)
				}), user, user, PopupType.Small);
				this.DeleteAndSpawnTrash(foodComp, food, new EntityUid?(user));
				return false;
			}
			if (this.IsMouthBlocked(target, new EntityUid?(user)))
			{
				return false;
			}
			List<UtensilComponent> utensils;
			if (!this.TryGetRequiredUtensils(user, foodComp, out utensils, null))
			{
				return false;
			}
			if (!this._interactionSystem.InRangeUnobstructed(user, food, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, true))
			{
				return true;
			}
			foodComp.ForceFeed = (user != target);
			if (foodComp.ForceFeed)
			{
				EntityUid userName = Identity.Entity(user, this.EntityManager);
				this._popupSystem.PopupEntity(Loc.GetString("food-system-force-feed", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", userName)
				}), user, target, PopupType.Small);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(21, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user), "user", "ToPrettyString(user)");
				logStringHandler.AppendLiteral(" is forcing ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				logStringHandler.AppendLiteral(" to eat ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(food), "food", "ToPrettyString(food)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(foodSolution));
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Ingestion;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(12, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				logStringHandler.AppendLiteral(" is eating ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(food), "food", "ToPrettyString(food)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(foodSolution));
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			FoodSystem.FoodData foodData = new FoodSystem.FoodData(foodSolution, flavors, utensils);
			float delay = foodComp.ForceFeed ? foodComp.ForceFeedDelay : foodComp.Delay;
			EntityUid? target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(food);
			DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user, delay, default(CancellationToken), target2, used)
			{
				RaiseOnTarget = foodComp.ForceFeed,
				RaiseOnUser = !foodComp.ForceFeed,
				BreakOnUserMove = foodComp.ForceFeed,
				BreakOnDamage = true,
				BreakOnStun = true,
				BreakOnTargetMove = foodComp.ForceFeed,
				MovementThreshold = 0.01f,
				DistanceThreshold = new float?(1f),
				NeedHand = true
			};
			this._doAfterSystem.DoAfter<FoodSystem.FoodData>(doAfterEventArgs, foodData);
			return true;
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x00052DF4 File Offset: 0x00050FF4
		private void OnDoAfter(EntityUid uid, FoodComponent component, DoAfterEvent<FoodSystem.FoodData> args)
		{
			if (args.Cancelled && component.ForceFeed)
			{
				component.ForceFeed = false;
				return;
			}
			if (args.Cancelled || args.Handled || component.Deleted || args.Args.Target == null)
			{
				return;
			}
			BodyComponent body;
			if (!base.TryComp<BodyComponent>(args.Args.Target.Value, ref body))
			{
				return;
			}
			List<ValueTuple<StomachComponent, OrganComponent>> stomachs;
			if (!this._bodySystem.TryGetBodyOrganComponents<StomachComponent>(args.Args.Target.Value, out stomachs, body))
			{
				return;
			}
			FixedPoint2 transferAmount = (component.TransferAmount != null) ? FixedPoint2.Min(component.TransferAmount.Value, args.AdditionalData.FoodSolution.Volume) : args.AdditionalData.FoodSolution.Volume;
			Solution split = this._solutionContainerSystem.SplitSolution(uid, args.AdditionalData.FoodSolution, transferAmount);
			ValueTuple<StomachComponent, OrganComponent>? firstStomach = Extensions.FirstOrNull<ValueTuple<StomachComponent, OrganComponent>>(stomachs, ([TupleElementNames(new string[]
			{
				"Comp",
				"Organ"
			})] ValueTuple<StomachComponent, OrganComponent> stomach) => this._stomachSystem.CanTransferSolution(stomach.Item1.Owner, split, null));
			if (firstStomach == null)
			{
				this._solutionContainerSystem.TryAddSolution(uid, args.AdditionalData.FoodSolution, split);
				this._popupSystem.PopupEntity(component.ForceFeed ? Loc.GetString("food-system-you-cannot-eat-any-more-other") : Loc.GetString("food-system-you-cannot-eat-any-more"), args.Args.Target.Value, args.Args.User, PopupType.Small);
				args.Handled = true;
				return;
			}
			this._reaction.DoEntityReaction(args.Args.Target.Value, args.AdditionalData.FoodSolution, ReactionMethod.Ingestion);
			this._stomachSystem.TryTransferSolution(firstStomach.Value.Item1.Owner, split, firstStomach.Value.Item1, null);
			string flavors = args.AdditionalData.FlavorMessage;
			if (component.ForceFeed)
			{
				EntityUid targetName = Identity.Entity(args.Args.Target.Value, this.EntityManager);
				EntityUid userName = Identity.Entity(args.Args.User, this.EntityManager);
				this._popupSystem.PopupEntity(Loc.GetString("food-system-force-feed-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("user", userName),
					new ValueTuple<string, object>("flavors", flavors)
				}), uid, uid, PopupType.Small);
				this._popupSystem.PopupEntity(Loc.GetString("food-system-force-feed-success-user", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("target", targetName)
				}), args.Args.User, args.Args.User, PopupType.Small);
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(16, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "user", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" forced ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "target", "ToPrettyString(args.Args.User)");
				logStringHandler.AppendLiteral(" to eat ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "food", "ToPrettyString(uid)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				this._popupSystem.PopupEntity(Loc.GetString(component.EatMessage, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("foodComp", uid),
					new ValueTuple<string, object>("flavors", flavors)
				}), args.Args.User, args.Args.User, PopupType.Small);
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Ingestion;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(5, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "target", "ToPrettyString(args.Args.User)");
				logStringHandler.AppendLiteral(" ate ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "food", "ToPrettyString(uid)");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			this._audio.Play(component.UseSound, Filter.Pvs(args.Args.Target.Value, 2f, null, null, null), args.Args.Target.Value, true, new AudioParams?(AudioParams.Default.WithVolume(-1f)));
			foreach (UtensilComponent utensil in args.AdditionalData.Utensils)
			{
				this._utensilSystem.TryBreak(utensil.Owner, args.Args.User, null);
			}
			if (component.UsesRemaining > 0)
			{
				args.Handled = true;
				return;
			}
			if (string.IsNullOrEmpty(component.TrashPrototype))
			{
				this.EntityManager.QueueDeleteEntity(uid);
			}
			else
			{
				this.DeleteAndSpawnTrash(component, uid, new EntityUid?(args.Args.User));
			}
			args.Handled = true;
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x000532FC File Offset: 0x000514FC
		private void DeleteAndSpawnTrash(FoodComponent component, EntityUid food, EntityUid? user = null)
		{
			MapCoordinates position = base.Transform(food).MapPosition;
			EntityUid finisher = this.EntityManager.SpawnEntity(component.TrashPrototype, position);
			Hand hand;
			if (user != null && this._handsSystem.IsHolding(user.Value, new EntityUid?(food), out hand, null))
			{
				this.EntityManager.DeleteEntity(food);
				this._handsSystem.TryPickup(user.Value, finisher, hand, true, false, null, null);
				return;
			}
			this.EntityManager.QueueDeleteEntity(food);
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x00053384 File Offset: 0x00051584
		private void AddEatVerb(EntityUid uid, FoodComponent component, GetVerbsEvent<AlternativeVerb> ev)
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
					this.TryFeed(ev.User, ev.User, uid, component);
				},
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png", "/")),
				Text = Loc.GetString("food-system-verb-eat"),
				Priority = -1
			};
			ev.Verbs.Add(verb);
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x000534A4 File Offset: 0x000516A4
		[NullableContext(2)]
		public void ProjectileForceFeed(EntityUid uid, EntityUid target, EntityUid? user, FoodComponent food = null, BodyComponent body = null)
		{
			if (!base.Resolve<FoodComponent>(uid, ref food, false) || !base.Resolve<BodyComponent>(target, ref body, false))
			{
				return;
			}
			if (this.IsMouthBlocked(target, null))
			{
				return;
			}
			Solution foodSolution;
			if (!this._solutionContainerSystem.TryGetSolution(uid, food.SolutionName, out foodSolution, null))
			{
				return;
			}
			List<ValueTuple<StomachComponent, OrganComponent>> stomachs;
			if (!this._bodySystem.TryGetBodyOrganComponents<StomachComponent>(target, out stomachs, body))
			{
				return;
			}
			if (food.UsesRemaining <= 0)
			{
				this.DeleteAndSpawnTrash(food, uid, null);
			}
			ValueTuple<StomachComponent, OrganComponent>? firstStomach = Extensions.FirstOrNull<ValueTuple<StomachComponent, OrganComponent>>(stomachs, ([TupleElementNames(new string[]
			{
				"Comp",
				"Organ"
			})] ValueTuple<StomachComponent, OrganComponent> stomach) => this._stomachSystem.CanTransferSolution(stomach.Item1.Owner, foodSolution, null));
			if (firstStomach == null)
			{
				return;
			}
			if (user == null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogStringHandler logStringHandler = new LogStringHandler(31, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "food", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(foodSolution), 0, "solution");
				logStringHandler.AppendLiteral(" was thrown into the mouth of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				adminLogger.Add(type, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.ForceFeed;
				LogStringHandler logStringHandler = new LogStringHandler(27, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
				logStringHandler.AppendLiteral(" threw ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "food", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(foodSolution), 0, "solution");
				logStringHandler.AppendLiteral(" into the mouth of ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(target), "target", "ToPrettyString(target)");
				adminLogger2.Add(type2, ref logStringHandler);
			}
			Filter filter = (user == null) ? Filter.Entities(new EntityUid[]
			{
				target
			}) : Filter.Entities(new EntityUid[]
			{
				target,
				user.Value
			});
			this._popupSystem.PopupEntity(Loc.GetString(food.EatMessage, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("food", food.Owner)
			}), target, filter, true, PopupType.Small);
			foodSolution.DoEntityReaction(uid, ReactionMethod.Ingestion);
			this._stomachSystem.TryTransferSolution(firstStomach.Value.Item1.Owner, foodSolution, firstStomach.Value.Item1, null);
			SoundSystem.Play(food.UseSound.GetSound(null, null), Filter.Pvs(target, 2f, null, null, null), target, new AudioParams?(AudioParams.Default.WithVolume(-1f)));
			if (string.IsNullOrEmpty(food.TrashPrototype))
			{
				this.EntityManager.QueueDeleteEntity(food.Owner);
				return;
			}
			this.DeleteAndSpawnTrash(food, uid, null);
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x000537A8 File Offset: 0x000519A8
		private bool TryGetRequiredUtensils(EntityUid user, FoodComponent component, out List<UtensilComponent> utensils, [Nullable(2)] HandsComponent hands = null)
		{
			utensils = new List<UtensilComponent>();
			if (component.Utensil != UtensilType.None)
			{
				return true;
			}
			if (!base.Resolve<HandsComponent>(user, ref hands, false))
			{
				return false;
			}
			UtensilType usedTypes = UtensilType.None;
			foreach (EntityUid item in this._handsSystem.EnumerateHeld(user, hands))
			{
				UtensilComponent utensil;
				if (this.EntityManager.TryGetComponent<UtensilComponent>(item, ref utensil) && (utensil.Types & component.Utensil) != UtensilType.None && (usedTypes & utensil.Types) != utensil.Types)
				{
					usedTypes |= utensil.Types;
					utensils.Add(utensil);
				}
			}
			if (component.UtensilRequired && (usedTypes & component.Utensil) != component.Utensil)
			{
				this._popupSystem.PopupEntity(Loc.GetString("food-you-need-to-hold-utensil", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("utensil", component.Utensil ^ usedTypes)
				}), user, user, PopupType.Small);
				return false;
			}
			return true;
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x000538B0 File Offset: 0x00051AB0
		private void OnInventoryIngestAttempt(EntityUid uid, InventoryComponent component, IngestionAttemptEvent args)
		{
			if (args.Cancelled)
			{
				return;
			}
			EntityUid? maskUid;
			IngestionBlockerComponent blocker;
			if (this._inventorySystem.TryGetSlotEntity(uid, "mask", out maskUid, null, null) && this.EntityManager.TryGetComponent<IngestionBlockerComponent>(maskUid, ref blocker) && blocker.Enabled)
			{
				args.Blocker = maskUid;
				args.Cancel();
				return;
			}
			EntityUid? headUid;
			if (this._inventorySystem.TryGetSlotEntity(uid, "head", out headUid, null, null) && this.EntityManager.TryGetComponent<IngestionBlockerComponent>(headUid, ref blocker) && blocker.Enabled)
			{
				args.Blocker = headUid;
				args.Cancel();
			}
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x00053940 File Offset: 0x00051B40
		public bool IsMouthBlocked(EntityUid uid, EntityUid? popupUid = null)
		{
			IngestionAttemptEvent attempt = new IngestionAttemptEvent();
			base.RaiseLocalEvent<IngestionAttemptEvent>(uid, attempt, false);
			if (attempt.Cancelled && attempt.Blocker != null && popupUid != null)
			{
				string name = this.EntityManager.GetComponent<MetaDataComponent>(attempt.Blocker.Value).EntityName;
				this._popupSystem.PopupEntity(Loc.GetString("food-system-remove-mask", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", name)
				}), uid, popupUid.Value, PopupType.Small);
			}
			return attempt.Cancelled;
		}

		// Token: 0x04000951 RID: 2385
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x04000952 RID: 2386
		[Dependency]
		private readonly FlavorProfileSystem _flavorProfileSystem;

		// Token: 0x04000953 RID: 2387
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000954 RID: 2388
		[Dependency]
		private readonly StomachSystem _stomachSystem;

		// Token: 0x04000955 RID: 2389
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000956 RID: 2390
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000957 RID: 2391
		[Dependency]
		private readonly UtensilSystem _utensilSystem;

		// Token: 0x04000958 RID: 2392
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000959 RID: 2393
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x0400095A RID: 2394
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x0400095B RID: 2395
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x0400095C RID: 2396
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x0400095D RID: 2397
		[Dependency]
		private readonly ReactiveSystem _reaction;

		// Token: 0x0400095E RID: 2398
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x02000960 RID: 2400
		[NullableContext(0)]
		private struct FoodData : IEquatable<FoodSystem.FoodData>
		{
			// Token: 0x06003221 RID: 12833 RVA: 0x00101269 File Offset: 0x000FF469
			[NullableContext(1)]
			public FoodData(Solution FoodSolution, string FlavorMessage, List<UtensilComponent> Utensils)
			{
				this.FoodSolution = FoodSolution;
				this.FlavorMessage = FlavorMessage;
				this.Utensils = Utensils;
			}

			// Token: 0x06003222 RID: 12834 RVA: 0x00101280 File Offset: 0x000FF480
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FoodData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003223 RID: 12835 RVA: 0x001012CC File Offset: 0x000FF4CC
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("FoodSolution = ");
				builder.Append(this.FoodSolution);
				builder.Append(", FlavorMessage = ");
				builder.Append(this.FlavorMessage);
				builder.Append(", Utensils = ");
				builder.Append(this.Utensils);
				return true;
			}

			// Token: 0x06003224 RID: 12836 RVA: 0x00101325 File Offset: 0x000FF525
			[CompilerGenerated]
			public static bool operator !=(FoodSystem.FoodData left, FoodSystem.FoodData right)
			{
				return !(left == right);
			}

			// Token: 0x06003225 RID: 12837 RVA: 0x00101331 File Offset: 0x000FF531
			[CompilerGenerated]
			public static bool operator ==(FoodSystem.FoodData left, FoodSystem.FoodData right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003226 RID: 12838 RVA: 0x0010133B File Offset: 0x000FF53B
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<Solution>.Default.GetHashCode(this.FoodSolution) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.FlavorMessage)) * -1521134295 + EqualityComparer<List<UtensilComponent>>.Default.GetHashCode(this.Utensils);
			}

			// Token: 0x06003227 RID: 12839 RVA: 0x0010137B File Offset: 0x000FF57B
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is FoodSystem.FoodData && this.Equals((FoodSystem.FoodData)obj);
			}

			// Token: 0x06003228 RID: 12840 RVA: 0x00101394 File Offset: 0x000FF594
			[CompilerGenerated]
			public readonly bool Equals(FoodSystem.FoodData other)
			{
				return EqualityComparer<Solution>.Default.Equals(this.FoodSolution, other.FoodSolution) && EqualityComparer<string>.Default.Equals(this.FlavorMessage, other.FlavorMessage) && EqualityComparer<List<UtensilComponent>>.Default.Equals(this.Utensils, other.Utensils);
			}

			// Token: 0x06003229 RID: 12841 RVA: 0x001013E9 File Offset: 0x000FF5E9
			[NullableContext(1)]
			[CompilerGenerated]
			public readonly void Deconstruct(out Solution FoodSolution, out string FlavorMessage, out List<UtensilComponent> Utensils)
			{
				FoodSolution = this.FoodSolution;
				FlavorMessage = this.FlavorMessage;
				Utensils = this.Utensils;
			}

			// Token: 0x04002008 RID: 8200
			[Nullable(1)]
			public readonly Solution FoodSolution;

			// Token: 0x04002009 RID: 8201
			[Nullable(1)]
			public readonly string FlavorMessage;

			// Token: 0x0400200A RID: 8202
			[Nullable(1)]
			public readonly List<UtensilComponent> Utensils;
		}
	}
}
