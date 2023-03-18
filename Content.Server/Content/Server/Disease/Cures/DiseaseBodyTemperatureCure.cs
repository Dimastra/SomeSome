using System;
using System.Runtime.CompilerServices;
using Content.Server.Temperature.Components;
using Content.Shared.Disease;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Cures
{
	// Token: 0x02000570 RID: 1392
	public sealed class DiseaseBodyTemperatureCure : DiseaseCure
	{
		// Token: 0x06001D5C RID: 7516 RVA: 0x0009C8C8 File Offset: 0x0009AAC8
		public override bool Cure(DiseaseEffectArgs args)
		{
			TemperatureComponent temp;
			return args.EntityManager.TryGetComponent<TemperatureComponent>(args.DiseasedEntity, ref temp) && temp.CurrentTemperature > this.Min && temp.CurrentTemperature < float.MaxValue;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x0009C90C File Offset: 0x0009AB0C
		[NullableContext(1)]
		public override string CureText()
		{
			if (this.Min == 0f)
			{
				return Loc.GetString("diagnoser-cure-temp-max", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("max", Math.Round((double)this.Max))
				});
			}
			if (this.Max == 3.4028235E+38f)
			{
				return Loc.GetString("diagnoser-cure-temp-min", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("min", Math.Round((double)this.Min))
				});
			}
			return Loc.GetString("diagnoser-cure-temp-both", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("max", Math.Round((double)this.Max)),
				new ValueTuple<string, object>("min", Math.Round((double)this.Min))
			});
		}

		// Token: 0x040012D0 RID: 4816
		[DataField("min", false, 1, false, false, null)]
		public float Min;

		// Token: 0x040012D1 RID: 4817
		[DataField("max", false, 1, false, false, null)]
		public float Max = float.MaxValue;
	}
}
