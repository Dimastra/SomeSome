using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Roles;

namespace Content.Server.Ghost.Roles
{
	// Token: 0x02000494 RID: 1172
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostRoleMarkerRole : Role
	{
		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001783 RID: 6019 RVA: 0x0007B541 File Offset: 0x00079741
		public override string Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001784 RID: 6020 RVA: 0x0007B549 File Offset: 0x00079749
		public override bool Antagonist
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x0007B54C File Offset: 0x0007974C
		public GhostRoleMarkerRole(Mind mind, string name) : base(mind)
		{
			this._name = name;
		}

		// Token: 0x04000EA1 RID: 3745
		private readonly string _name;
	}
}
