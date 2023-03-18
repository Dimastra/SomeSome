using System;
using System.Runtime.CompilerServices;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000114 RID: 276
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StylesheetManager : IStylesheetManager
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0002C5EF File Offset: 0x0002A7EF
		// (set) Token: 0x060007B1 RID: 1969 RVA: 0x0002C5F7 File Offset: 0x0002A7F7
		public Stylesheet SheetNano { get; private set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0002C600 File Offset: 0x0002A800
		// (set) Token: 0x060007B3 RID: 1971 RVA: 0x0002C608 File Offset: 0x0002A808
		public Stylesheet SheetSpace { get; private set; }

		// Token: 0x060007B4 RID: 1972 RVA: 0x0002C611 File Offset: 0x0002A811
		public void Initialize()
		{
			this.SheetNano = new StyleNano(this._resourceCache).Stylesheet;
			this.SheetSpace = new StyleSpace(this._resourceCache).Stylesheet;
			this._userInterfaceManager.Stylesheet = this.SheetNano;
		}

		// Token: 0x040003DF RID: 991
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x040003E0 RID: 992
		[Dependency]
		private readonly IResourceCache _resourceCache;
	}
}
