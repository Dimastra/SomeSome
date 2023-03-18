using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000790 RID: 1936
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosDebugOverlaySystem : SharedAtmosDebugOverlaySystem
	{
		// Token: 0x06002936 RID: 10550 RVA: 0x000D6921 File Offset: 0x000D4B21
		public override void Initialize()
		{
			base.Initialize();
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
		}

		// Token: 0x06002937 RID: 10551 RVA: 0x000D6940 File Offset: 0x000D4B40
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x000D695F File Offset: 0x000D4B5F
		public bool AddObserver(IPlayerSession observer)
		{
			return this._playerObservers.Add(observer);
		}

		// Token: 0x06002939 RID: 10553 RVA: 0x000D696D File Offset: 0x000D4B6D
		public bool HasObserver(IPlayerSession observer)
		{
			return this._playerObservers.Contains(observer);
		}

		// Token: 0x0600293A RID: 10554 RVA: 0x000D697C File Offset: 0x000D4B7C
		public bool RemoveObserver(IPlayerSession observer)
		{
			if (!this._playerObservers.Remove(observer))
			{
				return false;
			}
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage message = new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayDisableMessage();
			base.RaiseNetworkEvent(message, observer.ConnectedClient);
			return true;
		}

		// Token: 0x0600293B RID: 10555 RVA: 0x000D69AD File Offset: 0x000D4BAD
		public bool ToggleObserver(IPlayerSession observer)
		{
			if (this.HasObserver(observer))
			{
				this.RemoveObserver(observer);
				return false;
			}
			this.AddObserver(observer);
			return true;
		}

		// Token: 0x0600293C RID: 10556 RVA: 0x000D69CB File Offset: 0x000D4BCB
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus != 3)
			{
				this.RemoveObserver(e.Session);
			}
		}

		// Token: 0x0600293D RID: 10557 RVA: 0x000D69E4 File Offset: 0x000D4BE4
		[NullableContext(2)]
		private SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData ConvertTileToData(TileAtmosphere tile, bool mapIsSpace)
		{
			float[] gases = new float[Atmospherics.AdjustedNumberOfGases];
			if (((tile != null) ? tile.Air : null) == null)
			{
				return new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData(2.7f, gases, AtmosDirection.Invalid, (tile != null) ? tile.LastPressureDirection : AtmosDirection.Invalid, 0, (tile != null) ? tile.BlockedAirflow : AtmosDirection.Invalid, (tile != null) ? tile.Space : mapIsSpace);
			}
			NumericsHelpers.Add(gases, tile.Air.Moles);
			float temperature = tile.Air.Temperature;
			float[] moles = gases;
			AtmosDirection pressureDirection = tile.PressureDirection;
			AtmosDirection lastPressureDirection = tile.LastPressureDirection;
			ExcitedGroup excitedGroup = tile.ExcitedGroup;
			return new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData(temperature, moles, pressureDirection, lastPressureDirection, (excitedGroup != null) ? excitedGroup.GetHashCode() : 0, tile.BlockedAirflow, tile.Space);
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x000D6A94 File Offset: 0x000D4C94
		public override void Update(float frameTime)
		{
			this.AccumulatedFrameTime += frameTime;
			this._updateCooldown = 1f / this._configManager.GetCVar<float>(CCVars.NetAtmosDebugOverlayTickRate);
			if (this.AccumulatedFrameTime < this._updateCooldown)
			{
				return;
			}
			this.AccumulatedFrameTime -= this._updateCooldown;
			foreach (IPlayerSession session in this._playerObservers)
			{
				EntityUid? attachedEntity = session.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid entity = attachedEntity.GetValueOrDefault();
					if (entity.Valid)
					{
						TransformComponent transform = this.EntityManager.GetComponent<TransformComponent>(entity);
						EntityUid? mapUid = transform.MapUid;
						bool mapIsSpace = this._atmosphereSystem.IsTileSpace(null, mapUid, Vector2i.Zero, null);
						Box2 worldBounds = Box2.CenteredAround(transform.WorldPosition, new Vector2(16f, 16f));
						foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(transform.MapID, worldBounds, false))
						{
							EntityUid uid = grid.Owner;
							GridAtmosphereComponent gridAtmos;
							if (base.Exists(uid) && base.TryComp<GridAtmosphereComponent>(uid, ref gridAtmos))
							{
								Vector2i entityTile = grid.GetTileRef(transform.Coordinates).GridIndices;
								Vector2i baseTile;
								baseTile..ctor(entityTile.X - 8, entityTile.Y - 8);
								SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData[] debugOverlayContent = new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData[256];
								int index = 0;
								for (int y = 0; y < 16; y++)
								{
									for (int x = 0; x < 16; x++)
									{
										Vector2i vector;
										vector..ctor(baseTile.X + x, baseTile.Y + y);
										TileAtmosphere tile;
										debugOverlayContent[index++] = this.ConvertTileToData(gridAtmos.Tiles.TryGetValue(vector, out tile) ? tile : null, mapIsSpace);
									}
								}
								base.RaiseNetworkEvent(new SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage(grid.Owner, baseTile, debugOverlayContent), session.ConnectedClient);
							}
						}
					}
				}
			}
		}

		// Token: 0x0400199A RID: 6554
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400199B RID: 6555
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x0400199C RID: 6556
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x0400199D RID: 6557
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400199E RID: 6558
		private readonly HashSet<IPlayerSession> _playerObservers = new HashSet<IPlayerSession>();

		// Token: 0x0400199F RID: 6559
		private float _updateCooldown;
	}
}
