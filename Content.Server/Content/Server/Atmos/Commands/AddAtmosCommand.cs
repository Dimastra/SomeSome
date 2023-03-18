using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B1 RID: 1969
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class AddAtmosCommand : IConsoleCommand
	{
		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06002AB4 RID: 10932 RVA: 0x000E0038 File Offset: 0x000DE238
		public string Command
		{
			get
			{
				return "addatmos";
			}
		}

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06002AB5 RID: 10933 RVA: 0x000E003F File Offset: 0x000DE23F
		public string Description
		{
			get
			{
				return "Adds atmos support to a grid.";
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06002AB6 RID: 10934 RVA: 0x000E0046 File Offset: 0x000DE246
		public string Help
		{
			get
			{
				return this.Command + " <GridId>";
			}
		}

		// Token: 0x06002AB7 RID: 10935 RVA: 0x000E0058 File Offset: 0x000DE258
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid euid;
			if (!EntityUid.TryParse(args[0], ref euid))
			{
				shell.WriteError("Failed to parse euid '" + args[0] + "'.");
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!entMan.HasComponent<MapGridComponent>(euid))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Euid '");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
				defaultInterpolatedStringHandler.AppendLiteral("' does not exist or is not a grid.");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (entMan.EntitySysManager.GetEntitySystem<AtmosphereSystem>().HasAtmosphere(euid))
			{
				shell.WriteLine("Grid already has an atmosphere.");
				return;
			}
			this._entities.AddComponent<GridAtmosphereComponent>(euid);
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Added atmosphere to grid ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x04001A83 RID: 6787
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
