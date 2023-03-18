using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Construction;
using Content.Server.Power.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Chemistry.EntitySystems
{
	// Token: 0x0200069C RID: 1692
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SolutionHeaterSystem : EntitySystem
	{
		// Token: 0x06002350 RID: 9040 RVA: 0x000B8623 File Offset: 0x000B6823
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SolutionHeaterComponent, PowerChangedEvent>(new ComponentEventRefHandler<SolutionHeaterComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<SolutionHeaterComponent, RefreshPartsEvent>(new ComponentEventHandler<SolutionHeaterComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<SolutionHeaterComponent, UpgradeExamineEvent>(new ComponentEventHandler<SolutionHeaterComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000B8661 File Offset: 0x000B6861
		private void OnPowerChanged(EntityUid uid, SolutionHeaterComponent component, ref PowerChangedEvent args)
		{
			if (args.Powered)
			{
				base.EnsureComp<ActiveSolutionHeaterComponent>(uid);
				return;
			}
			base.RemComp<ActiveSolutionHeaterComponent>(uid);
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x000B867C File Offset: 0x000B687C
		private void OnRefreshParts(EntityUid uid, SolutionHeaterComponent component, RefreshPartsEvent args)
		{
			float heatRating = args.PartRatings[component.MachinePartHeatPerSecond] - 1f;
			component.HeatMultiplier = MathF.Pow(component.PartRatingHeatMultiplier, heatRating);
		}

		// Token: 0x06002353 RID: 9043 RVA: 0x000B86B3 File Offset: 0x000B68B3
		private void OnUpgradeExamine(EntityUid uid, SolutionHeaterComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("solution-heater-upgrade-heat", component.HeatMultiplier);
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x000B86C8 File Offset: 0x000B68C8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveSolutionHeaterComponent, SolutionHeaterComponent> valueTuple in base.EntityQuery<ActiveSolutionHeaterComponent, SolutionHeaterComponent>(false))
			{
				SolutionHeaterComponent heater = valueTuple.Item2;
				EntityUid? itemOrNull = this._itemSlots.GetItemOrNull(heater.Owner, heater.BeakerSlotId, null);
				if (itemOrNull != null)
				{
					EntityUid item = itemOrNull.GetValueOrDefault();
					SolutionContainerManagerComponent solution;
					if (base.TryComp<SolutionContainerManagerComponent>(item, ref solution))
					{
						float energy = heater.HeatPerSecond * heater.HeatMultiplier * frameTime;
						foreach (Solution s in solution.Solutions.Values)
						{
							this._solution.AddThermalEnergy(solution.Owner, s, energy);
						}
					}
				}
			}
		}

		// Token: 0x040015BD RID: 5565
		[Dependency]
		private readonly ItemSlotsSystem _itemSlots;

		// Token: 0x040015BE RID: 5566
		[Dependency]
		private readonly SolutionContainerSystem _solution;
	}
}
