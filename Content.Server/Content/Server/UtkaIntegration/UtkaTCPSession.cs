using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using NetCoreServer;
using Newtonsoft.Json.Linq;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000E0 RID: 224
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaTCPSession : TcpSession
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000410 RID: 1040 RVA: 0x000153C4 File Offset: 0x000135C4
		// (remove) Token: 0x06000411 RID: 1041 RVA: 0x000153FC File Offset: 0x000135FC
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event EventHandler<UtkaBaseMessage> OnMessageReceived;

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x00015431 File Offset: 0x00013631
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x00015439 File Offset: 0x00013639
		public bool Authenticated { get; set; }

		// Token: 0x06000414 RID: 1044 RVA: 0x00015442 File Offset: 0x00013642
		public UtkaTCPSession(TcpServer server) : base(server)
		{
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0001544C File Offset: 0x0001364C
		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			UtkaBaseMessage message;
			if (!this.ValidateMessage(buffer, offset, size, out message))
			{
				this.SendAsync("Validation fail");
				return;
			}
			EventHandler<UtkaBaseMessage> onMessageReceived = this.OnMessageReceived;
			if (onMessageReceived == null)
			{
				return;
			}
			onMessageReceived(this, message);
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00015485 File Offset: 0x00013685
		protected override void OnError(SocketError error)
		{
			this.SendAsync(error.ToString() ?? "");
			base.OnError(error);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000154AB File Offset: 0x000136AB
		protected override void OnConnected()
		{
			this.SendAsync("Hello from грабли, знай утка я ебал тебя в зад!!!");
			base.OnConnected();
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000154C0 File Offset: 0x000136C0
		private bool ValidateMessage(byte[] buffer, long offset, long size, [Nullable(2)] out UtkaBaseMessage fromDiscordMessage)
		{
			string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
			fromDiscordMessage = null;
			if (string.IsNullOrEmpty(message))
			{
				return false;
			}
			JToken commandName = JObject.Parse(message)["command"];
			if (commandName == null)
			{
				return false;
			}
			IUtkaCommand utkaCommand = UtkaTCPServer.Commands.Values.FirstOrDefault((IUtkaCommand x) => x.Name == commandName.ToString());
			if (utkaCommand == null)
			{
				return false;
			}
			Type messageType = utkaCommand.RequestMessageType;
			try
			{
				fromDiscordMessage = (JsonSerializer.Deserialize(message, messageType, null) as UtkaBaseMessage);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00015568 File Offset: 0x00013768
		protected override void OnDisconnected()
		{
			base.OnDisconnecting();
			base.Dispose();
		}
	}
}
