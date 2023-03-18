using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Radio.Components;
using Content.Server.Radio.EntitySystems;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Salvage;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Salvage
{
	// Token: 0x02000222 RID: 546
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SalvageSystem : EntitySystem
	{
		// Token: 0x06000ACA RID: 2762 RVA: 0x0003850C File Offset: 0x0003670C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SalvageMagnetComponent, InteractHandEvent>(new ComponentEventHandler<SalvageMagnetComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<SalvageMagnetComponent, ExaminedEvent>(new ComponentEventHandler<SalvageMagnetComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<SalvageMagnetComponent, ComponentShutdown>(new ComponentEventHandler<SalvageMagnetComponent, ComponentShutdown>(this.OnMagnetRemoval), null, null);
			base.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoval), null, null);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRoundEnd), null, null);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x00038583 File Offset: 0x00036783
		private void OnRoundEnd(GameRunLevelChangedEvent ev)
		{
			if (ev.New != GameRunLevel.InRound)
			{
				this._salvageGridStates.Clear();
			}
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0003859C File Offset: 0x0003679C
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, SalvageMagnetComponent component = null)
		{
			if (!base.Resolve<SalvageMagnetComponent>(uid, ref component, false))
			{
				return;
			}
			this._appearanceSystem.SetData(uid, SalvageMagnetVisuals.ReadyBlinking, component.MagnetState.StateType == MagnetStateType.Attaching, null);
			this._appearanceSystem.SetData(uid, SalvageMagnetVisuals.Ready, component.MagnetState.StateType == MagnetStateType.Holding, null);
			this._appearanceSystem.SetData(uid, SalvageMagnetVisuals.Unready, component.MagnetState.StateType == MagnetStateType.CoolingDown, null);
			this._appearanceSystem.SetData(uid, SalvageMagnetVisuals.UnreadyBlinking, component.MagnetState.StateType == MagnetStateType.Detaching, null);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x00038650 File Offset: 0x00036850
		[NullableContext(2)]
		private void UpdateChargeStateAppearance(EntityUid uid, TimeSpan currentTime, SalvageMagnetComponent component = null)
		{
			if (!base.Resolve<SalvageMagnetComponent>(uid, ref component, false))
			{
				return;
			}
			int timeLeft = Convert.ToInt32(component.MagnetState.Until.TotalSeconds - currentTime.TotalSeconds);
			if (component.MagnetState.StateType == MagnetStateType.Inactive)
			{
				component.ChargeRemaining = 5;
			}
			else if (component.MagnetState.StateType == MagnetStateType.Holding)
			{
				component.ChargeRemaining = timeLeft / (Convert.ToInt32(component.HoldTime.TotalSeconds) / component.ChargeCapacity) + 1;
			}
			else if (component.MagnetState.StateType == MagnetStateType.Detaching)
			{
				component.ChargeRemaining = 0;
			}
			else if (component.MagnetState.StateType == MagnetStateType.CoolingDown)
			{
				component.ChargeRemaining = component.ChargeCapacity - timeLeft / (Convert.ToInt32(component.CooldownTime.TotalSeconds) / component.ChargeCapacity) - 1;
			}
			if (component.PreviousCharge != component.ChargeRemaining)
			{
				this._appearanceSystem.SetData(uid, SalvageMagnetVisuals.ChargeState, component.ChargeRemaining, null);
				component.PreviousCharge = component.ChargeRemaining;
			}
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x00038758 File Offset: 0x00036958
		private void OnGridRemoval(GridRemovalEvent ev)
		{
			SalvageGridComponent salvComp;
			if (this._salvageGridStates.Remove(ev.EntityUid) && this.EntityManager.TryGetComponent<SalvageGridComponent>(ev.EntityUid, ref salvComp) && salvComp.SpawnerMagnet != null)
			{
				this.Report(salvComp.SpawnerMagnet.Owner, salvComp.SpawnerMagnet.SalvageChannel, "salvage-system-announcement-spawn-magnet-lost", Array.Empty<ValueTuple<string, object>>());
			}
			foreach (KeyValuePair<EntityUid, SalvageGridState> gridState in this._salvageGridStates)
			{
				foreach (SalvageMagnetComponent magnet in gridState.Value.ActiveMagnets)
				{
					EntityUid? attachedEntity = magnet.AttachedEntity;
					EntityUid entityUid = ev.EntityUid;
					if (attachedEntity != null && (attachedEntity == null || attachedEntity.GetValueOrDefault() == entityUid))
					{
						magnet.AttachedEntity = null;
						magnet.MagnetState = MagnetState.Inactive;
						return;
					}
				}
			}
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x00038898 File Offset: 0x00036A98
		private void OnMagnetRemoval(EntityUid uid, SalvageMagnetComponent component, ComponentShutdown args)
		{
			if (component.MagnetState.StateType == MagnetStateType.Inactive)
			{
				return;
			}
			EntityUid? gridUid = this.EntityManager.GetComponent<TransformComponent>(component.Owner).GridUid;
			if (gridUid != null)
			{
				EntityUid gridId = gridUid.GetValueOrDefault();
				SalvageGridState salvageGridState;
				if (this._salvageGridStates.TryGetValue(gridId, out salvageGridState))
				{
					salvageGridState.ActiveMagnets.Remove(component);
					this.Report(uid, component.SalvageChannel, "salvage-system-announcement-spawn-magnet-lost", Array.Empty<ValueTuple<string, object>>());
					if (component.AttachedEntity != null)
					{
						this.SafeDeleteSalvage(component.AttachedEntity.Value);
						component.AttachedEntity = null;
						this.Report(uid, component.SalvageChannel, "salvage-system-announcement-lost", Array.Empty<ValueTuple<string, object>>());
					}
					else
					{
						MagnetState magnetState = component.MagnetState;
						if (magnetState.StateType == MagnetStateType.Attaching)
						{
							this.Report(uid, component.SalvageChannel, "salvage-system-announcement-spawn-no-debris-available", Array.Empty<ValueTuple<string, object>>());
						}
					}
					component.MagnetState = MagnetState.Inactive;
					return;
				}
			}
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x00038988 File Offset: 0x00036B88
		private void OnExamined(EntityUid uid, SalvageMagnetComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			switch (component.MagnetState.StateType)
			{
			case MagnetStateType.Inactive:
				args.PushMarkup(Loc.GetString("salvage-system-magnet-examined-inactive"));
				return;
			case MagnetStateType.Attaching:
				args.PushMarkup(Loc.GetString("salvage-system-magnet-examined-pulling-in"));
				return;
			case MagnetStateType.Holding:
			{
				EntityUid? gridUid = this.EntityManager.GetComponent<TransformComponent>(component.Owner).GridUid;
				if (gridUid != null)
				{
					EntityUid gridId = gridUid.GetValueOrDefault();
					SalvageGridState salvageGridState;
					if (this._salvageGridStates.TryGetValue(gridId, out salvageGridState))
					{
						args.PushMarkup(Loc.GetString("salvage-system-magnet-examined-active", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("timeLeft", Math.Ceiling((component.MagnetState.Until - salvageGridState.CurrentTime).TotalSeconds))
						}));
						return;
					}
				}
				Logger.WarningS("salvage", "Failed to load salvage grid state, can't display remaining time");
				return;
			}
			case MagnetStateType.Detaching:
				args.PushMarkup(Loc.GetString("salvage-system-magnet-examined-releasing"));
				return;
			case MagnetStateType.CoolingDown:
				args.PushMarkup(Loc.GetString("salvage-system-magnet-examined-cooling-down"));
				return;
			default:
				throw new NotImplementedException("Unexpected magnet state type");
			}
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x00038AB0 File Offset: 0x00036CB0
		private void OnInteractHand(EntityUid uid, SalvageMagnetComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.StartMagnet(component, args.User);
			this.UpdateAppearance(uid, component);
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x00038AD8 File Offset: 0x00036CD8
		private void StartMagnet(SalvageMagnetComponent component, EntityUid user)
		{
			switch (component.MagnetState.StateType)
			{
			case MagnetStateType.Inactive:
			{
				this.ShowPopup("salvage-system-report-activate-success", component, user);
				EntityUid? gridUid = this.EntityManager.GetComponent<TransformComponent>(component.Owner).GridUid;
				if (gridUid == null)
				{
					throw new InvalidOperationException("Magnet had no grid associated");
				}
				EntityUid gridId = gridUid.GetValueOrDefault();
				SalvageGridState gridState;
				if (!this._salvageGridStates.TryGetValue(gridId, out gridState))
				{
					gridState = new SalvageGridState();
					this._salvageGridStates[gridId] = gridState;
				}
				gridState.ActiveMagnets.Add(component);
				component.MagnetState = new MagnetState(MagnetStateType.Attaching, gridState.CurrentTime + component.AttachingTime);
				base.RaiseLocalEvent<SalvageMagnetActivatedEvent>(new SalvageMagnetActivatedEvent(component.Owner));
				this.Report(component.Owner, component.SalvageChannel, "salvage-system-report-activate-success", Array.Empty<ValueTuple<string, object>>());
				return;
			}
			case MagnetStateType.Attaching:
			case MagnetStateType.Holding:
				this.ShowPopup("salvage-system-report-already-active", component, user);
				return;
			case MagnetStateType.Detaching:
			case MagnetStateType.CoolingDown:
				this.ShowPopup("salvage-system-report-cooling-down", component, user);
				return;
			default:
				throw new NotImplementedException("Unexpected magnet state type");
			}
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00038BF0 File Offset: 0x00036DF0
		private void ShowPopup(string messageKey, SalvageMagnetComponent component, EntityUid user)
		{
			this._popupSystem.PopupEntity(Loc.GetString(messageKey), component.Owner, user, PopupType.Small);
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x00038C0C File Offset: 0x00036E0C
		private void SafeDeleteSalvage(EntityUid salvage)
		{
			TransformComponent salvageTransform;
			if (!this.EntityManager.TryGetComponent<TransformComponent>(salvage, ref salvageTransform))
			{
				Logger.ErrorS("salvage", "Salvage entity was missing transform component");
				return;
			}
			if (salvageTransform.GridUid == null)
			{
				Logger.ErrorS("salvage", "Salvage entity has no associated grid?");
				return;
			}
			foreach (ICommonSession player in Filter.Empty().AddInGrid(salvageTransform.GridUid.Value, this.EntityManager).Recipients)
			{
				if (player.AttachedEntity != null)
				{
					EntityUid playerEntityUid = player.AttachedEntity.Value;
					if (!base.HasComp<SalvageMobRestrictionsComponent>(playerEntityUid))
					{
						base.Transform(playerEntityUid).AttachParent(salvageTransform.ParentUid);
					}
				}
			}
			this.EntityManager.DeleteEntity(salvage);
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x00038CF8 File Offset: 0x00036EF8
		private void TryGetSalvagePlacementLocation(SalvageMagnetComponent component, out MapCoordinates coords, out Angle angle)
		{
			coords = MapCoordinates.Nullspace;
			angle = Angle.Zero;
			TransformComponent tsc = base.Transform(component.Owner);
			coords = new EntityCoordinates(component.Owner, component.Offset).ToMap(this.EntityManager);
			MapGridComponent magnetGrid;
			TransformComponent gridXform;
			if (this._mapManager.TryGetGrid(tsc.GridUid, ref magnetGrid) && base.TryComp<TransformComponent>(magnetGrid.Owner, ref gridXform))
			{
				angle = gridXform.WorldRotation;
			}
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x00038D7E File Offset: 0x00036F7E
		private IEnumerable<SalvageMapPrototype> GetAllSalvageMaps()
		{
			return this._prototypeManager.EnumeratePrototypes<SalvageMapPrototype>();
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x00038D8C File Offset: 0x00036F8C
		private bool SpawnSalvage(SalvageMagnetComponent component)
		{
			MapCoordinates spl;
			Angle spAngle;
			this.TryGetSalvagePlacementLocation(component, out spl, out spAngle);
			string forcedSalvage = this._configurationManager.GetCVar<string>(CCVars.SalvageForced);
			List<SalvageMapPrototype> allSalvageMaps;
			if (string.IsNullOrWhiteSpace(forcedSalvage))
			{
				allSalvageMaps = this.GetAllSalvageMaps().ToList<SalvageMapPrototype>();
			}
			else
			{
				allSalvageMaps = new List<SalvageMapPrototype>();
				SalvageMapPrototype forcedMap;
				if (this._prototypeManager.TryIndex<SalvageMapPrototype>(forcedSalvage, ref forcedMap))
				{
					allSalvageMaps.Add(forcedMap);
				}
				else
				{
					Logger.ErrorS("c.s.salvage", "Unable to get forced salvage map prototype " + forcedSalvage);
				}
			}
			SalvageMapPrototype map = null;
			Vector2 spawnLocation = Vector2.Zero;
			for (int i = 0; i < allSalvageMaps.Count; i++)
			{
				SalvageMapPrototype attemptedMap = RandomExtensions.PickAndTake<SalvageMapPrototype>(this._random, allSalvageMaps);
				for (int attempt = 0; attempt < SalvageSystem.SalvageLocationPlaceAttempts; attempt++)
				{
					float randomRadius = this._random.NextFloat(component.OffsetRadiusMin, component.OffsetRadiusMax);
					Vector2 randomOffset = this._random.NextAngle().ToWorldVec() * randomRadius;
					spawnLocation = spl.Position + randomOffset;
					Box2 box2 = Box2.CenteredAround(spawnLocation + attemptedMap.Bounds.Center, attemptedMap.Bounds.Size);
					Box2Rotated box2rot;
					box2rot..ctor(box2, spAngle, spawnLocation);
					if (!this._mapManager.FindGridsIntersecting(spl.MapId, box2rot, false).Any<MapGridComponent>())
					{
						map = attemptedMap;
						break;
					}
				}
				if (map != null)
				{
					break;
				}
			}
			if (map == null)
			{
				this.Report(component.Owner, component.SalvageChannel, "salvage-system-announcement-spawn-no-debris-available", Array.Empty<ValueTuple<string, object>>());
				return false;
			}
			MapLoadOptions opts = new MapLoadOptions
			{
				Offset = spawnLocation
			};
			EntityUid? salvageEntityId = this._map.LoadGrid(spl.MapId, map.MapPath.ToString(), opts);
			if (salvageEntityId == null)
			{
				this.Report(component.Owner, component.SalvageChannel, "salvage-system-announcement-spawn-debris-disintegrated", Array.Empty<ValueTuple<string, object>>());
				return false;
			}
			component.AttachedEntity = salvageEntityId;
			this.EntityManager.EnsureComponent<SalvageGridComponent>(salvageEntityId.Value).SpawnerMagnet = component;
			this.EntityManager.GetComponent<TransformComponent>(salvageEntityId.Value).WorldRotation = spAngle;
			this.Report(component.Owner, component.SalvageChannel, "salvage-system-announcement-arrived", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("timeLeft", component.HoldTime.TotalSeconds)
			});
			return true;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x00038FE4 File Offset: 0x000371E4
		private void Report(EntityUid source, string channelName, string messageKey, [Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] params ValueTuple<string, object>[] args)
		{
			IntrinsicRadioReceiverComponent radio;
			if (!base.TryComp<IntrinsicRadioReceiverComponent>(source, ref radio))
			{
				return;
			}
			string message = (args.Length == 0) ? Loc.GetString(messageKey) : Loc.GetString(messageKey, args);
			RadioChannelPrototype channel = this._prototypeManager.Index<RadioChannelPrototype>(channelName);
			this._radioSystem.SendRadioMessage(source, message, channel, null);
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x00039038 File Offset: 0x00037238
		private void Transition(SalvageMagnetComponent magnet, TimeSpan currentTime)
		{
			switch (magnet.MagnetState.StateType)
			{
			case MagnetStateType.Attaching:
				if (this.SpawnSalvage(magnet))
				{
					magnet.MagnetState = new MagnetState(MagnetStateType.Holding, currentTime + magnet.HoldTime);
				}
				else
				{
					magnet.MagnetState = new MagnetState(MagnetStateType.CoolingDown, currentTime + magnet.CooldownTime);
				}
				break;
			case MagnetStateType.Holding:
				this.Report(magnet.Owner, magnet.SalvageChannel, "salvage-system-announcement-losing", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("timeLeft", magnet.DetachingTime.TotalSeconds)
				});
				magnet.MagnetState = new MagnetState(MagnetStateType.Detaching, currentTime + magnet.DetachingTime);
				break;
			case MagnetStateType.Detaching:
				if (magnet.AttachedEntity != null)
				{
					this.SafeDeleteSalvage(magnet.AttachedEntity.Value);
				}
				else
				{
					Logger.ErrorS("salvage", "Salvage detaching was expecting attached entity but it was null");
				}
				this.Report(magnet.Owner, magnet.SalvageChannel, "salvage-system-announcement-lost", Array.Empty<ValueTuple<string, object>>());
				magnet.MagnetState = new MagnetState(MagnetStateType.CoolingDown, currentTime + magnet.CooldownTime);
				break;
			case MagnetStateType.CoolingDown:
				magnet.MagnetState = MagnetState.Inactive;
				break;
			}
			this.UpdateAppearance(magnet.Owner, magnet);
			this.UpdateChargeStateAppearance(magnet.Owner, currentTime, magnet);
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00039194 File Offset: 0x00037394
		public override void Update(float frameTime)
		{
			TimeSpan secondsPassed = TimeSpan.FromSeconds((double)frameTime);
			foreach (KeyValuePair<EntityUid, SalvageGridState> gridIdAndState in this._salvageGridStates)
			{
				SalvageGridState state = gridIdAndState.Value;
				if (state.ActiveMagnets.Count != 0)
				{
					EntityUid gridId = gridIdAndState.Key;
					if (!base.MetaData(gridId).EntityPaused)
					{
						state.CurrentTime += secondsPassed;
						RemQueue<SalvageMagnetComponent> deleteQueue = default(RemQueue<SalvageMagnetComponent>);
						foreach (SalvageMagnetComponent magnet in state.ActiveMagnets)
						{
							this.UpdateChargeStateAppearance(magnet.Owner, state.CurrentTime, magnet);
							if (!(magnet.MagnetState.Until > state.CurrentTime))
							{
								this.Transition(magnet, state.CurrentTime);
								if (magnet.MagnetState.StateType == MagnetStateType.Inactive)
								{
									deleteQueue.Add(magnet);
								}
							}
						}
						foreach (SalvageMagnetComponent magnet2 in deleteQueue)
						{
							state.ActiveMagnets.Remove(magnet2);
						}
					}
				}
			}
		}

		// Token: 0x040006A5 RID: 1701
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040006A6 RID: 1702
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040006A7 RID: 1703
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040006A8 RID: 1704
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040006A9 RID: 1705
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x040006AA RID: 1706
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040006AB RID: 1707
		[Dependency]
		private readonly RadioSystem _radioSystem;

		// Token: 0x040006AC RID: 1708
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x040006AD RID: 1709
		private static readonly int SalvageLocationPlaceAttempts = 16;

		// Token: 0x040006AE RID: 1710
		private readonly Dictionary<EntityUid, SalvageGridState> _salvageGridStates = new Dictionary<EntityUid, SalvageGridState>();
	}
}
