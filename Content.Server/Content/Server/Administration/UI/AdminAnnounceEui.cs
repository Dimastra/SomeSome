using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Administration.UI
{
	// Token: 0x02000806 RID: 2054
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminAnnounceEui : BaseEui
	{
		// Token: 0x06002C88 RID: 11400 RVA: 0x000E82B2 File Offset: 0x000E64B2
		public AdminAnnounceEui()
		{
			IoCManager.InjectDependencies<AdminAnnounceEui>(this);
			this._chatSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ChatSystem>();
		}

		// Token: 0x06002C89 RID: 11401 RVA: 0x000E82D1 File Offset: 0x000E64D1
		public override void Opened()
		{
			base.StateDirty();
		}

		// Token: 0x06002C8A RID: 11402 RVA: 0x000E82D9 File Offset: 0x000E64D9
		public override EuiStateBase GetNewState()
		{
			return new AdminAnnounceEuiState();
		}

		// Token: 0x06002C8B RID: 11403 RVA: 0x000E82E0 File Offset: 0x000E64E0
		public override void HandleMessage(EuiMessageBase msg)
		{
			if (msg is AdminAnnounceEuiMsg.Close)
			{
				base.Close();
				return;
			}
			AdminAnnounceEuiMsg.DoAnnounce doAnnounce = msg as AdminAnnounceEuiMsg.DoAnnounce;
			if (doAnnounce == null)
			{
				return;
			}
			if (!this._adminManager.HasAdminFlag(base.Player, AdminFlags.Admin))
			{
				base.Close();
				return;
			}
			AdminAnnounceType announceType = doAnnounce.AnnounceType;
			if (announceType != AdminAnnounceType.Station)
			{
				if (announceType == AdminAnnounceType.Server)
				{
					this._chatManager.DispatchServerAnnouncement(doAnnounce.Announcement, null);
				}
			}
			else
			{
				this._chatSystem.DispatchGlobalAnnouncement(doAnnounce.Announcement, doAnnounce.Announcer, true, null, new Color?(Color.Gold));
			}
			base.StateDirty();
			if (doAnnounce.CloseAfter)
			{
				base.Close();
			}
		}

		// Token: 0x04001B84 RID: 7044
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B85 RID: 7045
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001B86 RID: 7046
		private readonly ChatSystem _chatSystem;
	}
}
