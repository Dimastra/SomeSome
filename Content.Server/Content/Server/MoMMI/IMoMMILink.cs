using System;
using System.Runtime.CompilerServices;

namespace Content.Server.MoMMI
{
	// Token: 0x0200039D RID: 925
	[NullableContext(1)]
	public interface IMoMMILink
	{
		// Token: 0x060012E0 RID: 4832
		void SendOOCMessage(string sender, string message);
	}
}
