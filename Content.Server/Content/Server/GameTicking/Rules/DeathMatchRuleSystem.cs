using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Shared.CCVar;
using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004B7 RID: 1207
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeathMatchRuleSystem : GameRuleSystem
	{
		// Token: 0x1700037A RID: 890
		// (get) Token: 0x0600188E RID: 6286 RVA: 0x0007F9BC File Offset: 0x0007DBBC
		public override string Prototype
		{
			get
			{
				return "DeathMatch";
			}
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x0007F9C3 File Offset: 0x0007DBC3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DamageChangedEvent>(new EntityEventHandler<DamageChangedEvent>(this.OnHealthChanged), null, null);
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x0007F9E0 File Offset: 0x0007DBE0
		public override void Started()
		{
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-death-match-added-announcement"), null);
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x0007FA22 File Offset: 0x0007DC22
		public override void Ended()
		{
			this._deadCheckTimer = null;
			this._restartTimer = null;
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x0007FA53 File Offset: 0x0007DC53
		private void OnHealthChanged(DamageChangedEvent _)
		{
			this.RunDelayedCheck();
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0007FA5B File Offset: 0x0007DC5B
		private void OnPlayerStatusChanged([Nullable(2)] object _, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 4)
			{
				this.RunDelayedCheck();
			}
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x0007FA6C File Offset: 0x0007DC6C
		private void RunDelayedCheck()
		{
			if (!base.RuleAdded || this._deadCheckTimer != null)
			{
				return;
			}
			this._deadCheckTimer = new float?(5f);
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0007FA94 File Offset: 0x0007DC94
		public override void Update(float frameTime)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (this._restartTimer != null)
			{
				this._restartTimer -= frameTime;
				float? num = this._restartTimer;
				float num2 = 0f;
				if (num.GetValueOrDefault() > num2 & num != null)
				{
					return;
				}
				this.GameTicker.EndRound("");
				this.GameTicker.RestartRound();
				return;
			}
			else
			{
				if (!this._cfg.GetCVar<bool>(CCVars.GameLobbyEnableWin) || this._deadCheckTimer == null)
				{
					return;
				}
				this._deadCheckTimer -= frameTime;
				float? num = this._deadCheckTimer;
				float num2 = 0f;
				if (num.GetValueOrDefault() > num2 & num != null)
				{
					return;
				}
				this._deadCheckTimer = null;
				IPlayerSession winner = null;
				foreach (IPlayerSession playerSession in this._playerManager.ServerSessions)
				{
					EntityUid? attachedEntity = playerSession.AttachedEntity;
					if (attachedEntity != null)
					{
						EntityUid playerEntity = attachedEntity.GetValueOrDefault();
						MobStateComponent state;
						if (playerEntity.Valid && base.TryComp<MobStateComponent>(playerEntity, ref state) && this._mobStateSystem.IsAlive(playerEntity, state))
						{
							if (winner != null)
							{
								return;
							}
							winner = playerSession;
						}
					}
				}
				this._chatManager.DispatchServerAnnouncement((winner == null) ? Loc.GetString("rule-death-match-check-winner-stalemate") : Loc.GetString("rule-death-match-check-winner", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("winner", winner)
				}), null);
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-restarting-in-seconds", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("seconds", 10f)
				}), null);
				this._restartTimer = new float?(10f);
				return;
			}
		}

		// Token: 0x04000F4B RID: 3915
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000F4C RID: 3916
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F4D RID: 3917
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000F4E RID: 3918
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000F4F RID: 3919
		private const float RestartDelay = 10f;

		// Token: 0x04000F50 RID: 3920
		private const float DeadCheckDelay = 5f;

		// Token: 0x04000F51 RID: 3921
		private float? _deadCheckTimer;

		// Token: 0x04000F52 RID: 3922
		private float? _restartTimer;
	}
}
