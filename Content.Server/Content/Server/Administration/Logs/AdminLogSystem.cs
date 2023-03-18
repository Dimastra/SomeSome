using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Logs
{
	// Token: 0x0200081A RID: 2074
	public sealed class AdminLogSystem : EntitySystem
	{
		// Token: 0x06002D83 RID: 11651 RVA: 0x000EF977 File Offset: 0x000EDB77
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundStartingEvent>(delegate(RoundStartingEvent ev)
			{
				this._adminLogs.RoundStarting(ev.Id);
			}, null, null);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(delegate(GameRunLevelChangedEvent ev)
			{
				this._adminLogs.RunLevelChanged(ev.New);
			}, null, null);
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x000EF9A7 File Offset: 0x000EDBA7
		public override void Update(float frameTime)
		{
			this._adminLogs.Update();
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x000EF9B4 File Offset: 0x000EDBB4
		public override void Shutdown()
		{
			this._adminLogs.Shutdown();
		}

		// Token: 0x04001C31 RID: 7217
		[Nullable(1)]
		[Dependency]
		private readonly IAdminLogManager _adminLogs;
	}
}
