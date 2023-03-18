using System;
using System.Runtime.CompilerServices;
using Content.Client.Hands.Systems;
using Content.Shared.CCVar;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Hands
{
	// Token: 0x020002DC RID: 732
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowHandItemOverlay : Overlay
	{
		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06001275 RID: 4725 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0006E38C File Offset: 0x0006C58C
		public ShowHandItemOverlay()
		{
			IoCManager.InjectDependencies<ShowHandItemOverlay>(this);
			IClyde clyde = this._clyde;
			Vector2i vector2i = new ValueTuple<int, int>(64, 64);
			RenderTargetFormatParameters renderTargetFormatParameters = new RenderTargetFormatParameters(1, true);
			TextureSampleParameters value = default(TextureSampleParameters);
			value.Filter = true;
			this._renderBackbuffer = clyde.CreateRenderTarget(vector2i, renderTargetFormatParameters, new TextureSampleParameters?(value), "ShowHandItemOverlay");
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0006E3E7 File Offset: 0x0006C5E7
		protected override void DisposeBehavior()
		{
			base.DisposeBehavior();
			this._renderBackbuffer.Dispose();
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0006E3FC File Offset: 0x0006C5FC
		protected override void Draw(in OverlayDrawArgs args)
		{
			ShowHandItemOverlay.<>c__DisplayClass11_0 CS$<>8__locals1 = new ShowHandItemOverlay.<>c__DisplayClass11_0();
			if (!this._cfg.GetCVar<bool>(CCVars.HudHeldItemShow))
			{
				return;
			}
			CS$<>8__locals1.screen = args.ScreenHandle;
			float cvar = this._cfg.GetCVar<float>(CCVars.HudHeldItemOffset);
			Vector2 position = this._inputManager.MouseScreenPosition.Position;
			if (this.IconOverride != null)
			{
				CS$<>8__locals1.screen.DrawTexture(this.IconOverride, position - this.IconOverride.Size / 2 + cvar, new Color?(Color.White.WithAlpha(0.75f)));
				return;
			}
			ShowHandItemOverlay.<>c__DisplayClass11_0 CS$<>8__locals2 = CS$<>8__locals1;
			EntityUid? entityOverride = this.EntityOverride;
			CS$<>8__locals2.handEntity = ((entityOverride != null) ? entityOverride : EntitySystem.Get<HandsSystem>().GetActiveHandEntity());
			if (CS$<>8__locals1.handEntity == null || !this._entMan.HasComponent<SpriteComponent>(CS$<>8__locals1.handEntity))
			{
				return;
			}
			CS$<>8__locals1.halfSize = this._renderBackbuffer.Size / 2;
			ShowHandItemOverlay.<>c__DisplayClass11_0 CS$<>8__locals3 = CS$<>8__locals1;
			Control control = args.ViewportControl as Control;
			CS$<>8__locals3.uiScale = ((control != null) ? control.UIScale : 1f);
			CS$<>8__locals1.screen.RenderInRenderTarget(this._renderBackbuffer, delegate()
			{
				CS$<>8__locals1.screen.DrawEntity(CS$<>8__locals1.handEntity.Value, CS$<>8__locals1.halfSize, new Vector2(1f, 1f) * CS$<>8__locals1.uiScale, new Direction?(0));
			}, new Color?(Color.Transparent));
			CS$<>8__locals1.screen.DrawTexture(this._renderBackbuffer.Texture, position - CS$<>8__locals1.halfSize + cvar, new Color?(Color.White.WithAlpha(0.75f)));
		}

		// Token: 0x04000923 RID: 2339
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000924 RID: 2340
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000925 RID: 2341
		[Dependency]
		private readonly IClyde _clyde;

		// Token: 0x04000926 RID: 2342
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000927 RID: 2343
		private readonly IRenderTexture _renderBackbuffer;

		// Token: 0x04000928 RID: 2344
		[Nullable(2)]
		public Texture IconOverride;

		// Token: 0x04000929 RID: 2345
		public EntityUid? EntityOverride;
	}
}
