using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Actions
{
	// Token: 0x0200075F RID: 1887
	[ImplicitDataDefinitionForInheritors]
	public abstract class BaseActionEvent : HandledEntityEventArgs
	{
		// Token: 0x04001708 RID: 5896
		public EntityUid Performer;
	}
}
