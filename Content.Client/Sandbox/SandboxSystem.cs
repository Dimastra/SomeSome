using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Shared.Sandbox;
using Robust.Client.Console;
using Robust.Client.Placement;
using Robust.Client.UserInterface;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Players;

namespace Content.Client.Sandbox
{
	// Token: 0x02000162 RID: 354
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class SandboxSystem : SharedSandboxSystem
	{
		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x000361C2 File Offset: 0x000343C2
		// (set) Token: 0x06000935 RID: 2357 RVA: 0x000361CA File Offset: 0x000343CA
		public bool SandboxAllowed { get; private set; }

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06000936 RID: 2358 RVA: 0x000361D4 File Offset: 0x000343D4
		// (remove) Token: 0x06000937 RID: 2359 RVA: 0x0003620C File Offset: 0x0003440C
		public event Action SandboxEnabled;

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06000938 RID: 2360 RVA: 0x00036244 File Offset: 0x00034444
		// (remove) Token: 0x06000939 RID: 2361 RVA: 0x0003627C File Offset: 0x0003447C
		public event Action SandboxDisabled;

		// Token: 0x0600093A RID: 2362 RVA: 0x000362B1 File Offset: 0x000344B1
		public override void Initialize()
		{
			this._adminManager.AdminStatusUpdated += this.CheckStatus;
			base.SubscribeNetworkEvent<SharedSandboxSystem.MsgSandboxStatus>(new EntityEventHandler<SharedSandboxSystem.MsgSandboxStatus>(this.OnSandboxStatus), null, null);
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x000362E0 File Offset: 0x000344E0
		private void CheckStatus()
		{
			bool flag = this._sandboxEnabled || this._adminManager.IsActive();
			if (flag == this.SandboxAllowed)
			{
				return;
			}
			this.SandboxAllowed = flag;
			if (this.SandboxAllowed)
			{
				Action sandboxEnabled = this.SandboxEnabled;
				if (sandboxEnabled == null)
				{
					return;
				}
				sandboxEnabled();
				return;
			}
			else
			{
				Action sandboxDisabled = this.SandboxDisabled;
				if (sandboxDisabled == null)
				{
					return;
				}
				sandboxDisabled();
				return;
			}
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0003633E File Offset: 0x0003453E
		public override void Shutdown()
		{
			this._adminManager.AdminStatusUpdated -= this.CheckStatus;
			base.Shutdown();
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0003635D File Offset: 0x0003455D
		[NullableContext(1)]
		private void OnSandboxStatus(SharedSandboxSystem.MsgSandboxStatus ev)
		{
			this.SetAllowed(ev.SandboxAllowed);
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0003636B File Offset: 0x0003456B
		private void SetAllowed(bool sandboxEnabled)
		{
			this._sandboxEnabled = sandboxEnabled;
			this.CheckStatus();
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0003637A File Offset: 0x0003457A
		public void Respawn()
		{
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxRespawn());
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00036387 File Offset: 0x00034587
		public void GiveAdminAccess()
		{
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxGiveAccess());
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00036394 File Offset: 0x00034594
		public void GiveAGhost()
		{
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxGiveAghost());
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x000363A1 File Offset: 0x000345A1
		public void Suicide()
		{
			base.RaiseNetworkEvent(new SharedSandboxSystem.MsgSandboxSuicide());
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x000363B0 File Offset: 0x000345B0
		public bool Copy(ICommonSession session, EntityCoordinates coords, EntityUid uid)
		{
			if (!this.SandboxAllowed)
			{
				return false;
			}
			MetaDataComponent metaDataComponent;
			if (uid.IsValid() && this.EntityManager.TryGetComponent<MetaDataComponent>(uid, ref metaDataComponent) && !metaDataComponent.EntityDeleted)
			{
				if (metaDataComponent.EntityPrototype == null || metaDataComponent.EntityPrototype.NoSpawn || metaDataComponent.EntityPrototype.Abstract)
				{
					return false;
				}
				if (this._placement.Eraser)
				{
					this._placement.ToggleEraser();
				}
				this._placement.BeginPlacing(new PlacementInformation
				{
					EntityType = metaDataComponent.EntityPrototype.ID,
					IsTile = false,
					TileType = 0,
					PlacementOption = metaDataComponent.EntityPrototype.PlacementMode
				}, null);
				return true;
			}
			else
			{
				MapGridComponent mapGridComponent;
				TileRef tileRef;
				if (!this._map.TryFindGridAt(coords.ToMap(this.EntityManager), ref mapGridComponent) || !mapGridComponent.TryGetTileRef(coords, ref tileRef))
				{
					return false;
				}
				if (this._placement.Eraser)
				{
					this._placement.ToggleEraser();
				}
				this._placement.BeginPlacing(new PlacementInformation
				{
					EntityType = null,
					IsTile = true,
					TileType = tileRef.Tile.TypeId,
					PlacementOption = "AlignTileAny"
				}, null);
				return true;
			}
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x000364EA File Offset: 0x000346EA
		public void ToggleLight()
		{
			this._consoleHost.ExecuteCommand("togglelight");
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x000364FC File Offset: 0x000346FC
		public void ToggleFov()
		{
			this._consoleHost.ExecuteCommand("togglefov");
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0003650E File Offset: 0x0003470E
		public void ToggleShadows()
		{
			this._consoleHost.ExecuteCommand("toggleshadows");
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00036520 File Offset: 0x00034720
		public void ToggleSubFloor()
		{
			this._consoleHost.ExecuteCommand("showsubfloor");
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x00036532 File Offset: 0x00034732
		public void ShowMarkers()
		{
			this._consoleHost.ExecuteCommand("showmarkers");
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00036544 File Offset: 0x00034744
		public void ShowBb()
		{
			this._consoleHost.ExecuteCommand("physics shapes");
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00036556 File Offset: 0x00034756
		public void MachineLinking()
		{
			this._consoleHost.ExecuteCommand("signallink");
		}

		// Token: 0x040004A7 RID: 1191
		[Nullable(1)]
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x040004A8 RID: 1192
		[Nullable(1)]
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x040004A9 RID: 1193
		[Nullable(1)]
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x040004AA RID: 1194
		[Nullable(1)]
		[Dependency]
		private readonly IPlacementManager _placement;

		// Token: 0x040004AB RID: 1195
		[Nullable(1)]
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x040004AC RID: 1196
		private bool _sandboxEnabled;
	}
}
