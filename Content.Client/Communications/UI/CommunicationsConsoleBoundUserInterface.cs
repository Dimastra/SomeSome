using System;
using System.Runtime.CompilerServices;
using Content.Shared.Communications;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Communications.UI
{
	// Token: 0x0200039C RID: 924
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CommunicationsConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x060016F8 RID: 5880 RVA: 0x00085B99 File Offset: 0x00083D99
		// (set) Token: 0x060016F9 RID: 5881 RVA: 0x00085BA1 File Offset: 0x00083DA1
		public bool CanAnnounce { get; private set; }

		// Token: 0x17000494 RID: 1172
		// (get) Token: 0x060016FA RID: 5882 RVA: 0x00085BAA File Offset: 0x00083DAA
		// (set) Token: 0x060016FB RID: 5883 RVA: 0x00085BB2 File Offset: 0x00083DB2
		public bool CanCall { get; private set; }

		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x060016FC RID: 5884 RVA: 0x00085BBB File Offset: 0x00083DBB
		// (set) Token: 0x060016FD RID: 5885 RVA: 0x00085BC3 File Offset: 0x00083DC3
		public bool CountdownStarted { get; private set; }

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x060016FE RID: 5886 RVA: 0x00085BCC File Offset: 0x00083DCC
		// (set) Token: 0x060016FF RID: 5887 RVA: 0x00085BD4 File Offset: 0x00083DD4
		public bool AlertLevelSelectable { get; private set; }

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x00085BDD File Offset: 0x00083DDD
		// (set) Token: 0x06001701 RID: 5889 RVA: 0x00085BE5 File Offset: 0x00083DE5
		public string CurrentLevel { get; private set; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x00085BF0 File Offset: 0x00083DF0
		public int Countdown
		{
			get
			{
				if (this._expectedCountdownTime != null)
				{
					return Math.Max((int)this._expectedCountdownTime.Value.Subtract(this._gameTiming.CurTime).TotalSeconds, 0);
				}
				return 0;
			}
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x000021BC File Offset: 0x000003BC
		public CommunicationsConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00085C39 File Offset: 0x00083E39
		protected override void Open()
		{
			base.Open();
			this._menu = new CommunicationsConsoleMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00085C6F File Offset: 0x00083E6F
		public void AlertLevelSelected(string level)
		{
			if (this.AlertLevelSelectable)
			{
				this.CurrentLevel = level;
				base.SendMessage(new CommunicationsConsoleSelectAlertLevelMessage(level));
			}
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x00085C8C File Offset: 0x00083E8C
		public void EmergencyShuttleButtonPressed()
		{
			if (this.CountdownStarted)
			{
				this.RecallShuttle();
				return;
			}
			this.CallShuttle();
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x00085CA4 File Offset: 0x00083EA4
		public void AnnounceButtonPressed(string message)
		{
			string message2 = (message.Length <= 256) ? message.Trim() : (message.Trim().Substring(0, 256) + "...");
			base.SendMessage(new CommunicationsConsoleAnnounceMessage(message2));
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x00085CEE File Offset: 0x00083EEE
		public void CallShuttle()
		{
			base.SendMessage(new CommunicationsConsoleCallEmergencyShuttleMessage());
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x00085CFB File Offset: 0x00083EFB
		public void RecallShuttle()
		{
			base.SendMessage(new CommunicationsConsoleRecallEmergencyShuttleMessage());
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x00085D08 File Offset: 0x00083F08
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CommunicationsConsoleInterfaceState communicationsConsoleInterfaceState = state as CommunicationsConsoleInterfaceState;
			if (communicationsConsoleInterfaceState == null)
			{
				return;
			}
			this.CanAnnounce = communicationsConsoleInterfaceState.CanAnnounce;
			this.CanCall = communicationsConsoleInterfaceState.CanCall;
			this._expectedCountdownTime = communicationsConsoleInterfaceState.ExpectedCountdownEnd;
			this.CountdownStarted = communicationsConsoleInterfaceState.CountdownStarted;
			this.AlertLevelSelectable = (communicationsConsoleInterfaceState.AlertLevels != null && !float.IsNaN(communicationsConsoleInterfaceState.CurrentAlertDelay) && communicationsConsoleInterfaceState.CurrentAlertDelay <= 0f);
			this.CurrentLevel = communicationsConsoleInterfaceState.CurrentAlert;
			if (this._menu != null)
			{
				this._menu.UpdateCountdown();
				this._menu.UpdateAlertLevels(communicationsConsoleInterfaceState.AlertLevels, this.CurrentLevel);
				this._menu.AlertLevelButton.Disabled = !this.AlertLevelSelectable;
				this._menu.EmergencyShuttleButton.Disabled = !this.CanCall;
				this._menu.AnnounceButton.Disabled = !this.CanAnnounce;
			}
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00085E06 File Offset: 0x00084006
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			CommunicationsConsoleMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000BEE RID: 3054
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000BEF RID: 3055
		[Nullable(2)]
		[ViewVariables]
		private CommunicationsConsoleMenu _menu;

		// Token: 0x04000BF5 RID: 3061
		private TimeSpan? _expectedCountdownTime;
	}
}
