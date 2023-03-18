using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Implants.Components
{
	// Token: 0x020003F3 RID: 1011
	[NetSerializable]
	[Serializable]
	public sealed class ImplanterComponentState : ComponentState
	{
		// Token: 0x06000BE2 RID: 3042 RVA: 0x0002744D File Offset: 0x0002564D
		public ImplanterComponentState(ImplanterToggleMode currentMode, bool implantOnly)
		{
			this.CurrentMode = currentMode;
			this.ImplantOnly = implantOnly;
		}

		// Token: 0x04000BD6 RID: 3030
		public ImplanterToggleMode CurrentMode;

		// Token: 0x04000BD7 RID: 3031
		public bool ImplantOnly;
	}
}
