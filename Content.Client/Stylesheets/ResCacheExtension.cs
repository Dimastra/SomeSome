using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000112 RID: 274
	public static class ResCacheExtension
	{
		// Token: 0x060007AC RID: 1964 RVA: 0x00028660 File Offset: 0x00026860
		[NullableContext(1)]
		public static Font NotoStack(this IResourceCache resCache, string variation = "Regular", int size = 10, bool display = false)
		{
			string value = display ? "Display" : "";
			string str = variation.StartsWith("Bold", StringComparison.Ordinal) ? "Bold" : "Regular";
			string[] array = new string[3];
			int num = 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 3);
			defaultInterpolatedStringHandler.AppendLiteral("/Fonts/NotoSans");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral("/NotoSans");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral("-");
			defaultInterpolatedStringHandler.AppendFormatted(variation);
			defaultInterpolatedStringHandler.AppendLiteral(".ttf");
			array[num] = defaultInterpolatedStringHandler.ToStringAndClear();
			array[1] = "/Fonts/NotoSans/NotoSansSymbols-" + str + ".ttf";
			array[2] = "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf";
			return resCache.GetFont(array, size);
		}
	}
}
