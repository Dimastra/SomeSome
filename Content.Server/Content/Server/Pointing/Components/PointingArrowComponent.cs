using System;
using Content.Server.Pointing.EntitySystems;
using Content.Shared.Pointing.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Pointing.Components
{
	// Token: 0x020002CE RID: 718
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PointingSystem)
	})]
	public sealed class PointingArrowComponent : SharedPointingArrowComponent
	{
		// Token: 0x0400088C RID: 2188
		[ViewVariables]
		[DataField("rogue", false, 1, false, false, null)]
		public bool Rogue;
	}
}
