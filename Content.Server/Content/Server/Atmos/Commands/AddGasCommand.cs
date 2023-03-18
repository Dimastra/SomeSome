using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B2 RID: 1970
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class AddGasCommand : IConsoleCommand
	{
		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06002AB9 RID: 10937 RVA: 0x000E0154 File Offset: 0x000DE354
		public string Command
		{
			get
			{
				return "addgas";
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06002ABA RID: 10938 RVA: 0x000E015B File Offset: 0x000DE35B
		public string Description
		{
			get
			{
				return "Adds gas at a certain position.";
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x06002ABB RID: 10939 RVA: 0x000E0162 File Offset: 0x000DE362
		public string Help
		{
			get
			{
				return "addgas <X> <Y> <GridEid> <Gas> <moles>";
			}
		}

		// Token: 0x06002ABC RID: 10940 RVA: 0x000E016C File Offset: 0x000DE36C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 5)
			{
				return;
			}
			int x;
			int y;
			EntityUid euid;
			int gasId;
			float moles;
			if (!int.TryParse(args[0], out x) || !int.TryParse(args[1], out y) || !EntityUid.TryParse(args[2], ref euid) || !AtmosCommandUtils.TryParseGasID(args[3], out gasId) || !float.TryParse(args[4], out moles))
			{
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			if (!entMan.HasComponent<MapGridComponent>(euid))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Euid '");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
				defaultInterpolatedStringHandler.AppendLiteral("' does not exist or is not a grid.");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			AtmosphereSystem entitySystem = entMan.EntitySysManager.GetEntitySystem<AtmosphereSystem>();
			Vector2i indices;
			indices..ctor(x, y);
			GasMixture tile = entitySystem.GetTileMixture(new EntityUid?(euid), null, indices, true);
			if (tile == null)
			{
				shell.WriteLine("Invalid coordinates or tile.");
				return;
			}
			tile.AdjustMoles(gasId, moles);
		}
	}
}
