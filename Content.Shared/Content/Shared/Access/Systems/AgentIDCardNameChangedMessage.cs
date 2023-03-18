using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems
{
	// Token: 0x02000773 RID: 1907
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AgentIDCardNameChangedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x0004C521 File Offset: 0x0004A721
		public string Name { get; }

		// Token: 0x06001783 RID: 6019 RVA: 0x0004C529 File Offset: 0x0004A729
		public AgentIDCardNameChangedMessage(string name)
		{
			this.Name = name;
		}
	}
}
