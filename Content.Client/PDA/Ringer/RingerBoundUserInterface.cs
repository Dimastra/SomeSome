using System;
using System.Runtime.CompilerServices;
using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.PDA.Ringer
{
	// Token: 0x020001C7 RID: 455
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RingerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000BF7 RID: 3063 RVA: 0x000021BC File Offset: 0x000003BC
		public RingerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x00045230 File Offset: 0x00043430
		protected override void Open()
		{
			base.Open();
			this._menu = new RingtoneMenu();
			this._menu.OpenToLeft();
			this._menu.OnClose += base.Close;
			this._menu.TestRingerButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new RingerPlayRingtoneMessage());
			};
			this._menu.SetRingerButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Note[] ringTone;
				if (!this.TryGetRingtone(out ringTone))
				{
					return;
				}
				base.SendMessage(new RingerSetRingtoneMessage(ringTone));
			};
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x000452A8 File Offset: 0x000434A8
		private bool TryGetRingtone(out Note[] ringtone)
		{
			if (this._menu == null)
			{
				ringtone = Array.Empty<Note>();
				return false;
			}
			ringtone = new Note[4];
			for (int i = 0; i < this._menu.RingerNoteInputs.Length; i++)
			{
				Note note;
				if (!Enum.TryParse<Note>(this._menu.RingerNoteInputs[i].Text.Replace("#", "sharp"), false, out note))
				{
					return false;
				}
				ringtone[i] = note;
			}
			return true;
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x0004531C File Offset: 0x0004351C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._menu != null)
			{
				RingerUpdateState ringerUpdateState = state as RingerUpdateState;
				if (ringerUpdateState != null)
				{
					for (int i = 0; i < this._menu.RingerNoteInputs.Length; i++)
					{
						string text = ringerUpdateState.Ringtone[i].ToString();
						if (RingtoneMenu.IsNote(text))
						{
							this._menu.PreviousNoteInputs[i] = text.Replace("sharp", "#");
							this._menu.RingerNoteInputs[i].Text = this._menu.PreviousNoteInputs[i];
						}
					}
					this._menu.TestRingerButton.Visible = !ringerUpdateState.IsPlaying;
					return;
				}
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x000453D0 File Offset: 0x000435D0
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			RingtoneMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x040005BC RID: 1468
		[Nullable(2)]
		private RingtoneMenu _menu;
	}
}
