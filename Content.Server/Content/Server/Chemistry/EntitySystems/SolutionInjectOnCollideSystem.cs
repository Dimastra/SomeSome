using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069D RID: 1693
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SolutionInjectOnCollideSystem : EntitySystem
	{
		// Token: 0x06002356 RID: 9046 RVA: 0x000B87C8 File Offset: 0x000B69C8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SolutionInjectOnCollideComponent, ComponentInit>(new ComponentEventHandler<SolutionInjectOnCollideComponent, ComponentInit>(this.HandleInit), null, null);
			base.SubscribeLocalEvent<SolutionInjectOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<SolutionInjectOnCollideComponent, StartCollideEvent>(this.HandleInjection), null, null);
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x000B87F8 File Offset: 0x000B69F8
		private void HandleInit(EntityUid uid, SolutionInjectOnCollideComponent component, ComponentInit args)
		{
			EntityUid owner = component.Owner;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
			defaultInterpolatedStringHandler.AppendFormatted("SolutionInjectOnCollideComponent");
			defaultInterpolatedStringHandler.AppendLiteral(" requires a SolutionContainerManager on ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(component.Owner);
			defaultInterpolatedStringHandler.AppendLiteral("!");
			ComponentExt.EnsureComponentWarn<SolutionContainerManagerComponent>(owner, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x000B8854 File Offset: 0x000B6A54
		private void HandleInjection(EntityUid uid, SolutionInjectOnCollideComponent component, ref StartCollideEvent args)
		{
			EntityUid target = args.OtherFixture.Body.Owner;
			BloodstreamComponent bloodstream;
			Solution solution;
			if (!args.OtherFixture.Body.Hard || !this.EntityManager.TryGetComponent<BloodstreamComponent>(target, ref bloodstream) || !this._solutionsSystem.TryGetInjectableSolution(component.Owner, out solution, null, null))
			{
				return;
			}
			InventoryComponent inventory;
			if (component.BlockSlots != SlotFlags.NONE && base.TryComp<InventoryComponent>(target, ref inventory))
			{
				InventorySystem.ContainerSlotEnumerator containerEnumerator = new InventorySystem.ContainerSlotEnumerator(target, inventory.TemplateId, this._protoManager, this._inventorySystem, component.BlockSlots);
				ContainerSlot container;
				while (containerEnumerator.MoveNext(out container))
				{
					if (container.ContainedEntity != null)
					{
						return;
					}
				}
			}
			Solution solution2 = solution.SplitSolution(component.TransferAmount);
			FixedPoint2 solRemovedVol = solution2.Volume;
			Solution solToInject = solution2.SplitSolution(solRemovedVol * component.TransferEfficiency);
			this._bloodstreamSystem.TryAddToChemicals(target, solToInject, bloodstream);
		}

		// Token: 0x040015BF RID: 5567
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x040015C0 RID: 5568
		[Dependency]
		private readonly SolutionContainerSystem _solutionsSystem;

		// Token: 0x040015C1 RID: 5569
		[Dependency]
		private readonly BloodstreamSystem _bloodstreamSystem;

		// Token: 0x040015C2 RID: 5570
		[Dependency]
		private readonly InventorySystem _inventorySystem;
	}
}
