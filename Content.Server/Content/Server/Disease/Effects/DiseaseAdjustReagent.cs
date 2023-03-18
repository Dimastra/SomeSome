using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Disease;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Disease.Effects
{
	// Token: 0x02000567 RID: 1383
	public sealed class DiseaseAdjustReagent : DiseaseEffect
	{
		// Token: 0x06001D4D RID: 7501 RVA: 0x0009C42C File Offset: 0x0009A62C
		public override void Effect(DiseaseEffectArgs args)
		{
			BloodstreamComponent bloodstream;
			if (!args.EntityManager.TryGetComponent<BloodstreamComponent>(args.DiseasedEntity, ref bloodstream))
			{
				return;
			}
			Solution stream = bloodstream.ChemicalSolution;
			if (stream == null)
			{
				return;
			}
			SolutionContainerSystem solutionSys = args.EntityManager.EntitySysManager.GetEntitySystem<SolutionContainerSystem>();
			if (this.Reagent == null)
			{
				return;
			}
			if (this.Amount < 0 && stream.ContainsReagent(this.Reagent))
			{
				solutionSys.TryRemoveReagent(args.DiseasedEntity, stream, this.Reagent, -this.Amount);
			}
			if (this.Amount > 0)
			{
				FixedPoint2 fixedPoint;
				solutionSys.TryAddReagent(args.DiseasedEntity, stream, this.Reagent, this.Amount, out fixedPoint, null);
			}
		}

		// Token: 0x040012B6 RID: 4790
		[Nullable(2)]
		[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string Reagent;

		// Token: 0x040012B7 RID: 4791
		[DataField("amount", false, 1, true, false, null)]
		public FixedPoint2 Amount;
	}
}
