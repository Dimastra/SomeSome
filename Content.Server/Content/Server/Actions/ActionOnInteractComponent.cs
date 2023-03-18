using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Actions
{
	// Token: 0x02000873 RID: 2163
	[RegisterComponent]
	public sealed class ActionOnInteractComponent : Component
	{
		// Token: 0x04001C6E RID: 7278
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("activateActions", false, 1, false, false, null)]
		public List<InstantAction> ActivateActions;

		// Token: 0x04001C6F RID: 7279
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("entityActions", false, 1, false, false, null)]
		public List<EntityTargetAction> EntityActions;

		// Token: 0x04001C70 RID: 7280
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("worldActions", false, 1, false, false, null)]
		public List<WorldTargetAction> WorldActions;
	}
}
