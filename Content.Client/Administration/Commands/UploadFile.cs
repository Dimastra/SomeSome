using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;

namespace Content.Client.Administration.Commands
{
	// Token: 0x020004EE RID: 1262
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UploadFile : IConsoleCommand
	{
		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x0600200D RID: 8205 RVA: 0x000BA214 File Offset: 0x000B8414
		public string Command
		{
			get
			{
				return "uploadfile";
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600200E RID: 8206 RVA: 0x000BA21B File Offset: 0x000B841B
		public string Description
		{
			get
			{
				return "Uploads a resource to the server.";
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x0600200F RID: 8207 RVA: 0x000BA222 File Offset: 0x000B8422
		public string Help
		{
			get
			{
				return this.Command + " [relative path for the resource]";
			}
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000BA234 File Offset: 0x000B8434
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			UploadFile.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<UploadFile.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
