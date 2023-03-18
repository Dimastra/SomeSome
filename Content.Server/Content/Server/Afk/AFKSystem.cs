using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Afk.Events;
using Content.Server.GameTicking;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Timing;

namespace Content.Server.Afk
{
	// Token: 0x020007F4 RID: 2036
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AFKSystem : EntitySystem
	{
		// Token: 0x06002C0C RID: 11276 RVA: 0x000E6ABA File Offset: 0x000E4CBA
		public override void Initialize()
		{
			base.Initialize();
			this._playerManager.PlayerStatusChanged += this.OnPlayerChange;
			this._configManager.OnValueChanged<float>(CCVars.AfkTime, new Action<float>(this.SetAfkDelay), true);
		}

		// Token: 0x06002C0D RID: 11277 RVA: 0x000E6AF6 File Offset: 0x000E4CF6
		private void SetAfkDelay(float obj)
		{
			this._checkDelay = obj;
		}

		// Token: 0x06002C0E RID: 11278 RVA: 0x000E6AFF File Offset: 0x000E4CFF
		private void OnPlayerChange([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 4)
			{
				this._afkPlayers.Remove(e.Session);
			}
		}

		// Token: 0x06002C0F RID: 11279 RVA: 0x000E6B1C File Offset: 0x000E4D1C
		public override void Shutdown()
		{
			base.Shutdown();
			this._afkPlayers.Clear();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerChange;
			this._configManager.UnsubValueChanged<float>(CCVars.AfkTime, new Action<float>(this.SetAfkDelay));
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x000E6B70 File Offset: 0x000E4D70
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this._ticker.RunLevel != GameRunLevel.InRound)
			{
				this._afkPlayers.Clear();
				this._checkTime = TimeSpan.Zero;
				return;
			}
			if (this._timing.CurTime < this._checkTime)
			{
				return;
			}
			this._checkTime = this._timing.CurTime + TimeSpan.FromSeconds((double)this._checkDelay);
			foreach (ICommonSession session in Filter.GetAllPlayers(null))
			{
				if (session.Status == 3)
				{
					IPlayerSession pSession = (IPlayerSession)session;
					bool isAfk = this._afkManager.IsAfk(pSession);
					if (isAfk && this._afkPlayers.Add(pSession))
					{
						AFKEvent ev = new AFKEvent(pSession);
						base.RaiseLocalEvent<AFKEvent>(ref ev);
					}
					else if (!isAfk && this._afkPlayers.Remove(pSession))
					{
						UnAFKEvent ev2 = new UnAFKEvent(pSession);
						base.RaiseLocalEvent<UnAFKEvent>(ref ev2);
					}
				}
			}
		}

		// Token: 0x04001B48 RID: 6984
		[Dependency]
		private readonly IAfkManager _afkManager;

		// Token: 0x04001B49 RID: 6985
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04001B4A RID: 6986
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B4B RID: 6987
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04001B4C RID: 6988
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x04001B4D RID: 6989
		private float _checkDelay;

		// Token: 0x04001B4E RID: 6990
		private TimeSpan _checkTime;

		// Token: 0x04001B4F RID: 6991
		private readonly HashSet<IPlayerSession> _afkPlayers = new HashSet<IPlayerSession>();
	}
}
