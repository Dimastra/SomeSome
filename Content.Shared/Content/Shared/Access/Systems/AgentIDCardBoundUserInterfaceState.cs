using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000772 RID: 1906
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AgentIDCardBoundUserInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x0600177F RID: 6015 RVA: 0x0004C4FB File Offset: 0x0004A6FB
		public string CurrentName { get; }

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x0004C503 File Offset: 0x0004A703
		public string CurrentJob { get; }

		// Token: 0x06001781 RID: 6017 RVA: 0x0004C50B File Offset: 0x0004A70B
		public AgentIDCardBoundUserInterfaceState(string currentName, string currentJob)
		{
			this.CurrentName = currentName;
			this.CurrentJob = currentJob;
		}
	}
}
