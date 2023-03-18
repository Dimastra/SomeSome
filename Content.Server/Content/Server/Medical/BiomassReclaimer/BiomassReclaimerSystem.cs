using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Body.Components;
using Content.Server.Climbing;
using Content.Server.Construction;
using Content.Server.DoAfter;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Materials;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Power.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Construction.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Throwing;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Random;

namespace Content.Server.Medical.BiomassReclaimer
{
	// Token: 0x020003C3 RID: 963
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BiomassReclaimerSystem : EntitySystem
	{
		// Token: 0x060013C0 RID: 5056 RVA: 0x00066278 File Offset: 0x00064478
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveBiomassReclaimerComponent, BiomassReclaimerComponent> valueTuple in base.EntityQuery<ActiveBiomassReclaimerComponent, BiomassReclaimerComponent>(false))
			{
				BiomassReclaimerComponent reclaimer = valueTuple.Item2;
				reclaimer.ProcessingTimer -= frameTime;
				reclaimer.RandomMessTimer -= frameTime;
				if (reclaimer.RandomMessTimer <= 0f)
				{
					if (RandomExtensions.Prob(this._robustRandom, 0.2f) && reclaimer.BloodReagent != null)
					{
						Solution blood = new Solution();
						blood.AddReagent(reclaimer.BloodReagent, 50, true);
						this._spillableSystem.SpillAt(reclaimer.Owner, blood, "PuddleBlood", true, true, null);
					}
					if (RandomExtensions.Prob(this._robustRandom, 0.03f) && reclaimer.SpawnedEntities.Count > 0)
					{
						EntityUid thrown = base.Spawn(RandomExtensions.Pick<EntitySpawnEntry>(this._robustRandom, reclaimer.SpawnedEntities).PrototypeId, base.Transform(reclaimer.Owner).Coordinates);
						Vector2 direction = new ValueTuple<float, float>((float)this._robustRandom.Next(-30, 30), (float)this._robustRandom.Next(-30, 30));
						this._throwing.TryThrow(thrown, direction, (float)this._robustRandom.Next(1, 10), null, 5f, null, null, null, null);
					}
					reclaimer.RandomMessTimer += (float)reclaimer.RandomMessInterval.TotalSeconds;
				}
				if (reclaimer.ProcessingTimer <= 0f)
				{
					this._material.SpawnMultipleFromMaterial(reclaimer.CurrentExpectedYield, "Biomass", base.Transform(reclaimer.Owner).Coordinates);
					reclaimer.BloodReagent = null;
					reclaimer.SpawnedEntities.Clear();
					base.RemCompDeferred<ActiveBiomassReclaimerComponent>(reclaimer.Owner);
				}
			}
		}

		// Token: 0x060013C1 RID: 5057 RVA: 0x00066488 File Offset: 0x00064688
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ActiveBiomassReclaimerComponent, ComponentInit>(new ComponentEventHandler<ActiveBiomassReclaimerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ActiveBiomassReclaimerComponent, ComponentShutdown>(new ComponentEventHandler<ActiveBiomassReclaimerComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<ActiveBiomassReclaimerComponent, UnanchorAttemptEvent>(new ComponentEventHandler<ActiveBiomassReclaimerComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, AfterInteractUsingEvent>(new ComponentEventHandler<BiomassReclaimerComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, ClimbedOnEvent>(new ComponentEventHandler<BiomassReclaimerComponent, ClimbedOnEvent>(this.OnClimbedOn), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, RefreshPartsEvent>(new ComponentEventHandler<BiomassReclaimerComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, UpgradeExamineEvent>(new ComponentEventHandler<BiomassReclaimerComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, PowerChangedEvent>(new ComponentEventRefHandler<BiomassReclaimerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, SuicideEvent>(new ComponentEventHandler<BiomassReclaimerComponent, SuicideEvent>(this.OnSuicide), null, null);
			base.SubscribeLocalEvent<BiomassReclaimerComponent, DoAfterEvent>(new ComponentEventHandler<BiomassReclaimerComponent, DoAfterEvent>(this.OnDoAfter), null, null);
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x00066564 File Offset: 0x00064764
		private void OnSuicide(EntityUid uid, BiomassReclaimerComponent component, SuicideEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (base.HasComp<ActiveBiomassReclaimerComponent>(uid))
			{
				return;
			}
			ApcPowerReceiverComponent power;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref power) && !power.Powered)
			{
				return;
			}
			this._popup.PopupEntity(Loc.GetString("biomass-reclaimer-suicide-others", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("victim", args.Victim)
			}), uid, PopupType.LargeCaution);
			this.StartProcessing(args.Victim, component, null);
			args.SetHandled(SuicideKind.Blunt);
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x000665E8 File Offset: 0x000647E8
		private void OnInit(EntityUid uid, ActiveBiomassReclaimerComponent component, ComponentInit args)
		{
			this._jitteringSystem.AddJitter(uid, -10f, 100f);
			this._sharedAudioSystem.PlayPvs("/Audio/Machines/reclaimer_startup.ogg", uid, null);
			this._ambientSoundSystem.SetAmbience(uid, true, null);
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x00066634 File Offset: 0x00064834
		private void OnShutdown(EntityUid uid, ActiveBiomassReclaimerComponent component, ComponentShutdown args)
		{
			base.RemComp<JitteringComponent>(uid);
			this._ambientSoundSystem.SetAmbience(uid, false, null);
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x0006664C File Offset: 0x0006484C
		private void OnPowerChanged(EntityUid uid, BiomassReclaimerComponent component, ref PowerChangedEvent args)
		{
			if (args.Powered)
			{
				if (component.ProcessingTimer > 0f)
				{
					base.EnsureComp<ActiveBiomassReclaimerComponent>(uid);
					return;
				}
			}
			else
			{
				base.RemComp<ActiveBiomassReclaimerComponent>(component.Owner);
			}
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x00066679 File Offset: 0x00064879
		private void OnUnanchorAttempt(EntityUid uid, ActiveBiomassReclaimerComponent component, UnanchorAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x00066684 File Offset: 0x00064884
		private void OnAfterInteractUsing(EntityUid uid, BiomassReclaimerComponent component, AfterInteractUsingEvent args)
		{
			if (!args.CanReach || args.Target == null)
			{
				return;
			}
			if (!base.HasComp<MobStateComponent>(args.Used) || !this.CanGib(uid, args.Used, component))
			{
				return;
			}
			SharedDoAfterSystem doAfterSystem = this._doAfterSystem;
			EntityUid user = args.User;
			float delay = 7f;
			EntityUid? target = args.Target;
			EntityUid? used = new EntityUid?(args.Used);
			doAfterSystem.DoAfter(new DoAfterEventArgs(user, delay, default(CancellationToken), target, used)
			{
				BreakOnTargetMove = true,
				BreakOnUserMove = true,
				BreakOnStun = true,
				NeedHand = true
			});
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x00066720 File Offset: 0x00064920
		private void OnClimbedOn(EntityUid uid, BiomassReclaimerComponent component, ClimbedOnEvent args)
		{
			if (!this.CanGib(uid, args.Climber, component))
			{
				Vector2 direction = new ValueTuple<float, float>((float)this._robustRandom.Next(-2, 2), (float)this._robustRandom.Next(-2, 2));
				this._throwing.TryThrow(args.Climber, direction, 0.5f, null, 5f, null, null, null, null);
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Extreme;
			LogStringHandler logStringHandler = new LogStringHandler(37, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Instigator), "player", "ToPrettyString(args.Instigator)");
			logStringHandler.AppendLiteral(" used a biomass reclaimer to gib ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Climber), "target", "ToPrettyString(args.Climber)");
			logStringHandler.AppendLiteral(" in ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "reclaimer", "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.StartProcessing(args.Climber, component, null);
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x00066834 File Offset: 0x00064A34
		private void OnRefreshParts(EntityUid uid, BiomassReclaimerComponent component, RefreshPartsEvent args)
		{
			float laserRating = args.PartRatings[component.MachinePartProcessingSpeed];
			float manipRating = args.PartRatings[component.MachinePartYieldAmount];
			component.ProcessingTimePerUnitMass = component.BaseProcessingTimePerUnitMass / MathF.Pow(component.PartRatingSpeedMultiplier, laserRating - 1f);
			component.YieldPerUnitMass = component.BaseYieldPerUnitMass * MathF.Pow(component.PartRatingYieldAmountMultiplier, manipRating - 1f);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x000668A3 File Offset: 0x00064AA3
		private void OnUpgradeExamine(EntityUid uid, BiomassReclaimerComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("biomass-reclaimer-component-upgrade-speed", component.BaseProcessingTimePerUnitMass / component.ProcessingTimePerUnitMass);
			args.AddPercentageUpgrade("biomass-reclaimer-component-upgrade-biomass-yield", component.YieldPerUnitMass / component.BaseYieldPerUnitMass);
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x000668D8 File Offset: 0x00064AD8
		private void OnDoAfter(EntityUid uid, BiomassReclaimerComponent component, DoAfterEvent args)
		{
			if (args.Handled || args.Cancelled || args.Args.Target == null || base.HasComp<BiomassReclaimerComponent>(args.Args.Target.Value))
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Extreme;
			LogStringHandler logStringHandler = new LogStringHandler(37, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.User), "player", "ToPrettyString(args.Args.User)");
			logStringHandler.AppendLiteral(" used a biomass reclaimer to gib ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Args.Target.Value), "target", "ToPrettyString(args.Args.Target.Value)");
			logStringHandler.AppendLiteral(" in ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "reclaimer", "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.StartProcessing(args.Args.Target.Value, component, null);
			args.Handled = true;
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x000669D4 File Offset: 0x00064BD4
		private void StartProcessing(EntityUid toProcess, BiomassReclaimerComponent component, [Nullable(2)] PhysicsComponent physics = null)
		{
			if (!base.Resolve<PhysicsComponent>(toProcess, ref physics, true))
			{
				return;
			}
			base.AddComp<ActiveBiomassReclaimerComponent>(component.Owner);
			BloodstreamComponent stream;
			if (base.TryComp<BloodstreamComponent>(toProcess, ref stream))
			{
				component.BloodReagent = stream.BloodReagent;
			}
			ButcherableComponent butcherableComponent;
			if (base.TryComp<ButcherableComponent>(toProcess, ref butcherableComponent))
			{
				component.SpawnedEntities = butcherableComponent.SpawnedEntities;
			}
			component.CurrentExpectedYield = (int)Math.Max(0f, physics.FixturesMass * component.YieldPerUnitMass);
			component.ProcessingTimer = physics.FixturesMass * component.ProcessingTimePerUnitMass;
			base.QueueDel(toProcess);
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x00066A64 File Offset: 0x00064C64
		private bool CanGib(EntityUid uid, EntityUid dragged, BiomassReclaimerComponent component)
		{
			if (base.HasComp<ActiveBiomassReclaimerComponent>(uid))
			{
				return false;
			}
			if (!base.HasComp<MobStateComponent>(dragged))
			{
				return false;
			}
			if (!base.Transform(uid).Anchored)
			{
				return false;
			}
			ApcPowerReceiverComponent power;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref power) && !power.Powered)
			{
				return false;
			}
			if (component.SafetyEnabled && !this._mobState.IsDead(dragged, null))
			{
				return false;
			}
			MindComponent mindComp;
			if (this._configManager.GetCVar<bool>(CCVars.BiomassEasyMode) && base.HasComp<HumanoidAppearanceComponent>(dragged) && base.TryComp<MindComponent>(dragged, ref mindComp))
			{
				Mind mind = mindComp.Mind;
				IPlayerSession playerSession;
				if (mind != null && mind.UserId != null && this._playerManager.TryGetSessionById(mindComp.Mind.UserId.Value, ref playerSession))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000C35 RID: 3125
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000C36 RID: 3126
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000C37 RID: 3127
		[Dependency]
		private readonly SharedJitteringSystem _jitteringSystem;

		// Token: 0x04000C38 RID: 3128
		[Dependency]
		private readonly SharedAudioSystem _sharedAudioSystem;

		// Token: 0x04000C39 RID: 3129
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04000C3A RID: 3130
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000C3B RID: 3131
		[Dependency]
		private readonly SpillableSystem _spillableSystem;

		// Token: 0x04000C3C RID: 3132
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x04000C3D RID: 3133
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000C3E RID: 3134
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x04000C3F RID: 3135
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x04000C40 RID: 3136
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000C41 RID: 3137
		[Dependency]
		private readonly MaterialStorageSystem _material;
	}
}
