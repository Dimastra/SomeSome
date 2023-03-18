using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.White.Mindshield
{
	// Token: 0x02000036 RID: 54
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MindShieldComponent : Component
	{
		// Token: 0x0400009D RID: 157
		[Nullable(1)]
		public static string LayerName = "MindShieldHud";
	}
}
