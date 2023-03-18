using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Cloning.Components;
using Content.Server.MachineLinking.Components;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Medical.Components;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.UserInterface;
using Content.Shared.Administration.Logs;
using Content.Shared.Cloning;
using Content.Shared.Cloning.CloningConsole;
using Content.Shared.Database;
using Content.Shared.IdentityManagement;
using Content.Shared.MachineLinking.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Cloning
{
	// Token: 0x0200063F RID: 1599
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CloningConsoleSystem : EntitySystem
	{
		// Token: 0x060021F2 RID: 8690 RVA: 0x000B0EA0 File Offset: 0x000AF0A0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CloningConsoleComponent, ComponentInit>(new ComponentEventHandler<CloningConsoleComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, UiButtonPressedMessage>(new ComponentEventHandler<CloningConsoleComponent, UiButtonPressedMessage>(this.OnButtonPressed), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<CloningConsoleComponent, AfterActivatableUIOpenEvent>(this.OnUIOpen), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, PowerChangedEvent>(new ComponentEventRefHandler<CloningConsoleComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, MapInitEvent>(new ComponentEventHandler<CloningConsoleComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, NewLinkEvent>(new ComponentEventHandler<CloningConsoleComponent, NewLinkEvent>(this.OnNewLink), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, PortDisconnectedEvent>(new ComponentEventHandler<CloningConsoleComponent, PortDisconnectedEvent>(this.OnPortDisconnected), null, null);
			base.SubscribeLocalEvent<CloningConsoleComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<CloningConsoleComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x000B0F53 File Offset: 0x000AF153
		private void OnInit(EntityUid uid, CloningConsoleComponent component, ComponentInit args)
		{
			this._signalSystem.EnsureTransmitterPorts(uid, new string[]
			{
				"MedicalScannerSender",
				"CloningPodSender"
			});
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x000B0F78 File Offset: 0x000AF178
		private void OnButtonPressed(EntityUid uid, CloningConsoleComponent consoleComponent, UiButtonPressedMessage args)
		{
			if (!this._powerReceiverSystem.IsPowered(uid, null))
			{
				return;
			}
			if (args.Button == UiButton.Clone && consoleComponent.GeneticScanner != null && consoleComponent.CloningPod != null)
			{
				this.TryClone(uid, consoleComponent.CloningPod.Value, consoleComponent.GeneticScanner.Value, null, null, consoleComponent);
			}
			this.UpdateUserInterface(consoleComponent);
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x000B0FDF File Offset: 0x000AF1DF
		private void OnPowerChanged(EntityUid uid, CloningConsoleComponent component, ref PowerChangedEvent args)
		{
			this.UpdateUserInterface(component);
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x000B0FE8 File Offset: 0x000AF1E8
		private void OnMapInit(EntityUid uid, CloningConsoleComponent component, MapInitEvent args)
		{
			SignalTransmitterComponent receiver;
			if (!base.TryComp<SignalTransmitterComponent>(uid, ref receiver))
			{
				return;
			}
			foreach (PortIdentifier port in receiver.Outputs.Values.SelectMany((List<PortIdentifier> ports) => ports))
			{
				MedicalScannerComponent scanner;
				if (base.TryComp<MedicalScannerComponent>(port.Uid, ref scanner))
				{
					component.GeneticScanner = new EntityUid?(port.Uid);
					scanner.ConnectedConsole = new EntityUid?(uid);
				}
				CloningPodComponent pod;
				if (base.TryComp<CloningPodComponent>(port.Uid, ref pod))
				{
					component.CloningPod = new EntityUid?(port.Uid);
					pod.ConnectedConsole = new EntityUid?(uid);
				}
			}
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x000B10C0 File Offset: 0x000AF2C0
		private void OnNewLink(EntityUid uid, CloningConsoleComponent component, NewLinkEvent args)
		{
			MedicalScannerComponent scanner;
			if (base.TryComp<MedicalScannerComponent>(args.Receiver, ref scanner) && args.TransmitterPort == "MedicalScannerSender")
			{
				component.GeneticScanner = new EntityUid?(args.Receiver);
				scanner.ConnectedConsole = new EntityUid?(uid);
			}
			CloningPodComponent pod;
			if (base.TryComp<CloningPodComponent>(args.Receiver, ref pod) && args.TransmitterPort == "CloningPodSender")
			{
				component.CloningPod = new EntityUid?(args.Receiver);
				pod.ConnectedConsole = new EntityUid?(uid);
			}
			this.RecheckConnections(uid, component.CloningPod, component.GeneticScanner, component);
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x000B1160 File Offset: 0x000AF360
		private void OnPortDisconnected(EntityUid uid, CloningConsoleComponent component, PortDisconnectedEvent args)
		{
			if (args.Port == "MedicalScannerSender")
			{
				component.GeneticScanner = null;
			}
			if (args.Port == "CloningPodSender")
			{
				component.CloningPod = null;
			}
			this.UpdateUserInterface(component);
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x000B11B0 File Offset: 0x000AF3B0
		private void OnUIOpen(EntityUid uid, CloningConsoleComponent component, AfterActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(component);
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x000B11B9 File Offset: 0x000AF3B9
		private void OnAnchorChanged(EntityUid uid, CloningConsoleComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				this.RecheckConnections(uid, component.CloningPod, component.GeneticScanner, component);
				return;
			}
			this.UpdateUserInterface(component);
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x000B11E0 File Offset: 0x000AF3E0
		public void UpdateUserInterface(CloningConsoleComponent consoleComponent)
		{
			BoundUserInterface ui = this._uiSystem.GetUiOrNull(consoleComponent.Owner, CloningConsoleUiKey.Key, null);
			if (ui == null)
			{
				return;
			}
			if (!this._powerReceiverSystem.IsPowered(consoleComponent.Owner, null))
			{
				this._uiSystem.CloseAll(ui);
				return;
			}
			CloningConsoleBoundUserInterfaceState newState = this.GetUserInterfaceState(consoleComponent);
			this._uiSystem.SetUiState(ui, newState, null, true);
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x000B1244 File Offset: 0x000AF444
		[NullableContext(2)]
		public void TryClone(EntityUid uid, EntityUid cloningPodUid, EntityUid scannerUid, CloningPodComponent cloningPod = null, MedicalScannerComponent scannerComp = null, CloningConsoleComponent consoleComponent = null)
		{
			if (!base.Resolve<CloningConsoleComponent>(uid, ref consoleComponent, true) || !base.Resolve<CloningPodComponent>(cloningPodUid, ref cloningPod, true) || !base.Resolve<MedicalScannerComponent>(scannerUid, ref scannerComp, true))
			{
				return;
			}
			if (!base.Transform(cloningPodUid).Anchored || !base.Transform(scannerUid).Anchored)
			{
				return;
			}
			if (!consoleComponent.CloningPodInRange || !consoleComponent.GeneticScannerInRange)
			{
				return;
			}
			EntityUid? body = scannerComp.BodyContainer.ContainedEntity;
			if (body == null)
			{
				return;
			}
			MindComponent mindComp;
			if (!base.TryComp<MindComponent>(body, ref mindComp))
			{
				return;
			}
			Mind mind = mindComp.Mind;
			if (mind == null || mind.UserId == null || mind.Session == null)
			{
				return;
			}
			if (this._cloningSystem.TryCloning(cloningPodUid, body.Value, mind, cloningPod, scannerComp.CloningFailChanceMultiplier))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Action;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(22, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" successfully cloned ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(body.Value), "ToPrettyString(body.Value)");
				logStringHandler.AppendLiteral(".");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x000B136C File Offset: 0x000AF56C
		[NullableContext(2)]
		public void RecheckConnections(EntityUid console, EntityUid? cloningPod, EntityUid? scanner, CloningConsoleComponent consoleComp = null)
		{
			if (!base.Resolve<CloningConsoleComponent>(console, ref consoleComp, true))
			{
				return;
			}
			if (scanner != null)
			{
				float scannerDistance;
				base.Transform(scanner.Value).Coordinates.TryDistance(this.EntityManager, base.Transform(console).Coordinates, ref scannerDistance);
				consoleComp.GeneticScannerInRange = (scannerDistance <= consoleComp.MaxDistance);
			}
			if (cloningPod != null)
			{
				float podDistance;
				base.Transform(cloningPod.Value).Coordinates.TryDistance(this.EntityManager, base.Transform(console).Coordinates, ref podDistance);
				consoleComp.CloningPodInRange = (podDistance <= consoleComp.MaxDistance);
			}
			this.UpdateUserInterface(consoleComp);
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x000B1428 File Offset: 0x000AF628
		private CloningConsoleBoundUserInterfaceState GetUserInterfaceState(CloningConsoleComponent consoleComponent)
		{
			ClonerStatus clonerStatus = ClonerStatus.Ready;
			string scanBodyInfo = Loc.GetString("generic-unknown");
			bool scannerConnected = false;
			bool scannerInRange = consoleComponent.GeneticScannerInRange;
			MedicalScannerComponent scanner;
			if (consoleComponent.GeneticScanner != null && base.TryComp<MedicalScannerComponent>(consoleComponent.GeneticScanner, ref scanner))
			{
				scannerConnected = true;
				EntityUid? scanBody = scanner.BodyContainer.ContainedEntity;
				if (scanBody == null || !base.HasComp<MobStateComponent>(scanBody))
				{
					clonerStatus = ClonerStatus.ScannerEmpty;
				}
				else
				{
					scanBodyInfo = base.MetaData(scanBody.Value).EntityName;
					MindComponent mindComp;
					base.TryComp<MindComponent>(scanBody, ref mindComp);
					IPlayerSession playerSession;
					if (!this._mobStateSystem.IsDead(scanBody.Value, null))
					{
						clonerStatus = ClonerStatus.ScannerOccupantAlive;
					}
					else if (mindComp == null || mindComp.Mind == null || mindComp.Mind.UserId == null || !this._playerManager.TryGetSessionById(mindComp.Mind.UserId.Value, ref playerSession))
					{
						clonerStatus = ClonerStatus.NoMindDetected;
					}
				}
			}
			string cloneBodyInfo = Loc.GetString("generic-unknown");
			bool clonerConnected = false;
			bool clonerMindPresent = false;
			bool clonerInRange = consoleComponent.CloningPodInRange;
			CloningPodComponent clonePod;
			if (consoleComponent.CloningPod != null && base.TryComp<CloningPodComponent>(consoleComponent.CloningPod, ref clonePod) && base.Transform(consoleComponent.CloningPod.Value).Anchored)
			{
				clonerConnected = true;
				EntityUid? cloneBody = clonePod.BodyContainer.ContainedEntity;
				clonerMindPresent = (clonePod.Status == CloningPodStatus.Cloning);
				if (base.HasComp<ActiveCloningPodComponent>(consoleComponent.CloningPod))
				{
					if (cloneBody != null)
					{
						cloneBodyInfo = Identity.Name(cloneBody.Value, this.EntityManager, null);
					}
					clonerStatus = ClonerStatus.ClonerOccupied;
				}
			}
			else
			{
				clonerStatus = ClonerStatus.NoClonerDetected;
			}
			return new CloningConsoleBoundUserInterfaceState(scanBodyInfo, cloneBodyInfo, clonerMindPresent, clonerStatus, scannerConnected, scannerInRange, clonerConnected, clonerInRange);
		}

		// Token: 0x040014D0 RID: 5328
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x040014D1 RID: 5329
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040014D2 RID: 5330
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040014D3 RID: 5331
		[Dependency]
		private readonly CloningSystem _cloningSystem;

		// Token: 0x040014D4 RID: 5332
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x040014D5 RID: 5333
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040014D6 RID: 5334
		[Dependency]
		private readonly PowerReceiverSystem _powerReceiverSystem;
	}
}
