using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Content.Server.Chat.Managers;
using Robust.Server.ServerStatus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Server.MoMMI
{
	// Token: 0x0200039E RID: 926
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MoMMILink : IMoMMILink, IPostInjectInit
	{
		// Token: 0x060012E1 RID: 4833 RVA: 0x00061C52 File Offset: 0x0005FE52
		void IPostInjectInit.PostInject()
		{
			this._statusHost.AddHandler(new StatusHostHandlerAsync(this.HandleChatPost));
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x00061C6C File Offset: 0x0005FE6C
		public void SendOOCMessage(string sender, string message)
		{
			MoMMILink.<SendOOCMessage>d__6 <SendOOCMessage>d__;
			<SendOOCMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendOOCMessage>d__.<>4__this = this;
			<SendOOCMessage>d__.sender = sender;
			<SendOOCMessage>d__.message = message;
			<SendOOCMessage>d__.<>1__state = -1;
			<SendOOCMessage>d__.<>t__builder.Start<MoMMILink.<SendOOCMessage>d__6>(ref <SendOOCMessage>d__);
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x00061CB4 File Offset: 0x0005FEB4
		private Task SendMessageInternal(string type, object messageObject)
		{
			MoMMILink.<SendMessageInternal>d__7 <SendMessageInternal>d__;
			<SendMessageInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendMessageInternal>d__.<>4__this = this;
			<SendMessageInternal>d__.type = type;
			<SendMessageInternal>d__.messageObject = messageObject;
			<SendMessageInternal>d__.<>1__state = -1;
			<SendMessageInternal>d__.<>t__builder.Start<MoMMILink.<SendMessageInternal>d__7>(ref <SendMessageInternal>d__);
			return <SendMessageInternal>d__.<>t__builder.Task;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x00061D08 File Offset: 0x0005FF08
		private Task<bool> HandleChatPost(IStatusHandlerContext context)
		{
			MoMMILink.<HandleChatPost>d__8 <HandleChatPost>d__;
			<HandleChatPost>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<HandleChatPost>d__.<>4__this = this;
			<HandleChatPost>d__.context = context;
			<HandleChatPost>d__.<>1__state = -1;
			<HandleChatPost>d__.<>t__builder.Start<MoMMILink.<HandleChatPost>d__8>(ref <HandleChatPost>d__);
			return <HandleChatPost>d__.<>t__builder.Task;
		}

		// Token: 0x04000B8F RID: 2959
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000B90 RID: 2960
		[Dependency]
		private readonly IStatusHost _statusHost;

		// Token: 0x04000B91 RID: 2961
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000B92 RID: 2962
		[Dependency]
		private readonly ITaskManager _taskManager;

		// Token: 0x04000B93 RID: 2963
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x02000997 RID: 2455
		[Nullable(0)]
		private sealed class MoMMIMessageBase
		{
			// Token: 0x04002181 RID: 8577
			[JsonInclude]
			[JsonPropertyName("password")]
			public string Password;

			// Token: 0x04002182 RID: 8578
			[JsonInclude]
			[JsonPropertyName("type")]
			public string Type;

			// Token: 0x04002183 RID: 8579
			[JsonInclude]
			[JsonPropertyName("contents")]
			public object Contents;
		}

		// Token: 0x02000998 RID: 2456
		[Nullable(0)]
		private sealed class MoMMIMessageOOC
		{
			// Token: 0x04002184 RID: 8580
			[JsonInclude]
			[JsonPropertyName("sender")]
			public string Sender;

			// Token: 0x04002185 RID: 8581
			[JsonInclude]
			[JsonPropertyName("contents")]
			public string Contents;
		}

		// Token: 0x02000999 RID: 2457
		[Nullable(0)]
		private sealed class OOCPostMessage
		{
			// Token: 0x04002186 RID: 8582
			[JsonInclude]
			[JsonPropertyName("password")]
			public string Password;

			// Token: 0x04002187 RID: 8583
			[JsonInclude]
			[JsonPropertyName("sender")]
			public string Sender;

			// Token: 0x04002188 RID: 8584
			[JsonInclude]
			[JsonPropertyName("contents")]
			public string Contents;
		}
	}
}
