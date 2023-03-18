using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Reactions;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Doors.Components;
using Content.Server.Doors.Systems;
using Content.Server.Maps;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000794 RID: 1940
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosphereSystem : SharedAtmosphereSystem
	{
		// Token: 0x06002944 RID: 10564 RVA: 0x000D6D98 File Offset: 0x000D4F98
		[NullableContext(2)]
		public GasMixture GetContainingMixture(EntityUid uid, bool ignoreExposed = false, bool excite = false, TransformComponent transform = null)
		{
			if (!ignoreExposed)
			{
				AtmosExposedGetAirEvent ev = new AtmosExposedGetAirEvent(uid, excite);
				base.RaiseLocalEvent<AtmosExposedGetAirEvent>(uid, ref ev, false);
				if (ev.Handled)
				{
					return ev.Gas;
				}
				if (!base.Resolve<TransformComponent>(uid, ref transform, true) || !transform.ParentUid.IsValid() || transform.MapUid == null)
				{
					return this.GetTileMixture(null, null, Vector2i.Zero, excite);
				}
				base.RaiseLocalEvent<AtmosExposedGetAirEvent>(transform.ParentUid, ref ev, false);
				if (ev.Handled)
				{
					return ev.Gas;
				}
			}
			else if (!base.Resolve<TransformComponent>(uid, ref transform, true))
			{
				return this.GetTileMixture(null, null, Vector2i.Zero, excite);
			}
			EntityUid? gridUid = transform.GridUid;
			EntityUid? mapUid = transform.MapUid;
			Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
			return this.GetTileMixture(gridUid, mapUid, position, excite);
		}

		// Token: 0x06002945 RID: 10565 RVA: 0x000D6E94 File Offset: 0x000D5094
		public bool HasAtmosphere(EntityUid gridUid)
		{
			AtmosphereSystem.HasAtmosphereMethodEvent ev = new AtmosphereSystem.HasAtmosphereMethodEvent(gridUid, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.HasAtmosphereMethodEvent>(gridUid, ref ev, false);
			return ev.Result;
		}

		// Token: 0x06002946 RID: 10566 RVA: 0x000D6EBC File Offset: 0x000D50BC
		public bool SetSimulatedGrid(EntityUid gridUid, bool simulated)
		{
			AtmosphereSystem.SetSimulatedGridMethodEvent ev = new AtmosphereSystem.SetSimulatedGridMethodEvent(gridUid, simulated, false);
			base.RaiseLocalEvent<AtmosphereSystem.SetSimulatedGridMethodEvent>(gridUid, ref ev, false);
			return ev.Handled;
		}

		// Token: 0x06002947 RID: 10567 RVA: 0x000D6EE4 File Offset: 0x000D50E4
		public bool IsSimulatedGrid(EntityUid gridUid)
		{
			AtmosphereSystem.IsSimulatedGridMethodEvent ev = new AtmosphereSystem.IsSimulatedGridMethodEvent(gridUid, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.IsSimulatedGridMethodEvent>(gridUid, ref ev, false);
			return ev.Simulated;
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000D6F0C File Offset: 0x000D510C
		public IEnumerable<GasMixture> GetAllMixtures(EntityUid gridUid, bool excite = false)
		{
			AtmosphereSystem.GetAllMixturesMethodEvent ev = new AtmosphereSystem.GetAllMixturesMethodEvent(gridUid, excite, null, false);
			base.RaiseLocalEvent<AtmosphereSystem.GetAllMixturesMethodEvent>(gridUid, ref ev, false);
			if (!ev.Handled)
			{
				return Enumerable.Empty<GasMixture>();
			}
			return ev.Mixtures;
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x000D6F44 File Offset: 0x000D5144
		public void InvalidateTile(EntityUid gridUid, Vector2i tile)
		{
			AtmosphereSystem.InvalidateTileMethodEvent ev = new AtmosphereSystem.InvalidateTileMethodEvent(gridUid, tile, false);
			base.RaiseLocalEvent<AtmosphereSystem.InvalidateTileMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x0600294A RID: 10570 RVA: 0x000D6F68 File Offset: 0x000D5168
		[return: Nullable(2)]
		public GasMixture[] GetTileMixtures(EntityUid? gridUid, EntityUid? mapUid, List<Vector2i> tiles, bool excite = false)
		{
			AtmosphereSystem.GetTileMixturesMethodEvent ev = new AtmosphereSystem.GetTileMixturesMethodEvent(gridUid, mapUid, tiles, excite, null, false);
			if (gridUid != null)
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixturesMethodEvent>(gridUid.Value, ref ev, false);
			}
			if (ev.Handled)
			{
				return ev.Mixtures;
			}
			if (mapUid != null)
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixturesMethodEvent>(mapUid.Value, ref ev, true);
			}
			else
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixturesMethodEvent>(ref ev);
			}
			if (ev.Handled)
			{
				return ev.Mixtures;
			}
			ref AtmosphereSystem.GetTileMixturesMethodEvent ptr = ref ev;
			if (ptr.Mixtures == null)
			{
				ptr.Mixtures = new GasMixture[tiles.Count];
			}
			for (int i = 0; i < tiles.Count; i++)
			{
				ref GasMixture ptr2 = ref ev.Mixtures[i];
				if (ptr2 == null)
				{
					ptr2 = GasMixture.SpaceGas;
				}
			}
			return ev.Mixtures;
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x000D7034 File Offset: 0x000D5234
		[NullableContext(2)]
		public GasMixture GetTileMixture(EntityUid? gridUid, EntityUid? mapUid, Vector2i tile, bool excite = false)
		{
			AtmosphereSystem.GetTileMixtureMethodEvent ev = new AtmosphereSystem.GetTileMixtureMethodEvent(gridUid, mapUid, tile, excite, null, false);
			if (gridUid != null)
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixtureMethodEvent>(gridUid.Value, ref ev, false);
			}
			if (ev.Handled)
			{
				return ev.Mixture;
			}
			if (mapUid != null)
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixtureMethodEvent>(mapUid.Value, ref ev, true);
			}
			else
			{
				base.RaiseLocalEvent<AtmosphereSystem.GetTileMixtureMethodEvent>(ref ev);
			}
			return ev.Mixture ?? GasMixture.SpaceGas;
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x000D70AC File Offset: 0x000D52AC
		public ReactionResult ReactTile(EntityUid gridId, Vector2i tile)
		{
			AtmosphereSystem.ReactTileMethodEvent ev = new AtmosphereSystem.ReactTileMethodEvent(gridId, tile, ReactionResult.NoReaction, false);
			base.RaiseLocalEvent<AtmosphereSystem.ReactTileMethodEvent>(gridId, ref ev, false);
			ev.Handled = true;
			return ev.Result;
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x000D70E0 File Offset: 0x000D52E0
		[NullableContext(2)]
		public bool IsTileAirBlocked(EntityUid gridUid, Vector2i tile, AtmosDirection directions = AtmosDirection.All, MapGridComponent mapGridComp = null)
		{
			AtmosphereSystem.IsTileAirBlockedMethodEvent ev = new AtmosphereSystem.IsTileAirBlockedMethodEvent(gridUid, tile, directions, mapGridComp, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.IsTileAirBlockedMethodEvent>(gridUid, ref ev, false);
			return ev.Result;
		}

		// Token: 0x0600294E RID: 10574 RVA: 0x000D710C File Offset: 0x000D530C
		[NullableContext(2)]
		public bool IsTileSpace(EntityUid? gridUid, EntityUid? mapUid, Vector2i tile, MapGridComponent mapGridComp = null)
		{
			AtmosphereSystem.IsTileSpaceMethodEvent ev = new AtmosphereSystem.IsTileSpaceMethodEvent(gridUid, mapUid, tile, mapGridComp, true, false);
			if (gridUid != null)
			{
				base.RaiseLocalEvent<AtmosphereSystem.IsTileSpaceMethodEvent>(gridUid.Value, ref ev, false);
			}
			if (mapUid != null && !ev.Handled)
			{
				base.RaiseLocalEvent<AtmosphereSystem.IsTileSpaceMethodEvent>(mapUid.Value, ref ev, true);
			}
			else if (mapUid == null && !ev.Handled)
			{
				base.RaiseLocalEvent<AtmosphereSystem.IsTileSpaceMethodEvent>(ref ev);
			}
			return ev.Result;
		}

		// Token: 0x0600294F RID: 10575 RVA: 0x000D7185 File Offset: 0x000D5385
		public bool IsTileMixtureProbablySafe(EntityUid? gridUid, EntityUid mapUid, Vector2i tile)
		{
			return this.IsMixtureProbablySafe(this.GetTileMixture(gridUid, new EntityUid?(mapUid), tile, false));
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x000D719C File Offset: 0x000D539C
		public float GetTileHeatCapacity(EntityUid? gridUid, EntityUid mapUid, Vector2i tile)
		{
			return this.GetHeatCapacity(this.GetTileMixture(gridUid, new EntityUid?(mapUid), tile, false) ?? GasMixture.SpaceGas);
		}

		// Token: 0x06002951 RID: 10577 RVA: 0x000D71BC File Offset: 0x000D53BC
		public IEnumerable<Vector2i> GetAdjacentTiles(EntityUid gridUid, Vector2i tile)
		{
			AtmosphereSystem.GetAdjacentTilesMethodEvent ev = new AtmosphereSystem.GetAdjacentTilesMethodEvent(gridUid, tile, null, false);
			base.RaiseLocalEvent<AtmosphereSystem.GetAdjacentTilesMethodEvent>(gridUid, ref ev, false);
			return ev.Result ?? Enumerable.Empty<Vector2i>();
		}

		// Token: 0x06002952 RID: 10578 RVA: 0x000D71F0 File Offset: 0x000D53F0
		public IEnumerable<GasMixture> GetAdjacentTileMixtures(EntityUid gridUid, Vector2i tile, bool includeBlocked = false, bool excite = false)
		{
			AtmosphereSystem.GetAdjacentTileMixturesMethodEvent ev = new AtmosphereSystem.GetAdjacentTileMixturesMethodEvent(gridUid, tile, includeBlocked, excite, null, false);
			base.RaiseLocalEvent<AtmosphereSystem.GetAdjacentTileMixturesMethodEvent>(gridUid, ref ev, false);
			return ev.Result ?? Enumerable.Empty<GasMixture>();
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x000D7228 File Offset: 0x000D5428
		[NullableContext(2)]
		public void UpdateAdjacent(EntityUid gridUid, Vector2i tile, MapGridComponent mapGridComp = null)
		{
			AtmosphereSystem.UpdateAdjacentMethodEvent ev = new AtmosphereSystem.UpdateAdjacentMethodEvent(gridUid, tile, mapGridComp, false);
			base.RaiseLocalEvent<AtmosphereSystem.UpdateAdjacentMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x06002954 RID: 10580 RVA: 0x000D724C File Offset: 0x000D544C
		public void HotspotExpose(EntityUid gridUid, Vector2i tile, float exposedTemperature, float exposedVolume, bool soh = false)
		{
			AtmosphereSystem.HotspotExposeMethodEvent ev = new AtmosphereSystem.HotspotExposeMethodEvent(gridUid, tile, exposedTemperature, exposedVolume, soh, false);
			base.RaiseLocalEvent<AtmosphereSystem.HotspotExposeMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x06002955 RID: 10581 RVA: 0x000D7274 File Offset: 0x000D5474
		public void HotspotExtinguish(EntityUid gridUid, Vector2i tile)
		{
			AtmosphereSystem.HotspotExtinguishMethodEvent ev = new AtmosphereSystem.HotspotExtinguishMethodEvent(gridUid, tile, false);
			base.RaiseLocalEvent<AtmosphereSystem.HotspotExtinguishMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x06002956 RID: 10582 RVA: 0x000D7298 File Offset: 0x000D5498
		public bool IsHotspotActive(EntityUid gridUid, Vector2i tile)
		{
			AtmosphereSystem.IsHotspotActiveMethodEvent ev = new AtmosphereSystem.IsHotspotActiveMethodEvent(gridUid, tile, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.IsHotspotActiveMethodEvent>(gridUid, ref ev, false);
			return ev.Result;
		}

		// Token: 0x06002957 RID: 10583 RVA: 0x000D72C4 File Offset: 0x000D54C4
		public void FixTileVacuum(EntityUid gridUid, Vector2i tile)
		{
			AtmosphereSystem.FixTileVacuumMethodEvent ev = new AtmosphereSystem.FixTileVacuumMethodEvent(gridUid, tile, false);
			base.RaiseLocalEvent<AtmosphereSystem.FixTileVacuumMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x06002958 RID: 10584 RVA: 0x000D72E8 File Offset: 0x000D54E8
		public void AddPipeNet(EntityUid gridUid, PipeNet pipeNet)
		{
			AtmosphereSystem.AddPipeNetMethodEvent ev = new AtmosphereSystem.AddPipeNetMethodEvent(gridUid, pipeNet, false);
			base.RaiseLocalEvent<AtmosphereSystem.AddPipeNetMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x06002959 RID: 10585 RVA: 0x000D730C File Offset: 0x000D550C
		public void RemovePipeNet(EntityUid gridUid, PipeNet pipeNet)
		{
			AtmosphereSystem.RemovePipeNetMethodEvent ev = new AtmosphereSystem.RemovePipeNetMethodEvent(gridUid, pipeNet, false);
			base.RaiseLocalEvent<AtmosphereSystem.RemovePipeNetMethodEvent>(gridUid, ref ev, false);
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x000D7330 File Offset: 0x000D5530
		public bool AddAtmosDevice(EntityUid gridUid, AtmosDeviceComponent device)
		{
			AtmosphereSystem.AddAtmosDeviceMethodEvent ev = new AtmosphereSystem.AddAtmosDeviceMethodEvent(gridUid, device, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.AddAtmosDeviceMethodEvent>(gridUid, ref ev, false);
			return ev.Result;
		}

		// Token: 0x0600295B RID: 10587 RVA: 0x000D735C File Offset: 0x000D555C
		public bool RemoveAtmosDevice(EntityUid gridUid, AtmosDeviceComponent device)
		{
			AtmosphereSystem.RemoveAtmosDeviceMethodEvent ev = new AtmosphereSystem.RemoveAtmosDeviceMethodEvent(gridUid, device, false, false);
			base.RaiseLocalEvent<AtmosphereSystem.RemoveAtmosDeviceMethodEvent>(gridUid, ref ev, false);
			return ev.Result;
		}

		// Token: 0x0600295C RID: 10588 RVA: 0x000D7385 File Offset: 0x000D5585
		private void InitializeBreathTool()
		{
			base.SubscribeLocalEvent<BreathToolComponent, ComponentShutdown>(new ComponentEventHandler<BreathToolComponent, ComponentShutdown>(this.OnBreathToolShutdown), null, null);
		}

		// Token: 0x0600295D RID: 10589 RVA: 0x000D739B File Offset: 0x000D559B
		private void OnBreathToolShutdown(EntityUid uid, BreathToolComponent component, ComponentShutdown args)
		{
			this.DisconnectInternals(component);
		}

		// Token: 0x0600295E RID: 10590 RVA: 0x000D73A4 File Offset: 0x000D55A4
		public void DisconnectInternals(BreathToolComponent component)
		{
			EntityUid? old = component.ConnectedInternalsEntity;
			component.ConnectedInternalsEntity = null;
			InternalsComponent internalsComponent;
			if (base.TryComp<InternalsComponent>(old, ref internalsComponent))
			{
				this._internals.DisconnectBreathTool(internalsComponent);
			}
			component.IsFunctional = false;
		}

		// Token: 0x0600295F RID: 10591 RVA: 0x000D73E2 File Offset: 0x000D55E2
		private void InitializeCommands()
		{
			this._consoleHost.RegisterCommand("fixgridatmos", "Makes every tile on a grid have a roundstart gas mix.", "fixgridatmos <grid Ids>", new ConCommandCallback(this.FixGridAtmosCommand), new ConCommandCompletionCallback(this.FixGridAtmosCommandCompletions), false);
		}

		// Token: 0x06002960 RID: 10592 RVA: 0x000D7417 File Offset: 0x000D5617
		private void ShutdownCommands()
		{
			this._consoleHost.UnregisterCommand("fixgridatmos");
		}

		// Token: 0x06002961 RID: 10593 RVA: 0x000D742C File Offset: 0x000D562C
		[AdminCommand(AdminFlags.Debug)]
		private void FixGridAtmosCommand(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length == 0)
			{
				shell.WriteError("Not enough arguments.");
				return;
			}
			GasMixture[] mixtures = new GasMixture[7];
			for (int i = 0; i < mixtures.Length; i++)
			{
				mixtures[i] = new GasMixture(2500f)
				{
					Temperature = 293.15f
				};
			}
			mixtures[0].AdjustMoles(Gas.Oxygen, 21.824879f);
			mixtures[0].AdjustMoles(Gas.Nitrogen, 82.10312f);
			mixtures[2].AdjustMoles(Gas.Oxygen, 6666.982f);
			mixtures[3].AdjustMoles(Gas.Nitrogen, 6666.982f);
			mixtures[4].AdjustMoles(Gas.Plasma, 6666.982f);
			mixtures[5].AdjustMoles(Gas.Oxygen, 6666.982f);
			mixtures[5].AdjustMoles(Gas.Plasma, 6666.982f);
			mixtures[5].Temperature = 5000f;
			mixtures[6].AdjustMoles(Gas.Oxygen, 21.824879f);
			mixtures[6].AdjustMoles(Gas.Nitrogen, 82.10312f);
			mixtures[6].Temperature = 235f;
			foreach (string arg in args)
			{
				EntityUid euid;
				if (!EntityUid.TryParse(arg, ref euid))
				{
					shell.WriteError("Failed to parse euid '" + arg + "'.");
					return;
				}
				MapGridComponent gridComp;
				if (!base.TryComp<MapGridComponent>(euid, ref gridComp))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Euid '");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
					defaultInterpolatedStringHandler.AppendLiteral("' does not exist or is not a grid.");
					shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				GridAtmosphereComponent gridAtmosphere;
				if (!base.TryComp<GridAtmosphereComponent>(euid, ref gridAtmosphere))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Grid \"");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
					defaultInterpolatedStringHandler.AppendLiteral("\" has no atmosphere component, try addatmos.");
					shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					TransformComponent transform = base.Transform(euid);
					foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in gridAtmosphere.Tiles)
					{
						Vector2i vector2i;
						TileAtmosphere tileAtmosphere;
						keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
						Vector2i indices = vector2i;
						TileAtmosphere tileMain = tileAtmosphere;
						GasMixture tile = tileMain.Air;
						if (tile != null)
						{
							if (tile.Immutable && !this.IsTileSpace(new EntityUid?(euid), transform.MapUid, indices, gridComp))
							{
								tile = new GasMixture(tile.Volume)
								{
									Temperature = tile.Temperature
								};
								tileMain.Air = tile;
							}
							tile.Clear();
							int mixtureId = 0;
							foreach (EntityUid entUid in gridComp.GetAnchoredEntities(indices))
							{
								AtmosFixMarkerComponent afm;
								if (base.TryComp<AtmosFixMarkerComponent>(entUid, ref afm))
								{
									mixtureId = afm.Mode;
									break;
								}
							}
							GasMixture mixture = mixtures[mixtureId];
							this.Merge(tile, mixture);
							tile.Temperature = mixture.Temperature;
							gridAtmosphere.InvalidatedCoords.Add(indices);
						}
					}
				}
			}
		}

		// Token: 0x06002962 RID: 10594 RVA: 0x000D7740 File Offset: 0x000D5940
		private CompletionResult FixGridAtmosCommandCompletions(IConsoleShell shell, string[] args)
		{
			MapId? playerMap = null;
			ICommonSession player = shell.Player;
			if (player != null)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEnt = attachedEntity.GetValueOrDefault();
					playerMap = new MapId?(base.Transform(playerEnt).MapID);
				}
			}
			List<CompletionOption> options = new List<CompletionOption>();
			if (playerMap == null)
			{
				return CompletionResult.FromOptions(options);
			}
			foreach (MapGridComponent grid in from o in this._mapManager.GetAllMapGrids(playerMap.Value)
			orderby o.Owner
			select o)
			{
				TransformComponent gridXform;
				if (base.TryComp<TransformComponent>(grid.Owner, ref gridXform))
				{
					List<CompletionOption> list = options;
					string text = grid.Owner.ToString();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 2);
					defaultInterpolatedStringHandler.AppendFormatted(base.MetaData(grid.Owner).EntityName);
					defaultInterpolatedStringHandler.AppendLiteral(" - Map ");
					defaultInterpolatedStringHandler.AppendFormatted<MapId>(gridXform.MapID);
					list.Add(new CompletionOption(text, defaultInterpolatedStringHandler.ToStringAndClear(), 0));
				}
			}
			return CompletionResult.FromOptions(options);
		}

		// Token: 0x06002963 RID: 10595 RVA: 0x000D7890 File Offset: 0x000D5A90
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(NodeGroupSystem));
			this.InitializeBreathTool();
			this.InitializeGases();
			this.InitializeCommands();
			this.InitializeCVars();
			this.InitializeGridAtmosphere();
			this.InitializeMap();
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged), null, null);
		}

		// Token: 0x06002964 RID: 10596 RVA: 0x000D78F0 File Offset: 0x000D5AF0
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownCommands();
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x000D78FE File Offset: 0x000D5AFE
		private void OnTileChanged(ref TileChangedEvent ev)
		{
			this.InvalidateTile(ev.NewTile.GridUid, ev.NewTile.GridIndices);
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x000D791C File Offset: 0x000D5B1C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateProcessing(frameTime);
			this.UpdateHighPressure(frameTime);
			this._exposedTimer += frameTime;
			if (this._exposedTimer < 1f)
			{
				return;
			}
			foreach (ValueTuple<AtmosExposedComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<AtmosExposedComponent, TransformComponent>(false))
			{
				AtmosExposedComponent exposed = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				GasMixture air = this.GetContainingMixture(exposed.Owner, false, false, transform);
				if (air != null)
				{
					AtmosExposedUpdateEvent updateEvent = new AtmosExposedUpdateEvent(transform.Coordinates, air, transform);
					base.RaiseLocalEvent<AtmosExposedUpdateEvent>(exposed.Owner, ref updateEvent, false);
				}
			}
			this._exposedTimer -= 1f;
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06002967 RID: 10599 RVA: 0x000D79E8 File Offset: 0x000D5BE8
		// (set) Token: 0x06002968 RID: 10600 RVA: 0x000D79F0 File Offset: 0x000D5BF0
		public bool SpaceWind { get; private set; }

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x06002969 RID: 10601 RVA: 0x000D79F9 File Offset: 0x000D5BF9
		// (set) Token: 0x0600296A RID: 10602 RVA: 0x000D7A01 File Offset: 0x000D5C01
		public float SpaceWindPressureForceDivisorThrow { get; private set; }

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x0600296B RID: 10603 RVA: 0x000D7A0A File Offset: 0x000D5C0A
		// (set) Token: 0x0600296C RID: 10604 RVA: 0x000D7A12 File Offset: 0x000D5C12
		public float SpaceWindPressureForceDivisorPush { get; private set; }

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x0600296D RID: 10605 RVA: 0x000D7A1B File Offset: 0x000D5C1B
		// (set) Token: 0x0600296E RID: 10606 RVA: 0x000D7A23 File Offset: 0x000D5C23
		public float SpaceWindMaxVelocity { get; private set; }

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x0600296F RID: 10607 RVA: 0x000D7A2C File Offset: 0x000D5C2C
		// (set) Token: 0x06002970 RID: 10608 RVA: 0x000D7A34 File Offset: 0x000D5C34
		public float SpaceWindMaxPushForce { get; private set; }

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06002971 RID: 10609 RVA: 0x000D7A3D File Offset: 0x000D5C3D
		// (set) Token: 0x06002972 RID: 10610 RVA: 0x000D7A45 File Offset: 0x000D5C45
		public bool MonstermosEqualization { get; private set; }

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06002973 RID: 10611 RVA: 0x000D7A4E File Offset: 0x000D5C4E
		// (set) Token: 0x06002974 RID: 10612 RVA: 0x000D7A56 File Offset: 0x000D5C56
		public bool MonstermosDepressurization { get; private set; }

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06002975 RID: 10613 RVA: 0x000D7A5F File Offset: 0x000D5C5F
		// (set) Token: 0x06002976 RID: 10614 RVA: 0x000D7A67 File Offset: 0x000D5C67
		public bool MonstermosRipTiles { get; private set; }

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06002977 RID: 10615 RVA: 0x000D7A70 File Offset: 0x000D5C70
		// (set) Token: 0x06002978 RID: 10616 RVA: 0x000D7A78 File Offset: 0x000D5C78
		public bool GridImpulse { get; private set; }

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06002979 RID: 10617 RVA: 0x000D7A81 File Offset: 0x000D5C81
		// (set) Token: 0x0600297A RID: 10618 RVA: 0x000D7A89 File Offset: 0x000D5C89
		public bool Superconduction { get; private set; }

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x0600297B RID: 10619 RVA: 0x000D7A92 File Offset: 0x000D5C92
		// (set) Token: 0x0600297C RID: 10620 RVA: 0x000D7A9A File Offset: 0x000D5C9A
		public bool ExcitedGroups { get; private set; }

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x0600297D RID: 10621 RVA: 0x000D7AA3 File Offset: 0x000D5CA3
		// (set) Token: 0x0600297E RID: 10622 RVA: 0x000D7AAB File Offset: 0x000D5CAB
		public bool ExcitedGroupsSpaceIsAllConsuming { get; private set; }

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x0600297F RID: 10623 RVA: 0x000D7AB4 File Offset: 0x000D5CB4
		// (set) Token: 0x06002980 RID: 10624 RVA: 0x000D7ABC File Offset: 0x000D5CBC
		public float AtmosMaxProcessTime { get; private set; }

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06002981 RID: 10625 RVA: 0x000D7AC5 File Offset: 0x000D5CC5
		// (set) Token: 0x06002982 RID: 10626 RVA: 0x000D7ACD File Offset: 0x000D5CCD
		public float AtmosTickRate { get; private set; }

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06002983 RID: 10627 RVA: 0x000D7AD6 File Offset: 0x000D5CD6
		public float AtmosTime
		{
			get
			{
				return 1f / this.AtmosTickRate;
			}
		}

		// Token: 0x06002984 RID: 10628 RVA: 0x000D7AE4 File Offset: 0x000D5CE4
		private void InitializeCVars()
		{
			this._cfg.OnValueChanged<bool>(CCVars.SpaceWind, delegate(bool value)
			{
				this.SpaceWind = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.SpaceWindPressureForceDivisorThrow, delegate(float value)
			{
				this.SpaceWindPressureForceDivisorThrow = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.SpaceWindPressureForceDivisorPush, delegate(float value)
			{
				this.SpaceWindPressureForceDivisorPush = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.SpaceWindMaxVelocity, delegate(float value)
			{
				this.SpaceWindMaxVelocity = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.SpaceWindMaxPushForce, delegate(float value)
			{
				this.SpaceWindMaxPushForce = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.MonstermosEqualization, delegate(bool value)
			{
				this.MonstermosEqualization = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.MonstermosDepressurization, delegate(bool value)
			{
				this.MonstermosDepressurization = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.MonstermosRipTiles, delegate(bool value)
			{
				this.MonstermosRipTiles = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.AtmosGridImpulse, delegate(bool value)
			{
				this.GridImpulse = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.Superconduction, delegate(bool value)
			{
				this.Superconduction = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.AtmosMaxProcessTime, delegate(float value)
			{
				this.AtmosMaxProcessTime = value;
			}, true);
			this._cfg.OnValueChanged<float>(CCVars.AtmosTickRate, delegate(float value)
			{
				this.AtmosTickRate = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.ExcitedGroups, delegate(bool value)
			{
				this.ExcitedGroups = value;
			}, true);
			this._cfg.OnValueChanged<bool>(CCVars.ExcitedGroupsSpaceIsAllConsuming, delegate(bool value)
			{
				this.ExcitedGroupsSpaceIsAllConsuming = value;
			}, true);
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x000D7C87 File Offset: 0x000D5E87
		private void ExcitedGroupAddTile(ExcitedGroup excitedGroup, TileAtmosphere tile)
		{
			excitedGroup.Tiles.Add(tile);
			tile.ExcitedGroup = excitedGroup;
			this.ExcitedGroupResetCooldowns(excitedGroup);
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x000D7CA3 File Offset: 0x000D5EA3
		private void ExcitedGroupRemoveTile(ExcitedGroup excitedGroup, TileAtmosphere tile)
		{
			tile.ExcitedGroup = null;
			excitedGroup.Tiles.Remove(tile);
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x000D7CBC File Offset: 0x000D5EBC
		private void ExcitedGroupMerge(GridAtmosphereComponent gridAtmosphere, ExcitedGroup ourGroup, ExcitedGroup otherGroup)
		{
			int count = ourGroup.Tiles.Count;
			int otherSize = otherGroup.Tiles.Count;
			ExcitedGroup winner;
			ExcitedGroup loser;
			if (count > otherSize)
			{
				winner = ourGroup;
				loser = otherGroup;
			}
			else
			{
				winner = otherGroup;
				loser = ourGroup;
			}
			foreach (TileAtmosphere tile in loser.Tiles)
			{
				tile.ExcitedGroup = winner;
				winner.Tiles.Add(tile);
			}
			loser.Tiles.Clear();
			this.ExcitedGroupDispose(gridAtmosphere, loser);
			this.ExcitedGroupResetCooldowns(winner);
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x000D7D5C File Offset: 0x000D5F5C
		private void ExcitedGroupResetCooldowns(ExcitedGroup excitedGroup)
		{
			excitedGroup.BreakdownCooldown = 0;
			excitedGroup.DismantleCooldown = 0;
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x000D7D6C File Offset: 0x000D5F6C
		private void ExcitedGroupSelfBreakdown(GridAtmosphereComponent gridAtmosphere, ExcitedGroup excitedGroup)
		{
			GasMixture combined = new GasMixture(2500f);
			int tileSize = excitedGroup.Tiles.Count;
			if (excitedGroup.Disposed)
			{
				return;
			}
			if (tileSize == 0)
			{
				this.ExcitedGroupDispose(gridAtmosphere, excitedGroup);
				return;
			}
			foreach (TileAtmosphere tile in excitedGroup.Tiles)
			{
				if (((tile != null) ? tile.Air : null) != null)
				{
					this.Merge(combined, tile.Air);
					if (this.ExcitedGroupsSpaceIsAllConsuming && tile.Space)
					{
						combined.Clear();
						break;
					}
				}
			}
			combined.Multiply(1f / (float)tileSize);
			foreach (TileAtmosphere tile2 in excitedGroup.Tiles)
			{
				if (((tile2 != null) ? tile2.Air : null) != null)
				{
					tile2.Air.CopyFromMutable(combined);
					this.InvalidateVisuals(tile2.GridIndex, tile2.GridIndices, null);
				}
			}
			excitedGroup.BreakdownCooldown = 0;
		}

		// Token: 0x0600298A RID: 10634 RVA: 0x000D7E9C File Offset: 0x000D609C
		private void ExcitedGroupDismantle(GridAtmosphereComponent gridAtmosphere, ExcitedGroup excitedGroup, bool unexcite = true)
		{
			foreach (TileAtmosphere tile in excitedGroup.Tiles)
			{
				tile.ExcitedGroup = null;
				if (unexcite)
				{
					this.RemoveActiveTile(gridAtmosphere, tile, true);
				}
			}
			excitedGroup.Tiles.Clear();
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x000D7F08 File Offset: 0x000D6108
		private void ExcitedGroupDispose(GridAtmosphereComponent gridAtmosphere, ExcitedGroup excitedGroup)
		{
			if (excitedGroup.Disposed)
			{
				return;
			}
			excitedGroup.Disposed = true;
			gridAtmosphere.ExcitedGroups.Remove(excitedGroup);
			this.ExcitedGroupDismantle(gridAtmosphere, excitedGroup, false);
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x0600298C RID: 10636 RVA: 0x000D7F30 File Offset: 0x000D6130
		public IEnumerable<GasReactionPrototype> GasReactions
		{
			get
			{
				return this._gasReactions;
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x0600298D RID: 10637 RVA: 0x000D7F38 File Offset: 0x000D6138
		public float[] GasSpecificHeats
		{
			get
			{
				return this._gasSpecificHeats;
			}
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x000D7F40 File Offset: 0x000D6140
		private void InitializeGases()
		{
			this._gasReactions = this._protoMan.EnumeratePrototypes<GasReactionPrototype>().ToArray<GasReactionPrototype>();
			Array.Sort<GasReactionPrototype>(this._gasReactions, (GasReactionPrototype a, GasReactionPrototype b) => b.Priority.CompareTo(a.Priority));
			Array.Resize<float>(ref this._gasSpecificHeats, MathHelper.NextMultipleOf(9, 4));
			for (int i = 0; i < this.GasPrototypes.Length; i++)
			{
				this._gasSpecificHeats[i] = this.GasPrototypes[i].SpecificHeat;
				this.GasReagents[i] = this.GasPrototypes[i].Reagent;
			}
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x000D7FDD File Offset: 0x000D61DD
		public float GetHeatCapacity(GasMixture mixture)
		{
			return this.GetHeatCapacityCalculation(mixture.Moles, mixture.Immutable);
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x000D7FF4 File Offset: 0x000D61F4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe float GetHeatCapacityCalculation(float[] moles, bool space)
		{
			if (space && MathHelper.CloseTo(NumericsHelpers.HorizontalAdd(moles), 0f, 1E-07f))
			{
				return 7000f;
			}
			int num = moles.Length;
			Span<float> tmp = new Span<float>(stackalloc byte[checked(unchecked((UIntPtr)num) * 4)], num);
			NumericsHelpers.Multiply(moles, this.GasSpecificHeats, tmp);
			return MathF.Max(NumericsHelpers.HorizontalAdd(tmp), 0.0003f);
		}

		// Token: 0x06002991 RID: 10641 RVA: 0x000D8063 File Offset: 0x000D6263
		public float GetThermalEnergy(GasMixture mixture)
		{
			return mixture.Temperature * this.GetHeatCapacity(mixture);
		}

		// Token: 0x06002992 RID: 10642 RVA: 0x000D8073 File Offset: 0x000D6273
		public float GetThermalEnergy(GasMixture mixture, float cachedHeatCapacity)
		{
			return mixture.Temperature * cachedHeatCapacity;
		}

		// Token: 0x06002993 RID: 10643 RVA: 0x000D8080 File Offset: 0x000D6280
		public void Merge(GasMixture receiver, GasMixture giver)
		{
			if (receiver.Immutable)
			{
				return;
			}
			if (MathF.Abs(receiver.Temperature - giver.Temperature) > 0.5f)
			{
				float receiverHeatCapacity = this.GetHeatCapacity(receiver);
				float giverHeatCapacity = this.GetHeatCapacity(giver);
				float combinedHeatCapacity = receiverHeatCapacity + giverHeatCapacity;
				if (combinedHeatCapacity > 0f)
				{
					receiver.Temperature = (this.GetThermalEnergy(giver, giverHeatCapacity) + this.GetThermalEnergy(receiver, receiverHeatCapacity)) / combinedHeatCapacity;
				}
			}
			NumericsHelpers.Add(receiver.Moles, giver.Moles);
		}

		// Token: 0x06002994 RID: 10644 RVA: 0x000D8100 File Offset: 0x000D6300
		public void DivideInto(GasMixture source, List<GasMixture> receivers)
		{
			float totalVolume = 0f;
			foreach (GasMixture receiver in receivers)
			{
				if (!receiver.Immutable)
				{
					totalVolume += receiver.Volume;
				}
			}
			float? sourceHeatCapacity = null;
			float[] buffer = new float[Atmospherics.AdjustedNumberOfGases];
			foreach (GasMixture receiver2 in receivers)
			{
				if (!receiver2.Immutable)
				{
					float fraction = receiver2.Volume / totalVolume;
					if (MathF.Abs(receiver2.Temperature - source.Temperature) > 0.5f)
					{
						if (receiver2.TotalMoles == 0f)
						{
							receiver2.Temperature = source.Temperature;
						}
						else
						{
							float value = sourceHeatCapacity.GetValueOrDefault();
							if (sourceHeatCapacity == null)
							{
								value = this.GetHeatCapacity(source);
								sourceHeatCapacity = new float?(value);
							}
							float receiverHeatCapacity = this.GetHeatCapacity(receiver2);
							float combinedHeatCapacity = receiverHeatCapacity + sourceHeatCapacity.Value * fraction;
							if (combinedHeatCapacity > 0f)
							{
								receiver2.Temperature = (this.GetThermalEnergy(source, sourceHeatCapacity.Value * fraction) + this.GetThermalEnergy(receiver2, receiverHeatCapacity)) / combinedHeatCapacity;
							}
						}
					}
					NumericsHelpers.Multiply(source.Moles, fraction, buffer);
					NumericsHelpers.Add(receiver2.Moles, buffer);
				}
			}
		}

		// Token: 0x06002995 RID: 10645 RVA: 0x000D82B4 File Offset: 0x000D64B4
		public bool ReleaseGasTo(GasMixture mixture, [Nullable(2)] GasMixture output, float targetPressure)
		{
			float outputStartingPressure = (output != null) ? output.Pressure : 0f;
			float inputStartingPressure = mixture.Pressure;
			if (outputStartingPressure >= MathF.Min(targetPressure, inputStartingPressure - 10f))
			{
				return false;
			}
			if (mixture.TotalMoles <= 0f || mixture.Temperature <= 0f)
			{
				return false;
			}
			float transferMoles = MathF.Min(targetPressure - outputStartingPressure, (inputStartingPressure - outputStartingPressure) / 2f) * ((output != null) ? output.Volume : 2500f) / (mixture.Temperature * 8.314463f);
			GasMixture removed = mixture.Remove(transferMoles);
			if (output != null)
			{
				this.Merge(output, removed);
			}
			return true;
		}

		// Token: 0x06002996 RID: 10646 RVA: 0x000D834C File Offset: 0x000D654C
		public bool PumpGasTo(GasMixture mixture, GasMixture output, float targetPressure)
		{
			float outputStartingPressure = output.Pressure;
			float pressureDelta = targetPressure - outputStartingPressure;
			if ((double)pressureDelta < 0.01)
			{
				return false;
			}
			if (mixture.TotalMoles <= 0f || mixture.Temperature <= 0f)
			{
				return false;
			}
			float transferMoles = pressureDelta * output.Volume / (mixture.Temperature * 8.314463f);
			GasMixture removed = mixture.Remove(transferMoles);
			this.Merge(output, removed);
			return true;
		}

		// Token: 0x06002997 RID: 10647 RVA: 0x000D83B8 File Offset: 0x000D65B8
		public void ScrubInto(GasMixture mixture, GasMixture destination, IReadOnlyCollection<Gas> filterGases)
		{
			GasMixture buffer = new GasMixture(mixture.Volume)
			{
				Temperature = mixture.Temperature
			};
			foreach (Gas gas in filterGases)
			{
				buffer.AdjustMoles(gas, mixture.GetMoles(gas));
				mixture.SetMoles(gas, 0f);
			}
			this.Merge(destination, buffer);
		}

		// Token: 0x06002998 RID: 10648 RVA: 0x000D8434 File Offset: 0x000D6634
		[NullableContext(2)]
		public bool IsMixtureProbablySafe(GasMixture air)
		{
			if (air == null)
			{
				return false;
			}
			float num = air.Pressure;
			if (num <= 50f || num >= 385f)
			{
				return false;
			}
			num = air.Temperature;
			return num > 260f && num < 360f;
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x000D847C File Offset: 0x000D667C
		public AtmosphereSystem.GasCompareResult CompareExchange(GasMixture sample, GasMixture otherSample)
		{
			float moles = 0f;
			for (int i = 0; i < 9; i++)
			{
				float gasMoles = sample.Moles[i];
				float delta = MathF.Abs(gasMoles - otherSample.Moles[i]);
				if (delta > 0.103928f && delta > gasMoles * 0.001f)
				{
					return (AtmosphereSystem.GasCompareResult)i;
				}
				moles += gasMoles;
			}
			if (moles > 0.103928f && MathF.Abs(sample.Temperature - otherSample.Temperature) > 4f)
			{
				return AtmosphereSystem.GasCompareResult.TemperatureExchange;
			}
			return AtmosphereSystem.GasCompareResult.NoExchange;
		}

		// Token: 0x0600299A RID: 10650 RVA: 0x000D84F4 File Offset: 0x000D66F4
		public ReactionResult React(GasMixture mixture, [Nullable(2)] IGasMixtureHolder holder)
		{
			ReactionResult reaction = ReactionResult.NoReaction;
			float temperature = mixture.Temperature;
			float energy = this.GetThermalEnergy(mixture);
			foreach (GasReactionPrototype prototype in this.GasReactions)
			{
				if (energy >= prototype.MinimumEnergyRequirement && temperature >= prototype.MinimumTemperatureRequirement && temperature <= prototype.MaximumTemperatureRequirement)
				{
					bool doReaction = true;
					for (int i = 0; i < prototype.MinimumRequirements.Length; i++)
					{
						if (i >= 9)
						{
							throw new IndexOutOfRangeException("Reaction Gas Minimum Requirements Array Prototype exceeds total number of gases!");
						}
						float req = prototype.MinimumRequirements[i];
						if (mixture.GetMoles(i) < req)
						{
							doReaction = false;
							break;
						}
					}
					if (doReaction)
					{
						reaction = prototype.React(mixture, holder, this);
						if (reaction.HasFlag(ReactionResult.StopReactions))
						{
							break;
						}
					}
				}
			}
			return reaction;
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000D85E4 File Offset: 0x000D67E4
		private void InitializeGridAtmosphere()
		{
			base.SubscribeLocalEvent<GridAtmosphereComponent, ComponentInit>(new ComponentEventHandler<GridAtmosphereComponent, ComponentInit>(this.OnGridAtmosphereInit), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, GridSplitEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, GridSplitEvent>(this.OnGridSplit), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.HasAtmosphereMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.HasAtmosphereMethodEvent>(this.GridHasAtmosphere), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.IsSimulatedGridMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.IsSimulatedGridMethodEvent>(this.GridIsSimulated), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.GetAllMixturesMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.GetAllMixturesMethodEvent>(this.GridGetAllMixtures), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.InvalidateTileMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.InvalidateTileMethodEvent>(this.GridInvalidateTile), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.GetTileMixtureMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.GetTileMixtureMethodEvent>(this.GridGetTileMixture), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.GetTileMixturesMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.GetTileMixturesMethodEvent>(this.GridGetTileMixtures), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.ReactTileMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.ReactTileMethodEvent>(this.GridReactTile), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.IsTileAirBlockedMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.IsTileAirBlockedMethodEvent>(this.GridIsTileAirBlocked), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.IsTileSpaceMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.IsTileSpaceMethodEvent>(this.GridIsTileSpace), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.GetAdjacentTilesMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.GetAdjacentTilesMethodEvent>(this.GridGetAdjacentTiles), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.GetAdjacentTileMixturesMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.GetAdjacentTileMixturesMethodEvent>(this.GridGetAdjacentTileMixtures), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.UpdateAdjacentMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.UpdateAdjacentMethodEvent>(this.GridUpdateAdjacent), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.HotspotExposeMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.HotspotExposeMethodEvent>(this.GridHotspotExpose), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.HotspotExtinguishMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.HotspotExtinguishMethodEvent>(this.GridHotspotExtinguish), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.IsHotspotActiveMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.IsHotspotActiveMethodEvent>(this.GridIsHotspotActive), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.FixTileVacuumMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.FixTileVacuumMethodEvent>(this.GridFixTileVacuum), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.AddPipeNetMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.AddPipeNetMethodEvent>(this.GridAddPipeNet), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.RemovePipeNetMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.RemovePipeNetMethodEvent>(this.GridRemovePipeNet), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.AddAtmosDeviceMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.AddAtmosDeviceMethodEvent>(this.GridAddAtmosDevice), null, null);
			base.SubscribeLocalEvent<GridAtmosphereComponent, AtmosphereSystem.RemoveAtmosDeviceMethodEvent>(new ComponentEventRefHandler<GridAtmosphereComponent, AtmosphereSystem.RemoveAtmosDeviceMethodEvent>(this.GridRemoveAtmosDevice), null, null);
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000D87AC File Offset: 0x000D69AC
		private void OnGridAtmosphereInit(EntityUid uid, GridAtmosphereComponent gridAtmosphere, ComponentInit args)
		{
			base.Initialize();
			MapGridComponent mapGrid;
			if (!base.TryComp<MapGridComponent>(uid, ref mapGrid))
			{
				return;
			}
			base.EnsureComp<GasTileOverlayComponent>(uid);
			foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in gridAtmosphere.Tiles)
			{
				Vector2i vector2i;
				TileAtmosphere tileAtmosphere;
				keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
				Vector2i indices = vector2i;
				TileAtmosphere tileAtmosphere2 = tileAtmosphere;
				gridAtmosphere.InvalidatedCoords.Add(indices);
				tileAtmosphere2.GridIndex = uid;
			}
			this.GridRepopulateTiles(mapGrid, gridAtmosphere);
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x000D8840 File Offset: 0x000D6A40
		private void OnGridSplit(EntityUid uid, GridAtmosphereComponent originalGridAtmos, ref GridSplitEvent args)
		{
			foreach (EntityUid newGrid in args.NewGrids)
			{
				MapGridComponent mapGrid;
				if (this._mapManager.TryGetGrid(new EntityUid?(newGrid), ref mapGrid))
				{
					EntityUid entity = mapGrid.Owner;
					GridAtmosphereComponent newGridAtmos;
					if (!base.TryComp<GridAtmosphereComponent>(entity, ref newGridAtmos))
					{
						newGridAtmos = base.AddComp<GridAtmosphereComponent>(entity);
					}
					TileRef? tile;
					while (mapGrid.GetAllTilesEnumerator(true).MoveNext(ref tile))
					{
						Vector2i indices = tile.Value.GridIndices;
						TileAtmosphere tileAtmosphere;
						TileAtmosphere newTileAtmosphere;
						if (originalGridAtmos.Tiles.TryGetValue(indices, out tileAtmosphere) && newGridAtmos.Tiles.TryGetValue(indices, out newTileAtmosphere))
						{
							TileAtmosphere tileAtmosphere2 = newTileAtmosphere;
							GasMixture air = tileAtmosphere.Air;
							tileAtmosphere2.Air = (((air != null) ? air.Clone() : null) ?? null);
							newTileAtmosphere.MolesArchived = ((newTileAtmosphere.Air == null) ? null : new float[Atmospherics.AdjustedNumberOfGases]);
							newTileAtmosphere.Hotspot = tileAtmosphere.Hotspot;
							newTileAtmosphere.HeatCapacity = tileAtmosphere.HeatCapacity;
							newTileAtmosphere.Temperature = tileAtmosphere.Temperature;
							newTileAtmosphere.PressureDifference = tileAtmosphere.PressureDifference;
							newTileAtmosphere.PressureDirection = tileAtmosphere.PressureDirection;
							originalGridAtmos.InvalidatedCoords.Add(indices);
							newGridAtmos.InvalidatedCoords.Add(indices);
						}
					}
				}
			}
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000D8992 File Offset: 0x000D6B92
		private void GridHasAtmosphere(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.HasAtmosphereMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Result = true;
			args.Handled = true;
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000D89AB File Offset: 0x000D6BAB
		private void GridIsSimulated(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.IsSimulatedGridMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Simulated = component.Simulated;
			args.Handled = true;
		}

		// Token: 0x060029A0 RID: 10656 RVA: 0x000D89C9 File Offset: 0x000D6BC9
		private void GridGetAllMixtures(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.GetAllMixturesMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Mixtures = this.<GridGetAllMixtures>g__EnumerateMixtures|163_0(uid, component, args.Excite);
			args.Handled = true;
		}

		// Token: 0x060029A1 RID: 10657 RVA: 0x000D89EF File Offset: 0x000D6BEF
		private void GridInvalidateTile(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.InvalidateTileMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			component.InvalidatedCoords.Add(args.Tile);
			args.Handled = true;
		}

		// Token: 0x060029A2 RID: 10658 RVA: 0x000D8A14 File Offset: 0x000D6C14
		private void GridGetTileMixture(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.GetTileMixtureMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			if (args.Excite)
			{
				component.InvalidatedCoords.Add(args.Tile);
			}
			args.Mixture = tile.Air;
			args.Handled = true;
		}

		// Token: 0x060029A3 RID: 10659 RVA: 0x000D8A70 File Offset: 0x000D6C70
		private void GridGetTileMixtures(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.GetTileMixturesMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			args.Mixtures = new GasMixture[args.Tiles.Count];
			for (int i = 0; i < args.Tiles.Count; i++)
			{
				Vector2i tile = args.Tiles[i];
				TileAtmosphere atmosTile;
				if (!component.Tiles.TryGetValue(tile, out atmosTile))
				{
					args.Handled = false;
				}
				else
				{
					if (args.Excite)
					{
						component.InvalidatedCoords.Add(tile);
					}
					args.Mixtures[i] = atmosTile.Air;
				}
			}
		}

		// Token: 0x060029A4 RID: 10660 RVA: 0x000D8B04 File Offset: 0x000D6D04
		private void GridReactTile(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.ReactTileMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			GasMixture air = tile.Air;
			args.Result = ((air != null) ? this.React(air, tile) : ReactionResult.NoReaction);
			args.Handled = true;
		}

		// Token: 0x060029A5 RID: 10661 RVA: 0x000D8B54 File Offset: 0x000D6D54
		private void GridIsTileAirBlocked(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.IsTileAirBlockedMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			MapGridComponent mapGridComp = args.MapGridComponent;
			if (!base.Resolve<MapGridComponent>(uid, ref mapGridComp, true))
			{
				return;
			}
			AtmosDirection directions = AtmosDirection.Invalid;
			AirtightComponent obstructingComponent;
			while (this.GetObstructingComponentsEnumerator(mapGridComp, args.Tile).MoveNext(out obstructingComponent))
			{
				if (obstructingComponent.AirBlocked)
				{
					directions |= obstructingComponent.AirBlockedDirection;
					args.NoAir |= obstructingComponent.NoAirWhenFullyAirBlocked;
					if (directions.IsFlagSet(args.Direction))
					{
						args.Result = true;
						args.Handled = true;
						return;
					}
				}
			}
			args.Result = false;
			args.Handled = true;
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x000D8BE8 File Offset: 0x000D6DE8
		private void GridIsTileSpace(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.IsTileSpaceMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			args.Result = tile.Space;
			args.Handled = true;
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x000D8C28 File Offset: 0x000D6E28
		private void GridGetAdjacentTiles(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.GetAdjacentTilesMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			args.Result = AtmosphereSystem.<GridGetAdjacentTiles>g__EnumerateAdjacent|170_0(component, tile);
			args.Handled = true;
		}

		// Token: 0x060029A8 RID: 10664 RVA: 0x000D8C68 File Offset: 0x000D6E68
		private void GridGetAdjacentTileMixtures(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.GetAdjacentTileMixturesMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			args.Result = AtmosphereSystem.<GridGetAdjacentTileMixtures>g__EnumerateAdjacent|171_0(component, tile);
			args.Handled = true;
		}

		// Token: 0x060029A9 RID: 10665 RVA: 0x000D8CA8 File Offset: 0x000D6EA8
		private void GridUpdateAdjacent(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.UpdateAdjacentMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			MapGridComponent mapGridComp = args.MapGridComponent;
			if (!base.Resolve<MapGridComponent>(uid, ref mapGridComp, true))
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			EntityUid? mapUid = this._mapManager.MapExists(xform.MapID) ? new EntityUid?(this._mapManager.GetMapEntityId(xform.MapID)) : null;
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			tile.AdjacentBits = AtmosDirection.Invalid;
			tile.BlockedAirflow = this.GetBlockedDirections(mapGridComp, tile.GridIndices);
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				Vector2i otherIndices = tile.GridIndices.Offset(direction);
				TileAtmosphere adjacent;
				if (!component.Tiles.TryGetValue(otherIndices, out adjacent))
				{
					adjacent = new TileAtmosphere(tile.GridIndex, otherIndices, this.GetTileMixture(null, mapUid, otherIndices, false), false, this.IsTileSpace(null, mapUid, otherIndices, mapGridComp));
				}
				AtmosDirection oppositeDirection = direction.GetOpposite();
				adjacent.BlockedAirflow = this.GetBlockedDirections(mapGridComp, adjacent.GridIndices);
				AtmosphereSystem.IsTileAirBlockedMethodEvent tileBlockedEv = new AtmosphereSystem.IsTileAirBlockedMethodEvent(uid, tile.GridIndices, direction, mapGridComp, false, false);
				this.GridIsTileAirBlocked(uid, component, ref tileBlockedEv);
				AtmosphereSystem.IsTileAirBlockedMethodEvent adjacentBlockedEv = new AtmosphereSystem.IsTileAirBlockedMethodEvent(uid, adjacent.GridIndices, oppositeDirection, mapGridComp, false, false);
				this.GridIsTileAirBlocked(uid, component, ref adjacentBlockedEv);
				if (!adjacent.BlockedAirflow.IsFlagSet(oppositeDirection) && !tileBlockedEv.Result)
				{
					adjacent.AdjacentBits |= oppositeDirection;
					adjacent.AdjacentTiles[oppositeDirection.ToIndex()] = tile;
				}
				else
				{
					adjacent.AdjacentBits &= ~oppositeDirection;
					adjacent.AdjacentTiles[oppositeDirection.ToIndex()] = null;
				}
				if (!tile.BlockedAirflow.IsFlagSet(direction) && !adjacentBlockedEv.Result)
				{
					tile.AdjacentBits |= direction;
					tile.AdjacentTiles[direction.ToIndex()] = adjacent;
				}
				else
				{
					tile.AdjacentBits &= ~direction;
					tile.AdjacentTiles[direction.ToIndex()] = null;
				}
				if (!adjacent.AdjacentBits.IsFlagSet(adjacent.MonstermosInfo.CurrentTransferDirection))
				{
					adjacent.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
				}
			}
			if (!tile.AdjacentBits.IsFlagSet(tile.MonstermosInfo.CurrentTransferDirection))
			{
				tile.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
			}
		}

		// Token: 0x060029AA RID: 10666 RVA: 0x000D8F10 File Offset: 0x000D7110
		private void GridHotspotExpose(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.HotspotExposeMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			this.HotspotExpose(component, tile, args.ExposedTemperature, args.ExposedVolume, args.soh);
			args.Handled = true;
		}

		// Token: 0x060029AB RID: 10667 RVA: 0x000D8F60 File Offset: 0x000D7160
		private void GridHotspotExtinguish(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.HotspotExtinguishMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			tile.Hotspot = default(Hotspot);
			args.Handled = true;
			this.AddActiveTile(component, tile);
		}

		// Token: 0x060029AC RID: 10668 RVA: 0x000D8FA8 File Offset: 0x000D71A8
		private void GridIsHotspotActive(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.IsHotspotActiveMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TileAtmosphere tile;
			if (!component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			args.Result = tile.Hotspot.Valid;
			args.Handled = true;
		}

		// Token: 0x060029AD RID: 10669 RVA: 0x000D8FEC File Offset: 0x000D71EC
		private void GridFixTileVacuum(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.FixTileVacuumMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			AtmosphereSystem.GetAdjacentTileMixturesMethodEvent adjEv = new AtmosphereSystem.GetAdjacentTileMixturesMethodEvent(uid, args.Tile, false, true, null, false);
			this.GridGetAdjacentTileMixtures(uid, component, ref adjEv);
			TileAtmosphere tile;
			if (!adjEv.Handled || !component.Tiles.TryGetValue(args.Tile, out tile))
			{
				return;
			}
			MapGridComponent mapGridComp;
			if (!base.TryComp<MapGridComponent>(uid, ref mapGridComp))
			{
				return;
			}
			GasMixture[] adjacent = adjEv.Result.ToArray<GasMixture>();
			if (adjacent.Length == 0)
			{
				return;
			}
			tile.Air = new GasMixture
			{
				Volume = this.GetVolumeForTiles(mapGridComp, 1),
				Temperature = 293.15f
			};
			tile.MolesArchived = new float[Atmospherics.AdjustedNumberOfGases];
			tile.ArchivedCycle = 0;
			float ratio = 1f / (float)adjacent.Length;
			float totalTemperature = 0f;
			foreach (GasMixture adj in adjacent)
			{
				totalTemperature += adj.Temperature;
				GasMixture mix = adj.RemoveRatio(ratio);
				this.Merge(tile.Air, mix);
				this.Merge(adj, mix);
			}
			tile.Air.Temperature = totalTemperature / (float)adjacent.Length;
		}

		// Token: 0x060029AE RID: 10670 RVA: 0x000D9107 File Offset: 0x000D7307
		private void GridAddPipeNet(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.AddPipeNetMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = component.PipeNets.Add(args.PipeNet);
		}

		// Token: 0x060029AF RID: 10671 RVA: 0x000D9129 File Offset: 0x000D7329
		private void GridRemovePipeNet(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.RemovePipeNetMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = component.PipeNets.Remove(args.PipeNet);
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x000D914B File Offset: 0x000D734B
		private void GridAddAtmosDevice(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.AddAtmosDeviceMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.AtmosDevices.Add(args.Device))
			{
				return;
			}
			args.Device.JoinedGrid = new EntityUid?(uid);
			args.Handled = true;
			args.Result = true;
		}

		// Token: 0x060029B1 RID: 10673 RVA: 0x000D918C File Offset: 0x000D738C
		private void GridRemoveAtmosDevice(EntityUid uid, GridAtmosphereComponent component, ref AtmosphereSystem.RemoveAtmosDeviceMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (!component.AtmosDevices.Remove(args.Device))
			{
				return;
			}
			args.Device.JoinedGrid = null;
			args.Handled = true;
			args.Result = true;
		}

		// Token: 0x060029B2 RID: 10674 RVA: 0x000D91D8 File Offset: 0x000D73D8
		private void GridRepopulateTiles(MapGridComponent mapGrid, GridAtmosphereComponent gridAtmosphere)
		{
			float volume = this.GetVolumeForTiles(mapGrid, 1);
			foreach (TileRef tile in mapGrid.GetAllTiles(true))
			{
				if (!gridAtmosphere.Tiles.ContainsKey(tile.GridIndices))
				{
					gridAtmosphere.Tiles[tile.GridIndices] = new TileAtmosphere(tile.GridUid, tile.GridIndices, new GasMixture(volume)
					{
						Temperature = 293.15f
					}, false, false);
				}
				gridAtmosphere.InvalidatedCoords.Add(tile.GridIndices);
			}
			EntityUid uid = gridAtmosphere.Owner;
			GasTileOverlayComponent overlay;
			base.TryComp<GasTileOverlayComponent>(gridAtmosphere.Owner, ref overlay);
			foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in Extensions.ToArray<Vector2i, TileAtmosphere>(gridAtmosphere.Tiles))
			{
				Vector2i vector2i;
				TileAtmosphere tileAtmosphere;
				keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
				Vector2i position = vector2i;
				AtmosphereSystem.UpdateAdjacentMethodEvent ev = new AtmosphereSystem.UpdateAdjacentMethodEvent(uid, position, null, false);
				this.GridUpdateAdjacent(uid, gridAtmosphere, ref ev);
				this.InvalidateVisuals(mapGrid.Owner, position, overlay);
			}
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x060029B3 RID: 10675 RVA: 0x000D9300 File Offset: 0x000D7500
		// (set) Token: 0x060029B4 RID: 10676 RVA: 0x000D9308 File Offset: 0x000D7508
		[Nullable(2)]
		[ViewVariables]
		public string SpaceWindSound { [NullableContext(2)] get; [NullableContext(2)] private set; } = "/Audio/Effects/space_wind.ogg";

		// Token: 0x060029B5 RID: 10677 RVA: 0x000D9314 File Offset: 0x000D7514
		private void UpdateHighPressure(float frameTime)
		{
			RemQueue<MovedByPressureComponent> toRemove = default(RemQueue<MovedByPressureComponent>);
			foreach (MovedByPressureComponent comp in this._activePressures)
			{
				EntityUid uid = comp.Owner;
				MetaDataComponent metadata = null;
				if (base.Deleted(uid, metadata))
				{
					toRemove.Add(comp);
				}
				else if (!base.Paused(uid, metadata))
				{
					comp.Accumulator += frameTime;
					if (comp.Accumulator >= 2f)
					{
						comp.Accumulator = 0f;
						toRemove.Add(comp);
						PhysicsComponent body;
						if (base.HasComp<MobStateComponent>(uid) && base.TryComp<PhysicsComponent>(uid, ref body))
						{
							this._physics.SetBodyStatus(body, 0, true);
						}
						FixturesComponent fixtures;
						if (base.TryComp<FixturesComponent>(uid, ref fixtures))
						{
							foreach (Fixture fixture in fixtures.Fixtures.Values)
							{
								this._physics.AddCollisionMask(uid, fixture, 4, fixtures, null);
							}
						}
					}
				}
			}
			foreach (MovedByPressureComponent comp2 in toRemove)
			{
				this._activePressures.Remove(comp2);
			}
		}

		// Token: 0x060029B6 RID: 10678 RVA: 0x000D949C File Offset: 0x000D769C
		private void AddMobMovedByPressure(MovedByPressureComponent component, PhysicsComponent body)
		{
			FixturesComponent fixtures;
			if (!base.TryComp<FixturesComponent>(component.Owner, ref fixtures))
			{
				return;
			}
			this._physics.SetBodyStatus(body, 1, true);
			foreach (Fixture fixture in fixtures.Fixtures.Values)
			{
				this._physics.RemoveCollisionMask(body.Owner, fixture, 4, fixtures, null);
			}
			component.Accumulator = 0f;
			this._activePressures.Add(component);
		}

		// Token: 0x060029B7 RID: 10679 RVA: 0x000D953C File Offset: 0x000D773C
		private void HighPressureMovements(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> bodies, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xforms, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MovedByPressureComponent> pressureQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<MetaDataComponent> metas)
		{
			if (tile.PressureDifference > 15f && !tile.Hotspot.Valid && this._spaceWindSoundCooldown == 0 && !string.IsNullOrEmpty(this.SpaceWindSound))
			{
				EntityCoordinates coordinates = CoordinatesExtensions.ToEntityCoordinates(tile.GridIndices, tile.GridIndex, this._mapManager);
				SoundSystem.Play(this.SpaceWindSound, Filter.Pvs(coordinates, 2f, null, null), coordinates, new AudioParams?(AudioHelpers.WithVariation(0.125f).WithVolume(MathHelper.Clamp(tile.PressureDifference / 10f, 10f, 100f))));
			}
			float pressureDifference = tile.PressureDifference;
			int spaceWindSoundCooldown = this._spaceWindSoundCooldown;
			this._spaceWindSoundCooldown = spaceWindSoundCooldown + 1;
			if (spaceWindSoundCooldown > 75)
			{
				this._spaceWindSoundCooldown = 0;
			}
			if (!this.SpaceWind)
			{
				return;
			}
			Angle gridWorldRotation = xforms.GetComponent(gridAtmosphere.Owner).WorldRotation;
			if (this.MonstermosEqualization)
			{
				TileAtmosphere curTile = tile;
				int i = 0;
				while (i < 3 && curTile.PressureDirection != AtmosDirection.Invalid && curTile.AdjacentBits.IsFlagSet(curTile.PressureDirection))
				{
					curTile = curTile.AdjacentTiles[curTile.PressureDirection.ToIndex()];
					i++;
				}
				if (curTile != tile)
				{
					tile.PressureSpecificTarget = curTile;
				}
			}
			foreach (EntityUid entity in this._lookup.GetEntitiesIntersecting(tile.GridIndex, tile.GridIndices, 46))
			{
				PhysicsComponent body;
				MovedByPressureComponent pressure;
				if (bodies.TryGetComponent(entity, ref body) && pressureQuery.TryGetComponent(entity, ref pressure) && pressure.Enabled && !this._containers.IsEntityInContainer(entity, metas.GetComponent(entity)))
				{
					MovedByPressureComponent pressureMovements = base.EnsureComp<MovedByPressureComponent>(entity);
					if (pressure.LastHighPressureMovementAirCycle < gridAtmosphere.UpdateCounter)
					{
						MovedByPressureComponent component = pressureMovements;
						int updateCounter = gridAtmosphere.UpdateCounter;
						float pressureDifference2 = tile.PressureDifference;
						AtmosDirection pressureDirection = tile.PressureDirection;
						float pressureResistanceProbDelta = 0f;
						TileAtmosphere pressureSpecificTarget = tile.PressureSpecificTarget;
						this.ExperiencePressureDifference(component, updateCounter, pressureDifference2, pressureDirection, pressureResistanceProbDelta, (pressureSpecificTarget != null) ? CoordinatesExtensions.ToEntityCoordinates(pressureSpecificTarget.GridIndices, tile.GridIndex, this._mapManager) : EntityCoordinates.Invalid, gridWorldRotation, xforms.GetComponent(entity), body);
					}
				}
			}
		}

		// Token: 0x060029B8 RID: 10680 RVA: 0x000D978C File Offset: 0x000D798C
		private void ConsiderPressureDifference(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, AtmosDirection differenceDirection, float difference)
		{
			gridAtmosphere.HighPressureDelta.Add(tile);
			if (difference <= tile.PressureDifference)
			{
				return;
			}
			tile.PressureDifference = difference;
			tile.PressureDirection = differenceDirection;
		}

		// Token: 0x060029B9 RID: 10681 RVA: 0x000D97B8 File Offset: 0x000D79B8
		[NullableContext(2)]
		public void ExperiencePressureDifference([Nullable(1)] MovedByPressureComponent component, int cycle, float pressureDifference, AtmosDirection direction, float pressureResistanceProbDelta, EntityCoordinates throwTarget, Angle gridWorldRotation, TransformComponent xform = null, PhysicsComponent physics = null)
		{
			EntityUid uid = component.Owner;
			if (!base.Resolve<PhysicsComponent>(uid, ref physics, false))
			{
				return;
			}
			if (!base.Resolve<TransformComponent>(uid, ref xform, true))
			{
				return;
			}
			float maxForce = MathF.Sqrt(pressureDifference) * 2.25f;
			float moveProb = 100f;
			if (component.PressureResistance > 0f)
			{
				moveProb = MathF.Abs(pressureDifference / component.PressureResistance * 10f - 25f);
			}
			if ((moveProb > 25f && RandomExtensions.Prob(this._robustRandom, MathF.Min(moveProb / 100f, 1f)) && !float.IsPositiveInfinity(component.MoveResist) && physics.BodyType != 4 && maxForce >= component.MoveResist * 1f) || (physics.BodyType == 4 && maxForce >= component.MoveResist * 1f))
			{
				if (base.HasComp<MobStateComponent>(uid))
				{
					this.AddMobMovedByPressure(component, physics);
				}
				if (maxForce > 100f)
				{
					float moveForce = maxForce;
					moveForce /= ((throwTarget != EntityCoordinates.Invalid) ? this.SpaceWindPressureForceDivisorThrow : this.SpaceWindPressureForceDivisorPush);
					moveForce *= MathHelper.Clamp(moveProb, 0f, 100f);
					float maxSafeForceForObject = this.SpaceWindMaxVelocity * physics.Mass;
					moveForce = MathF.Min(moveForce, maxSafeForceForObject);
					Vector2 dirVec = (direction.ToAngle() + gridWorldRotation).ToWorldVec();
					if (throwTarget != EntityCoordinates.Invalid)
					{
						Vector2 pos = ((throwTarget.ToMap(this.EntityManager).Position - xform.WorldPosition).Normalized + dirVec).Normalized;
						this._physics.ApplyLinearImpulse(uid, pos * moveForce, null, physics);
					}
					else
					{
						moveForce = MathF.Min(moveForce, this.SpaceWindMaxPushForce);
						this._physics.ApplyLinearImpulse(uid, dirVec * moveForce, null, physics);
					}
					component.LastHighPressureMovementAirCycle = cycle;
				}
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x060029BA RID: 10682 RVA: 0x000D9999 File Offset: 0x000D7B99
		// (set) Token: 0x060029BB RID: 10683 RVA: 0x000D99A1 File Offset: 0x000D7BA1
		[Nullable(2)]
		[ViewVariables]
		public string HotspotSound { [NullableContext(2)] get; [NullableContext(2)] private set; } = "/Audio/Effects/fire.ogg";

		// Token: 0x060029BC RID: 10684 RVA: 0x000D99AC File Offset: 0x000D7BAC
		private void ProcessHotspot(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			if (!tile.Hotspot.Valid)
			{
				gridAtmosphere.HotspotTiles.Remove(tile);
				return;
			}
			if (!tile.Excited)
			{
				this.AddActiveTile(gridAtmosphere, tile);
			}
			if (!tile.Hotspot.SkippedFirstProcess)
			{
				tile.Hotspot.SkippedFirstProcess = true;
				return;
			}
			if (tile.ExcitedGroup != null)
			{
				this.ExcitedGroupResetCooldowns(tile.ExcitedGroup);
			}
			if (tile.Hotspot.Temperature < 373.15f || tile.Hotspot.Volume <= 1f || tile.Air == null || tile.Air.GetMoles(Gas.Oxygen) < 0.5f || (tile.Air.GetMoles(Gas.Plasma) < 0.5f && tile.Air.GetMoles(Gas.Tritium) < 0.5f))
			{
				tile.Hotspot = default(Hotspot);
				this.InvalidateVisuals(tile.GridIndex, tile.GridIndices, null);
				return;
			}
			this.PerformHotspotExposure(tile);
			if (tile.Hotspot.Bypassing)
			{
				tile.Hotspot.State = 3;
				if (tile.Air.Temperature > 423.15f)
				{
					float radiatedTemperature = tile.Air.Temperature * 0.85f;
					foreach (TileAtmosphere otherTile in tile.AdjacentTiles)
					{
						if (otherTile != null && !otherTile.Hotspot.Valid)
						{
							this.HotspotExpose(gridAtmosphere, otherTile, radiatedTemperature, 625f, false);
						}
					}
				}
			}
			else
			{
				tile.Hotspot.State = ((tile.Hotspot.Volume > 1000f) ? 2 : 1);
			}
			if (tile.Hotspot.Temperature > tile.MaxFireTemperatureSustained)
			{
				tile.MaxFireTemperatureSustained = tile.Hotspot.Temperature;
			}
			int i = this._hotspotSoundCooldown;
			this._hotspotSoundCooldown = i + 1;
			if (i == 0 && !string.IsNullOrEmpty(this.HotspotSound))
			{
				EntityCoordinates coordinates = CoordinatesExtensions.ToEntityCoordinates(tile.GridIndices, tile.GridIndex, this._mapManager);
				SoundSystem.Play(this.HotspotSound, Filter.Pvs(coordinates, 2f, null, null), coordinates, new AudioParams?(AudioHelpers.WithVariation(0.15f / (float)tile.Hotspot.State).WithVolume(-5f + 5f * (float)tile.Hotspot.State)));
			}
			if (this._hotspotSoundCooldown > 200)
			{
				this._hotspotSoundCooldown = 0;
			}
		}

		// Token: 0x060029BD RID: 10685 RVA: 0x000D9C04 File Offset: 0x000D7E04
		private void HotspotExpose(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, float exposedTemperature, float exposedVolume, bool soh = false)
		{
			if (tile.Air == null)
			{
				return;
			}
			if (tile.Air.GetMoles(Gas.Oxygen) < 0.5f)
			{
				return;
			}
			float plasma = tile.Air.GetMoles(Gas.Plasma);
			float tritium = tile.Air.GetMoles(Gas.Tritium);
			if (tile.Hotspot.Valid)
			{
				if (soh && (plasma > 0.5f || tritium > 0.5f))
				{
					if (tile.Hotspot.Temperature < exposedTemperature)
					{
						tile.Hotspot.Temperature = exposedTemperature;
					}
					if (tile.Hotspot.Volume < exposedVolume)
					{
						tile.Hotspot.Volume = exposedVolume;
					}
				}
				return;
			}
			if (exposedTemperature > 373.15f && (plasma > 0.5f || tritium > 0.5f))
			{
				tile.Hotspot = new Hotspot
				{
					Volume = exposedVolume * 25f,
					Temperature = exposedTemperature,
					SkippedFirstProcess = (tile.CurrentCycle > gridAtmosphere.UpdateCounter),
					Valid = true,
					State = 1
				};
				this.AddActiveTile(gridAtmosphere, tile);
				gridAtmosphere.HotspotTiles.Add(tile);
			}
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x000D9D18 File Offset: 0x000D7F18
		private void PerformHotspotExposure(TileAtmosphere tile)
		{
			if (tile.Air == null || !tile.Hotspot.Valid)
			{
				return;
			}
			tile.Hotspot.Bypassing = (tile.Hotspot.SkippedFirstProcess && tile.Hotspot.Volume > tile.Air.Volume * 0.95f);
			if (tile.Hotspot.Bypassing)
			{
				tile.Hotspot.Volume = tile.Air.ReactionResults[GasReaction.Fire] * 40000f;
				tile.Hotspot.Temperature = tile.Air.Temperature;
			}
			else
			{
				GasMixture affected = tile.Air.RemoveVolume(tile.Hotspot.Volume);
				affected.Temperature = tile.Hotspot.Temperature;
				this.React(affected, tile);
				tile.Hotspot.Temperature = affected.Temperature;
				tile.Hotspot.Volume = affected.ReactionResults[GasReaction.Fire] * 40000f;
				this.Merge(tile.Air, affected);
			}
			TileFireEvent fireEvent = new TileFireEvent(tile.Hotspot.Temperature, tile.Hotspot.Volume);
			foreach (EntityUid entity in this._lookup.GetEntitiesIntersecting(tile.GridIndex, tile.GridIndices, 46))
			{
				base.RaiseLocalEvent<TileFireEvent>(entity, ref fireEvent, false);
			}
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x000D9EA0 File Offset: 0x000D80A0
		private void ProcessCell(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, int fireCount, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (tile.Air == null)
			{
				this.RemoveActiveTile(gridAtmosphere, tile, true);
				return;
			}
			if (tile.ArchivedCycle < fireCount)
			{
				this.Archive(tile, fireCount);
			}
			tile.CurrentCycle = fireCount;
			int adjacentTileLength = 0;
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (tile.AdjacentBits.IsFlagSet(direction))
				{
					adjacentTileLength++;
				}
			}
			for (int j = 0; j < 4; j++)
			{
				AtmosDirection direction2 = (AtmosDirection)(1 << j);
				if (tile.AdjacentBits.IsFlagSet(direction2))
				{
					TileAtmosphere enemyTile = tile.AdjacentTiles[j];
					if (((enemyTile != null) ? enemyTile.Air : null) != null && fireCount > enemyTile.CurrentCycle)
					{
						this.Archive(enemyTile, fireCount);
						bool shouldShareAir = false;
						if (this.ExcitedGroups && tile.ExcitedGroup != null && enemyTile.ExcitedGroup != null)
						{
							if (tile.ExcitedGroup != enemyTile.ExcitedGroup)
							{
								this.ExcitedGroupMerge(gridAtmosphere, tile.ExcitedGroup, enemyTile.ExcitedGroup);
							}
							shouldShareAir = true;
						}
						else if (this.CompareExchange(tile.Air, enemyTile.Air) != AtmosphereSystem.GasCompareResult.NoExchange)
						{
							if (!enemyTile.Excited)
							{
								this.AddActiveTile(gridAtmosphere, enemyTile);
							}
							if (this.ExcitedGroups)
							{
								ExcitedGroup excitedGroup = tile.ExcitedGroup;
								if (excitedGroup == null)
								{
									excitedGroup = enemyTile.ExcitedGroup;
								}
								if (excitedGroup == null)
								{
									excitedGroup = new ExcitedGroup();
									gridAtmosphere.ExcitedGroups.Add(excitedGroup);
								}
								if (tile.ExcitedGroup == null)
								{
									this.ExcitedGroupAddTile(excitedGroup, tile);
								}
								if (enemyTile.ExcitedGroup == null)
								{
									this.ExcitedGroupAddTile(excitedGroup, enemyTile);
								}
							}
							shouldShareAir = true;
						}
						if (shouldShareAir)
						{
							float difference = this.Share(tile, enemyTile, adjacentTileLength);
							if (!this.MonstermosEqualization)
							{
								if (difference >= 0f)
								{
									this.ConsiderPressureDifference(gridAtmosphere, tile, direction2, difference);
								}
								else
								{
									this.ConsiderPressureDifference(gridAtmosphere, enemyTile, direction2.GetOpposite(), -difference);
								}
							}
							this.LastShareCheck(tile);
						}
					}
				}
			}
			if (tile.Air != null)
			{
				this.React(tile.Air, tile);
			}
			this.InvalidateVisuals(tile.GridIndex, tile.GridIndices, visuals);
			bool remove = true;
			if (tile.Air.Temperature > 693.15f && this.ConsiderSuperconductivity(gridAtmosphere, tile, true))
			{
				remove = false;
			}
			if (this.ExcitedGroups && tile.ExcitedGroup == null && remove)
			{
				this.RemoveActiveTile(gridAtmosphere, tile, true);
			}
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x000DA0E8 File Offset: 0x000D82E8
		private void Archive(TileAtmosphere tile, int fireCount)
		{
			if (tile.Air != null)
			{
				tile.Air.Moles.AsSpan<float>().CopyTo(tile.MolesArchived.AsSpan<float>());
				tile.TemperatureArchived = tile.Air.Temperature;
			}
			else
			{
				tile.TemperatureArchived = tile.Temperature;
			}
			tile.ArchivedCycle = fireCount;
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x000DA148 File Offset: 0x000D8348
		private void LastShareCheck(TileAtmosphere tile)
		{
			if (tile.Air == null || tile.ExcitedGroup == null)
			{
				return;
			}
			float lastShare = tile.LastShare;
			if (lastShare > 10.392799f)
			{
				this.ExcitedGroupResetCooldowns(tile.ExcitedGroup);
				return;
			}
			if (lastShare <= 0.103928f)
			{
				return;
			}
			tile.ExcitedGroup.DismantleCooldown = 0;
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x000DA197 File Offset: 0x000D8397
		private void AddActiveTile(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			if (tile.Air == null)
			{
				return;
			}
			tile.Excited = true;
			gridAtmosphere.ActiveTiles.Add(tile);
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x000DA1B6 File Offset: 0x000D83B6
		private void RemoveActiveTile(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, bool disposeExcitedGroup = true)
		{
			tile.Excited = false;
			gridAtmosphere.ActiveTiles.Remove(tile);
			if (tile.ExcitedGroup == null)
			{
				return;
			}
			if (disposeExcitedGroup)
			{
				this.ExcitedGroupDispose(gridAtmosphere, tile.ExcitedGroup);
				return;
			}
			this.ExcitedGroupRemoveTile(tile.ExcitedGroup, tile);
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x000DA1F3 File Offset: 0x000D83F3
		public float GetHeatCapacityArchived(TileAtmosphere tile)
		{
			if (tile.Air == null)
			{
				return tile.HeatCapacity;
			}
			return this.GetHeatCapacityCalculation(tile.MolesArchived, tile.Space);
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x000DA218 File Offset: 0x000D8418
		public float Share(TileAtmosphere tileReceiver, TileAtmosphere tileSharer, int atmosAdjacentTurfs)
		{
			GasMixture receiver = tileReceiver.Air;
			if (receiver != null)
			{
				GasMixture sharer = tileSharer.Air;
				if (sharer != null)
				{
					float temperatureDelta = tileReceiver.TemperatureArchived - tileSharer.TemperatureArchived;
					float absTemperatureDelta = Math.Abs(temperatureDelta);
					float oldHeatCapacity = 0f;
					float oldSharerHeatCapacity = 0f;
					if (absTemperatureDelta > 0.5f)
					{
						oldHeatCapacity = this.GetHeatCapacity(receiver);
						oldSharerHeatCapacity = this.GetHeatCapacity(sharer);
					}
					float heatCapacityToSharer = 0f;
					float heatCapacitySharerToThis = 0f;
					float movedMoles = 0f;
					float absMovedMoles = 0f;
					for (int i = 0; i < 9; i++)
					{
						float num = receiver.Moles[i];
						float sharerValue = sharer.Moles[i];
						float delta = (num - sharerValue) / (float)(atmosAdjacentTurfs + 1);
						if (MathF.Abs(delta) >= 5E-08f)
						{
							if (absTemperatureDelta > 0.5f)
							{
								float gasHeatCapacity = delta * this.GasSpecificHeats[i];
								if (delta > 0f)
								{
									heatCapacityToSharer += gasHeatCapacity;
								}
								else
								{
									heatCapacitySharerToThis -= gasHeatCapacity;
								}
							}
							if (!receiver.Immutable)
							{
								receiver.Moles[i] -= delta;
							}
							if (!sharer.Immutable)
							{
								sharer.Moles[i] += delta;
							}
							movedMoles += delta;
							absMovedMoles += MathF.Abs(delta);
						}
					}
					tileReceiver.LastShare = absMovedMoles;
					if (absTemperatureDelta > 0.5f)
					{
						float newHeatCapacity = oldHeatCapacity + heatCapacitySharerToThis - heatCapacityToSharer;
						float newSharerHeatCapacity = oldSharerHeatCapacity + heatCapacityToSharer - heatCapacitySharerToThis;
						if (!receiver.Immutable && newHeatCapacity > 0.0003f)
						{
							receiver.Temperature = (oldHeatCapacity * receiver.Temperature - heatCapacityToSharer * tileReceiver.TemperatureArchived + heatCapacitySharerToThis * tileSharer.TemperatureArchived) / newHeatCapacity;
						}
						if (!sharer.Immutable && newSharerHeatCapacity > 0.0003f)
						{
							sharer.Temperature = (oldSharerHeatCapacity * sharer.Temperature - heatCapacitySharerToThis * tileSharer.TemperatureArchived + heatCapacityToSharer * tileReceiver.TemperatureArchived) / newSharerHeatCapacity;
						}
						if (MathF.Abs(oldSharerHeatCapacity) > 0.0003f && (double)MathF.Abs(newSharerHeatCapacity / oldSharerHeatCapacity - 1f) < 0.1)
						{
							this.TemperatureShare(tileReceiver, tileSharer, 0.4f);
						}
					}
					if (temperatureDelta <= 393.15f && MathF.Abs(movedMoles) <= 0.103928f)
					{
						return 0f;
					}
					float moles = receiver.TotalMoles;
					float theirMoles = sharer.TotalMoles;
					return tileReceiver.TemperatureArchived * (moles + movedMoles) - tileSharer.TemperatureArchived * (theirMoles - movedMoles) * 8.314463f / receiver.Volume;
				}
			}
			return 0f;
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x000DA474 File Offset: 0x000D8674
		public float TemperatureShare(TileAtmosphere tileReceiver, TileAtmosphere tileSharer, float conductionCoefficient)
		{
			GasMixture receiver = tileReceiver.Air;
			if (receiver != null)
			{
				GasMixture sharer = tileSharer.Air;
				if (sharer != null)
				{
					float temperatureDelta = tileReceiver.TemperatureArchived - tileSharer.TemperatureArchived;
					if (MathF.Abs(temperatureDelta) > 0.5f)
					{
						float heatCapacity = this.GetHeatCapacityArchived(tileReceiver);
						float sharerHeatCapacity = this.GetHeatCapacityArchived(tileSharer);
						if (sharerHeatCapacity > 0.0003f && heatCapacity > 0.0003f)
						{
							float heat = conductionCoefficient * temperatureDelta * (heatCapacity * sharerHeatCapacity / (heatCapacity + sharerHeatCapacity));
							if (!receiver.Immutable)
							{
								receiver.Temperature = MathF.Abs(MathF.Max(receiver.Temperature - heat / heatCapacity, 2.7f));
							}
							if (!sharer.Immutable)
							{
								sharer.Temperature = MathF.Abs(MathF.Max(sharer.Temperature + heat / sharerHeatCapacity, 2.7f));
							}
						}
					}
					return sharer.Temperature;
				}
			}
			return 0f;
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x000DA544 File Offset: 0x000D8744
		public float TemperatureShare(TileAtmosphere tileReceiver, float conductionCoefficient, float sharerTemperature, float sharerHeatCapacity)
		{
			GasMixture receiver = tileReceiver.Air;
			if (receiver == null)
			{
				return 0f;
			}
			float temperatureDelta = tileReceiver.TemperatureArchived - sharerTemperature;
			if (MathF.Abs(temperatureDelta) > 0.5f)
			{
				float heatCapacity = this.GetHeatCapacityArchived(tileReceiver);
				if (sharerHeatCapacity > 0.0003f && heatCapacity > 0.0003f)
				{
					float heat = conductionCoefficient * temperatureDelta * (heatCapacity * sharerHeatCapacity / (heatCapacity + sharerHeatCapacity));
					if (!receiver.Immutable)
					{
						receiver.Temperature = MathF.Abs(MathF.Max(receiver.Temperature - heat / heatCapacity, 2.7f));
					}
					sharerTemperature = MathF.Abs(MathF.Max(sharerTemperature + heat / sharerHeatCapacity, 2.7f));
				}
			}
			return sharerTemperature;
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x000DA5DD File Offset: 0x000D87DD
		private void InitializeMap()
		{
			base.SubscribeLocalEvent<MapAtmosphereComponent, AtmosphereSystem.IsTileSpaceMethodEvent>(new ComponentEventRefHandler<MapAtmosphereComponent, AtmosphereSystem.IsTileSpaceMethodEvent>(this.MapIsTileSpace), null, null);
			base.SubscribeLocalEvent<MapAtmosphereComponent, AtmosphereSystem.GetTileMixtureMethodEvent>(new ComponentEventRefHandler<MapAtmosphereComponent, AtmosphereSystem.GetTileMixtureMethodEvent>(this.MapGetTileMixture), null, null);
			base.SubscribeLocalEvent<MapAtmosphereComponent, AtmosphereSystem.GetTileMixturesMethodEvent>(new ComponentEventRefHandler<MapAtmosphereComponent, AtmosphereSystem.GetTileMixturesMethodEvent>(this.MapGetTileMixtures), null, null);
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x000DA61B File Offset: 0x000D881B
		private void MapIsTileSpace(EntityUid uid, MapAtmosphereComponent component, ref AtmosphereSystem.IsTileSpaceMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Result = component.Space;
			args.Handled = true;
		}

		// Token: 0x060029CA RID: 10698 RVA: 0x000DA639 File Offset: 0x000D8839
		private void MapGetTileMixture(EntityUid uid, MapAtmosphereComponent component, ref AtmosphereSystem.GetTileMixtureMethodEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			GasMixture mixture = component.Mixture;
			args.Mixture = ((mixture != null) ? mixture.Clone() : null);
			args.Handled = true;
		}

		// Token: 0x060029CB RID: 10699 RVA: 0x000DA664 File Offset: 0x000D8864
		private void MapGetTileMixtures(EntityUid uid, MapAtmosphereComponent component, ref AtmosphereSystem.GetTileMixturesMethodEvent args)
		{
			if (args.Handled || component.Mixture == null)
			{
				return;
			}
			args.Handled = true;
			ref AtmosphereSystem.GetTileMixturesMethodEvent ptr = ref args;
			if (ptr.Mixtures == null)
			{
				ptr.Mixtures = new GasMixture[args.Tiles.Count];
			}
			for (int i = 0; i < args.Tiles.Count; i++)
			{
				ref GasMixture ptr2 = ref args.Mixtures[i];
				if (ptr2 == null)
				{
					ptr2 = component.Mixture.Clone();
				}
			}
		}

		// Token: 0x060029CC RID: 10700 RVA: 0x000DA6E0 File Offset: 0x000D88E0
		private void EqualizePressureInZone(MapGridComponent mapGrid, GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, int cycleNum, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (tile.Air == null || tile.MonstermosInfo.LastCycle >= cycleNum)
			{
				return;
			}
			tile.MonstermosInfo = default(MonstermosInfo);
			float startingMoles = tile.Air.TotalMoles;
			bool runAtmos = false;
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (tile.AdjacentBits.IsFlagSet(direction))
				{
					TileAtmosphere other = tile.AdjacentTiles[i];
					if (((other != null) ? other.Air : null) != null && MathF.Abs(other.Air.TotalMoles - startingMoles) > 0.103928f)
					{
						runAtmos = true;
						break;
					}
				}
			}
			if (!runAtmos)
			{
				tile.MonstermosInfo.LastCycle = cycleNum;
				return;
			}
			long num = gridAtmosphere.EqualizationQueueCycleControl + 1L;
			gridAtmosphere.EqualizationQueueCycleControl = num;
			long queueCycle = num;
			float totalMoles = 0f;
			this._equalizeTiles[0] = tile;
			tile.MonstermosInfo.LastQueueCycle = queueCycle;
			int tileCount = 1;
			int j = 0;
			while (j < tileCount && j <= 2000)
			{
				TileAtmosphere exploring = this._equalizeTiles[j];
				if (j < 200)
				{
					float tileMoles = exploring.Air.TotalMoles;
					exploring.MonstermosInfo.MoleDelta = tileMoles;
					totalMoles += tileMoles;
				}
				for (int k = 0; k < 4; k++)
				{
					AtmosDirection direction2 = (AtmosDirection)(1 << k);
					if (exploring.AdjacentBits.IsFlagSet(direction2))
					{
						TileAtmosphere adj = exploring.AdjacentTiles[k];
						if (((adj != null) ? adj.Air : null) != null && adj.MonstermosInfo.LastQueueCycle != queueCycle)
						{
							adj.MonstermosInfo = new MonstermosInfo
							{
								LastQueueCycle = queueCycle
							};
							if (tileCount < 2000)
							{
								this._equalizeTiles[tileCount++] = adj;
							}
							if (adj.Space && this.MonstermosDepressurization)
							{
								this.ExplosivelyDepressurize(mapGrid, gridAtmosphere, tile, cycleNum, visuals);
								return;
							}
						}
					}
				}
				j++;
			}
			if (tileCount > 200)
			{
				for (int l = 200; l < tileCount; l++)
				{
					TileAtmosphere otherTile = this._equalizeTiles[l];
					if (otherTile != null)
					{
						otherTile.MonstermosInfo.LastQueueCycle = 0L;
					}
				}
				tileCount = 200;
			}
			float averageMoles = totalMoles / (float)tileCount;
			int giverTilesLength = 0;
			int takerTilesLength = 0;
			for (int m = 0; m < tileCount; m++)
			{
				TileAtmosphere otherTile2 = this._equalizeTiles[m];
				otherTile2.MonstermosInfo.LastCycle = cycleNum;
				TileAtmosphere tileAtmosphere = otherTile2;
				tileAtmosphere.MonstermosInfo.MoleDelta = tileAtmosphere.MonstermosInfo.MoleDelta - averageMoles;
				if (otherTile2.MonstermosInfo.MoleDelta > 0f)
				{
					this._equalizeGiverTiles[giverTilesLength++] = otherTile2;
				}
				else
				{
					this._equalizeTakerTiles[takerTilesLength++] = otherTile2;
				}
			}
			float logN = MathF.Log2((float)tileCount);
			if ((float)giverTilesLength > logN && (float)takerTilesLength > logN)
			{
				Array.Sort<TileAtmosphere>(this._equalizeTiles, 0, tileCount, this._monstermosComparer);
				for (int n = 0; n < tileCount; n++)
				{
					TileAtmosphere otherTile3 = this._equalizeTiles[n];
					otherTile3.MonstermosInfo.FastDone = true;
					if (otherTile3.MonstermosInfo.MoleDelta > 0f)
					{
						AtmosDirection eligibleDirections = AtmosDirection.Invalid;
						int eligibleDirectionCount = 0;
						for (int j2 = 0; j2 < 4; j2++)
						{
							AtmosDirection direction3 = (AtmosDirection)(1 << j2);
							if (otherTile3.AdjacentBits.IsFlagSet(direction3))
							{
								TileAtmosphere tile2 = otherTile3.AdjacentTiles[j2];
								if (!tile2.MonstermosInfo.FastDone && tile2.MonstermosInfo.LastQueueCycle == queueCycle)
								{
									eligibleDirections |= direction3;
									eligibleDirectionCount++;
								}
							}
						}
						if (eligibleDirectionCount > 0)
						{
							float molesToMove = otherTile3.MonstermosInfo.MoleDelta / (float)eligibleDirectionCount;
							for (int j3 = 0; j3 < 4; j3++)
							{
								AtmosDirection direction4 = (AtmosDirection)(1 << j3);
								if (eligibleDirections.IsFlagSet(direction4))
								{
									this.AdjustEqMovement(otherTile3, direction4, molesToMove);
									TileAtmosphere tileAtmosphere2 = otherTile3;
									tileAtmosphere2.MonstermosInfo.MoleDelta = tileAtmosphere2.MonstermosInfo.MoleDelta - molesToMove;
									TileAtmosphere tileAtmosphere3 = otherTile3.AdjacentTiles[j3];
									tileAtmosphere3.MonstermosInfo.MoleDelta = tileAtmosphere3.MonstermosInfo.MoleDelta + molesToMove;
								}
							}
						}
					}
				}
				giverTilesLength = 0;
				takerTilesLength = 0;
				for (int i2 = 0; i2 < tileCount; i2++)
				{
					TileAtmosphere otherTile4 = this._equalizeTiles[i2];
					if (otherTile4.MonstermosInfo.MoleDelta > 0f)
					{
						this._equalizeGiverTiles[giverTilesLength++] = otherTile4;
					}
					else
					{
						this._equalizeTakerTiles[takerTilesLength++] = otherTile4;
					}
				}
			}
			if (giverTilesLength < takerTilesLength)
			{
				for (int j4 = 0; j4 < giverTilesLength; j4++)
				{
					TileAtmosphere giver = this._equalizeGiverTiles[j4];
					giver.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
					giver.MonstermosInfo.CurrentTransferAmount = 0f;
					num = gridAtmosphere.EqualizationQueueCycleControl + 1L;
					gridAtmosphere.EqualizationQueueCycleControl = num;
					long queueCycleSlow = num;
					int queueLength = 0;
					this._equalizeQueue[queueLength++] = giver;
					giver.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow;
					int i3 = 0;
					while (i3 < queueLength && giver.MonstermosInfo.MoleDelta > 0f)
					{
						TileAtmosphere otherTile5 = this._equalizeQueue[i3];
						for (int k2 = 0; k2 < 4; k2++)
						{
							AtmosDirection direction5 = (AtmosDirection)(1 << k2);
							if (otherTile5.AdjacentBits.IsFlagSet(direction5))
							{
								TileAtmosphere otherTile6 = otherTile5.AdjacentTiles[k2];
								if (giver.MonstermosInfo.MoleDelta <= 0f)
								{
									break;
								}
								if (otherTile6 != null && otherTile6.MonstermosInfo.LastQueueCycle == queueCycle && otherTile6.MonstermosInfo.LastSlowQueueCycle != queueCycleSlow)
								{
									this._equalizeQueue[queueLength++] = otherTile6;
									otherTile6.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow;
									otherTile6.MonstermosInfo.CurrentTransferDirection = direction5.GetOpposite();
									otherTile6.MonstermosInfo.CurrentTransferAmount = 0f;
									if (otherTile6.MonstermosInfo.MoleDelta < 0f)
									{
										if (-otherTile6.MonstermosInfo.MoleDelta > giver.MonstermosInfo.MoleDelta)
										{
											TileAtmosphere tileAtmosphere4 = otherTile6;
											tileAtmosphere4.MonstermosInfo.CurrentTransferAmount = tileAtmosphere4.MonstermosInfo.CurrentTransferAmount - giver.MonstermosInfo.MoleDelta;
											TileAtmosphere tileAtmosphere5 = otherTile6;
											tileAtmosphere5.MonstermosInfo.MoleDelta = tileAtmosphere5.MonstermosInfo.MoleDelta + giver.MonstermosInfo.MoleDelta;
											giver.MonstermosInfo.MoleDelta = 0f;
										}
										else
										{
											TileAtmosphere tileAtmosphere6 = otherTile6;
											tileAtmosphere6.MonstermosInfo.CurrentTransferAmount = tileAtmosphere6.MonstermosInfo.CurrentTransferAmount + otherTile6.MonstermosInfo.MoleDelta;
											TileAtmosphere tileAtmosphere7 = giver;
											tileAtmosphere7.MonstermosInfo.MoleDelta = tileAtmosphere7.MonstermosInfo.MoleDelta + otherTile6.MonstermosInfo.MoleDelta;
											otherTile6.MonstermosInfo.MoleDelta = 0f;
										}
									}
								}
							}
						}
						i3++;
					}
					for (int i4 = queueLength - 1; i4 >= 0; i4--)
					{
						TileAtmosphere otherTile7 = this._equalizeQueue[i4];
						if (otherTile7.MonstermosInfo.CurrentTransferAmount != 0f && otherTile7.MonstermosInfo.CurrentTransferDirection != AtmosDirection.Invalid)
						{
							this.AdjustEqMovement(otherTile7, otherTile7.MonstermosInfo.CurrentTransferDirection, otherTile7.MonstermosInfo.CurrentTransferAmount);
							TileAtmosphere tileAtmosphere8 = otherTile7.AdjacentTiles[otherTile7.MonstermosInfo.CurrentTransferDirection.ToIndex()];
							tileAtmosphere8.MonstermosInfo.CurrentTransferAmount = tileAtmosphere8.MonstermosInfo.CurrentTransferAmount + otherTile7.MonstermosInfo.CurrentTransferAmount;
							otherTile7.MonstermosInfo.CurrentTransferAmount = 0f;
						}
					}
				}
			}
			else
			{
				for (int j5 = 0; j5 < takerTilesLength; j5++)
				{
					TileAtmosphere taker = this._equalizeTakerTiles[j5];
					taker.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
					taker.MonstermosInfo.CurrentTransferAmount = 0f;
					num = gridAtmosphere.EqualizationQueueCycleControl + 1L;
					gridAtmosphere.EqualizationQueueCycleControl = num;
					long queueCycleSlow2 = num;
					int queueLength2 = 0;
					this._equalizeQueue[queueLength2++] = taker;
					taker.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow2;
					int i5 = 0;
					while (i5 < queueLength2 && taker.MonstermosInfo.MoleDelta < 0f)
					{
						TileAtmosphere otherTile8 = this._equalizeQueue[i5];
						for (int k3 = 0; k3 < 4; k3++)
						{
							AtmosDirection direction6 = (AtmosDirection)(1 << k3);
							if (otherTile8.AdjacentBits.IsFlagSet(direction6))
							{
								TileAtmosphere otherTile9 = otherTile8.AdjacentTiles[k3];
								if (taker.MonstermosInfo.MoleDelta >= 0f)
								{
									break;
								}
								if (otherTile9 != null && otherTile9.MonstermosInfo.LastQueueCycle == queueCycle && otherTile9.MonstermosInfo.LastSlowQueueCycle != queueCycleSlow2)
								{
									this._equalizeQueue[queueLength2++] = otherTile9;
									otherTile9.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow2;
									otherTile9.MonstermosInfo.CurrentTransferDirection = direction6.GetOpposite();
									otherTile9.MonstermosInfo.CurrentTransferAmount = 0f;
									if (otherTile9.MonstermosInfo.MoleDelta > 0f)
									{
										if (otherTile9.MonstermosInfo.MoleDelta > -taker.MonstermosInfo.MoleDelta)
										{
											TileAtmosphere tileAtmosphere9 = otherTile9;
											tileAtmosphere9.MonstermosInfo.CurrentTransferAmount = tileAtmosphere9.MonstermosInfo.CurrentTransferAmount - taker.MonstermosInfo.MoleDelta;
											TileAtmosphere tileAtmosphere10 = otherTile9;
											tileAtmosphere10.MonstermosInfo.MoleDelta = tileAtmosphere10.MonstermosInfo.MoleDelta + taker.MonstermosInfo.MoleDelta;
											taker.MonstermosInfo.MoleDelta = 0f;
										}
										else
										{
											TileAtmosphere tileAtmosphere11 = otherTile9;
											tileAtmosphere11.MonstermosInfo.CurrentTransferAmount = tileAtmosphere11.MonstermosInfo.CurrentTransferAmount + otherTile9.MonstermosInfo.MoleDelta;
											TileAtmosphere tileAtmosphere12 = taker;
											tileAtmosphere12.MonstermosInfo.MoleDelta = tileAtmosphere12.MonstermosInfo.MoleDelta + otherTile9.MonstermosInfo.MoleDelta;
											otherTile9.MonstermosInfo.MoleDelta = 0f;
										}
									}
								}
							}
						}
						i5++;
					}
					for (int i6 = queueLength2 - 1; i6 >= 0; i6--)
					{
						TileAtmosphere otherTile10 = this._equalizeQueue[i6];
						if (otherTile10.MonstermosInfo.CurrentTransferAmount != 0f && otherTile10.MonstermosInfo.CurrentTransferDirection != AtmosDirection.Invalid)
						{
							this.AdjustEqMovement(otherTile10, otherTile10.MonstermosInfo.CurrentTransferDirection, otherTile10.MonstermosInfo.CurrentTransferAmount);
							TileAtmosphere tileAtmosphere13 = otherTile10.AdjacentTiles[otherTile10.MonstermosInfo.CurrentTransferDirection.ToIndex()];
							tileAtmosphere13.MonstermosInfo.CurrentTransferAmount = tileAtmosphere13.MonstermosInfo.CurrentTransferAmount + otherTile10.MonstermosInfo.CurrentTransferAmount;
							otherTile10.MonstermosInfo.CurrentTransferAmount = 0f;
						}
					}
				}
			}
			for (int i7 = 0; i7 < tileCount; i7++)
			{
				TileAtmosphere otherTile11 = this._equalizeTiles[i7];
				this.FinalizeEq(gridAtmosphere, otherTile11, visuals);
			}
			for (int i8 = 0; i8 < tileCount; i8++)
			{
				TileAtmosphere otherTile12 = this._equalizeTiles[i8];
				for (int j6 = 0; j6 < 4; j6++)
				{
					AtmosDirection direction7 = (AtmosDirection)(1 << j6);
					if (otherTile12.AdjacentBits.IsFlagSet(direction7))
					{
						TileAtmosphere otherTile13 = otherTile12.AdjacentTiles[j6];
						if (otherTile13.Air == null || this.CompareExchange(otherTile13.Air, tile.Air) != AtmosphereSystem.GasCompareResult.NoExchange)
						{
							this.AddActiveTile(gridAtmosphere, otherTile13);
							break;
						}
					}
				}
			}
			Array.Clear(this._equalizeTiles, 0, 2000);
			Array.Clear(this._equalizeGiverTiles, 0, 200);
			Array.Clear(this._equalizeTakerTiles, 0, 200);
			Array.Clear(this._equalizeQueue, 0, 200);
		}

		// Token: 0x060029CD RID: 10701 RVA: 0x000DB1E4 File Offset: 0x000D93E4
		private void ExplosivelyDepressurize(MapGridComponent mapGrid, GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, int cycleNum, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (!this.MonstermosDepressurization || tile.Air == null)
			{
				return;
			}
			float totalMolesRemoved = 0f;
			long num = gridAtmosphere.EqualizationQueueCycleControl + 1L;
			gridAtmosphere.EqualizationQueueCycleControl = num;
			long queueCycle = num;
			int tileCount = 0;
			int spaceTileCount = 0;
			this._depressurizeTiles[tileCount++] = tile;
			tile.MonstermosInfo = new MonstermosInfo
			{
				LastQueueCycle = queueCycle
			};
			for (int i = 0; i < tileCount; i++)
			{
				TileAtmosphere otherTile = this._depressurizeTiles[i];
				otherTile.MonstermosInfo.LastCycle = cycleNum;
				otherTile.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
				if (!otherTile.Space)
				{
					for (int j = 0; j < 4; j++)
					{
						AtmosDirection direction = (AtmosDirection)(1 << j);
						if (otherTile.AdjacentBits.IsFlagSet(direction))
						{
							TileAtmosphere otherTile2 = otherTile.AdjacentTiles[j];
							if (((otherTile2 != null) ? otherTile2.Air : null) != null && otherTile2.MonstermosInfo.LastQueueCycle != queueCycle)
							{
								this.ConsiderFirelocks(gridAtmosphere, otherTile, otherTile2, visuals, mapGrid);
								if (otherTile.AdjacentBits.IsFlagSet(direction))
								{
									otherTile2.MonstermosInfo = new MonstermosInfo
									{
										LastQueueCycle = queueCycle
									};
									this._depressurizeTiles[tileCount++] = otherTile2;
									if (tileCount >= 2000)
									{
										break;
									}
								}
							}
						}
					}
				}
				else
				{
					this._depressurizeSpaceTiles[spaceTileCount++] = otherTile;
					otherTile.PressureSpecificTarget = otherTile;
				}
				if (tileCount >= 2000 || spaceTileCount >= 2000)
				{
					break;
				}
			}
			num = gridAtmosphere.EqualizationQueueCycleControl + 1L;
			gridAtmosphere.EqualizationQueueCycleControl = num;
			long queueCycleSlow = num;
			int progressionCount = 0;
			for (int k = 0; k < spaceTileCount; k++)
			{
				TileAtmosphere otherTile3 = this._depressurizeSpaceTiles[k];
				this._depressurizeProgressionOrder[progressionCount++] = otherTile3;
				otherTile3.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow;
				otherTile3.MonstermosInfo.CurrentTransferDirection = AtmosDirection.Invalid;
			}
			for (int l = 0; l < progressionCount; l++)
			{
				TileAtmosphere otherTile4 = this._depressurizeProgressionOrder[l];
				for (int m = 0; m < 4; m++)
				{
					AtmosDirection direction2 = (AtmosDirection)(1 << m);
					if (otherTile4.AdjacentBits.IsFlagSet(direction2) || otherTile4.Space)
					{
						TileAtmosphere tile2 = otherTile4.AdjacentTiles[m];
						if (tile2 != null && tile2.MonstermosInfo.LastQueueCycle == queueCycle && tile2.MonstermosInfo.LastSlowQueueCycle != queueCycleSlow && !tile2.Space)
						{
							tile2.MonstermosInfo.CurrentTransferDirection = direction2.GetOpposite();
							tile2.MonstermosInfo.CurrentTransferAmount = 0f;
							tile2.PressureSpecificTarget = otherTile4.PressureSpecificTarget;
							tile2.MonstermosInfo.LastSlowQueueCycle = queueCycleSlow;
							this._depressurizeProgressionOrder[progressionCount++] = tile2;
						}
					}
				}
			}
			for (int n = progressionCount - 1; n >= 0; n--)
			{
				TileAtmosphere otherTile5 = this._depressurizeProgressionOrder[n];
				if (otherTile5.MonstermosInfo.CurrentTransferDirection != AtmosDirection.Invalid)
				{
					gridAtmosphere.HighPressureDelta.Add(otherTile5);
					this.AddActiveTile(gridAtmosphere, otherTile5);
					TileAtmosphere otherTile6 = otherTile5.AdjacentTiles[otherTile5.MonstermosInfo.CurrentTransferDirection.ToIndex()];
					if (((otherTile6 != null) ? otherTile6.Air : null) != null)
					{
						float sum = otherTile6.Air.TotalMoles;
						totalMolesRemoved += sum;
						TileAtmosphere tileAtmosphere = otherTile5;
						tileAtmosphere.MonstermosInfo.CurrentTransferAmount = tileAtmosphere.MonstermosInfo.CurrentTransferAmount + sum;
						TileAtmosphere tileAtmosphere2 = otherTile6;
						tileAtmosphere2.MonstermosInfo.CurrentTransferAmount = tileAtmosphere2.MonstermosInfo.CurrentTransferAmount + otherTile5.MonstermosInfo.CurrentTransferAmount;
						otherTile5.PressureDifference = otherTile5.MonstermosInfo.CurrentTransferAmount;
						otherTile5.PressureDirection = otherTile5.MonstermosInfo.CurrentTransferDirection;
						if (otherTile6.MonstermosInfo.CurrentTransferDirection == AtmosDirection.Invalid)
						{
							otherTile6.PressureDifference = otherTile6.MonstermosInfo.CurrentTransferAmount;
							otherTile6.PressureDirection = otherTile5.MonstermosInfo.CurrentTransferDirection;
						}
						otherTile5.Air.Clear();
						otherTile5.Air.Temperature = 2.7f;
						this.InvalidateVisuals(otherTile5.GridIndex, otherTile5.GridIndices, visuals);
						this.HandleDecompressionFloorRip(mapGrid, otherTile5, sum);
					}
				}
			}
			if (this.GridImpulse && tileCount > 0)
			{
				Vector2 vector = this._depressurizeTiles[tileCount - 1].GridIndices - tile.GridIndices;
				Vector2 direction3 = vector.Normalized;
				PhysicsComponent gridPhysics = base.Comp<PhysicsComponent>(mapGrid.Owner);
				this._physics.ApplyLinearImpulse(mapGrid.Owner, direction3 * totalMolesRemoved * gridPhysics.Mass, null, gridPhysics);
				SharedPhysicsSystem physics = this._physics;
				EntityUid owner = mapGrid.Owner;
				vector = tile.GridIndices - gridPhysics.LocalCenter;
				physics.ApplyAngularImpulse(owner, Vector2.Cross(ref vector, ref direction3) * totalMolesRemoved, null, gridPhysics);
			}
			if (tileCount > 10 && totalMolesRemoved / (float)tileCount > 20f)
			{
				ISharedAdminLogManager adminLog = this._adminLog;
				LogType type = LogType.ExplosiveDepressurization;
				LogImpact impact = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(89, 4);
				logStringHandler.AppendLiteral("Explosive depressurization removed ");
				logStringHandler.AppendFormatted<float>(totalMolesRemoved, "totalMolesRemoved");
				logStringHandler.AppendLiteral(" moles from ");
				logStringHandler.AppendFormatted<int>(tileCount, "tileCount");
				logStringHandler.AppendLiteral(" tiles starting from position ");
				logStringHandler.AppendFormatted<Vector2i>(tile.GridIndices, "position", "tile.GridIndices");
				logStringHandler.AppendLiteral(" on grid ID ");
				logStringHandler.AppendFormatted<EntityUid>(tile.GridIndex, "grid", "tile.GridIndex");
				adminLog.Add(type, impact, ref logStringHandler);
			}
			Array.Clear(this._depressurizeTiles, 0, 2000);
			Array.Clear(this._depressurizeSpaceTiles, 0, 2000);
			Array.Clear(this._depressurizeProgressionOrder, 0, 4000);
		}

		// Token: 0x060029CE RID: 10702 RVA: 0x000DB78C File Offset: 0x000D998C
		private void ConsiderFirelocks(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, TileAtmosphere other, [Nullable(2)] GasTileOverlayComponent visuals, MapGridComponent mapGrid)
		{
			bool reconsiderAdjacent = false;
			foreach (EntityUid entity in mapGrid.GetAnchoredEntities(tile.GridIndices))
			{
				FirelockComponent firelock;
				if (base.TryComp<FirelockComponent>(entity, ref firelock))
				{
					reconsiderAdjacent |= this._firelockSystem.EmergencyPressureStop(entity, firelock, null);
				}
			}
			foreach (EntityUid entity2 in mapGrid.GetAnchoredEntities(other.GridIndices))
			{
				FirelockComponent firelock2;
				if (base.TryComp<FirelockComponent>(entity2, ref firelock2))
				{
					reconsiderAdjacent |= this._firelockSystem.EmergencyPressureStop(entity2, firelock2, null);
				}
			}
			if (!reconsiderAdjacent)
			{
				return;
			}
			AtmosphereSystem.UpdateAdjacentMethodEvent tileEv = new AtmosphereSystem.UpdateAdjacentMethodEvent(mapGrid.Owner, tile.GridIndices, null, false);
			AtmosphereSystem.UpdateAdjacentMethodEvent otherEv = new AtmosphereSystem.UpdateAdjacentMethodEvent(mapGrid.Owner, other.GridIndices, null, false);
			this.GridUpdateAdjacent(mapGrid.Owner, gridAtmosphere, ref tileEv);
			this.GridUpdateAdjacent(mapGrid.Owner, gridAtmosphere, ref otherEv);
			this.InvalidateVisuals(tile.GridIndex, tile.GridIndices, visuals);
			this.InvalidateVisuals(other.GridIndex, other.GridIndices, visuals);
		}

		// Token: 0x060029CF RID: 10703 RVA: 0x000DB8D0 File Offset: 0x000D9AD0
		private unsafe void FinalizeEq(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			Span<float> transferDirections = new Span<float>(stackalloc byte[(UIntPtr)16], 4);
			bool hasTransferDirs = false;
			for (int i = 0; i < 4; i++)
			{
				float amount = tile.MonstermosInfo[i];
				if (amount != 0f)
				{
					*transferDirections[i] = amount;
					tile.MonstermosInfo[i] = 0f;
					hasTransferDirs = true;
				}
			}
			if (!hasTransferDirs)
			{
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << j);
				if (tile.AdjacentBits.IsFlagSet(direction))
				{
					float amount2 = *transferDirections[j];
					TileAtmosphere otherTile = tile.AdjacentTiles[j];
					if (((otherTile != null) ? otherTile.Air : null) != null && amount2 > 0f)
					{
						if (tile.Air.TotalMoles < amount2)
						{
							this.FinalizeEqNeighbors(gridAtmosphere, tile, transferDirections, visuals);
						}
						otherTile.MonstermosInfo[direction.GetOpposite()] = 0f;
						this.Merge(otherTile.Air, tile.Air.Remove(amount2));
						this.InvalidateVisuals(tile.GridIndex, tile.GridIndices, visuals);
						this.InvalidateVisuals(otherTile.GridIndex, otherTile.GridIndices, visuals);
						this.ConsiderPressureDifference(gridAtmosphere, tile, direction, amount2);
					}
				}
			}
		}

		// Token: 0x060029D0 RID: 10704 RVA: 0x000DBA18 File Offset: 0x000D9C18
		private unsafe void FinalizeEqNeighbors(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, [Nullable(0)] ReadOnlySpan<float> transferDirs, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (*transferDirs[i] < 0f && tile.AdjacentBits.IsFlagSet(direction))
				{
					this.FinalizeEq(gridAtmosphere, tile.AdjacentTiles[i], visuals);
				}
			}
		}

		// Token: 0x060029D1 RID: 10705 RVA: 0x000DBA68 File Offset: 0x000D9C68
		private void AdjustEqMovement(TileAtmosphere tile, AtmosDirection direction, float amount)
		{
			if (tile == null)
			{
				Logger.Error("Encountered null-tile in AdjustEqMovement. Trace: " + Environment.StackTrace);
				return;
			}
			TileAtmosphere adj = tile.AdjacentTiles[direction.ToIndex()];
			if (adj == null)
			{
				int nonNull = (from x in tile.AdjacentTiles
				where x != null
				select x).Count<TileAtmosphere>();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(80, 5);
				defaultInterpolatedStringHandler.AppendLiteral("Encountered null adjacent tile in ");
				defaultInterpolatedStringHandler.AppendFormatted("AdjustEqMovement");
				defaultInterpolatedStringHandler.AppendLiteral(". Dir: ");
				defaultInterpolatedStringHandler.AppendFormatted<AtmosDirection>(direction);
				defaultInterpolatedStringHandler.AppendLiteral(", Tile: ");
				defaultInterpolatedStringHandler.AppendFormatted<TileRef?>(tile.Tile);
				defaultInterpolatedStringHandler.AppendLiteral(", non-null adj count: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(nonNull);
				defaultInterpolatedStringHandler.AppendLiteral(", Trace: ");
				defaultInterpolatedStringHandler.AppendFormatted(Environment.StackTrace);
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			ref MonstermosInfo ptr = ref tile.MonstermosInfo;
			ptr[direction] += amount;
			ptr = ref adj.MonstermosInfo;
			AtmosDirection opposite = direction.GetOpposite();
			ptr[opposite] -= amount;
		}

		// Token: 0x060029D2 RID: 10706 RVA: 0x000DBB98 File Offset: 0x000D9D98
		private void HandleDecompressionFloorRip(MapGridComponent mapGrid, TileAtmosphere tile, float sum)
		{
			if (!this.MonstermosRipTiles)
			{
				return;
			}
			float chance = MathHelper.Clamp(sum / 500f, 0.005f, 0.5f);
			if (sum > 20f && RandomExtensions.Prob(this._robustRandom, chance))
			{
				this.PryTile(mapGrid, tile.GridIndices);
			}
		}

		// Token: 0x060029D3 RID: 10707 RVA: 0x000DBBE8 File Offset: 0x000D9DE8
		private bool ProcessRevalidate(GridAtmosphereComponent atmosphere, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunInvalidatedCoordinates = new Queue<Vector2i>(atmosphere.InvalidatedCoords);
				atmosphere.InvalidatedCoords.Clear();
			}
			EntityUid uid = atmosphere.Owner;
			MapGridComponent mapGridComp;
			if (!base.TryComp<MapGridComponent>(uid, ref mapGridComp))
			{
				return true;
			}
			EntityUid mapUid = this._mapManager.GetMapEntityIdOrThrow(base.Transform(mapGridComp.Owner).MapID);
			float volume = this.GetVolumeForTiles(mapGridComp, 1);
			int number = 0;
			Vector2i indices;
			while (atmosphere.CurrentRunInvalidatedCoordinates.TryDequeue(out indices))
			{
				TileAtmosphere tile;
				if (!atmosphere.Tiles.TryGetValue(indices, out tile))
				{
					tile = new TileAtmosphere(mapGridComp.Owner, indices, new GasMixture(volume)
					{
						Temperature = 293.15f
					}, false, false);
					atmosphere.Tiles[indices] = tile;
				}
				AtmosphereSystem.IsTileAirBlockedMethodEvent airBlockedEv = new AtmosphereSystem.IsTileAirBlockedMethodEvent(uid, indices, AtmosDirection.All, mapGridComp, false, false);
				this.GridIsTileAirBlocked(uid, atmosphere, ref airBlockedEv);
				bool isAirBlocked = airBlockedEv.Result;
				AtmosDirection oldBlocked = tile.BlockedAirflow;
				AtmosphereSystem.UpdateAdjacentMethodEvent updateAdjacentEv = new AtmosphereSystem.UpdateAdjacentMethodEvent(uid, indices, mapGridComp, false);
				this.GridUpdateAdjacent(uid, atmosphere, ref updateAdjacentEv);
				if (tile.Excited && tile.BlockedAirflow != oldBlocked)
				{
					this.RemoveActiveTile(atmosphere, tile, true);
				}
				TileRef t;
				if ((!mapGridComp.TryGetTileRef(indices, ref t) || t.IsSpace(this._tileDefinitionManager)) && !isAirBlocked)
				{
					tile.Air = this.GetTileMixture(null, new EntityUid?(mapUid), indices, false);
					tile.MolesArchived = ((tile.Air != null) ? new float[Atmospherics.AdjustedNumberOfGases] : null);
					tile.Space = this.IsTileSpace(null, new EntityUid?(mapUid), indices, mapGridComp);
				}
				else
				{
					if (!isAirBlocked)
					{
						if (tile.Air == null && this.NeedsVacuumFixing(mapGridComp, indices))
						{
							AtmosphereSystem.FixTileVacuumMethodEvent vacuumEv = new AtmosphereSystem.FixTileVacuumMethodEvent(uid, indices, false);
							this.GridFixTileVacuum(uid, atmosphere, ref vacuumEv);
						}
						if (tile.Space)
						{
							goto IL_222;
						}
						GasMixture air = tile.Air;
						if (air != null && air.Immutable)
						{
							goto IL_222;
						}
						IL_24E:
						TileAtmosphere tileAtmosphere = tile;
						if (tileAtmosphere.Air == null)
						{
							tileAtmosphere.Air = new GasMixture(volume)
							{
								Temperature = 293.15f
							};
						}
						tileAtmosphere = tile;
						if (tileAtmosphere.MolesArchived == null)
						{
							tileAtmosphere.MolesArchived = new float[Atmospherics.AdjustedNumberOfGases];
							goto IL_294;
						}
						goto IL_294;
						IL_222:
						tile.Air = null;
						tile.MolesArchived = null;
						tile.ArchivedCycle = 0;
						tile.LastShare = 0f;
						tile.Space = false;
						goto IL_24E;
					}
					if (airBlockedEv.NoAir)
					{
						tile.Air = null;
						tile.MolesArchived = null;
						tile.ArchivedCycle = 0;
						tile.LastShare = 0f;
						tile.Hotspot = default(Hotspot);
					}
				}
				IL_294:
				this.AddActiveTile(atmosphere, tile);
				TileRef tileRef;
				ContentTileDefinition tileDef = mapGridComp.TryGetTileRef(indices, ref tileRef) ? tileRef.GetContentTileDefinition(this._tileDefinitionManager) : null;
				tile.ThermalConductivity = ((tileDef != null) ? tileDef.ThermalConductivity : 0.5f);
				tile.HeatCapacity = ((tileDef != null) ? tileDef.HeatCapacity : float.PositiveInfinity);
				this.InvalidateVisuals(mapGridComp.Owner, indices, visuals);
				for (int i = 0; i < 4; i++)
				{
					AtmosDirection direction = (AtmosDirection)(1 << i);
					Vector2i otherIndices = indices.Offset(direction);
					TileAtmosphere otherTile;
					if (atmosphere.Tiles.TryGetValue(otherIndices, out otherTile))
					{
						this.AddActiveTile(atmosphere, otherTile);
					}
				}
				if (number++ >= 50)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D4 RID: 10708 RVA: 0x000DBF64 File Offset: 0x000DA164
		private bool ProcessTileEqualize(GridAtmosphereComponent atmosphere, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunTiles = new Queue<TileAtmosphere>(atmosphere.ActiveTiles);
			}
			EntityUid uid = atmosphere.Owner;
			MapGridComponent mapGridComp;
			if (!base.TryComp<MapGridComponent>(uid, ref mapGridComp))
			{
				throw new Exception("Tried to process a grid atmosphere on an entity that isn't a grid!");
			}
			int number = 0;
			TileAtmosphere tile;
			while (atmosphere.CurrentRunTiles.TryDequeue(out tile))
			{
				this.EqualizePressureInZone(mapGridComp, atmosphere, tile, atmosphere.UpdateCounter, visuals);
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D5 RID: 10709 RVA: 0x000DBFF4 File Offset: 0x000DA1F4
		private bool ProcessActiveTiles(GridAtmosphereComponent atmosphere, [Nullable(2)] GasTileOverlayComponent visuals)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunTiles = new Queue<TileAtmosphere>(atmosphere.ActiveTiles);
			}
			int number = 0;
			TileAtmosphere tile;
			while (atmosphere.CurrentRunTiles.TryDequeue(out tile))
			{
				this.ProcessCell(atmosphere, tile, atmosphere.UpdateCounter, visuals);
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D6 RID: 10710 RVA: 0x000DC064 File Offset: 0x000DA264
		private bool ProcessExcitedGroups(GridAtmosphereComponent gridAtmosphere)
		{
			if (!gridAtmosphere.ProcessingPaused)
			{
				gridAtmosphere.CurrentRunExcitedGroups = new Queue<ExcitedGroup>(gridAtmosphere.ExcitedGroups);
			}
			int number = 0;
			ExcitedGroup excitedGroup;
			while (gridAtmosphere.CurrentRunExcitedGroups.TryDequeue(out excitedGroup))
			{
				ExcitedGroup excitedGroup2 = excitedGroup;
				int num = excitedGroup2.BreakdownCooldown;
				excitedGroup2.BreakdownCooldown = num + 1;
				ExcitedGroup excitedGroup3 = excitedGroup;
				num = excitedGroup3.DismantleCooldown;
				excitedGroup3.DismantleCooldown = num + 1;
				if (excitedGroup.BreakdownCooldown > 4)
				{
					this.ExcitedGroupSelfBreakdown(gridAtmosphere, excitedGroup);
				}
				else if (excitedGroup.DismantleCooldown > 16)
				{
					this.ExcitedGroupDismantle(gridAtmosphere, excitedGroup, true);
				}
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D7 RID: 10711 RVA: 0x000DC10C File Offset: 0x000DA30C
		private bool ProcessHighPressureDelta(GridAtmosphereComponent atmosphere)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunTiles = new Queue<TileAtmosphere>(atmosphere.HighPressureDelta);
			}
			int number = 0;
			EntityQuery<PhysicsComponent> bodies = this.EntityManager.GetEntityQuery<PhysicsComponent>();
			EntityQuery<TransformComponent> xforms = this.EntityManager.GetEntityQuery<TransformComponent>();
			EntityQuery<MetaDataComponent> metas = this.EntityManager.GetEntityQuery<MetaDataComponent>();
			EntityQuery<MovedByPressureComponent> pressureQuery = this.EntityManager.GetEntityQuery<MovedByPressureComponent>();
			TileAtmosphere tile;
			while (atmosphere.CurrentRunTiles.TryDequeue(out tile))
			{
				this.HighPressureMovements(atmosphere, tile, bodies, xforms, pressureQuery, metas);
				tile.PressureDifference = 0f;
				tile.LastPressureDirection = tile.PressureDirection;
				tile.PressureDirection = AtmosDirection.Invalid;
				tile.PressureSpecificTarget = null;
				atmosphere.HighPressureDelta.Remove(tile);
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D8 RID: 10712 RVA: 0x000DC1E8 File Offset: 0x000DA3E8
		private bool ProcessHotspots(GridAtmosphereComponent atmosphere)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunTiles = new Queue<TileAtmosphere>(atmosphere.HotspotTiles);
			}
			int number = 0;
			TileAtmosphere hotspot;
			while (atmosphere.CurrentRunTiles.TryDequeue(out hotspot))
			{
				this.ProcessHotspot(atmosphere, hotspot);
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029D9 RID: 10713 RVA: 0x000DC254 File Offset: 0x000DA454
		private bool ProcessSuperconductivity(GridAtmosphereComponent atmosphere)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunTiles = new Queue<TileAtmosphere>(atmosphere.SuperconductivityTiles);
			}
			int number = 0;
			TileAtmosphere superconductivity;
			while (atmosphere.CurrentRunTiles.TryDequeue(out superconductivity))
			{
				this.Superconduct(atmosphere, superconductivity);
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029DA RID: 10714 RVA: 0x000DC2C0 File Offset: 0x000DA4C0
		private bool ProcessPipeNets(GridAtmosphereComponent atmosphere)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunPipeNet = new Queue<IPipeNet>(atmosphere.PipeNets);
			}
			int number = 0;
			IPipeNet pipenet;
			while (atmosphere.CurrentRunPipeNet.TryDequeue(out pipenet))
			{
				pipenet.Update();
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029DB RID: 10715 RVA: 0x000DC328 File Offset: 0x000DA528
		private bool ProcessAtmosDevices(GridAtmosphereComponent atmosphere)
		{
			if (!atmosphere.ProcessingPaused)
			{
				atmosphere.CurrentRunAtmosDevices = new Queue<AtmosDeviceComponent>(atmosphere.AtmosDevices);
			}
			TimeSpan time = this._gameTiming.CurTime;
			int number = 0;
			AtmosDeviceComponent device;
			while (atmosphere.CurrentRunAtmosDevices.TryDequeue(out device))
			{
				base.RaiseLocalEvent<AtmosDeviceUpdateEvent>(device.Owner, this._updateEvent, false);
				device.LastProcess = time;
				if (number++ >= 30)
				{
					number = 0;
					if (this._simulationStopwatch.Elapsed.TotalMilliseconds >= (double)this.AtmosMaxProcessTime)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060029DC RID: 10716 RVA: 0x000DC3B0 File Offset: 0x000DA5B0
		private void UpdateProcessing(float frameTime)
		{
			this._simulationStopwatch.Restart();
			if (!this._simulationPaused)
			{
				this._currentRunAtmosphereIndex = 0;
				this._currentRunAtmosphere.Clear();
				this._currentRunAtmosphere.AddRange(this.EntityManager.EntityQuery<GridAtmosphereComponent>(false));
			}
			this._simulationPaused = true;
			while (this._currentRunAtmosphereIndex < this._currentRunAtmosphere.Count)
			{
				GridAtmosphereComponent atmosphere = this._currentRunAtmosphere[this._currentRunAtmosphereIndex];
				GasTileOverlayComponent visuals;
				base.TryComp<GasTileOverlayComponent>(atmosphere.Owner, ref visuals);
				if (atmosphere.LifeStage < 7 && !base.Paused(atmosphere.Owner, null) && atmosphere.Simulated)
				{
					atmosphere.Timer += frameTime;
					if (atmosphere.Timer >= this.AtmosTime)
					{
						atmosphere.Timer -= this.AtmosTime;
						switch (atmosphere.State)
						{
						case AtmosphereProcessingState.Revalidate:
							if (!this.ProcessRevalidate(atmosphere, visuals))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = (this.MonstermosEqualization ? AtmosphereProcessingState.TileEqualize : AtmosphereProcessingState.ActiveTiles);
							goto IL_264;
						case AtmosphereProcessingState.TileEqualize:
							if (!this.ProcessTileEqualize(atmosphere, visuals))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.ActiveTiles;
							goto IL_264;
						case AtmosphereProcessingState.ActiveTiles:
							if (!this.ProcessActiveTiles(atmosphere, visuals))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = (this.ExcitedGroups ? AtmosphereProcessingState.ExcitedGroups : AtmosphereProcessingState.HighPressureDelta);
							goto IL_264;
						case AtmosphereProcessingState.ExcitedGroups:
							if (!this.ProcessExcitedGroups(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.HighPressureDelta;
							goto IL_264;
						case AtmosphereProcessingState.HighPressureDelta:
							if (!this.ProcessHighPressureDelta(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.Hotspots;
							goto IL_264;
						case AtmosphereProcessingState.Hotspots:
							if (!this.ProcessHotspots(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = (this.Superconduction ? AtmosphereProcessingState.Superconductivity : AtmosphereProcessingState.PipeNet);
							goto IL_264;
						case AtmosphereProcessingState.Superconductivity:
							if (!this.ProcessSuperconductivity(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.PipeNet;
							goto IL_264;
						case AtmosphereProcessingState.PipeNet:
							if (!this.ProcessPipeNets(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.AtmosDevices;
							goto IL_264;
						case AtmosphereProcessingState.AtmosDevices:
							if (!this.ProcessAtmosDevices(atmosphere))
							{
								atmosphere.ProcessingPaused = true;
								return;
							}
							atmosphere.ProcessingPaused = false;
							atmosphere.State = AtmosphereProcessingState.Revalidate;
							break;
						}
						GridAtmosphereComponent gridAtmosphereComponent = atmosphere;
						int updateCounter = gridAtmosphereComponent.UpdateCounter;
						gridAtmosphereComponent.UpdateCounter = updateCounter + 1;
					}
				}
				IL_264:
				this._currentRunAtmosphereIndex++;
			}
			this._simulationPaused = false;
		}

		// Token: 0x060029DD RID: 10717 RVA: 0x000DC64C File Offset: 0x000DA84C
		private void Superconduct(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			AtmosDirection directions = this.ConductivityDirections(gridAtmosphere, tile);
			for (int i = 0; i < 4; i++)
			{
				AtmosDirection direction = (AtmosDirection)(1 << i);
				if (directions.IsFlagSet(direction))
				{
					TileAtmosphere adjacent = tile.AdjacentTiles[direction.ToIndex()];
					if (adjacent != null && adjacent.ThermalConductivity != 0f)
					{
						if (adjacent.ArchivedCycle < gridAtmosphere.UpdateCounter)
						{
							this.Archive(adjacent, gridAtmosphere.UpdateCounter);
						}
						this.NeighborConductWithSource(gridAtmosphere, adjacent, tile);
						this.ConsiderSuperconductivity(gridAtmosphere, adjacent);
					}
				}
			}
			this.RadiateToSpace(tile);
			this.FinishSuperconduction(gridAtmosphere, tile);
		}

		// Token: 0x060029DE RID: 10718 RVA: 0x000DC6D8 File Offset: 0x000DA8D8
		private AtmosDirection ConductivityDirections(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			if (tile.Air == null)
			{
				if (tile.ArchivedCycle < gridAtmosphere.UpdateCounter)
				{
					this.Archive(tile, gridAtmosphere.UpdateCounter);
				}
				return AtmosDirection.All;
			}
			return AtmosDirection.All;
		}

		// Token: 0x060029DF RID: 10719 RVA: 0x000DC702 File Offset: 0x000DA902
		public bool ConsiderSuperconductivity(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			if (tile.ThermalConductivity == 0f || !this.Superconduction)
			{
				return false;
			}
			gridAtmosphere.SuperconductivityTiles.Add(tile);
			return true;
		}

		// Token: 0x060029E0 RID: 10720 RVA: 0x000DC72C File Offset: 0x000DA92C
		public bool ConsiderSuperconductivity(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, bool starting)
		{
			return this.Superconduction && tile.Air != null && tile.Air.Temperature >= (starting ? 693.15f : 373.15f) && this.GetHeatCapacity(tile.Air) >= 0.51963997f && this.ConsiderSuperconductivity(gridAtmosphere, tile);
		}

		// Token: 0x060029E1 RID: 10721 RVA: 0x000DC788 File Offset: 0x000DA988
		public void FinishSuperconduction(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile)
		{
			if (tile.Air != null)
			{
				tile.Temperature = this.TemperatureShare(tile, tile.ThermalConductivity, tile.Temperature, tile.HeatCapacity);
			}
			GasMixture air = tile.Air;
			this.FinishSuperconduction(gridAtmosphere, tile, (air != null) ? air.Temperature : tile.Temperature);
		}

		// Token: 0x060029E2 RID: 10722 RVA: 0x000DC7DB File Offset: 0x000DA9DB
		public void FinishSuperconduction(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, float temperature)
		{
			if (temperature < 373.15f)
			{
				gridAtmosphere.SuperconductivityTiles.Remove(tile);
			}
		}

		// Token: 0x060029E3 RID: 10723 RVA: 0x000DC7F4 File Offset: 0x000DA9F4
		public void NeighborConductWithSource(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile, TileAtmosphere other)
		{
			if (tile.Air != null)
			{
				if (other.Air != null)
				{
					this.TemperatureShare(other, tile, 0.1f);
				}
				else
				{
					this.TemperatureShareOpenToSolid(tile, other);
				}
				this.AddActiveTile(gridAtmosphere, tile);
				return;
			}
			if (other.Tile != null)
			{
				this.TemperatureShareOpenToSolid(other, tile);
				return;
			}
			this.TemperatureShareMutualSolid(other, tile, tile.ThermalConductivity);
		}

		// Token: 0x060029E4 RID: 10724 RVA: 0x000DC859 File Offset: 0x000DAA59
		private void TemperatureShareOpenToSolid(TileAtmosphere tile, TileAtmosphere other)
		{
			if (tile.Air == null)
			{
				return;
			}
			other.Temperature = this.TemperatureShare(tile, other.ThermalConductivity, other.Temperature, other.HeatCapacity);
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x000DC884 File Offset: 0x000DAA84
		private void TemperatureShareMutualSolid(TileAtmosphere tile, TileAtmosphere other, float conductionCoefficient)
		{
			float deltaTemperature = tile.TemperatureArchived - other.TemperatureArchived;
			if (MathF.Abs(deltaTemperature) > 0.5f && tile.HeatCapacity != 0f && other.HeatCapacity != 0f)
			{
				float heat = conductionCoefficient * deltaTemperature * (tile.HeatCapacity * other.HeatCapacity / (tile.HeatCapacity + other.HeatCapacity));
				tile.Temperature -= heat / tile.HeatCapacity;
				other.Temperature += heat / other.HeatCapacity;
			}
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x000DC910 File Offset: 0x000DAB10
		public void RadiateToSpace(TileAtmosphere tile)
		{
			if (tile.Temperature > 273.15f)
			{
				float deltaTemperature = tile.TemperatureArchived - 2.7f;
				if (tile.HeatCapacity > 0f && MathF.Abs(deltaTemperature) > 0.5f)
				{
					float heat = tile.ThermalConductivity * deltaTemperature * (tile.HeatCapacity * 7000f / (tile.HeatCapacity + 7000f));
					tile.Temperature -= heat;
				}
			}
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x000DC984 File Offset: 0x000DAB84
		public double GetPrice(GasMixture mixture)
		{
			float basePrice = 0f;
			float totalMoles = 0f;
			float maxComponent = 0f;
			for (int i = 0; i < 9; i++)
			{
				basePrice += mixture.Moles[i] * base.GetGas(i).PricePerMole;
				totalMoles += mixture.Moles[i];
				maxComponent = Math.Max(maxComponent, mixture.Moles[i]);
			}
			float purity = 1f;
			if (totalMoles > 0f)
			{
				purity = maxComponent / totalMoles;
			}
			return (double)(basePrice * purity);
		}

		// Token: 0x060029E8 RID: 10728 RVA: 0x000DCA00 File Offset: 0x000DAC00
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InvalidateVisuals(EntityUid gridUid, Vector2i tile, GasTileOverlayComponent comp = null)
		{
			this._gasTileOverlaySystem.Invalidate(gridUid, tile, comp);
		}

		// Token: 0x060029E9 RID: 10729 RVA: 0x000DCA10 File Offset: 0x000DAC10
		public bool NeedsVacuumFixing(MapGridComponent mapGrid, Vector2i indices)
		{
			bool value = false;
			AirtightComponent airtight;
			while (this.GetObstructingComponentsEnumerator(mapGrid, indices).MoveNext(out airtight))
			{
				value |= airtight.FixVacuum;
			}
			return value;
		}

		// Token: 0x060029EA RID: 10730 RVA: 0x000DCA3F File Offset: 0x000DAC3F
		private float GetVolumeForTiles(MapGridComponent mapGrid, int tiles = 1)
		{
			return 2500f * (float)mapGrid.TileSize * (float)tiles;
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x000DCA54 File Offset: 0x000DAC54
		public AtmosObstructionEnumerator GetObstructingComponentsEnumerator(MapGridComponent mapGrid, Vector2i tile)
		{
			AnchoredEntitiesEnumerator ancEnumerator = mapGrid.GetAnchoredEntitiesEnumerator(tile);
			EntityQuery<AirtightComponent> airQuery = base.GetEntityQuery<AirtightComponent>();
			return new AtmosObstructionEnumerator(ancEnumerator, airQuery);
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x000DCA78 File Offset: 0x000DAC78
		private AtmosDirection GetBlockedDirections(MapGridComponent mapGrid, Vector2i indices)
		{
			AtmosDirection value = AtmosDirection.Invalid;
			AirtightComponent airtight;
			while (this.GetObstructingComponentsEnumerator(mapGrid, indices).MoveNext(out airtight))
			{
				if (airtight.AirBlocked)
				{
					value |= airtight.AirBlockedDirection;
				}
			}
			return value;
		}

		// Token: 0x060029ED RID: 10733 RVA: 0x000DCAB0 File Offset: 0x000DACB0
		private void PryTile(MapGridComponent mapGrid, Vector2i tile)
		{
			TileRef tileRef;
			if (!mapGrid.TryGetTileRef(tile, ref tileRef))
			{
				return;
			}
			this._tile.PryTile(tileRef);
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x000DCC4C File Offset: 0x000DAE4C
		[CompilerGenerated]
		private IEnumerable<GasMixture> <GridGetAllMixtures>g__EnumerateMixtures|163_0(EntityUid gridUid, GridAtmosphereComponent grid, bool invalidate)
		{
			foreach (KeyValuePair<Vector2i, TileAtmosphere> keyValuePair in grid.Tiles)
			{
				Vector2i vector2i;
				TileAtmosphere tileAtmosphere;
				keyValuePair.Deconstruct(out vector2i, out tileAtmosphere);
				TileAtmosphere tile = tileAtmosphere;
				if (tile.Air != null)
				{
					if (invalidate)
					{
						this.AddActiveTile(grid, tile);
					}
					yield return tile.Air;
				}
			}
			Dictionary<Vector2i, TileAtmosphere>.Enumerator enumerator = default(Dictionary<Vector2i, TileAtmosphere>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x000DCC6A File Offset: 0x000DAE6A
		[CompilerGenerated]
		internal static IEnumerable<Vector2i> <GridGetAdjacentTiles>g__EnumerateAdjacent|170_0(GridAtmosphereComponent grid, TileAtmosphere t)
		{
			foreach (TileAtmosphere adj in t.AdjacentTiles)
			{
				if (adj != null)
				{
					yield return adj.GridIndices;
				}
			}
			TileAtmosphere[] array = null;
			yield break;
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x000DCC7A File Offset: 0x000DAE7A
		[CompilerGenerated]
		internal static IEnumerable<GasMixture> <GridGetAdjacentTileMixtures>g__EnumerateAdjacent|171_0(GridAtmosphereComponent grid, TileAtmosphere t)
		{
			foreach (TileAtmosphere adj in t.AdjacentTiles)
			{
				if (((adj != null) ? adj.Air : null) != null)
				{
					yield return adj.Air;
				}
			}
			TileAtmosphere[] array = null;
			yield break;
		}

		// Token: 0x040019A9 RID: 6569
		[Dependency]
		private readonly IConsoleHost _consoleHost;

		// Token: 0x040019AA RID: 6570
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040019AB RID: 6571
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x040019AC RID: 6572
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x040019AD RID: 6573
		[Dependency]
		private readonly IAdminLogManager _adminLog;

		// Token: 0x040019AE RID: 6574
		[Dependency]
		private readonly InternalsSystem _internals;

		// Token: 0x040019AF RID: 6575
		[Dependency]
		private readonly SharedContainerSystem _containers;

		// Token: 0x040019B0 RID: 6576
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040019B1 RID: 6577
		[Dependency]
		private readonly GasTileOverlaySystem _gasTileOverlaySystem;

		// Token: 0x040019B2 RID: 6578
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x040019B3 RID: 6579
		[Dependency]
		private readonly TileSystem _tile;

		// Token: 0x040019B4 RID: 6580
		private const float ExposedUpdateDelay = 1f;

		// Token: 0x040019B5 RID: 6581
		private float _exposedTimer;

		// Token: 0x040019B6 RID: 6582
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040019C5 RID: 6597
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x040019C6 RID: 6598
		private GasReactionPrototype[] _gasReactions = Array.Empty<GasReactionPrototype>();

		// Token: 0x040019C7 RID: 6599
		private float[] _gasSpecificHeats = new float[9];

		// Token: 0x040019C8 RID: 6600
		[Nullable(new byte[]
		{
			1,
			2
		})]
		public string[] GasReagents = new string[9];

		// Token: 0x040019C9 RID: 6601
		private const int SpaceWindSoundCooldownCycles = 75;

		// Token: 0x040019CA RID: 6602
		private int _spaceWindSoundCooldown;

		// Token: 0x040019CC RID: 6604
		private HashSet<MovedByPressureComponent> _activePressures = new HashSet<MovedByPressureComponent>(8);

		// Token: 0x040019CD RID: 6605
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040019CE RID: 6606
		private const int HotspotSoundCooldownCycles = 200;

		// Token: 0x040019CF RID: 6607
		private int _hotspotSoundCooldown;

		// Token: 0x040019D1 RID: 6609
		[Dependency]
		private readonly FirelockSystem _firelockSystem;

		// Token: 0x040019D2 RID: 6610
		private readonly AtmosphereSystem.TileAtmosphereComparer _monstermosComparer = new AtmosphereSystem.TileAtmosphereComparer();

		// Token: 0x040019D3 RID: 6611
		[Nullable(new byte[]
		{
			1,
			2
		})]
		private readonly TileAtmosphere[] _equalizeTiles = new TileAtmosphere[2000];

		// Token: 0x040019D4 RID: 6612
		private readonly TileAtmosphere[] _equalizeGiverTiles = new TileAtmosphere[200];

		// Token: 0x040019D5 RID: 6613
		private readonly TileAtmosphere[] _equalizeTakerTiles = new TileAtmosphere[200];

		// Token: 0x040019D6 RID: 6614
		private readonly TileAtmosphere[] _equalizeQueue = new TileAtmosphere[200];

		// Token: 0x040019D7 RID: 6615
		private readonly TileAtmosphere[] _depressurizeTiles = new TileAtmosphere[2000];

		// Token: 0x040019D8 RID: 6616
		private readonly TileAtmosphere[] _depressurizeSpaceTiles = new TileAtmosphere[2000];

		// Token: 0x040019D9 RID: 6617
		private readonly TileAtmosphere[] _depressurizeProgressionOrder = new TileAtmosphere[4000];

		// Token: 0x040019DA RID: 6618
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040019DB RID: 6619
		private readonly AtmosDeviceUpdateEvent _updateEvent = new AtmosDeviceUpdateEvent();

		// Token: 0x040019DC RID: 6620
		private readonly Stopwatch _simulationStopwatch = new Stopwatch();

		// Token: 0x040019DD RID: 6621
		private const int LagCheckIterations = 30;

		// Token: 0x040019DE RID: 6622
		private const int InvalidCoordinatesLagCheckIterations = 50;

		// Token: 0x040019DF RID: 6623
		private int _currentRunAtmosphereIndex;

		// Token: 0x040019E0 RID: 6624
		private bool _simulationPaused;

		// Token: 0x040019E1 RID: 6625
		private readonly List<GridAtmosphereComponent> _currentRunAtmosphere = new List<GridAtmosphereComponent>();

		// Token: 0x02000B14 RID: 2836
		[NullableContext(0)]
		[ByRefEvent]
		private struct HasAtmosphereMethodEvent : IEquatable<AtmosphereSystem.HasAtmosphereMethodEvent>
		{
			// Token: 0x060036F0 RID: 14064 RVA: 0x0012235E File Offset: 0x0012055E
			public HasAtmosphereMethodEvent(EntityUid Grid, bool Result = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x17000871 RID: 2161
			// (get) Token: 0x060036F1 RID: 14065 RVA: 0x00122375 File Offset: 0x00120575
			// (set) Token: 0x060036F2 RID: 14066 RVA: 0x0012237D File Offset: 0x0012057D
			public EntityUid Grid { readonly get; set; }

			// Token: 0x17000872 RID: 2162
			// (get) Token: 0x060036F3 RID: 14067 RVA: 0x00122386 File Offset: 0x00120586
			// (set) Token: 0x060036F4 RID: 14068 RVA: 0x0012238E File Offset: 0x0012058E
			public bool Result { readonly get; set; }

			// Token: 0x17000873 RID: 2163
			// (get) Token: 0x060036F5 RID: 14069 RVA: 0x00122397 File Offset: 0x00120597
			// (set) Token: 0x060036F6 RID: 14070 RVA: 0x0012239F File Offset: 0x0012059F
			public bool Handled { readonly get; set; }

			// Token: 0x060036F7 RID: 14071 RVA: 0x001223A8 File Offset: 0x001205A8
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("HasAtmosphereMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060036F8 RID: 14072 RVA: 0x001223F4 File Offset: 0x001205F4
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060036F9 RID: 14073 RVA: 0x00122477 File Offset: 0x00120677
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.HasAtmosphereMethodEvent left, AtmosphereSystem.HasAtmosphereMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060036FA RID: 14074 RVA: 0x00122483 File Offset: 0x00120683
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.HasAtmosphereMethodEvent left, AtmosphereSystem.HasAtmosphereMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060036FB RID: 14075 RVA: 0x0012248D File Offset: 0x0012068D
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060036FC RID: 14076 RVA: 0x001224CD File Offset: 0x001206CD
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.HasAtmosphereMethodEvent && this.Equals((AtmosphereSystem.HasAtmosphereMethodEvent)obj);
			}

			// Token: 0x060036FD RID: 14077 RVA: 0x001224E8 File Offset: 0x001206E8
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.HasAtmosphereMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060036FE RID: 14078 RVA: 0x0012253D File Offset: 0x0012073D
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B15 RID: 2837
		[NullableContext(0)]
		[ByRefEvent]
		private struct SetSimulatedGridMethodEvent : IEquatable<AtmosphereSystem.SetSimulatedGridMethodEvent>
		{
			// Token: 0x060036FF RID: 14079 RVA: 0x0012255B File Offset: 0x0012075B
			public SetSimulatedGridMethodEvent(EntityUid Grid, bool Simulated, bool Handled = false)
			{
				this.Grid = Grid;
				this.Simulated = Simulated;
				this.Handled = Handled;
			}

			// Token: 0x17000874 RID: 2164
			// (get) Token: 0x06003700 RID: 14080 RVA: 0x00122572 File Offset: 0x00120772
			// (set) Token: 0x06003701 RID: 14081 RVA: 0x0012257A File Offset: 0x0012077A
			public EntityUid Grid { readonly get; set; }

			// Token: 0x17000875 RID: 2165
			// (get) Token: 0x06003702 RID: 14082 RVA: 0x00122583 File Offset: 0x00120783
			// (set) Token: 0x06003703 RID: 14083 RVA: 0x0012258B File Offset: 0x0012078B
			public bool Simulated { readonly get; set; }

			// Token: 0x17000876 RID: 2166
			// (get) Token: 0x06003704 RID: 14084 RVA: 0x00122594 File Offset: 0x00120794
			// (set) Token: 0x06003705 RID: 14085 RVA: 0x0012259C File Offset: 0x0012079C
			public bool Handled { readonly get; set; }

			// Token: 0x06003706 RID: 14086 RVA: 0x001225A8 File Offset: 0x001207A8
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("SetSimulatedGridMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003707 RID: 14087 RVA: 0x001225F4 File Offset: 0x001207F4
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Simulated = ");
				builder.Append(this.Simulated.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003708 RID: 14088 RVA: 0x00122677 File Offset: 0x00120877
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.SetSimulatedGridMethodEvent left, AtmosphereSystem.SetSimulatedGridMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003709 RID: 14089 RVA: 0x00122683 File Offset: 0x00120883
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.SetSimulatedGridMethodEvent left, AtmosphereSystem.SetSimulatedGridMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600370A RID: 14090 RVA: 0x0012268D File Offset: 0x0012088D
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Simulated>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600370B RID: 14091 RVA: 0x001226CD File Offset: 0x001208CD
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.SetSimulatedGridMethodEvent && this.Equals((AtmosphereSystem.SetSimulatedGridMethodEvent)obj);
			}

			// Token: 0x0600370C RID: 14092 RVA: 0x001226E8 File Offset: 0x001208E8
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.SetSimulatedGridMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Simulated>k__BackingField, other.<Simulated>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600370D RID: 14093 RVA: 0x0012273D File Offset: 0x0012093D
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out bool Simulated, out bool Handled)
			{
				Grid = this.Grid;
				Simulated = this.Simulated;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B16 RID: 2838
		[NullableContext(0)]
		[ByRefEvent]
		private struct IsSimulatedGridMethodEvent : IEquatable<AtmosphereSystem.IsSimulatedGridMethodEvent>
		{
			// Token: 0x0600370E RID: 14094 RVA: 0x0012275B File Offset: 0x0012095B
			public IsSimulatedGridMethodEvent(EntityUid Grid, bool Simulated = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Simulated = Simulated;
				this.Handled = Handled;
			}

			// Token: 0x17000877 RID: 2167
			// (get) Token: 0x0600370F RID: 14095 RVA: 0x00122772 File Offset: 0x00120972
			// (set) Token: 0x06003710 RID: 14096 RVA: 0x0012277A File Offset: 0x0012097A
			public EntityUid Grid { readonly get; set; }

			// Token: 0x17000878 RID: 2168
			// (get) Token: 0x06003711 RID: 14097 RVA: 0x00122783 File Offset: 0x00120983
			// (set) Token: 0x06003712 RID: 14098 RVA: 0x0012278B File Offset: 0x0012098B
			public bool Simulated { readonly get; set; }

			// Token: 0x17000879 RID: 2169
			// (get) Token: 0x06003713 RID: 14099 RVA: 0x00122794 File Offset: 0x00120994
			// (set) Token: 0x06003714 RID: 14100 RVA: 0x0012279C File Offset: 0x0012099C
			public bool Handled { readonly get; set; }

			// Token: 0x06003715 RID: 14101 RVA: 0x001227A8 File Offset: 0x001209A8
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("IsSimulatedGridMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003716 RID: 14102 RVA: 0x001227F4 File Offset: 0x001209F4
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Simulated = ");
				builder.Append(this.Simulated.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003717 RID: 14103 RVA: 0x00122877 File Offset: 0x00120A77
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.IsSimulatedGridMethodEvent left, AtmosphereSystem.IsSimulatedGridMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003718 RID: 14104 RVA: 0x00122883 File Offset: 0x00120A83
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.IsSimulatedGridMethodEvent left, AtmosphereSystem.IsSimulatedGridMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003719 RID: 14105 RVA: 0x0012288D File Offset: 0x00120A8D
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Simulated>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600371A RID: 14106 RVA: 0x001228CD File Offset: 0x00120ACD
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.IsSimulatedGridMethodEvent && this.Equals((AtmosphereSystem.IsSimulatedGridMethodEvent)obj);
			}

			// Token: 0x0600371B RID: 14107 RVA: 0x001228E8 File Offset: 0x00120AE8
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.IsSimulatedGridMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Simulated>k__BackingField, other.<Simulated>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600371C RID: 14108 RVA: 0x0012293D File Offset: 0x00120B3D
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out bool Simulated, out bool Handled)
			{
				Grid = this.Grid;
				Simulated = this.Simulated;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B17 RID: 2839
		[NullableContext(0)]
		[ByRefEvent]
		private struct GetAllMixturesMethodEvent : IEquatable<AtmosphereSystem.GetAllMixturesMethodEvent>
		{
			// Token: 0x0600371D RID: 14109 RVA: 0x0012295B File Offset: 0x00120B5B
			public GetAllMixturesMethodEvent(EntityUid Grid, bool Excite = false, [Nullable(new byte[]
			{
				2,
				1
			})] IEnumerable<GasMixture> Mixtures = null, bool Handled = false)
			{
				this.Grid = Grid;
				this.Excite = Excite;
				this.Mixtures = Mixtures;
				this.Handled = Handled;
			}

			// Token: 0x1700087A RID: 2170
			// (get) Token: 0x0600371E RID: 14110 RVA: 0x0012297A File Offset: 0x00120B7A
			// (set) Token: 0x0600371F RID: 14111 RVA: 0x00122982 File Offset: 0x00120B82
			public EntityUid Grid { readonly get; set; }

			// Token: 0x1700087B RID: 2171
			// (get) Token: 0x06003720 RID: 14112 RVA: 0x0012298B File Offset: 0x00120B8B
			// (set) Token: 0x06003721 RID: 14113 RVA: 0x00122993 File Offset: 0x00120B93
			public bool Excite { readonly get; set; }

			// Token: 0x1700087C RID: 2172
			// (get) Token: 0x06003722 RID: 14114 RVA: 0x0012299C File Offset: 0x00120B9C
			// (set) Token: 0x06003723 RID: 14115 RVA: 0x001229A4 File Offset: 0x00120BA4
			[Nullable(new byte[]
			{
				2,
				1
			})]
			public IEnumerable<GasMixture> Mixtures { [return: Nullable(new byte[]
			{
				2,
				1
			})] readonly get; [param: Nullable(new byte[]
			{
				2,
				1
			})] set; }

			// Token: 0x1700087D RID: 2173
			// (get) Token: 0x06003724 RID: 14116 RVA: 0x001229AD File Offset: 0x00120BAD
			// (set) Token: 0x06003725 RID: 14117 RVA: 0x001229B5 File Offset: 0x00120BB5
			public bool Handled { readonly get; set; }

			// Token: 0x06003726 RID: 14118 RVA: 0x001229C0 File Offset: 0x00120BC0
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GetAllMixturesMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003727 RID: 14119 RVA: 0x00122A0C File Offset: 0x00120C0C
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Excite = ");
				builder.Append(this.Excite.ToString());
				builder.Append(", Mixtures = ");
				builder.Append(this.Mixtures);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003728 RID: 14120 RVA: 0x00122AA8 File Offset: 0x00120CA8
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.GetAllMixturesMethodEvent left, AtmosphereSystem.GetAllMixturesMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003729 RID: 14121 RVA: 0x00122AB4 File Offset: 0x00120CB4
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.GetAllMixturesMethodEvent left, AtmosphereSystem.GetAllMixturesMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600372A RID: 14122 RVA: 0x00122AC0 File Offset: 0x00120CC0
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Excite>k__BackingField)) * -1521134295 + EqualityComparer<IEnumerable<GasMixture>>.Default.GetHashCode(this.<Mixtures>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600372B RID: 14123 RVA: 0x00122B22 File Offset: 0x00120D22
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.GetAllMixturesMethodEvent && this.Equals((AtmosphereSystem.GetAllMixturesMethodEvent)obj);
			}

			// Token: 0x0600372C RID: 14124 RVA: 0x00122B3C File Offset: 0x00120D3C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.GetAllMixturesMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Excite>k__BackingField, other.<Excite>k__BackingField) && EqualityComparer<IEnumerable<GasMixture>>.Default.Equals(this.<Mixtures>k__BackingField, other.<Mixtures>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600372D RID: 14125 RVA: 0x00122BA9 File Offset: 0x00120DA9
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out bool Excite, [Nullable(new byte[]
			{
				2,
				1
			})] out IEnumerable<GasMixture> Mixtures, out bool Handled)
			{
				Grid = this.Grid;
				Excite = this.Excite;
				Mixtures = this.Mixtures;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B18 RID: 2840
		[NullableContext(0)]
		[ByRefEvent]
		private struct InvalidateTileMethodEvent : IEquatable<AtmosphereSystem.InvalidateTileMethodEvent>
		{
			// Token: 0x0600372E RID: 14126 RVA: 0x00122BD0 File Offset: 0x00120DD0
			public InvalidateTileMethodEvent(EntityUid Grid, Vector2i Tile, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Handled = Handled;
			}

			// Token: 0x1700087E RID: 2174
			// (get) Token: 0x0600372F RID: 14127 RVA: 0x00122BE7 File Offset: 0x00120DE7
			// (set) Token: 0x06003730 RID: 14128 RVA: 0x00122BEF File Offset: 0x00120DEF
			public EntityUid Grid { readonly get; set; }

			// Token: 0x1700087F RID: 2175
			// (get) Token: 0x06003731 RID: 14129 RVA: 0x00122BF8 File Offset: 0x00120DF8
			// (set) Token: 0x06003732 RID: 14130 RVA: 0x00122C00 File Offset: 0x00120E00
			public Vector2i Tile { readonly get; set; }

			// Token: 0x17000880 RID: 2176
			// (get) Token: 0x06003733 RID: 14131 RVA: 0x00122C09 File Offset: 0x00120E09
			// (set) Token: 0x06003734 RID: 14132 RVA: 0x00122C11 File Offset: 0x00120E11
			public bool Handled { readonly get; set; }

			// Token: 0x06003735 RID: 14133 RVA: 0x00122C1C File Offset: 0x00120E1C
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("InvalidateTileMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003736 RID: 14134 RVA: 0x00122C68 File Offset: 0x00120E68
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003737 RID: 14135 RVA: 0x00122CEB File Offset: 0x00120EEB
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.InvalidateTileMethodEvent left, AtmosphereSystem.InvalidateTileMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003738 RID: 14136 RVA: 0x00122CF7 File Offset: 0x00120EF7
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.InvalidateTileMethodEvent left, AtmosphereSystem.InvalidateTileMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003739 RID: 14137 RVA: 0x00122D01 File Offset: 0x00120F01
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600373A RID: 14138 RVA: 0x00122D41 File Offset: 0x00120F41
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.InvalidateTileMethodEvent && this.Equals((AtmosphereSystem.InvalidateTileMethodEvent)obj);
			}

			// Token: 0x0600373B RID: 14139 RVA: 0x00122D5C File Offset: 0x00120F5C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.InvalidateTileMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600373C RID: 14140 RVA: 0x00122DB1 File Offset: 0x00120FB1
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B19 RID: 2841
		[Nullable(0)]
		[ByRefEvent]
		private struct GetTileMixturesMethodEvent : IEquatable<AtmosphereSystem.GetTileMixturesMethodEvent>
		{
			// Token: 0x0600373D RID: 14141 RVA: 0x00122DD3 File Offset: 0x00120FD3
			public GetTileMixturesMethodEvent(EntityUid? GridUid, EntityUid? MapUid, List<Vector2i> Tiles, bool Excite = false, [Nullable(2)] GasMixture[] Mixtures = null, bool Handled = false)
			{
				this.GridUid = GridUid;
				this.MapUid = MapUid;
				this.Tiles = Tiles;
				this.Excite = Excite;
				this.Mixtures = Mixtures;
				this.Handled = Handled;
			}

			// Token: 0x17000881 RID: 2177
			// (get) Token: 0x0600373E RID: 14142 RVA: 0x00122E02 File Offset: 0x00121002
			// (set) Token: 0x0600373F RID: 14143 RVA: 0x00122E0A File Offset: 0x0012100A
			public EntityUid? GridUid { readonly get; set; }

			// Token: 0x17000882 RID: 2178
			// (get) Token: 0x06003740 RID: 14144 RVA: 0x00122E13 File Offset: 0x00121013
			// (set) Token: 0x06003741 RID: 14145 RVA: 0x00122E1B File Offset: 0x0012101B
			public EntityUid? MapUid { readonly get; set; }

			// Token: 0x17000883 RID: 2179
			// (get) Token: 0x06003742 RID: 14146 RVA: 0x00122E24 File Offset: 0x00121024
			// (set) Token: 0x06003743 RID: 14147 RVA: 0x00122E2C File Offset: 0x0012102C
			public List<Vector2i> Tiles { readonly get; set; }

			// Token: 0x17000884 RID: 2180
			// (get) Token: 0x06003744 RID: 14148 RVA: 0x00122E35 File Offset: 0x00121035
			// (set) Token: 0x06003745 RID: 14149 RVA: 0x00122E3D File Offset: 0x0012103D
			public bool Excite { readonly get; set; }

			// Token: 0x17000885 RID: 2181
			// (get) Token: 0x06003746 RID: 14150 RVA: 0x00122E46 File Offset: 0x00121046
			// (set) Token: 0x06003747 RID: 14151 RVA: 0x00122E4E File Offset: 0x0012104E
			[Nullable(2)]
			public GasMixture[] Mixtures { [NullableContext(2)] readonly get; [NullableContext(2)] set; }

			// Token: 0x17000886 RID: 2182
			// (get) Token: 0x06003748 RID: 14152 RVA: 0x00122E57 File Offset: 0x00121057
			// (set) Token: 0x06003749 RID: 14153 RVA: 0x00122E5F File Offset: 0x0012105F
			public bool Handled { readonly get; set; }

			// Token: 0x0600374A RID: 14154 RVA: 0x00122E68 File Offset: 0x00121068
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GetTileMixturesMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x0600374B RID: 14155 RVA: 0x00122EB4 File Offset: 0x001210B4
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("GridUid = ");
				builder.Append(this.GridUid.ToString());
				builder.Append(", MapUid = ");
				builder.Append(this.MapUid.ToString());
				builder.Append(", Tiles = ");
				builder.Append(this.Tiles);
				builder.Append(", Excite = ");
				builder.Append(this.Excite.ToString());
				builder.Append(", Mixtures = ");
				builder.Append(this.Mixtures);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x0600374C RID: 14156 RVA: 0x00122F90 File Offset: 0x00121190
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.GetTileMixturesMethodEvent left, AtmosphereSystem.GetTileMixturesMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x0600374D RID: 14157 RVA: 0x00122F9C File Offset: 0x0012119C
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.GetTileMixturesMethodEvent left, AtmosphereSystem.GetTileMixturesMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600374E RID: 14158 RVA: 0x00122FA8 File Offset: 0x001211A8
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((((EqualityComparer<EntityUid?>.Default.GetHashCode(this.<GridUid>k__BackingField) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<MapUid>k__BackingField)) * -1521134295 + EqualityComparer<List<Vector2i>>.Default.GetHashCode(this.<Tiles>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Excite>k__BackingField)) * -1521134295 + EqualityComparer<GasMixture[]>.Default.GetHashCode(this.<Mixtures>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600374F RID: 14159 RVA: 0x00123038 File Offset: 0x00121238
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.GetTileMixturesMethodEvent && this.Equals((AtmosphereSystem.GetTileMixturesMethodEvent)obj);
			}

			// Token: 0x06003750 RID: 14160 RVA: 0x00123050 File Offset: 0x00121250
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.GetTileMixturesMethodEvent other)
			{
				return EqualityComparer<EntityUid?>.Default.Equals(this.<GridUid>k__BackingField, other.<GridUid>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<MapUid>k__BackingField, other.<MapUid>k__BackingField) && EqualityComparer<List<Vector2i>>.Default.Equals(this.<Tiles>k__BackingField, other.<Tiles>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Excite>k__BackingField, other.<Excite>k__BackingField) && EqualityComparer<GasMixture[]>.Default.Equals(this.<Mixtures>k__BackingField, other.<Mixtures>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x06003751 RID: 14161 RVA: 0x001230ED File Offset: 0x001212ED
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid? GridUid, out EntityUid? MapUid, out List<Vector2i> Tiles, out bool Excite, [Nullable(2)] out GasMixture[] Mixtures, out bool Handled)
			{
				GridUid = this.GridUid;
				MapUid = this.MapUid;
				Tiles = this.Tiles;
				Excite = this.Excite;
				Mixtures = this.Mixtures;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B1A RID: 2842
		[NullableContext(2)]
		[Nullable(0)]
		[ByRefEvent]
		private struct GetTileMixtureMethodEvent : IEquatable<AtmosphereSystem.GetTileMixtureMethodEvent>
		{
			// Token: 0x06003752 RID: 14162 RVA: 0x0012312A File Offset: 0x0012132A
			public GetTileMixtureMethodEvent(EntityUid? GridUid, EntityUid? MapUid, Vector2i Tile, bool Excite = false, GasMixture Mixture = null, bool Handled = false)
			{
				this.GridUid = GridUid;
				this.MapUid = MapUid;
				this.Tile = Tile;
				this.Excite = Excite;
				this.Mixture = Mixture;
				this.Handled = Handled;
			}

			// Token: 0x17000887 RID: 2183
			// (get) Token: 0x06003753 RID: 14163 RVA: 0x00123159 File Offset: 0x00121359
			// (set) Token: 0x06003754 RID: 14164 RVA: 0x00123161 File Offset: 0x00121361
			public EntityUid? GridUid { readonly get; set; }

			// Token: 0x17000888 RID: 2184
			// (get) Token: 0x06003755 RID: 14165 RVA: 0x0012316A File Offset: 0x0012136A
			// (set) Token: 0x06003756 RID: 14166 RVA: 0x00123172 File Offset: 0x00121372
			public EntityUid? MapUid { readonly get; set; }

			// Token: 0x17000889 RID: 2185
			// (get) Token: 0x06003757 RID: 14167 RVA: 0x0012317B File Offset: 0x0012137B
			// (set) Token: 0x06003758 RID: 14168 RVA: 0x00123183 File Offset: 0x00121383
			public Vector2i Tile { readonly get; set; }

			// Token: 0x1700088A RID: 2186
			// (get) Token: 0x06003759 RID: 14169 RVA: 0x0012318C File Offset: 0x0012138C
			// (set) Token: 0x0600375A RID: 14170 RVA: 0x00123194 File Offset: 0x00121394
			public bool Excite { readonly get; set; }

			// Token: 0x1700088B RID: 2187
			// (get) Token: 0x0600375B RID: 14171 RVA: 0x0012319D File Offset: 0x0012139D
			// (set) Token: 0x0600375C RID: 14172 RVA: 0x001231A5 File Offset: 0x001213A5
			public GasMixture Mixture { readonly get; set; }

			// Token: 0x1700088C RID: 2188
			// (get) Token: 0x0600375D RID: 14173 RVA: 0x001231AE File Offset: 0x001213AE
			// (set) Token: 0x0600375E RID: 14174 RVA: 0x001231B6 File Offset: 0x001213B6
			public bool Handled { readonly get; set; }

			// Token: 0x0600375F RID: 14175 RVA: 0x001231C0 File Offset: 0x001213C0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GetTileMixtureMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003760 RID: 14176 RVA: 0x0012320C File Offset: 0x0012140C
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("GridUid = ");
				builder.Append(this.GridUid.ToString());
				builder.Append(", MapUid = ");
				builder.Append(this.MapUid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Excite = ");
				builder.Append(this.Excite.ToString());
				builder.Append(", Mixture = ");
				builder.Append(this.Mixture);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003761 RID: 14177 RVA: 0x001232F6 File Offset: 0x001214F6
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.GetTileMixtureMethodEvent left, AtmosphereSystem.GetTileMixtureMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003762 RID: 14178 RVA: 0x00123302 File Offset: 0x00121502
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.GetTileMixtureMethodEvent left, AtmosphereSystem.GetTileMixtureMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003763 RID: 14179 RVA: 0x0012330C File Offset: 0x0012150C
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((((EqualityComparer<EntityUid?>.Default.GetHashCode(this.<GridUid>k__BackingField) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<MapUid>k__BackingField)) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Excite>k__BackingField)) * -1521134295 + EqualityComparer<GasMixture>.Default.GetHashCode(this.<Mixture>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x06003764 RID: 14180 RVA: 0x0012339C File Offset: 0x0012159C
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.GetTileMixtureMethodEvent && this.Equals((AtmosphereSystem.GetTileMixtureMethodEvent)obj);
			}

			// Token: 0x06003765 RID: 14181 RVA: 0x001233B4 File Offset: 0x001215B4
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.GetTileMixtureMethodEvent other)
			{
				return EqualityComparer<EntityUid?>.Default.Equals(this.<GridUid>k__BackingField, other.<GridUid>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<MapUid>k__BackingField, other.<MapUid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Excite>k__BackingField, other.<Excite>k__BackingField) && EqualityComparer<GasMixture>.Default.Equals(this.<Mixture>k__BackingField, other.<Mixture>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x06003766 RID: 14182 RVA: 0x00123454 File Offset: 0x00121654
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid? GridUid, out EntityUid? MapUid, out Vector2i Tile, out bool Excite, out GasMixture Mixture, out bool Handled)
			{
				GridUid = this.GridUid;
				MapUid = this.MapUid;
				Tile = this.Tile;
				Excite = this.Excite;
				Mixture = this.Mixture;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B1B RID: 2843
		[NullableContext(0)]
		[ByRefEvent]
		private struct ReactTileMethodEvent : IEquatable<AtmosphereSystem.ReactTileMethodEvent>
		{
			// Token: 0x06003767 RID: 14183 RVA: 0x001234A0 File Offset: 0x001216A0
			public ReactTileMethodEvent(EntityUid GridId, Vector2i Tile, ReactionResult Result = ReactionResult.NoReaction, bool Handled = false)
			{
				this.GridId = GridId;
				this.Tile = Tile;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x1700088D RID: 2189
			// (get) Token: 0x06003768 RID: 14184 RVA: 0x001234BF File Offset: 0x001216BF
			// (set) Token: 0x06003769 RID: 14185 RVA: 0x001234C7 File Offset: 0x001216C7
			public EntityUid GridId { readonly get; set; }

			// Token: 0x1700088E RID: 2190
			// (get) Token: 0x0600376A RID: 14186 RVA: 0x001234D0 File Offset: 0x001216D0
			// (set) Token: 0x0600376B RID: 14187 RVA: 0x001234D8 File Offset: 0x001216D8
			public Vector2i Tile { readonly get; set; }

			// Token: 0x1700088F RID: 2191
			// (get) Token: 0x0600376C RID: 14188 RVA: 0x001234E1 File Offset: 0x001216E1
			// (set) Token: 0x0600376D RID: 14189 RVA: 0x001234E9 File Offset: 0x001216E9
			public ReactionResult Result { readonly get; set; }

			// Token: 0x17000890 RID: 2192
			// (get) Token: 0x0600376E RID: 14190 RVA: 0x001234F2 File Offset: 0x001216F2
			// (set) Token: 0x0600376F RID: 14191 RVA: 0x001234FA File Offset: 0x001216FA
			public bool Handled { readonly get; set; }

			// Token: 0x06003770 RID: 14192 RVA: 0x00123504 File Offset: 0x00121704
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("ReactTileMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003771 RID: 14193 RVA: 0x00123550 File Offset: 0x00121750
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("GridId = ");
				builder.Append(this.GridId.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003772 RID: 14194 RVA: 0x001235FA File Offset: 0x001217FA
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.ReactTileMethodEvent left, AtmosphereSystem.ReactTileMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003773 RID: 14195 RVA: 0x00123606 File Offset: 0x00121806
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.ReactTileMethodEvent left, AtmosphereSystem.ReactTileMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003774 RID: 14196 RVA: 0x00123610 File Offset: 0x00121810
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<GridId>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<ReactionResult>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x06003775 RID: 14197 RVA: 0x00123672 File Offset: 0x00121872
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.ReactTileMethodEvent && this.Equals((AtmosphereSystem.ReactTileMethodEvent)obj);
			}

			// Token: 0x06003776 RID: 14198 RVA: 0x0012368C File Offset: 0x0012188C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.ReactTileMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<GridId>k__BackingField, other.<GridId>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<ReactionResult>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x06003777 RID: 14199 RVA: 0x001236F9 File Offset: 0x001218F9
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid GridId, out Vector2i Tile, out ReactionResult Result, out bool Handled)
			{
				GridId = this.GridId;
				Tile = this.Tile;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B1C RID: 2844
		[NullableContext(2)]
		[Nullable(0)]
		[ByRefEvent]
		private struct IsTileAirBlockedMethodEvent : IEquatable<AtmosphereSystem.IsTileAirBlockedMethodEvent>
		{
			// Token: 0x06003778 RID: 14200 RVA: 0x00123724 File Offset: 0x00121924
			public IsTileAirBlockedMethodEvent(EntityUid Grid, Vector2i Tile, AtmosDirection Direction = AtmosDirection.All, MapGridComponent MapGridComponent = null, bool Result = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Direction = Direction;
				this.MapGridComponent = MapGridComponent;
				this.Result = Result;
				this.Handled = Handled;
				this.NoAir = false;
			}

			// Token: 0x17000891 RID: 2193
			// (get) Token: 0x06003779 RID: 14201 RVA: 0x0012375A File Offset: 0x0012195A
			// (set) Token: 0x0600377A RID: 14202 RVA: 0x00123762 File Offset: 0x00121962
			public EntityUid Grid { readonly get; set; }

			// Token: 0x17000892 RID: 2194
			// (get) Token: 0x0600377B RID: 14203 RVA: 0x0012376B File Offset: 0x0012196B
			// (set) Token: 0x0600377C RID: 14204 RVA: 0x00123773 File Offset: 0x00121973
			public Vector2i Tile { readonly get; set; }

			// Token: 0x17000893 RID: 2195
			// (get) Token: 0x0600377D RID: 14205 RVA: 0x0012377C File Offset: 0x0012197C
			// (set) Token: 0x0600377E RID: 14206 RVA: 0x00123784 File Offset: 0x00121984
			public AtmosDirection Direction { readonly get; set; }

			// Token: 0x17000894 RID: 2196
			// (get) Token: 0x0600377F RID: 14207 RVA: 0x0012378D File Offset: 0x0012198D
			// (set) Token: 0x06003780 RID: 14208 RVA: 0x00123795 File Offset: 0x00121995
			public MapGridComponent MapGridComponent { readonly get; set; }

			// Token: 0x17000895 RID: 2197
			// (get) Token: 0x06003781 RID: 14209 RVA: 0x0012379E File Offset: 0x0012199E
			// (set) Token: 0x06003782 RID: 14210 RVA: 0x001237A6 File Offset: 0x001219A6
			public bool Result { readonly get; set; }

			// Token: 0x17000896 RID: 2198
			// (get) Token: 0x06003783 RID: 14211 RVA: 0x001237AF File Offset: 0x001219AF
			// (set) Token: 0x06003784 RID: 14212 RVA: 0x001237B7 File Offset: 0x001219B7
			public bool Handled { readonly get; set; }

			// Token: 0x06003785 RID: 14213 RVA: 0x001237C0 File Offset: 0x001219C0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("IsTileAirBlockedMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003786 RID: 14214 RVA: 0x0012380C File Offset: 0x00121A0C
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Direction = ");
				builder.Append(this.Direction.ToString());
				builder.Append(", MapGridComponent = ");
				builder.Append(this.MapGridComponent);
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				builder.Append(", NoAir = ");
				builder.Append(this.NoAir.ToString());
				return true;
			}

			// Token: 0x06003787 RID: 14215 RVA: 0x0012391A File Offset: 0x00121B1A
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.IsTileAirBlockedMethodEvent left, AtmosphereSystem.IsTileAirBlockedMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003788 RID: 14216 RVA: 0x00123926 File Offset: 0x00121B26
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.IsTileAirBlockedMethodEvent left, AtmosphereSystem.IsTileAirBlockedMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003789 RID: 14217 RVA: 0x00123930 File Offset: 0x00121B30
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (((((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<AtmosDirection>.Default.GetHashCode(this.<Direction>k__BackingField)) * -1521134295 + EqualityComparer<MapGridComponent>.Default.GetHashCode(this.<MapGridComponent>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.NoAir);
			}

			// Token: 0x0600378A RID: 14218 RVA: 0x001239D7 File Offset: 0x00121BD7
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.IsTileAirBlockedMethodEvent && this.Equals((AtmosphereSystem.IsTileAirBlockedMethodEvent)obj);
			}

			// Token: 0x0600378B RID: 14219 RVA: 0x001239F0 File Offset: 0x00121BF0
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.IsTileAirBlockedMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<AtmosDirection>.Default.Equals(this.<Direction>k__BackingField, other.<Direction>k__BackingField) && EqualityComparer<MapGridComponent>.Default.Equals(this.<MapGridComponent>k__BackingField, other.<MapGridComponent>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.NoAir, other.NoAir);
			}

			// Token: 0x0600378C RID: 14220 RVA: 0x00123AA8 File Offset: 0x00121CA8
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out AtmosDirection Direction, out MapGridComponent MapGridComponent, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Direction = this.Direction;
				MapGridComponent = this.MapGridComponent;
				Result = this.Result;
				Handled = this.Handled;
			}

			// Token: 0x04002908 RID: 10504
			public bool NoAir;
		}

		// Token: 0x02000B1D RID: 2845
		[NullableContext(2)]
		[Nullable(0)]
		[ByRefEvent]
		private struct IsTileSpaceMethodEvent : IEquatable<AtmosphereSystem.IsTileSpaceMethodEvent>
		{
			// Token: 0x0600378D RID: 14221 RVA: 0x00123AE5 File Offset: 0x00121CE5
			public IsTileSpaceMethodEvent(EntityUid? Grid, EntityUid? Map, Vector2i Tile, MapGridComponent MapGridComponent = null, bool Result = true, bool Handled = false)
			{
				this.Grid = Grid;
				this.Map = Map;
				this.Tile = Tile;
				this.MapGridComponent = MapGridComponent;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x17000897 RID: 2199
			// (get) Token: 0x0600378E RID: 14222 RVA: 0x00123B14 File Offset: 0x00121D14
			// (set) Token: 0x0600378F RID: 14223 RVA: 0x00123B1C File Offset: 0x00121D1C
			public EntityUid? Grid { readonly get; set; }

			// Token: 0x17000898 RID: 2200
			// (get) Token: 0x06003790 RID: 14224 RVA: 0x00123B25 File Offset: 0x00121D25
			// (set) Token: 0x06003791 RID: 14225 RVA: 0x00123B2D File Offset: 0x00121D2D
			public EntityUid? Map { readonly get; set; }

			// Token: 0x17000899 RID: 2201
			// (get) Token: 0x06003792 RID: 14226 RVA: 0x00123B36 File Offset: 0x00121D36
			// (set) Token: 0x06003793 RID: 14227 RVA: 0x00123B3E File Offset: 0x00121D3E
			public Vector2i Tile { readonly get; set; }

			// Token: 0x1700089A RID: 2202
			// (get) Token: 0x06003794 RID: 14228 RVA: 0x00123B47 File Offset: 0x00121D47
			// (set) Token: 0x06003795 RID: 14229 RVA: 0x00123B4F File Offset: 0x00121D4F
			public MapGridComponent MapGridComponent { readonly get; set; }

			// Token: 0x1700089B RID: 2203
			// (get) Token: 0x06003796 RID: 14230 RVA: 0x00123B58 File Offset: 0x00121D58
			// (set) Token: 0x06003797 RID: 14231 RVA: 0x00123B60 File Offset: 0x00121D60
			public bool Result { readonly get; set; }

			// Token: 0x1700089C RID: 2204
			// (get) Token: 0x06003798 RID: 14232 RVA: 0x00123B69 File Offset: 0x00121D69
			// (set) Token: 0x06003799 RID: 14233 RVA: 0x00123B71 File Offset: 0x00121D71
			public bool Handled { readonly get; set; }

			// Token: 0x0600379A RID: 14234 RVA: 0x00123B7C File Offset: 0x00121D7C
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("IsTileSpaceMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x0600379B RID: 14235 RVA: 0x00123BC8 File Offset: 0x00121DC8
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Map = ");
				builder.Append(this.Map.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", MapGridComponent = ");
				builder.Append(this.MapGridComponent);
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x0600379C RID: 14236 RVA: 0x00123CB2 File Offset: 0x00121EB2
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.IsTileSpaceMethodEvent left, AtmosphereSystem.IsTileSpaceMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x0600379D RID: 14237 RVA: 0x00123CBE File Offset: 0x00121EBE
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.IsTileSpaceMethodEvent left, AtmosphereSystem.IsTileSpaceMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600379E RID: 14238 RVA: 0x00123CC8 File Offset: 0x00121EC8
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((((EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.<Map>k__BackingField)) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<MapGridComponent>.Default.GetHashCode(this.<MapGridComponent>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600379F RID: 14239 RVA: 0x00123D58 File Offset: 0x00121F58
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.IsTileSpaceMethodEvent && this.Equals((AtmosphereSystem.IsTileSpaceMethodEvent)obj);
			}

			// Token: 0x060037A0 RID: 14240 RVA: 0x00123D70 File Offset: 0x00121F70
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.IsTileSpaceMethodEvent other)
			{
				return EqualityComparer<EntityUid?>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<EntityUid?>.Default.Equals(this.<Map>k__BackingField, other.<Map>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<MapGridComponent>.Default.Equals(this.<MapGridComponent>k__BackingField, other.<MapGridComponent>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037A1 RID: 14241 RVA: 0x00123E10 File Offset: 0x00122010
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid? Grid, out EntityUid? Map, out Vector2i Tile, out MapGridComponent MapGridComponent, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Map = this.Map;
				Tile = this.Tile;
				MapGridComponent = this.MapGridComponent;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B1E RID: 2846
		[NullableContext(2)]
		[Nullable(0)]
		[ByRefEvent]
		private struct GetAdjacentTilesMethodEvent : IEquatable<AtmosphereSystem.GetAdjacentTilesMethodEvent>
		{
			// Token: 0x060037A2 RID: 14242 RVA: 0x00123E5C File Offset: 0x0012205C
			public GetAdjacentTilesMethodEvent(EntityUid Grid, Vector2i Tile, IEnumerable<Vector2i> Result = null, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x1700089D RID: 2205
			// (get) Token: 0x060037A3 RID: 14243 RVA: 0x00123E7B File Offset: 0x0012207B
			// (set) Token: 0x060037A4 RID: 14244 RVA: 0x00123E83 File Offset: 0x00122083
			public EntityUid Grid { readonly get; set; }

			// Token: 0x1700089E RID: 2206
			// (get) Token: 0x060037A5 RID: 14245 RVA: 0x00123E8C File Offset: 0x0012208C
			// (set) Token: 0x060037A6 RID: 14246 RVA: 0x00123E94 File Offset: 0x00122094
			public Vector2i Tile { readonly get; set; }

			// Token: 0x1700089F RID: 2207
			// (get) Token: 0x060037A7 RID: 14247 RVA: 0x00123E9D File Offset: 0x0012209D
			// (set) Token: 0x060037A8 RID: 14248 RVA: 0x00123EA5 File Offset: 0x001220A5
			public IEnumerable<Vector2i> Result { readonly get; set; }

			// Token: 0x170008A0 RID: 2208
			// (get) Token: 0x060037A9 RID: 14249 RVA: 0x00123EAE File Offset: 0x001220AE
			// (set) Token: 0x060037AA RID: 14250 RVA: 0x00123EB6 File Offset: 0x001220B6
			public bool Handled { readonly get; set; }

			// Token: 0x060037AB RID: 14251 RVA: 0x00123EC0 File Offset: 0x001220C0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GetAdjacentTilesMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060037AC RID: 14252 RVA: 0x00123F0C File Offset: 0x0012210C
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Result = ");
				builder.Append(this.Result);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060037AD RID: 14253 RVA: 0x00123FA8 File Offset: 0x001221A8
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.GetAdjacentTilesMethodEvent left, AtmosphereSystem.GetAdjacentTilesMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060037AE RID: 14254 RVA: 0x00123FB4 File Offset: 0x001221B4
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.GetAdjacentTilesMethodEvent left, AtmosphereSystem.GetAdjacentTilesMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060037AF RID: 14255 RVA: 0x00123FC0 File Offset: 0x001221C0
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<IEnumerable<Vector2i>>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060037B0 RID: 14256 RVA: 0x00124022 File Offset: 0x00122222
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.GetAdjacentTilesMethodEvent && this.Equals((AtmosphereSystem.GetAdjacentTilesMethodEvent)obj);
			}

			// Token: 0x060037B1 RID: 14257 RVA: 0x0012403C File Offset: 0x0012223C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.GetAdjacentTilesMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<IEnumerable<Vector2i>>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037B2 RID: 14258 RVA: 0x001240A9 File Offset: 0x001222A9
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out IEnumerable<Vector2i> Result, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B1F RID: 2847
		[NullableContext(0)]
		[ByRefEvent]
		private struct GetAdjacentTileMixturesMethodEvent : IEquatable<AtmosphereSystem.GetAdjacentTileMixturesMethodEvent>
		{
			// Token: 0x060037B3 RID: 14259 RVA: 0x001240D4 File Offset: 0x001222D4
			public GetAdjacentTileMixturesMethodEvent(EntityUid Grid, Vector2i Tile, bool IncludeBlocked, bool Excite, [Nullable(new byte[]
			{
				2,
				1
			})] IEnumerable<GasMixture> Result = null, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.IncludeBlocked = IncludeBlocked;
				this.Excite = Excite;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x170008A1 RID: 2209
			// (get) Token: 0x060037B4 RID: 14260 RVA: 0x00124103 File Offset: 0x00122303
			// (set) Token: 0x060037B5 RID: 14261 RVA: 0x0012410B File Offset: 0x0012230B
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008A2 RID: 2210
			// (get) Token: 0x060037B6 RID: 14262 RVA: 0x00124114 File Offset: 0x00122314
			// (set) Token: 0x060037B7 RID: 14263 RVA: 0x0012411C File Offset: 0x0012231C
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008A3 RID: 2211
			// (get) Token: 0x060037B8 RID: 14264 RVA: 0x00124125 File Offset: 0x00122325
			// (set) Token: 0x060037B9 RID: 14265 RVA: 0x0012412D File Offset: 0x0012232D
			public bool IncludeBlocked { readonly get; set; }

			// Token: 0x170008A4 RID: 2212
			// (get) Token: 0x060037BA RID: 14266 RVA: 0x00124136 File Offset: 0x00122336
			// (set) Token: 0x060037BB RID: 14267 RVA: 0x0012413E File Offset: 0x0012233E
			public bool Excite { readonly get; set; }

			// Token: 0x170008A5 RID: 2213
			// (get) Token: 0x060037BC RID: 14268 RVA: 0x00124147 File Offset: 0x00122347
			// (set) Token: 0x060037BD RID: 14269 RVA: 0x0012414F File Offset: 0x0012234F
			[Nullable(new byte[]
			{
				2,
				1
			})]
			public IEnumerable<GasMixture> Result { [return: Nullable(new byte[]
			{
				2,
				1
			})] readonly get; [param: Nullable(new byte[]
			{
				2,
				1
			})] set; }

			// Token: 0x170008A6 RID: 2214
			// (get) Token: 0x060037BE RID: 14270 RVA: 0x00124158 File Offset: 0x00122358
			// (set) Token: 0x060037BF RID: 14271 RVA: 0x00124160 File Offset: 0x00122360
			public bool Handled { readonly get; set; }

			// Token: 0x060037C0 RID: 14272 RVA: 0x0012416C File Offset: 0x0012236C
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GetAdjacentTileMixturesMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060037C1 RID: 14273 RVA: 0x001241B8 File Offset: 0x001223B8
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", IncludeBlocked = ");
				builder.Append(this.IncludeBlocked.ToString());
				builder.Append(", Excite = ");
				builder.Append(this.Excite.ToString());
				builder.Append(", Result = ");
				builder.Append(this.Result);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060037C2 RID: 14274 RVA: 0x001242A2 File Offset: 0x001224A2
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.GetAdjacentTileMixturesMethodEvent left, AtmosphereSystem.GetAdjacentTileMixturesMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060037C3 RID: 14275 RVA: 0x001242AE File Offset: 0x001224AE
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.GetAdjacentTileMixturesMethodEvent left, AtmosphereSystem.GetAdjacentTileMixturesMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060037C4 RID: 14276 RVA: 0x001242B8 File Offset: 0x001224B8
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<IncludeBlocked>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Excite>k__BackingField)) * -1521134295 + EqualityComparer<IEnumerable<GasMixture>>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060037C5 RID: 14277 RVA: 0x00124348 File Offset: 0x00122548
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.GetAdjacentTileMixturesMethodEvent && this.Equals((AtmosphereSystem.GetAdjacentTileMixturesMethodEvent)obj);
			}

			// Token: 0x060037C6 RID: 14278 RVA: 0x00124360 File Offset: 0x00122560
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.GetAdjacentTileMixturesMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<IncludeBlocked>k__BackingField, other.<IncludeBlocked>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Excite>k__BackingField, other.<Excite>k__BackingField) && EqualityComparer<IEnumerable<GasMixture>>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037C7 RID: 14279 RVA: 0x001243FD File Offset: 0x001225FD
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out bool IncludeBlocked, out bool Excite, [Nullable(new byte[]
			{
				2,
				1
			})] out IEnumerable<GasMixture> Result, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				IncludeBlocked = this.IncludeBlocked;
				Excite = this.Excite;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B20 RID: 2848
		[NullableContext(2)]
		[Nullable(0)]
		[ByRefEvent]
		private struct UpdateAdjacentMethodEvent : IEquatable<AtmosphereSystem.UpdateAdjacentMethodEvent>
		{
			// Token: 0x060037C8 RID: 14280 RVA: 0x0012443A File Offset: 0x0012263A
			public UpdateAdjacentMethodEvent(EntityUid Grid, Vector2i Tile, MapGridComponent MapGridComponent = null, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.MapGridComponent = MapGridComponent;
				this.Handled = Handled;
			}

			// Token: 0x170008A7 RID: 2215
			// (get) Token: 0x060037C9 RID: 14281 RVA: 0x00124459 File Offset: 0x00122659
			// (set) Token: 0x060037CA RID: 14282 RVA: 0x00124461 File Offset: 0x00122661
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008A8 RID: 2216
			// (get) Token: 0x060037CB RID: 14283 RVA: 0x0012446A File Offset: 0x0012266A
			// (set) Token: 0x060037CC RID: 14284 RVA: 0x00124472 File Offset: 0x00122672
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008A9 RID: 2217
			// (get) Token: 0x060037CD RID: 14285 RVA: 0x0012447B File Offset: 0x0012267B
			// (set) Token: 0x060037CE RID: 14286 RVA: 0x00124483 File Offset: 0x00122683
			public MapGridComponent MapGridComponent { readonly get; set; }

			// Token: 0x170008AA RID: 2218
			// (get) Token: 0x060037CF RID: 14287 RVA: 0x0012448C File Offset: 0x0012268C
			// (set) Token: 0x060037D0 RID: 14288 RVA: 0x00124494 File Offset: 0x00122694
			public bool Handled { readonly get; set; }

			// Token: 0x060037D1 RID: 14289 RVA: 0x001244A0 File Offset: 0x001226A0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("UpdateAdjacentMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060037D2 RID: 14290 RVA: 0x001244EC File Offset: 0x001226EC
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", MapGridComponent = ");
				builder.Append(this.MapGridComponent);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060037D3 RID: 14291 RVA: 0x00124588 File Offset: 0x00122788
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.UpdateAdjacentMethodEvent left, AtmosphereSystem.UpdateAdjacentMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060037D4 RID: 14292 RVA: 0x00124594 File Offset: 0x00122794
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.UpdateAdjacentMethodEvent left, AtmosphereSystem.UpdateAdjacentMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060037D5 RID: 14293 RVA: 0x001245A0 File Offset: 0x001227A0
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<MapGridComponent>.Default.GetHashCode(this.<MapGridComponent>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060037D6 RID: 14294 RVA: 0x00124602 File Offset: 0x00122802
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.UpdateAdjacentMethodEvent && this.Equals((AtmosphereSystem.UpdateAdjacentMethodEvent)obj);
			}

			// Token: 0x060037D7 RID: 14295 RVA: 0x0012461C File Offset: 0x0012281C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.UpdateAdjacentMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<MapGridComponent>.Default.Equals(this.<MapGridComponent>k__BackingField, other.<MapGridComponent>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037D8 RID: 14296 RVA: 0x00124689 File Offset: 0x00122889
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out MapGridComponent MapGridComponent, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				MapGridComponent = this.MapGridComponent;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B21 RID: 2849
		[NullableContext(0)]
		[ByRefEvent]
		private struct HotspotExposeMethodEvent : IEquatable<AtmosphereSystem.HotspotExposeMethodEvent>
		{
			// Token: 0x060037D9 RID: 14297 RVA: 0x001246B4 File Offset: 0x001228B4
			public HotspotExposeMethodEvent(EntityUid Grid, Vector2i Tile, float ExposedTemperature, float ExposedVolume, bool soh, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.ExposedTemperature = ExposedTemperature;
				this.ExposedVolume = ExposedVolume;
				this.soh = soh;
				this.Handled = Handled;
			}

			// Token: 0x170008AB RID: 2219
			// (get) Token: 0x060037DA RID: 14298 RVA: 0x001246E3 File Offset: 0x001228E3
			// (set) Token: 0x060037DB RID: 14299 RVA: 0x001246EB File Offset: 0x001228EB
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008AC RID: 2220
			// (get) Token: 0x060037DC RID: 14300 RVA: 0x001246F4 File Offset: 0x001228F4
			// (set) Token: 0x060037DD RID: 14301 RVA: 0x001246FC File Offset: 0x001228FC
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008AD RID: 2221
			// (get) Token: 0x060037DE RID: 14302 RVA: 0x00124705 File Offset: 0x00122905
			// (set) Token: 0x060037DF RID: 14303 RVA: 0x0012470D File Offset: 0x0012290D
			public float ExposedTemperature { readonly get; set; }

			// Token: 0x170008AE RID: 2222
			// (get) Token: 0x060037E0 RID: 14304 RVA: 0x00124716 File Offset: 0x00122916
			// (set) Token: 0x060037E1 RID: 14305 RVA: 0x0012471E File Offset: 0x0012291E
			public float ExposedVolume { readonly get; set; }

			// Token: 0x170008AF RID: 2223
			// (get) Token: 0x060037E2 RID: 14306 RVA: 0x00124727 File Offset: 0x00122927
			// (set) Token: 0x060037E3 RID: 14307 RVA: 0x0012472F File Offset: 0x0012292F
			public bool soh { readonly get; set; }

			// Token: 0x170008B0 RID: 2224
			// (get) Token: 0x060037E4 RID: 14308 RVA: 0x00124738 File Offset: 0x00122938
			// (set) Token: 0x060037E5 RID: 14309 RVA: 0x00124740 File Offset: 0x00122940
			public bool Handled { readonly get; set; }

			// Token: 0x060037E6 RID: 14310 RVA: 0x0012474C File Offset: 0x0012294C
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("HotspotExposeMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060037E7 RID: 14311 RVA: 0x00124798 File Offset: 0x00122998
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", ExposedTemperature = ");
				builder.Append(this.ExposedTemperature.ToString());
				builder.Append(", ExposedVolume = ");
				builder.Append(this.ExposedVolume.ToString());
				builder.Append(", soh = ");
				builder.Append(this.soh.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060037E8 RID: 14312 RVA: 0x00124890 File Offset: 0x00122A90
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.HotspotExposeMethodEvent left, AtmosphereSystem.HotspotExposeMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060037E9 RID: 14313 RVA: 0x0012489C File Offset: 0x00122A9C
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.HotspotExposeMethodEvent left, AtmosphereSystem.HotspotExposeMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060037EA RID: 14314 RVA: 0x001248A8 File Offset: 0x00122AA8
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<ExposedTemperature>k__BackingField)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.<ExposedVolume>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<soh>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060037EB RID: 14315 RVA: 0x00124938 File Offset: 0x00122B38
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.HotspotExposeMethodEvent && this.Equals((AtmosphereSystem.HotspotExposeMethodEvent)obj);
			}

			// Token: 0x060037EC RID: 14316 RVA: 0x00124950 File Offset: 0x00122B50
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.HotspotExposeMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<ExposedTemperature>k__BackingField, other.<ExposedTemperature>k__BackingField) && EqualityComparer<float>.Default.Equals(this.<ExposedVolume>k__BackingField, other.<ExposedVolume>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<soh>k__BackingField, other.<soh>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037ED RID: 14317 RVA: 0x001249ED File Offset: 0x00122BED
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out float ExposedTemperature, out float ExposedVolume, out bool soh, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				ExposedTemperature = this.ExposedTemperature;
				ExposedVolume = this.ExposedVolume;
				soh = this.soh;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B22 RID: 2850
		[NullableContext(0)]
		[ByRefEvent]
		private struct HotspotExtinguishMethodEvent : IEquatable<AtmosphereSystem.HotspotExtinguishMethodEvent>
		{
			// Token: 0x060037EE RID: 14318 RVA: 0x00124A2A File Offset: 0x00122C2A
			public HotspotExtinguishMethodEvent(EntityUid Grid, Vector2i Tile, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Handled = Handled;
			}

			// Token: 0x170008B1 RID: 2225
			// (get) Token: 0x060037EF RID: 14319 RVA: 0x00124A41 File Offset: 0x00122C41
			// (set) Token: 0x060037F0 RID: 14320 RVA: 0x00124A49 File Offset: 0x00122C49
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008B2 RID: 2226
			// (get) Token: 0x060037F1 RID: 14321 RVA: 0x00124A52 File Offset: 0x00122C52
			// (set) Token: 0x060037F2 RID: 14322 RVA: 0x00124A5A File Offset: 0x00122C5A
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008B3 RID: 2227
			// (get) Token: 0x060037F3 RID: 14323 RVA: 0x00124A63 File Offset: 0x00122C63
			// (set) Token: 0x060037F4 RID: 14324 RVA: 0x00124A6B File Offset: 0x00122C6B
			public bool Handled { readonly get; set; }

			// Token: 0x060037F5 RID: 14325 RVA: 0x00124A74 File Offset: 0x00122C74
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("HotspotExtinguishMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060037F6 RID: 14326 RVA: 0x00124AC0 File Offset: 0x00122CC0
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x060037F7 RID: 14327 RVA: 0x00124B43 File Offset: 0x00122D43
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.HotspotExtinguishMethodEvent left, AtmosphereSystem.HotspotExtinguishMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x060037F8 RID: 14328 RVA: 0x00124B4F File Offset: 0x00122D4F
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.HotspotExtinguishMethodEvent left, AtmosphereSystem.HotspotExtinguishMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x060037F9 RID: 14329 RVA: 0x00124B59 File Offset: 0x00122D59
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x060037FA RID: 14330 RVA: 0x00124B99 File Offset: 0x00122D99
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.HotspotExtinguishMethodEvent && this.Equals((AtmosphereSystem.HotspotExtinguishMethodEvent)obj);
			}

			// Token: 0x060037FB RID: 14331 RVA: 0x00124BB4 File Offset: 0x00122DB4
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.HotspotExtinguishMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x060037FC RID: 14332 RVA: 0x00124C09 File Offset: 0x00122E09
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B23 RID: 2851
		[NullableContext(0)]
		[ByRefEvent]
		private struct IsHotspotActiveMethodEvent : IEquatable<AtmosphereSystem.IsHotspotActiveMethodEvent>
		{
			// Token: 0x060037FD RID: 14333 RVA: 0x00124C2B File Offset: 0x00122E2B
			public IsHotspotActiveMethodEvent(EntityUid Grid, Vector2i Tile, bool Result = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x170008B4 RID: 2228
			// (get) Token: 0x060037FE RID: 14334 RVA: 0x00124C4A File Offset: 0x00122E4A
			// (set) Token: 0x060037FF RID: 14335 RVA: 0x00124C52 File Offset: 0x00122E52
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008B5 RID: 2229
			// (get) Token: 0x06003800 RID: 14336 RVA: 0x00124C5B File Offset: 0x00122E5B
			// (set) Token: 0x06003801 RID: 14337 RVA: 0x00124C63 File Offset: 0x00122E63
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008B6 RID: 2230
			// (get) Token: 0x06003802 RID: 14338 RVA: 0x00124C6C File Offset: 0x00122E6C
			// (set) Token: 0x06003803 RID: 14339 RVA: 0x00124C74 File Offset: 0x00122E74
			public bool Result { readonly get; set; }

			// Token: 0x170008B7 RID: 2231
			// (get) Token: 0x06003804 RID: 14340 RVA: 0x00124C7D File Offset: 0x00122E7D
			// (set) Token: 0x06003805 RID: 14341 RVA: 0x00124C85 File Offset: 0x00122E85
			public bool Handled { readonly get; set; }

			// Token: 0x06003806 RID: 14342 RVA: 0x00124C90 File Offset: 0x00122E90
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("IsHotspotActiveMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003807 RID: 14343 RVA: 0x00124CDC File Offset: 0x00122EDC
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003808 RID: 14344 RVA: 0x00124D86 File Offset: 0x00122F86
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.IsHotspotActiveMethodEvent left, AtmosphereSystem.IsHotspotActiveMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003809 RID: 14345 RVA: 0x00124D92 File Offset: 0x00122F92
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.IsHotspotActiveMethodEvent left, AtmosphereSystem.IsHotspotActiveMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600380A RID: 14346 RVA: 0x00124D9C File Offset: 0x00122F9C
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600380B RID: 14347 RVA: 0x00124DFE File Offset: 0x00122FFE
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.IsHotspotActiveMethodEvent && this.Equals((AtmosphereSystem.IsHotspotActiveMethodEvent)obj);
			}

			// Token: 0x0600380C RID: 14348 RVA: 0x00124E18 File Offset: 0x00123018
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.IsHotspotActiveMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600380D RID: 14349 RVA: 0x00124E85 File Offset: 0x00123085
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B24 RID: 2852
		[NullableContext(0)]
		[ByRefEvent]
		private struct FixTileVacuumMethodEvent : IEquatable<AtmosphereSystem.FixTileVacuumMethodEvent>
		{
			// Token: 0x0600380E RID: 14350 RVA: 0x00124EB0 File Offset: 0x001230B0
			public FixTileVacuumMethodEvent(EntityUid Grid, Vector2i Tile, bool Handled = false)
			{
				this.Grid = Grid;
				this.Tile = Tile;
				this.Handled = Handled;
			}

			// Token: 0x170008B8 RID: 2232
			// (get) Token: 0x0600380F RID: 14351 RVA: 0x00124EC7 File Offset: 0x001230C7
			// (set) Token: 0x06003810 RID: 14352 RVA: 0x00124ECF File Offset: 0x001230CF
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008B9 RID: 2233
			// (get) Token: 0x06003811 RID: 14353 RVA: 0x00124ED8 File Offset: 0x001230D8
			// (set) Token: 0x06003812 RID: 14354 RVA: 0x00124EE0 File Offset: 0x001230E0
			public Vector2i Tile { readonly get; set; }

			// Token: 0x170008BA RID: 2234
			// (get) Token: 0x06003813 RID: 14355 RVA: 0x00124EE9 File Offset: 0x001230E9
			// (set) Token: 0x06003814 RID: 14356 RVA: 0x00124EF1 File Offset: 0x001230F1
			public bool Handled { readonly get; set; }

			// Token: 0x06003815 RID: 14357 RVA: 0x00124EFC File Offset: 0x001230FC
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FixTileVacuumMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003816 RID: 14358 RVA: 0x00124F48 File Offset: 0x00123148
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Tile = ");
				builder.Append(this.Tile.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003817 RID: 14359 RVA: 0x00124FCB File Offset: 0x001231CB
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.FixTileVacuumMethodEvent left, AtmosphereSystem.FixTileVacuumMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003818 RID: 14360 RVA: 0x00124FD7 File Offset: 0x001231D7
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.FixTileVacuumMethodEvent left, AtmosphereSystem.FixTileVacuumMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003819 RID: 14361 RVA: 0x00124FE1 File Offset: 0x001231E1
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this.<Tile>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600381A RID: 14362 RVA: 0x00125021 File Offset: 0x00123221
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.FixTileVacuumMethodEvent && this.Equals((AtmosphereSystem.FixTileVacuumMethodEvent)obj);
			}

			// Token: 0x0600381B RID: 14363 RVA: 0x0012503C File Offset: 0x0012323C
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.FixTileVacuumMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<Vector2i>.Default.Equals(this.<Tile>k__BackingField, other.<Tile>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600381C RID: 14364 RVA: 0x00125091 File Offset: 0x00123291
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out Vector2i Tile, out bool Handled)
			{
				Grid = this.Grid;
				Tile = this.Tile;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B25 RID: 2853
		[Nullable(0)]
		[ByRefEvent]
		private struct AddPipeNetMethodEvent : IEquatable<AtmosphereSystem.AddPipeNetMethodEvent>
		{
			// Token: 0x0600381D RID: 14365 RVA: 0x001250B3 File Offset: 0x001232B3
			public AddPipeNetMethodEvent(EntityUid Grid, PipeNet PipeNet, bool Handled = false)
			{
				this.Grid = Grid;
				this.PipeNet = PipeNet;
				this.Handled = Handled;
			}

			// Token: 0x170008BB RID: 2235
			// (get) Token: 0x0600381E RID: 14366 RVA: 0x001250CA File Offset: 0x001232CA
			// (set) Token: 0x0600381F RID: 14367 RVA: 0x001250D2 File Offset: 0x001232D2
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008BC RID: 2236
			// (get) Token: 0x06003820 RID: 14368 RVA: 0x001250DB File Offset: 0x001232DB
			// (set) Token: 0x06003821 RID: 14369 RVA: 0x001250E3 File Offset: 0x001232E3
			public PipeNet PipeNet { readonly get; set; }

			// Token: 0x170008BD RID: 2237
			// (get) Token: 0x06003822 RID: 14370 RVA: 0x001250EC File Offset: 0x001232EC
			// (set) Token: 0x06003823 RID: 14371 RVA: 0x001250F4 File Offset: 0x001232F4
			public bool Handled { readonly get; set; }

			// Token: 0x06003824 RID: 14372 RVA: 0x00125100 File Offset: 0x00123300
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AddPipeNetMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003825 RID: 14373 RVA: 0x0012514C File Offset: 0x0012334C
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", PipeNet = ");
				builder.Append(this.PipeNet);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003826 RID: 14374 RVA: 0x001251C1 File Offset: 0x001233C1
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.AddPipeNetMethodEvent left, AtmosphereSystem.AddPipeNetMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003827 RID: 14375 RVA: 0x001251CD File Offset: 0x001233CD
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.AddPipeNetMethodEvent left, AtmosphereSystem.AddPipeNetMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003828 RID: 14376 RVA: 0x001251D7 File Offset: 0x001233D7
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<PipeNet>.Default.GetHashCode(this.<PipeNet>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x06003829 RID: 14377 RVA: 0x00125217 File Offset: 0x00123417
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.AddPipeNetMethodEvent && this.Equals((AtmosphereSystem.AddPipeNetMethodEvent)obj);
			}

			// Token: 0x0600382A RID: 14378 RVA: 0x00125230 File Offset: 0x00123430
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.AddPipeNetMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<PipeNet>.Default.Equals(this.<PipeNet>k__BackingField, other.<PipeNet>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600382B RID: 14379 RVA: 0x00125285 File Offset: 0x00123485
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out PipeNet PipeNet, out bool Handled)
			{
				Grid = this.Grid;
				PipeNet = this.PipeNet;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B26 RID: 2854
		[Nullable(0)]
		[ByRefEvent]
		private struct RemovePipeNetMethodEvent : IEquatable<AtmosphereSystem.RemovePipeNetMethodEvent>
		{
			// Token: 0x0600382C RID: 14380 RVA: 0x001252A3 File Offset: 0x001234A3
			public RemovePipeNetMethodEvent(EntityUid Grid, PipeNet PipeNet, bool Handled = false)
			{
				this.Grid = Grid;
				this.PipeNet = PipeNet;
				this.Handled = Handled;
			}

			// Token: 0x170008BE RID: 2238
			// (get) Token: 0x0600382D RID: 14381 RVA: 0x001252BA File Offset: 0x001234BA
			// (set) Token: 0x0600382E RID: 14382 RVA: 0x001252C2 File Offset: 0x001234C2
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008BF RID: 2239
			// (get) Token: 0x0600382F RID: 14383 RVA: 0x001252CB File Offset: 0x001234CB
			// (set) Token: 0x06003830 RID: 14384 RVA: 0x001252D3 File Offset: 0x001234D3
			public PipeNet PipeNet { readonly get; set; }

			// Token: 0x170008C0 RID: 2240
			// (get) Token: 0x06003831 RID: 14385 RVA: 0x001252DC File Offset: 0x001234DC
			// (set) Token: 0x06003832 RID: 14386 RVA: 0x001252E4 File Offset: 0x001234E4
			public bool Handled { readonly get; set; }

			// Token: 0x06003833 RID: 14387 RVA: 0x001252F0 File Offset: 0x001234F0
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("RemovePipeNetMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003834 RID: 14388 RVA: 0x0012533C File Offset: 0x0012353C
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", PipeNet = ");
				builder.Append(this.PipeNet);
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003835 RID: 14389 RVA: 0x001253B1 File Offset: 0x001235B1
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.RemovePipeNetMethodEvent left, AtmosphereSystem.RemovePipeNetMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003836 RID: 14390 RVA: 0x001253BD File Offset: 0x001235BD
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.RemovePipeNetMethodEvent left, AtmosphereSystem.RemovePipeNetMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003837 RID: 14391 RVA: 0x001253C7 File Offset: 0x001235C7
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<PipeNet>.Default.GetHashCode(this.<PipeNet>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x06003838 RID: 14392 RVA: 0x00125407 File Offset: 0x00123607
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.RemovePipeNetMethodEvent && this.Equals((AtmosphereSystem.RemovePipeNetMethodEvent)obj);
			}

			// Token: 0x06003839 RID: 14393 RVA: 0x00125420 File Offset: 0x00123620
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.RemovePipeNetMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<PipeNet>.Default.Equals(this.<PipeNet>k__BackingField, other.<PipeNet>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600383A RID: 14394 RVA: 0x00125475 File Offset: 0x00123675
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out PipeNet PipeNet, out bool Handled)
			{
				Grid = this.Grid;
				PipeNet = this.PipeNet;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B27 RID: 2855
		[Nullable(0)]
		[ByRefEvent]
		private struct AddAtmosDeviceMethodEvent : IEquatable<AtmosphereSystem.AddAtmosDeviceMethodEvent>
		{
			// Token: 0x0600383B RID: 14395 RVA: 0x00125493 File Offset: 0x00123693
			public AddAtmosDeviceMethodEvent(EntityUid Grid, AtmosDeviceComponent Device, bool Result = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Device = Device;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x170008C1 RID: 2241
			// (get) Token: 0x0600383C RID: 14396 RVA: 0x001254B2 File Offset: 0x001236B2
			// (set) Token: 0x0600383D RID: 14397 RVA: 0x001254BA File Offset: 0x001236BA
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008C2 RID: 2242
			// (get) Token: 0x0600383E RID: 14398 RVA: 0x001254C3 File Offset: 0x001236C3
			// (set) Token: 0x0600383F RID: 14399 RVA: 0x001254CB File Offset: 0x001236CB
			public AtmosDeviceComponent Device { readonly get; set; }

			// Token: 0x170008C3 RID: 2243
			// (get) Token: 0x06003840 RID: 14400 RVA: 0x001254D4 File Offset: 0x001236D4
			// (set) Token: 0x06003841 RID: 14401 RVA: 0x001254DC File Offset: 0x001236DC
			public bool Result { readonly get; set; }

			// Token: 0x170008C4 RID: 2244
			// (get) Token: 0x06003842 RID: 14402 RVA: 0x001254E5 File Offset: 0x001236E5
			// (set) Token: 0x06003843 RID: 14403 RVA: 0x001254ED File Offset: 0x001236ED
			public bool Handled { readonly get; set; }

			// Token: 0x06003844 RID: 14404 RVA: 0x001254F8 File Offset: 0x001236F8
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AddAtmosDeviceMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003845 RID: 14405 RVA: 0x00125544 File Offset: 0x00123744
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Device = ");
				builder.Append(this.Device);
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003846 RID: 14406 RVA: 0x001255E0 File Offset: 0x001237E0
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.AddAtmosDeviceMethodEvent left, AtmosphereSystem.AddAtmosDeviceMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003847 RID: 14407 RVA: 0x001255EC File Offset: 0x001237EC
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.AddAtmosDeviceMethodEvent left, AtmosphereSystem.AddAtmosDeviceMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003848 RID: 14408 RVA: 0x001255F8 File Offset: 0x001237F8
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<AtmosDeviceComponent>.Default.GetHashCode(this.<Device>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x06003849 RID: 14409 RVA: 0x0012565A File Offset: 0x0012385A
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.AddAtmosDeviceMethodEvent && this.Equals((AtmosphereSystem.AddAtmosDeviceMethodEvent)obj);
			}

			// Token: 0x0600384A RID: 14410 RVA: 0x00125674 File Offset: 0x00123874
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.AddAtmosDeviceMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<AtmosDeviceComponent>.Default.Equals(this.<Device>k__BackingField, other.<Device>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600384B RID: 14411 RVA: 0x001256E1 File Offset: 0x001238E1
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out AtmosDeviceComponent Device, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Device = this.Device;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B28 RID: 2856
		[Nullable(0)]
		[ByRefEvent]
		private struct RemoveAtmosDeviceMethodEvent : IEquatable<AtmosphereSystem.RemoveAtmosDeviceMethodEvent>
		{
			// Token: 0x0600384C RID: 14412 RVA: 0x00125708 File Offset: 0x00123908
			public RemoveAtmosDeviceMethodEvent(EntityUid Grid, AtmosDeviceComponent Device, bool Result = false, bool Handled = false)
			{
				this.Grid = Grid;
				this.Device = Device;
				this.Result = Result;
				this.Handled = Handled;
			}

			// Token: 0x170008C5 RID: 2245
			// (get) Token: 0x0600384D RID: 14413 RVA: 0x00125727 File Offset: 0x00123927
			// (set) Token: 0x0600384E RID: 14414 RVA: 0x0012572F File Offset: 0x0012392F
			public EntityUid Grid { readonly get; set; }

			// Token: 0x170008C6 RID: 2246
			// (get) Token: 0x0600384F RID: 14415 RVA: 0x00125738 File Offset: 0x00123938
			// (set) Token: 0x06003850 RID: 14416 RVA: 0x00125740 File Offset: 0x00123940
			public AtmosDeviceComponent Device { readonly get; set; }

			// Token: 0x170008C7 RID: 2247
			// (get) Token: 0x06003851 RID: 14417 RVA: 0x00125749 File Offset: 0x00123949
			// (set) Token: 0x06003852 RID: 14418 RVA: 0x00125751 File Offset: 0x00123951
			public bool Result { readonly get; set; }

			// Token: 0x170008C8 RID: 2248
			// (get) Token: 0x06003853 RID: 14419 RVA: 0x0012575A File Offset: 0x0012395A
			// (set) Token: 0x06003854 RID: 14420 RVA: 0x00125762 File Offset: 0x00123962
			public bool Handled { readonly get; set; }

			// Token: 0x06003855 RID: 14421 RVA: 0x0012576C File Offset: 0x0012396C
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("RemoveAtmosDeviceMethodEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003856 RID: 14422 RVA: 0x001257B8 File Offset: 0x001239B8
			[NullableContext(0)]
			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Grid = ");
				builder.Append(this.Grid.ToString());
				builder.Append(", Device = ");
				builder.Append(this.Device);
				builder.Append(", Result = ");
				builder.Append(this.Result.ToString());
				builder.Append(", Handled = ");
				builder.Append(this.Handled.ToString());
				return true;
			}

			// Token: 0x06003857 RID: 14423 RVA: 0x00125854 File Offset: 0x00123A54
			[CompilerGenerated]
			public static bool operator !=(AtmosphereSystem.RemoveAtmosDeviceMethodEvent left, AtmosphereSystem.RemoveAtmosDeviceMethodEvent right)
			{
				return !(left == right);
			}

			// Token: 0x06003858 RID: 14424 RVA: 0x00125860 File Offset: 0x00123A60
			[CompilerGenerated]
			public static bool operator ==(AtmosphereSystem.RemoveAtmosDeviceMethodEvent left, AtmosphereSystem.RemoveAtmosDeviceMethodEvent right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003859 RID: 14425 RVA: 0x0012586C File Offset: 0x00123A6C
			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return ((EqualityComparer<EntityUid>.Default.GetHashCode(this.<Grid>k__BackingField) * -1521134295 + EqualityComparer<AtmosDeviceComponent>.Default.GetHashCode(this.<Device>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Result>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<Handled>k__BackingField);
			}

			// Token: 0x0600385A RID: 14426 RVA: 0x001258CE File Offset: 0x00123ACE
			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is AtmosphereSystem.RemoveAtmosDeviceMethodEvent && this.Equals((AtmosphereSystem.RemoveAtmosDeviceMethodEvent)obj);
			}

			// Token: 0x0600385B RID: 14427 RVA: 0x001258E8 File Offset: 0x00123AE8
			[CompilerGenerated]
			public readonly bool Equals(AtmosphereSystem.RemoveAtmosDeviceMethodEvent other)
			{
				return EqualityComparer<EntityUid>.Default.Equals(this.<Grid>k__BackingField, other.<Grid>k__BackingField) && EqualityComparer<AtmosDeviceComponent>.Default.Equals(this.<Device>k__BackingField, other.<Device>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Result>k__BackingField, other.<Result>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<Handled>k__BackingField, other.<Handled>k__BackingField);
			}

			// Token: 0x0600385C RID: 14428 RVA: 0x00125955 File Offset: 0x00123B55
			[CompilerGenerated]
			public readonly void Deconstruct(out EntityUid Grid, out AtmosDeviceComponent Device, out bool Result, out bool Handled)
			{
				Grid = this.Grid;
				Device = this.Device;
				Result = this.Result;
				Handled = this.Handled;
			}
		}

		// Token: 0x02000B29 RID: 2857
		[NullableContext(0)]
		public enum GasCompareResult
		{
			// Token: 0x0400293C RID: 10556
			NoExchange = -2,
			// Token: 0x0400293D RID: 10557
			TemperatureExchange
		}

		// Token: 0x02000B2A RID: 2858
		[NullableContext(0)]
		private sealed class TileAtmosphereComparer : IComparer<TileAtmosphere>
		{
			// Token: 0x0600385D RID: 14429 RVA: 0x0012597C File Offset: 0x00123B7C
			[NullableContext(2)]
			public int Compare(TileAtmosphere a, TileAtmosphere b)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (a == null)
				{
					return -1;
				}
				if (b == null)
				{
					return 1;
				}
				return a.MonstermosInfo.MoleDelta.CompareTo(b.MonstermosInfo.MoleDelta);
			}
		}
	}
}
