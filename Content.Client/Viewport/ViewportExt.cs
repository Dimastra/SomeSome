using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Viewport
{
	// Token: 0x0200005B RID: 91
	public static class ViewportExt
	{
		// Token: 0x060001AD RID: 429 RVA: 0x0000C504 File Offset: 0x0000A704
		[NullableContext(1)]
		public static int GetRenderScale(this IViewportControl viewport)
		{
			ScalingViewport scalingViewport = viewport as ScalingViewport;
			if (scalingViewport != null)
			{
				return scalingViewport.CurrentRenderScale;
			}
			return 1;
		}
	}
}
