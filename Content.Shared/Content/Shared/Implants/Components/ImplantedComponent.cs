using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Shared.Implants.Components
{
	// Token: 0x020003F1 RID: 1009
	[RegisterComponent]
	public sealed class ImplantedComponent : Component
	{
		// Token: 0x04000BCA RID: 3018
		[Nullable(1)]
		public Container ImplantContainer;
	}
}
