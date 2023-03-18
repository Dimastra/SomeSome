using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085E RID: 2142
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RollCommand : IConsoleCommand
	{
		// Token: 0x170007A2 RID: 1954
		// (get) Token: 0x06002ED8 RID: 11992 RVA: 0x000F301C File Offset: 0x000F121C
		public string Command
		{
			get
			{
				return "roll";
			}
		}

		// Token: 0x170007A3 RID: 1955
		// (get) Token: 0x06002ED9 RID: 11993 RVA: 0x000F3023 File Offset: 0x000F1223
		public string Description
		{
			get
			{
				return "Roll a number from 1 to specified value.";
			}
		}

		// Token: 0x170007A4 RID: 1956
		// (get) Token: 0x06002EDA RID: 11994 RVA: 0x000F302A File Offset: 0x000F122A
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <value:int>.";
			}
		}

		// Token: 0x06002EDB RID: 11995 RVA: 0x000F3044 File Offset: 0x000F1244
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You cannot use this command from the server console.");
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			int maxNum;
			if (!int.TryParse(args[0], out maxNum))
			{
				shell.WriteLine(this.Help);
				return;
			}
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			IChatManager chatManager = IoCManager.Resolve<IChatManager>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 3);
			defaultInterpolatedStringHandler.AppendFormatted(player.Name);
			defaultInterpolatedStringHandler.AppendLiteral(" has thrown the D");
			defaultInterpolatedStringHandler.AppendFormatted<int>(maxNum);
			defaultInterpolatedStringHandler.AppendLiteral(" and the ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(random.Next(1, maxNum));
			defaultInterpolatedStringHandler.AppendLiteral(" rolled.");
			chatManager.DispatchServerAnnouncement(defaultInterpolatedStringHandler.ToStringAndClear(), null);
		}
	}
}
