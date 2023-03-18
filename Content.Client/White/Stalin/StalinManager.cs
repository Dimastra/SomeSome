using System;
using System.Runtime.CompilerServices;
using Content.Shared.White.SaltedYayca;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.White.Stalin
{
	// Token: 0x02000020 RID: 32
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StalinManager
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004BBB File Offset: 0x00002DBB
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<DiscordAuthResponse>(new ProcessMessage<DiscordAuthResponse>(this.OnStalinResponse), 3);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004BD5 File Offset: 0x00002DD5
		public void RequestUri()
		{
			this._netManager.ClientSendMessage(new DiscordAuthRequest());
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004BE7 File Offset: 0x00002DE7
		private void OnStalinResponse(DiscordAuthResponse message)
		{
			this._uriOpener.OpenUri(message.Uri);
		}

		// Token: 0x04000042 RID: 66
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x04000043 RID: 67
		[Dependency]
		private readonly IUriOpener _uriOpener;
	}
}
