using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;

namespace Content.Client.Administration.Managers
{
	// Token: 0x020004E6 RID: 1254
	[NullableContext(1)]
	public interface IClientAdminManager
	{
		// Token: 0x140000C5 RID: 197
		// (add) Token: 0x06001FEE RID: 8174
		// (remove) Token: 0x06001FEF RID: 8175
		event Action AdminStatusUpdated;

		// Token: 0x06001FF0 RID: 8176
		bool IsActive();

		// Token: 0x06001FF1 RID: 8177
		bool HasFlag(AdminFlags flag);

		// Token: 0x06001FF2 RID: 8178
		bool CanCommand(string cmdName);

		// Token: 0x06001FF3 RID: 8179
		bool CanViewVar();

		// Token: 0x06001FF4 RID: 8180
		bool CanAdminPlace();

		// Token: 0x06001FF5 RID: 8181
		bool CanScript();

		// Token: 0x06001FF6 RID: 8182
		bool CanAdminMenu();

		// Token: 0x06001FF7 RID: 8183
		void Initialize();
	}
}
