using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.GameTicking;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Popups
{
	// Token: 0x020001AD RID: 429
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PopupSystem : SharedPopupSystem
	{
		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000B25 RID: 2853 RVA: 0x00041371 File Offset: 0x0003F571
		public IReadOnlyList<PopupSystem.WorldPopupLabel> WorldLabels
		{
			get
			{
				return this._aliveWorldLabels;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000B26 RID: 2854 RVA: 0x00041379 File Offset: 0x0003F579
		public IReadOnlyList<PopupSystem.CursorPopupLabel> CursorLabels
		{
			get
			{
				return this._aliveCursorLabels;
			}
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x00041384 File Offset: 0x0003F584
		public override void Initialize()
		{
			base.SubscribeNetworkEvent<PopupCursorEvent>(new EntityEventHandler<PopupCursorEvent>(this.OnPopupCursorEvent), null, null);
			base.SubscribeNetworkEvent<PopupCoordinatesEvent>(new EntityEventHandler<PopupCoordinatesEvent>(this.OnPopupCoordinatesEvent), null, null);
			base.SubscribeNetworkEvent<PopupEntityEvent>(new EntityEventHandler<PopupEntityEvent>(this.OnPopupEntityEvent), null, null);
			base.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			this._overlay.AddOverlay(new PopupOverlay(this._configManager, this.EntityManager, this._playerManager, this._prototype, this._resource, this._uiManager, this));
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00041417 File Offset: 0x0003F617
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlay.RemoveOverlay<PopupOverlay>();
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0004142C File Offset: 0x0003F62C
		private void PopupMessage(string message, PopupType type, EntityCoordinates coordinates, EntityUid? entity = null)
		{
			PopupSystem.WorldPopupLabel item = new PopupSystem.WorldPopupLabel(coordinates)
			{
				Text = message,
				Type = type
			};
			this._aliveWorldLabels.Add(item);
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0004145C File Offset: 0x0003F65C
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, PopupType type = PopupType.Small)
		{
			this.PopupMessage(message, type, coordinates, null);
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0004147C File Offset: 0x0003F67C
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (((localPlayer != null) ? localPlayer.Session : null) == recipient)
			{
				this.PopupMessage(message, type, coordinates, null);
			}
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x000414B8 File Offset: 0x0003F6B8
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, EntityUid recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == recipient));
			}
			if (flag)
			{
				this.PopupMessage(message, type, coordinates, null);
			}
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x0004151C File Offset: 0x0003F71C
		public override void PopupCursor(string message, PopupType type = PopupType.Small)
		{
			PopupSystem.CursorPopupLabel item = new PopupSystem.CursorPopupLabel(this._inputManager.MouseScreenPosition)
			{
				Text = message,
				Type = type
			};
			this._aliveCursorLabels.Add(item);
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00041554 File Offset: 0x0003F754
		public override void PopupCursor(string message, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (((localPlayer != null) ? localPlayer.Session : null) == recipient)
			{
				this.PopupCursor(message, type);
			}
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x00041578 File Offset: 0x0003F778
		public override void PopupCursor(string message, EntityUid recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == recipient));
			}
			if (flag)
			{
				this.PopupCursor(message, type);
			}
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x000415CE File Offset: 0x0003F7CE
		public override void PopupCoordinates(string message, EntityCoordinates coordinates, Filter filter, bool replayRecord, PopupType type = PopupType.Small)
		{
			this.PopupCoordinates(message, coordinates, type);
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x000415DC File Offset: 0x0003F7DC
		public override void PopupEntity(string message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			bool flag;
			if (localPlayer == null)
			{
				flag = false;
			}
			else
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				flag = (controlledEntity != null && (controlledEntity == null || controlledEntity.GetValueOrDefault() == recipient));
			}
			if (flag)
			{
				this.PopupEntity(message, uid, type);
			}
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00041634 File Offset: 0x0003F834
		public override void PopupEntity(string message, EntityUid uid, ICommonSession recipient, PopupType type = PopupType.Small)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (((localPlayer != null) ? localPlayer.Session : null) == recipient)
			{
				this.PopupEntity(message, uid, type);
			}
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0004165A File Offset: 0x0003F85A
		public override void PopupEntity(string message, EntityUid uid, Filter filter, bool recordReplay, PopupType type = PopupType.Small)
		{
			this.PopupEntity(message, uid, type);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00041668 File Offset: 0x0003F868
		public override void PopupEntity(string message, EntityUid uid, PopupType type = PopupType.Small)
		{
			if (!this.EntityManager.EntityExists(uid))
			{
				return;
			}
			TransformComponent component = this.EntityManager.GetComponent<TransformComponent>(uid);
			this.PopupMessage(message, type, component.Coordinates, new EntityUid?(uid));
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x000416A5 File Offset: 0x0003F8A5
		private void OnPopupCursorEvent(PopupCursorEvent ev)
		{
			this.PopupCursor(ev.Message, ev.Type);
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x000416B9 File Offset: 0x0003F8B9
		private void OnPopupCoordinatesEvent(PopupCoordinatesEvent ev)
		{
			this.PopupCoordinates(ev.Message, ev.Coordinates, ev.Type);
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x000416D3 File Offset: 0x0003F8D3
		private void OnPopupEntityEvent(PopupEntityEvent ev)
		{
			this.PopupEntity(ev.Message, ev.Uid, ev.Type);
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x000416ED File Offset: 0x0003F8ED
		private void OnRoundRestart(RoundRestartCleanupEvent ev)
		{
			this._aliveCursorLabels.Clear();
			this._aliveWorldLabels.Clear();
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00041708 File Offset: 0x0003F908
		public override void FrameUpdate(float frameTime)
		{
			if (this._aliveWorldLabels.Count == 0 && this._aliveCursorLabels.Count == 0)
			{
				return;
			}
			for (int i = 0; i < this._aliveWorldLabels.Count; i++)
			{
				PopupSystem.WorldPopupLabel worldPopupLabel = this._aliveWorldLabels[i];
				worldPopupLabel.TotalTime += frameTime;
				if (worldPopupLabel.TotalTime > 3f || base.Deleted(worldPopupLabel.InitialPos.EntityId, null))
				{
					Extensions.RemoveSwap<PopupSystem.WorldPopupLabel>(this._aliveWorldLabels, i);
					i--;
				}
			}
			for (int j = 0; j < this._aliveCursorLabels.Count; j++)
			{
				PopupSystem.CursorPopupLabel cursorPopupLabel = this._aliveCursorLabels[j];
				cursorPopupLabel.TotalTime += frameTime;
				if (cursorPopupLabel.TotalTime > 3f)
				{
					Extensions.RemoveSwap<PopupSystem.CursorPopupLabel>(this._aliveCursorLabels, j);
					j--;
				}
			}
		}

		// Token: 0x04000571 RID: 1393
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000572 RID: 1394
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000573 RID: 1395
		[Dependency]
		private readonly IOverlayManager _overlay;

		// Token: 0x04000574 RID: 1396
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000575 RID: 1397
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000576 RID: 1398
		[Dependency]
		private readonly IResourceCache _resource;

		// Token: 0x04000577 RID: 1399
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x04000578 RID: 1400
		private readonly List<PopupSystem.WorldPopupLabel> _aliveWorldLabels = new List<PopupSystem.WorldPopupLabel>();

		// Token: 0x04000579 RID: 1401
		private readonly List<PopupSystem.CursorPopupLabel> _aliveCursorLabels = new List<PopupSystem.CursorPopupLabel>();

		// Token: 0x0400057A RID: 1402
		public const float PopupLifetime = 3f;

		// Token: 0x020001AE RID: 430
		[Nullable(0)]
		public abstract class PopupLabel
		{
			// Token: 0x17000226 RID: 550
			// (get) Token: 0x06000B3B RID: 2875 RVA: 0x000417FE File Offset: 0x0003F9FE
			// (set) Token: 0x06000B3C RID: 2876 RVA: 0x00041806 File Offset: 0x0003FA06
			public string Text { get; set; } = string.Empty;

			// Token: 0x17000227 RID: 551
			// (get) Token: 0x06000B3D RID: 2877 RVA: 0x0004180F File Offset: 0x0003FA0F
			// (set) Token: 0x06000B3E RID: 2878 RVA: 0x00041817 File Offset: 0x0003FA17
			public float TotalTime { get; set; }

			// Token: 0x0400057B RID: 1403
			public PopupType Type;
		}

		// Token: 0x020001AF RID: 431
		[NullableContext(0)]
		public sealed class CursorPopupLabel : PopupSystem.PopupLabel
		{
			// Token: 0x06000B40 RID: 2880 RVA: 0x00041833 File Offset: 0x0003FA33
			public CursorPopupLabel(ScreenCoordinates screenCoords)
			{
				this.InitialPos = screenCoords;
			}

			// Token: 0x0400057E RID: 1406
			public ScreenCoordinates InitialPos;
		}

		// Token: 0x020001B0 RID: 432
		[NullableContext(0)]
		public sealed class WorldPopupLabel : PopupSystem.PopupLabel
		{
			// Token: 0x06000B41 RID: 2881 RVA: 0x00041842 File Offset: 0x0003FA42
			public WorldPopupLabel(EntityCoordinates coordinates)
			{
				this.InitialPos = coordinates;
			}

			// Token: 0x0400057F RID: 1407
			public EntityCoordinates InitialPos;
		}
	}
}
