using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000204 RID: 516
	[RegisterComponent]
	public sealed class EmergencyShuttleConsoleComponent : Component
	{
		// Token: 0x04000641 RID: 1601
		[Nullable(1)]
		[ViewVariables]
		[DataField("authorized", false, 1, false, false, null)]
		public HashSet<string> AuthorizedEntities = new HashSet<string>();

		// Token: 0x04000642 RID: 1602
		[ViewVariables]
		[DataField("authorizationsRequired", false, 1, false, false, null)]
		public int AuthorizationsRequired = 3;
	}
}
