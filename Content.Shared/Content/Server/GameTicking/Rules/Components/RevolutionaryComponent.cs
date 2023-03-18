using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameTicking.Rules.Components
{
	// Token: 0x0200000B RID: 11
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RevolutionaryComponent : Component
	{
		// Token: 0x04000008 RID: 8
		[Nullable(1)]
		public static string LayerName = "RevHud";

		// Token: 0x04000009 RID: 9
		[DataField("HeadRevolutionary", false, 1, true, false, null)]
		[ViewVariables]
		public bool HeadRevolutionary = true;
	}
}
