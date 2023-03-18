using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events
{
	// Token: 0x020002D2 RID: 722
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class NPCSteeringDebugEvent : EntityEventArgs
	{
		// Token: 0x060007E2 RID: 2018 RVA: 0x0001A2D8 File Offset: 0x000184D8
		public NPCSteeringDebugEvent(List<NPCSteeringDebugData> data)
		{
			this.Data = data;
		}

		// Token: 0x04000821 RID: 2081
		public List<NPCSteeringDebugData> Data;
	}
}
