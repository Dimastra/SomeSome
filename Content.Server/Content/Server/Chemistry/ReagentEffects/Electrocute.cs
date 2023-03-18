using System;
using Content.Server.Electrocution;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000661 RID: 1633
	public sealed class Electrocute : ReagentEffect
	{
		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06002262 RID: 8802 RVA: 0x000B3CB1 File Offset: 0x000B1EB1
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x000B3CB4 File Offset: 0x000B1EB4
		public override void Effect(ReagentEffectArgs args)
		{
			EntitySystem.Get<ElectrocutionSystem>().TryDoElectrocution(args.SolutionEntity, null, Math.Max((args.Quantity * this.ElectrocuteDamageScale).Int(), 1), TimeSpan.FromSeconds((double)this.ElectrocuteTime), this.Refresh, 1f, null, true);
			Solution source = args.Source;
			if (source == null)
			{
				return;
			}
			source.RemoveReagent(args.Reagent.ID, args.Quantity);
		}

		// Token: 0x04001544 RID: 5444
		[DataField("electrocuteTime", false, 1, false, false, null)]
		public int ElectrocuteTime = 2;

		// Token: 0x04001545 RID: 5445
		[DataField("electrocuteDamageScale", false, 1, false, false, null)]
		public int ElectrocuteDamageScale = 5;

		// Token: 0x04001546 RID: 5446
		[DataField("refresh", false, 1, false, false, null)]
		public bool Refresh = true;
	}
}
