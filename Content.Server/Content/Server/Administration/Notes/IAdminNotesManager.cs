using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.Administration.Notes;
using Robust.Server.Player;

namespace Content.Server.Administration.Notes
{
	// Token: 0x02000814 RID: 2068
	[NullableContext(1)]
	public interface IAdminNotesManager
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06002D0B RID: 11531
		// (remove) Token: 0x06002D0C RID: 11532
		[Nullable(new byte[]
		{
			2,
			1
		})]
		event Action<SharedAdminNote> NoteAdded;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06002D0D RID: 11533
		// (remove) Token: 0x06002D0E RID: 11534
		[Nullable(new byte[]
		{
			2,
			1
		})]
		event Action<SharedAdminNote> NoteModified;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06002D0F RID: 11535
		// (remove) Token: 0x06002D10 RID: 11536
		[Nullable(2)]
		event Action<int> NoteDeleted;

		// Token: 0x06002D11 RID: 11537
		bool CanCreate(IPlayerSession admin);

		// Token: 0x06002D12 RID: 11538
		bool CanDelete(IPlayerSession admin);

		// Token: 0x06002D13 RID: 11539
		bool CanEdit(IPlayerSession admin);

		// Token: 0x06002D14 RID: 11540
		bool CanView(IPlayerSession admin);

		// Token: 0x06002D15 RID: 11541
		Task OpenEui(IPlayerSession admin, Guid notedPlayer);

		// Token: 0x06002D16 RID: 11542
		Task AddNote(IPlayerSession createdBy, Guid player, string message);

		// Token: 0x06002D17 RID: 11543
		Task DeleteNote(int noteId, IPlayerSession deletedBy);

		// Token: 0x06002D18 RID: 11544
		Task ModifyNote(int noteId, IPlayerSession editedBy, string message);

		// Token: 0x06002D19 RID: 11545
		Task<List<AdminNote>> GetNotes(Guid player);

		// Token: 0x06002D1A RID: 11546
		Task<string> GetPlayerName(Guid player);
	}
}
