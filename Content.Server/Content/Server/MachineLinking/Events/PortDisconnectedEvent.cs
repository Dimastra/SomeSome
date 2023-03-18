using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.MachineLinking.Events
{
	// Token: 0x020003F9 RID: 1017
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PortDisconnectedEvent : EntityEventArgs
	{
		// Token: 0x060014C9 RID: 5321 RVA: 0x0006D031 File Offset: 0x0006B231
		public PortDisconnectedEvent(string port)
		{
			this.Port = port;
		}

		// Token: 0x04000CD6 RID: 3286
		public readonly string Port;
	}
}
