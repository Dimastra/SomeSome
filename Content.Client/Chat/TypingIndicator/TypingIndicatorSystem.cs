using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Chat.TypingIndicator;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Chat.TypingIndicator
{
	// Token: 0x020003E7 RID: 999
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TypingIndicatorSystem : SharedTypingIndicatorSystem
	{
		// Token: 0x06001890 RID: 6288 RVA: 0x0008DB6B File Offset: 0x0008BD6B
		public override void Initialize()
		{
			base.Initialize();
			this._cfg.OnValueChanged<bool>(CCVars.ChatShowTypingIndicator, new Action<bool>(this.OnShowTypingChanged), false);
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0008DB90 File Offset: 0x0008BD90
		public void ClientChangedChatText()
		{
			if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
			{
				return;
			}
			this._isClientTyping = true;
			this.ClientUpdateTyping();
			this._lastTextChange = this._time.CurTime;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0008DBC3 File Offset: 0x0008BDC3
		public void ClientSubmittedChatText()
		{
			if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
			{
				return;
			}
			this._isClientChatFocused = false;
			this._isClientTyping = false;
			this.ClientUpdateTyping();
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0008DBEC File Offset: 0x0008BDEC
		public void ClientChangedChatFocus(bool isFocused)
		{
			if (!this._cfg.GetCVar<bool>(CCVars.ChatShowTypingIndicator))
			{
				return;
			}
			this._isClientChatFocused = isFocused;
			this.ClientUpdateTyping();
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0008DC10 File Offset: 0x0008BE10
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this._isClientTyping && this._time.CurTime - this._lastTextChange > this._typingTimeout)
			{
				this._isClientTyping = false;
				this.ClientUpdateTyping();
			}
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0008DC5C File Offset: 0x0008BE5C
		private void ClientUpdateTyping()
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null || localPlayer.ControlledEntity == null)
			{
				return;
			}
			TypingIndicatorState state = TypingIndicatorState.None;
			if (this._isClientChatFocused)
			{
				state = (this._isClientTyping ? TypingIndicatorState.Typing : TypingIndicatorState.Idle);
			}
			base.RaiseNetworkEvent(new TypingChangedEvent(state));
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x0008DCB1 File Offset: 0x0008BEB1
		private void OnShowTypingChanged(bool showTyping)
		{
			if (!showTyping)
			{
				this._isClientTyping = false;
				this.ClientUpdateTyping();
			}
		}

		// Token: 0x04000C88 RID: 3208
		[Dependency]
		private readonly IGameTiming _time;

		// Token: 0x04000C89 RID: 3209
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000C8A RID: 3210
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000C8B RID: 3211
		private readonly TimeSpan _typingTimeout = TimeSpan.FromSeconds(2.0);

		// Token: 0x04000C8C RID: 3212
		private TimeSpan _lastTextChange;

		// Token: 0x04000C8D RID: 3213
		private bool _isClientTyping;

		// Token: 0x04000C8E RID: 3214
		private bool _isClientChatFocused;
	}
}
