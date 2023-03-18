using System;
using System.Runtime.CompilerServices;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000DD RID: 221
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MainViewport : UIWidget
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000637 RID: 1591 RVA: 0x0002169C File Offset: 0x0001F89C
		public ScalingViewport Viewport { get; }

		// Token: 0x06000638 RID: 1592 RVA: 0x000216A4 File Offset: 0x0001F8A4
		public MainViewport()
		{
			IoCManager.InjectDependencies<MainViewport>(this);
			this.Viewport = new ScalingViewport
			{
				AlwaysRender = true,
				RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt,
				MouseFilter = 0
			};
			base.AddChild(this.Viewport);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x000216DF File Offset: 0x0001F8DF
		protected override void EnteredTree()
		{
			base.EnteredTree();
			this._vpManager.AddViewport(this);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x000216F3 File Offset: 0x0001F8F3
		protected override void ExitedTree()
		{
			base.ExitedTree();
			this._vpManager.RemoveViewport(this);
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00021708 File Offset: 0x0001F908
		public void UpdateCfg()
		{
			bool cvar = this._cfg.GetCVar<bool>(CCVars.ViewportStretch);
			bool cvar2 = this._cfg.GetCVar<bool>(CCVars.ViewportScaleRender);
			int num = this._cfg.GetCVar<int>(CCVars.ViewportFixedScaleFactor);
			if (cvar)
			{
				int? num2 = this.CalcSnappingFactor();
				if (num2 == null)
				{
					this.Viewport.FixedStretchSize = null;
					this.Viewport.StretchMode = ScalingViewportStretchMode.Bilinear;
					if (cvar2)
					{
						this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.CeilInt;
						return;
					}
					this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
					this.Viewport.FixedRenderScale = 1;
					return;
				}
				else
				{
					num = num2.Value;
				}
			}
			this.Viewport.FixedStretchSize = new Vector2i?(this.Viewport.ViewportSize * num);
			this.Viewport.StretchMode = ScalingViewportStretchMode.Nearest;
			if (cvar2)
			{
				this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
				this.Viewport.FixedRenderScale = num;
				return;
			}
			this.Viewport.RenderScaleMode = ScalingViewportRenderScaleMode.Fixed;
			this.Viewport.FixedRenderScale = 1;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0002180C File Offset: 0x0001FA0C
		private int? CalcSnappingFactor()
		{
			int cvar = this._cfg.GetCVar<int>(CCVars.ViewportSnapToleranceMargin);
			int cvar2 = this._cfg.GetCVar<int>(CCVars.ViewportSnapToleranceClip);
			for (int i = 1; i <= 10; i++)
			{
				MainViewport.<>c__DisplayClass9_0 CS$<>8__locals1;
				CS$<>8__locals1.toleranceMargin = i * cvar;
				CS$<>8__locals1.toleranceClip = i * cvar2;
				Vector2 vector = this.Viewport.ViewportSize * (float)i;
				float num;
				float num2;
				(base.PixelSize - vector).Deconstruct(ref num, ref num2);
				float a = num;
				float a2 = num2;
				if ((MainViewport.<CalcSnappingFactor>g__Fits|9_1(a, ref CS$<>8__locals1) && MainViewport.<CalcSnappingFactor>g__Fits|9_1(a2, ref CS$<>8__locals1)) || (MainViewport.<CalcSnappingFactor>g__Fits|9_1(a, ref CS$<>8__locals1) && MainViewport.<CalcSnappingFactor>g__Larger|9_0(a2, ref CS$<>8__locals1)) || (MainViewport.<CalcSnappingFactor>g__Larger|9_0(a, ref CS$<>8__locals1) && MainViewport.<CalcSnappingFactor>g__Fits|9_1(a2, ref CS$<>8__locals1)))
				{
					return new int?(i);
				}
			}
			return null;
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x000218F5 File Offset: 0x0001FAF5
		protected override void Resized()
		{
			base.Resized();
			this.UpdateCfg();
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00021903 File Offset: 0x0001FB03
		[CompilerGenerated]
		internal static bool <CalcSnappingFactor>g__Larger|9_0(float a, ref MainViewport.<>c__DisplayClass9_0 A_1)
		{
			return a > (float)A_1.toleranceMargin;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0002190F File Offset: 0x0001FB0F
		[CompilerGenerated]
		internal static bool <CalcSnappingFactor>g__Fits|9_1(float a, ref MainViewport.<>c__DisplayClass9_0 A_1)
		{
			return a <= (float)A_1.toleranceMargin && a >= (float)(-(float)A_1.toleranceClip);
		}

		// Token: 0x040002C4 RID: 708
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040002C5 RID: 709
		[Dependency]
		private readonly ViewportManager _vpManager;
	}
}
