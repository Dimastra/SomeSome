using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Labels
{
	// Token: 0x02000385 RID: 901
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class HandLabelerLabelChangedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000A73 RID: 2675 RVA: 0x000226EB File Offset: 0x000208EB
		public string Label { get; }

		// Token: 0x06000A74 RID: 2676 RVA: 0x000226F3 File Offset: 0x000208F3
		public HandLabelerLabelChangedMessage(string label)
		{
			this.Label = label;
		}
	}
}
