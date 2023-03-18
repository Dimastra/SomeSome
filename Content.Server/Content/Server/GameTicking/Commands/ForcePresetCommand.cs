using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D5 RID: 1237
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class ForcePresetCommand : IConsoleCommand
	{
		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06001986 RID: 6534 RVA: 0x000863F2 File Offset: 0x000845F2
		public string Command
		{
			get
			{
				return "forcepreset";
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06001987 RID: 6535 RVA: 0x000863F9 File Offset: 0x000845F9
		public string Description
		{
			get
			{
				return "Forces a specific game preset to start for the current lobby.";
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06001988 RID: 6536 RVA: 0x00086400 File Offset: 0x00084600
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <preset>";
			}
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x00086418 File Offset: 0x00084618
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (ticker.RunLevel != GameRunLevel.PreRoundLobby)
			{
				shell.WriteLine("This can only be executed while the game is in the pre-round lobby.");
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteLine("Need exactly one argument.");
				return;
			}
			string name = args[0];
			GamePresetPrototype type;
			if (!ticker.TryFindGamePreset(name, out type))
			{
				shell.WriteLine("No preset exists with name " + name + ".");
				return;
			}
			ticker.SetGamePreset(type, true);
			shell.WriteLine("Forced the game to start with preset " + name + ".");
			ticker.UpdateInfoText();
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x0008649C File Offset: 0x0008469C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(from p in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<GamePresetPrototype>()
				orderby p.ID
				select p.ID, "<preset>");
			}
			return CompletionResult.Empty;
		}
	}
}
