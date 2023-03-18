using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;

namespace Content.Client.Resources
{
	// Token: 0x0200016B RID: 363
	[NullableContext(1)]
	[Nullable(0)]
	public static class ResourceCacheExtensions
	{
		// Token: 0x0600096F RID: 2415 RVA: 0x000371C4 File Offset: 0x000353C4
		public static Texture GetTexture(this IResourceCache cache, ResourcePath path)
		{
			return cache.GetResource<TextureResource>(path, true);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x000371D3 File Offset: 0x000353D3
		public static Texture GetTexture(this IResourceCache cache, string path)
		{
			return cache.GetTexture(new ResourcePath(path, "/"));
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x000371E6 File Offset: 0x000353E6
		public static Font GetFont(this IResourceCache cache, ResourcePath path, int size)
		{
			return new VectorFont(cache.GetResource<FontResource>(path, true), size);
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x000371F6 File Offset: 0x000353F6
		public static Font GetFont(this IResourceCache cache, string path, int size)
		{
			return cache.GetFont(new ResourcePath(path, "/"), size);
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0003720C File Offset: 0x0003540C
		public static Font GetFont(this IResourceCache cache, ResourcePath[] path, int size)
		{
			Font[] array = new Font[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = new VectorFont(cache.GetResource<FontResource>(path[i], true), size);
			}
			return new StackedFont(array);
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0003724C File Offset: 0x0003544C
		public static Font GetFont(this IResourceCache cache, string[] path, int size)
		{
			ResourcePath[] array = new ResourcePath[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				array[i] = new ResourcePath(path[i], "/");
			}
			return cache.GetFont(array, size);
		}
	}
}
