using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200066A RID: 1642
	public sealed class ModifyLungGas : ReagentEffect
	{
		// Token: 0x06002279 RID: 8825 RVA: 0x000B40B8 File Offset: 0x000B22B8
		public override void Effect(ReagentEffectArgs args)
		{
			LungComponent lung;
			if (args.EntityManager.TryGetComponent<LungComponent>(args.OrganEntity, ref lung))
			{
				foreach (KeyValuePair<Gas, float> keyValuePair in this._ratios)
				{
					Gas gas2;
					float num;
					keyValuePair.Deconstruct(out gas2, out num);
					Gas gas = gas2;
					float ratio = num;
					lung.Air.Moles[(int)gas] += ratio * args.Quantity.Float() / 1144f;
				}
			}
		}

		// Token: 0x04001551 RID: 5457
		[Nullable(1)]
		[DataField("ratios", false, 1, true, false, null)]
		private Dictionary<Gas, float> _ratios;
	}
}
