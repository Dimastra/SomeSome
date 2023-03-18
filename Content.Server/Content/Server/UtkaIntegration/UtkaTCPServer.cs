using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using Content.Shared.CCVar;
using NetCoreServer;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DF RID: 223
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaTCPServer : TcpServer
	{
		// Token: 0x06000405 RID: 1029 RVA: 0x0001515F File Offset: 0x0001335F
		protected override TcpSession CreateSession()
		{
			return new UtkaTCPSession(this);
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00015167 File Offset: 0x00013367
		public UtkaTCPServer(IPAddress address, int port) : base(address, port)
		{
			IoCManager.InjectDependencies<UtkaTCPServer>(this);
			this._cfg.OnValueChanged<string>(CCVars.UtkaSocketKey, delegate(string key)
			{
				this._key = key;
			}, true);
			base.OptionKeepAlive = true;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000151A8 File Offset: 0x000133A8
		public void SendMessageToAll(UtkaBaseMessage message)
		{
			foreach (UtkaTCPSession session in this.Sessions.Values.Cast<UtkaTCPSession>())
			{
				if (session.Authenticated)
				{
					session.SendAsync(JsonSerializer.Serialize(message, message.GetType(), null));
				}
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00015214 File Offset: 0x00013414
		public void SendMessageToClient(UtkaTCPSession session, UtkaBaseMessage message)
		{
			session.SendAsync(JsonSerializer.Serialize(message, message.GetType(), null));
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0001522C File Offset: 0x0001342C
		protected override void OnConnected(TcpSession session)
		{
			UtkaTCPSession utkaSession = (UtkaTCPSession)session;
			CancellationTokenSource cancellationToken = new CancellationTokenSource();
			utkaSession.OnMessageReceived += delegate([Nullable(2)] object sender, UtkaBaseMessage message)
			{
				this.ExecuteCommand(utkaSession, message);
			};
			Timer autoDisconnectionTimer = new Timer(25000, false, delegate()
			{
				if (!utkaSession.Authenticated)
				{
					utkaSession.Disconnect();
				}
			});
			this._timerManager.AddTimer(autoDisconnectionTimer, cancellationToken.Token);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00015299 File Offset: 0x00013499
		protected override void OnDisconnecting(TcpSession session)
		{
			this._authenticatedSessions.Remove(session as UtkaTCPSession);
			base.OnDisconnecting(session);
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x000152B4 File Offset: 0x000134B4
		protected override void OnError(SocketError error)
		{
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x000152B8 File Offset: 0x000134B8
		private void ExecuteCommand(UtkaTCPSession session, UtkaBaseMessage fromUtkaMessage)
		{
			string command = fromUtkaMessage.Command;
			if (!UtkaTCPServer.Commands.ContainsKey(command))
			{
				return;
			}
			this._taskManager.RunOnMainThread(delegate()
			{
				UtkaTCPServer.Commands[command].Execute(session, fromUtkaMessage);
			});
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00015314 File Offset: 0x00013514
		public static void RegisterCommands()
		{
			foreach (Type type2 in (from type in Assembly.GetExecutingAssembly().GetTypes()
			where typeof(IUtkaCommand).IsAssignableFrom(type) && type.GetInterfaces().Contains(typeof(IUtkaCommand))
			select type).ToList<Type>())
			{
				IUtkaCommand utkaCommand = Activator.CreateInstance(type2) as IUtkaCommand;
				if (utkaCommand != null)
				{
					UtkaTCPServer.Commands[utkaCommand.Name] = utkaCommand;
				}
			}
		}

		// Token: 0x04000278 RID: 632
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000279 RID: 633
		[Dependency]
		private readonly ITaskManager _taskManager;

		// Token: 0x0400027A RID: 634
		[Dependency]
		private readonly ITimerManager _timerManager;

		// Token: 0x0400027B RID: 635
		public static readonly Dictionary<string, IUtkaCommand> Commands = new Dictionary<string, IUtkaCommand>();

		// Token: 0x0400027C RID: 636
		private List<UtkaTCPSession> _authenticatedSessions = new List<UtkaTCPSession>();

		// Token: 0x0400027D RID: 637
		[Nullable(2)]
		private string _key;
	}
}
