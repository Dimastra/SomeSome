using System;
using System.Runtime.CompilerServices;
using Content.Shared.Eye.Blinding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Eye.Blinding
{
	// Token: 0x0200031D RID: 797
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlindOverlay : Overlay
	{
		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06001422 RID: 5154 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x000763E4 File Offset: 0x000745E4
		public BlindOverlay()
		{
			IoCManager.InjectDependencies<BlindOverlay>(this);
			this._greyscaleShader = this._prototypeManager.Index<ShaderPrototype>("GreyscaleFullscreen").InstanceUnique();
			this._circleMaskShader = this._prototypeManager.Index<ShaderPrototype>("CircleMask").InstanceUnique();
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x00076434 File Offset: 0x00074634
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			IEntityManager entityManager = this._entityManager;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EyeComponent eyeComponent;
			if (!entityManager.TryGetComponent<EyeComponent>((localPlayer != null) ? localPlayer.ControlledEntity : null, ref eyeComponent))
			{
				return false;
			}
			if (args.Viewport.Eye != eyeComponent.Eye)
			{
				return false;
			}
			LocalPlayer localPlayer2 = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer2 != null) ? localPlayer2.ControlledEntity : null;
			if (entityUid == null)
			{
				return false;
			}
			BlindableComponent blindableComponent;
			if (!this._entityManager.TryGetComponent<BlindableComponent>(entityUid, ref blindableComponent))
			{
				return false;
			}
			this._blindableComponent = blindableComponent;
			bool flag = this._blindableComponent.Sources > 0;
			if (!flag && this._blindableComponent.LightSetup)
			{
				this._lightManager.Enabled = true;
				this._blindableComponent.LightSetup = false;
				this._blindableComponent.GraceFrame = true;
				return true;
			}
			return flag;
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x00076514 File Offset: 0x00074714
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			if (!this._blindableComponent.GraceFrame)
			{
				this._blindableComponent.LightSetup = true;
				this._lightManager.Enabled = false;
			}
			else
			{
				this._blindableComponent.GraceFrame = false;
			}
			ShaderInstance greyscaleShader = this._greyscaleShader;
			if (greyscaleShader != null)
			{
				greyscaleShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			Box2Rotated worldBounds = args.WorldBounds;
			worldHandle.UseShader(this._greyscaleShader);
			worldHandle.DrawRect(ref worldBounds, Color.White, true);
			worldHandle.UseShader(this._circleMaskShader);
			worldHandle.DrawRect(ref worldBounds, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x04000A17 RID: 2583
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A18 RID: 2584
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A19 RID: 2585
		[Dependency]
		private IEntityManager _entityManager;

		// Token: 0x04000A1A RID: 2586
		[Dependency]
		private ILightManager _lightManager;

		// Token: 0x04000A1B RID: 2587
		private readonly ShaderInstance _greyscaleShader;

		// Token: 0x04000A1C RID: 2588
		private readonly ShaderInstance _circleMaskShader;

		// Token: 0x04000A1D RID: 2589
		private BlindableComponent _blindableComponent;
	}
}
