using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest
{
	// Token: 0x0200054D RID: 1357
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrewManifestEuiState : EuiStateBase
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x00036199 File Offset: 0x00034399
		public string StationName { get; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001089 RID: 4233 RVA: 0x000361A1 File Offset: 0x000343A1
		[Nullable(2)]
		public CrewManifestEntries Entries { [NullableContext(2)] get; }

		// Token: 0x0600108A RID: 4234 RVA: 0x000361A9 File Offset: 0x000343A9
		public CrewManifestEuiState(string stationName, [Nullable(2)] CrewManifestEntries entries)
		{
			this.StationName = stationName;
			this.Entries = entries;
		}
	}
}
