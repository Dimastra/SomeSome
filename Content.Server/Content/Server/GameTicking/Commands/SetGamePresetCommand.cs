using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.GameTicking.Presets;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DC RID: 1244
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class SetGamePresetCommand : IConsoleCommand
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060019AA RID: 6570 RVA: 0x0008684B File Offset: 0x00084A4B
		public string Command
		{
			get
			{
				return "setgamepreset";
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x060019AB RID: 6571 RVA: 0x00086852 File Offset: 0x00084A52
		public string Description
		{
			get
			{
				return Loc.GetString("set-game-preset-command-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060019AC RID: 6572 RVA: 0x0008687B File Offset: 0x00084A7B
		public string Help
		{
			get
			{
				return Loc.GetString("set-game-preset-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x000868A4 File Offset: 0x00084AA4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("properAmount", 1),
					new ValueTuple<string, object>("currentAmount", args.Length)
				}));
				return;
			}
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			GamePresetPrototype preset;
			if (!ticker.TryFindGamePreset(args[0], out preset))
			{
				shell.WriteError(Loc.GetString("set-game-preset-preset-error", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("preset", args[0])
				}));
				return;
			}
			ticker.SetGamePreset(preset, false);
			shell.WriteLine(Loc.GetString("set-game-preset-preset-set", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("preset", preset.ID)
			}));
		}
	}
}
