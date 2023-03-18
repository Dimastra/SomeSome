using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Labels;
using Content.Server.Popups;
using Content.Server.Storage.Components;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000696 RID: 1686
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChemMasterSystem : EntitySystem
	{
		// Token: 0x060022FC RID: 8956 RVA: 0x000B68C0 File Offset: 0x000B4AC0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ChemMasterComponent, ComponentStartup>(delegate(EntityUid _, ChemMasterComponent comp, ComponentStartup _)
			{
				this.UpdateUiState(comp, false);
			}, null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, SolutionChangedEvent>(delegate(EntityUid _, ChemMasterComponent comp, SolutionChangedEvent _)
			{
				this.UpdateUiState(comp, false);
			}, null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, EntInsertedIntoContainerMessage>(delegate(EntityUid _, ChemMasterComponent comp, EntInsertedIntoContainerMessage _)
			{
				this.UpdateUiState(comp, false);
			}, null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, EntRemovedFromContainerMessage>(delegate(EntityUid _, ChemMasterComponent comp, EntRemovedFromContainerMessage _)
			{
				this.UpdateUiState(comp, false);
			}, null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, BoundUIOpenedEvent>(delegate(EntityUid _, ChemMasterComponent comp, BoundUIOpenedEvent _)
			{
				this.UpdateUiState(comp, false);
			}, null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetModeMessage>(new ComponentEventHandler<ChemMasterComponent, ChemMasterSetModeMessage>(this.OnSetModeMessage), null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetPillTypeMessage>(new ComponentEventHandler<ChemMasterComponent, ChemMasterSetPillTypeMessage>(this.OnSetPillTypeMessage), null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, ChemMasterReagentAmountButtonMessage>(new ComponentEventHandler<ChemMasterComponent, ChemMasterReagentAmountButtonMessage>(this.OnReagentButtonMessage), null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, ChemMasterCreatePillsMessage>(new ComponentEventHandler<ChemMasterComponent, ChemMasterCreatePillsMessage>(this.OnCreatePillsMessage), null, null);
			base.SubscribeLocalEvent<ChemMasterComponent, ChemMasterOutputToBottleMessage>(new ComponentEventHandler<ChemMasterComponent, ChemMasterOutputToBottleMessage>(this.OnOutputToBottleMessage), null, null);
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x000B699C File Offset: 0x000B4B9C
		private void UpdateUiState(ChemMasterComponent chemMaster, bool updateLabel = false)
		{
			Solution bufferSolution;
			if (!this._solutionContainerSystem.TryGetSolution(chemMaster.Owner, "buffer", out bufferSolution, null))
			{
				return;
			}
			EntityUid? inputContainer = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "beakerSlot", null);
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "outputSlot", null);
			List<Solution.ReagentQuantity> bufferReagents = bufferSolution.Contents;
			FixedPoint2 bufferCurrentVolume = bufferSolution.Volume;
			ChemMasterBoundUserInterfaceState state = new ChemMasterBoundUserInterfaceState(chemMaster.Mode, this.BuildInputContainerInfo(inputContainer), this.BuildOutputContainerInfo(outputContainer), bufferReagents, bufferCurrentVolume, chemMaster.PillType, chemMaster.PillDosageLimit, updateLabel);
			this._userInterfaceSystem.TrySetUiState(chemMaster.Owner, ChemMasterUiKey.Key, state, null, null, true);
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x000B6A4C File Offset: 0x000B4C4C
		private void OnSetModeMessage(EntityUid uid, ChemMasterComponent chemMaster, ChemMasterSetModeMessage message)
		{
			if (!Enum.IsDefined(typeof(ChemMasterMode), message.ChemMasterMode))
			{
				return;
			}
			chemMaster.Mode = message.ChemMasterMode;
			this.UpdateUiState(chemMaster, false);
			this.ClickSound(chemMaster);
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000B6A86 File Offset: 0x000B4C86
		private void OnSetPillTypeMessage(EntityUid uid, ChemMasterComponent chemMaster, ChemMasterSetPillTypeMessage message)
		{
			if (message.PillType > 19U)
			{
				return;
			}
			chemMaster.PillType = message.PillType;
			this.UpdateUiState(chemMaster, false);
			this.ClickSound(chemMaster);
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x000B6AB0 File Offset: 0x000B4CB0
		private void OnReagentButtonMessage(EntityUid uid, ChemMasterComponent chemMaster, ChemMasterReagentAmountButtonMessage message)
		{
			if (!Enum.IsDefined(typeof(ChemMasterReagentAmount), message.Amount))
			{
				return;
			}
			ChemMasterMode mode = chemMaster.Mode;
			if (mode != ChemMasterMode.Transfer)
			{
				if (mode != ChemMasterMode.Discard)
				{
					return;
				}
				this.DiscardReagents(chemMaster, message.ReagentId, message.Amount.GetFixedPoint(), message.FromBuffer);
			}
			else
			{
				this.TransferReagents(chemMaster, message.ReagentId, message.Amount.GetFixedPoint(), message.FromBuffer);
			}
			this.ClickSound(chemMaster);
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x000B6B30 File Offset: 0x000B4D30
		private void TransferReagents(ChemMasterComponent chemMaster, string reagentId, FixedPoint2 amount, bool fromBuffer)
		{
			EntityUid? container = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "beakerSlot", null);
			Solution containerSolution;
			Solution bufferSolution;
			if (container == null || !this._solutionContainerSystem.TryGetFitsInDispenser(container.Value, out containerSolution, null, null) || !this._solutionContainerSystem.TryGetSolution(chemMaster.Owner, "buffer", out bufferSolution, null))
			{
				return;
			}
			if (fromBuffer)
			{
				amount = FixedPoint2.Min(amount, containerSolution.AvailableVolume);
				amount = bufferSolution.RemoveReagent(reagentId, amount);
				FixedPoint2 fixedPoint;
				this._solutionContainerSystem.TryAddReagent(container.Value, containerSolution, reagentId, amount, out fixedPoint, null);
			}
			else
			{
				amount = FixedPoint2.Min(amount, containerSolution.GetReagentQuantity(reagentId));
				this._solutionContainerSystem.TryRemoveReagent(container.Value, containerSolution, reagentId, amount);
				bufferSolution.AddReagent(reagentId, amount, true);
			}
			this.UpdateUiState(chemMaster, true);
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x000B6C08 File Offset: 0x000B4E08
		private void DiscardReagents(ChemMasterComponent chemMaster, string reagentId, FixedPoint2 amount, bool fromBuffer)
		{
			if (fromBuffer)
			{
				Solution bufferSolution;
				if (!this._solutionContainerSystem.TryGetSolution(chemMaster.Owner, "buffer", out bufferSolution, null))
				{
					return;
				}
				bufferSolution.RemoveReagent(reagentId, amount);
			}
			else
			{
				EntityUid? container = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "beakerSlot", null);
				Solution containerSolution;
				if (container == null || !this._solutionContainerSystem.TryGetFitsInDispenser(container.Value, out containerSolution, null, null))
				{
					return;
				}
				this._solutionContainerSystem.TryRemoveReagent(container.Value, containerSolution, reagentId, amount);
			}
			this.UpdateUiState(chemMaster, fromBuffer);
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x000B6C9C File Offset: 0x000B4E9C
		private void OnCreatePillsMessage(EntityUid uid, ChemMasterComponent chemMaster, ChemMasterCreatePillsMessage message)
		{
			EntityUid? user = message.Session.AttachedEntity;
			EntityUid? maybeContainer = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "outputSlot", null);
			if (maybeContainer != null)
			{
				EntityUid container = maybeContainer.GetValueOrDefault();
				ServerStorageComponent storage;
				if (container.Valid && base.TryComp<ServerStorageComponent>(container, ref storage) && storage.Storage != null)
				{
					if (message.Number == 0U || (ulong)message.Number > (ulong)((long)(storage.StorageCapacityMax - storage.StorageUsed)))
					{
						return;
					}
					if (message.Dosage == 0U || message.Dosage > chemMaster.PillDosageLimit)
					{
						return;
					}
					if ((long)message.Label.Length > 50L)
					{
						return;
					}
					uint needed = message.Dosage * message.Number;
					Solution withdrawal;
					if (!this.WithdrawFromBuffer(chemMaster, needed, user, out withdrawal))
					{
						return;
					}
					this._labelSystem.Label(container, message.Label, null, null);
					int i = 0;
					while ((long)i < (long)((ulong)message.Number))
					{
						EntityUid item = base.Spawn("Pill", base.Transform(container).Coordinates);
						this._storageSystem.Insert(container, item, storage, true);
						this._labelSystem.Label(item, message.Label, null, null);
						Solution itemSolution = this._solutionContainerSystem.EnsureSolution(item, "food", null);
						this._solutionContainerSystem.TryAddSolution(item, itemSolution, withdrawal.SplitSolution(message.Dosage));
						SpriteComponent spriteComp;
						if (base.TryComp<SpriteComponent>(item, ref spriteComp))
						{
							spriteComp.LayerSetState(0, "pill" + (chemMaster.PillType + 1U).ToString());
						}
						if (user != null)
						{
							ISharedAdminLogManager adminLogger = this._adminLogger;
							LogType type = LogType.Action;
							LogImpact impact = LogImpact.Low;
							LogStringHandler logStringHandler = new LogStringHandler(10, 3);
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
							logStringHandler.AppendLiteral(" printed ");
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "pill", "ToPrettyString(item)");
							logStringHandler.AppendLiteral(" ");
							logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(itemSolution));
							adminLogger.Add(type, impact, ref logStringHandler);
						}
						else
						{
							ISharedAdminLogManager adminLogger2 = this._adminLogger;
							LogType type2 = LogType.Action;
							LogImpact impact2 = LogImpact.Low;
							LogStringHandler logStringHandler = new LogStringHandler(17, 2);
							logStringHandler.AppendLiteral("Unknown printed ");
							logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(item), "pill", "ToPrettyString(item)");
							logStringHandler.AppendLiteral(" ");
							logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(itemSolution));
							adminLogger2.Add(type2, impact2, ref logStringHandler);
						}
						i++;
					}
					this.UpdateUiState(chemMaster, false);
					this.ClickSound(chemMaster);
					return;
				}
			}
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x000B6F2C File Offset: 0x000B512C
		private void OnOutputToBottleMessage(EntityUid uid, ChemMasterComponent chemMaster, ChemMasterOutputToBottleMessage message)
		{
			EntityUid? user = message.Session.AttachedEntity;
			EntityUid? maybeContainer = this._itemSlotsSystem.GetItemOrNull(chemMaster.Owner, "outputSlot", null);
			if (maybeContainer != null)
			{
				EntityUid container = maybeContainer.GetValueOrDefault();
				Solution solution;
				if (container.Valid && this._solutionContainerSystem.TryGetSolution(container, "drink", out solution, null))
				{
					if (message.Dosage == 0U || message.Dosage > solution.AvailableVolume)
					{
						return;
					}
					if ((long)message.Label.Length > 50L)
					{
						return;
					}
					Solution withdrawal;
					if (!this.WithdrawFromBuffer(chemMaster, message.Dosage, user, out withdrawal))
					{
						return;
					}
					this._labelSystem.Label(container, message.Label, null, null);
					this._solutionContainerSystem.TryAddSolution(container, solution, withdrawal);
					if (user != null)
					{
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Action;
						LogImpact impact = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(10, 3);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(user.Value), "user", "ToPrettyString(user.Value)");
						logStringHandler.AppendLiteral(" bottled ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(container), "bottle", "ToPrettyString(container)");
						logStringHandler.AppendLiteral(" ");
						logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(solution));
						adminLogger.Add(type, impact, ref logStringHandler);
					}
					else
					{
						ISharedAdminLogManager adminLogger2 = this._adminLogger;
						LogType type2 = LogType.Action;
						LogImpact impact2 = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(17, 2);
						logStringHandler.AppendLiteral("Unknown bottled ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(container), "bottle", "ToPrettyString(container)");
						logStringHandler.AppendLiteral(" ");
						logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(solution));
						adminLogger2.Add(type2, impact2, ref logStringHandler);
					}
					this.UpdateUiState(chemMaster, false);
					this.ClickSound(chemMaster);
					return;
				}
			}
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x000B70E8 File Offset: 0x000B52E8
		private bool WithdrawFromBuffer(IComponent chemMaster, FixedPoint2 neededVolume, EntityUid? user, [Nullable(2)] [NotNullWhen(true)] out Solution outputSolution)
		{
			outputSolution = null;
			Solution solution;
			if (!this._solutionContainerSystem.TryGetSolution(chemMaster.Owner, "buffer", out solution, null))
			{
				return false;
			}
			if (solution.Volume == 0)
			{
				if (user != null)
				{
					this._popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-empty-text"), user.Value, PopupType.Small);
				}
				return false;
			}
			if (neededVolume > solution.Volume)
			{
				if (user != null)
				{
					this._popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-low-text"), user.Value, PopupType.Small);
				}
				return false;
			}
			outputSolution = solution.SplitSolution(neededVolume);
			return true;
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x000B718D File Offset: 0x000B538D
		private void ClickSound(ChemMasterComponent chemMaster)
		{
			this._audioSystem.PlayPvs(chemMaster.ClickSound, chemMaster.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x000B71BC File Offset: 0x000B53BC
		[NullableContext(2)]
		private ContainerInfo BuildInputContainerInfo(EntityUid? container)
		{
			if (container == null || !container.GetValueOrDefault().Valid)
			{
				return null;
			}
			FitsInDispenserComponent fits;
			Solution solution;
			if (!base.TryComp<FitsInDispenserComponent>(container, ref fits) || !this._solutionContainerSystem.TryGetSolution(container.Value, fits.Solution, out solution, null))
			{
				return null;
			}
			return ChemMasterSystem.BuildContainerInfo(base.Name(container.Value, null), solution);
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x000B7224 File Offset: 0x000B5424
		[NullableContext(2)]
		private ContainerInfo BuildOutputContainerInfo(EntityUid? container)
		{
			if (container == null || !container.GetValueOrDefault().Valid)
			{
				return null;
			}
			string name = base.Name(container.Value, null);
			Solution solution;
			if (this._solutionContainerSystem.TryGetSolution(container.Value, "drink", out solution, null))
			{
				return ChemMasterSystem.BuildContainerInfo(name, solution);
			}
			ServerStorageComponent storage;
			if (!base.TryComp<ServerStorageComponent>(container, ref storage))
			{
				return null;
			}
			Container storage2 = storage.Storage;
			List<ValueTuple<string, FixedPoint2>> pills = (storage2 != null) ? storage2.ContainedEntities.Select(delegate(EntityUid pill)
			{
				Solution solution2;
				this._solutionContainerSystem.TryGetSolution(pill, "food", out solution2, null);
				FixedPoint2 quantity = (solution2 != null) ? solution2.Volume : FixedPoint2.Zero;
				return new ValueTuple<string, FixedPoint2>(base.Name(pill, null), quantity);
			}).ToList<ValueTuple<string, FixedPoint2>>() : null;
			if (pills != null)
			{
				return new ContainerInfo(name, false, storage.StorageUsed, storage.StorageCapacityMax, pills);
			}
			return null;
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x000B72DC File Offset: 0x000B54DC
		private static ContainerInfo BuildContainerInfo(string name, Solution solution)
		{
			List<ValueTuple<string, FixedPoint2>> reagents = (from reagent in solution.Contents
			select new ValueTuple<string, FixedPoint2>(reagent.ReagentId, reagent.Quantity)).ToList<ValueTuple<string, FixedPoint2>>();
			return new ContainerInfo(name, true, solution.Volume, solution.MaxVolume, reagents);
		}

		// Token: 0x040015A9 RID: 5545
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040015AA RID: 5546
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x040015AB RID: 5547
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040015AC RID: 5548
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040015AD RID: 5549
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040015AE RID: 5550
		[Dependency]
		private readonly StorageSystem _storageSystem;

		// Token: 0x040015AF RID: 5551
		[Dependency]
		private readonly LabelSystem _labelSystem;

		// Token: 0x040015B0 RID: 5552
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040015B1 RID: 5553
		private const string PillPrototypeId = "Pill";
	}
}
