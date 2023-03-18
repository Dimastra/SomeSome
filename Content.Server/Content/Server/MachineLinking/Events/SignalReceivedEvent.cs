using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.MachineLinking.Events
{
	// Token: 0x020003FA RID: 1018
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalReceivedEvent : EntityEventArgs
	{
		// Token: 0x060014CA RID: 5322 RVA: 0x0006D040 File Offset: 0x0006B240
		public SignalReceivedEvent(string port, EntityUid? trigger)
		{
			this.Port = port;
			this.Trigger = trigger;
		}

		// Token: 0x04000CD7 RID: 3287
		public readonly string Port;

		// Token: 0x04000CD8 RID: 3288
		public readonly EntityUid? Trigger;
	}
}
