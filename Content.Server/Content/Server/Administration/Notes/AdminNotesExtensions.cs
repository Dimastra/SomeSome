using System;
using System.Runtime.CompilerServices;
using Content.Server.Database;
using Content.Shared.Administration.Notes;

namespace Content.Server.Administration.Notes
{
	// Token: 0x02000811 RID: 2065
	public static class AdminNotesExtensions
	{
		// Token: 0x06002CF5 RID: 11509 RVA: 0x000ED8C0 File Offset: 0x000EBAC0
		[NullableContext(1)]
		public static SharedAdminNote ToShared(this AdminNote note)
		{
			return new SharedAdminNote(note.Id, note.RoundId, note.Message, note.CreatedBy.LastSeenUserName, note.LastEditedBy.LastSeenUserName, note.CreatedAt, note.LastEditedAt);
		}
	}
}
