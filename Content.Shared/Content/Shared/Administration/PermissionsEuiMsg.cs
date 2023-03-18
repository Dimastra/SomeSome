using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200073D RID: 1853
	public static class PermissionsEuiMsg
	{
		// Token: 0x02000893 RID: 2195
		[NetSerializable]
		[Serializable]
		public sealed class Close : EuiMessageBase
		{
		}

		// Token: 0x02000894 RID: 2196
		[NetSerializable]
		[Serializable]
		public sealed class AddAdmin : EuiMessageBase
		{
			// Token: 0x04001A76 RID: 6774
			[Nullable(1)]
			public string UserNameOrId = string.Empty;

			// Token: 0x04001A77 RID: 6775
			[Nullable(2)]
			public string Title;

			// Token: 0x04001A78 RID: 6776
			public AdminFlags PosFlags;

			// Token: 0x04001A79 RID: 6777
			public AdminFlags NegFlags;

			// Token: 0x04001A7A RID: 6778
			public int? RankId;
		}

		// Token: 0x02000895 RID: 2197
		[NetSerializable]
		[Serializable]
		public sealed class RemoveAdmin : EuiMessageBase
		{
			// Token: 0x04001A7B RID: 6779
			public NetUserId UserId;
		}

		// Token: 0x02000896 RID: 2198
		[NetSerializable]
		[Serializable]
		public sealed class UpdateAdmin : EuiMessageBase
		{
			// Token: 0x04001A7C RID: 6780
			public NetUserId UserId;

			// Token: 0x04001A7D RID: 6781
			[Nullable(2)]
			public string Title;

			// Token: 0x04001A7E RID: 6782
			public AdminFlags PosFlags;

			// Token: 0x04001A7F RID: 6783
			public AdminFlags NegFlags;

			// Token: 0x04001A80 RID: 6784
			public int? RankId;
		}

		// Token: 0x02000897 RID: 2199
		[NetSerializable]
		[Serializable]
		public sealed class AddAdminRank : EuiMessageBase
		{
			// Token: 0x04001A81 RID: 6785
			[Nullable(1)]
			public string Name = string.Empty;

			// Token: 0x04001A82 RID: 6786
			public AdminFlags Flags;
		}

		// Token: 0x02000898 RID: 2200
		[NetSerializable]
		[Serializable]
		public sealed class RemoveAdminRank : EuiMessageBase
		{
			// Token: 0x04001A83 RID: 6787
			public int Id;
		}

		// Token: 0x02000899 RID: 2201
		[NetSerializable]
		[Serializable]
		public sealed class UpdateAdminRank : EuiMessageBase
		{
			// Token: 0x04001A84 RID: 6788
			public int Id;

			// Token: 0x04001A85 RID: 6789
			[Nullable(1)]
			public string Name = string.Empty;

			// Token: 0x04001A86 RID: 6790
			public AdminFlags Flags;
		}
	}
}
