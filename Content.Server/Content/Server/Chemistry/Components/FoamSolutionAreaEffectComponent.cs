using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Foam;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006A5 RID: 1701
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SolutionAreaEffectComponent))]
	public sealed class FoamSolutionAreaEffectComponent : SolutionAreaEffectComponent
	{
		// Token: 0x06002376 RID: 9078 RVA: 0x000B96D0 File Offset: 0x000B78D0
		protected override void UpdateVisuals()
		{
			AppearanceComponent appearance;
			Solution solution;
			if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance) && EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(base.Owner, "solutionArea", out solution, null))
			{
				appearance.SetData(FoamVisuals.Color, solution.GetColor(this._proto).WithAlpha(0.8f));
			}
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x000B9738 File Offset: 0x000B7938
		protected override void ReactWithEntity(EntityUid entity, double solutionFraction)
		{
			Solution solution;
			if (!EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(base.Owner, "solutionArea", out solution, null))
			{
				return;
			}
			BloodstreamComponent bloodstream;
			if (!this._entMan.TryGetComponent<BloodstreamComponent>(entity, ref bloodstream))
			{
				return;
			}
			InventorySystem invSystem = EntitySystem.Get<InventorySystem>();
			float protection = 0f;
			SlotDefinition[] slotDefinitions;
			if (invSystem.TryGetSlots(entity, out slotDefinitions, null))
			{
				foreach (SlotDefinition slot in slotDefinitions)
				{
					EntityUid? entityUid;
					if (!(slot.Name == "back") && !(slot.Name == "pocket1") && !(slot.Name == "pocket2") && !(slot.Name == "id") && invSystem.TryGetSlotEntity(entity, slot.Name, out entityUid, null, null))
					{
						protection += 0.025f;
					}
				}
			}
			BloodstreamSystem bloodstreamSystem = EntitySystem.Get<BloodstreamSystem>();
			Solution solution2 = solution.Clone();
			FixedPoint2 transferAmount = FixedPoint2.Min(solution2.Volume * solutionFraction * (1f - protection), bloodstream.ChemicalSolution.AvailableVolume);
			Solution transferSolution = solution2.SplitSolution(transferAmount);
			if (bloodstreamSystem.TryAddToChemicals(entity, transferSolution, bloodstream))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(22, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(entity), "target", "_entMan.ToPrettyString(entity)");
				logStringHandler.AppendLiteral(" was affected by foam ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(transferSolution));
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x000B98AC File Offset: 0x000B7AAC
		protected override void OnKill()
		{
			if (this._entMan.Deleted(base.Owner))
			{
				return;
			}
			AppearanceComponent appearance;
			if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance))
			{
				appearance.SetData(FoamVisuals.State, true);
			}
			TimerExtensions.SpawnTimer(base.Owner, 600, delegate()
			{
				if (!string.IsNullOrEmpty(this._foamedMetalPrototype))
				{
					this._entMan.SpawnEntity(this._foamedMetalPrototype, this._entMan.GetComponent<TransformComponent>(base.Owner).Coordinates);
				}
				this._entMan.QueueDeleteEntity(base.Owner);
			}, default(CancellationToken));
		}

		// Token: 0x040015D9 RID: 5593
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040015DA RID: 5594
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040015DB RID: 5595
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040015DC RID: 5596
		public new const string SolutionName = "solutionArea";

		// Token: 0x040015DD RID: 5597
		[Nullable(2)]
		[DataField("foamedMetalPrototype", false, 1, false, false, null)]
		private string _foamedMetalPrototype;
	}
}
