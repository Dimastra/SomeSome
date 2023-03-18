using System;
using Content.Server.Shuttles.Systems;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000207 RID: 519
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ShuttleSystem)
	})]
	public sealed class IFFConsoleComponent : Component
	{
		// Token: 0x0400064E RID: 1614
		[ViewVariables]
		[DataField("allowedFlags", false, 1, false, false, null)]
		public IFFFlags AllowedFlags = IFFFlags.HideLabel;
	}
}
