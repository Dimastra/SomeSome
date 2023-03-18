using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D2 RID: 1234
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class DelayStartCommand : IConsoleCommand
	{
		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06001976 RID: 6518 RVA: 0x000861CF File Offset: 0x000843CF
		public string Command
		{
			get
			{
				return "delaystart";
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x000861D6 File Offset: 0x000843D6
		public string Description
		{
			get
			{
				return "Delays the round start.";
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06001978 RID: 6520 RVA: 0x000861DD File Offset: 0x000843DD
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <seconds>\nPauses/Resumes the countdown if no argument is provided.";
			}
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x000861F4 File Offset: 0x000843F4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (ticker.RunLevel != GameRunLevel.PreRoundLobby)
			{
				shell.WriteLine("This can only be executed while the game is in the pre-round lobby.");
				return;
			}
			if (args.Length == 0)
			{
				shell.WriteLine(ticker.TogglePause() ? "Paused the countdown." : "Resumed the countdown.");
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteLine("Need zero or one arguments.");
				return;
			}
			uint seconds;
			if (!uint.TryParse(args[0], out seconds) || seconds == 0U)
			{
				shell.WriteLine(args[0] + " isn't a valid amount of seconds.");
				return;
			}
			TimeSpan time = TimeSpan.FromSeconds(seconds);
			if (!ticker.DelayStart(time))
			{
				shell.WriteLine("An unknown error has occurred.");
			}
		}
	}
}
