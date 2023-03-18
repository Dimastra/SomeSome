using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Roles;
using Robust.Shared.Localization;

namespace Content.Server.Ghost
{
	// Token: 0x02000493 RID: 1171
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ObserverRole : Role
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06001780 RID: 6016 RVA: 0x0007B529 File Offset: 0x00079729
		public override string Name
		{
			get
			{
				return Loc.GetString("observer-role-name");
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06001781 RID: 6017 RVA: 0x0007B535 File Offset: 0x00079735
		public override bool Antagonist
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0007B538 File Offset: 0x00079738
		public ObserverRole(Mind mind) : base(mind)
		{
		}
	}
}
