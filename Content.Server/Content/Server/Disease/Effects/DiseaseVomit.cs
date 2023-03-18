using System;
using Content.Server.Medical;
using Content.Shared.Disease;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Effects
{
	// Token: 0x0200056E RID: 1390
	public sealed class DiseaseVomit : DiseaseEffect
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x0009C76D File Offset: 0x0009A96D
		public override void Effect(DiseaseEffectArgs args)
		{
			args.EntityManager.EntitySysManager.GetEntitySystem<VomitSystem>().Vomit(args.DiseasedEntity, this.ThirstAmount, this.HungerAmount);
		}

		// Token: 0x040012CB RID: 4811
		[DataField("thirstAmount", false, 1, false, false, null)]
		public float ThirstAmount = -40f;

		// Token: 0x040012CC RID: 4812
		[DataField("hungerAmount", false, 1, false, false, null)]
		public float HungerAmount = -40f;
	}
}
