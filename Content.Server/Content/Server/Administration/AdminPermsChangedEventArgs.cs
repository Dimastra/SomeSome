using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;

namespace Content.Server.Administration
{
	// Token: 0x020007FD RID: 2045
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminPermsChangedEventArgs : EventArgs
	{
		// Token: 0x06002C34 RID: 11316 RVA: 0x000E70A9 File Offset: 0x000E52A9
		public AdminPermsChangedEventArgs(IPlayerSession player, AdminFlags? flags)
		{
			this.Player = player;
			this.Flags = flags;
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002C35 RID: 11317 RVA: 0x000E70BF File Offset: 0x000E52BF
		public IPlayerSession Player { get; }

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06002C36 RID: 11318 RVA: 0x000E70C7 File Offset: 0x000E52C7
		public AdminFlags? Flags { get; }

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06002C37 RID: 11319 RVA: 0x000E70D0 File Offset: 0x000E52D0
		public bool IsAdmin
		{
			get
			{
				return this.Flags != null;
			}
		}
	}
}
