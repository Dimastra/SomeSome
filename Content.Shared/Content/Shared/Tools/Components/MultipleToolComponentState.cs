using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000B4 RID: 180
	[NetSerializable]
	[Serializable]
	public sealed class MultipleToolComponentState : ComponentState
	{
		// Token: 0x06000205 RID: 517 RVA: 0x0000A987 File Offset: 0x00008B87
		public MultipleToolComponentState(uint selected)
		{
			this.Selected = selected;
		}

		// Token: 0x04000277 RID: 631
		public readonly uint Selected;
	}
}
