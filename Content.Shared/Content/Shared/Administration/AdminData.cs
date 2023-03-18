using System;
using System.Runtime.CompilerServices;

namespace Content.Shared.Administration
{
	// Token: 0x0200072F RID: 1839
	public sealed class AdminData
	{
		// Token: 0x06001643 RID: 5699 RVA: 0x00048E72 File Offset: 0x00047072
		public bool HasFlag(AdminFlags flag)
		{
			return this.Active && (this.Flags & flag) == flag;
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x00048E89 File Offset: 0x00047089
		public bool CanAdminPlace()
		{
			return this.HasFlag(AdminFlags.Spawn);
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x00048E93 File Offset: 0x00047093
		public bool CanScript()
		{
			return this.HasFlag((AdminFlags)2147483648U);
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x00048EA0 File Offset: 0x000470A0
		public bool CanAdminMenu()
		{
			return this.HasFlag(AdminFlags.Admin);
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x00048EA9 File Offset: 0x000470A9
		public bool CanAdminReloadPrototypes()
		{
			return this.HasFlag((AdminFlags)2147483648U);
		}

		// Token: 0x0400168F RID: 5775
		public bool Active;

		// Token: 0x04001690 RID: 5776
		[Nullable(2)]
		public string Title;

		// Token: 0x04001691 RID: 5777
		public AdminFlags Flags;
	}
}
