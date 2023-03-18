using System;
using Content.Client.Access.UI;
using Content.Client.Administration.UI;
using Content.Client.Administration.UI.BanList;
using Content.Client.Administration.UI.Bwoink;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.Administration.UI.Logs;
using Content.Client.Administration.UI.ManageSolutions;
using Content.Client.Administration.UI.Notes;
using Content.Client.Administration.UI.SetOutfit;
using Content.Client.Administration.UI.SpawnExplosion;
using Content.Client.Administration.UI.Tabs;
using Content.Client.Administration.UI.Tabs.AdminbusTab;
using Content.Client.Administration.UI.Tabs.AdminTab;
using Content.Client.Administration.UI.Tabs.AtmosTab;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.AirlockPainter.UI;
using Content.Client.AME.UI;
using Content.Client.Anomaly.Ui;
using Content.Client.Atmos.Monitor.UI;
using Content.Client.Atmos.Monitor.UI.Widgets;
using Content.Client.Atmos.UI;
using Content.Client.Borgs;
using Content.Client.Cargo.UI;
using Content.Client.CartridgeLoader.Cartridges;
using Content.Client.Changelog;
using Content.Client.Chemistry.UI;
using Content.Client.CloningConsole.UI;
using Content.Client.Clothing.UI;
using Content.Client.Construction.UI;
using Content.Client.ContextMenu.UI;
using Content.Client.Crayon.UI;
using Content.Client.Credits;
using Content.Client.CrewManifest;
using Content.Client.Decals.UI;
using Content.Client.Disposal.UI;
using Content.Client.Fax.UI;
using Content.Client.FlavorText;
using Content.Client.Fluids.UI;
using Content.Client.Forensics;
using Content.Client.Gravity.UI;
using Content.Client.Guidebook.Controls;
using Content.Client.Hands.UI;
using Content.Client.HealthAnalyzer.UI;
using Content.Client.Humanoid;
using Content.Client.Info;
using Content.Client.Instruments.UI;
using Content.Client.Kitchen.UI;
using Content.Client.Labels.UI;
using Content.Client.Lathe.UI;
using Content.Client.Launcher;
using Content.Client.Lobby.UI;
using Content.Client.MachineLinking.UI;
using Content.Client.MagicMirror;
using Content.Client.MainMenu.UI;
using Content.Client.Mech.Ui;
using Content.Client.Mech.Ui.Equipment;
using Content.Client.Medical.CrewMonitoring;
using Content.Client.NetworkConfigurator;
using Content.Client.NPC;
using Content.Client.Nuke;
using Content.Client.Options.UI;
using Content.Client.Options.UI.Tabs;
using Content.Client.Paper.UI;
using Content.Client.PDA;
using Content.Client.PDA.Ringer;
using Content.Client.Power;
using Content.Client.Power.APC.UI;
using Content.Client.Preferences.UI;
using Content.Client.Research.UI;
using Content.Client.Shuttles.UI;
using Content.Client.StationRecords;
using Content.Client.Store.Ui;
using Content.Client.SurveillanceCamera.UI;
using Content.Client.Suspicion;
using Content.Client.Tabletop.UI;
using Content.Client.Targeting.UI;
using Content.Client.UserInterface;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Controls.FancyTree;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Actions.Controls;
using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Actions.Windows;
using Content.Client.UserInterface.Systems.Alerts.Widgets;
using Content.Client.UserInterface.Systems.Character.Controls;
using Content.Client.UserInterface.Systems.Character.Windows;
using Content.Client.UserInterface.Systems.Chat.Controls;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Client.UserInterface.Systems.Emotions.Windows;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Content.Client.UserInterface.Systems.Ghost.Controls.Roles;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Content.Client.UserInterface.Systems.Inventory.Windows;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Objectives.Controls;
using Content.Client.UserInterface.Systems.Sandbox.Windows;
using Content.Client.VendingMachines.UI;
using Content.Client.VoiceMask;
using Content.Client.Voting.UI;
using Content.Client.White.JoinQueue;
using Content.Client.Xenoarchaeology.Ui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;

namespace CompiledRobustXaml
{
	// Token: 0x02000507 RID: 1287
	internal class !EmbeddedResource
	{
		// Token: 0x060020A3 RID: 8355 RVA: 0x000BDE40 File Offset: 0x000BC040
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AgentIDCardWindow.Populate:Content.Client.Access.UI.AgentIDCardWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x000BDE64 File Offset: 0x000BC064
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			IdCardConsoleWindow.Populate:Content.Client.Access.UI.IdCardConsoleWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x000BDE88 File Offset: 0x000BC088
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminAnnounceWindow.Populate:Content.Client.Administration.UI.AdminAnnounceWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020A6 RID: 8358 RVA: 0x000BDEAC File Offset: 0x000BC0AC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminMenuWindow.Populate:Content.Client.Administration.UI.AdminMenuWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x000BDED0 File Offset: 0x000BC0D0
		public static BanListControl xaml(IServiceProvider A_0)
		{
			BanListControl banListControl = new BanListControl();
			BanListControl.Populate:Content.Client.Administration.UI.BanList.BanListControl.xaml(A_0, banListControl);
			return banListControl;
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x000BDEF4 File Offset: 0x000BC0F4
		public static ContainerButton xaml(IServiceProvider A_0)
		{
			ContainerButton containerButton = new ContainerButton();
			BanListHeader.Populate:Content.Client.Administration.UI.BanList.BanListHeader.xaml(A_0, containerButton);
			return containerButton;
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000BDF18 File Offset: 0x000BC118
		public static Popup xaml(IServiceProvider A_0)
		{
			Popup popup = new Popup();
			BanListIdsPopup.Populate:Content.Client.Administration.UI.BanList.BanListIdsPopup.xaml(A_0, popup);
			return popup;
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x000BDF3C File Offset: 0x000BC13C
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			BanListLine.Populate:Content.Client.Administration.UI.BanList.BanListLine.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020AB RID: 8363 RVA: 0x000BDF60 File Offset: 0x000BC160
		public static BanListWindow xaml(IServiceProvider A_0)
		{
			BanListWindow banListWindow = new BanListWindow();
			BanListWindow.Populate:Content.Client.Administration.UI.BanList.BanListWindow.xaml(A_0, banListWindow);
			return banListWindow;
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x000BDF84 File Offset: 0x000BC184
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			BwoinkControl.Populate:Content.Client.Administration.UI.Bwoink.BwoinkControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x000BDFA8 File Offset: 0x000BC1A8
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			BwoinkPanel.Populate:Content.Client.Administration.UI.Bwoink.BwoinkPanel.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x000BDFCC File Offset: 0x000BC1CC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			BwoinkWindow.Populate:Content.Client.Administration.UI.Bwoink.BwoinkWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x000BDFF0 File Offset: 0x000BC1F0
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			PlayerListControl.Populate:Content.Client.Administration.UI.CustomControls.PlayerListControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x000BE014 File Offset: 0x000BC214
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AdminLogsControl.Populate:Content.Client.Administration.UI.Logs.AdminLogsControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x000BE038 File Offset: 0x000BC238
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminLogsWindow.Populate:Content.Client.Administration.UI.Logs.AdminLogsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x000BE05C File Offset: 0x000BC25C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AddReagentWindow.Populate:Content.Client.Administration.UI.ManageSolutions.AddReagentWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x000BE080 File Offset: 0x000BC280
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			EditSolutionsWindow.Populate:Content.Client.Administration.UI.ManageSolutions.EditSolutionsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x000BE0A4 File Offset: 0x000BC2A4
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AdminNotesControl.Populate:Content.Client.Administration.UI.Notes.AdminNotesControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x000BE0C8 File Offset: 0x000BC2C8
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			AdminNotesLine.Populate:Content.Client.Administration.UI.Notes.AdminNotesLine.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020B6 RID: 8374 RVA: 0x000BE0EC File Offset: 0x000BC2EC
		public static Popup xaml(IServiceProvider A_0)
		{
			Popup popup = new Popup();
			AdminNotesLinePopup.Populate:Content.Client.Administration.UI.Notes.AdminNotesLinePopup.xaml(A_0, popup);
			return popup;
		}

		// Token: 0x060020B7 RID: 8375 RVA: 0x000BE110 File Offset: 0x000BC310
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminNotesWindow.Populate:Content.Client.Administration.UI.Notes.AdminNotesWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020B8 RID: 8376 RVA: 0x000BE134 File Offset: 0x000BC334
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SetOutfitMenu.Populate:Content.Client.Administration.UI.SetOutfit.SetOutfitMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020B9 RID: 8377 RVA: 0x000BE158 File Offset: 0x000BC358
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SpawnExplosionWindow.Populate:Content.Client.Administration.UI.SpawnExplosion.SpawnExplosionWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020BA RID: 8378 RVA: 0x000BE17C File Offset: 0x000BC37C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AdminbusTab.Populate:Content.Client.Administration.UI.Tabs.AdminbusTab.AdminbusTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020BB RID: 8379 RVA: 0x000BE1A0 File Offset: 0x000BC3A0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			LoadBlueprintsWindow.Populate:Content.Client.Administration.UI.Tabs.AdminbusTab.LoadBlueprintsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x000BE1C4 File Offset: 0x000BC3C4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminShuttleCallEnableWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleCallEnableWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x000BE1E8 File Offset: 0x000BC3E8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AdminShuttleWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x000BE20C File Offset: 0x000BC40C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AdminTab.Populate:Content.Client.Administration.UI.Tabs.AdminTab.AdminTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020BF RID: 8383 RVA: 0x000BE230 File Offset: 0x000BC430
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			BanWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.BanWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x000BE254 File Offset: 0x000BC454
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			PlayerActionsWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.PlayerActionsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x000BE278 File Offset: 0x000BC478
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			RoleBanWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.RoleBanWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C2 RID: 8386 RVA: 0x000BE29C File Offset: 0x000BC49C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			TeleportWindow.Populate:Content.Client.Administration.UI.Tabs.AdminTab.TeleportWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C3 RID: 8387 RVA: 0x000BE2C0 File Offset: 0x000BC4C0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AddAtmosWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.AddAtmosWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x000BE2E4 File Offset: 0x000BC4E4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AddGasWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.AddGasWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x000BE308 File Offset: 0x000BC508
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AtmosTab.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.AtmosTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020C6 RID: 8390 RVA: 0x000BE32C File Offset: 0x000BC52C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			FillGasWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.FillGasWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C7 RID: 8391 RVA: 0x000BE350 File Offset: 0x000BC550
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SetTemperatureWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.SetTemperatureWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020C8 RID: 8392 RVA: 0x000BE374 File Offset: 0x000BC574
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			ObjectsTab.Populate:Content.Client.Administration.UI.Tabs.ObjectsTab.ObjectsTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020C9 RID: 8393 RVA: 0x000BE398 File Offset: 0x000BC598
		public static ContainerButton xaml(IServiceProvider A_0)
		{
			ContainerButton containerButton = new ContainerButton();
			ObjectsTabEntry.Populate:Content.Client.Administration.UI.Tabs.ObjectsTab.ObjectsTabEntry.xaml(A_0, containerButton);
			return containerButton;
		}

		// Token: 0x060020CA RID: 8394 RVA: 0x000BE3BC File Offset: 0x000BC5BC
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			PlayerTab.Populate:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020CB RID: 8395 RVA: 0x000BE3E0 File Offset: 0x000BC5E0
		public static ContainerButton xaml(IServiceProvider A_0)
		{
			ContainerButton containerButton = new ContainerButton();
			PlayerTabEntry.Populate:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTabEntry.xaml(A_0, containerButton);
			return containerButton;
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x000BE404 File Offset: 0x000BC604
		public static ContainerButton xaml(IServiceProvider A_0)
		{
			ContainerButton containerButton = new ContainerButton();
			PlayerTabHeader.Populate:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTabHeader.xaml(A_0, containerButton);
			return containerButton;
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x000BE428 File Offset: 0x000BC628
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			RoundTab.Populate:Content.Client.Administration.UI.Tabs.RoundTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020CE RID: 8398 RVA: 0x000BE44C File Offset: 0x000BC64C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			ServerTab.Populate:Content.Client.Administration.UI.Tabs.ServerTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x000BE470 File Offset: 0x000BC670
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AirlockPainterWindow.Populate:Content.Client.AirlockPainter.UI.AirlockPainterWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020D0 RID: 8400 RVA: 0x000BE494 File Offset: 0x000BC694
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			AMEWindow.Populate:Content.Client.AME.UI.AMEWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020D1 RID: 8401 RVA: 0x000BE4B8 File Offset: 0x000BC6B8
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			AnomalyGeneratorWindow.Populate:Content.Client.Anomaly.Ui.AnomalyGeneratorWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020D2 RID: 8402 RVA: 0x000BE4DC File Offset: 0x000BC6DC
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			AnomalyScannerMenu.Populate:Content.Client.Anomaly.Ui.AnomalyScannerMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020D3 RID: 8403 RVA: 0x000BE500 File Offset: 0x000BC700
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			AirAlarmWindow.Populate:Content.Client.Atmos.Monitor.UI.AirAlarmWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020D4 RID: 8404 RVA: 0x000BE524 File Offset: 0x000BC724
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			PumpControl.Populate:Content.Client.Atmos.Monitor.UI.Widgets.PumpControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020D5 RID: 8405 RVA: 0x000BE548 File Offset: 0x000BC748
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			ScrubberControl.Populate:Content.Client.Atmos.Monitor.UI.Widgets.ScrubberControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020D6 RID: 8406 RVA: 0x000BE56C File Offset: 0x000BC76C
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			SensorInfo.Populate:Content.Client.Atmos.Monitor.UI.Widgets.SensorInfo.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020D7 RID: 8407 RVA: 0x000BE590 File Offset: 0x000BC790
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			ThresholdBoundControl.Populate:Content.Client.Atmos.Monitor.UI.Widgets.ThresholdBoundControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020D8 RID: 8408 RVA: 0x000BE5B4 File Offset: 0x000BC7B4
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			ThresholdControl.Populate:Content.Client.Atmos.Monitor.UI.Widgets.ThresholdControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x000BE5D8 File Offset: 0x000BC7D8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasAnalyzerWindow.Populate:Content.Client.Atmos.UI.GasAnalyzerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x000BE5FC File Offset: 0x000BC7FC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasCanisterWindow.Populate:Content.Client.Atmos.UI.GasCanisterWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x000BE620 File Offset: 0x000BC820
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasFilterWindow.Populate:Content.Client.Atmos.UI.GasFilterWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x000BE644 File Offset: 0x000BC844
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasMixerWindow.Populate:Content.Client.Atmos.UI.GasMixerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x000BE668 File Offset: 0x000BC868
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasPressurePumpWindow.Populate:Content.Client.Atmos.UI.GasPressurePumpWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x000BE68C File Offset: 0x000BC88C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasThermomachineWindow.Populate:Content.Client.Atmos.UI.GasThermomachineWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x000BE6B0 File Offset: 0x000BC8B0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GasVolumePumpWindow.Populate:Content.Client.Atmos.UI.GasVolumePumpWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x000BE6D4 File Offset: 0x000BC8D4
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			LawsMenu.Populate:Content.Client.Borgs.LawsMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x000BE6F8 File Offset: 0x000BC8F8
		public static PanelContainer xaml(IServiceProvider A_0)
		{
			PanelContainer panelContainer = new PanelContainer();
			LawUIContainer.Populate:Content.Client.Borgs.LawUIContainer.xaml(A_0, panelContainer);
			return panelContainer;
		}

		// Token: 0x060020E2 RID: 8418 RVA: 0x000BE71C File Offset: 0x000BC91C
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			CargoConsoleMenu.Populate:Content.Client.Cargo.UI.CargoConsoleMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x000BE740 File Offset: 0x000BC940
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CargoConsoleOrderMenu.Populate:Content.Client.Cargo.UI.CargoConsoleOrderMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x000BE764 File Offset: 0x000BC964
		public static PanelContainer xaml(IServiceProvider A_0)
		{
			PanelContainer panelContainer = new PanelContainer();
			CargoOrderRow.Populate:Content.Client.Cargo.UI.CargoOrderRow.xaml(A_0, panelContainer);
			return panelContainer;
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x000BE788 File Offset: 0x000BC988
		public static PanelContainer xaml(IServiceProvider A_0)
		{
			PanelContainer panelContainer = new PanelContainer();
			CargoProductRow.Populate:Content.Client.Cargo.UI.CargoProductRow.xaml(A_0, panelContainer);
			return panelContainer;
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x000BE7AC File Offset: 0x000BC9AC
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			CargoShuttleMenu.Populate:Content.Client.Cargo.UI.CargoShuttleMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x000BE7D0 File Offset: 0x000BC9D0
		public static NetProbeUiFragment xaml(IServiceProvider A_0)
		{
			NetProbeUiFragment netProbeUiFragment = new NetProbeUiFragment();
			NetProbeUiFragment.Populate:Content.Client.CartridgeLoader.Cartridges.NetProbeUiFragment.xaml(A_0, netProbeUiFragment);
			return netProbeUiFragment;
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x000BE7F4 File Offset: 0x000BC9F4
		public static NotekeeperUiFragment xaml(IServiceProvider A_0)
		{
			NotekeeperUiFragment notekeeperUiFragment = new NotekeeperUiFragment();
			NotekeeperUiFragment.Populate:Content.Client.CartridgeLoader.Cartridges.NotekeeperUiFragment.xaml(A_0, notekeeperUiFragment);
			return notekeeperUiFragment;
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x000BE818 File Offset: 0x000BCA18
		public static ChangelogWindow xaml(IServiceProvider A_0)
		{
			ChangelogWindow changelogWindow = new ChangelogWindow();
			ChangelogWindow.Populate:Content.Client.Changelog.ChangelogWindow.xaml(A_0, changelogWindow);
			return changelogWindow;
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000BE83C File Offset: 0x000BCA3C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ChemMasterWindow.Populate:Content.Client.Chemistry.UI.ChemMasterWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x000BE860 File Offset: 0x000BCA60
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ReagentDispenserWindow.Populate:Content.Client.Chemistry.UI.ReagentDispenserWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x000BE884 File Offset: 0x000BCA84
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			TransferAmountWindow.Populate:Content.Client.Chemistry.UI.TransferAmountWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020ED RID: 8429 RVA: 0x000BE8A8 File Offset: 0x000BCAA8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CloningConsoleWindow.Populate:Content.Client.CloningConsole.UI.CloningConsoleWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020EE RID: 8430 RVA: 0x000BE8CC File Offset: 0x000BCACC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ChameleonMenu.Populate:Content.Client.Clothing.UI.ChameleonMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020EF RID: 8431 RVA: 0x000BE8F0 File Offset: 0x000BCAF0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ConstructionMenu.Populate:Content.Client.Construction.UI.ConstructionMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x000BE914 File Offset: 0x000BCB14
		public static ContainerButton xaml(IServiceProvider A_0)
		{
			ContainerButton containerButton = new ContainerButton();
			ContextMenuElement.Populate:Content.Client.ContextMenu.UI.ContextMenuElement.xaml(A_0, containerButton);
			return containerButton;
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x000BE938 File Offset: 0x000BCB38
		public static Popup xaml(IServiceProvider A_0)
		{
			Popup popup = new Popup();
			ContextMenuPopup.Populate:Content.Client.ContextMenu.UI.ContextMenuPopup.xaml(A_0, popup);
			return popup;
		}

		// Token: 0x060020F2 RID: 8434 RVA: 0x000BE95C File Offset: 0x000BCB5C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CrayonWindow.Populate:Content.Client.Crayon.UI.CrayonWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x000BE980 File Offset: 0x000BCB80
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CreditsWindow.Populate:Content.Client.Credits.CreditsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x000BE9A4 File Offset: 0x000BCBA4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CrewManifestUi.Populate:Content.Client.CrewManifest.CrewManifestUi.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x000BE9C8 File Offset: 0x000BCBC8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			DecalPlacerWindow.Populate:Content.Client.Decals.UI.DecalPlacerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000BE9EC File Offset: 0x000BCBEC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			PaletteColorPicker.Populate:Content.Client.Decals.UI.PaletteColorPicker.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000BEA10 File Offset: 0x000BCC10
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			DisposalRouterWindow.Populate:Content.Client.Disposal.UI.DisposalRouterWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x000BEA34 File Offset: 0x000BCC34
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			DisposalTaggerWindow.Populate:Content.Client.Disposal.UI.DisposalTaggerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020F9 RID: 8441 RVA: 0x000BEA58 File Offset: 0x000BCC58
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			DisposalUnitWindow.Populate:Content.Client.Disposal.UI.DisposalUnitWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020FA RID: 8442 RVA: 0x000BEA7C File Offset: 0x000BCC7C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			MailingUnitWindow.Populate:Content.Client.Disposal.UI.MailingUnitWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020FB RID: 8443 RVA: 0x000BEAA0 File Offset: 0x000BCCA0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			FaxWindow.Populate:Content.Client.Fax.UI.FaxWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020FC RID: 8444 RVA: 0x000BEAC4 File Offset: 0x000BCCC4
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			FlavorText.Populate:Content.Client.FlavorText.FlavorText.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020FD RID: 8445 RVA: 0x000BEAE8 File Offset: 0x000BCCE8
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AbsorbentItemStatus.Populate:Content.Client.Fluids.UI.AbsorbentItemStatus.xaml(A_0, control);
			return control;
		}

		// Token: 0x060020FE RID: 8446 RVA: 0x000BEB0C File Offset: 0x000BCD0C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ForensicScannerMenu.Populate:Content.Client.Forensics.ForensicScannerMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x060020FF RID: 8447 RVA: 0x000BEB30 File Offset: 0x000BCD30
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			GravityGeneratorWindow.Populate:Content.Client.Gravity.UI.GravityGeneratorWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002100 RID: 8448 RVA: 0x000BEB54 File Offset: 0x000BCD54
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			GuidebookWindow.Populate:Content.Client.Guidebook.Controls.GuidebookWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002101 RID: 8449 RVA: 0x000BEB78 File Offset: 0x000BCD78
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			GuideEntityEmbed.Populate:Content.Client.Guidebook.Controls.GuideEntityEmbed.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x06002102 RID: 8450 RVA: 0x000BEB9C File Offset: 0x000BCD9C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			HandVirtualItemStatus.Populate:Content.Client.Hands.UI.HandVirtualItemStatus.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x000BEBC0 File Offset: 0x000BCDC0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			HealthAnalyzerWindow.Populate:Content.Client.HealthAnalyzer.UI.HealthAnalyzerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002104 RID: 8452 RVA: 0x000BEBE4 File Offset: 0x000BCDE4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			HumanoidMarkingModifierWindow.Populate:Content.Client.Humanoid.HumanoidMarkingModifierWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002105 RID: 8453 RVA: 0x000BEC08 File Offset: 0x000BCE08
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			MarkingPicker.Populate:Content.Client.Humanoid.MarkingPicker.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002106 RID: 8454 RVA: 0x000BEC2C File Offset: 0x000BCE2C
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			SingleMarkingPicker.Populate:Content.Client.Humanoid.SingleMarkingPicker.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x06002107 RID: 8455 RVA: 0x000BEC50 File Offset: 0x000BCE50
		public static ScrollContainer xaml(IServiceProvider A_0)
		{
			ScrollContainer scrollContainer = new ScrollContainer();
			Info.Populate:Content.Client.Info.Info.xaml(A_0, scrollContainer);
			return scrollContainer;
		}

		// Token: 0x06002108 RID: 8456 RVA: 0x000BEC74 File Offset: 0x000BCE74
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			InfoControlsSection.Populate:Content.Client.Info.InfoControlsSection.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x06002109 RID: 8457 RVA: 0x000BEC98 File Offset: 0x000BCE98
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			InfoSection.Populate:Content.Client.Info.InfoSection.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x0600210A RID: 8458 RVA: 0x000BECBC File Offset: 0x000BCEBC
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			RulesControl.Populate:Content.Client.Info.RulesControl.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x0600210B RID: 8459 RVA: 0x000BECE0 File Offset: 0x000BCEE0
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			RulesPopup.Populate:Content.Client.Info.RulesPopup.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600210C RID: 8460 RVA: 0x000BED04 File Offset: 0x000BCF04
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			InstrumentMenu.Populate:Content.Client.Instruments.UI.InstrumentMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600210D RID: 8461 RVA: 0x000BED28 File Offset: 0x000BCF28
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GrinderMenu.Populate:Content.Client.Kitchen.UI.GrinderMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600210E RID: 8462 RVA: 0x000BED4C File Offset: 0x000BCF4C
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			LabelledContentBox.Populate:Content.Client.Kitchen.UI.LabelledContentBox.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x0600210F RID: 8463 RVA: 0x000BED70 File Offset: 0x000BCF70
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			MicrowaveMenu.Populate:Content.Client.Kitchen.UI.MicrowaveMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002110 RID: 8464 RVA: 0x000BED94 File Offset: 0x000BCF94
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			HandLabelerWindow.Populate:Content.Client.Labels.UI.HandLabelerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002111 RID: 8465 RVA: 0x000BEDB8 File Offset: 0x000BCFB8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			LatheMenu.Populate:Content.Client.Lathe.UI.LatheMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002112 RID: 8466 RVA: 0x000BEDDC File Offset: 0x000BCFDC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			LatheQueueMenu.Populate:Content.Client.Lathe.UI.LatheQueueMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002113 RID: 8467 RVA: 0x000BEE00 File Offset: 0x000BD000
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			RecipeControl.Populate:Content.Client.Lathe.UI.RecipeControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002114 RID: 8468 RVA: 0x000BEE24 File Offset: 0x000BD024
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			LauncherConnectingGui.Populate:Content.Client.Launcher.LauncherConnectingGui.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002115 RID: 8469 RVA: 0x000BEE48 File Offset: 0x000BD048
		public static LobbyGui xaml(IServiceProvider A_0)
		{
			LobbyGui lobbyGui = new LobbyGui();
			LobbyGui.Populate:Content.Client.Lobby.UI.LobbyGui.xaml(A_0, lobbyGui);
			return lobbyGui;
		}

		// Token: 0x06002116 RID: 8470 RVA: 0x000BEE6C File Offset: 0x000BD06C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ObserveWarningWindow.Populate:Content.Client.Lobby.UI.ObserveWarningWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002117 RID: 8471 RVA: 0x000BEE90 File Offset: 0x000BD090
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SignalPortSelectorMenu.Populate:Content.Client.MachineLinking.UI.SignalPortSelectorMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002118 RID: 8472 RVA: 0x000BEEB4 File Offset: 0x000BD0B4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SignalTimerWindow.Populate:Content.Client.MachineLinking.UI.SignalTimerWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002119 RID: 8473 RVA: 0x000BEED8 File Offset: 0x000BD0D8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			MagicMirrorWindow.Populate:Content.Client.MagicMirror.MagicMirrorWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600211A RID: 8474 RVA: 0x000BEEFC File Offset: 0x000BD0FC
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			MainMenuControl.Populate:Content.Client.MainMenu.UI.MainMenuControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x000BEF20 File Offset: 0x000BD120
		public static MechGrabberUiFragment xaml(IServiceProvider A_0)
		{
			MechGrabberUiFragment mechGrabberUiFragment = new MechGrabberUiFragment();
			MechGrabberUiFragment.Populate:Content.Client.Mech.Ui.Equipment.MechGrabberUiFragment.xaml(A_0, mechGrabberUiFragment);
			return mechGrabberUiFragment;
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x000BEF44 File Offset: 0x000BD144
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			MechEquipmentControl.Populate:Content.Client.Mech.Ui.MechEquipmentControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x000BEF68 File Offset: 0x000BD168
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			MechMenu.Populate:Content.Client.Mech.Ui.MechMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x000BEF8C File Offset: 0x000BD18C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			CrewMonitoringWindow.Populate:Content.Client.Medical.CrewMonitoring.CrewMonitoringWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x000BEFB0 File Offset: 0x000BD1B0
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			NetworkConfiguratorConfigurationMenu.Populate:Content.Client.NetworkConfigurator.NetworkConfiguratorConfigurationMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002120 RID: 8480 RVA: 0x000BEFD4 File Offset: 0x000BD1D4
		public static ScrollContainer xaml(IServiceProvider A_0)
		{
			ScrollContainer scrollContainer = new ScrollContainer();
			NetworkConfiguratorDeviceList.Populate:Content.Client.NetworkConfigurator.NetworkConfiguratorDeviceList.xaml(A_0, scrollContainer);
			return scrollContainer;
		}

		// Token: 0x06002121 RID: 8481 RVA: 0x000BEFF8 File Offset: 0x000BD1F8
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			NetworkConfiguratorListMenu.Populate:Content.Client.NetworkConfigurator.NetworkConfiguratorListMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002122 RID: 8482 RVA: 0x000BF01C File Offset: 0x000BD21C
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			NPCWindow.Populate:Content.Client.NPC.NPCWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x000BF040 File Offset: 0x000BD240
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			NukeMenu.Populate:Content.Client.Nuke.NukeMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002124 RID: 8484 RVA: 0x000BF064 File Offset: 0x000BD264
		public static EscapeMenu xaml(IServiceProvider A_0)
		{
			EscapeMenu escapeMenu = new EscapeMenu();
			EscapeMenu.Populate:Content.Client.Options.UI.EscapeMenu.xaml(A_0, escapeMenu);
			return escapeMenu;
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x000BF088 File Offset: 0x000BD288
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			OptionsMenu.Populate:Content.Client.Options.UI.OptionsMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x000BF0AC File Offset: 0x000BD2AC
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AdminSettingsTab.Populate:Content.Client.Options.UI.Tabs.AdminSettingsTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x000BF0D0 File Offset: 0x000BD2D0
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			AudioTab.Populate:Content.Client.Options.UI.Tabs.AudioTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x000BF0F4 File Offset: 0x000BD2F4
		public static GraphicsTab xaml(IServiceProvider A_0)
		{
			GraphicsTab graphicsTab = new GraphicsTab();
			GraphicsTab.Populate:Content.Client.Options.UI.Tabs.GraphicsTab.xaml(A_0, graphicsTab);
			return graphicsTab;
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x000BF118 File Offset: 0x000BD318
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			KeyRebindTab.Populate:Content.Client.Options.UI.Tabs.KeyRebindTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x000BF13C File Offset: 0x000BD33C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			NetworkTab.Populate:Content.Client.Options.UI.Tabs.NetworkTab.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x000BF160 File Offset: 0x000BD360
		public static PaperWindow xaml(IServiceProvider A_0)
		{
			PaperWindow paperWindow = new PaperWindow();
			PaperWindow.Populate:Content.Client.Paper.UI.PaperWindow.xaml(A_0, paperWindow);
			return paperWindow;
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x000BF184 File Offset: 0x000BD384
		public static StampWidget xaml(IServiceProvider A_0)
		{
			StampWidget stampWidget = new StampWidget();
			StampWidget.Populate:Content.Client.Paper.UI.StampWidget.xaml(A_0, stampWidget);
			return stampWidget;
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x000BF1A8 File Offset: 0x000BD3A8
		public static PDAWindow xaml(IServiceProvider A_0)
		{
			PDAWindow pdawindow = new PDAWindow();
			PDAMenu.Populate:Content.Client.PDA.PDAMenu.xaml(A_0, pdawindow);
			return pdawindow;
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x000BF1CC File Offset: 0x000BD3CC
		public static PDANavigationButton xaml(IServiceProvider A_0)
		{
			PDANavigationButton pdanavigationButton = new PDANavigationButton();
			PDANavigationButton.Populate:Content.Client.PDA.PDANavigationButton.xaml(A_0, pdanavigationButton);
			return pdanavigationButton;
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x000BF1F0 File Offset: 0x000BD3F0
		public static PDAProgramItem xaml(IServiceProvider A_0)
		{
			PDAProgramItem pdaprogramItem = new PDAProgramItem();
			PDAProgramItem.Populate:Content.Client.PDA.PDAProgramItem.xaml(A_0, pdaprogramItem);
			return pdaprogramItem;
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x000BF214 File Offset: 0x000BD414
		public static PDASettingsButton xaml(IServiceProvider A_0)
		{
			PDASettingsButton pdasettingsButton = new PDASettingsButton();
			PDASettingsButton.Populate:Content.Client.PDA.PDASettingsButton.xaml(A_0, pdasettingsButton);
			return pdasettingsButton;
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x000BF238 File Offset: 0x000BD438
		public static PDAWindow xaml(IServiceProvider A_0)
		{
			PDAWindow pdawindow = new PDAWindow();
			PDAWindow.Populate:Content.Client.PDA.PDAWindow.xaml(A_0, pdawindow);
			return pdawindow;
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x000BF25C File Offset: 0x000BD45C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			RingtoneMenu.Populate:Content.Client.PDA.Ringer.RingtoneMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x000BF280 File Offset: 0x000BD480
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ApcMenu.Populate:Content.Client.Power.APC.UI.ApcMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000BF2A4 File Offset: 0x000BD4A4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			PowerMonitoringWindow.Populate:Content.Client.Power.PowerMonitoringWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x000BF2C8 File Offset: 0x000BD4C8
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SolarControlWindow.Populate:Content.Client.Power.SolarControlWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x000BF2EC File Offset: 0x000BD4EC
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			CharacterSetupGui.Populate:Content.Client.Preferences.UI.CharacterSetupGui.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x000BF310 File Offset: 0x000BD510
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			HumanoidProfileEditor.Populate:Content.Client.Preferences.UI.HumanoidProfileEditor.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x000BF334 File Offset: 0x000BD534
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			DiskConsoleMenu.Populate:Content.Client.Research.UI.DiskConsoleMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x000BF358 File Offset: 0x000BD558
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ResearchClientServerSelectionMenu.Populate:Content.Client.Research.UI.ResearchClientServerSelectionMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x000BF37C File Offset: 0x000BD57C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			ResearchConsoleMenu.Populate:Content.Client.Research.UI.ResearchConsoleMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x000BF3A0 File Offset: 0x000BD5A0
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			EmergencyConsoleWindow.Populate:Content.Client.Shuttles.UI.EmergencyConsoleWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x000BF3C4 File Offset: 0x000BD5C4
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			IFFConsoleWindow.Populate:Content.Client.Shuttles.UI.IFFConsoleWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000BF3E8 File Offset: 0x000BD5E8
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			RadarConsoleWindow.Populate:Content.Client.Shuttles.UI.RadarConsoleWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000BF40C File Offset: 0x000BD60C
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			ShuttleConsoleWindow.Populate:Content.Client.Shuttles.UI.ShuttleConsoleWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x000BF430 File Offset: 0x000BD630
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GeneralStationRecordConsoleWindow.Populate:Content.Client.StationRecords.GeneralStationRecordConsoleWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x000BF454 File Offset: 0x000BD654
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			StoreListingControl.Populate:Content.Client.Store.Ui.StoreListingControl.xaml(A_0, control);
			return control;
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x000BF478 File Offset: 0x000BD678
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			StoreMenu.Populate:Content.Client.Store.Ui.StoreMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x000BF49C File Offset: 0x000BD69C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			StoreWithdrawWindow.Populate:Content.Client.Store.Ui.StoreWithdrawWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x000BF4C0 File Offset: 0x000BD6C0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SurveillanceCameraMonitorWindow.Populate:Content.Client.SurveillanceCamera.UI.SurveillanceCameraMonitorWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x000BF4E4 File Offset: 0x000BD6E4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			SurveillanceCameraSetupWindow.Populate:Content.Client.SurveillanceCamera.UI.SurveillanceCameraSetupWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x000BF508 File Offset: 0x000BD708
		public static SuspicionGui xaml(IServiceProvider A_0)
		{
			SuspicionGui suspicionGui = new SuspicionGui();
			SuspicionGui.Populate:Content.Client.Suspicion.SuspicionGui.xaml(A_0, suspicionGui);
			return suspicionGui;
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x000BF52C File Offset: 0x000BD72C
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			TabletopWindow.Populate:Content.Client.Tabletop.UI.TabletopWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x000BF550 File Offset: 0x000BD750
		public static TargetingDoll xaml(IServiceProvider A_0)
		{
			TargetingDoll targetingDoll = new TargetingDoll();
			TargetingDoll.Populate:Content.Client.Targeting.UI.TargetingDoll.xaml(A_0, targetingDoll);
			return targetingDoll;
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000BF574 File Offset: 0x000BD774
		public static FancyTree xaml(IServiceProvider A_0)
		{
			FancyTree fancyTree = new FancyTree();
			FancyTree.Populate:Content.Client.UserInterface.Controls.FancyTree.FancyTree.xaml(A_0, fancyTree);
			return fancyTree;
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x000BF598 File Offset: 0x000BD798
		public static TreeItem xaml(IServiceProvider A_0)
		{
			TreeItem treeItem = new TreeItem();
			TreeItem.Populate:Content.Client.UserInterface.Controls.FancyTree.TreeItem.xaml(A_0, treeItem);
			return treeItem;
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x000BF5BC File Offset: 0x000BD7BC
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			FancyWindow.Populate:Content.Client.UserInterface.Controls.FancyWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x000BF5E0 File Offset: 0x000BD7E0
		public static DefaultGameScreen xaml(IServiceProvider A_0)
		{
			DefaultGameScreen defaultGameScreen = new DefaultGameScreen();
			DefaultGameScreen.Populate:Content.Client.UserInterface.Screens.DefaultGameScreen.xaml(A_0, defaultGameScreen);
			return defaultGameScreen;
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x000BF604 File Offset: 0x000BD804
		public static SeparatedChatGameScreen xaml(IServiceProvider A_0)
		{
			SeparatedChatGameScreen separatedChatGameScreen = new SeparatedChatGameScreen();
			SeparatedChatGameScreen.Populate:Content.Client.UserInterface.Screens.SeparatedChatGameScreen.xaml(A_0, separatedChatGameScreen);
			return separatedChatGameScreen;
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x000BF628 File Offset: 0x000BD828
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			StatsWindow.Populate:Content.Client.UserInterface.StatsWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x000BF64C File Offset: 0x000BD84C
		public static ActionPageButtons xaml(IServiceProvider A_0)
		{
			ActionPageButtons actionPageButtons = new ActionPageButtons();
			ActionPageButtons.Populate:Content.Client.UserInterface.Systems.Actions.Controls.ActionPageButtons.xaml(A_0, actionPageButtons);
			return actionPageButtons;
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x000BF670 File Offset: 0x000BD870
		public static ActionTooltip xaml(IServiceProvider A_0)
		{
			ActionTooltip actionTooltip = new ActionTooltip();
			ActionTooltip.Populate:Content.Client.UserInterface.Systems.Actions.Controls.ActionTooltip.xaml(A_0, actionTooltip);
			return actionTooltip;
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x000BF694 File Offset: 0x000BD894
		public static ActionsBar xaml(IServiceProvider A_0)
		{
			ActionsBar actionsBar = new ActionsBar();
			ActionsBar.Populate:Content.Client.UserInterface.Systems.Actions.Widgets.ActionsBar.xaml(A_0, actionsBar);
			return actionsBar;
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000BF6B8 File Offset: 0x000BD8B8
		public static ActionsWindow xaml(IServiceProvider A_0)
		{
			ActionsWindow actionsWindow = new ActionsWindow();
			ActionsWindow.Populate:Content.Client.UserInterface.Systems.Actions.Windows.ActionsWindow.xaml(A_0, actionsWindow);
			return actionsWindow;
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000BF6DC File Offset: 0x000BD8DC
		public static AlertsUI xaml(IServiceProvider A_0)
		{
			AlertsUI alertsUI = new AlertsUI();
			AlertsUI.Populate:Content.Client.UserInterface.Systems.Alerts.Widgets.AlertsUI.xaml(A_0, alertsUI);
			return alertsUI;
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000BF700 File Offset: 0x000BD900
		public static CharacterObjectiveControl xaml(IServiceProvider A_0)
		{
			CharacterObjectiveControl characterObjectiveControl = new CharacterObjectiveControl();
			CharacterObjectiveControl.Populate:Content.Client.UserInterface.Systems.Character.Controls.CharacterObjectiveControl.xaml(A_0, characterObjectiveControl);
			return characterObjectiveControl;
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000BF724 File Offset: 0x000BD924
		public static CharacterWindow xaml(IServiceProvider A_0)
		{
			CharacterWindow characterWindow = new CharacterWindow();
			CharacterWindow.Populate:Content.Client.UserInterface.Systems.Character.Windows.CharacterWindow.xaml(A_0, characterWindow);
			return characterWindow;
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x000BF748 File Offset: 0x000BD948
		public static ChannelFilterPopup xaml(IServiceProvider A_0)
		{
			ChannelFilterPopup channelFilterPopup = new ChannelFilterPopup();
			ChannelFilterPopup.Populate:Content.Client.UserInterface.Systems.Chat.Controls.ChannelFilterPopup.xaml(A_0, channelFilterPopup);
			return channelFilterPopup;
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x000BF76C File Offset: 0x000BD96C
		public static ChatBox xaml(IServiceProvider A_0)
		{
			ChatBox chatBox = new ChatBox();
			ChatBox.Populate:Content.Client.UserInterface.Systems.Chat.Widgets.ChatBox.xaml(A_0, chatBox);
			return chatBox;
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x000BF790 File Offset: 0x000BD990
		public static EmotionsWindow xaml(IServiceProvider A_0)
		{
			EmotionsWindow emotionsWindow = new EmotionsWindow();
			EmotionsWindow.Populate:Content.Client.UserInterface.Systems.Emotions.Windows.EmotionsWindow.xaml(A_0, emotionsWindow);
			return emotionsWindow;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x000BF7B4 File Offset: 0x000BD9B4
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GhostTargetWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.GhostTargetWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x000BF7D8 File Offset: 0x000BD9D8
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			GhostRoleEntryButtons.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRoleEntryButtons.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x000BF7FC File Offset: 0x000BD9FC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GhostRoleRulesWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRoleRulesWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x000BF820 File Offset: 0x000BDA20
		public static BoxContainer xaml(IServiceProvider A_0)
		{
			BoxContainer boxContainer = new BoxContainer();
			GhostRolesEntry.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRolesEntry.xaml(A_0, boxContainer);
			return boxContainer;
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x000BF844 File Offset: 0x000BDA44
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			GhostRolesWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRolesWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x000BF868 File Offset: 0x000BDA68
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			MakeGhostRoleWindow.Populate:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.MakeGhostRoleWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x000BF88C File Offset: 0x000BDA8C
		public static GhostGui xaml(IServiceProvider A_0)
		{
			GhostGui ghostGui = new GhostGui();
			GhostGui.Populate:Content.Client.UserInterface.Systems.Ghost.Widgets.GhostGui.xaml(A_0, ghostGui);
			return ghostGui;
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x000BF8B0 File Offset: 0x000BDAB0
		public static HotbarGui xaml(IServiceProvider A_0)
		{
			HotbarGui hotbarGui = new HotbarGui();
			HotbarGui.Populate:Content.Client.UserInterface.Systems.Hotbar.Widgets.HotbarGui.xaml(A_0, hotbarGui);
			return hotbarGui;
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000BF8D4 File Offset: 0x000BDAD4
		public static ItemStatusPanel xaml(IServiceProvider A_0)
		{
			ItemStatusPanel itemStatusPanel = new ItemStatusPanel();
			ItemStatusPanel.Populate:Content.Client.UserInterface.Systems.Inventory.Controls.ItemStatusPanel.xaml(A_0, itemStatusPanel);
			return itemStatusPanel;
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000BF8F8 File Offset: 0x000BDAF8
		public static StrippingWindow xaml(IServiceProvider A_0)
		{
			StrippingWindow strippingWindow = new StrippingWindow();
			StrippingWindow.Populate:Content.Client.UserInterface.Systems.Inventory.Windows.StrippingWindow.xaml(A_0, strippingWindow);
			return strippingWindow;
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x000BF91C File Offset: 0x000BDB1C
		public static GameTopMenuBar xaml(IServiceProvider A_0)
		{
			GameTopMenuBar gameTopMenuBar = new GameTopMenuBar();
			GameTopMenuBar.Populate:Content.Client.UserInterface.Systems.MenuBar.Widgets.GameTopMenuBar.xaml(A_0, gameTopMenuBar);
			return gameTopMenuBar;
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x000BF940 File Offset: 0x000BDB40
		public static ObjectiveBriefingControl xaml(IServiceProvider A_0)
		{
			ObjectiveBriefingControl objectiveBriefingControl = new ObjectiveBriefingControl();
			ObjectiveBriefingControl.Populate:Content.Client.UserInterface.Systems.Objectives.Controls.ObjectiveBriefingControl.xaml(A_0, objectiveBriefingControl);
			return objectiveBriefingControl;
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x000BF964 File Offset: 0x000BDB64
		public static ObjectiveConditionsControl xaml(IServiceProvider A_0)
		{
			ObjectiveConditionsControl objectiveConditionsControl = new ObjectiveConditionsControl();
			ObjectiveConditionsControl.Populate:Content.Client.UserInterface.Systems.Objectives.Controls.ObjectiveConditionsControl.xaml(A_0, objectiveConditionsControl);
			return objectiveConditionsControl;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x000BF988 File Offset: 0x000BDB88
		public static SandboxWindow xaml(IServiceProvider A_0)
		{
			SandboxWindow sandboxWindow = new SandboxWindow();
			SandboxWindow.Populate:Content.Client.UserInterface.Systems.Sandbox.Windows.SandboxWindow.xaml(A_0, sandboxWindow);
			return sandboxWindow;
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x000BF9AC File Offset: 0x000BDBAC
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			VendingMachineMenu.Populate:Content.Client.VendingMachines.UI.VendingMachineMenu.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x000BF9D0 File Offset: 0x000BDBD0
		public static DefaultWindow xaml(IServiceProvider A_0)
		{
			DefaultWindow defaultWindow = new DefaultWindow();
			VoiceMaskNameChangeWindow.Populate:Content.Client.VoiceMask.VoiceMaskNameChangeWindow.xaml(A_0, defaultWindow);
			return defaultWindow;
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x000BF9F4 File Offset: 0x000BDBF4
		public static VoteCallMenu xaml(IServiceProvider A_0)
		{
			VoteCallMenu voteCallMenu = new VoteCallMenu();
			VoteCallMenu.Populate:Content.Client.Voting.UI.VoteCallMenu.xaml(A_0, voteCallMenu);
			return voteCallMenu;
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x000BFA18 File Offset: 0x000BDC18
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			VotePopup.Populate:Content.Client.Voting.UI.VotePopup.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x000BFA3C File Offset: 0x000BDC3C
		public static Control xaml(IServiceProvider A_0)
		{
			Control control = new Control();
			QueueGui.Populate:Content.Client.White.JoinQueue.QueueGui.xaml(A_0, control);
			return control;
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x000BFA60 File Offset: 0x000BDC60
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			AnalysisConsoleMenu.Populate:Content.Client.Xenoarchaeology.Ui.AnalysisConsoleMenu.xaml(A_0, fancyWindow);
			return fancyWindow;
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x000BFA84 File Offset: 0x000BDC84
		public static FancyWindow xaml(IServiceProvider A_0)
		{
			FancyWindow fancyWindow = new FancyWindow();
			AnalysisDestroyWindow.Populate:Content.Client.Xenoarchaeology.Ui.AnalysisDestroyWindow.xaml(A_0, fancyWindow);
			return fancyWindow;
		}
	}
}
