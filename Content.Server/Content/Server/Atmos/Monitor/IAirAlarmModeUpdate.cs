using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Monitor
{
	// Token: 0x02000774 RID: 1908
	[NullableContext(1)]
	public interface IAirAlarmModeUpdate
	{
		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06002885 RID: 10373
		// (set) Token: 0x06002886 RID: 10374
		string NetOwner { get; set; }

		// Token: 0x06002887 RID: 10375
		void Update(EntityUid uid);
	}
}
