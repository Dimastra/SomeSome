using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Anomaly.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Audio;
using Content.Server.Construction;
using Content.Server.DoAfter;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Materials;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Anomaly;
using Content.Shared.Anomaly.Components;
using Content.Shared.Atmos;
using Content.Shared.CCVar;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Materials;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Research.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Anomaly
{
	// Token: 0x020007C3 RID: 1987
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnomalySystem : SharedAnomalySystem
	{
		// Token: 0x06002B26 RID: 11046 RVA: 0x000E1D58 File Offset: 0x000DFF58
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AnomalyComponent, MapInitEvent>(new ComponentEventHandler<AnomalyComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, ComponentShutdown>(new ComponentEventHandler<AnomalyComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<AnomalyComponent, StartCollideEvent>(new ComponentEventRefHandler<AnomalyComponent, StartCollideEvent>(this.OnStartCollide), null, null);
			this.InitializeGenerator();
			this.InitializeScanner();
			this.InitializeVessel();
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x000E1DBC File Offset: 0x000DFFBC
		private void OnMapInit(EntityUid uid, AnomalyComponent component, MapInitEvent args)
		{
			component.NextPulseTime = this.Timing.CurTime + base.GetPulseLength(component) * 3.0;
			base.ChangeAnomalyStability(uid, this.Random.NextFloat(component.InitialStabilityRange.Item1, component.InitialStabilityRange.Item2), component);
			base.ChangeAnomalySeverity(uid, this.Random.NextFloat(component.InitialSeverityRange.Item1, component.InitialSeverityRange.Item2), component);
			List<AnomalousParticleType> particles = new List<AnomalousParticleType>
			{
				AnomalousParticleType.Delta,
				AnomalousParticleType.Epsilon,
				AnomalousParticleType.Zeta
			};
			component.SeverityParticleType = RandomExtensions.PickAndTake<AnomalousParticleType>(this.Random, particles);
			component.DestabilizingParticleType = RandomExtensions.PickAndTake<AnomalousParticleType>(this.Random, particles);
			component.WeakeningParticleType = RandomExtensions.PickAndTake<AnomalousParticleType>(this.Random, particles);
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x000E1E97 File Offset: 0x000E0097
		private void OnShutdown(EntityUid uid, AnomalyComponent component, ComponentShutdown args)
		{
			base.EndAnomaly(uid, component, false);
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x000E1EA4 File Offset: 0x000E00A4
		private void OnStartCollide(EntityUid uid, AnomalyComponent component, ref StartCollideEvent args)
		{
			AnomalousParticleComponent particleComponent;
			if (!base.TryComp<AnomalousParticleComponent>(args.OtherFixture.Body.Owner, ref particleComponent))
			{
				return;
			}
			if (args.OtherFixture.ID != particleComponent.FixtureId)
			{
				return;
			}
			if (particleComponent.ParticleType == component.DestabilizingParticleType)
			{
				base.ChangeAnomalyStability(uid, this.<OnStartCollide>g__VaryValue|14_0(component.StabilityPerDestabilizingHit), component);
				return;
			}
			if (particleComponent.ParticleType == component.SeverityParticleType)
			{
				base.ChangeAnomalySeverity(uid, this.<OnStartCollide>g__VaryValue|14_0(component.SeverityPerSeverityHit), component);
				return;
			}
			if (particleComponent.ParticleType == component.WeakeningParticleType)
			{
				base.ChangeAnomalyHealth(uid, this.<OnStartCollide>g__VaryValue|14_0(component.HealthPerWeakeningeHit), component);
				base.ChangeAnomalyStability(uid, this.<OnStartCollide>g__VaryValue|14_0(component.StabilityPerWeakeningeHit), component);
			}
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x000E1F64 File Offset: 0x000E0164
		[NullableContext(2)]
		public int GetAnomalyPointValue(EntityUid anomaly, AnomalyComponent component = null)
		{
			if (!base.Resolve<AnomalyComponent>(anomaly, ref component, false))
			{
				return 0;
			}
			float multiplier = 1f;
			if (component.Stability > component.GrowthThreshold)
			{
				multiplier = component.GrowingPointMultiplier;
			}
			multiplier *= MathF.Pow(1.5f, component.Health) - 0.5f;
			float severityValue = 1f / (1f + MathF.Pow(2.7182817f, -7f * (component.Severity - 0.5f)));
			return (int)((float)(component.MaxPointsPerSecond - component.MinPointsPerSecond) * severityValue * multiplier) + component.MinPointsPerSecond;
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x000E1FF8 File Offset: 0x000E01F8
		public string GetParticleLocale(AnomalousParticleType type)
		{
			string @string;
			switch (type)
			{
			case AnomalousParticleType.Delta:
				@string = Loc.GetString("anomaly-particles-delta");
				break;
			case AnomalousParticleType.Epsilon:
				@string = Loc.GetString("anomaly-particles-epsilon");
				break;
			case AnomalousParticleType.Zeta:
				@string = Loc.GetString("anomaly-particles-zeta");
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return @string;
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000E2047 File Offset: 0x000E0247
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateGenerator();
			this.UpdateVessels();
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000E205C File Offset: 0x000E025C
		private void InitializeGenerator()
		{
			base.SubscribeLocalEvent<AnomalyGeneratorComponent, BoundUIOpenedEvent>(new ComponentEventHandler<AnomalyGeneratorComponent, BoundUIOpenedEvent>(this.OnGeneratorBUIOpened), null, null);
			base.SubscribeLocalEvent<AnomalyGeneratorComponent, MaterialAmountChangedEvent>(new ComponentEventRefHandler<AnomalyGeneratorComponent, MaterialAmountChangedEvent>(this.OnGeneratorMaterialAmountChanged), null, null);
			base.SubscribeLocalEvent<AnomalyGeneratorComponent, AnomalyGeneratorGenerateButtonPressedEvent>(new ComponentEventHandler<AnomalyGeneratorComponent, AnomalyGeneratorGenerateButtonPressedEvent>(this.OnGenerateButtonPressed), null, null);
			base.SubscribeLocalEvent<AnomalyGeneratorComponent, PowerChangedEvent>(new ComponentEventRefHandler<AnomalyGeneratorComponent, PowerChangedEvent>(this.OnGeneratorPowerChanged), null, null);
			base.SubscribeLocalEvent<AnomalyGeneratorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalyGeneratorComponent, EntityUnpausedEvent>(this.OnGeneratorUnpaused), null, null);
			base.SubscribeLocalEvent<GeneratingAnomalyGeneratorComponent, ComponentStartup>(new ComponentEventHandler<GeneratingAnomalyGeneratorComponent, ComponentStartup>(this.OnGeneratingStartup), null, null);
			base.SubscribeLocalEvent<GeneratingAnomalyGeneratorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GeneratingAnomalyGeneratorComponent, EntityUnpausedEvent>(this.OnGeneratingUnpaused), null, null);
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000E20F5 File Offset: 0x000E02F5
		private void OnGeneratorPowerChanged(EntityUid uid, AnomalyGeneratorComponent component, ref PowerChangedEvent args)
		{
			this._ambient.SetAmbience(uid, args.Powered, null);
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x000E210A File Offset: 0x000E030A
		private void OnGeneratorBUIOpened(EntityUid uid, AnomalyGeneratorComponent component, BoundUIOpenedEvent args)
		{
			this.UpdateGeneratorUi(uid, component);
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000E2114 File Offset: 0x000E0314
		private void OnGeneratorMaterialAmountChanged(EntityUid uid, AnomalyGeneratorComponent component, ref MaterialAmountChangedEvent args)
		{
			this.UpdateGeneratorUi(uid, component);
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000E211E File Offset: 0x000E031E
		private void OnGenerateButtonPressed(EntityUid uid, AnomalyGeneratorComponent component, AnomalyGeneratorGenerateButtonPressedEvent args)
		{
			this.TryGeneratorCreateAnomaly(uid, component);
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x000E2128 File Offset: 0x000E0328
		private void OnGeneratorUnpaused(EntityUid uid, AnomalyGeneratorComponent component, ref EntityUnpausedEvent args)
		{
			component.CooldownEndTime += args.PausedTime;
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x000E2144 File Offset: 0x000E0344
		public void UpdateGeneratorUi(EntityUid uid, AnomalyGeneratorComponent component)
		{
			int materialAmount = this._material.GetMaterialAmount(uid, component.RequiredMaterial, null);
			AnomalyGeneratorUserInterfaceState state = new AnomalyGeneratorUserInterfaceState(component.CooldownEndTime, materialAmount, component.MaterialPerAnomaly);
			this._ui.TrySetUiState(uid, AnomalyGeneratorUiKey.Key, state, null, null, true);
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000E2190 File Offset: 0x000E0390
		[NullableContext(2)]
		public void TryGeneratorCreateAnomaly(EntityUid uid, AnomalyGeneratorComponent component = null)
		{
			if (!base.Resolve<AnomalyGeneratorComponent>(uid, ref component, true))
			{
				return;
			}
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (this.Timing.CurTime < component.CooldownEndTime)
			{
				return;
			}
			if (!this._material.TryChangeMaterialAmount(uid, component.RequiredMaterial, -component.MaterialPerAnomaly, null))
			{
				return;
			}
			GeneratingAnomalyGeneratorComponent generatingAnomalyGeneratorComponent = base.EnsureComp<GeneratingAnomalyGeneratorComponent>(uid);
			generatingAnomalyGeneratorComponent.EndTime = this.Timing.CurTime + component.GenerationLength;
			generatingAnomalyGeneratorComponent.AudioStream = this.Audio.PlayPvs(component.GeneratingSound, uid, new AudioParams?(AudioParams.Default.WithLoop(true)));
			component.CooldownEndTime = this.Timing.CurTime + component.CooldownLength;
			this.UpdateGeneratorUi(uid, component);
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x000E2260 File Offset: 0x000E0460
		public void SpawnOnRandomGridLocation(EntityUid grid, string toSpawn)
		{
			MapGridComponent gridComp;
			if (!base.TryComp<MapGridComponent>(grid, ref gridComp))
			{
				return;
			}
			TransformComponent xform = base.Transform(grid);
			EntityCoordinates targetCoords = xform.Coordinates;
			Box2 gridBounds = gridComp.LocalAABB;
			gridBounds.Scale(this._configuration.GetCVar<float>(CCVars.AnomalyGenerationGridBoundsScale));
			for (int i = 0; i < 25; i++)
			{
				int randomX = this.Random.Next((int)gridBounds.Left, (int)gridBounds.Right);
				int randomY = this.Random.Next((int)gridBounds.Bottom, (int)gridBounds.Top);
				Vector2i tile;
				tile..ctor(randomX, randomY);
				if (!this._atmosphere.IsTileSpace(new EntityUid?(grid), xform.MapUid, tile, gridComp) && !this._atmosphere.IsTileAirBlocked(grid, tile, AtmosDirection.All, gridComp))
				{
					EntityQuery<PhysicsComponent> physQuery = base.GetEntityQuery<PhysicsComponent>();
					bool valid = true;
					foreach (EntityUid ent in gridComp.GetAnchoredEntities(tile))
					{
						PhysicsComponent body;
						if (physQuery.TryGetComponent(ent, ref body) && body.BodyType == 4 && body.Hard && (body.CollisionLayer & 2) != 0)
						{
							valid = false;
							break;
						}
					}
					if (valid)
					{
						targetCoords = gridComp.GridTileToLocal(tile);
						break;
					}
				}
			}
			base.Spawn(toSpawn, targetCoords);
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x000E23C8 File Offset: 0x000E05C8
		private void OnGeneratingStartup(EntityUid uid, GeneratingAnomalyGeneratorComponent component, ComponentStartup args)
		{
			this.Appearance.SetData(uid, AnomalyGeneratorVisuals.Generating, true, null);
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x000E23E3 File Offset: 0x000E05E3
		private void OnGeneratingUnpaused(EntityUid uid, GeneratingAnomalyGeneratorComponent component, ref EntityUnpausedEvent args)
		{
			component.EndTime += args.PausedTime;
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x000E23FC File Offset: 0x000E05FC
		private void OnGeneratingFinished(EntityUid uid, AnomalyGeneratorComponent component)
		{
			EntityUid? grid = base.Transform(uid).GridUid;
			if (grid == null)
			{
				return;
			}
			this.SpawnOnRandomGridLocation(grid.Value, component.SpawnerPrototype);
			base.RemComp<GeneratingAnomalyGeneratorComponent>(uid);
			this.Appearance.SetData(uid, AnomalyGeneratorVisuals.Generating, false, null);
			this.Audio.PlayPvs(component.GeneratingFinishedSound, uid, null);
			string message = Loc.GetString("anomaly-generator-announcement");
			this._radio.SendRadioMessage(uid, message, this._prototype.Index<RadioChannelPrototype>(component.ScienceChannel), null);
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x000E24A4 File Offset: 0x000E06A4
		private void UpdateGenerator()
		{
			foreach (ValueTuple<GeneratingAnomalyGeneratorComponent, AnomalyGeneratorComponent> valueTuple in base.EntityQuery<GeneratingAnomalyGeneratorComponent, AnomalyGeneratorComponent>(false))
			{
				GeneratingAnomalyGeneratorComponent active = valueTuple.Item1;
				AnomalyGeneratorComponent gen = valueTuple.Item2;
				EntityUid ent = active.Owner;
				if (!(this.Timing.CurTime < active.EndTime))
				{
					IPlayingAudioStream audioStream = active.AudioStream;
					if (audioStream != null)
					{
						audioStream.Stop();
					}
					this.OnGeneratingFinished(ent, gen);
				}
			}
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x000E2530 File Offset: 0x000E0730
		private void InitializeScanner()
		{
			base.SubscribeLocalEvent<AnomalyScannerComponent, BoundUIOpenedEvent>(new ComponentEventHandler<AnomalyScannerComponent, BoundUIOpenedEvent>(this.OnScannerUiOpened), null, null);
			base.SubscribeLocalEvent<AnomalyScannerComponent, AfterInteractEvent>(new ComponentEventHandler<AnomalyScannerComponent, AfterInteractEvent>(this.OnScannerAfterInteract), null, null);
			base.SubscribeLocalEvent<AnomalyScannerComponent, DoAfterEvent>(new ComponentEventHandler<AnomalyScannerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<AnomalyShutdownEvent>(new EntityEventRefHandler<AnomalyShutdownEvent>(this.OnScannerAnomalyShutdown), null, null);
			base.SubscribeLocalEvent<AnomalySeverityChangedEvent>(new EntityEventRefHandler<AnomalySeverityChangedEvent>(this.OnScannerAnomalySeverityChanged), null, null);
			base.SubscribeLocalEvent<AnomalyStabilityChangedEvent>(new EntityEventRefHandler<AnomalyStabilityChangedEvent>(this.OnScannerAnomalyStabilityChanged), null, null);
			base.SubscribeLocalEvent<AnomalyHealthChangedEvent>(new EntityEventRefHandler<AnomalyHealthChangedEvent>(this.OnScannerAnomalyHealthChanged), null, null);
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x000E25CC File Offset: 0x000E07CC
		private void OnScannerAnomalyShutdown(ref AnomalyShutdownEvent args)
		{
			foreach (AnomalyScannerComponent component in base.EntityQuery<AnomalyScannerComponent>(false))
			{
				EntityUid? scannedAnomaly = component.ScannedAnomaly;
				EntityUid anomaly = args.Anomaly;
				if (scannedAnomaly != null && (scannedAnomaly == null || !(scannedAnomaly.GetValueOrDefault() != anomaly)))
				{
					this._ui.TryCloseAll(component.Owner, AnomalyScannerUiKey.Key, null);
				}
			}
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x000E2664 File Offset: 0x000E0864
		private void OnScannerAnomalySeverityChanged(ref AnomalySeverityChangedEvent args)
		{
			foreach (AnomalyScannerComponent component in base.EntityQuery<AnomalyScannerComponent>(false))
			{
				EntityUid? scannedAnomaly = component.ScannedAnomaly;
				EntityUid anomaly = args.Anomaly;
				if (scannedAnomaly != null && (scannedAnomaly == null || !(scannedAnomaly.GetValueOrDefault() != anomaly)))
				{
					this.UpdateScannerUi(component.Owner, component);
				}
			}
		}

		// Token: 0x06002B3D RID: 11069 RVA: 0x000E26F0 File Offset: 0x000E08F0
		private void OnScannerAnomalyStabilityChanged(ref AnomalyStabilityChangedEvent args)
		{
			foreach (AnomalyScannerComponent component in base.EntityQuery<AnomalyScannerComponent>(false))
			{
				EntityUid? scannedAnomaly = component.ScannedAnomaly;
				EntityUid anomaly = args.Anomaly;
				if (scannedAnomaly != null && (scannedAnomaly == null || !(scannedAnomaly.GetValueOrDefault() != anomaly)))
				{
					this.UpdateScannerUi(component.Owner, component);
				}
			}
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x000E277C File Offset: 0x000E097C
		private void OnScannerAnomalyHealthChanged(ref AnomalyHealthChangedEvent args)
		{
			foreach (AnomalyScannerComponent component in base.EntityQuery<AnomalyScannerComponent>(false))
			{
				EntityUid? scannedAnomaly = component.ScannedAnomaly;
				EntityUid anomaly = args.Anomaly;
				if (scannedAnomaly != null && (scannedAnomaly == null || !(scannedAnomaly.GetValueOrDefault() != anomaly)))
				{
					this.UpdateScannerUi(component.Owner, component);
				}
			}
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x000E2808 File Offset: 0x000E0A08
		private void OnScannerUiOpened(EntityUid uid, AnomalyScannerComponent component, BoundUIOpenedEvent args)
		{
			this.UpdateScannerUi(uid, component);
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000E2814 File Offset: 0x000E0A14
		private void OnScannerAfterInteract(EntityUid uid, AnomalyScannerComponent component, AfterInteractEvent args)
		{
			EntityUid? target2 = args.Target;
			if (target2 == null)
			{
				return;
			}
			EntityUid target = target2.GetValueOrDefault();
			if (!base.HasComp<AnomalyComponent>(target))
			{
				return;
			}
			SharedDoAfterSystem doAfter = this._doAfter;
			EntityUid user = args.User;
			float scanDoAfterDuration = component.ScanDoAfterDuration;
			target2 = new EntityUid?(target);
			EntityUid? used = new EntityUid?(uid);
			doAfter.DoAfter(new DoAfterEventArgs(user, scanDoAfterDuration, default(CancellationToken), target2, used)
			{
				DistanceThreshold = new float?(2f)
			});
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x000E2890 File Offset: 0x000E0A90
		private void OnDoAfter(EntityUid uid, AnomalyScannerComponent component, DoAfterEvent args)
		{
			if (args.Cancelled || args.Handled || args.Args.Target == null)
			{
				return;
			}
			this.Audio.PlayPvs(component.CompleteSound, uid, null);
			this.Popup.PopupEntity(Loc.GetString("anomaly-scanner-component-scan-complete"), uid, PopupType.Small);
			this.UpdateScannerWithNewAnomaly(uid, args.Args.Target.Value, component, null);
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(args.Args.User, ref actor))
			{
				this._ui.TryOpen(uid, AnomalyScannerUiKey.Key, actor.PlayerSession, null);
			}
			args.Handled = true;
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x000E2944 File Offset: 0x000E0B44
		[NullableContext(2)]
		public void UpdateScannerUi(EntityUid uid, AnomalyScannerComponent component = null)
		{
			if (!base.Resolve<AnomalyScannerComponent>(uid, ref component, true))
			{
				return;
			}
			TimeSpan? nextPulse = null;
			AnomalyComponent anomalyComponent;
			if (base.TryComp<AnomalyComponent>(component.ScannedAnomaly, ref anomalyComponent))
			{
				nextPulse = new TimeSpan?(anomalyComponent.NextPulseTime);
			}
			AnomalyScannerUserInterfaceState state = new AnomalyScannerUserInterfaceState(this.GetScannerMessage(component), nextPulse);
			this._ui.TrySetUiState(uid, AnomalyScannerUiKey.Key, state, null, null, true);
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000E29A8 File Offset: 0x000E0BA8
		[NullableContext(2)]
		public void UpdateScannerWithNewAnomaly(EntityUid scanner, EntityUid anomaly, AnomalyScannerComponent scannerComp = null, AnomalyComponent anomalyComp = null)
		{
			if (!base.Resolve<AnomalyScannerComponent>(scanner, ref scannerComp, true) || !base.Resolve<AnomalyComponent>(anomaly, ref anomalyComp, true))
			{
				return;
			}
			scannerComp.ScannedAnomaly = new EntityUid?(anomaly);
			this.UpdateScannerUi(scanner, scannerComp);
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x000E29D8 File Offset: 0x000E0BD8
		public FormattedMessage GetScannerMessage(AnomalyScannerComponent component)
		{
			FormattedMessage msg = new FormattedMessage();
			EntityUid? scannedAnomaly = component.ScannedAnomaly;
			if (scannedAnomaly != null)
			{
				EntityUid anomaly = scannedAnomaly.GetValueOrDefault();
				AnomalyComponent anomalyComp;
				if (base.TryComp<AnomalyComponent>(anomaly, ref anomalyComp))
				{
					msg.AddMarkup(Loc.GetString("anomaly-scanner-severity-percentage", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("percent", anomalyComp.Severity.ToString("P"))
					}));
					msg.PushNewline();
					string stateLoc;
					if (anomalyComp.Stability < anomalyComp.DecayThreshold)
					{
						stateLoc = Loc.GetString("anomaly-scanner-stability-low");
					}
					else if (anomalyComp.Stability > anomalyComp.GrowthThreshold)
					{
						stateLoc = Loc.GetString("anomaly-scanner-stability-high");
					}
					else
					{
						stateLoc = Loc.GetString("anomaly-scanner-stability-medium");
					}
					msg.AddMarkup(stateLoc);
					msg.PushNewline();
					int points = this.GetAnomalyPointValue(anomaly, anomalyComp) / 10 * 10;
					msg.AddMarkup(Loc.GetString("anomaly-scanner-point-output", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("point", points)
					}));
					msg.PushNewline();
					msg.PushNewline();
					msg.AddMarkup(Loc.GetString("anomaly-scanner-particle-readout"));
					msg.PushNewline();
					msg.AddMarkup(Loc.GetString("anomaly-scanner-particle-danger", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("type", this.GetParticleLocale(anomalyComp.SeverityParticleType))
					}));
					msg.PushNewline();
					msg.AddMarkup(Loc.GetString("anomaly-scanner-particle-unstable", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("type", this.GetParticleLocale(anomalyComp.DestabilizingParticleType))
					}));
					msg.PushNewline();
					msg.AddMarkup(Loc.GetString("anomaly-scanner-particle-containment", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("type", this.GetParticleLocale(anomalyComp.WeakeningParticleType))
					}));
					return msg;
				}
			}
			msg.AddMarkup(Loc.GetString("anomaly-scanner-no-anomaly"));
			return msg;
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x000E2BB4 File Offset: 0x000E0DB4
		private void InitializeVessel()
		{
			base.SubscribeLocalEvent<AnomalyVesselComponent, ComponentShutdown>(new ComponentEventHandler<AnomalyVesselComponent, ComponentShutdown>(this.OnVesselShutdown), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, MapInitEvent>(new ComponentEventHandler<AnomalyVesselComponent, MapInitEvent>(this.OnVesselMapInit), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, RefreshPartsEvent>(new ComponentEventHandler<AnomalyVesselComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, UpgradeExamineEvent>(new ComponentEventHandler<AnomalyVesselComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, InteractUsingEvent>(new ComponentEventHandler<AnomalyVesselComponent, InteractUsingEvent>(this.OnVesselInteractUsing), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, ExaminedEvent>(new ComponentEventHandler<AnomalyVesselComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, ResearchServerGetPointsPerSecondEvent>(new ComponentEventRefHandler<AnomalyVesselComponent, ResearchServerGetPointsPerSecondEvent>(this.OnVesselGetPointsPerSecond), null, null);
			base.SubscribeLocalEvent<AnomalyVesselComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AnomalyVesselComponent, EntityUnpausedEvent>(this.OnUnpaused), null, null);
			base.SubscribeLocalEvent<AnomalyShutdownEvent>(new EntityEventRefHandler<AnomalyShutdownEvent>(this.OnVesselAnomalyShutdown), null, null);
			base.SubscribeLocalEvent<AnomalyStabilityChangedEvent>(new EntityEventRefHandler<AnomalyStabilityChangedEvent>(this.OnVesselAnomalyStabilityChanged), null, null);
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x000E2C89 File Offset: 0x000E0E89
		private void OnExamined(EntityUid uid, AnomalyVesselComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			args.PushText((component.Anomaly == null) ? Loc.GetString("anomaly-vessel-component-not-assigned") : Loc.GetString("anomaly-vessel-component-assigned"));
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x000E2CC0 File Offset: 0x000E0EC0
		private void OnVesselShutdown(EntityUid uid, AnomalyVesselComponent component, ComponentShutdown args)
		{
			EntityUid? anomaly2 = component.Anomaly;
			if (anomaly2 == null)
			{
				return;
			}
			EntityUid anomaly = anomaly2.GetValueOrDefault();
			AnomalyComponent anomalyComp;
			if (!base.TryComp<AnomalyComponent>(anomaly, ref anomalyComp))
			{
				return;
			}
			anomalyComp.ConnectedVessel = null;
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000E2D00 File Offset: 0x000E0F00
		private void OnVesselMapInit(EntityUid uid, AnomalyVesselComponent component, MapInitEvent args)
		{
			this.UpdateVesselAppearance(uid, component);
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x000E2D0C File Offset: 0x000E0F0C
		private void OnRefreshParts(EntityUid uid, AnomalyVesselComponent component, RefreshPartsEvent args)
		{
			float modifierRating = args.PartRatings[component.MachinePartPointModifier] - 1f;
			component.PointMultiplier = MathF.Pow(component.PartRatingPointModifier, modifierRating);
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x000E2D43 File Offset: 0x000E0F43
		private void OnUpgradeExamine(EntityUid uid, AnomalyVesselComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("anomaly-vessel-component-upgrade-output", component.PointMultiplier);
		}

		// Token: 0x06002B4B RID: 11083 RVA: 0x000E2D58 File Offset: 0x000E0F58
		private void OnVesselInteractUsing(EntityUid uid, AnomalyVesselComponent component, InteractUsingEvent args)
		{
			AnomalyScannerComponent scanner;
			if (component.Anomaly == null && base.TryComp<AnomalyScannerComponent>(args.Used, ref scanner))
			{
				EntityUid? scannedAnomaly = scanner.ScannedAnomaly;
				if (scannedAnomaly != null)
				{
					EntityUid anomaly = scannedAnomaly.GetValueOrDefault();
					AnomalyComponent anomalyComponent;
					if (!base.TryComp<AnomalyComponent>(anomaly, ref anomalyComponent) || anomalyComponent.ConnectedVessel != null)
					{
						return;
					}
					component.Anomaly = scanner.ScannedAnomaly;
					anomalyComponent.ConnectedVessel = new EntityUid?(uid);
					this.UpdateVesselAppearance(uid, component);
					this.Popup.PopupEntity(Loc.GetString("anomaly-vessel-component-anomaly-assigned"), uid, PopupType.Small);
					return;
				}
			}
		}

		// Token: 0x06002B4C RID: 11084 RVA: 0x000E2DF0 File Offset: 0x000E0FF0
		private void OnVesselGetPointsPerSecond(EntityUid uid, AnomalyVesselComponent component, ref ResearchServerGetPointsPerSecondEvent args)
		{
			if (this.IsPowered(uid, this.EntityManager, null))
			{
				EntityUid? anomaly2 = component.Anomaly;
				if (anomaly2 != null)
				{
					EntityUid anomaly = anomaly2.GetValueOrDefault();
					args.Points += (int)((float)this.GetAnomalyPointValue(anomaly, null) * component.PointMultiplier);
					return;
				}
			}
		}

		// Token: 0x06002B4D RID: 11085 RVA: 0x000E2E46 File Offset: 0x000E1046
		private void OnUnpaused(EntityUid uid, AnomalyVesselComponent component, ref EntityUnpausedEvent args)
		{
			component.NextBeep += args.PausedTime;
		}

		// Token: 0x06002B4E RID: 11086 RVA: 0x000E2E60 File Offset: 0x000E1060
		private void OnVesselAnomalyShutdown(ref AnomalyShutdownEvent args)
		{
			foreach (AnomalyVesselComponent component in base.EntityQuery<AnomalyVesselComponent>(false))
			{
				EntityUid ent = component.Owner;
				if (!(args.Anomaly != component.Anomaly))
				{
					component.Anomaly = null;
					this.UpdateVesselAppearance(ent, component);
					if (args.Supercritical)
					{
						this._explosion.TriggerExplosive(ent, null, true, null, null, null);
					}
				}
			}
		}

		// Token: 0x06002B4F RID: 11087 RVA: 0x000E2F28 File Offset: 0x000E1128
		private void OnVesselAnomalyStabilityChanged(ref AnomalyStabilityChangedEvent args)
		{
			foreach (AnomalyVesselComponent component in base.EntityQuery<AnomalyVesselComponent>(false))
			{
				EntityUid ent = component.Owner;
				if (!(args.Anomaly != component.Anomaly))
				{
					this.UpdateVesselAppearance(ent, component);
				}
			}
		}

		// Token: 0x06002B50 RID: 11088 RVA: 0x000E2FA8 File Offset: 0x000E11A8
		[NullableContext(2)]
		public void UpdateVesselAppearance(EntityUid uid, AnomalyVesselComponent component = null)
		{
			if (!base.Resolve<AnomalyVesselComponent>(uid, ref component, true))
			{
				return;
			}
			bool on = component.Anomaly != null;
			AppearanceComponent appearanceComponent;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
			{
				return;
			}
			this.Appearance.SetData(uid, AnomalyVesselVisuals.HasAnomaly, on, appearanceComponent);
			SharedPointLightComponent pointLightComponent;
			if (base.TryComp<SharedPointLightComponent>(uid, ref pointLightComponent))
			{
				pointLightComponent.Enabled = on;
			}
			int value = 1;
			AnomalyComponent anomalyComp;
			if (base.TryComp<AnomalyComponent>(component.Anomaly, ref anomalyComp))
			{
				if (anomalyComp.Stability <= anomalyComp.DecayThreshold)
				{
					value = 2;
				}
				else if (anomalyComp.Stability >= anomalyComp.GrowthThreshold)
				{
					value = 3;
				}
			}
			this.Appearance.SetData(uid, AnomalyVesselVisuals.AnomalyState, value, appearanceComponent);
			this._ambient.SetAmbience(uid, on, null);
		}

		// Token: 0x06002B51 RID: 11089 RVA: 0x000E3064 File Offset: 0x000E1264
		private void UpdateVessels()
		{
			foreach (AnomalyVesselComponent vessel in base.EntityQuery<AnomalyVesselComponent>(false))
			{
				EntityUid vesselEnt = vessel.Owner;
				EntityUid? anomaly2 = vessel.Anomaly;
				if (anomaly2 != null)
				{
					EntityUid anomUid = anomaly2.GetValueOrDefault();
					AnomalyComponent anomaly;
					if (base.TryComp<AnomalyComponent>(anomUid, ref anomaly) && !(this.Timing.CurTime < vessel.NextBeep))
					{
						float timerPercentage;
						if (anomaly.Stability <= anomaly.DecayThreshold)
						{
							timerPercentage = (anomaly.DecayThreshold - anomaly.Stability) / anomaly.DecayThreshold;
						}
						else
						{
							if (anomaly.Stability < anomaly.GrowthThreshold)
							{
								continue;
							}
							timerPercentage = (anomaly.Stability - anomaly.GrowthThreshold) / (1f - anomaly.GrowthThreshold);
						}
						this.Audio.PlayPvs(vessel.BeepSound, vesselEnt, null);
						TimeSpan beepInterval = (vessel.MaxBeepInterval - vessel.MinBeepInterval) * (double)(1f - timerPercentage) + vessel.MinBeepInterval;
						vessel.NextBeep = beepInterval + this.Timing.CurTime;
					}
				}
			}
		}

		// Token: 0x06002B53 RID: 11091 RVA: 0x000E31D0 File Offset: 0x000E13D0
		[CompilerGenerated]
		private float <OnStartCollide>g__VaryValue|14_0(float v)
		{
			return v * this.Random.NextFloat(0.8f, 1.2f);
		}

		// Token: 0x04001AB1 RID: 6833
		[Dependency]
		private readonly IConfigurationManager _configuration;

		// Token: 0x04001AB2 RID: 6834
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04001AB3 RID: 6835
		[Dependency]
		private readonly AmbientSoundSystem _ambient;

		// Token: 0x04001AB4 RID: 6836
		[Dependency]
		private readonly AtmosphereSystem _atmosphere;

		// Token: 0x04001AB5 RID: 6837
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04001AB6 RID: 6838
		[Dependency]
		private readonly ExplosionSystem _explosion;

		// Token: 0x04001AB7 RID: 6839
		[Dependency]
		private readonly MaterialStorageSystem _material;

		// Token: 0x04001AB8 RID: 6840
		[Dependency]
		private readonly RadioSystem _radio;

		// Token: 0x04001AB9 RID: 6841
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x04001ABA RID: 6842
		public const float MinParticleVariation = 0.8f;

		// Token: 0x04001ABB RID: 6843
		public const float MaxParticleVariation = 1.2f;
	}
}
