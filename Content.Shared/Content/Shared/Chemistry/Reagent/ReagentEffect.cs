using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.Database;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E2 RID: 1506
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class ReagentEffect
	{
		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600120D RID: 4621 RVA: 0x0003B469 File Offset: 0x00039669
		[JsonPropertyName("id")]
		private protected string _id
		{
			get
			{
				return base.GetType().Name;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x0003B476 File Offset: 0x00039676
		[JsonIgnore]
		[DataField("logImpact", false, 1, false, false, null)]
		public virtual LogImpact LogImpact { get; } = -1;

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x0003B47E File Offset: 0x0003967E
		[JsonIgnore]
		[DataField("shouldLog", false, 1, false, false, null)]
		public virtual bool ShouldLog { get; }

		// Token: 0x06001210 RID: 4624
		public abstract void Effect(ReagentEffectArgs args);

		// Token: 0x04001104 RID: 4356
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[JsonPropertyName("conditions")]
		[DataField("conditions", false, 1, false, false, null)]
		public ReagentEffectCondition[] Conditions;

		// Token: 0x04001105 RID: 4357
		[JsonPropertyName("probability")]
		[DataField("probability", false, 1, false, false, null)]
		public float Probability = 1f;
	}
}
