using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Clothing.Components
{
	// Token: 0x0200063C RID: 1596
	[Access(new Type[]
	{
		typeof(MaskSystem)
	})]
	[RegisterComponent]
	public sealed class MaskComponent : Component
	{
		// Token: 0x040014CC RID: 5324
		[Nullable(2)]
		[DataField("toggleAction", false, 1, false, false, null)]
		public InstantAction ToggleAction;

		// Token: 0x040014CD RID: 5325
		public bool IsToggled;
	}
}
