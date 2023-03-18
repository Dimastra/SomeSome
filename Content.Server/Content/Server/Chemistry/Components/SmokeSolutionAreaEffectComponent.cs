using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Smoking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006AD RID: 1709
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SolutionAreaEffectComponent))]
	public sealed class SmokeSolutionAreaEffectComponent : SolutionAreaEffectComponent
	{
		// Token: 0x06002396 RID: 9110 RVA: 0x000B9B94 File Offset: 0x000B7D94
		protected override void UpdateVisuals()
		{
			AppearanceComponent appearance;
			Solution solution;
			if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance) && EntitySystem.Get<SolutionContainerSystem>().TryGetSolution(base.Owner, "solutionArea", out solution, null))
			{
				appearance.SetData(SmokeVisuals.Color, solution.GetColor(this._proto));
			}
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x000B9BF0 File Offset: 0x000B7DF0
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
			InternalsComponent internals;
			if (this._entMan.TryGetComponent<InternalsComponent>(entity, ref internals) && IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<InternalsSystem>().AreInternalsWorking(internals))
			{
				return;
			}
			ReactiveSystem chemistry = EntitySystem.Get<ReactiveSystem>();
			Solution solution2 = solution.Clone();
			FixedPoint2 transferAmount = FixedPoint2.Min(solution2.Volume * solutionFraction, bloodstream.ChemicalSolution.AvailableVolume);
			Solution transferSolution = solution2.SplitSolution(transferAmount);
			foreach (Solution.ReagentQuantity reagentQuantity in transferSolution.Contents.ToArray())
			{
				if (!(reagentQuantity.Quantity == FixedPoint2.Zero))
				{
					chemistry.ReactionEntity(entity, ReactionMethod.Ingestion, reagentQuantity.ReagentId, reagentQuantity.Quantity, transferSolution);
				}
			}
			if (EntitySystem.Get<BloodstreamSystem>().TryAddToChemicals(entity, transferSolution, bloodstream))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ForceFeed;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(23, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(entity), "target", "_entMan.ToPrettyString(entity)");
				logStringHandler.AppendLiteral(" was affected by smoke ");
				logStringHandler.AppendFormatted(SolutionContainerSystem.ToPrettyString(transferSolution));
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x000B9D37 File Offset: 0x000B7F37
		protected override void OnKill()
		{
			if (this._entMan.Deleted(base.Owner))
			{
				return;
			}
			this._entMan.DeleteEntity(base.Owner);
		}

		// Token: 0x040015F9 RID: 5625
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040015FA RID: 5626
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040015FB RID: 5627
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040015FC RID: 5628
		public new const string SolutionName = "solutionArea";
	}
}
