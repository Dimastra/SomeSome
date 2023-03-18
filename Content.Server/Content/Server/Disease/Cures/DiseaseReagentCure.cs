using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Disease;
using Content.Shared.FixedPoint;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Cures
{
	// Token: 0x02000572 RID: 1394
	public sealed class DiseaseReagentCure : DiseaseCure
	{
		// Token: 0x06001D62 RID: 7522 RVA: 0x0009CA64 File Offset: 0x0009AC64
		public override bool Cure(DiseaseEffectArgs args)
		{
			BloodstreamComponent bloodstream;
			if (!args.EntityManager.TryGetComponent<BloodstreamComponent>(args.DiseasedEntity, ref bloodstream))
			{
				return false;
			}
			FixedPoint2 quant = FixedPoint2.Zero;
			if (this.Reagent != null && bloodstream.ChemicalSolution.ContainsReagent(this.Reagent))
			{
				quant = bloodstream.ChemicalSolution.GetReagentQuantity(this.Reagent);
			}
			return quant >= this.Min;
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x0009CACC File Offset: 0x0009ACCC
		[NullableContext(1)]
		public override string CureText()
		{
			IPrototypeManager prototypeMan = IoCManager.Resolve<IPrototypeManager>();
			ReagentPrototype reagentProt;
			if (this.Reagent == null || !prototypeMan.TryIndex<ReagentPrototype>(this.Reagent, ref reagentProt))
			{
				return string.Empty;
			}
			return Loc.GetString("diagnoser-cure-reagent", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("units", this.Min),
				new ValueTuple<string, object>("reagent", reagentProt.LocalizedName)
			});
		}

		// Token: 0x040012D4 RID: 4820
		[DataField("min", false, 1, false, false, null)]
		public FixedPoint2 Min = 5;

		// Token: 0x040012D5 RID: 4821
		[Nullable(2)]
		[DataField("reagent", false, 1, false, false, null)]
		public string Reagent;
	}
}
