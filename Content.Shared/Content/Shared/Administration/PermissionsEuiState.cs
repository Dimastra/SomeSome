using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200073C RID: 1852
	[NetSerializable]
	[Serializable]
	public sealed class PermissionsEuiState : EuiStateBase
	{
		// Token: 0x040016B3 RID: 5811
		public bool IsLoading;

		// Token: 0x040016B4 RID: 5812
		[Nullable(1)]
		public PermissionsEuiState.AdminData[] Admins = Array.Empty<PermissionsEuiState.AdminData>();

		// Token: 0x040016B5 RID: 5813
		[Nullable(1)]
		public Dictionary<int, PermissionsEuiState.AdminRankData> AdminRanks = new Dictionary<int, PermissionsEuiState.AdminRankData>();

		// Token: 0x02000891 RID: 2193
		[NullableContext(2)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public struct AdminData
		{
			// Token: 0x04001A6E RID: 6766
			public NetUserId UserId;

			// Token: 0x04001A6F RID: 6767
			public string UserName;

			// Token: 0x04001A70 RID: 6768
			public string Title;

			// Token: 0x04001A71 RID: 6769
			public AdminFlags PosFlags;

			// Token: 0x04001A72 RID: 6770
			public AdminFlags NegFlags;

			// Token: 0x04001A73 RID: 6771
			public int? RankId;
		}

		// Token: 0x02000892 RID: 2194
		[NetSerializable]
		[Serializable]
		public struct AdminRankData
		{
			// Token: 0x04001A74 RID: 6772
			[Nullable(1)]
			public string Name;

			// Token: 0x04001A75 RID: 6773
			public AdminFlags Flags;
		}
	}
}
