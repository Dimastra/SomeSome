using System;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.Components
{
	// Token: 0x0200036C RID: 876
	public abstract class NPCComponent : SharedNPCComponent
	{
		// Token: 0x04000B03 RID: 2819
		[Nullable(1)]
		[DataField("blackboard", false, 1, false, false, typeof(NPCBlackboardSerializer))]
		public NPCBlackboard Blackboard = new NPCBlackboard();
	}
}
