using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Client.Graphics;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Parallax.Data
{
	// Token: 0x020001E8 RID: 488
	[NullableContext(1)]
	[ImplicitDataDefinitionForInheritors]
	public interface IParallaxTextureSource
	{
		// Token: 0x06000C82 RID: 3202
		Task<Texture> GenerateTexture(CancellationToken cancel = default(CancellationToken));
	}
}
