using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Administration.Notes;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Server.Administration.Notes
{
	// Token: 0x02000812 RID: 2066
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminNotesManager : IAdminNotesManager, IPostInjectInit
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06002CF6 RID: 11510 RVA: 0x000ED8FC File Offset: 0x000EBAFC
		// (remove) Token: 0x06002CF7 RID: 11511 RVA: 0x000ED934 File Offset: 0x000EBB34
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<SharedAdminNote> NoteAdded;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06002CF8 RID: 11512 RVA: 0x000ED96C File Offset: 0x000EBB6C
		// (remove) Token: 0x06002CF9 RID: 11513 RVA: 0x000ED9A4 File Offset: 0x000EBBA4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<SharedAdminNote> NoteModified;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06002CFA RID: 11514 RVA: 0x000ED9DC File Offset: 0x000EBBDC
		// (remove) Token: 0x06002CFB RID: 11515 RVA: 0x000EDA14 File Offset: 0x000EBC14
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<int> NoteDeleted;

		// Token: 0x06002CFC RID: 11516 RVA: 0x000EDA49 File Offset: 0x000EBC49
		public bool CanCreate(IPlayerSession admin)
		{
			return this.CanEdit(admin);
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x000EDA52 File Offset: 0x000EBC52
		public bool CanDelete(IPlayerSession admin)
		{
			return this.CanEdit(admin);
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x000EDA5B File Offset: 0x000EBC5B
		public bool CanEdit(IPlayerSession admin)
		{
			return this._admins.HasAdminFlag(admin, AdminFlags.EditNotes);
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x000EDA6E File Offset: 0x000EBC6E
		public bool CanView(IPlayerSession admin)
		{
			return this._admins.HasAdminFlag(admin, AdminFlags.ViewNotes);
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x000EDA84 File Offset: 0x000EBC84
		public Task OpenEui(IPlayerSession admin, Guid notedPlayer)
		{
			AdminNotesManager.<OpenEui>d__20 <OpenEui>d__;
			<OpenEui>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OpenEui>d__.<>4__this = this;
			<OpenEui>d__.admin = admin;
			<OpenEui>d__.notedPlayer = notedPlayer;
			<OpenEui>d__.<>1__state = -1;
			<OpenEui>d__.<>t__builder.Start<AdminNotesManager.<OpenEui>d__20>(ref <OpenEui>d__);
			return <OpenEui>d__.<>t__builder.Task;
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x000EDAD8 File Offset: 0x000EBCD8
		public Task AddNote(IPlayerSession createdBy, Guid player, string message)
		{
			AdminNotesManager.<AddNote>d__21 <AddNote>d__;
			<AddNote>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddNote>d__.<>4__this = this;
			<AddNote>d__.createdBy = createdBy;
			<AddNote>d__.player = player;
			<AddNote>d__.message = message;
			<AddNote>d__.<>1__state = -1;
			<AddNote>d__.<>t__builder.Start<AdminNotesManager.<AddNote>d__21>(ref <AddNote>d__);
			return <AddNote>d__.<>t__builder.Task;
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x000EDB34 File Offset: 0x000EBD34
		public Task DeleteNote(int noteId, IPlayerSession deletedBy)
		{
			AdminNotesManager.<DeleteNote>d__22 <DeleteNote>d__;
			<DeleteNote>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteNote>d__.<>4__this = this;
			<DeleteNote>d__.noteId = noteId;
			<DeleteNote>d__.deletedBy = deletedBy;
			<DeleteNote>d__.<>1__state = -1;
			<DeleteNote>d__.<>t__builder.Start<AdminNotesManager.<DeleteNote>d__22>(ref <DeleteNote>d__);
			return <DeleteNote>d__.<>t__builder.Task;
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x000EDB88 File Offset: 0x000EBD88
		public Task ModifyNote(int noteId, IPlayerSession editedBy, string message)
		{
			AdminNotesManager.<ModifyNote>d__23 <ModifyNote>d__;
			<ModifyNote>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ModifyNote>d__.<>4__this = this;
			<ModifyNote>d__.noteId = noteId;
			<ModifyNote>d__.editedBy = editedBy;
			<ModifyNote>d__.message = message;
			<ModifyNote>d__.<>1__state = -1;
			<ModifyNote>d__.<>t__builder.Start<AdminNotesManager.<ModifyNote>d__23>(ref <ModifyNote>d__);
			return <ModifyNote>d__.<>t__builder.Task;
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x000EDBE4 File Offset: 0x000EBDE4
		public Task<List<AdminNote>> GetNotes(Guid player)
		{
			AdminNotesManager.<GetNotes>d__24 <GetNotes>d__;
			<GetNotes>d__.<>t__builder = AsyncTaskMethodBuilder<List<AdminNote>>.Create();
			<GetNotes>d__.<>4__this = this;
			<GetNotes>d__.player = player;
			<GetNotes>d__.<>1__state = -1;
			<GetNotes>d__.<>t__builder.Start<AdminNotesManager.<GetNotes>d__24>(ref <GetNotes>d__);
			return <GetNotes>d__.<>t__builder.Task;
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x000EDC30 File Offset: 0x000EBE30
		public Task<string> GetPlayerName(Guid player)
		{
			AdminNotesManager.<GetPlayerName>d__25 <GetPlayerName>d__;
			<GetPlayerName>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetPlayerName>d__.<>4__this = this;
			<GetPlayerName>d__.player = player;
			<GetPlayerName>d__.<>1__state = -1;
			<GetPlayerName>d__.<>t__builder.Start<AdminNotesManager.<GetPlayerName>d__25>(ref <GetPlayerName>d__);
			return <GetPlayerName>d__.<>t__builder.Task;
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x000EDC7B File Offset: 0x000EBE7B
		public void PostInject()
		{
			this._sawmill = this._logManager.GetSawmill("admin.notes");
		}

		// Token: 0x04001BE4 RID: 7140
		[Dependency]
		private readonly IAdminManager _admins;

		// Token: 0x04001BE5 RID: 7141
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04001BE6 RID: 7142
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04001BE7 RID: 7143
		[Dependency]
		private readonly EuiManager _euis;

		// Token: 0x04001BE8 RID: 7144
		[Dependency]
		private readonly IEntitySystemManager _systems;

		// Token: 0x04001BE9 RID: 7145
		public const string SawmillId = "admin.notes";

		// Token: 0x04001BED RID: 7149
		private ISawmill _sawmill;
	}
}
