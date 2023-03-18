using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Server.Cloning.Components;
using Content.Server.Construction;
using Content.Server.EUI;
using Content.Server.Fluids.EntitySystems;
using Content.Server.Humanoid;
using Content.Server.Jobs;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.Materials;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Bed.Sleep;
using Content.Shared.CCVar;
using Content.Shared.Chemistry.Components;
using Content.Shared.Cloning;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Cloning
{
	// Token: 0x02000640 RID: 1600
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CloningSystem : EntitySystem
	{
		// Token: 0x06002200 RID: 8704 RVA: 0x000B15DC File Offset: 0x000AF7DC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CloningPodComponent, ComponentInit>(new ComponentEventHandler<CloningPodComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<CloningPodComponent, RefreshPartsEvent>(new ComponentEventHandler<CloningPodComponent, RefreshPartsEvent>(this.OnPartsRefreshed), null, null);
			base.SubscribeLocalEvent<CloningPodComponent, UpgradeExamineEvent>(new ComponentEventHandler<CloningPodComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<BeingClonedComponent, MindAddedMessage>(new ComponentEventHandler<BeingClonedComponent, MindAddedMessage>(this.HandleMindAdded), null, null);
			base.SubscribeLocalEvent<CloningPodComponent, PortDisconnectedEvent>(new ComponentEventHandler<CloningPodComponent, PortDisconnectedEvent>(this.OnPortDisconnected), null, null);
			base.SubscribeLocalEvent<CloningPodComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<CloningPodComponent, AnchorStateChangedEvent>(this.OnAnchor), null, null);
			base.SubscribeLocalEvent<CloningPodComponent, ExaminedEvent>(new ComponentEventHandler<CloningPodComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x000B168F File Offset: 0x000AF88F
		private void OnComponentInit(EntityUid uid, CloningPodComponent clonePod, ComponentInit args)
		{
			clonePod.BodyContainer = this._containerSystem.EnsureContainer<ContainerSlot>(clonePod.Owner, "clonepod-bodyContainer", null);
			this._signalSystem.EnsureReceiverPorts(uid, new string[]
			{
				"CloningPodReceiver"
			});
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x000B16C8 File Offset: 0x000AF8C8
		private void OnPartsRefreshed(EntityUid uid, CloningPodComponent component, RefreshPartsEvent args)
		{
			float materialRating = args.PartRatings[component.MachinePartMaterialUse];
			float speedRating = args.PartRatings[component.MachinePartCloningSpeed];
			component.BiomassRequirementMultiplier = MathF.Pow(component.PartRatingMaterialMultiplier, materialRating - 1f);
			component.CloningTime = component.BaseCloningTime * MathF.Pow(component.PartRatingSpeedMultiplier, speedRating - 1f);
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x000B1730 File Offset: 0x000AF930
		private void OnUpgradeExamine(EntityUid uid, CloningPodComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("cloning-pod-component-upgrade-speed", component.BaseCloningTime / component.CloningTime);
			args.AddPercentageUpgrade("cloning-pod-component-upgrade-biomass-requirement", component.BiomassRequirementMultiplier);
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x000B175C File Offset: 0x000AF95C
		internal void TransferMindToClone(Mind mind)
		{
			EntityUid entity;
			MindComponent mindComp;
			if (!this.ClonesWaitingForMind.TryGetValue(mind, out entity) || !this.EntityManager.EntityExists(entity) || !base.TryComp<MindComponent>(entity, ref mindComp) || mindComp.Mind != null)
			{
				return;
			}
			mind.TransferTo(new EntityUid?(entity), true, false);
			mind.UnVisit();
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.GhostRoleTaken;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(17, 1);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity), "entity", "ToPrettyString(entity)");
			logStringHandler.AppendLiteral(" has been cloned!");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.ClonesWaitingForMind.Remove(mind);
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x000B17FC File Offset: 0x000AF9FC
		private void HandleMindAdded(EntityUid uid, BeingClonedComponent clonedComponent, MindAddedMessage message)
		{
			CloningPodComponent cloningPodComponent;
			if (clonedComponent.Parent == EntityUid.Invalid || !this.EntityManager.EntityExists(clonedComponent.Parent) || !base.TryComp<CloningPodComponent>(clonedComponent.Parent, ref cloningPodComponent) || clonedComponent.Owner != cloningPodComponent.BodyContainer.ContainedEntity)
			{
				this.EntityManager.RemoveComponent<BeingClonedComponent>(clonedComponent.Owner);
				return;
			}
			this.UpdateStatus(CloningPodStatus.Cloning, cloningPodComponent);
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x000B1887 File Offset: 0x000AFA87
		private void OnPortDisconnected(EntityUid uid, CloningPodComponent pod, PortDisconnectedEvent args)
		{
			pod.ConnectedConsole = null;
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x000B1898 File Offset: 0x000AFA98
		private void OnAnchor(EntityUid uid, CloningPodComponent component, ref AnchorStateChangedEvent args)
		{
			CloningConsoleComponent console;
			if (component.ConnectedConsole == null || !base.TryComp<CloningConsoleComponent>(component.ConnectedConsole, ref console))
			{
				return;
			}
			if (args.Anchored)
			{
				this._cloningConsoleSystem.RecheckConnections(component.ConnectedConsole.Value, new EntityUid?(uid), console.GeneticScanner, console);
				return;
			}
			this._cloningConsoleSystem.UpdateUserInterface(console);
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x000B18FC File Offset: 0x000AFAFC
		private void OnExamined(EntityUid uid, CloningPodComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange || !this._powerReceiverSystem.IsPowered(uid, null))
			{
				return;
			}
			args.PushMarkup(Loc.GetString("cloning-pod-biomass", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("number", this._material.GetMaterialAmount(uid, component.RequiredMaterial, null))
			}));
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x000B1960 File Offset: 0x000AFB60
		public bool TryCloning(EntityUid uid, EntityUid bodyToClone, Mind mind, [Nullable(2)] CloningPodComponent clonePod, float failChanceModifier = 1f)
		{
			if (!base.Resolve<CloningPodComponent>(uid, ref clonePod, true))
			{
				return false;
			}
			if (base.HasComp<ActiveCloningPodComponent>(uid))
			{
				return false;
			}
			EntityUid clone;
			if (this.ClonesWaitingForMind.TryGetValue(mind, out clone))
			{
				MindComponent cloneMindComp;
				if (this.EntityManager.EntityExists(clone) && !this._mobStateSystem.IsDead(clone, null) && base.TryComp<MindComponent>(clone, ref cloneMindComp) && (cloneMindComp.Mind == null || cloneMindComp.Mind == mind))
				{
					return false;
				}
				this.ClonesWaitingForMind.Remove(mind);
			}
			if (mind.OwnedEntity != null && !this._mobStateSystem.IsDead(mind.OwnedEntity.Value, null))
			{
				return false;
			}
			IPlayerSession client;
			if (mind.UserId == null || !this._playerManager.TryGetSessionById(mind.UserId.Value, ref client))
			{
				return false;
			}
			HumanoidAppearanceComponent humanoid;
			if (!base.TryComp<HumanoidAppearanceComponent>(bodyToClone, ref humanoid))
			{
				return false;
			}
			SpeciesPrototype speciesPrototype;
			if (!this._prototype.TryIndex<SpeciesPrototype>(humanoid.Species, ref speciesPrototype))
			{
				return false;
			}
			PhysicsComponent physics;
			if (!base.TryComp<PhysicsComponent>(bodyToClone, ref physics))
			{
				return false;
			}
			int cloningCost = (int)Math.Round((double)(physics.FixturesMass * clonePod.BiomassRequirementMultiplier));
			if (this._configManager.GetCVar<bool>(CCVars.BiomassEasyMode))
			{
				cloningCost = (int)Math.Round((double)((float)cloningCost * 0.7f));
			}
			if (this._material.GetMaterialAmount(uid, clonePod.RequiredMaterial, null) < cloningCost)
			{
				if (clonePod.ConnectedConsole != null)
				{
					this._chatSystem.TrySendInGameICMessage(clonePod.ConnectedConsole.Value, Loc.GetString("cloning-console-chat-error", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("units", cloningCost)
					}), InGameICChatType.Speak, false, false, null, null, null, true, false);
				}
				return false;
			}
			this._material.TryChangeMaterialAmount(uid, clonePod.RequiredMaterial, -cloningCost, null);
			clonePod.UsedBiomass = cloningCost;
			DamageableComponent damageable;
			FixedPoint2 cellularDmg;
			if (base.TryComp<DamageableComponent>(bodyToClone, ref damageable) && damageable.Damage.DamageDict.TryGetValue("Cellular", out cellularDmg))
			{
				float chance = Math.Clamp((float)(cellularDmg / 100f), 0f, 1f);
				chance *= failChanceModifier;
				if (cellularDmg > 0 && clonePod.ConnectedConsole != null)
				{
					this._chatSystem.TrySendInGameICMessage(clonePod.ConnectedConsole.Value, Loc.GetString("cloning-console-cellular-warning", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("percent", Math.Round((double)(100f - chance * 100f)))
					}), InGameICChatType.Speak, false, false, null, null, null, true, false);
				}
				if (RandomExtensions.Prob(this._robustRandom, chance))
				{
					this.UpdateStatus(CloningPodStatus.Gore, clonePod);
					clonePod.FailedClone = true;
					base.AddComp<ActiveCloningPodComponent>(uid);
					return true;
				}
			}
			EntityUid mob = base.Spawn(speciesPrototype.Prototype, base.Transform(clonePod.Owner).MapPosition);
			this._humanoidSystem.CloneAppearance(bodyToClone, mob, null, null);
			CloningEvent ev = new CloningEvent(bodyToClone, mob);
			base.RaiseLocalEvent<CloningEvent>(bodyToClone, ref ev, false);
			if (!ev.NameHandled)
			{
				base.MetaData(mob).EntityName = base.MetaData(bodyToClone).EntityName;
			}
			BeingClonedComponent beingClonedComponent = this.EntityManager.AddComponent<BeingClonedComponent>(mob);
			beingClonedComponent.Mind = mind;
			beingClonedComponent.Parent = clonePod.Owner;
			clonePod.BodyContainer.Insert(mob, null, null, null, null, null);
			this.ClonesWaitingForMind.Add(mind, mob);
			this.UpdateStatus(CloningPodStatus.NoMind, clonePod);
			this._euiManager.OpenEui(new AcceptCloningEui(mind, this), client);
			base.AddComp<ForcedSleepingComponent>(mob);
			base.AddComp<ActiveCloningPodComponent>(uid);
			if (mind.CurrentJob != null)
			{
				foreach (JobSpecial special in mind.CurrentJob.Prototype.Special)
				{
					if (special is AddComponentSpecial)
					{
						special.AfterEquip(mob);
					}
				}
			}
			return true;
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x000B1D4A File Offset: 0x000AFF4A
		public void UpdateStatus(CloningPodStatus status, CloningPodComponent cloningPod)
		{
			cloningPod.Status = status;
			this._appearance.SetData(cloningPod.Owner, CloningPodVisuals.Status, cloningPod.Status, null);
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x000B1D78 File Offset: 0x000AFF78
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<ActiveCloningPodComponent, CloningPodComponent> valueTuple in this.EntityManager.EntityQuery<ActiveCloningPodComponent, CloningPodComponent>(false))
			{
				CloningPodComponent cloning = valueTuple.Item2;
				if (this._powerReceiverSystem.IsPowered(cloning.Owner, null) && (cloning.BodyContainer.ContainedEntity != null || cloning.FailedClone))
				{
					cloning.CloningProgress += frameTime;
					if (cloning.CloningProgress >= cloning.CloningTime)
					{
						if (cloning.FailedClone)
						{
							this.EndFailedCloning(cloning.Owner, cloning);
						}
						else
						{
							this.Eject(cloning.Owner, cloning);
						}
					}
				}
			}
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x000B1E40 File Offset: 0x000B0040
		[NullableContext(2)]
		public void Eject(EntityUid uid, CloningPodComponent clonePod)
		{
			if (!base.Resolve<CloningPodComponent>(uid, ref clonePod, true))
			{
				return;
			}
			EntityUid? containedEntity = clonePod.BodyContainer.ContainedEntity;
			if (containedEntity != null)
			{
				EntityUid entity = containedEntity.GetValueOrDefault();
				if (entity.Valid && clonePod.CloningProgress >= clonePod.CloningTime)
				{
					this.EntityManager.RemoveComponent<BeingClonedComponent>(entity);
					clonePod.BodyContainer.Remove(entity, null, null, null, true, false, null, null);
					clonePod.CloningProgress = 0f;
					clonePod.UsedBiomass = 0;
					this.UpdateStatus(CloningPodStatus.Idle, clonePod);
					base.RemCompDeferred<ActiveCloningPodComponent>(uid);
					base.RemComp<ForcedSleepingComponent>(entity);
					return;
				}
			}
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x000B1EEC File Offset: 0x000B00EC
		private void EndFailedCloning(EntityUid uid, CloningPodComponent clonePod)
		{
			clonePod.FailedClone = false;
			clonePod.CloningProgress = 0f;
			this.UpdateStatus(CloningPodStatus.Idle, clonePod);
			TransformComponent transform = base.Transform(uid);
			Vector2i indices = this._transformSystem.GetGridOrMapTilePosition(uid, null);
			GasMixture tileMix = this._atmosphereSystem.GetTileMixture(transform.GridUid, null, indices, true);
			Solution bloodSolution = new Solution();
			int i = 0;
			while (i < 1)
			{
				if (tileMix != null)
				{
					tileMix.AdjustMoles(Gas.Miasma, 6f);
				}
				bloodSolution.AddReagent("Blood", 50, true);
				if (RandomExtensions.Prob(this._robustRandom, 0.2f))
				{
					i++;
				}
			}
			this._spillableSystem.SpillAt(uid, bloodSolution, "PuddleBlood", true, true, null);
			this._material.SpawnMultipleFromMaterial(this._robustRandom.Next(1, (int)((double)clonePod.UsedBiomass / 2.5)), clonePod.RequiredMaterial, base.Transform(uid).Coordinates);
			clonePod.UsedBiomass = 0;
			base.RemCompDeferred<ActiveCloningPodComponent>(uid);
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x000B1FF2 File Offset: 0x000B01F2
		public void Reset(RoundRestartCleanupEvent ev)
		{
			this.ClonesWaitingForMind.Clear();
		}

		// Token: 0x040014D7 RID: 5335
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x040014D8 RID: 5336
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040014D9 RID: 5337
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x040014DA RID: 5338
		[Dependency]
		private readonly EuiManager _euiManager;

		// Token: 0x040014DB RID: 5339
		[Dependency]
		private readonly CloningConsoleSystem _cloningConsoleSystem;

		// Token: 0x040014DC RID: 5340
		[Dependency]
		private readonly HumanoidAppearanceSystem _humanoidSystem;

		// Token: 0x040014DD RID: 5341
		[Dependency]
		private readonly ContainerSystem _containerSystem;

		// Token: 0x040014DE RID: 5342
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x040014DF RID: 5343
		[Dependency]
		private readonly PowerReceiverSystem _powerReceiverSystem;

		// Token: 0x040014E0 RID: 5344
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040014E1 RID: 5345
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040014E2 RID: 5346
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x040014E3 RID: 5347
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x040014E4 RID: 5348
		[Dependency]
		private readonly SpillableSystem _spillableSystem;

		// Token: 0x040014E5 RID: 5349
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x040014E6 RID: 5350
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x040014E7 RID: 5351
		[Dependency]
		private readonly MaterialStorageSystem _material;

		// Token: 0x040014E8 RID: 5352
		[Dependency]
		private readonly ISharedAdminLogManager _adminLogger;

		// Token: 0x040014E9 RID: 5353
		public readonly Dictionary<Mind, EntityUid> ClonesWaitingForMind = new Dictionary<Mind, EntityUid>();

		// Token: 0x040014EA RID: 5354
		public const float EasyModeCloningCost = 0.7f;
	}
}
