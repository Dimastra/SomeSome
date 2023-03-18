using System;
using System.Runtime.CompilerServices;
using Content.Client.Resources;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.XamlExtensions
{
	// Token: 0x0200006F RID: 111
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TexExtension
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000E4B7 File Offset: 0x0000C6B7
		public string Path { get; }

		// Token: 0x06000204 RID: 516 RVA: 0x0000E4BF File Offset: 0x0000C6BF
		public TexExtension(string path)
		{
			this._resourceCache = IoCManager.Resolve<IResourceCache>();
			this.Path = path;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000E4D9 File Offset: 0x0000C6D9
		public object ProvideValue()
		{
			return this._resourceCache.GetTexture(this.Path);
		}

		// Token: 0x04000145 RID: 325
		private IResourceCache _resourceCache;
	}
}
