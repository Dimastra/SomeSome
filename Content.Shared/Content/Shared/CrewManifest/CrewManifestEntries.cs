using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest
{
	// Token: 0x0200054F RID: 1359
	[NetSerializable]
	[Serializable]
	public sealed class CrewManifestEntries
	{
		// Token: 0x04000F83 RID: 3971
		[Nullable(1)]
		public List<CrewManifestEntry> Entries = new List<CrewManifestEntry>();
	}
}
