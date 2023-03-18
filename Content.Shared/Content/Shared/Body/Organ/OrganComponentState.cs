using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Body.Organ
{
	// Token: 0x02000660 RID: 1632
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class OrganComponentState : ComponentState
	{
		// Token: 0x06001401 RID: 5121 RVA: 0x0004311B File Offset: 0x0004131B
		public OrganComponentState(EntityUid? body, OrganSlot parent)
		{
			this.Body = body;
			this.Parent = parent;
		}

		// Token: 0x040013B3 RID: 5043
		public readonly EntityUid? Body;

		// Token: 0x040013B4 RID: 5044
		public readonly OrganSlot Parent;
	}
}
