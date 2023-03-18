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
using Content.Client.Administration.UI.Tabs;
using Content.Client.Administration.UI.Tabs.AdminbusTab;
using Content.Client.Administration.UI.Tabs.AdminTab;
using Content.Client.Administration.UI.Tabs.AtmosTab;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.AirlockPainter.UI;
using Content.Client.Anomaly.Ui;
using Content.Client.Atmos.UI;
using Content.Client.Borgs;
using Content.Client.Cargo.UI;
using Content.Client.CartridgeLoader.Cartridges;
using Content.Client.Changelog;
using Content.Client.Chemistry.UI;
using Content.Client.CloningConsole.UI;
using Content.Client.Clothing.UI;
using Content.Client.Construction.UI;
using Content.Client.Credits;
using Content.Client.CrewManifest;
using Content.Client.Decals.UI;
using Content.Client.Disposal.UI;
using Content.Client.Fax.UI;
using Content.Client.FlavorText;
using Content.Client.Forensics;
using Content.Client.Guidebook.Controls;
using Content.Client.Hands.UI;
using Content.Client.HealthAnalyzer.UI;
using Content.Client.Humanoid;
using Content.Client.Info;
using Content.Client.Kitchen.UI;
using Content.Client.Labels.UI;
using Content.Client.Lathe.UI;
using Content.Client.Lobby.UI;
using Content.Client.MagicMirror;
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
using Content.Client.Research.UI;
using Content.Client.Shuttles.UI;
using Content.Client.StationRecords;
using Content.Client.Store.Ui;
using Content.Client.SurveillanceCamera.UI;
using Content.Client.Suspicion;
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

namespace CompiledRobustXaml
{
	// Token: 0x02000506 RID: 1286
	internal class !XamlLoader
	{
		// Token: 0x060020A2 RID: 8354 RVA: 0x000BD0B8 File Offset: 0x000BB2B8
		public static object TryLoad(string A_0)
		{
			if (string.Equals(A_0, "resm:Content.Client.Access.UI.AgentIDCardWindow.xaml?assembly=Content.Client"))
			{
				return new AgentIDCardWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.AdminAnnounceWindow.xaml?assembly=Content.Client"))
			{
				return new AdminAnnounceWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.AdminMenuWindow.xaml?assembly=Content.Client"))
			{
				return new AdminMenuWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.BanList.BanListControl.xaml?assembly=Content.Client"))
			{
				return new BanListControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.BanList.BanListHeader.xaml?assembly=Content.Client"))
			{
				return new BanListHeader();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.BanList.BanListWindow.xaml?assembly=Content.Client"))
			{
				return new BanListWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Bwoink.BwoinkControl.xaml?assembly=Content.Client"))
			{
				return new BwoinkControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Bwoink.BwoinkWindow.xaml?assembly=Content.Client"))
			{
				return new BwoinkWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.CustomControls.PlayerListControl.xaml?assembly=Content.Client"))
			{
				return new PlayerListControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Logs.AdminLogsControl.xaml?assembly=Content.Client"))
			{
				return new AdminLogsControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Logs.AdminLogsWindow.xaml?assembly=Content.Client"))
			{
				return new AdminLogsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.ManageSolutions.EditSolutionsWindow.xaml?assembly=Content.Client"))
			{
				return new EditSolutionsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Notes.AdminNotesControl.xaml?assembly=Content.Client"))
			{
				return new AdminNotesControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Notes.AdminNotesWindow.xaml?assembly=Content.Client"))
			{
				return new AdminNotesWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.SetOutfit.SetOutfitMenu.xaml?assembly=Content.Client"))
			{
				return new SetOutfitMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminbusTab.AdminbusTab.xaml?assembly=Content.Client"))
			{
				return new AdminbusTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminbusTab.LoadBlueprintsWindow.xaml?assembly=Content.Client"))
			{
				return new LoadBlueprintsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleCallEnableWindow.xaml?assembly=Content.Client"))
			{
				return new AdminShuttleCallEnableWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.AdminShuttleWindow.xaml?assembly=Content.Client"))
			{
				return new AdminShuttleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.AdminTab.xaml?assembly=Content.Client"))
			{
				return new AdminTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.BanWindow.xaml?assembly=Content.Client"))
			{
				return new BanWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.PlayerActionsWindow.xaml?assembly=Content.Client"))
			{
				return new PlayerActionsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.RoleBanWindow.xaml?assembly=Content.Client"))
			{
				return new RoleBanWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AdminTab.TeleportWindow.xaml?assembly=Content.Client"))
			{
				return new TeleportWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.AddAtmosWindow.xaml?assembly=Content.Client"))
			{
				return new AddAtmosWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.AddGasWindow.xaml?assembly=Content.Client"))
			{
				return new AddGasWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.AtmosTab.xaml?assembly=Content.Client"))
			{
				return new AtmosTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.FillGasWindow.xaml?assembly=Content.Client"))
			{
				return new FillGasWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.SetTemperatureWindow.xaml?assembly=Content.Client"))
			{
				return new SetTemperatureWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.ObjectsTab.ObjectsTab.xaml?assembly=Content.Client"))
			{
				return new ObjectsTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTab.xaml?assembly=Content.Client"))
			{
				return new PlayerTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.PlayerTab.PlayerTabHeader.xaml?assembly=Content.Client"))
			{
				return new PlayerTabHeader();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.RoundTab.xaml?assembly=Content.Client"))
			{
				return new RoundTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Administration.UI.Tabs.ServerTab.xaml?assembly=Content.Client"))
			{
				return new ServerTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.AirlockPainter.UI.AirlockPainterWindow.xaml?assembly=Content.Client"))
			{
				return new AirlockPainterWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Anomaly.Ui.AnomalyScannerMenu.xaml?assembly=Content.Client"))
			{
				return new AnomalyScannerMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Atmos.UI.GasCanisterWindow.xaml?assembly=Content.Client"))
			{
				return new GasCanisterWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Atmos.UI.GasMixerWindow.xaml?assembly=Content.Client"))
			{
				return new GasMixerWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Atmos.UI.GasPressurePumpWindow.xaml?assembly=Content.Client"))
			{
				return new GasPressurePumpWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Atmos.UI.GasThermomachineWindow.xaml?assembly=Content.Client"))
			{
				return new GasThermomachineWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Atmos.UI.GasVolumePumpWindow.xaml?assembly=Content.Client"))
			{
				return new GasVolumePumpWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Borgs.LawUIContainer.xaml?assembly=Content.Client"))
			{
				return new LawUIContainer();
			}
			if (string.Equals(A_0, "resm:Content.Client.Cargo.UI.CargoConsoleOrderMenu.xaml?assembly=Content.Client"))
			{
				return new CargoConsoleOrderMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Cargo.UI.CargoOrderRow.xaml?assembly=Content.Client"))
			{
				return new CargoOrderRow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Cargo.UI.CargoProductRow.xaml?assembly=Content.Client"))
			{
				return new CargoProductRow();
			}
			if (string.Equals(A_0, "resm:Content.Client.CartridgeLoader.Cartridges.NetProbeUiFragment.xaml?assembly=Content.Client"))
			{
				return new NetProbeUiFragment();
			}
			if (string.Equals(A_0, "resm:Content.Client.CartridgeLoader.Cartridges.NotekeeperUiFragment.xaml?assembly=Content.Client"))
			{
				return new NotekeeperUiFragment();
			}
			if (string.Equals(A_0, "resm:Content.Client.Changelog.ChangelogWindow.xaml?assembly=Content.Client"))
			{
				return new ChangelogWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Chemistry.UI.ChemMasterWindow.xaml?assembly=Content.Client"))
			{
				return new ChemMasterWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Chemistry.UI.ReagentDispenserWindow.xaml?assembly=Content.Client"))
			{
				return new ReagentDispenserWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Chemistry.UI.TransferAmountWindow.xaml?assembly=Content.Client"))
			{
				return new TransferAmountWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.CloningConsole.UI.CloningConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new CloningConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Clothing.UI.ChameleonMenu.xaml?assembly=Content.Client"))
			{
				return new ChameleonMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Construction.UI.ConstructionMenu.xaml?assembly=Content.Client"))
			{
				return new ConstructionMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Credits.CreditsWindow.xaml?assembly=Content.Client"))
			{
				return new CreditsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.CrewManifest.CrewManifestUi.xaml?assembly=Content.Client"))
			{
				return new CrewManifestUi();
			}
			if (string.Equals(A_0, "resm:Content.Client.Decals.UI.DecalPlacerWindow.xaml?assembly=Content.Client"))
			{
				return new DecalPlacerWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Decals.UI.PaletteColorPicker.xaml?assembly=Content.Client"))
			{
				return new PaletteColorPicker();
			}
			if (string.Equals(A_0, "resm:Content.Client.Disposal.UI.DisposalRouterWindow.xaml?assembly=Content.Client"))
			{
				return new DisposalRouterWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Disposal.UI.DisposalTaggerWindow.xaml?assembly=Content.Client"))
			{
				return new DisposalTaggerWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Disposal.UI.DisposalUnitWindow.xaml?assembly=Content.Client"))
			{
				return new DisposalUnitWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Disposal.UI.MailingUnitWindow.xaml?assembly=Content.Client"))
			{
				return new MailingUnitWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Fax.UI.FaxWindow.xaml?assembly=Content.Client"))
			{
				return new FaxWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.FlavorText.FlavorText.xaml?assembly=Content.Client"))
			{
				return new FlavorText();
			}
			if (string.Equals(A_0, "resm:Content.Client.Forensics.ForensicScannerMenu.xaml?assembly=Content.Client"))
			{
				return new ForensicScannerMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Guidebook.Controls.GuidebookWindow.xaml?assembly=Content.Client"))
			{
				return new GuidebookWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Guidebook.Controls.GuideEntityEmbed.xaml?assembly=Content.Client"))
			{
				return new GuideEntityEmbed();
			}
			if (string.Equals(A_0, "resm:Content.Client.Hands.UI.HandVirtualItemStatus.xaml?assembly=Content.Client"))
			{
				return new HandVirtualItemStatus();
			}
			if (string.Equals(A_0, "resm:Content.Client.HealthAnalyzer.UI.HealthAnalyzerWindow.xaml?assembly=Content.Client"))
			{
				return new HealthAnalyzerWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Humanoid.HumanoidMarkingModifierWindow.xaml?assembly=Content.Client"))
			{
				return new HumanoidMarkingModifierWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Humanoid.MarkingPicker.xaml?assembly=Content.Client"))
			{
				return new MarkingPicker();
			}
			if (string.Equals(A_0, "resm:Content.Client.Humanoid.SingleMarkingPicker.xaml?assembly=Content.Client"))
			{
				return new SingleMarkingPicker();
			}
			if (string.Equals(A_0, "resm:Content.Client.Info.Info.xaml?assembly=Content.Client"))
			{
				return new Info();
			}
			if (string.Equals(A_0, "resm:Content.Client.Info.InfoControlsSection.xaml?assembly=Content.Client"))
			{
				return new InfoControlsSection();
			}
			if (string.Equals(A_0, "resm:Content.Client.Info.RulesControl.xaml?assembly=Content.Client"))
			{
				return new RulesControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.Info.RulesPopup.xaml?assembly=Content.Client"))
			{
				return new RulesPopup();
			}
			if (string.Equals(A_0, "resm:Content.Client.Kitchen.UI.LabelledContentBox.xaml?assembly=Content.Client"))
			{
				return new LabelledContentBox();
			}
			if (string.Equals(A_0, "resm:Content.Client.Labels.UI.HandLabelerWindow.xaml?assembly=Content.Client"))
			{
				return new HandLabelerWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Lathe.UI.LatheQueueMenu.xaml?assembly=Content.Client"))
			{
				return new LatheQueueMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Lobby.UI.LobbyGui.xaml?assembly=Content.Client"))
			{
				return new LobbyGui();
			}
			if (string.Equals(A_0, "resm:Content.Client.Lobby.UI.ObserveWarningWindow.xaml?assembly=Content.Client"))
			{
				return new ObserveWarningWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.MagicMirror.MagicMirrorWindow.xaml?assembly=Content.Client"))
			{
				return new MagicMirrorWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Mech.Ui.Equipment.MechGrabberUiFragment.xaml?assembly=Content.Client"))
			{
				return new MechGrabberUiFragment();
			}
			if (string.Equals(A_0, "resm:Content.Client.Medical.CrewMonitoring.CrewMonitoringWindow.xaml?assembly=Content.Client"))
			{
				return new CrewMonitoringWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.NetworkConfigurator.NetworkConfiguratorConfigurationMenu.xaml?assembly=Content.Client"))
			{
				return new NetworkConfiguratorConfigurationMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.NetworkConfigurator.NetworkConfiguratorDeviceList.xaml?assembly=Content.Client"))
			{
				return new NetworkConfiguratorDeviceList();
			}
			if (string.Equals(A_0, "resm:Content.Client.NPC.NPCWindow.xaml?assembly=Content.Client"))
			{
				return new NPCWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Nuke.NukeMenu.xaml?assembly=Content.Client"))
			{
				return new NukeMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.EscapeMenu.xaml?assembly=Content.Client"))
			{
				return new EscapeMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.OptionsMenu.xaml?assembly=Content.Client"))
			{
				return new OptionsMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.Tabs.AdminSettingsTab.xaml?assembly=Content.Client"))
			{
				return new AdminSettingsTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.Tabs.AudioTab.xaml?assembly=Content.Client"))
			{
				return new AudioTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.Tabs.GraphicsTab.xaml?assembly=Content.Client"))
			{
				return new GraphicsTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.Tabs.KeyRebindTab.xaml?assembly=Content.Client"))
			{
				return new KeyRebindTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Options.UI.Tabs.NetworkTab.xaml?assembly=Content.Client"))
			{
				return new NetworkTab();
			}
			if (string.Equals(A_0, "resm:Content.Client.Paper.UI.PaperWindow.xaml?assembly=Content.Client"))
			{
				return new PaperWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Paper.UI.StampWidget.xaml?assembly=Content.Client"))
			{
				return new StampWidget();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.PDAMenu.xaml?assembly=Content.Client"))
			{
				return new PDAMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.PDANavigationButton.xaml?assembly=Content.Client"))
			{
				return new PDANavigationButton();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.PDAProgramItem.xaml?assembly=Content.Client"))
			{
				return new PDAProgramItem();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.PDASettingsButton.xaml?assembly=Content.Client"))
			{
				return new PDASettingsButton();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.PDAWindow.xaml?assembly=Content.Client"))
			{
				return new PDAWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.PDA.Ringer.RingtoneMenu.xaml?assembly=Content.Client"))
			{
				return new RingtoneMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Power.PowerMonitoringWindow.xaml?assembly=Content.Client"))
			{
				return new PowerMonitoringWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Power.SolarControlWindow.xaml?assembly=Content.Client"))
			{
				return new SolarControlWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Research.UI.DiskConsoleMenu.xaml?assembly=Content.Client"))
			{
				return new DiskConsoleMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Shuttles.UI.EmergencyConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new EmergencyConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Shuttles.UI.IFFConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new IFFConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Shuttles.UI.RadarConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new RadarConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Shuttles.UI.ShuttleConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new ShuttleConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.StationRecords.GeneralStationRecordConsoleWindow.xaml?assembly=Content.Client"))
			{
				return new GeneralStationRecordConsoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Store.Ui.StoreWithdrawWindow.xaml?assembly=Content.Client"))
			{
				return new StoreWithdrawWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.SurveillanceCamera.UI.SurveillanceCameraMonitorWindow.xaml?assembly=Content.Client"))
			{
				return new SurveillanceCameraMonitorWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.SurveillanceCamera.UI.SurveillanceCameraSetupWindow.xaml?assembly=Content.Client"))
			{
				return new SurveillanceCameraSetupWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Suspicion.SuspicionGui.xaml?assembly=Content.Client"))
			{
				return new SuspicionGui();
			}
			if (string.Equals(A_0, "resm:Content.Client.Targeting.UI.TargetingDoll.xaml?assembly=Content.Client"))
			{
				return new TargetingDoll();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Controls.FancyTree.FancyTree.xaml?assembly=Content.Client"))
			{
				return new FancyTree();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Controls.FancyTree.TreeItem.xaml?assembly=Content.Client"))
			{
				return new TreeItem();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Controls.FancyWindow.xaml?assembly=Content.Client"))
			{
				return new FancyWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Screens.DefaultGameScreen.xaml?assembly=Content.Client"))
			{
				return new DefaultGameScreen();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Screens.SeparatedChatGameScreen.xaml?assembly=Content.Client"))
			{
				return new SeparatedChatGameScreen();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.StatsWindow.xaml?assembly=Content.Client"))
			{
				return new StatsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Actions.Controls.ActionPageButtons.xaml?assembly=Content.Client"))
			{
				return new ActionPageButtons();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Actions.Controls.ActionTooltip.xaml?assembly=Content.Client"))
			{
				return new ActionTooltip();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Actions.Widgets.ActionsBar.xaml?assembly=Content.Client"))
			{
				return new ActionsBar();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Actions.Windows.ActionsWindow.xaml?assembly=Content.Client"))
			{
				return new ActionsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Alerts.Widgets.AlertsUI.xaml?assembly=Content.Client"))
			{
				return new AlertsUI();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Character.Controls.CharacterObjectiveControl.xaml?assembly=Content.Client"))
			{
				return new CharacterObjectiveControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Character.Windows.CharacterWindow.xaml?assembly=Content.Client"))
			{
				return new CharacterWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Chat.Controls.ChannelFilterPopup.xaml?assembly=Content.Client"))
			{
				return new ChannelFilterPopup();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Chat.Widgets.ChatBox.xaml?assembly=Content.Client"))
			{
				return new ChatBox();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Emotions.Windows.EmotionsWindow.xaml?assembly=Content.Client"))
			{
				return new EmotionsWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.GhostTargetWindow.xaml?assembly=Content.Client"))
			{
				return new GhostTargetWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRoleEntryButtons.xaml?assembly=Content.Client"))
			{
				return new GhostRoleEntryButtons();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRolesWindow.xaml?assembly=Content.Client"))
			{
				return new GhostRolesWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Ghost.Controls.Roles.MakeGhostRoleWindow.xaml?assembly=Content.Client"))
			{
				return new MakeGhostRoleWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Ghost.Widgets.GhostGui.xaml?assembly=Content.Client"))
			{
				return new GhostGui();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Hotbar.Widgets.HotbarGui.xaml?assembly=Content.Client"))
			{
				return new HotbarGui();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Inventory.Controls.ItemStatusPanel.xaml?assembly=Content.Client"))
			{
				return new ItemStatusPanel();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Inventory.Windows.StrippingWindow.xaml?assembly=Content.Client"))
			{
				return new StrippingWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.MenuBar.Widgets.GameTopMenuBar.xaml?assembly=Content.Client"))
			{
				return new GameTopMenuBar();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Objectives.Controls.ObjectiveBriefingControl.xaml?assembly=Content.Client"))
			{
				return new ObjectiveBriefingControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Objectives.Controls.ObjectiveConditionsControl.xaml?assembly=Content.Client"))
			{
				return new ObjectiveConditionsControl();
			}
			if (string.Equals(A_0, "resm:Content.Client.UserInterface.Systems.Sandbox.Windows.SandboxWindow.xaml?assembly=Content.Client"))
			{
				return new SandboxWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.VendingMachines.UI.VendingMachineMenu.xaml?assembly=Content.Client"))
			{
				return new VendingMachineMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.VoiceMask.VoiceMaskNameChangeWindow.xaml?assembly=Content.Client"))
			{
				return new VoiceMaskNameChangeWindow();
			}
			if (string.Equals(A_0, "resm:Content.Client.Voting.UI.VoteCallMenu.xaml?assembly=Content.Client"))
			{
				return new VoteCallMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.White.JoinQueue.QueueGui.xaml?assembly=Content.Client"))
			{
				return new QueueGui();
			}
			if (string.Equals(A_0, "resm:Content.Client.Xenoarchaeology.Ui.AnalysisConsoleMenu.xaml?assembly=Content.Client"))
			{
				return new AnalysisConsoleMenu();
			}
			if (string.Equals(A_0, "resm:Content.Client.Xenoarchaeology.Ui.AnalysisDestroyWindow.xaml?assembly=Content.Client"))
			{
				return new AnalysisDestroyWindow();
			}
			return null;
		}
	}
}
