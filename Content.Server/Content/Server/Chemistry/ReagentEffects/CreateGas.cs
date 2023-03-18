using System;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065F RID: 1631
	public sealed class CreateGas : ReagentEffect
	{
		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x0600225C RID: 8796 RVA: 0x000B3BE6 File Offset: 0x000B1DE6
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700052A RID: 1322
		// (get) Token: 0x0600225D RID: 8797 RVA: 0x000B3BE9 File Offset: 0x000B1DE9
		public override LogImpact LogImpact
		{
			get
			{
				return LogImpact.High;
			}
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x000B3BEC File Offset: 0x000B1DEC
		public override void Effect(ReagentEffectArgs args)
		{
			GasMixture tileMix = args.EntityManager.EntitySysManager.GetEntitySystem<AtmosphereSystem>().GetContainingMixture(args.SolutionEntity, false, true, null);
			if (tileMix != null)
			{
				tileMix.AdjustMoles(this.Gas, args.Quantity.Float() * this.Multiplier);
			}
		}

		// Token: 0x04001540 RID: 5440
		[DataField("gas", false, 1, true, false, null)]
		public Gas Gas;

		// Token: 0x04001541 RID: 5441
		[DataField("multiplier", false, 1, false, false, null)]
		public float Multiplier = 3f;
	}
}
