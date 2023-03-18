using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D6 RID: 1238
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class GoLobbyCommand : IConsoleCommand
	{
		// Token: 0x170003BD RID: 957
		// (get) Token: 0x0600198C RID: 6540 RVA: 0x00086519 File Offset: 0x00084719
		public string Command
		{
			get
			{
				return "golobby";
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x0600198D RID: 6541 RVA: 0x00086520 File Offset: 0x00084720
		public string Description
		{
			get
			{
				return "Enables the lobby and restarts the round.";
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x0600198E RID: 6542 RVA: 0x00086528 File Offset: 0x00084728
		public string Help
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Usage: ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <preset>");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x00086584 File Offset: 0x00084784
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GamePresetPrototype preset = null;
			string presetName = string.Join(" ", args);
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (args.Length != 0 && !ticker.TryFindGamePreset(presetName, out preset))
			{
				shell.WriteLine("No preset found with name " + presetName);
				return;
			}
			IoCManager.Resolve<IConfigurationManager>().SetCVar<bool>(CCVars.GameLobbyEnabled, true, false);
			ticker.RestartRound();
			if (preset != null)
			{
				ticker.SetGamePreset(preset, false);
			}
			shell.WriteLine("Enabling the lobby and restarting the round." + ((preset == null) ? "" : ("\nPreset set to " + presetName)));
		}
	}
}
