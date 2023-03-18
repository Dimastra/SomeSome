using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A9 RID: 937
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GroupingEntityMenuCommand : IConsoleCommand
	{
		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x00086818 File Offset: 0x00084A18
		public string Command
		{
			get
			{
				return "entitymenug";
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x0008681F File Offset: 0x00084A1F
		public string Description
		{
			get
			{
				return "Sets the entity menu grouping type.";
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x00086828 File Offset: 0x00084A28
		public string Help
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Usage: entitymenug <0:");
				defaultInterpolatedStringHandler.AppendFormatted<int>(2);
				defaultInterpolatedStringHandler.AppendLiteral(">");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x00086868 File Offset: 0x00084A68
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			int num;
			if (!int.TryParse(args[0], out num))
			{
				shell.WriteLine(args[0] + " is not a valid integer.");
				return;
			}
			if (num < 0 || num > 1)
			{
				shell.WriteLine(args[0] + " is not a valid integer.");
				return;
			}
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			CVarDef<int> entityMenuGroupingType = CCVars.EntityMenuGroupingType;
			configurationManager.SetCVar<int>(entityMenuGroupingType, num, false);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Context Menu Grouping set to type: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(configurationManager.GetCVar<int>(entityMenuGroupingType));
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
