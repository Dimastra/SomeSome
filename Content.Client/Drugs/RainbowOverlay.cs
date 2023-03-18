using System;
using System.Runtime.CompilerServices;
using Content.Shared.Drugs;
using Content.Shared.StatusEffect;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drugs
{
	// Token: 0x02000339 RID: 825
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RainbowOverlay : Overlay
	{
		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x00078934 File Offset: 0x00076B34
		private float EffectScale
		{
			get
			{
				return Math.Clamp((this.Intoxication - 10f) / 250f, 0f, 1f);
			}
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00078957 File Offset: 0x00076B57
		public RainbowOverlay()
		{
			IoCManager.InjectDependencies<RainbowOverlay>(this);
			this._rainbowShader = this._prototypeManager.Index<ShaderPrototype>("Rainbow").InstanceUnique();
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x00078984 File Offset: 0x00076B84
		protected override void FrameUpdate(FrameEventArgs args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			StatusEffectsComponent status;
			if (!this._entityManager.HasComponent<SeeingRainbowsComponent>(entityUid) || !this._entityManager.TryGetComponent<StatusEffectsComponent>(entityUid, ref status))
			{
				return;
			}
			ValueTuple<TimeSpan, TimeSpan>? valueTuple;
			if (!this._sysMan.GetEntitySystem<StatusEffectsSystem>().TryGetTime(entityUid.Value, DrugOverlaySystem.RainbowKey, out valueTuple, status))
			{
				return;
			}
			float num = (float)(valueTuple.Value.Item2 - valueTuple.Value.Item1).TotalSeconds;
			this.Intoxication += (num - this.Intoxication) * args.DeltaSeconds / 16f;
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00078A48 File Offset: 0x00076C48
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			IEntityManager entityManager = this._entityManager;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EyeComponent eyeComponent;
			return entityManager.TryGetComponent<EyeComponent>((localPlayer != null) ? localPlayer.ControlledEntity : null, ref eyeComponent) && args.Viewport.Eye == eyeComponent.Eye && this.EffectScale > 0f;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x00078AA8 File Offset: 0x00076CA8
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			this._rainbowShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
			this._rainbowShader.SetParameter("effectScale", this.EffectScale);
			worldHandle.UseShader(this._rainbowShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x04000A8C RID: 2700
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000A8D RID: 2701
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A8E RID: 2702
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A8F RID: 2703
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x04000A90 RID: 2704
		private readonly ShaderInstance _rainbowShader;

		// Token: 0x04000A91 RID: 2705
		public float Intoxication;

		// Token: 0x04000A92 RID: 2706
		private const float VisualThreshold = 10f;

		// Token: 0x04000A93 RID: 2707
		private const float PowerDivisor = 250f;
	}
}
