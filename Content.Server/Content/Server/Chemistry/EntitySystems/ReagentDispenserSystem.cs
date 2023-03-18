using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chemistry.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Dispenser;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x02000697 RID: 1687
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ReagentDispenserSystem : EntitySystem
	{
		// Token: 0x06002311 RID: 8977 RVA: 0x000B73AC File Offset: 0x000B55AC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ReagentDispenserComponent, ComponentStartup>(delegate(EntityUid _, ReagentDispenserComponent comp, ComponentStartup _)
			{
				this.UpdateUiState(comp);
			}, null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, SolutionChangedEvent>(delegate(EntityUid _, ReagentDispenserComponent comp, SolutionChangedEvent _)
			{
				this.UpdateUiState(comp);
			}, null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, EntInsertedIntoContainerMessage>(delegate(EntityUid _, ReagentDispenserComponent comp, EntInsertedIntoContainerMessage _)
			{
				this.UpdateUiState(comp);
			}, null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, EntRemovedFromContainerMessage>(delegate(EntityUid _, ReagentDispenserComponent comp, EntRemovedFromContainerMessage _)
			{
				this.UpdateUiState(comp);
			}, null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, BoundUIOpenedEvent>(delegate(EntityUid _, ReagentDispenserComponent comp, BoundUIOpenedEvent _)
			{
				this.UpdateUiState(comp);
			}, null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, GotEmaggedEvent>(new ComponentEventRefHandler<ReagentDispenserComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, ReagentDispenserSetDispenseAmountMessage>(new ComponentEventHandler<ReagentDispenserComponent, ReagentDispenserSetDispenseAmountMessage>(this.OnSetDispenseAmountMessage), null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, ReagentDispenserDispenseReagentMessage>(new ComponentEventHandler<ReagentDispenserComponent, ReagentDispenserDispenseReagentMessage>(this.OnDispenseReagentMessage), null, null);
			base.SubscribeLocalEvent<ReagentDispenserComponent, ReagentDispenserClearContainerSolutionMessage>(new ComponentEventHandler<ReagentDispenserComponent, ReagentDispenserClearContainerSolutionMessage>(this.OnClearContainerSolutionMessage), null, null);
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x000B7474 File Offset: 0x000B5674
		private void UpdateUiState(ReagentDispenserComponent reagentDispenser)
		{
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(reagentDispenser.Owner, "beakerSlot", null);
			ContainerInfo outputContainer2 = this.BuildOutputContainerInfo(outputContainer);
			List<string> inventory = this.GetInventory(reagentDispenser);
			ReagentDispenserBoundUserInterfaceState state = new ReagentDispenserBoundUserInterfaceState(outputContainer2, inventory, reagentDispenser.DispenseAmount);
			this._userInterfaceSystem.TrySetUiState(reagentDispenser.Owner, ReagentDispenserUiKey.Key, state, null, null, true);
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000B74D4 File Offset: 0x000B56D4
		[NullableContext(2)]
		private ContainerInfo BuildOutputContainerInfo(EntityUid? container)
		{
			if (container == null || !container.GetValueOrDefault().Valid)
			{
				return null;
			}
			Solution solution;
			if (this._solutionContainerSystem.TryGetFitsInDispenser(container.Value, out solution, null, null))
			{
				List<ValueTuple<string, FixedPoint2>> reagents = (from reagent in solution.Contents
				select new ValueTuple<string, FixedPoint2>(reagent.ReagentId, reagent.Quantity)).ToList<ValueTuple<string, FixedPoint2>>();
				return new ContainerInfo(base.Name(container.Value, null), true, solution.Volume, solution.MaxVolume, reagents);
			}
			return null;
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x000B7568 File Offset: 0x000B5768
		private List<string> GetInventory(ReagentDispenserComponent reagentDispenser)
		{
			List<string> inventory = new List<string>();
			ReagentDispenserInventoryPrototype packPrototype;
			if (reagentDispenser.PackPrototypeId != null && this._prototypeManager.TryIndex<ReagentDispenserInventoryPrototype>(reagentDispenser.PackPrototypeId, ref packPrototype))
			{
				inventory.AddRange(packPrototype.Inventory);
			}
			ReagentDispenserInventoryPrototype emagPackPrototype;
			if (base.HasComp<EmaggedComponent>(reagentDispenser.Owner) && reagentDispenser.EmagPackPrototypeId != null && this._prototypeManager.TryIndex<ReagentDispenserInventoryPrototype>(reagentDispenser.EmagPackPrototypeId, ref emagPackPrototype))
			{
				inventory.AddRange(emagPackPrototype.Inventory);
			}
			return inventory;
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x000B75DC File Offset: 0x000B57DC
		private void OnEmagged(EntityUid uid, ReagentDispenserComponent reagentDispenser, ref GotEmaggedEvent args)
		{
			this.EntityManager.AddComponent<EmaggedComponent>(uid);
			this.UpdateUiState(reagentDispenser);
			args.Handled = true;
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x000B75F9 File Offset: 0x000B57F9
		private void OnSetDispenseAmountMessage(EntityUid uid, ReagentDispenserComponent reagentDispenser, ReagentDispenserSetDispenseAmountMessage message)
		{
			reagentDispenser.DispenseAmount = message.ReagentDispenserDispenseAmount;
			this.UpdateUiState(reagentDispenser);
			this.ClickSound(reagentDispenser);
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x000B7618 File Offset: 0x000B5818
		private void OnDispenseReagentMessage(EntityUid uid, ReagentDispenserComponent reagentDispenser, ReagentDispenserDispenseReagentMessage message)
		{
			if (!this.GetInventory(reagentDispenser).Contains(message.ReagentId))
			{
				return;
			}
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(reagentDispenser.Owner, "beakerSlot", null);
			Solution solution;
			if (outputContainer == null || !outputContainer.GetValueOrDefault().Valid || !this._solutionContainerSystem.TryGetFitsInDispenser(outputContainer.Value, out solution, null, null))
			{
				return;
			}
			FixedPoint2 dispensedAmount;
			if (this._solutionContainerSystem.TryAddReagent(outputContainer.Value, solution, message.ReagentId, (int)reagentDispenser.DispenseAmount, out dispensedAmount, null) && message.Session.AttachedEntity != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ChemicalReaction;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(22, 4);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(message.Session.AttachedEntity.Value), "player", "ToPrettyString(message.Session.AttachedEntity.Value)");
				logStringHandler.AppendLiteral(" dispensed ");
				logStringHandler.AppendFormatted<FixedPoint2>(dispensedAmount, "dispensedAmount");
				logStringHandler.AppendLiteral("u of ");
				logStringHandler.AppendFormatted(message.ReagentId);
				logStringHandler.AppendLiteral(" into ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(outputContainer.Value), "entity", "ToPrettyString(outputContainer.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this.UpdateUiState(reagentDispenser);
			this.ClickSound(reagentDispenser);
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x000B7784 File Offset: 0x000B5984
		private void OnClearContainerSolutionMessage(EntityUid uid, ReagentDispenserComponent reagentDispenser, ReagentDispenserClearContainerSolutionMessage message)
		{
			EntityUid? outputContainer = this._itemSlotsSystem.GetItemOrNull(reagentDispenser.Owner, "beakerSlot", null);
			Solution solution;
			if (outputContainer == null || !outputContainer.GetValueOrDefault().Valid || !this._solutionContainerSystem.TryGetFitsInDispenser(outputContainer.Value, out solution, null, null))
			{
				return;
			}
			this._solutionContainerSystem.RemoveAllSolution(outputContainer.Value, solution);
			this.UpdateUiState(reagentDispenser);
			this.ClickSound(reagentDispenser);
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x000B77FD File Offset: 0x000B59FD
		private void ClickSound(ReagentDispenserComponent reagentDispenser)
		{
			this._audioSystem.PlayPvs(reagentDispenser.ClickSound, reagentDispenser.Owner, new AudioParams?(AudioParams.Default.WithVolume(-2f)));
		}

		// Token: 0x040015B2 RID: 5554
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x040015B3 RID: 5555
		[Dependency]
		private readonly SolutionContainerSystem _solutionContainerSystem;

		// Token: 0x040015B4 RID: 5556
		[Dependency]
		private readonly ItemSlotsSystem _itemSlotsSystem;

		// Token: 0x040015B5 RID: 5557
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040015B6 RID: 5558
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040015B7 RID: 5559
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
