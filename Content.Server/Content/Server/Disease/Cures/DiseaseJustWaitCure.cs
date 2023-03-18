using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Cures
{
	// Token: 0x02000571 RID: 1393
	public sealed class DiseaseJustWaitCure : DiseaseCure
	{
		// Token: 0x06001D5F RID: 7519 RVA: 0x0009CA00 File Offset: 0x0009AC00
		public override bool Cure(DiseaseEffectArgs args)
		{
			this.Ticker++;
			return this.Ticker >= this.MaxLength;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0009CA21 File Offset: 0x0009AC21
		[NullableContext(1)]
		public override string CureText()
		{
			return Loc.GetString("diagnoser-cure-wait", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("time", this.MaxLength)
			});
		}

		// Token: 0x040012D2 RID: 4818
		[ViewVariables]
		public int Ticker;

		// Token: 0x040012D3 RID: 4819
		[DataField("maxLength", false, 1, true, false, null)]
		[ViewVariables]
		public int MaxLength = 150;
	}
}
