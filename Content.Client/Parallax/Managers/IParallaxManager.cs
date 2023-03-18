using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Robust.Shared.Maths;

namespace Content.Client.Parallax.Managers
{
	// Token: 0x020001DD RID: 477
	[NullableContext(1)]
	public interface IParallaxManager
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000C57 RID: 3159
		// (set) Token: 0x06000C58 RID: 3160
		Vector2 ParallaxAnchor { get; set; }

		// Token: 0x06000C59 RID: 3161
		bool IsLoaded(string name);

		// Token: 0x06000C5A RID: 3162
		ParallaxLayerPrepared[] GetParallaxLayers(string name);

		// Token: 0x06000C5B RID: 3163
		void LoadDefaultParallax();

		// Token: 0x06000C5C RID: 3164
		Task LoadParallaxByName(string name);

		// Token: 0x06000C5D RID: 3165
		void UnloadParallax(string name);
	}
}
