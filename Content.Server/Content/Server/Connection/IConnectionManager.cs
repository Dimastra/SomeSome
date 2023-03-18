using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server.Connection
{
	// Token: 0x0200062C RID: 1580
	[NullableContext(1)]
	public interface IConnectionManager
	{
		// Token: 0x060021A7 RID: 8615
		void Initialize();

		// Token: 0x060021A8 RID: 8616
		Task<bool> HavePrivilegedJoin(NetUserId userId);
	}
}
