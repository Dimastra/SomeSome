using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Shared.CCVar;
using Robust.Server;
using Robust.Server.Player;
using Robust.Server.ServerStatus;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Server.ServerUpdates
{
	// Token: 0x02000212 RID: 530
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerUpdateManager
	{
		// Token: 0x06000A81 RID: 2689 RVA: 0x00037361 File Offset: 0x00035561
		public void Initialize()
		{
			this._watchdog.UpdateReceived += this.WatchdogOnUpdateReceived;
			this._playerManager.PlayerStatusChanged += this.PlayerManagerOnPlayerStatusChanged;
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00037394 File Offset: 0x00035594
		public void Update()
		{
			if (this._restartTime != null && this._restartTime < this._gameTiming.RealTime)
			{
				this.DoShutdown();
			}
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x000373E2 File Offset: 0x000355E2
		public bool RoundEnded()
		{
			if (this._updateOnRoundEnd)
			{
				this.DoShutdown();
				return true;
			}
			return false;
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x000373F8 File Offset: 0x000355F8
		private void PlayerManagerOnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			SessionStatus newStatus = e.NewStatus;
			if (newStatus == 1)
			{
				this._restartTime = null;
				return;
			}
			if (newStatus != 4)
			{
				return;
			}
			this.ServerEmptyUpdateRestartCheck();
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x00037428 File Offset: 0x00035628
		private void WatchdogOnUpdateReceived()
		{
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("server-updates-received"), null);
			this._updateOnRoundEnd = true;
			this.ServerEmptyUpdateRestartCheck();
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00037460 File Offset: 0x00035660
		private void ServerEmptyUpdateRestartCheck()
		{
			if (this._playerManager.Sessions.Any((ICommonSession p) => p.Status != 4) || !this._updateOnRoundEnd)
			{
				return;
			}
			if (this._restartTime != null)
			{
				return;
			}
			TimeSpan restartDelay = TimeSpan.FromSeconds((double)this._cfg.GetCVar<float>(CCVars.UpdateRestartDelay));
			this._restartTime = new TimeSpan?(restartDelay + this._gameTiming.RealTime);
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x000374E8 File Offset: 0x000356E8
		private void DoShutdown()
		{
			this._server.Shutdown(Loc.GetString("server-updates-shutdown"));
		}

		// Token: 0x04000669 RID: 1641
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400066A RID: 1642
		[Dependency]
		private readonly IWatchdogApi _watchdog;

		// Token: 0x0400066B RID: 1643
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400066C RID: 1644
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x0400066D RID: 1645
		[Dependency]
		private readonly IBaseServer _server;

		// Token: 0x0400066E RID: 1646
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400066F RID: 1647
		[ViewVariables]
		private bool _updateOnRoundEnd;

		// Token: 0x04000670 RID: 1648
		private TimeSpan? _restartTime;
	}
}
