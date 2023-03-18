using System;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Administration.Notes;
using Content.Server.Afk;
using Content.Server.Chat.Managers;
using Content.Server.Connection;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.GhostKick;
using Content.Server.Info;
using Content.Server.Maps;
using Content.Server.MoMMI;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.Objectives;
using Content.Server.Objectives.Interfaces;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Content.Server.ServerInfo;
using Content.Server.ServerUpdates;
using Content.Server.UtkaIntegration;
using Content.Server.Voting.Managers;
using Content.Server.White.JoinQueue;
using Content.Server.White.Sponsors;
using Content.Server.White.Stalin;
using Content.Server.White.TTS;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Kitchen;
using Robust.Shared.IoC;

namespace Content.Server.IoC
{
	// Token: 0x02000441 RID: 1089
	internal static class ServerContentIoC
	{
		// Token: 0x06001601 RID: 5633 RVA: 0x0007451C File Offset: 0x0007271C
		public static void Register()
		{
			IoCManager.Register<IChatManager, ChatManager>(false);
			IoCManager.Register<IChatSanitizationManager, ChatSanitizationManager>(false);
			IoCManager.Register<IMoMMILink, MoMMILink>(false);
			IoCManager.Register<IServerPreferencesManager, ServerPreferencesManager>(false);
			IoCManager.Register<IServerDbManager, ServerDbManager>(false);
			IoCManager.Register<RecipeManager, RecipeManager>(false);
			IoCManager.Register<INodeGroupFactory, NodeGroupFactory>(false);
			IoCManager.Register<IConnectionManager, ConnectionManager>(false);
			IoCManager.Register<ServerUpdateManager>(false);
			IoCManager.Register<IObjectivesManager, ObjectivesManager>(false);
			IoCManager.Register<IAdminManager, AdminManager>(false);
			IoCManager.Register<EuiManager, EuiManager>(false);
			IoCManager.Register<IVoteManager, VoteManager>(false);
			IoCManager.Register<IPlayerLocator, PlayerLocator>(false);
			IoCManager.Register<IAfkManager, AfkManager>(false);
			IoCManager.Register<IGameMapManager, GameMapManager>(false);
			IoCManager.Register<IGamePrototypeLoadManager, GamePrototypeLoadManager>(false);
			IoCManager.Register<RulesManager, RulesManager>(false);
			IoCManager.Register<RoleBanManager, RoleBanManager>(false);
			IoCManager.Register<NetworkResourceManager>(false);
			IoCManager.Register<IAdminNotesManager, AdminNotesManager>(false);
			IoCManager.Register<GhostKickManager>(false);
			IoCManager.Register<ISharedAdminLogManager, AdminLogManager>(false);
			IoCManager.Register<IAdminLogManager, AdminLogManager>(false);
			IoCManager.Register<PlayTimeTrackingManager>(false);
			IoCManager.Register<UserDbDataManager>(false);
			IoCManager.Register<ServerInfoManager>(false);
			IoCManager.Register<SponsorsManager>(false);
			IoCManager.Register<JoinQueueManager>(false);
			IoCManager.Register<TTSManager>(false);
			IoCManager.Register<UtkaTCPWrapper>(false);
			IoCManager.Register<StalinManager>(false);
		}
	}
}
