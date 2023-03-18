using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Forensics
{
	// Token: 0x020004E5 RID: 1253
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ForensicsComponent : Component
	{
		// Token: 0x04001038 RID: 4152
		[DataField("fingerprints", false, 1, false, false, null)]
		public HashSet<string> Fingerprints = new HashSet<string>();

		// Token: 0x04001039 RID: 4153
		[DataField("fibers", false, 1, false, false, null)]
		public HashSet<string> Fibers = new HashSet<string>();
	}
}
