using System;
using System.Runtime.CompilerServices;
using Content.Shared.VoiceMask;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.VoiceMask
{
	// Token: 0x0200004F RID: 79
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoiceMaskBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600016A RID: 362 RVA: 0x000021BC File Offset: 0x000003BC
		public VoiceMaskBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000B90C File Offset: 0x00009B0C
		protected override void Open()
		{
			base.Open();
			this._window = new VoiceMaskNameChangeWindow();
			this._window.OpenCentered();
			VoiceMaskNameChangeWindow window = this._window;
			window.OnNameChange = (Action<string>)Delegate.Combine(window.OnNameChange, new Action<string>(this.OnNameSelected));
			VoiceMaskNameChangeWindow window2 = this._window;
			window2.OnVoiceChange = (Action<string>)Delegate.Combine(window2.OnVoiceChange, new Action<string>(delegate(string value)
			{
				base.SendMessage(new VoiceMaskChangeVoiceMessage(value));
			}));
			this._window.OnClose += base.Close;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000B99A File Offset: 0x00009B9A
		private void OnNameSelected(string name)
		{
			base.SendMessage(new VoiceMaskChangeNameMessage(name));
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000B9A8 File Offset: 0x00009BA8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			VoiceMaskBuiState voiceMaskBuiState = state as VoiceMaskBuiState;
			if (voiceMaskBuiState == null || this._window == null)
			{
				return;
			}
			this._window.UpdateState(voiceMaskBuiState.Name, voiceMaskBuiState.Voice);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000B9DF File Offset: 0x00009BDF
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			VoiceMaskNameChangeWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x040000F5 RID: 245
		[Nullable(2)]
		private VoiceMaskNameChangeWindow _window;
	}
}
