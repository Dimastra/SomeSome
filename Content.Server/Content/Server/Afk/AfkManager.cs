using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Afk
{
	// Token: 0x020007F3 RID: 2035
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AfkManager : IAfkManager, IEntityEventSubscriber
	{
		// Token: 0x06002C05 RID: 11269 RVA: 0x000E697C File Offset: 0x000E4B7C
		public void Initialize()
		{
			this._playerManager.PlayerStatusChanged += this.PlayerStatusChanged;
			this._consoleHost.AnyCommandExecuted += new ConAnyCommandCallback(this.ConsoleHostOnAnyCommandExecuted);
			EventBusExt.SubscribeSessionEvent<FullInputCmdMessage>(this._entityManager.EventBus, 2, this, new EntitySessionEventHandler<FullInputCmdMessage>(this.HandleInputCmd));
		}

		// Token: 0x06002C06 RID: 11270 RVA: 0x000E69D5 File Offset: 0x000E4BD5
		public void PlayerDidAction(IPlayerSession player)
		{
			if (player.Status == 4)
			{
				return;
			}
			this._lastActionTimes[player] = this._gameTiming.RealTime;
		}

		// Token: 0x06002C07 RID: 11271 RVA: 0x000E69F8 File Offset: 0x000E4BF8
		public bool IsAfk(IPlayerSession player)
		{
			TimeSpan time;
			if (!this._lastActionTimes.TryGetValue(player, out time))
			{
				return true;
			}
			TimeSpan timeOut = TimeSpan.FromSeconds((double)this._cfg.GetCVar<float>(CCVars.AfkTime));
			return this._gameTiming.RealTime - time > timeOut;
		}

		// Token: 0x06002C08 RID: 11272 RVA: 0x000E6A45 File Offset: 0x000E4C45
		private void PlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 4)
			{
				this._lastActionTimes.Remove(e.Session);
				return;
			}
			this.PlayerDidAction(e.Session);
		}

		// Token: 0x06002C09 RID: 11273 RVA: 0x000E6A70 File Offset: 0x000E4C70
		private void ConsoleHostOnAnyCommandExecuted(IConsoleShell shell, string commandname, string argstr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player != null)
			{
				this.PlayerDidAction(player);
			}
		}

		// Token: 0x06002C0A RID: 11274 RVA: 0x000E6A93 File Offset: 0x000E4C93
		private void HandleInputCmd(FullInputCmdMessage msg, EntitySessionEventArgs args)
		{
			this.PlayerDidAction((IPlayerSession)args.SenderSession);
		}

		// Token: 0x04001B42 RID: 6978
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04001B43 RID: 6979
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B44 RID: 6980
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001B45 RID: 6981
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04001B46 RID: 6982
		[Dependency]
		private readonly IConsoleHost _consoleHost;

		// Token: 0x04001B47 RID: 6983
		private readonly Dictionary<IPlayerSession, TimeSpan> _lastActionTimes = new Dictionary<IPlayerSession, TimeSpan>();
	}
}
