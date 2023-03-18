using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes
{
	// Token: 0x02000748 RID: 1864
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AdminNotesEuiState : EuiStateBase
	{
		// Token: 0x0600169E RID: 5790 RVA: 0x00049B99 File Offset: 0x00047D99
		public AdminNotesEuiState(string notedPlayerName, Dictionary<int, SharedAdminNote> notes, bool canCreate, bool canDelete, bool canEdit)
		{
			this.NotedPlayerName = notedPlayerName;
			this.Notes = notes;
			this.CanCreate = canCreate;
			this.CanDelete = canDelete;
			this.CanEdit = canEdit;
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x0600169F RID: 5791 RVA: 0x00049BC6 File Offset: 0x00047DC6
		public string NotedPlayerName { get; }

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060016A0 RID: 5792 RVA: 0x00049BCE File Offset: 0x00047DCE
		public Dictionary<int, SharedAdminNote> Notes { get; }

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060016A1 RID: 5793 RVA: 0x00049BD6 File Offset: 0x00047DD6
		public bool CanCreate { get; }

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060016A2 RID: 5794 RVA: 0x00049BDE File Offset: 0x00047DDE
		public bool CanDelete { get; }

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x060016A3 RID: 5795 RVA: 0x00049BE6 File Offset: 0x00047DE6
		public bool CanEdit { get; }
	}
}
