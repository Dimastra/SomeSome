using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.Commands
{
	// Token: 0x020003B1 RID: 945
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ZoomCommand : IConsoleCommand
	{
		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x00086D56 File Offset: 0x00084F56
		public string Command
		{
			get
			{
				return "zoom";
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x00086D5D File Offset: 0x00084F5D
		public string Description
		{
			get
			{
				return Loc.GetString("zoom-command-description");
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x00086D69 File Offset: 0x00084F69
		public string Help
		{
			get
			{
				return Loc.GetString("zoom-command-help");
			}
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00086D78 File Offset: 0x00084F78
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			int num = args.Length;
			if (num != 1 && num != 2)
			{
				shell.WriteLine(this.Help);
				return;
			}
			float num2;
			if (!float.TryParse(args[0], out num2))
			{
				shell.WriteError(Loc.GetString("cmd-parse-failure-float", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[0])
				}));
				return;
			}
			if (num2 > 0f)
			{
				Vector2 zoom;
				zoom..ctor(num2, num2);
				if (args.Length == 2)
				{
					float num3;
					if (!float.TryParse(args[1], out num3))
					{
						shell.WriteError(Loc.GetString("cmd-parse-failure-float", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("arg", args[1])
						}));
						return;
					}
					if (num3 <= 0f)
					{
						shell.WriteError(Loc.GetString("zoom-command-error"));
						return;
					}
					zoom.Y = num3;
				}
				this._eyeMan.CurrentEye.Zoom = zoom;
				return;
			}
			shell.WriteError(Loc.GetString("zoom-command-error"));
		}

		// Token: 0x04000C01 RID: 3073
		[Dependency]
		private readonly IEyeManager _eyeMan;
	}
}
