using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Physics
{
	// Token: 0x0200027B RID: 635
	[NetSerializable]
	[Serializable]
	public sealed class PreventCollideComponentState : ComponentState
	{
		// Token: 0x0600073A RID: 1850 RVA: 0x00018A3B File Offset: 0x00016C3B
		[NullableContext(1)]
		public PreventCollideComponentState(PreventCollideComponent component)
		{
			this.Uid = component.Uid;
		}

		// Token: 0x0400074B RID: 1867
		public EntityUid Uid;
	}
}
