using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Launcher
{
	// Token: 0x02000279 RID: 633
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ExtendedDisconnectInformationManager
	{
		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x0006029F File Offset: 0x0005E49F
		// (set) Token: 0x06001013 RID: 4115 RVA: 0x000602A7 File Offset: 0x0005E4A7
		public NetDisconnectedArgs LastNetDisconnectedArgs
		{
			get
			{
				return this._lastNetDisconnectedArgs;
			}
			private set
			{
				this._lastNetDisconnectedArgs = value;
				Action<NetDisconnectedArgs> lastNetDisconnectedArgsChanged = this.LastNetDisconnectedArgsChanged;
				if (lastNetDisconnectedArgsChanged == null)
				{
					return;
				}
				lastNetDisconnectedArgsChanged(value);
			}
		}

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x06001014 RID: 4116 RVA: 0x000602C4 File Offset: 0x0005E4C4
		// (remove) Token: 0x06001015 RID: 4117 RVA: 0x000602FC File Offset: 0x0005E4FC
		public event Action<NetDisconnectedArgs> LastNetDisconnectedArgsChanged;

		// Token: 0x06001016 RID: 4118 RVA: 0x00060331 File Offset: 0x0005E531
		public void Initialize()
		{
			this._clientNetManager.Disconnect += this.OnNetDisconnect;
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x0006034A File Offset: 0x0005E54A
		[NullableContext(1)]
		private void OnNetDisconnect([Nullable(2)] object sender, NetDisconnectedArgs args)
		{
			this.LastNetDisconnectedArgs = args;
		}

		// Token: 0x040007F2 RID: 2034
		[Nullable(1)]
		[Dependency]
		private readonly IClientNetManager _clientNetManager;

		// Token: 0x040007F3 RID: 2035
		private NetDisconnectedArgs _lastNetDisconnectedArgs;
	}
}
