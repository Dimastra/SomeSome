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
	// Token: 0x0200031E RID: 798
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlurryVisionOverlay : Overlay
	{
		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x000765C0 File Offset: 0x000747C0
		public BlurryVisionOverlay()
		{
			IoCManager.InjectDependencies<BlurryVisionOverlay>(this);
			this._dim = this._prototypeManager.Index<ShaderPrototype>("Dim").InstanceUnique();
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x000765EC File Offset: 0x000747EC
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
			BlurryVisionComponent blurryVisionComponent;
			if (!this._entityManager.TryGetComponent<BlurryVisionComponent>(entityUid, ref blurryVisionComponent))
			{
				return false;
			}
			if (!blurryVisionComponent.Active)
			{
				return false;
			}
			BlindableComponent blindableComponent;
			if (this._entityManager.TryGetComponent<BlindableComponent>(entityUid, ref blindableComponent) && blindableComponent.Sources > 0)
			{
				return false;
			}
			this._blurryVisionComponent = blurryVisionComponent;
			return true;
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x000766AC File Offset: 0x000748AC
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			float num = -(this._blurryVisionComponent.Magnitude / 15f) + 0.9f;
			this._dim.SetParameter("DAMAGE_AMOUNT", num);
			DrawingHandleWorld worldHandle = args.WorldHandle;
			Box2Rotated worldBounds = args.WorldBounds;
			worldHandle.UseShader(this._dim);
			worldHandle.SetTransform(ref Matrix3.Identity);
			worldHandle.DrawRect(ref worldBounds, Color.Black, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x04000A1E RID: 2590
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A1F RID: 2591
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000A20 RID: 2592
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A21 RID: 2593
		private readonly ShaderInstance _dim;

		// Token: 0x04000A22 RID: 2594
		private BlurryVisionComponent _blurryVisionComponent;
	}
}
