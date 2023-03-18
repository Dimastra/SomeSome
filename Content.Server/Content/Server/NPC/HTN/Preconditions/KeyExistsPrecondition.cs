using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NPC.HTN.Preconditions
{
	// Token: 0x02000363 RID: 867
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class KeyExistsPrecondition : HTNPrecondition
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x0005E5A4 File Offset: 0x0005C7A4
		public override bool IsMet(NPCBlackboard blackboard)
		{
			return blackboard.ContainsKey(this.Key);
		}

		// Token: 0x04000AEC RID: 2796
		[DataField("key", false, 1, true, false, null)]
		public string Key = string.Empty;
	}
}
