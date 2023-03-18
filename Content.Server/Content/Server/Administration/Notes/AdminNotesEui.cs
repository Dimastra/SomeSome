using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared.Administration.Notes;
using Content.Shared.Eui;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Notes
{
	// Token: 0x02000810 RID: 2064
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminNotesEui : BaseEui
	{
		// Token: 0x06002CE3 RID: 11491 RVA: 0x000ED629 File Offset: 0x000EB829
		public AdminNotesEui()
		{
			IoCManager.InjectDependencies<AdminNotesEui>(this);
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06002CE4 RID: 11492 RVA: 0x000ED64E File Offset: 0x000EB84E
		// (set) Token: 0x06002CE5 RID: 11493 RVA: 0x000ED656 File Offset: 0x000EB856
		private Guid NotedPlayer { get; set; }

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06002CE6 RID: 11494 RVA: 0x000ED65F File Offset: 0x000EB85F
		// (set) Token: 0x06002CE7 RID: 11495 RVA: 0x000ED667 File Offset: 0x000EB867
		private string NotedPlayerName { get; set; } = string.Empty;

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002CE8 RID: 11496 RVA: 0x000ED670 File Offset: 0x000EB870
		// (set) Token: 0x06002CE9 RID: 11497 RVA: 0x000ED678 File Offset: 0x000EB878
		private Dictionary<int, SharedAdminNote> Notes { get; set; } = new Dictionary<int, SharedAdminNote>();

		// Token: 0x06002CEA RID: 11498 RVA: 0x000ED684 File Offset: 0x000EB884
		public override void Opened()
		{
			AdminNotesEui.<Opened>d__15 <Opened>d__;
			<Opened>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Opened>d__.<>4__this = this;
			<Opened>d__.<>1__state = -1;
			<Opened>d__.<>t__builder.Start<AdminNotesEui.<Opened>d__15>(ref <Opened>d__);
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x000ED6BC File Offset: 0x000EB8BC
		public override void Closed()
		{
			base.Closed();
			this._admins.OnPermsChanged -= this.OnPermsChanged;
			this._notesMan.NoteAdded -= this.NoteModified;
			this._notesMan.NoteModified -= this.NoteModified;
			this._notesMan.NoteDeleted -= this.NoteDeleted;
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x000ED72C File Offset: 0x000EB92C
		public override EuiStateBase GetNewState()
		{
			return new AdminNotesEuiState(this.NotedPlayerName, this.Notes, this._notesMan.CanCreate(base.Player), this._notesMan.CanDelete(base.Player), this._notesMan.CanEdit(base.Player));
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x000ED780 File Offset: 0x000EB980
		public override void HandleMessage(EuiMessageBase msg)
		{
			AdminNotesEui.<HandleMessage>d__18 <HandleMessage>d__;
			<HandleMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleMessage>d__.<>4__this = this;
			<HandleMessage>d__.msg = msg;
			<HandleMessage>d__.<>1__state = -1;
			<HandleMessage>d__.<>t__builder.Start<AdminNotesEui.<HandleMessage>d__18>(ref <HandleMessage>d__);
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x000ED7C0 File Offset: 0x000EB9C0
		public Task ChangeNotedPlayer(Guid notedPlayer)
		{
			AdminNotesEui.<ChangeNotedPlayer>d__19 <ChangeNotedPlayer>d__;
			<ChangeNotedPlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ChangeNotedPlayer>d__.<>4__this = this;
			<ChangeNotedPlayer>d__.notedPlayer = notedPlayer;
			<ChangeNotedPlayer>d__.<>1__state = -1;
			<ChangeNotedPlayer>d__.<>t__builder.Start<AdminNotesEui.<ChangeNotedPlayer>d__19>(ref <ChangeNotedPlayer>d__);
			return <ChangeNotedPlayer>d__.<>t__builder.Task;
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000ED80B File Offset: 0x000EBA0B
		private void NoteModified(SharedAdminNote note)
		{
			this.Notes[note.Id] = note;
			base.StateDirty();
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000ED825 File Offset: 0x000EBA25
		private void NoteDeleted(int id)
		{
			this.Notes.Remove(id);
			base.StateDirty();
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000ED83C File Offset: 0x000EBA3C
		private Task LoadFromDb()
		{
			AdminNotesEui.<LoadFromDb>d__22 <LoadFromDb>d__;
			<LoadFromDb>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadFromDb>d__.<>4__this = this;
			<LoadFromDb>d__.<>1__state = -1;
			<LoadFromDb>d__.<>t__builder.Start<AdminNotesEui.<LoadFromDb>d__22>(ref <LoadFromDb>d__);
			return <LoadFromDb>d__.<>t__builder.Task;
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x000ED87F File Offset: 0x000EBA7F
		private void OnPermsChanged(AdminPermsChangedEventArgs args)
		{
			if (args.Player == base.Player && !this._notesMan.CanView(base.Player))
			{
				base.Close();
				return;
			}
			base.StateDirty();
		}

		// Token: 0x04001BDF RID: 7135
		[Dependency]
		private readonly IAdminManager _admins;

		// Token: 0x04001BE0 RID: 7136
		[Dependency]
		private readonly IAdminNotesManager _notesMan;
	}
}
