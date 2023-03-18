using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Construction;
using Content.Server.MachineLinking.Components;
using Content.Server.MachineLinking.Events;
using Content.Server.Paper;
using Content.Server.Power.Components;
using Content.Server.Research.Systems;
using Content.Server.UserInterface;
using Content.Server.Xenoarchaeology.Equipment.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Audio;
using Content.Shared.MachineLinking.Events;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Content.Shared.Xenoarchaeology.Equipment;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Xenoarchaeology.Equipment.Systems
{
	// Token: 0x02000061 RID: 97
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ArtifactAnalyzerSystem : EntitySystem
	{
		// Token: 0x0600010A RID: 266 RVA: 0x00006E00 File Offset: 0x00005000
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ActiveScannedArtifactComponent, MoveEvent>(new ComponentEventRefHandler<ActiveScannedArtifactComponent, MoveEvent>(this.OnScannedMoved), null, null);
			base.SubscribeLocalEvent<ActiveScannedArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<ActiveScannedArtifactComponent, ArtifactActivatedEvent>(this.OnArtifactActivated), null, null);
			base.SubscribeLocalEvent<ActiveArtifactAnalyzerComponent, ComponentStartup>(new ComponentEventHandler<ActiveArtifactAnalyzerComponent, ComponentStartup>(this.OnAnalyzeStart), null, null);
			base.SubscribeLocalEvent<ActiveArtifactAnalyzerComponent, ComponentShutdown>(new ComponentEventHandler<ActiveArtifactAnalyzerComponent, ComponentShutdown>(this.OnAnalyzeEnd), null, null);
			base.SubscribeLocalEvent<ActiveArtifactAnalyzerComponent, PowerChangedEvent>(new ComponentEventRefHandler<ActiveArtifactAnalyzerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<ArtifactAnalyzerComponent, UpgradeExamineEvent>(new ComponentEventHandler<ArtifactAnalyzerComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<ArtifactAnalyzerComponent, RefreshPartsEvent>(new ComponentEventHandler<ArtifactAnalyzerComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<ArtifactAnalyzerComponent, StartCollideEvent>(new ComponentEventRefHandler<ArtifactAnalyzerComponent, StartCollideEvent>(this.OnCollide), null, null);
			base.SubscribeLocalEvent<ArtifactAnalyzerComponent, EndCollideEvent>(new ComponentEventRefHandler<ArtifactAnalyzerComponent, EndCollideEvent>(this.OnEndCollide), null, null);
			base.SubscribeLocalEvent<ArtifactAnalyzerComponent, MapInitEvent>(new ComponentEventHandler<ArtifactAnalyzerComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, NewLinkEvent>(new ComponentEventHandler<AnalysisConsoleComponent, NewLinkEvent>(this.OnNewLink), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, PortDisconnectedEvent>(new ComponentEventHandler<AnalysisConsoleComponent, PortDisconnectedEvent>(this.OnPortDisconnected), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, AnalysisConsoleServerSelectionMessage>(new ComponentEventHandler<AnalysisConsoleComponent, AnalysisConsoleServerSelectionMessage>(this.OnServerSelectionMessage), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, AnalysisConsoleScanButtonPressedMessage>(new ComponentEventHandler<AnalysisConsoleComponent, AnalysisConsoleScanButtonPressedMessage>(this.OnScanButton), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, AnalysisConsolePrintButtonPressedMessage>(new ComponentEventHandler<AnalysisConsoleComponent, AnalysisConsolePrintButtonPressedMessage>(this.OnPrintButton), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, AnalysisConsoleDestroyButtonPressedMessage>(new ComponentEventHandler<AnalysisConsoleComponent, AnalysisConsoleDestroyButtonPressedMessage>(this.OnDestroyButton), null, null);
			base.SubscribeLocalEvent<AnalysisConsoleComponent, ResearchClientServerSelectedMessage>(delegate(EntityUid e, AnalysisConsoleComponent c, ResearchClientServerSelectedMessage _)
			{
				this.UpdateUserInterface(e, c);
			}, null, new Type[]
			{
				typeof(ResearchSystem)
			});
			base.SubscribeLocalEvent<AnalysisConsoleComponent, ResearchClientServerDeselectedMessage>(delegate(EntityUid e, AnalysisConsoleComponent c, ResearchClientServerDeselectedMessage _)
			{
				this.UpdateUserInterface(e, c);
			}, null, new Type[]
			{
				typeof(ResearchSystem)
			});
			base.SubscribeLocalEvent<AnalysisConsoleComponent, BeforeActivatableUIOpenEvent>(delegate(EntityUid e, AnalysisConsoleComponent c, BeforeActivatableUIOpenEvent _)
			{
				this.UpdateUserInterface(e, c);
			}, null, null);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00006FB0 File Offset: 0x000051B0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveArtifactAnalyzerComponent, ArtifactAnalyzerComponent> valueTuple in base.EntityQuery<ActiveArtifactAnalyzerComponent, ArtifactAnalyzerComponent>(false))
			{
				ActiveArtifactAnalyzerComponent active = valueTuple.Item1;
				ArtifactAnalyzerComponent scan = valueTuple.Item2;
				if (scan.Console != null)
				{
					this.UpdateUserInterface(scan.Console.Value, null);
				}
				if (!(this._timing.CurTime - active.StartTime < scan.AnalysisDuration * (double)scan.AnalysisDurationMulitplier))
				{
					this.FinishScan(scan.Owner, scan, active);
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007068 File Offset: 0x00005268
		[NullableContext(2)]
		public void ResetAnalyzer(EntityUid uid, ArtifactAnalyzerComponent component = null)
		{
			if (!base.Resolve<ArtifactAnalyzerComponent>(uid, ref component, true))
			{
				return;
			}
			component.LastAnalyzedArtifact = null;
			component.ReadyToPrint = false;
			this.UpdateAnalyzerInformation(uid, component);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00007094 File Offset: 0x00005294
		[NullableContext(2)]
		private EntityUid? GetArtifactForAnalysis(EntityUid? uid, ArtifactAnalyzerComponent component = null)
		{
			if (uid == null)
			{
				return null;
			}
			if (!base.Resolve<ArtifactAnalyzerComponent>(uid.Value, ref component, true))
			{
				return null;
			}
			return Extensions.FirstOrNull<EntityUid>(component.Contacts.Where(new Func<EntityUid, bool>(base.HasComp<ArtifactComponent>)).ToHashSet<EntityUid>());
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000070F4 File Offset: 0x000052F4
		[NullableContext(2)]
		private void UpdateAnalyzerInformation(EntityUid uid, ArtifactAnalyzerComponent component = null)
		{
			if (!base.Resolve<ArtifactAnalyzerComponent>(uid, ref component, true))
			{
				return;
			}
			if (component.LastAnalyzedArtifact == null)
			{
				component.LastAnalyzerPointValue = null;
				component.LastAnalyzedNode = null;
				return;
			}
			ArtifactComponent artifact;
			if (base.TryComp<ArtifactComponent>(component.LastAnalyzedArtifact, ref artifact))
			{
				ArtifactNode currentNode = artifact.CurrentNode;
				ArtifactNode lastNode = (ArtifactNode)((currentNode != null) ? currentNode.Clone() : null);
				component.LastAnalyzedNode = lastNode;
				component.LastAnalyzerPointValue = new int?(this._artifact.GetResearchPointValue(component.LastAnalyzedArtifact.Value, artifact, false));
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00007184 File Offset: 0x00005384
		private void OnMapInit(EntityUid uid, ArtifactAnalyzerComponent component, MapInitEvent args)
		{
			SignalReceiverComponent receiver;
			if (!base.TryComp<SignalReceiverComponent>(uid, ref receiver))
			{
				return;
			}
			foreach (PortIdentifier port in receiver.Inputs.Values.SelectMany((List<PortIdentifier> ports) => ports))
			{
				AnalysisConsoleComponent analysis;
				if (base.TryComp<AnalysisConsoleComponent>(port.Uid, ref analysis))
				{
					component.Console = new EntityUid?(port.Uid);
					analysis.AnalyzerEntity = new EntityUid?(uid);
					break;
				}
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00007230 File Offset: 0x00005430
		private void OnNewLink(EntityUid uid, AnalysisConsoleComponent component, NewLinkEvent args)
		{
			ArtifactAnalyzerComponent analyzer;
			if (!base.TryComp<ArtifactAnalyzerComponent>(args.Receiver, ref analyzer))
			{
				return;
			}
			component.AnalyzerEntity = new EntityUid?(args.Receiver);
			analyzer.Console = new EntityUid?(uid);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00007274 File Offset: 0x00005474
		private void OnPortDisconnected(EntityUid uid, AnalysisConsoleComponent component, PortDisconnectedEvent args)
		{
			if (args.Port == component.LinkingPort && component.AnalyzerEntity != null)
			{
				ArtifactAnalyzerComponent analyzezr;
				if (base.TryComp<ArtifactAnalyzerComponent>(component.AnalyzerEntity, ref analyzezr))
				{
					analyzezr.Console = null;
				}
				component.AnalyzerEntity = null;
			}
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000072D4 File Offset: 0x000054D4
		[NullableContext(2)]
		private void UpdateUserInterface(EntityUid uid, AnalysisConsoleComponent component = null)
		{
			if (!base.Resolve<AnalysisConsoleComponent>(uid, ref component, false))
			{
				return;
			}
			EntityUid? artifact = null;
			FormattedMessage msg = null;
			TimeSpan totalTime = TimeSpan.Zero;
			bool canScan = false;
			bool canPrint = false;
			ArtifactAnalyzerComponent analyzer;
			if (component.AnalyzerEntity != null && base.TryComp<ArtifactAnalyzerComponent>(component.AnalyzerEntity, ref analyzer))
			{
				artifact = analyzer.LastAnalyzedArtifact;
				msg = this.GetArtifactScanMessage(analyzer);
				totalTime = analyzer.AnalysisDuration * (double)analyzer.AnalysisDurationMulitplier;
				canScan = analyzer.Contacts.Any<EntityUid>();
				canPrint = analyzer.ReadyToPrint;
			}
			bool analyzerConnected = component.AnalyzerEntity != null;
			ResearchClientComponent client;
			bool serverConnected = base.TryComp<ResearchClientComponent>(uid, ref client) && client.ConnectedToServer;
			ActiveArtifactAnalyzerComponent active;
			bool scanning = base.TryComp<ActiveArtifactAnalyzerComponent>(component.AnalyzerEntity, ref active);
			TimeSpan remaining = (active != null) ? (this._timing.CurTime - active.StartTime) : TimeSpan.Zero;
			AnalysisConsoleScanUpdateState state = new AnalysisConsoleScanUpdateState(artifact, analyzerConnected, serverConnected, canScan, canPrint, msg, scanning, remaining, totalTime);
			BoundUserInterface bui = this._ui.GetUi(uid, ArtifactAnalzyerUiKey.Key, null);
			this._ui.SetUiState(bui, state, null, true);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000073EF File Offset: 0x000055EF
		private void OnServerSelectionMessage(EntityUid uid, AnalysisConsoleComponent component, AnalysisConsoleServerSelectionMessage args)
		{
			this._ui.TryOpen(uid, ResearchClientUiKey.Key, (IPlayerSession)args.Session, null);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00007410 File Offset: 0x00005610
		private void OnScanButton(EntityUid uid, AnalysisConsoleComponent component, AnalysisConsoleScanButtonPressedMessage args)
		{
			if (component.AnalyzerEntity == null)
			{
				return;
			}
			if (base.HasComp<ActiveArtifactAnalyzerComponent>(component.AnalyzerEntity))
			{
				return;
			}
			EntityUid? ent = this.GetArtifactForAnalysis(component.AnalyzerEntity, null);
			if (ent == null)
			{
				return;
			}
			ActiveArtifactAnalyzerComponent activeArtifactAnalyzerComponent = base.EnsureComp<ActiveArtifactAnalyzerComponent>(component.AnalyzerEntity.Value);
			activeArtifactAnalyzerComponent.StartTime = this._timing.CurTime;
			activeArtifactAnalyzerComponent.Artifact = ent.Value;
			base.EnsureComp<ActiveScannedArtifactComponent>(ent.Value).Scanner = component.AnalyzerEntity.Value;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000074A0 File Offset: 0x000056A0
		private void OnPrintButton(EntityUid uid, AnalysisConsoleComponent component, AnalysisConsolePrintButtonPressedMessage args)
		{
			if (component.AnalyzerEntity == null)
			{
				return;
			}
			ArtifactAnalyzerComponent analyzer;
			if (!base.TryComp<ArtifactAnalyzerComponent>(component.AnalyzerEntity, ref analyzer) || analyzer.LastAnalyzedNode == null || analyzer.LastAnalyzerPointValue == null || !analyzer.ReadyToPrint)
			{
				return;
			}
			analyzer.ReadyToPrint = false;
			EntityUid report = base.Spawn(component.ReportEntityId, base.Transform(uid).Coordinates);
			base.MetaData(report).EntityName = Loc.GetString("analysis-report-title", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("id", analyzer.LastAnalyzedNode.Id)
			});
			FormattedMessage msg = this.GetArtifactScanMessage(analyzer);
			if (msg == null)
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("analysis-console-print-popup"), uid, PopupType.Small);
			this._paper.SetContent(report, msg.ToMarkup(), null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00007584 File Offset: 0x00005784
		[return: Nullable(2)]
		private FormattedMessage GetArtifactScanMessage(ArtifactAnalyzerComponent component)
		{
			FormattedMessage msg = new FormattedMessage();
			if (component.LastAnalyzedNode == null)
			{
				return null;
			}
			ArtifactNode i = component.LastAnalyzedNode;
			msg.AddMarkup(Loc.GetString("analysis-console-info-id", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("id", i.Id)
			}));
			msg.PushNewline();
			msg.AddMarkup(Loc.GetString("analysis-console-info-depth", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("depth", i.Depth)
			}));
			msg.PushNewline();
			string activated = i.Triggered ? "analysis-console-info-triggered-true" : "analysis-console-info-triggered-false";
			msg.AddMarkup(Loc.GetString(activated));
			msg.PushNewline();
			msg.PushNewline();
			bool needSecondNewline = false;
			if (i.Trigger.TriggerHint != null)
			{
				msg.AddMarkup(Loc.GetString("analysis-console-info-trigger", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("trigger", Loc.GetString(i.Trigger.TriggerHint))
				}) + "\n");
				needSecondNewline = true;
			}
			if (i.Effect.EffectHint != null)
			{
				msg.AddMarkup(Loc.GetString("analysis-console-info-effect", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("effect", Loc.GetString(i.Effect.EffectHint))
				}) + "\n");
				needSecondNewline = true;
			}
			if (needSecondNewline)
			{
				msg.PushNewline();
			}
			msg.AddMarkup(Loc.GetString("analysis-console-info-edges", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("edges", i.Edges.Count)
			}));
			msg.PushNewline();
			if (component.LastAnalyzerPointValue != null)
			{
				msg.AddMarkup(Loc.GetString("analysis-console-info-value", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("value", component.LastAnalyzerPointValue)
				}));
			}
			return msg;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007770 File Offset: 0x00005970
		private void OnDestroyButton(EntityUid uid, AnalysisConsoleComponent component, AnalysisConsoleDestroyButtonPressedMessage args)
		{
			if (component.AnalyzerEntity == null)
			{
				return;
			}
			EntityUid? server;
			ResearchServerComponent serverComponent;
			if (!this._research.TryGetClientServer(uid, out server, out serverComponent, null))
			{
				return;
			}
			EntityUid? entToDestroy = this.GetArtifactForAnalysis(component.AnalyzerEntity, null);
			if (entToDestroy == null)
			{
				return;
			}
			ArtifactAnalyzerComponent analyzer;
			if (base.TryComp<ArtifactAnalyzerComponent>(component.AnalyzerEntity.Value, ref analyzer) && analyzer.LastAnalyzedArtifact == entToDestroy)
			{
				this.ResetAnalyzer(component.AnalyzerEntity.Value, null);
			}
			this._research.AddPointsToServer(server.Value, this._artifact.GetResearchPointValue(entToDestroy.Value, null, false), serverComponent);
			this.EntityManager.DeleteEntity(entToDestroy.Value);
			this._audio.PlayPvs(component.DestroySound, component.AnalyzerEntity.Value, new AudioParams?(AudioParams.Default.WithVolume(2f)));
			this._popup.PopupEntity(Loc.GetString("analyzer-artifact-destroy-popup"), component.AnalyzerEntity.Value, PopupType.Large);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000078B0 File Offset: 0x00005AB0
		private void OnArtifactActivated(EntityUid uid, ActiveScannedArtifactComponent component, ArtifactActivatedEvent args)
		{
			this.CancelScan(uid, null, null);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000078BC File Offset: 0x00005ABC
		private void OnScannedMoved(EntityUid uid, ActiveScannedArtifactComponent component, ref MoveEvent args)
		{
			ArtifactAnalyzerComponent analyzer;
			if (!base.TryComp<ArtifactAnalyzerComponent>(component.Scanner, ref analyzer))
			{
				return;
			}
			if (analyzer.Contacts.Contains(uid))
			{
				return;
			}
			this.CancelScan(uid, component, analyzer);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000078F4 File Offset: 0x00005AF4
		[NullableContext(2)]
		public void CancelScan(EntityUid artifact, ActiveScannedArtifactComponent component = null, ArtifactAnalyzerComponent analyzer = null)
		{
			if (!base.Resolve<ActiveScannedArtifactComponent>(artifact, ref component, false))
			{
				return;
			}
			if (!base.Resolve<ArtifactAnalyzerComponent>(component.Scanner, ref analyzer, true))
			{
				return;
			}
			this._audio.PlayPvs(component.ScanFailureSound, component.Scanner, new AudioParams?(AudioParams.Default.WithVolume(3f)));
			base.RemComp<ActiveArtifactAnalyzerComponent>(component.Scanner);
			if (analyzer.Console != null)
			{
				this.UpdateUserInterface(analyzer.Console.Value, null);
			}
			base.RemCompDeferred(artifact, component);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00007980 File Offset: 0x00005B80
		[NullableContext(2)]
		public void FinishScan(EntityUid uid, ArtifactAnalyzerComponent component = null, ActiveArtifactAnalyzerComponent active = null)
		{
			if (!base.Resolve<ArtifactAnalyzerComponent, ActiveArtifactAnalyzerComponent>(uid, ref component, ref active, true))
			{
				return;
			}
			component.ReadyToPrint = true;
			this._audio.PlayPvs(component.ScanFinishedSound, uid, null);
			component.LastAnalyzedArtifact = new EntityUid?(active.Artifact);
			this.UpdateAnalyzerInformation(uid, component);
			base.RemComp<ActiveScannedArtifactComponent>(active.Artifact);
			base.RemComp(uid, active);
			if (component.Console != null)
			{
				this.UpdateUserInterface(component.Console.Value, null);
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00007A0C File Offset: 0x00005C0C
		private void OnRefreshParts(EntityUid uid, ArtifactAnalyzerComponent component, RefreshPartsEvent args)
		{
			float analysisRating = args.PartRatings[component.MachinePartAnalysisDuration];
			component.AnalysisDurationMulitplier = MathF.Pow(component.PartRatingAnalysisDurationMultiplier, analysisRating - 1f);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00007A43 File Offset: 0x00005C43
		private void OnUpgradeExamine(EntityUid uid, ArtifactAnalyzerComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("analyzer-artifact-component-upgrade-analysis", component.AnalysisDurationMulitplier);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00007A58 File Offset: 0x00005C58
		private void OnCollide(EntityUid uid, ArtifactAnalyzerComponent component, ref StartCollideEvent args)
		{
			EntityUid otherEnt = args.OtherFixture.Body.Owner;
			if (!base.HasComp<ArtifactComponent>(otherEnt))
			{
				return;
			}
			component.Contacts.Add(otherEnt);
			if (component.Console != null)
			{
				this.UpdateUserInterface(component.Console.Value, null);
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00007AAC File Offset: 0x00005CAC
		private void OnEndCollide(EntityUid uid, ArtifactAnalyzerComponent component, ref EndCollideEvent args)
		{
			EntityUid otherEnt = args.OtherFixture.Body.Owner;
			if (!base.HasComp<ArtifactComponent>(otherEnt))
			{
				return;
			}
			component.Contacts.Remove(otherEnt);
			if (component.Console != null && base.Exists(component.Console))
			{
				this.UpdateUserInterface(component.Console.Value, null);
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007B10 File Offset: 0x00005D10
		private void OnAnalyzeStart(EntityUid uid, ActiveArtifactAnalyzerComponent component, ComponentStartup args)
		{
			ApcPowerReceiverComponent powa;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref powa))
			{
				powa.NeedsPower = true;
			}
			this._ambienntSound.SetAmbience(uid, true, null);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007B40 File Offset: 0x00005D40
		private void OnAnalyzeEnd(EntityUid uid, ActiveArtifactAnalyzerComponent component, ComponentShutdown args)
		{
			ApcPowerReceiverComponent powa;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref powa))
			{
				powa.NeedsPower = false;
			}
			this._ambienntSound.SetAmbience(uid, false, null);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007B6D File Offset: 0x00005D6D
		private void OnPowerChanged(EntityUid uid, ActiveArtifactAnalyzerComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				this.CancelScan(component.Artifact, null, null);
			}
		}

		// Token: 0x040000E8 RID: 232
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040000E9 RID: 233
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040000EA RID: 234
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambienntSound;

		// Token: 0x040000EB RID: 235
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x040000EC RID: 236
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x040000ED RID: 237
		[Dependency]
		private readonly ArtifactSystem _artifact;

		// Token: 0x040000EE RID: 238
		[Dependency]
		private readonly PaperSystem _paper;

		// Token: 0x040000EF RID: 239
		[Dependency]
		private readonly ResearchSystem _research;
	}
}
