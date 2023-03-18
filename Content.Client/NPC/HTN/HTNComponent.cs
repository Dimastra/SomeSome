using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Client.NPC.HTN
{
	// Token: 0x02000213 RID: 531
	[RegisterComponent]
	public sealed class HTNComponent : NPCComponent
	{
		// Token: 0x040006D8 RID: 1752
		[Nullable(1)]
		public string DebugText = string.Empty;
	}
}
