using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.IoC;
using Nett;
using Robust.Client.Graphics;
using Robust.Shared.ContentPack;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client.Parallax.Data
{
	// Token: 0x020001E3 RID: 483
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class GeneratedParallaxTextureSource : IParallaxTextureSource
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00048D5A File Offset: 0x00046F5A
		[DataField("configPath", false, 1, false, false, null)]
		public ResourcePath ParallaxConfigPath { get; } = new ResourcePath("/parallax_config.toml", "/");

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000C71 RID: 3185 RVA: 0x00048D62 File Offset: 0x00046F62
		[DataField("id", false, 1, false, false, null)]
		public string Identifier { get; } = "other";

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x00048D6A File Offset: 0x00046F6A
		private ResourcePath ParallaxCachedImagePath
		{
			get
			{
				return new ResourcePath("/parallax_" + this.Identifier + "cache.png", "/");
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000C73 RID: 3187 RVA: 0x00048D8B File Offset: 0x00046F8B
		private ResourcePath PreviousParallaxConfigPath
		{
			get
			{
				return new ResourcePath("/parallax_" + this.Identifier + "config_old", "/");
			}
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00048DAC File Offset: 0x00046FAC
		Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
		{
			GeneratedParallaxTextureSource.<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__10 <Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__;
			<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.<>t__builder = AsyncTaskMethodBuilder<Texture>.Create();
			<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.<>4__this = this;
			<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.cancel = cancel;
			<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.<>1__state = -1;
			<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.<>t__builder.Start<GeneratedParallaxTextureSource.<Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__10>(ref <Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__);
			return <Content-Client-Parallax-Data-IParallaxTextureSource-GenerateTexture>d__.<>t__builder.Task;
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x00048DF8 File Offset: 0x00046FF8
		private Task UpdateCachedTexture(TomlTable config, bool saveDebugLayers, CancellationToken cancel = default(CancellationToken))
		{
			GeneratedParallaxTextureSource.<UpdateCachedTexture>d__11 <UpdateCachedTexture>d__;
			<UpdateCachedTexture>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateCachedTexture>d__.<>4__this = this;
			<UpdateCachedTexture>d__.config = config;
			<UpdateCachedTexture>d__.saveDebugLayers = saveDebugLayers;
			<UpdateCachedTexture>d__.cancel = cancel;
			<UpdateCachedTexture>d__.<>1__state = -1;
			<UpdateCachedTexture>d__.<>t__builder.Start<GeneratedParallaxTextureSource.<UpdateCachedTexture>d__11>(ref <UpdateCachedTexture>d__);
			return <UpdateCachedTexture>d__.<>t__builder.Task;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00048E54 File Offset: 0x00047054
		private Texture GetCachedTexture()
		{
			Texture result;
			using (Stream stream = WritableDirProviderExt.OpenRead(StaticIoC.ResC.UserData, this.ParallaxCachedImagePath))
			{
				result = Texture.LoadFromPNGStream(stream, "Parallax", null);
			}
			return result;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00048EAC File Offset: 0x000470AC
		[NullableContext(2)]
		private string GetParallaxConfig()
		{
			Stream stream;
			if (!StaticIoC.ResC.TryContentFileRead(this.ParallaxConfigPath, ref stream))
			{
				return null;
			}
			string result;
			using (StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8))
			{
				result = streamReader.ReadToEnd().Replace(Environment.NewLine, "\n");
			}
			return result;
		}
	}
}
