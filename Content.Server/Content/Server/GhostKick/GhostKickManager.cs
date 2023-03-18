using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.GhostKick;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Server.GhostKick
{
	// Token: 0x0200048B RID: 1163
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostKickManager
	{
		// Token: 0x06001753 RID: 5971 RVA: 0x0007A702 File Offset: 0x00078902
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgGhostKick>(null, 3);
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0007A714 File Offset: 0x00078914
		public void DoDisconnect(INetChannel channel, string reason)
		{
			Action <>9__1;
			Timer.Spawn(TimeSpan.FromMilliseconds(100.0), delegate()
			{
				if (!channel.IsConnected)
				{
					return;
				}
				channel.SendMessage(new MsgGhostKick());
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(100.0);
				Action action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate()
					{
						if (!channel.IsConnected)
						{
							return;
						}
						channel.Disconnect(reason, false);
					});
				}
				Timer.Spawn(timeSpan, action, default(CancellationToken));
			}, default(CancellationToken));
		}

		// Token: 0x04000E92 RID: 3730
		[Dependency]
		private readonly IServerNetManager _netManager;
	}
}
