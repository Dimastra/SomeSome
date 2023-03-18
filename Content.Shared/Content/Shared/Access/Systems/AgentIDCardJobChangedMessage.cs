using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000774 RID: 1908
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AgentIDCardJobChangedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x0004C538 File Offset: 0x0004A738
		public string Job { get; }

		// Token: 0x06001785 RID: 6021 RVA: 0x0004C540 File Offset: 0x0004A740
		public AgentIDCardJobChangedMessage(string job)
		{
			this.Job = job;
		}
	}
}
