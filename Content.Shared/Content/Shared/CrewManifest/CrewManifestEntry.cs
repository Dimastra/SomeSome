using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest
{
	// Token: 0x02000550 RID: 1360
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrewManifestEntry
	{
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600108D RID: 4237 RVA: 0x000361DA File Offset: 0x000343DA
		public string Name { get; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x000361E2 File Offset: 0x000343E2
		public string JobTitle { get; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x000361EA File Offset: 0x000343EA
		public string JobIcon { get; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x000361F2 File Offset: 0x000343F2
		public string JobPrototype { get; }

		// Token: 0x06001091 RID: 4241 RVA: 0x000361FA File Offset: 0x000343FA
		public CrewManifestEntry(string name, string jobTitle, string jobIcon, string jobPrototype)
		{
			this.Name = name;
			this.JobTitle = jobTitle;
			this.JobIcon = jobIcon;
			this.JobPrototype = jobPrototype;
		}
	}
}
