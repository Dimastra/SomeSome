using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x0200020A RID: 522
	[RegisterComponent]
	public sealed class ShuttleConsoleComponent : SharedShuttleConsoleComponent
	{
		// Token: 0x04000657 RID: 1623
		[Nullable(1)]
		[ViewVariables]
		public readonly List<PilotComponent> SubscribedPilots = new List<PilotComponent>();

		// Token: 0x04000658 RID: 1624
		[DataField("zoom", false, 1, false, false, null)]
		public Vector2 Zoom = new Vector2(1.5f, 1.5f);
	}
}
