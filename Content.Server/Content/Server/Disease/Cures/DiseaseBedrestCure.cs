using System;
using System.Runtime.CompilerServices;
using Content.Server.Bed.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Cures
{
	// Token: 0x0200056F RID: 1391
	public sealed class DiseaseBedrestCure : DiseaseCure
	{
		// Token: 0x06001D59 RID: 7513 RVA: 0x0009C7B8 File Offset: 0x0009A9B8
		public override bool Cure(DiseaseEffectArgs args)
		{
			BuckleComponent buckle;
			if (args.EntityManager.TryGetComponent<BuckleComponent>(args.DiseasedEntity, ref buckle))
			{
				IEntityManager entityManager = args.EntityManager;
				StrapComponent buckledTo = buckle.BuckledTo;
				if (entityManager.HasComponent<HealOnBuckleComponent>((buckledTo != null) ? new EntityUid?(buckledTo.Owner) : null))
				{
					int ticks = 1;
					if (args.EntityManager.HasComponent<SleepingComponent>(args.DiseasedEntity))
					{
						ticks *= this.SleepMultiplier;
					}
					if (buckle.Buckled)
					{
						this.Ticker += ticks;
					}
					return this.Ticker >= this.MaxLength;
				}
			}
			return false;
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x0009C854 File Offset: 0x0009AA54
		[NullableContext(1)]
		public override string CureText()
		{
			return Loc.GetString("diagnoser-cure-bedrest", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("time", this.MaxLength),
				new ValueTuple<string, object>("sleep", this.MaxLength / this.SleepMultiplier)
			});
		}

		// Token: 0x040012CD RID: 4813
		[ViewVariables]
		public int Ticker;

		// Token: 0x040012CE RID: 4814
		[DataField("sleepMultiplier", false, 1, false, false, null)]
		public int SleepMultiplier = 3;

		// Token: 0x040012CF RID: 4815
		[DataField("maxLength", false, 1, true, false, null)]
		[ViewVariables]
		public int MaxLength = 60;
	}
}
