using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x0200072D RID: 1837
	public static class AdminAnnounceEuiMsg
	{
		// Token: 0x0200088C RID: 2188
		[NetSerializable]
		[Serializable]
		public sealed class Close : EuiMessageBase
		{
		}

		// Token: 0x0200088D RID: 2189
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class DoAnnounce : EuiMessageBase
		{
			// Token: 0x04001A63 RID: 6755
			public bool CloseAfter;

			// Token: 0x04001A64 RID: 6756
			public string Announcer;

			// Token: 0x04001A65 RID: 6757
			public string Announcement;

			// Token: 0x04001A66 RID: 6758
			public AdminAnnounceType AnnounceType;
		}
	}
}
