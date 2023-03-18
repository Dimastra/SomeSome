using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Atmos.Prototypes;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B5 RID: 1973
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class ListGasesCommand : IConsoleCommand
	{
		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06002AC8 RID: 10952 RVA: 0x000E07B0 File Offset: 0x000DE9B0
		public string Command
		{
			get
			{
				return "listgases";
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06002AC9 RID: 10953 RVA: 0x000E07B7 File Offset: 0x000DE9B7
		public string Description
		{
			get
			{
				return "Prints a list of gases and their indices.";
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06002ACA RID: 10954 RVA: 0x000E07BE File Offset: 0x000DE9BE
		public string Help
		{
			get
			{
				return "listgases";
			}
		}

		// Token: 0x06002ACB RID: 10955 RVA: 0x000E07C8 File Offset: 0x000DE9C8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			foreach (GasPrototype gasPrototype in EntitySystem.Get<AtmosphereSystem>().Gases)
			{
				string gasName = Loc.GetString(gasPrototype.Name);
				shell.WriteLine(gasName + " ID: " + gasPrototype.ID);
			}
		}
	}
}
