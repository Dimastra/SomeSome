using System;
using System.Runtime.CompilerServices;
using Content.Client.Viewport;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Flash
{
	// Token: 0x02000316 RID: 790
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlashOverlay : Overlay
	{
		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060013E9 RID: 5097 RVA: 0x00026117 File Offset: 0x00024317
		public override OverlaySpace Space
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x000750F4 File Offset: 0x000732F4
		public FlashOverlay()
		{
			IoCManager.InjectDependencies<FlashOverlay>(this);
			this._shader = this._prototypeManager.Index<ShaderPrototype>("FlashedEffect").Instance().Duplicate();
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0007514C File Offset: 0x0007334C
		public void ReceiveFlash(double duration)
		{
			IMainViewportState mainViewportState = this._stateManager.CurrentState as IMainViewportState;
			if (mainViewportState != null)
			{
				mainViewportState.Viewport.Viewport.Screenshot(delegate(Image<Rgba32> image)
				{
					Image<Rgba32> image2 = image.CloneAs<Rgba32>(Configuration.Default);
					this._screenshotTexture = this._displayManager.LoadTextureFromImage<Rgba32>(image2, null, null);
				});
			}
			this._startTime = this._gameTiming.CurTime.TotalSeconds;
			this._lastsFor = duration;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x000751AC File Offset: 0x000733AC
		protected override void Draw(in OverlayDrawArgs args)
		{
			IEntityManager entityManager = this._entityManager;
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EyeComponent eyeComponent;
			if (!entityManager.TryGetComponent<EyeComponent>((localPlayer != null) ? localPlayer.ControlledEntity : null, ref eyeComponent))
			{
				return;
			}
			if (args.Viewport.Eye != eyeComponent.Eye)
			{
				return;
			}
			float num = (float)((this._gameTiming.CurTime.TotalSeconds - this._startTime) / this._lastsFor);
			if (num >= 1f)
			{
				return;
			}
			DrawingHandleScreen screenHandle = args.ScreenHandle;
			screenHandle.UseShader(this._shader);
			this._shader.SetParameter("percentComplete", num);
			UIBox2 uibox = UIBox2.FromDimensions(new ValueTuple<float, float>(0f, 0f), this._displayManager.ScreenSize);
			if (this._screenshotTexture != null)
			{
				screenHandle.DrawTextureRect(this._screenshotTexture, uibox, null);
			}
			screenHandle.UseShader(null);
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0007529E File Offset: 0x0007349E
		protected override void DisposeBehavior()
		{
			base.DisposeBehavior();
			this._screenshotTexture = null;
		}

		// Token: 0x040009FC RID: 2556
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040009FD RID: 2557
		[Dependency]
		private readonly IClyde _displayManager;

		// Token: 0x040009FE RID: 2558
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040009FF RID: 2559
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000A00 RID: 2560
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000A01 RID: 2561
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A02 RID: 2562
		private readonly ShaderInstance _shader;

		// Token: 0x04000A03 RID: 2563
		private double _startTime = -1.0;

		// Token: 0x04000A04 RID: 2564
		private double _lastsFor = 1.0;

		// Token: 0x04000A05 RID: 2565
		[Nullable(2)]
		private Texture _screenshotTexture;
	}
}
