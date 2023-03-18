using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Content.Server.Players
{
	// Token: 0x020002D1 RID: 721
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class PlayerData
	{
		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x00049EEE File Offset: 0x000480EE
		[ViewVariables]
		public NetUserId UserId { get; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000E81 RID: 3713 RVA: 0x00049EF6 File Offset: 0x000480F6
		[Nullable(1)]
		[ViewVariables]
		public string Name { [NullableContext(1)] get; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x00049EFE File Offset: 0x000480FE
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x00049F06 File Offset: 0x00048106
		[ViewVariables]
		public Mind Mind { get; private set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x00049F0F File Offset: 0x0004810F
		// (set) Token: 0x06000E85 RID: 3717 RVA: 0x00049F17 File Offset: 0x00048117
		public bool ExplicitlyDeadminned { get; set; }

		// Token: 0x06000E86 RID: 3718 RVA: 0x00049F20 File Offset: 0x00048120
		public void WipeMind()
		{
			Mind mind = this.Mind;
			if (mind != null)
			{
				mind.TransferTo(null, false, false);
			}
			Mind mind2 = this.Mind;
			if (mind2 == null)
			{
				return;
			}
			mind2.ChangeOwningPlayer(null);
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x00049F62 File Offset: 0x00048162
		public void UpdateMindFromMindChangeOwningPlayer(Mind mind)
		{
			this.Mind = mind;
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x00049F6B File Offset: 0x0004816B
		[NullableContext(1)]
		public PlayerData(NetUserId userId, string name)
		{
			this.UserId = userId;
			this.Name = name;
		}
	}
}
