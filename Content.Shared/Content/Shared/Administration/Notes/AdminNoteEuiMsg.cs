using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes
{
	// Token: 0x02000749 RID: 1865
	public static class AdminNoteEuiMsg
	{
		// Token: 0x0200089F RID: 2207
		[NetSerializable]
		[Serializable]
		public sealed class Close : EuiMessageBase
		{
		}

		// Token: 0x020008A0 RID: 2208
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class CreateNoteRequest : EuiMessageBase
		{
			// Token: 0x06001A10 RID: 6672 RVA: 0x00051CEA File Offset: 0x0004FEEA
			public CreateNoteRequest(string message)
			{
				this.Message = message;
			}

			// Token: 0x1700054E RID: 1358
			// (get) Token: 0x06001A11 RID: 6673 RVA: 0x00051CF9 File Offset: 0x0004FEF9
			// (set) Token: 0x06001A12 RID: 6674 RVA: 0x00051D01 File Offset: 0x0004FF01
			public string Message { get; set; }
		}

		// Token: 0x020008A1 RID: 2209
		[NetSerializable]
		[Serializable]
		public sealed class DeleteNoteRequest : EuiMessageBase
		{
			// Token: 0x06001A13 RID: 6675 RVA: 0x00051D0A File Offset: 0x0004FF0A
			public DeleteNoteRequest(int id)
			{
				this.Id = id;
			}

			// Token: 0x1700054F RID: 1359
			// (get) Token: 0x06001A14 RID: 6676 RVA: 0x00051D19 File Offset: 0x0004FF19
			// (set) Token: 0x06001A15 RID: 6677 RVA: 0x00051D21 File Offset: 0x0004FF21
			public int Id { get; set; }
		}

		// Token: 0x020008A2 RID: 2210
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class EditNoteRequest : EuiMessageBase
		{
			// Token: 0x06001A16 RID: 6678 RVA: 0x00051D2A File Offset: 0x0004FF2A
			public EditNoteRequest(int id, string message)
			{
				this.Id = id;
				this.Message = message;
			}

			// Token: 0x17000550 RID: 1360
			// (get) Token: 0x06001A17 RID: 6679 RVA: 0x00051D40 File Offset: 0x0004FF40
			// (set) Token: 0x06001A18 RID: 6680 RVA: 0x00051D48 File Offset: 0x0004FF48
			public int Id { get; set; }

			// Token: 0x17000551 RID: 1361
			// (get) Token: 0x06001A19 RID: 6681 RVA: 0x00051D51 File Offset: 0x0004FF51
			// (set) Token: 0x06001A1A RID: 6682 RVA: 0x00051D59 File Offset: 0x0004FF59
			public string Message { get; set; }
		}
	}
}
