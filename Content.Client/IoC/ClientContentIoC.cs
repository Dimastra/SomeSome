using System;
using Content.Client.Administration.Managers;
using Content.Client.Audio;
using Content.Client.Changelog;
using Content.Client.Chat.Managers;
using Content.Client.Clickable;
using Content.Client.Eui;
using Content.Client.GhostKick;
using Content.Client.Guidebook;
using Content.Client.Info;
using Content.Client.Launcher;
using Content.Client.Parallax.Managers;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Preferences;
using Content.Client.Screenshot;
using Content.Client.Stylesheets;
using Content.Client.Viewport;
using Content.Client.Voting;
using Content.Client.White.JoinQueue;
using Content.Client.White.Sponsors;
using Content.Client.White.Stalin;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Robust.Shared.IoC;

namespace Content.Client.IoC
{
	// Token: 0x020002A0 RID: 672
	internal static class ClientContentIoC
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x000654A0 File Offset: 0x000636A0
		public static void Register()
		{
			IoCManager.Register<IParallaxManager, ParallaxManager>(false);
			IoCManager.Register<IChatManager, ChatManager>(false);
			IoCManager.Register<IClientPreferencesManager, ClientPreferencesManager>(false);
			IoCManager.Register<IStylesheetManager, StylesheetManager>(false);
			IoCManager.Register<IScreenshotHook, ScreenshotHook>(false);
			IoCManager.Register<IClickMapManager, ClickMapManager>(false);
			IoCManager.Register<IClientAdminManager, ClientAdminManager>(false);
			IoCManager.Register<EuiManager, EuiManager>(false);
			IoCManager.Register<IVoteManager, VoteManager>(false);
			IoCManager.Register<ChangelogManager, ChangelogManager>(false);
			IoCManager.Register<RulesManager, RulesManager>(false);
			IoCManager.Register<ViewportManager, ViewportManager>(false);
			IoCManager.Register<IGamePrototypeLoadManager, GamePrototypeLoadManager>(false);
			IoCManager.Register<NetworkResourceManager>(false);
			IoCManager.Register<ISharedAdminLogManager, SharedAdminLogManager>(false);
			IoCManager.Register<GhostKickManager>(false);
			IoCManager.Register<ExtendedDisconnectInformationManager>(false);
			IoCManager.Register<PlayTimeTrackingManager>(false);
			IoCManager.Register<SponsorsManager>(false);
			IoCManager.Register<JoinQueueManager>(false);
			IoCManager.Register<UIAudioManager>(false);
			IoCManager.Register<DocumentParsingManager>(false);
			IoCManager.Register<StalinManager>(false);
		}
	}
}
