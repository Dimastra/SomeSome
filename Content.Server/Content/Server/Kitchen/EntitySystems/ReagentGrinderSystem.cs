using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Construction;
using Content.Server.Kitchen.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Stack;
using Content.Shared.Chemistry.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Kitchen;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Stacks;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Kitchen.EntitySystems
{
	// Token: 0x02000430 RID: 1072
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ReagentGrinderSystem : EntitySystem
	{
		// Token: 0x060015C6 RID: 5574 RVA: 0x00073134 File Offset: 0x00071334
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ReagentGrinderComponent, ComponentStartup>(delegate(EntityUid uid, ReagentGrinderComponent _, ComponentStartup _)
			{
				this.UpdateUiState(uid);
			}, null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, PowerChangedEvent>(delegate(EntityUid uid, ReagentGrinderComponent _, ref PowerChangedEvent _)
			{
				this.UpdateUiState(uid);
			}, null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, InteractUsingEvent>(new ComponentEventHandler<ReagentGrinderComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, RefreshPartsEvent>(new ComponentEventHandler<ReagentGrinderComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, UpgradeExamineEvent>(new ComponentEventHandler<ReagentGrinderComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ReagentGrinderComponent, EntInsertedIntoContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ReagentGrinderComponent, EntRemovedFromContainerMessage>(this.OnContainerModified), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, ContainerIsRemovingAttemptEvent>(new ComponentEventHandler<ReagentGrinderComponent, ContainerIsRemovingAttemptEvent>(this.OnEntRemoveAttempt), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, ReagentGrinderStartMessage>(new ComponentEventHandler<ReagentGrinderComponent, ReagentGrinderStartMessage>(this.OnStartMessage), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, ReagentGrinderEjectChamberAllMessage>(new ComponentEventHandler<ReagentGrinderComponent, ReagentGrinderEjectChamberAllMessage>(this.OnEjectChamberAllMessage), null, null);
			base.SubscribeLocalEvent<ReagentGrinderComponent, ReagentGrinderEjectChamberContentMessage>(new ComponentEventHandler<ReagentGrinderComponent, ReagentGrinderEjectChamberContentMessage>(this.OnEjectChamberContentMessage), null, null);
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x00073224 File Offset: 0x00071424
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveReagentGrinderComponent, ReagentGrinderComponent> valueTuple in base.EntityQuery<ActiveReagentGrinderComponent, ReagentGrinderComponent>(false))
			{
				ActiveReagentGrinderComponent active = valueTuple.Item1;
				ReagentGrinderComponent reagentGrinder = valueTuple.Item2;
				EntityUid uid = reagentGrinder.Owner;
				if (!(active.EndTime > this._timing.CurTime))
				{
					IPlayingAudioStream audioStream = reagentGrinder.AudioStream;
					if (audioStream != null)
					{
						audioStream.Stop();
					}
					base.RemCompDeferred<ActiveReagentGrinderComponent>(uid);
					Container inputContainer = this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null);
					EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(uid, SharedReagentGrinder.BeakerSlotId, null);
					Solution containerSolution;
					if (outputContainer != null && this._solutionsSystem.TryGetFitsInDispenser(outputContainer.Value, out containerSolution, null, null))
					{
						foreach (EntityUid item in inputContainer.ContainedEntities.ToList<EntityUid>())
						{
							GrinderProgram program = active.Program;
							Solution solution2;
							if (program != GrinderProgram.Grind)
							{
								if (program != GrinderProgram.Juice)
								{
									solution2 = null;
								}
								else
								{
									ExtractableComponent extractableComponent = base.CompOrNull<ExtractableComponent>(item);
									solution2 = ((extractableComponent != null) ? extractableComponent.JuiceSolution : null);
								}
							}
							else
							{
								solution2 = this.GetGrindSolution(item);
							}
							Solution solution = solution2;
							if (solution != null)
							{
								StackComponent stack;
								if (base.TryComp<StackComponent>(item, ref stack))
								{
									FixedPoint2 totalVolume = solution.Volume * stack.Count;
									if (totalVolume <= 0)
									{
										continue;
									}
									int fitsCount = (int)(stack.Count * FixedPoint2.Min(containerSolution.AvailableVolume / totalVolume + 0.01, 1));
									if (fitsCount <= 0)
									{
										continue;
									}
									solution.ScaleSolution(fitsCount);
									this._stackSystem.SetCount(item, stack.Count - fitsCount, null);
								}
								else
								{
									if (solution.Volume > containerSolution.AvailableVolume)
									{
										continue;
									}
									base.QueueDel(item);
								}
								this._solutionsSystem.TryAddSolution(outputContainer.Value, containerSolution, solution);
							}
						}
						this._userInterfaceSystem.TrySendUiMessage(uid, ReagentGrinderUiKey.Key, new ReagentGrinderWorkCompleteMessage(), null);
						this.UpdateUiState(uid);
					}
				}
			}
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000734A8 File Offset: 0x000716A8
		private void OnEntRemoveAttempt(EntityUid uid, ReagentGrinderComponent reagentGrinder, ContainerIsRemovingAttemptEvent args)
		{
			if (base.HasComp<ActiveReagentGrinderComponent>(uid))
			{
				args.Cancel();
			}
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000734BC File Offset: 0x000716BC
		private void OnContainerModified(EntityUid uid, ReagentGrinderComponent reagentGrinder, ContainerModifiedMessage args)
		{
			this.UpdateUiState(uid);
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(uid, SharedReagentGrinder.BeakerSlotId, null);
			this._appearanceSystem.SetData(uid, ReagentGrinderVisualState.BeakerAttached, outputContainer != null, null);
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x00073504 File Offset: 0x00071704
		private void OnInteractUsing(EntityUid uid, ReagentGrinderComponent reagentGrinder, InteractUsingEvent args)
		{
			EntityUid heldEnt = args.Used;
			Container inputContainer = this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null);
			if (!base.HasComp<ExtractableComponent>(heldEnt))
			{
				if (!base.HasComp<FitsInDispenserComponent>(heldEnt))
				{
					this._popupSystem.PopupEntity(Loc.GetString("reagent-grinder-component-cannot-put-entity-message"), uid, args.User, PopupType.Small);
				}
				return;
			}
			if (args.Handled)
			{
				return;
			}
			if (inputContainer.ContainedEntities.Count >= reagentGrinder.StorageMaxEntities)
			{
				return;
			}
			if (!inputContainer.Insert(heldEnt, this.EntityManager, null, null, null, null))
			{
				return;
			}
			args.Handled = true;
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x00073594 File Offset: 0x00071794
		private void OnRefreshParts(EntityUid uid, ReagentGrinderComponent component, RefreshPartsEvent args)
		{
			float ratingWorkTime = args.PartRatings[component.MachinePartWorkTime];
			float ratingStorage = args.PartRatings[component.MachinePartStorageMax];
			component.WorkTimeMultiplier = MathF.Pow(component.PartRatingWorkTimerMulitplier, ratingWorkTime - 1f);
			component.StorageMaxEntities = component.BaseStorageMaxEntities + (int)((float)component.StoragePerPartRating * (ratingStorage - 1f));
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x000735FA File Offset: 0x000717FA
		private void OnUpgradeExamine(EntityUid uid, ReagentGrinderComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("reagent-grinder-component-upgrade-work-time", component.WorkTimeMultiplier);
			args.AddNumberUpgrade("reagent-grinder-component-upgrade-storage", component.StorageMaxEntities - component.BaseStorageMaxEntities);
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x00073628 File Offset: 0x00071828
		private void UpdateUiState(EntityUid uid)
		{
			Container inputContainer = this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null);
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(uid, SharedReagentGrinder.BeakerSlotId, null);
			Solution containerSolution = null;
			bool isBusy = base.HasComp<ActiveReagentGrinderComponent>(uid);
			bool canJuice = false;
			bool canGrind = false;
			if (outputContainer != null && this._solutionsSystem.TryGetFitsInDispenser(outputContainer.Value, out containerSolution, null, null) && inputContainer.ContainedEntities.Count > 0)
			{
				canGrind = inputContainer.ContainedEntities.All(new Func<EntityUid, bool>(this.CanGrind));
				canJuice = inputContainer.ContainedEntities.All(new Func<EntityUid, bool>(this.CanJuice));
			}
			ReagentGrinderInterfaceState state = new ReagentGrinderInterfaceState(isBusy, outputContainer != null, this.IsPowered(uid, this.EntityManager, null), canJuice, canGrind, (from item in inputContainer.ContainedEntities
			select item).ToArray<EntityUid>(), (containerSolution != null) ? containerSolution.Contents.ToArray() : null);
			this._userInterfaceSystem.TrySetUiState(uid, ReagentGrinderUiKey.Key, state, null, null, true);
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x00073740 File Offset: 0x00071940
		private void OnStartMessage(EntityUid uid, ReagentGrinderComponent reagentGrinder, ReagentGrinderStartMessage message)
		{
			if (!this.IsPowered(uid, this.EntityManager, null) || base.HasComp<ActiveReagentGrinderComponent>(uid))
			{
				return;
			}
			this.DoWork(uid, reagentGrinder, message.Program);
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x0007376C File Offset: 0x0007196C
		private void OnEjectChamberAllMessage(EntityUid uid, ReagentGrinderComponent reagentGrinder, ReagentGrinderEjectChamberAllMessage message)
		{
			Container inputContainer = this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null);
			if (base.HasComp<ActiveReagentGrinderComponent>(uid) || inputContainer.ContainedEntities.Count <= 0)
			{
				return;
			}
			this.ClickSound(uid, reagentGrinder);
			foreach (EntityUid entity in inputContainer.ContainedEntities.ToList<EntityUid>())
			{
				inputContainer.Remove(entity, null, null, null, true, false, null, null);
				entity.RandomOffset(0.4f);
			}
			this.UpdateUiState(uid);
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x00073824 File Offset: 0x00071A24
		private void OnEjectChamberContentMessage(EntityUid uid, ReagentGrinderComponent reagentGrinder, ReagentGrinderEjectChamberContentMessage message)
		{
			if (base.HasComp<ActiveReagentGrinderComponent>(uid))
			{
				return;
			}
			if (this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null).Remove(message.EntityId, null, null, null, true, false, null, null))
			{
				message.EntityId.RandomOffset(0.4f);
				this.ClickSound(uid, reagentGrinder);
				this.UpdateUiState(uid);
			}
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x00073890 File Offset: 0x00071A90
		private void DoWork(EntityUid uid, ReagentGrinderComponent reagentGrinder, GrinderProgram program)
		{
			Container inputContainer = this._containerSystem.EnsureContainer<Container>(uid, SharedReagentGrinder.InputContainerId, null);
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(uid, SharedReagentGrinder.BeakerSlotId, null);
			if (inputContainer.ContainedEntities.Count <= 0 || !base.HasComp<FitsInDispenserComponent>(outputContainer))
			{
				return;
			}
			SoundSpecifier sound;
			if (program != GrinderProgram.Grind)
			{
				if (program != GrinderProgram.Juice)
				{
					return;
				}
				if (inputContainer.ContainedEntities.All(new Func<EntityUid, bool>(this.CanJuice)))
				{
					sound = reagentGrinder.JuiceSound;
					goto IL_8D;
				}
			}
			else if (inputContainer.ContainedEntities.All(new Func<EntityUid, bool>(this.CanGrind)))
			{
				sound = reagentGrinder.GrindSound;
				goto IL_8D;
			}
			return;
			IL_8D:
			ActiveReagentGrinderComponent activeReagentGrinderComponent = base.AddComp<ActiveReagentGrinderComponent>(uid);
			activeReagentGrinderComponent.EndTime = this._timing.CurTime + reagentGrinder.WorkTime * (double)reagentGrinder.WorkTimeMultiplier;
			activeReagentGrinderComponent.Program = program;
			reagentGrinder.AudioStream = this._audioSystem.PlayPvs(sound, uid, new AudioParams?(AudioParams.Default.WithPitchScale(1f / reagentGrinder.WorkTimeMultiplier)));
			this._userInterfaceSystem.TrySendUiMessage(uid, ReagentGrinderUiKey.Key, new ReagentGrinderWorkStartedMessage(program), null);
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x000739A7 File Offset: 0x00071BA7
		private void ClickSound(EntityUid uid, ReagentGrinderComponent reagentGrinder)
		{
			this._audioSystem.PlayPvs(reagentGrinder.ClickSound, uid, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x000739D0 File Offset: 0x00071BD0
		[NullableContext(2)]
		private Solution GetGrindSolution(EntityUid uid)
		{
			ExtractableComponent extractable;
			Solution solution;
			if (base.TryComp<ExtractableComponent>(uid, ref extractable) && extractable.GrindableSolution != null && this._solutionsSystem.TryGetSolution(uid, extractable.GrindableSolution, out solution, null))
			{
				return solution;
			}
			return null;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x00073A0C File Offset: 0x00071C0C
		private bool CanGrind(EntityUid uid)
		{
			ExtractableComponent extractableComponent = base.CompOrNull<ExtractableComponent>(uid);
			string solutionName = (extractableComponent != null) ? extractableComponent.GrindableSolution : null;
			Solution solution;
			return solutionName != null && this._solutionsSystem.TryGetSolution(uid, solutionName, out solution, null);
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00073A42 File Offset: 0x00071C42
		private bool CanJuice(EntityUid uid)
		{
			ExtractableComponent extractableComponent = base.CompOrNull<ExtractableComponent>(uid);
			return ((extractableComponent != null) ? extractableComponent.JuiceSolution : null) != null;
		}

		// Token: 0x04000D88 RID: 3464
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000D89 RID: 3465
		[Dependency]
		private readonly SolutionContainerSystem _solutionsSystem;

		// Token: 0x04000D8A RID: 3466
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x04000D8B RID: 3467
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04000D8C RID: 3468
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04000D8D RID: 3469
		[Dependency]
		private readonly StackSystem _stackSystem;

		// Token: 0x04000D8E RID: 3470
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000D8F RID: 3471
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x04000D90 RID: 3472
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
