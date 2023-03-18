using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x0200005E RID: 94
	[RegisterComponent]
	public sealed class TelepathicArtifactComponent : Component
	{
		// Token: 0x040000DD RID: 221
		[Nullable(1)]
		[DataField("messages", false, 1, false, false, null)]
		[ViewVariables]
		public List<string> Messages;

		// Token: 0x040000DE RID: 222
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("drastic", false, 1, false, false, null)]
		[ViewVariables]
		public List<string> DrasticMessages;

		// Token: 0x040000DF RID: 223
		[DataField("drasticProb", false, 1, false, false, null)]
		[ViewVariables]
		public float DrasticMessageProb = 0.2f;

		// Token: 0x040000E0 RID: 224
		[DataField("range", false, 1, false, false, null)]
		[ViewVariables]
		public float Range = 10f;
	}
}
