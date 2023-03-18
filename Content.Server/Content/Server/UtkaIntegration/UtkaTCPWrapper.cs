using System;
using System.Net;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E1 RID: 225
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaTCPWrapper
	{
		// Token: 0x0600041A RID: 1050 RVA: 0x00015578 File Offset: 0x00013778
		public void Initialize()
		{
			if (this._initialized)
			{
				return;
			}
			this._key = this._cfg.GetCVar<string>(CCVars.UtkaSocketKey);
			if (string.IsNullOrEmpty(this._key))
			{
				return;
			}
			int port = this._cfg.GetCVar<int>(CVars.NetPort) + 100;
			try
			{
				this._server = new UtkaTCPServer(IPAddress.Any, port);
			}
			catch (Exception)
			{
				return;
			}
			this._server.Start();
			this._initialized = true;
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00015600 File Offset: 0x00013800
		public void SendMessageToAll(UtkaBaseMessage message)
		{
			this._server.SendMessageToAll(message);
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0001560E File Offset: 0x0001380E
		public void SendMessageToClient(UtkaTCPSession session, UtkaBaseMessage message)
		{
			this._server.SendMessageToClient(session, message);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0001561D File Offset: 0x0001381D
		public void Shutdown()
		{
			this._server.Stop();
			this._server.Multicast("Server shutting down.");
			this._server.DisconnectAll();
			this._server.Dispose();
		}

		// Token: 0x04000280 RID: 640
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000281 RID: 641
		private UtkaTCPServer _server;

		// Token: 0x04000282 RID: 642
		private string _key = string.Empty;

		// Token: 0x04000283 RID: 643
		private bool _initialized;
	}
}
