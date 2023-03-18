using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Cargo.Systems;
using Content.Server.Chat.Managers;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Database;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x02000748 RID: 1864
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasCanisterSystem : EntitySystem
	{
		// Token: 0x0600271B RID: 10011 RVA: 0x000CDABC File Offset: 0x000CBCBC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasCanisterComponent, ComponentStartup>(new ComponentEventHandler<GasCanisterComponent, ComponentStartup>(this.OnCanisterStartup), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasCanisterComponent, AtmosDeviceUpdateEvent>(this.OnCanisterUpdated), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, ActivateInWorldEvent>(new ComponentEventHandler<GasCanisterComponent, ActivateInWorldEvent>(this.OnCanisterActivate), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, InteractHandEvent>(new ComponentEventHandler<GasCanisterComponent, InteractHandEvent>(this.OnCanisterInteractHand), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, InteractUsingEvent>(new ComponentEventHandler<GasCanisterComponent, InteractUsingEvent>(this.OnCanisterInteractUsing), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<GasCanisterComponent, EntInsertedIntoContainerMessage>(this.OnCanisterContainerInserted), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<GasCanisterComponent, EntRemovedFromContainerMessage>(this.OnCanisterContainerRemoved), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, PriceCalculationEvent>(new ComponentEventRefHandler<GasCanisterComponent, PriceCalculationEvent>(this.CalculateCanisterPrice), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<GasCanisterComponent, GasAnalyzerScanEvent>(this.OnAnalyzed), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterHoldingTankEjectMessage>(this.OnHoldingTankEjectMessage), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleasePressureMessage>(this.OnCanisterChangeReleasePressure), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>(new ComponentEventHandler<GasCanisterComponent, GasCanisterChangeReleaseValveMessage>(this.OnCanisterChangeReleaseValve), null, null);
			base.SubscribeLocalEvent<GasCanisterComponent, LockToggledEvent>(new ComponentEventRefHandler<GasCanisterComponent, LockToggledEvent>(this.OnLockToggled), null, null);
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000CDBD4 File Offset: 0x000CBDD4
		[NullableContext(2)]
		public void PurgeContents(EntityUid uid, GasCanisterComponent canister = null, TransformComponent transform = null)
		{
			if (!base.Resolve<GasCanisterComponent, TransformComponent>(uid, ref canister, ref transform, true))
			{
				return;
			}
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
			if (environment != null)
			{
				this._atmosphereSystem.Merge(environment, canister.Air);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterPurged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(55, 2);
			logStringHandler.AppendLiteral("Canister ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "canister", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" purged its contents of ");
			logStringHandler.AppendFormatted<GasMixture>(canister.Air, "gas", "canister.Air");
			logStringHandler.AppendLiteral(" into the environment.");
			adminLogger.Add(type, impact, ref logStringHandler);
			canister.Air.Clear();
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000CDC90 File Offset: 0x000CBE90
		private void OnCanisterStartup(EntityUid uid, GasCanisterComponent canister, ComponentStartup args)
		{
			ContainerManagerComponent containerManager = this.EntityManager.EnsureComponent<ContainerManagerComponent>(uid);
			IContainer container;
			if (!containerManager.TryGetContainer(canister.ContainerName, ref container))
			{
				containerManager.MakeContainer<ContainerSlot>(canister.ContainerName);
			}
			LockComponent lockComponent;
			if (base.TryComp<LockComponent>(uid, ref lockComponent))
			{
				this._appearanceSystem.SetData(uid, GasCanisterVisuals.Locked, lockComponent.Locked, null);
			}
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000CDCF0 File Offset: 0x000CBEF0
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasCanisterComponent canister = null, NodeContainerComponent nodeContainer = null, ContainerManagerComponent containerManager = null)
		{
			if (!base.Resolve<GasCanisterComponent, NodeContainerComponent, ContainerManagerComponent>(uid, ref canister, ref nodeContainer, ref containerManager, true))
			{
				return;
			}
			bool portStatus = false;
			string tankLabel = null;
			float tankPressure = 0f;
			PipeNode portNode;
			if (nodeContainer.TryGetNode<PipeNode>(canister.PortName, out portNode))
			{
				INodeGroup nodeGroup = portNode.NodeGroup;
				if (nodeGroup != null && nodeGroup.Nodes.Count > 1)
				{
					portStatus = true;
				}
			}
			IContainer tankContainer;
			if (containerManager.TryGetContainer(canister.ContainerName, ref tankContainer) && tankContainer.ContainedEntities.Count > 0)
			{
				EntityUid tank = tankContainer.ContainedEntities[0];
				GasTankComponent component = this.EntityManager.GetComponent<GasTankComponent>(tank);
				tankLabel = this.EntityManager.GetComponent<MetaDataComponent>(tank).EntityName;
				tankPressure = component.Air.Pressure;
			}
			this._userInterfaceSystem.TrySetUiState(uid, GasCanisterUiKey.Key, new GasCanisterBoundUserInterfaceState(this.EntityManager.GetComponent<MetaDataComponent>(canister.Owner).EntityName, canister.Air.Pressure, portStatus, tankLabel, tankPressure, canister.ReleasePressure, canister.ReleaseValve, canister.MinReleasePressure, canister.MaxReleasePressure), null, null, true);
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000CDDF8 File Offset: 0x000CBFF8
		private void OnHoldingTankEjectMessage(EntityUid uid, GasCanisterComponent canister, GasCanisterHoldingTankEjectMessage args)
		{
			ContainerManagerComponent containerManager;
			IContainer container;
			if (!this.EntityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager) || !containerManager.TryGetContainer(canister.ContainerName, ref container))
			{
				return;
			}
			if (container.ContainedEntities.Count == 0)
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterTankEjected;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(27, 3);
			logStringHandler.AppendLiteral("Player ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.GetValueOrDefault()), "player", "ToPrettyString(args.Session.AttachedEntity.GetValueOrDefault())");
			logStringHandler.AppendLiteral(" ejected tank ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(container.ContainedEntities[0]), "tank", "ToPrettyString(container.ContainedEntities[0])");
			logStringHandler.AppendLiteral(" from ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "canister", "ToPrettyString(uid)");
			adminLogger.Add(type, impact, ref logStringHandler);
			container.Remove(container.ContainedEntities[0], null, null, null, true, false, null, null);
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000CDF04 File Offset: 0x000CC104
		private void OnCanisterChangeReleasePressure(EntityUid uid, GasCanisterComponent canister, GasCanisterChangeReleasePressureMessage args)
		{
			float pressure = Math.Clamp(args.Pressure, canister.MinReleasePressure, canister.MaxReleasePressure);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterPressure;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(33, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.GetValueOrDefault()), "player", "ToPrettyString(args.Session.AttachedEntity.GetValueOrDefault())");
			logStringHandler.AppendLiteral(" set the release pressure on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "canister", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(args.Pressure, "args.Pressure");
			adminLogger.Add(type, impact, ref logStringHandler);
			canister.ReleasePressure = pressure;
			this.DirtyUI(uid, canister, null, null);
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000CDFC0 File Offset: 0x000CC1C0
		private void OnCanisterChangeReleaseValve(EntityUid uid, GasCanisterComponent canister, GasCanisterChangeReleaseValveMessage args)
		{
			LogImpact impact = LogImpact.High;
			ContainerManagerComponent containerManager;
			IContainer container;
			if (this.EntityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager) && containerManager.TryGetContainer(canister.ContainerName, ref container))
			{
				impact = ((container.ContainedEntities.Count != 0) ? LogImpact.Medium : LogImpact.High);
			}
			Dictionary<Gas, float> containedGasDict = new Dictionary<Gas, float>();
			Array containedGasArray = Enum.GetValues(typeof(Gas));
			for (int i = 0; i < containedGasArray.Length; i++)
			{
				containedGasDict.Add((Gas)i, canister.Air.Moles[i]);
			}
			EntityUid player = args.Session.AttachedEntity.GetValueOrDefault();
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterValve;
			LogImpact impact2 = impact;
			LogStringHandler logStringHandler = new LogStringHandler(44, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
			logStringHandler.AppendLiteral(" set the valve on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "canister", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<bool>(args.Valve, "valveState", "args.Valve");
			logStringHandler.AppendLiteral(" while it contained [");
			logStringHandler.AppendFormatted(string.Join<KeyValuePair<Gas, float>>(", ", containedGasDict));
			logStringHandler.AppendLiteral("]");
			adminLogger.Add(type, impact2, ref logStringHandler);
			if (args.Valve && containedGasDict[Gas.Plasma] >= (float)this._plasmaThreshold)
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-plasma-canister-opened", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("player", base.ToPrettyString(player)),
					new ValueTuple<string, object>("canister", base.ToPrettyString(uid))
				}));
			}
			canister.ReleaseValve = args.Valve;
			this.DirtyUI(uid, canister, null, null);
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000CE184 File Offset: 0x000CC384
		private void OnCanisterUpdated(EntityUid uid, GasCanisterComponent canister, AtmosDeviceUpdateEvent args)
		{
			this._atmosphereSystem.React(canister.Air, canister);
			NodeContainerComponent nodeContainer;
			AppearanceComponent appearance;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !this.EntityManager.TryGetComponent<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			PortablePipeNode portNode;
			if (!nodeContainer.TryGetNode<PortablePipeNode>(canister.PortName, out portNode))
			{
				return;
			}
			PipeNet net = portNode.NodeGroup as PipeNet;
			if (net != null && net.NodeCount > 1)
			{
				this.MixContainerWithPipeNet(canister.Air, net.Air);
			}
			ContainerManagerComponent containerManager = null;
			if (canister.ReleaseValve)
			{
				IContainer container;
				if (!this.EntityManager.TryGetComponent<ContainerManagerComponent>(uid, ref containerManager) || !containerManager.TryGetContainer(canister.ContainerName, ref container))
				{
					return;
				}
				if (container.ContainedEntities.Count > 0)
				{
					GasTankComponent gasTank = this.EntityManager.GetComponent<GasTankComponent>(container.ContainedEntities[0]);
					this._atmosphereSystem.ReleaseGasTo(canister.Air, gasTank.Air, canister.ReleasePressure);
				}
				else
				{
					GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
					this._atmosphereSystem.ReleaseGasTo(canister.Air, environment, canister.ReleasePressure);
				}
			}
			if (MathHelper.CloseToPercent(canister.Air.Pressure, canister.LastPressure, 1E-05))
			{
				return;
			}
			this.DirtyUI(uid, canister, nodeContainer, containerManager);
			canister.LastPressure = canister.Air.Pressure;
			if (canister.Air.Pressure < 10f)
			{
				this._appearanceSystem.SetData(uid, GasCanisterVisuals.PressureState, 0, appearance);
				return;
			}
			if (canister.Air.Pressure < 101.325f)
			{
				this._appearanceSystem.SetData(uid, GasCanisterVisuals.PressureState, 1, appearance);
				return;
			}
			if (canister.Air.Pressure < 1519.875f)
			{
				this._appearanceSystem.SetData(uid, GasCanisterVisuals.PressureState, 2, appearance);
				return;
			}
			this._appearanceSystem.SetData(uid, GasCanisterVisuals.PressureState, 3, appearance);
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x000CE380 File Offset: 0x000CC580
		private void OnCanisterActivate(EntityUid uid, GasCanisterComponent component, ActivateInWorldEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			LockComponent lockComponent;
			if (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked)
			{
				this._popupSystem.PopupEntity(Loc.GetString("gas-canister-popup-denied"), uid, args.User, PopupType.Small);
				if (component.AccessDeniedSound != null)
				{
					this._audioSys.PlayPvs(component.AccessDeniedSound, uid, null);
				}
				return;
			}
			this._userInterfaceSystem.TryOpen(uid, GasCanisterUiKey.Key, actor.PlayerSession, null);
			args.Handled = true;
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000CE41C File Offset: 0x000CC61C
		private void OnCanisterInteractHand(EntityUid uid, GasCanisterComponent component, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			LockComponent lockComponent;
			if (base.TryComp<LockComponent>(uid, ref lockComponent) && lockComponent.Locked)
			{
				return;
			}
			this._userInterfaceSystem.TryOpen(uid, GasCanisterUiKey.Key, actor.PlayerSession, null);
			args.Handled = true;
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000CE474 File Offset: 0x000CC674
		private void OnCanisterInteractUsing(EntityUid canister, GasCanisterComponent component, InteractUsingEvent args)
		{
			ContainerSlot container = this._containerSystem.EnsureContainer<ContainerSlot>(canister, component.ContainerName, null);
			if (container.ContainedEntity != null)
			{
				return;
			}
			GasTankComponent gasTankComponent;
			if (!this.EntityManager.TryGetComponent<GasTankComponent>(args.Used, ref gasTankComponent))
			{
				return;
			}
			if (!this._handsSystem.TryDropIntoContainer(args.User, args.Used, container, true, null))
			{
				return;
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterTankInserted;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(28, 3);
			logStringHandler.AppendLiteral("Player ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.User), "player", "ToPrettyString(args.User)");
			logStringHandler.AppendLiteral(" inserted tank ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(container.ContainedEntities[0]), "tank", "ToPrettyString(container.ContainedEntities[0])");
			logStringHandler.AppendLiteral(" into ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(canister), "canister", "ToPrettyString(canister)");
			adminLogger.Add(type, impact, ref logStringHandler);
			args.Handled = true;
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x000CE576 File Offset: 0x000CC776
		private void OnCanisterContainerInserted(EntityUid uid, GasCanisterComponent component, EntInsertedIntoContainerMessage args)
		{
			if (args.Container.ID != component.ContainerName)
			{
				return;
			}
			this.DirtyUI(uid, component, null, null);
			this._appearanceSystem.SetData(uid, GasCanisterVisuals.TankInserted, true, null);
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x000CE5B4 File Offset: 0x000CC7B4
		private void OnCanisterContainerRemoved(EntityUid uid, GasCanisterComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID != component.ContainerName)
			{
				return;
			}
			this.DirtyUI(uid, component, null, null);
			this._appearanceSystem.SetData(uid, GasCanisterVisuals.TankInserted, false, null);
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x000CE5F4 File Offset: 0x000CC7F4
		public void MixContainerWithPipeNet(GasMixture containerAir, GasMixture pipeNetAir)
		{
			GasMixture buffer = new GasMixture(pipeNetAir.Volume + containerAir.Volume);
			this._atmosphereSystem.Merge(buffer, pipeNetAir);
			this._atmosphereSystem.Merge(buffer, containerAir);
			pipeNetAir.Clear();
			this._atmosphereSystem.Merge(pipeNetAir, buffer);
			pipeNetAir.Multiply(pipeNetAir.Volume / buffer.Volume);
			containerAir.Clear();
			this._atmosphereSystem.Merge(containerAir, buffer);
			containerAir.Multiply(containerAir.Volume / buffer.Volume);
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x000CE67A File Offset: 0x000CC87A
		private void CalculateCanisterPrice(EntityUid uid, GasCanisterComponent component, ref PriceCalculationEvent args)
		{
			args.Price += this._atmosphereSystem.GetPrice(component.Air);
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x000CE697 File Offset: 0x000CC897
		private void OnAnalyzed(EntityUid uid, GasCanisterComponent component, GasAnalyzerScanEvent args)
		{
			args.GasMixtures = new Dictionary<string, GasMixture>
			{
				{
					base.Name(uid, null),
					component.Air
				}
			};
		}

		// Token: 0x0600272B RID: 10027 RVA: 0x000CE6B8 File Offset: 0x000CC8B8
		private void OnLockToggled(EntityUid uid, GasCanisterComponent component, ref LockToggledEvent args)
		{
			this._appearanceSystem.SetData(uid, GasCanisterVisuals.Locked, args.Locked, null);
		}

		// Token: 0x0400185C RID: 6236
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x0400185D RID: 6237
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400185E RID: 6238
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400185F RID: 6239
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04001860 RID: 6240
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04001861 RID: 6241
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001862 RID: 6242
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x04001863 RID: 6243
		[Dependency]
		private readonly SharedAudioSystem _audioSys;

		// Token: 0x04001864 RID: 6244
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04001865 RID: 6245
		private readonly int _plasmaThreshold = 1000;
	}
}
