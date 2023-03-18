using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Store
{
	// Token: 0x0200011E RID: 286
	[ImplicitDataDefinitionForInheritors]
	public abstract class ListingCondition
	{
		// Token: 0x06000343 RID: 835
		public abstract bool Condition(ListingConditionArgs args);
	}
}
