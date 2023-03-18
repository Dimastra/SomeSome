using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.IoC;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Parallax.Data
{
	// Token: 0x020001E7 RID: 487
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ImageParallaxTextureSource : IParallaxTextureSource
	{
		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000C7F RID: 3199 RVA: 0x0004948E File Offset: 0x0004768E
		[DataField("path", false, 1, true, false, null)]
		public ResourcePath Path { get; }

		// Token: 0x06000C80 RID: 3200 RVA: 0x00049496 File Offset: 0x00047696
		Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
		{
			return Task.FromResult<Texture>(StaticIoC.ResC.GetTexture(this.Path));
		}
	}
}
