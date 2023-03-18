using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.Administration.Managers
{
	// Token: 0x02000816 RID: 2070
	[NullableContext(1)]
	public interface IAdminManager
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06002D39 RID: 11577
		// (remove) Token: 0x06002D3A RID: 11578
		event Action<AdminPermsChangedEventArgs> OnPermsChanged;

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x06002D3B RID: 11579
		IEnumerable<IPlayerSession> ActiveAdmins { get; }

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x06002D3C RID: 11580
		IEnumerable<IPlayerSession> AdminsWithFlag { get; }

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x06002D3D RID: 11581
		IEnumerable<IPlayerSession> AllAdmins { get; }

		// Token: 0x06002D3E RID: 11582
		bool IsAdmin(IPlayerSession session, bool includeDeAdmin = false);

		// Token: 0x06002D3F RID: 11583
		[return: Nullable(2)]
		AdminData GetAdminData(IPlayerSession session, bool includeDeAdmin = false);

		// Token: 0x06002D40 RID: 11584
		[NullableContext(2)]
		AdminData GetAdminData(EntityUid uid, bool includeDeAdmin = false);

		// Token: 0x06002D41 RID: 11585 RVA: 0x000EE690 File Offset: 0x000EC890
		bool HasAdminFlag(EntityUid player, AdminFlags flag)
		{
			AdminData data = this.GetAdminData(player, false);
			return data != null && data.HasFlag(flag);
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x000EE6B4 File Offset: 0x000EC8B4
		bool HasAdminFlag(IPlayerSession player, AdminFlags flag)
		{
			AdminData data = this.GetAdminData(player, false);
			return data != null && data.HasFlag(flag);
		}

		// Token: 0x06002D43 RID: 11587
		void DeAdmin(IPlayerSession session);

		// Token: 0x06002D44 RID: 11588
		void ReAdmin(IPlayerSession session);

		// Token: 0x06002D45 RID: 11589
		void ReloadAdmin(IPlayerSession player);

		// Token: 0x06002D46 RID: 11590
		void ReloadAdminsWithRank(int rankId);

		// Token: 0x06002D47 RID: 11591
		void Initialize();

		// Token: 0x06002D48 RID: 11592
		void PromoteHost(IPlayerSession player);
	}
}
