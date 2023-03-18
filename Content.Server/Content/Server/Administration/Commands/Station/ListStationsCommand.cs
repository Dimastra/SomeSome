using System;
using System.Runtime.CompilerServices;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands.Station
{
	// Token: 0x0200086F RID: 2159
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ListStationsCommand : IConsoleCommand
	{
		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06002F32 RID: 12082 RVA: 0x000F4638 File Offset: 0x000F2838
		public string Command
		{
			get
			{
				return "lsstations";
			}
		}

		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x06002F33 RID: 12083 RVA: 0x000F463F File Offset: 0x000F283F
		public string Description
		{
			get
			{
				return "List all active stations";
			}
		}

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x06002F34 RID: 12084 RVA: 0x000F4646 File Offset: 0x000F2846
		public string Help
		{
			get
			{
				return "lsstations";
			}
		}

		// Token: 0x06002F35 RID: 12085 RVA: 0x000F4650 File Offset: 0x000F2850
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			foreach (EntityUid station in EntitySystem.Get<StationSystem>().Stations)
			{
				string name = this._entityManager.GetComponent<MetaDataComponent>(station).EntityName;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(station, -10);
				defaultInterpolatedStringHandler.AppendLiteral(" | ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x04001C66 RID: 7270
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
