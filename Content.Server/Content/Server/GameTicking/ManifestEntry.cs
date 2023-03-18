using System;
using System.Runtime.CompilerServices;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B6 RID: 1206
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ManifestEntry
	{
		// Token: 0x0600188B RID: 6283 RVA: 0x0007F996 File Offset: 0x0007DB96
		public ManifestEntry(string characterName, string jobId)
		{
			this.CharacterName = characterName;
			this.JobId = jobId;
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x0600188C RID: 6284 RVA: 0x0007F9AC File Offset: 0x0007DBAC
		[ViewVariables]
		public string CharacterName { get; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x0600188D RID: 6285 RVA: 0x0007F9B4 File Offset: 0x0007DBB4
		[ViewVariables]
		public string JobId { get; }
	}
}
