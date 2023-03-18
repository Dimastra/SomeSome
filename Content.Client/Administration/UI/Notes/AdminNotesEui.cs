using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Eui;

namespace Content.Client.Administration.UI.Notes
{
	// Token: 0x020004B8 RID: 1208
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminNotesEui : BaseEui
	{
		// Token: 0x06001E53 RID: 7763 RVA: 0x000B1E84 File Offset: 0x000B0084
		public AdminNotesEui()
		{
			this.NoteWindow = new AdminNotesWindow();
			this.NoteControl = this.NoteWindow.Notes;
			this.NoteControl.OnNoteChanged += delegate(int id, string text)
			{
				base.SendMessage(new AdminNoteEuiMsg.EditNoteRequest(id, text));
			};
			this.NoteControl.OnNewNoteEntered += delegate(string text)
			{
				base.SendMessage(new AdminNoteEuiMsg.CreateNoteRequest(text));
			};
			this.NoteControl.OnNoteDeleted += delegate(int id)
			{
				base.SendMessage(new AdminNoteEuiMsg.DeleteNoteRequest(id));
			};
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x06001E54 RID: 7764 RVA: 0x000B1EF8 File Offset: 0x000B00F8
		private AdminNotesWindow NoteWindow { get; }

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06001E55 RID: 7765 RVA: 0x000B1F00 File Offset: 0x000B0100
		private AdminNotesControl NoteControl { get; }

		// Token: 0x06001E56 RID: 7766 RVA: 0x000B1F08 File Offset: 0x000B0108
		public override void HandleState(EuiStateBase state)
		{
			AdminNotesEuiState adminNotesEuiState = state as AdminNotesEuiState;
			if (adminNotesEuiState == null)
			{
				return;
			}
			this.NoteWindow.SetTitlePlayer(adminNotesEuiState.NotedPlayerName);
			this.NoteControl.SetNotes(adminNotesEuiState.Notes);
			this.NoteControl.SetPermissions(adminNotesEuiState.CanCreate, adminNotesEuiState.CanDelete, adminNotesEuiState.CanEdit);
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x000B1F5F File Offset: 0x000B015F
		public override void Opened()
		{
			this.NoteWindow.OpenCentered();
		}
	}
}
