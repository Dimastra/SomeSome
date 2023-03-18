using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DB RID: 219
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaAuthenticationCommand : IUtkaCommand
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00014DDA File Offset: 0x00012FDA
		public string Name
		{
			get
			{
				return "handshake";
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00014DE1 File Offset: 0x00012FE1
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaHandshakeMessage);
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00014DF0 File Offset: 0x00012FF0
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			UtkaHandshakeMessage message = baseMessage as UtkaHandshakeMessage;
			if (message == null)
			{
				return;
			}
			IoCManager.InjectDependencies<UtkaAuthenticationCommand>(this);
			if (string.IsNullOrWhiteSpace(message.Key))
			{
				this.SendMessage(session, "key_missmatch");
				return;
			}
			if (this._configurationManager.GetCVar<string>(CCVars.UtkaSocketKey) != message.Key)
			{
				this.SendMessage(session, "key_missmatch");
				return;
			}
			if (session.Authenticated)
			{
				this.SendMessage(session, "already_authentificated");
				return;
			}
			session.Authenticated = true;
			this.SendMessage(session, "handshake_accepted");
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00014E7C File Offset: 0x0001307C
		private void SendMessage(UtkaTCPSession session, string message)
		{
			UtkaHandshakeMessage response = new UtkaHandshakeMessage
			{
				Key = this._configurationManager.GetCVar<string>(CCVars.UtkaSocketKey),
				Message = message
			};
			this._utkaTcpWrapper.SendMessageToClient(session, response);
		}

		// Token: 0x0400026E RID: 622
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x0400026F RID: 623
		[Dependency]
		private readonly UtkaTCPWrapper _utkaTcpWrapper;
	}
}
