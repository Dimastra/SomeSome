﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Shared.Administration.Notes;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Notes
{
	// Token: 0x020004B6 RID: 1206
	[GenerateTypedNameReferences]
	public sealed class AdminNotesControl : Control
	{
		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x06001E34 RID: 7732 RVA: 0x000B16C8 File Offset: 0x000AF8C8
		// (remove) Token: 0x06001E35 RID: 7733 RVA: 0x000B1700 File Offset: 0x000AF900
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
		public event Action<int, string> OnNoteChanged;

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x06001E36 RID: 7734 RVA: 0x000B1738 File Offset: 0x000AF938
		// (remove) Token: 0x06001E37 RID: 7735 RVA: 0x000B1770 File Offset: 0x000AF970
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
		public event Action<string> OnNewNoteEntered;

		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06001E38 RID: 7736 RVA: 0x000B17A8 File Offset: 0x000AF9A8
		// (remove) Token: 0x06001E39 RID: 7737 RVA: 0x000B17E0 File Offset: 0x000AF9E0
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<int> OnNoteDeleted;

		// Token: 0x06001E3A RID: 7738 RVA: 0x000B1815 File Offset: 0x000AFA15
		public AdminNotesControl()
		{
			AdminNotesControl.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<AdminNotesControl>(this);
			this.NewNote.OnTextEntered += this.NewNoteEntered;
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x06001E3B RID: 7739 RVA: 0x000B184C File Offset: 0x000AFA4C
		[Nullable(1)]
		private Dictionary<int, AdminNotesLine> Inputs { [NullableContext(1)] get; } = new Dictionary<int, AdminNotesLine>();

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x06001E3C RID: 7740 RVA: 0x000B1854 File Offset: 0x000AFA54
		// (set) Token: 0x06001E3D RID: 7741 RVA: 0x000B185C File Offset: 0x000AFA5C
		private bool CanCreate { get; set; }

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001E3E RID: 7742 RVA: 0x000B1865 File Offset: 0x000AFA65
		// (set) Token: 0x06001E3F RID: 7743 RVA: 0x000B186D File Offset: 0x000AFA6D
		private bool CanDelete { get; set; }

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001E40 RID: 7744 RVA: 0x000B1876 File Offset: 0x000AFA76
		// (set) Token: 0x06001E41 RID: 7745 RVA: 0x000B187E File Offset: 0x000AFA7E
		private bool CanEdit { get; set; }

		// Token: 0x06001E42 RID: 7746 RVA: 0x000B1887 File Offset: 0x000AFA87
		[NullableContext(1)]
		private void NewNoteEntered(LineEdit.LineEditEventArgs args)
		{
			if (string.IsNullOrWhiteSpace(args.Text))
			{
				return;
			}
			this.NewNote.Clear();
			Action<string> onNewNoteEntered = this.OnNewNoteEntered;
			if (onNewNoteEntered == null)
			{
				return;
			}
			onNewNoteEntered(args.Text);
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x000B18B8 File Offset: 0x000AFAB8
		[NullableContext(1)]
		private void NoteSubmitted(AdminNotesLine input)
		{
			string text = input.EditText.Trim();
			if (input.OriginalMessage == text)
			{
				return;
			}
			Action<int, string> onNoteChanged = this.OnNoteChanged;
			if (onNoteChanged == null)
			{
				return;
			}
			onNoteChanged(input.Id, text);
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x000B18F8 File Offset: 0x000AFAF8
		[NullableContext(1)]
		private bool NoteClicked(AdminNotesLine line)
		{
			this.ClosePopup();
			this._popup = new AdminNotesLinePopup(line.Note, this.CanDelete, this.CanEdit);
			this._popup.OnEditPressed += delegate(int noteId)
			{
				AdminNotesLine adminNotesLine;
				if (!this.Inputs.TryGetValue(noteId, out adminNotesLine))
				{
					return;
				}
				adminNotesLine.SetEditable(true);
			};
			this._popup.OnDeletePressed += delegate(int noteId)
			{
				Action<int> onNoteDeleted = this.OnNoteDeleted;
				if (onNoteDeleted == null)
				{
					return;
				}
				onNoteDeleted(noteId);
			};
			UIBox2 value = UIBox2.FromDimensions(base.UserInterfaceManager.MousePositionScaled.Position, new ValueTuple<float, float>(1f, 1f));
			this._popup.Open(new UIBox2?(value), null);
			return true;
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x000B199B File Offset: 0x000AFB9B
		private void ClosePopup()
		{
			AdminNotesLinePopup popup = this._popup;
			if (popup != null)
			{
				popup.Close();
			}
			this._popup = null;
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x000B19B8 File Offset: 0x000AFBB8
		[NullableContext(1)]
		public void SetNotes(Dictionary<int, SharedAdminNote> notes)
		{
			foreach (KeyValuePair<int, AdminNotesLine> keyValuePair in this.Inputs)
			{
				int num;
				AdminNotesLine adminNotesLine;
				keyValuePair.Deconstruct(out num, out adminNotesLine);
				int key = num;
				AdminNotesLine adminNotesLine2 = adminNotesLine;
				if (!notes.ContainsKey(key))
				{
					this.Notes.RemoveChild(adminNotesLine2);
					this.Inputs.Remove(key);
				}
			}
			foreach (SharedAdminNote sharedAdminNote in from note in notes.Values
			orderby note.Id
			select note)
			{
				AdminNotesLine adminNotesLine3;
				if (this.Inputs.TryGetValue(sharedAdminNote.Id, out adminNotesLine3))
				{
					adminNotesLine3.UpdateNote(sharedAdminNote);
				}
				else
				{
					adminNotesLine3 = new AdminNotesLine(sharedAdminNote);
					adminNotesLine3.OnSubmitted += this.NoteSubmitted;
					adminNotesLine3.OnClicked += this.NoteClicked;
					this.Notes.AddChild(adminNotesLine3);
					this.Inputs[sharedAdminNote.Id] = adminNotesLine3;
				}
			}
		}

		// Token: 0x06001E47 RID: 7751 RVA: 0x000B1B0C File Offset: 0x000AFD0C
		public void SetPermissions(bool create, bool delete, bool edit)
		{
			this.CanCreate = create;
			this.CanDelete = delete;
			this.CanEdit = edit;
			this.NewNoteLabel.Visible = create;
			this.NewNote.Visible = create;
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x000B1B3C File Offset: 0x000AFD3C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			foreach (AdminNotesLine adminNotesLine in this.Inputs.Values)
			{
				adminNotesLine.OnSubmitted -= this.NoteSubmitted;
			}
			this.Inputs.Clear();
			this.NewNote.OnTextEntered -= this.NewNoteEntered;
			if (this._popup != null)
			{
				base.UserInterfaceManager.PopupRoot.RemoveChild(this._popup);
			}
			this.OnNoteChanged = null;
			this.OnNewNoteEntered = null;
			this.OnNoteDeleted = null;
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06001E49 RID: 7753 RVA: 0x000B1C00 File Offset: 0x000AFE00
		public BoxContainer Notes
		{
			get
			{
				return base.FindControl<BoxContainer>("Notes");
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x06001E4A RID: 7754 RVA: 0x000B1C0D File Offset: 0x000AFE0D
		private Label NewNoteLabel
		{
			get
			{
				return base.FindControl<Label>("NewNoteLabel");
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x06001E4B RID: 7755 RVA: 0x000B1C1A File Offset: 0x000AFE1A
		private HistoryLineEdit NewNote
		{
			get
			{
				return base.FindControl<HistoryLineEdit>("NewNote");
			}
		}

		// Token: 0x06001E4E RID: 7758 RVA: 0x000B1C60 File Offset: 0x000AFE60
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Administration.UI.Notes.AdminNotesControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromXaml("#252525")
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.VerticalExpand = true;
			scrollContainer.HorizontalExpand = true;
			scrollContainer.HScrollEnabled = false;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			boxContainer2.Name = "Notes";
			Control control = boxContainer2;
			context.RobustNameScope.Register("Notes", control);
			boxContainer2.Access = new AccessLevel?(0);
			boxContainer2.VerticalExpand = true;
			control = boxContainer2;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
			boxContainer.XamlChildren.Add(control);
			Label label = new Label();
			label.Name = "NewNoteLabel";
			control = label;
			context.RobustNameScope.Register("NewNoteLabel", control);
			label.Text = (string)new LocExtension("admin-notes-new-note").ProvideValue();
			control = label;
			boxContainer.XamlChildren.Add(control);
			HistoryLineEdit historyLineEdit = new HistoryLineEdit();
			historyLineEdit.Name = "NewNote";
			control = historyLineEdit;
			context.RobustNameScope.Register("NewNote", control);
			control = historyLineEdit;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001E4F RID: 7759 RVA: 0x000B1E66 File Offset: 0x000B0066
		private static void !XamlIlPopulateTrampoline(AdminNotesControl A_0)
		{
			AdminNotesControl.Populate:Content.Client.Administration.UI.Notes.AdminNotesControl.xaml(null, A_0);
		}

		// Token: 0x04000EC6 RID: 3782
		[Nullable(2)]
		private AdminNotesLinePopup _popup;
	}
}
