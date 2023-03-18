using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays
{
	// Token: 0x020001F3 RID: 499
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GrainOverlay : Overlay
	{
		// Token: 0x06000CBB RID: 3259 RVA: 0x0004A6A2 File Offset: 0x000488A2
		public GrainOverlay()
		{
			IoCManager.InjectDependencies<GrainOverlay>(this);
			this._shader = this._prototype.Index<ShaderPrototype>("Grain").Instance().Duplicate();
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000CBC RID: 3260 RVA: 0x0000689B File Offset: 0x00004A9B
		public override OverlaySpace Space
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000CBD RID: 3261 RVA: 0x00003C56 File Offset: 0x00001E56
		public override bool RequestScreenTexture
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0004A6D1 File Offset: 0x000488D1
		protected override bool BeforeDraw(in OverlayDrawArgs args)
		{
			return base.BeforeDraw(ref args) && this._cfg.GetCVar<bool>(CCVars.FilmGrain);
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0004A6F0 File Offset: 0x000488F0
		protected override void Draw(in OverlayDrawArgs args)
		{
			if (this.ScreenTexture == null)
			{
				return;
			}
			DrawingHandleWorld worldHandle = args.WorldHandle;
			this._shader.SetParameter("SCREEN_TEXTURE", this.ScreenTexture);
			this._shader.SetParameter("strength", 50f);
			worldHandle.UseShader(this._shader);
			worldHandle.DrawRect(ref args.WorldBounds, Color.White, true);
			worldHandle.UseShader(null);
		}

		// Token: 0x04000675 RID: 1653
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000676 RID: 1654
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x04000677 RID: 1655
		private readonly ShaderInstance _shader;
	}
}
