using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Construction;
using Content.Server.Hands.Systems;
using Content.Server.Kitchen.Components;
using Content.Server.Power.Components;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Chemistry.Components;
using Content.Shared.Destructible;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Kitchen;
using Content.Shared.Kitchen.Components;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Tag;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server.Kitchen.EntitySystems
{
	// Token: 0x0200042F RID: 1071
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MicrowaveSystem : EntitySystem
	{
		// Token: 0x060015AE RID: 5550 RVA: 0x00071F10 File Offset: 0x00070110
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MicrowaveComponent, ComponentInit>(new ComponentEventHandler<MicrowaveComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, SolutionChangedEvent>(new ComponentEventHandler<MicrowaveComponent, SolutionChangedEvent>(this.OnSolutionChange), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, InteractUsingEvent>(new ComponentEventHandler<MicrowaveComponent, InteractUsingEvent>(this.OnInteractUsing), null, new Type[]
			{
				typeof(AnchorableSystem)
			});
			base.SubscribeLocalEvent<MicrowaveComponent, BreakageEventArgs>(new ComponentEventHandler<MicrowaveComponent, BreakageEventArgs>(this.OnBreak), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, PowerChangedEvent>(new ComponentEventRefHandler<MicrowaveComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, SuicideEvent>(new ComponentEventHandler<MicrowaveComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, RefreshPartsEvent>(new ComponentEventHandler<MicrowaveComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, UpgradeExamineEvent>(new ComponentEventHandler<MicrowaveComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, MicrowaveStartCookMessage>(delegate(EntityUid u, MicrowaveComponent c, MicrowaveStartCookMessage m)
			{
				this.Wzhzhzh(u, c, m.Session.AttachedEntity);
			}, null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, MicrowaveEjectMessage>(new ComponentEventHandler<MicrowaveComponent, MicrowaveEjectMessage>(this.OnEjectMessage), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, MicrowaveEjectSolidIndexedMessage>(new ComponentEventHandler<MicrowaveComponent, MicrowaveEjectSolidIndexedMessage>(this.OnEjectIndex), null, null);
			base.SubscribeLocalEvent<MicrowaveComponent, MicrowaveSelectCookTimeMessage>(new ComponentEventHandler<MicrowaveComponent, MicrowaveSelectCookTimeMessage>(this.OnSelectTime), null, null);
			base.SubscribeLocalEvent<ActiveMicrowaveComponent, ComponentStartup>(new ComponentEventHandler<ActiveMicrowaveComponent, ComponentStartup>(this.OnCookStart), null, null);
			base.SubscribeLocalEvent<ActiveMicrowaveComponent, ComponentShutdown>(new ComponentEventHandler<ActiveMicrowaveComponent, ComponentShutdown>(this.OnCookStop), null, null);
		}

		// Token: 0x060015AF RID: 5551 RVA: 0x00072050 File Offset: 0x00070250
		private void OnCookStart(EntityUid uid, ActiveMicrowaveComponent component, ComponentStartup args)
		{
			MicrowaveComponent microwaveComponent;
			if (!base.TryComp<MicrowaveComponent>(uid, ref microwaveComponent))
			{
				return;
			}
			this.SetAppearance(microwaveComponent, MicrowaveVisualState.Cooking);
			microwaveComponent.PlayingStream = this._audio.PlayPvs(microwaveComponent.LoopingSound, uid, new AudioParams?(AudioParams.Default.WithLoop(true).WithMaxDistance(5f)));
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x000720A8 File Offset: 0x000702A8
		private void OnCookStop(EntityUid uid, ActiveMicrowaveComponent component, ComponentShutdown args)
		{
			MicrowaveComponent microwaveComponent;
			if (!base.TryComp<MicrowaveComponent>(uid, ref microwaveComponent))
			{
				return;
			}
			this.SetAppearance(microwaveComponent, MicrowaveVisualState.Idle);
			IPlayingAudioStream playingStream = microwaveComponent.PlayingStream;
			if (playingStream == null)
			{
				return;
			}
			playingStream.Stop();
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x000720DC File Offset: 0x000702DC
		private void AddTemperature(MicrowaveComponent component, float time)
		{
			float heatToAdd = time * 100f;
			foreach (EntityUid entity in component.Storage.ContainedEntities)
			{
				TemperatureComponent tempComp;
				if (base.TryComp<TemperatureComponent>(entity, ref tempComp))
				{
					this._temperature.ChangeHeat(entity, heatToAdd, false, tempComp);
				}
				SolutionContainerManagerComponent solutions;
				if (base.TryComp<SolutionContainerManagerComponent>(entity, ref solutions))
				{
					foreach (KeyValuePair<string, Solution> keyValuePair in solutions.Solutions)
					{
						string text;
						Solution solution2;
						keyValuePair.Deconstruct(out text, out solution2);
						Solution solution = solution2;
						if (solution.Temperature <= component.TemperatureUpperThreshold)
						{
							this._solutionContainer.AddThermalEnergy(entity, solution, heatToAdd);
						}
					}
				}
			}
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x000721C8 File Offset: 0x000703C8
		private void SubtractContents(MicrowaveComponent component, FoodRecipePrototype recipe)
		{
			Dictionary<string, FixedPoint2> totalReagentsToRemove = new Dictionary<string, FixedPoint2>(recipe.IngredientsReagents);
			foreach (EntityUid item in component.Storage.ContainedEntities)
			{
				SolutionContainerManagerComponent solMan;
				if (base.TryComp<SolutionContainerManagerComponent>(item, ref solMan))
				{
					foreach (KeyValuePair<string, Solution> keyValuePair in solMan.Solutions)
					{
						string text;
						Solution solution2;
						keyValuePair.Deconstruct(out text, out solution2);
						Solution solution = solution2;
						foreach (KeyValuePair<string, FixedPoint2> keyValuePair2 in recipe.IngredientsReagents)
						{
							FixedPoint2 fixedPoint;
							keyValuePair2.Deconstruct(out text, out fixedPoint);
							string reagent = text;
							if (totalReagentsToRemove.ContainsKey(reagent) && solution.ContainsReagent(reagent))
							{
								FixedPoint2 quant = solution.GetReagentQuantity(reagent);
								if (quant >= totalReagentsToRemove[reagent])
								{
									quant = totalReagentsToRemove[reagent];
									totalReagentsToRemove.Remove(reagent);
								}
								else
								{
									Dictionary<string, FixedPoint2> dictionary = totalReagentsToRemove;
									text = reagent;
									dictionary[text] -= quant;
								}
								this._solutionContainer.TryRemoveReagent(item, solution, reagent, quant);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<string, FixedPoint2> recipeSolid in recipe.IngredientsSolids)
			{
				int i = 0;
				while (i < recipeSolid.Value)
				{
					foreach (EntityUid item2 in component.Storage.ContainedEntities)
					{
						MetaDataComponent metaData = base.MetaData(item2);
						if (metaData.EntityPrototype != null && metaData.EntityPrototype.ID == recipeSolid.Key)
						{
							component.Storage.Remove(item2, null, null, null, true, false, null, null);
							this.EntityManager.DeleteEntity(item2);
							break;
						}
					}
					i++;
				}
			}
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x00072490 File Offset: 0x00070690
		private void OnInit(EntityUid uid, MicrowaveComponent component, ComponentInit ags)
		{
			component.Storage = this._container.EnsureContainer<Container>(uid, "microwave_entity_container", null);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x000724AC File Offset: 0x000706AC
		private void OnSuicide(EntityUid uid, MicrowaveComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.SetHandled(SuicideKind.Heat);
			EntityUid victim = args.Victim;
			int headCount = 0;
			BodyComponent body;
			if (base.TryComp<BodyComponent>(victim, ref body))
			{
				foreach (ValueTuple<EntityUid, BodyPartComponent> part in this._bodySystem.GetBodyChildrenOfType(new EntityUid?(victim), BodyPartType.Head, body))
				{
					if (this._bodySystem.OrphanPart(new EntityUid?(part.Item1), part.Item2))
					{
						component.Storage.Insert(part.Item1, null, null, null, null, null);
						headCount++;
					}
				}
			}
			string othersMessage = (headCount > 1) ? Loc.GetString("microwave-component-suicide-multi-head-others-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", victim)
			}) : Loc.GetString("microwave-component-suicide-others-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", victim)
			});
			string selfMessage = (headCount > 1) ? Loc.GetString("microwave-component-suicide-multi-head-message") : Loc.GetString("microwave-component-suicide-message");
			this._popupSystem.PopupEntity(othersMessage, victim, Filter.PvsExcept(victim, 2f, null), true, PopupType.Small);
			this._popupSystem.PopupEntity(selfMessage, victim, victim, PopupType.Small);
			this._audio.PlayPvs(component.ClickSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			component.CurrentCookTimerTime = 10U;
			this.Wzhzhzh(uid, component, new EntityUid?(args.Victim));
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0007264C File Offset: 0x0007084C
		private void OnSolutionChange(EntityUid uid, MicrowaveComponent component, SolutionChangedEvent args)
		{
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x00072658 File Offset: 0x00070858
		private void OnInteractUsing(EntityUid uid, MicrowaveComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ApcPowerReceiverComponent apc;
			if (!base.TryComp<ApcPowerReceiverComponent>(uid, ref apc) || !apc.Powered)
			{
				this._popupSystem.PopupEntity(Loc.GetString("microwave-component-interact-using-no-power"), uid, args.User, PopupType.Small);
				return;
			}
			if (component.Broken)
			{
				this._popupSystem.PopupEntity(Loc.GetString("microwave-component-interact-using-broken"), uid, args.User, PopupType.Small);
				return;
			}
			if (!base.HasComp<ItemComponent>(args.Used))
			{
				this._popupSystem.PopupEntity(Loc.GetString("microwave-component-interact-using-transfer-fail"), uid, args.User, PopupType.Small);
				return;
			}
			args.Handled = true;
			this._handsSystem.TryDropIntoContainer(args.User, args.Used, component.Storage, true, null);
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00072720 File Offset: 0x00070920
		private void OnBreak(EntityUid uid, MicrowaveComponent component, BreakageEventArgs args)
		{
			component.Broken = true;
			this.SetAppearance(component, MicrowaveVisualState.Broken);
			base.RemComp<ActiveMicrowaveComponent>(uid);
			this._sharedContainer.EmptyContainer(component.Storage, false, null, false, null);
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x0007276C File Offset: 0x0007096C
		private void OnPowerChanged(EntityUid uid, MicrowaveComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				this.SetAppearance(component, MicrowaveVisualState.Idle);
				base.RemComp<ActiveMicrowaveComponent>(uid);
				this._sharedContainer.EmptyContainer(component.Storage, false, null, false, null);
			}
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x000727B8 File Offset: 0x000709B8
		private void OnRefreshParts(EntityUid uid, MicrowaveComponent component, RefreshPartsEvent args)
		{
			float cookRating = args.PartRatings[component.MachinePartCookTimeMultiplier];
			component.CookTimeMultiplier = MathF.Pow(component.CookTimeScalingConstant, cookRating - 1f);
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x000727EF File Offset: 0x000709EF
		private void OnUpgradeExamine(EntityUid uid, MicrowaveComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("microwave-component-upgrade-cook-time", component.CookTimeMultiplier);
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x00072804 File Offset: 0x00070A04
		public void UpdateUserInterfaceState(EntityUid uid, MicrowaveComponent component)
		{
			BoundUserInterface ui = this._userInterface.GetUiOrNull(uid, MicrowaveUiKey.Key, null);
			if (ui == null)
			{
				return;
			}
			MicrowaveUpdateUserInterfaceState state = new MicrowaveUpdateUserInterfaceState(component.Storage.ContainedEntities.ToArray<EntityUid>(), base.HasComp<ActiveMicrowaveComponent>(uid), component.CurrentCookTimeButtonIndex, component.CurrentCookTimerTime);
			this._userInterface.SetUiState(ui, state, null, true);
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x00072864 File Offset: 0x00070A64
		public void SetAppearance(MicrowaveComponent component, MicrowaveVisualState state)
		{
			MicrowaveVisualState display = component.Broken ? MicrowaveVisualState.Broken : state;
			this._appearance.SetData(component.Owner, PowerDeviceVisuals.VisualState, display, null);
		}

		// Token: 0x060015BD RID: 5565 RVA: 0x0007289C File Offset: 0x00070A9C
		public bool HasContents(MicrowaveComponent component)
		{
			return component.Storage.ContainedEntities.Any<EntityUid>();
		}

		// Token: 0x060015BE RID: 5566 RVA: 0x000728B0 File Offset: 0x00070AB0
		public void Wzhzhzh(EntityUid uid, MicrowaveComponent component, EntityUid? user)
		{
			if (!this.HasContents(component) || base.HasComp<ActiveMicrowaveComponent>(uid))
			{
				return;
			}
			Dictionary<string, int> solidsDict = new Dictionary<string, int>();
			Dictionary<string, FixedPoint2> reagentDict = new Dictionary<string, FixedPoint2>();
			foreach (EntityUid item in component.Storage.ContainedEntities)
			{
				BeingMicrowavedEvent ev = new BeingMicrowavedEvent(uid, user);
				base.RaiseLocalEvent<BeingMicrowavedEvent>(item, ev, false);
				if (ev.Handled)
				{
					this.UpdateUserInterfaceState(uid, component);
					return;
				}
				if (this._tag.HasTag(item, "MicrowaveMachineUnsafe") || this._tag.HasTag(item, "Metal"))
				{
					component.Broken = true;
					this.SetAppearance(component, MicrowaveVisualState.Broken);
					this._audio.PlayPvs(component.ItemBreakSound, uid, null);
					return;
				}
				if (this._tag.HasTag(item, "MicrowaveSelfUnsafe") || this._tag.HasTag(item, "Plastic"))
				{
					EntityUid junk = base.Spawn(component.BadRecipeEntityId, base.Transform(uid).Coordinates);
					component.Storage.Insert(junk, null, null, null, null, null);
					base.QueueDel(item);
				}
				MetaDataComponent metaData = base.MetaData(item);
				if (metaData.EntityPrototype != null)
				{
					if (solidsDict.ContainsKey(metaData.EntityPrototype.ID))
					{
						Dictionary<string, int> solidsDict2 = solidsDict;
						string key = metaData.EntityPrototype.ID;
						int num = solidsDict2[key];
						solidsDict2[key] = num + 1;
					}
					else
					{
						solidsDict.Add(metaData.EntityPrototype.ID, 1);
					}
					SolutionContainerManagerComponent solMan;
					if (base.TryComp<SolutionContainerManagerComponent>(item, ref solMan))
					{
						foreach (KeyValuePair<string, Solution> keyValuePair in solMan.Solutions)
						{
							string key;
							Solution solution;
							keyValuePair.Deconstruct(out key, out solution);
							foreach (Solution.ReagentQuantity reagent in solution.Contents)
							{
								if (reagentDict.ContainsKey(reagent.ReagentId))
								{
									Dictionary<string, FixedPoint2> reagentDict2 = reagentDict;
									key = reagent.ReagentId;
									reagentDict2[key] += reagent.Quantity;
								}
								else
								{
									reagentDict.Add(reagent.ReagentId, reagent.Quantity);
								}
							}
						}
					}
				}
			}
			ValueTuple<FoodRecipePrototype, int> portionedRecipe = (from r in this._recipeManager.Recipes
			select this.CanSatisfyRecipe(component, r, solidsDict, reagentDict)).FirstOrDefault((ValueTuple<FoodRecipePrototype, int> r) => r.Item2 > 0);
			this._audio.PlayPvs(component.StartCookingSound, uid, null);
			ActiveMicrowaveComponent activeMicrowaveComponent = base.AddComp<ActiveMicrowaveComponent>(uid);
			activeMicrowaveComponent.CookTimeRemaining = component.CurrentCookTimerTime * component.CookTimeMultiplier;
			activeMicrowaveComponent.TotalTime = component.CurrentCookTimerTime;
			activeMicrowaveComponent.PortionedRecipe = portionedRecipe;
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015BF RID: 5567 RVA: 0x00072C80 File Offset: 0x00070E80
		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		public ValueTuple<FoodRecipePrototype, int> CanSatisfyRecipe(MicrowaveComponent component, FoodRecipePrototype recipe, Dictionary<string, int> solids, Dictionary<string, FixedPoint2> reagents)
		{
			int portions = 0;
			if (component.CurrentCookTimerTime % recipe.CookTime != 0U)
			{
				return new ValueTuple<FoodRecipePrototype, int>(recipe, 0);
			}
			foreach (KeyValuePair<string, FixedPoint2> solid in recipe.IngredientsSolids)
			{
				if (!solids.ContainsKey(solid.Key))
				{
					return new ValueTuple<FoodRecipePrototype, int>(recipe, 0);
				}
				if (solids[solid.Key] < solid.Value)
				{
					return new ValueTuple<FoodRecipePrototype, int>(recipe, 0);
				}
				portions = ((portions == 0) ? (solids[solid.Key] / solid.Value.Int()) : Math.Min(portions, solids[solid.Key] / solid.Value.Int()));
			}
			foreach (KeyValuePair<string, FixedPoint2> reagent in recipe.IngredientsReagents)
			{
				if (!reagents.ContainsKey(reagent.Key))
				{
					return new ValueTuple<FoodRecipePrototype, int>(recipe, 0);
				}
				if (reagents[reagent.Key] < reagent.Value)
				{
					return new ValueTuple<FoodRecipePrototype, int>(recipe, 0);
				}
				portions = ((portions == 0) ? (reagents[reagent.Key].Int() / reagent.Value.Int()) : Math.Min(portions, reagents[reagent.Key].Int() / reagent.Value.Int()));
			}
			return new ValueTuple<FoodRecipePrototype, int>(recipe, (int)Math.Min((long)portions, (long)((ulong)(component.CurrentCookTimerTime / recipe.CookTime))));
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x00072E74 File Offset: 0x00071074
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveMicrowaveComponent, MicrowaveComponent> valueTuple in this.EntityManager.EntityQuery<ActiveMicrowaveComponent, MicrowaveComponent>(false))
			{
				ActiveMicrowaveComponent active = valueTuple.Item1;
				MicrowaveComponent microwave = valueTuple.Item2;
				active.CookTimeRemaining -= frameTime;
				if (active.CookTimeRemaining <= 0f)
				{
					this.AddTemperature(microwave, active.TotalTime);
					if (active.PortionedRecipe.Item1 != null)
					{
						EntityCoordinates coords = base.Transform(microwave.Owner).Coordinates;
						for (int i = 0; i < active.PortionedRecipe.Item2; i++)
						{
							this.SubtractContents(microwave, active.PortionedRecipe.Item1);
							base.Spawn(active.PortionedRecipe.Item1.Result, coords);
						}
					}
					this._sharedContainer.EmptyContainer(microwave.Storage, false, null, false, null);
					this.UpdateUserInterfaceState(microwave.Owner, microwave);
					this.EntityManager.RemoveComponentDeferred<ActiveMicrowaveComponent>(active.Owner);
					this._audio.PlayPvs(microwave.FoodDoneSound, microwave.Owner, new AudioParams?(AudioParams.Default.WithVolume(-1f)));
				}
			}
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x00072FDC File Offset: 0x000711DC
		private void OnEjectMessage(EntityUid uid, MicrowaveComponent component, MicrowaveEjectMessage args)
		{
			if (!this.HasContents(component) || base.HasComp<ActiveMicrowaveComponent>(uid))
			{
				return;
			}
			this._sharedContainer.EmptyContainer(component.Storage, false, null, false, null);
			this._audio.PlayPvs(component.ClickSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015C2 RID: 5570 RVA: 0x00073048 File Offset: 0x00071248
		private void OnEjectIndex(EntityUid uid, MicrowaveComponent component, MicrowaveEjectSolidIndexedMessage args)
		{
			if (!this.HasContents(component) || base.HasComp<ActiveMicrowaveComponent>(uid))
			{
				return;
			}
			component.Storage.Remove(args.EntityID, null, null, null, true, false, null, null);
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x060015C3 RID: 5571 RVA: 0x0007309C File Offset: 0x0007129C
		private void OnSelectTime(EntityUid uid, MicrowaveComponent component, MicrowaveSelectCookTimeMessage args)
		{
			ApcPowerReceiverComponent apc;
			if (!this.HasContents(component) || base.HasComp<ActiveMicrowaveComponent>(uid) || !base.TryComp<ApcPowerReceiverComponent>(uid, ref apc) || !apc.Powered)
			{
				return;
			}
			component.CurrentCookTimeButtonIndex = args.ButtonIndex;
			component.CurrentCookTimerTime = args.NewCookTime;
			this._audio.PlayPvs(component.ClickSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
			this.UpdateUserInterfaceState(uid, component);
		}

		// Token: 0x04000D7C RID: 3452
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x04000D7D RID: 3453
		[Dependency]
		private readonly ContainerSystem _container;

		// Token: 0x04000D7E RID: 3454
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000D7F RID: 3455
		[Dependency]
		private readonly RecipeManager _recipeManager;

		// Token: 0x04000D80 RID: 3456
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000D81 RID: 3457
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D82 RID: 3458
		[Dependency]
		private readonly SharedContainerSystem _sharedContainer;

		// Token: 0x04000D83 RID: 3459
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainer;

		// Token: 0x04000D84 RID: 3460
		[Dependency]
		private readonly TagSystem _tag;

		// Token: 0x04000D85 RID: 3461
		[Dependency]
		private readonly TemperatureSystem _temperature;

		// Token: 0x04000D86 RID: 3462
		[Dependency]
		private readonly UserInterfaceSystem _userInterface;

		// Token: 0x04000D87 RID: 3463
		[Dependency]
		private readonly HandsSystem _handsSystem;
	}
}
