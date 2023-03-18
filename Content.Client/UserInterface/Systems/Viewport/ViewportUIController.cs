using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Viewport
{
	// Token: 0x02000070 RID: 112
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ViewportUIController : UIController
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000E4EC File Offset: 0x0000C6EC
		[Nullable(2)]
		private MainViewport Viewport
		{
			[NullableContext(2)]
			get
			{
				UIScreen activeScreen = this.UIManager.ActiveScreen;
				if (activeScreen == null)
				{
					return null;
				}
				return activeScreen.GetWidget<MainViewport>();
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000E504 File Offset: 0x0000C704
		public override void Initialize()
		{
			this._configurationManager.OnValueChanged<int>(CCVars.ViewportMinimumWidth, delegate(int _)
			{
				this.UpdateViewportRatio();
			}, false);
			this._configurationManager.OnValueChanged<int>(CCVars.ViewportMaximumWidth, delegate(int _)
			{
				this.UpdateViewportRatio();
			}, false);
			this._configurationManager.OnValueChanged<int>(CCVars.ViewportWidth, delegate(int _)
			{
				this.UpdateViewportRatio();
			}, false);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000E568 File Offset: 0x0000C768
		private void UpdateViewportRatio()
		{
			if (this.Viewport == null)
			{
				return;
			}
			int cvar = this._configurationManager.GetCVar<int>(CCVars.ViewportMinimumWidth);
			int cvar2 = this._configurationManager.GetCVar<int>(CCVars.ViewportMaximumWidth);
			int num = this._configurationManager.GetCVar<int>(CCVars.ViewportWidth);
			if (num < cvar || num > cvar2)
			{
				num = CCVars.ViewportWidth.DefaultValue;
			}
			this.Viewport.Viewport.ViewportSize = new ValueTuple<int, int>(32 * num, 480);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000E5E8 File Offset: 0x0000C7E8
		public void ReloadViewport()
		{
			if (this.Viewport == null)
			{
				return;
			}
			this.UpdateViewportRatio();
			this.Viewport.Viewport.HorizontalExpand = true;
			this.Viewport.Viewport.VerticalExpand = true;
			this._eyeManager.MainViewport = this.Viewport.Viewport;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000E63C File Offset: 0x0000C83C
		public override void FrameUpdate(FrameEventArgs e)
		{
			if (this.Viewport == null)
			{
				return;
			}
			base.FrameUpdate(e);
			this.Viewport.Viewport.Eye = this._eyeManager.CurrentEye;
			LocalPlayer localPlayer = this._playerMan.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (this._eyeManager.CurrentEye.Position != default(MapCoordinates) || entityUid == null)
			{
				return;
			}
			EyeComponent eyeComponent;
			this._entMan.TryGetComponent<EyeComponent>(entityUid, ref eyeComponent);
			if (((eyeComponent != null) ? eyeComponent.Eye : null) == this._eyeManager.CurrentEye && this._entMan.GetComponent<TransformComponent>(entityUid.Value).WorldPosition == default(Vector2))
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(97, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Main viewport's eye is in nullspace (main eye is null?). Attached entity: ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(this._entMan.ToPrettyString(entityUid.Value));
			defaultInterpolatedStringHandler.AppendLiteral(". Entity has eye comp: ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(eyeComponent != null);
			Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04000147 RID: 327
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000148 RID: 328
		[Dependency]
		private readonly IPlayerManager _playerMan;

		// Token: 0x04000149 RID: 329
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400014A RID: 330
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400014B RID: 331
		public static readonly Vector2i ViewportSize = new ValueTuple<int, int>(672, 480);

		// Token: 0x0400014C RID: 332
		public const int ViewportHeight = 15;
	}
}
