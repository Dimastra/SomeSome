using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.CrewManifest
{
	// Token: 0x020005DA RID: 1498
	[RegisterComponent]
	public sealed class CrewManifestViewerComponent : Component
	{
		// Token: 0x040013DB RID: 5083
		[DataField("unsecure", false, 1, false, false, null)]
		public bool Unsecure;
	}
}
