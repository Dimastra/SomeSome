using System;
using System.Runtime.CompilerServices;
using Content.Shared.Drunk;
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

namespace Content.Client.Drunk
{
	// Token: 0x02000336 RID: 822
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DrunkOverlay : Overlay
	{
		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06001481 RID: 5249 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001482 RID: 5250 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00078441 File Offset: 0x00076641
		public DrunkOverlay()
		{
			IoCManager.InjectDependencies<DrunkOverlay>(this);
			this._drunkShader = this._prototypeManager.Index<ShaderPrototype>("Drunk").InstanceUnique();
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x0007846C File Offset: 0x0007666C
		protected override void FrameUpdate(FrameEventArgs args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			StatusEffectsComponent status;
			if (!this._entityManager.HasComponent<DrunkComponent>(entityUid) || !this._entityManager.TryGetComponent<StatusEffectsComponent>(entityUid, ref status))
			{
				return;
			}
			ValueTuple<TimeSpan, TimeSpan>? valueTuple;
			if (!this._sysMan.GetEntitySystem<StatusEffectsSystem>().TryGetTime(entityUid.Value, "Drunk", out valueTuple, status))
			{
				return;
			}
			float num = (float)(valueTuple.Value.Item2 - valueTuple.Value.Item1).TotalSeconds;
			this.CurrentBoozePower += (num - this.CurrentBoozePower) * args.DeltaSeconds / 16f;
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00078530 File Offset: 0x00076730
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
			this._visualScale = this.BoozePowerToVisual(this.CurrentBoozePower);
			return this._visualScale > 0f;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000785A4 File Offset: 0x000767A4
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			this._drunkShader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
			this._drunkShader.SetParameter("boozePower", this._visualScale);
			worldHandle.UseShader(this._drunkShader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00078610 File Offset: 0x00076810
		private float BoozePowerToVisual(float boozePower)
		{
			return Math.Clamp((boozePower - 10f) / 250f, 0f, 1f);
		}

		// Token: 0x04000A7C RID: 2684
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000A7D RID: 2685
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000A7E RID: 2686
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A7F RID: 2687
		[Dependency]
		private readonly IEntitySystemManager _sysMan;

		// Token: 0x04000A80 RID: 2688
		private readonly ShaderInstance _drunkShader;

		// Token: 0x04000A81 RID: 2689
		public float CurrentBoozePower;

		// Token: 0x04000A82 RID: 2690
		private const float VisualThreshold = 10f;

		// Token: 0x04000A83 RID: 2691
		private const float PowerDivisor = 250f;

		// Token: 0x04000A84 RID: 2692
		private float _visualScale;
	}
}
