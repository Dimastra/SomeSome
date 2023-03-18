using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E6 RID: 1510
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class ReagentEffectCondition
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x0003B956 File Offset: 0x00039B56
		[JsonPropertyName("id")]
		private protected string _id
		{
			get
			{
				return base.GetType().Name;
			}
		}

		// Token: 0x0600122D RID: 4653
		public abstract bool Condition(ReagentEffectArgs args);
	}
}
