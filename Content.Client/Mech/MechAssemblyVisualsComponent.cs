using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Mech
{
	// Token: 0x0200023B RID: 571
	[RegisterComponent]
	public sealed class MechAssemblyVisualsComponent : Component
	{
		// Token: 0x0400073B RID: 1851
		[Nullable(1)]
		[DataField("statePrefix", false, 1, true, false, null)]
		public string StatePrefix = string.Empty;
	}
}
