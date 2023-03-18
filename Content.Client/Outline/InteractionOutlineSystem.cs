using System;
using System.Runtime.CompilerServices;
using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Client.Interactable.Components;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.Outline
{
	// Token: 0x020001F4 RID: 500
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InteractionOutlineSystem : EntitySystem
	{
		// Token: 0x06000CC0 RID: 3264 RVA: 0x0004A75B File Offset: 0x0004895B
		public override void Initialize()
		{
			base.Initialize();
			this._configManager.OnValueChanged<bool>(CCVars.OutlineEnabled, new Action<bool>(this.SetCvarEnabled), false);
			base.UpdatesAfter.Add(typeof(EyeUpdateSystem));
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0004A795 File Offset: 0x00048995
		public override void Shutdown()
		{
			base.Shutdown();
			this._configManager.UnsubValueChanged<bool>(CCVars.OutlineEnabled, new Action<bool>(this.SetCvarEnabled));
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0004A7BC File Offset: 0x000489BC
		public void SetCvarEnabled(bool cvarEnabled)
		{
			this._cvarEnabled = cvarEnabled;
			if (this._cvarEnabled)
			{
				return;
			}
			if (this._lastHoveredEntity == null || base.Deleted(this._lastHoveredEntity))
			{
				return;
			}
			InteractionOutlineComponent interactionOutlineComponent;
			if (base.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.OnMouseLeave();
			}
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x0004A80C File Offset: 0x00048A0C
		public void SetEnabled(bool enabled)
		{
			if (enabled == this._enabled)
			{
				return;
			}
			this._enabled = enabled;
			if (enabled)
			{
				return;
			}
			if (this._lastHoveredEntity == null || base.Deleted(this._lastHoveredEntity))
			{
				return;
			}
			InteractionOutlineComponent interactionOutlineComponent;
			if (base.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.OnMouseLeave();
			}
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x0004A860 File Offset: 0x00048A60
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			if (!this._enabled || !this._cvarEnabled)
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null)
			{
				return;
			}
			GameplayStateBase gameplayStateBase = this._stateManager.CurrentState as GameplayStateBase;
			if (gameplayStateBase == null)
			{
				return;
			}
			EntityUid? entityUid = null;
			int renderScale = 1;
			IViewportControl viewportControl = this._uiManager.CurrentlyHovered as IViewportControl;
			if (viewportControl != null && this._inputManager.MouseScreenPosition.IsValid)
			{
				MapCoordinates coordinates = viewportControl.ScreenToMap(this._inputManager.MouseScreenPosition.Position);
				entityUid = gameplayStateBase.GetClickedEntity(coordinates);
				ScalingViewport scalingViewport = viewportControl as ScalingViewport;
				if (scalingViewport != null)
				{
					renderScale = scalingViewport.CurrentRenderScale;
				}
			}
			else
			{
				EntityMenuElement entityMenuElement = this._uiManager.CurrentlyHovered as EntityMenuElement;
				if (entityMenuElement != null)
				{
					entityUid = entityMenuElement.Entity;
					renderScale = this._eyeManager.MainViewport.GetRenderScale();
				}
			}
			bool inInteractionRange = false;
			if (localPlayer.ControlledEntity != null && !base.Deleted(entityUid))
			{
				inInteractionRange = this._interactionSystem.InRangeUnobstructed(localPlayer.ControlledEntity.Value, entityUid.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false);
			}
			InteractionOutlineComponent interactionOutlineComponent;
			if (entityUid == this._lastHoveredEntity)
			{
				if (entityUid != null && base.TryComp<InteractionOutlineComponent>(entityUid, ref interactionOutlineComponent))
				{
					interactionOutlineComponent.UpdateInRange(inInteractionRange, renderScale);
				}
				return;
			}
			if (this._lastHoveredEntity != null && !base.Deleted(this._lastHoveredEntity) && base.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.OnMouseLeave();
			}
			this._lastHoveredEntity = entityUid;
			if (this._lastHoveredEntity != null && base.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref interactionOutlineComponent))
			{
				interactionOutlineComponent.OnMouseEnter(inInteractionRange, renderScale);
			}
		}

		// Token: 0x04000678 RID: 1656
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000679 RID: 1657
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x0400067A RID: 1658
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x0400067B RID: 1659
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400067C RID: 1660
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x0400067D RID: 1661
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x0400067E RID: 1662
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x0400067F RID: 1663
		private bool _enabled = true;

		// Token: 0x04000680 RID: 1664
		private bool _cvarEnabled = true;

		// Token: 0x04000681 RID: 1665
		private EntityUid? _lastHoveredEntity;
	}
}
