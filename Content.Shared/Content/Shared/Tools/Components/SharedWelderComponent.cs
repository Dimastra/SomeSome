using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000B8 RID: 184
	[NetworkedComponent]
	public abstract class SharedWelderComponent : Component
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0000A99E File Offset: 0x00008B9E
		// (set) Token: 0x06000208 RID: 520 RVA: 0x0000A9A6 File Offset: 0x00008BA6
		public bool Lit { get; set; }
	}
}
