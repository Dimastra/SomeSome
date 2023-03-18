using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.MachineLinking.UI
{
	// Token: 0x02000258 RID: 600
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalTimerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000F31 RID: 3889 RVA: 0x000021BC File Offset: 0x000003BC
		public SignalTimerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0005B3B4 File Offset: 0x000595B4
		protected override void Open()
		{
			base.Open();
			this._window = new SignalTimerWindow(this);
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.OnCurrentTextChanged += this.OnTextChanged;
			this._window.OnCurrentDelayMinutesChanged += this.OnDelayChanged;
			this._window.OnCurrentDelaySecondsChanged += this.OnDelayChanged;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0005B44E File Offset: 0x0005964E
		public void OnStartTimer()
		{
			base.SendMessage(new SignalTimerStartMessage(base.Owner.Owner));
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0005B466 File Offset: 0x00059666
		private void OnTextChanged(string newText)
		{
			base.SendMessage(new SignalTimerTextChangedMessage(newText));
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0005B474 File Offset: 0x00059674
		private void OnDelayChanged(string newDelay)
		{
			if (this._window == null)
			{
				return;
			}
			base.SendMessage(new SignalTimerDelayChangedMessage(this._window.GetDelay()));
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0005B495 File Offset: 0x00059695
		public TimeSpan GetCurrentTime()
		{
			return this._gameTiming.CurTime;
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0005B4A4 File Offset: 0x000596A4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				SignalTimerBoundUserInterfaceState signalTimerBoundUserInterfaceState = state as SignalTimerBoundUserInterfaceState;
				if (signalTimerBoundUserInterfaceState != null)
				{
					this._window.SetCurrentText(signalTimerBoundUserInterfaceState.CurrentText);
					this._window.SetCurrentDelayMinutes(signalTimerBoundUserInterfaceState.CurrentDelayMinutes);
					this._window.SetCurrentDelaySeconds(signalTimerBoundUserInterfaceState.CurrentDelaySeconds);
					this._window.SetShowText(signalTimerBoundUserInterfaceState.ShowText);
					this._window.SetTriggerTime(signalTimerBoundUserInterfaceState.TriggerTime);
					this._window.SetTimerStarted(signalTimerBoundUserInterfaceState.TimerStarted);
					this._window.SetHasAccess(signalTimerBoundUserInterfaceState.HasAccess);
					return;
				}
			}
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0005B542 File Offset: 0x00059742
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			SignalTimerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x0400078E RID: 1934
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400078F RID: 1935
		[Nullable(2)]
		private SignalTimerWindow _window;
	}
}
