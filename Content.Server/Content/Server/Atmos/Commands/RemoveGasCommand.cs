using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B6 RID: 1974
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class RemoveGasCommand : IConsoleCommand
	{
		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06002ACD RID: 10957 RVA: 0x000E0840 File Offset: 0x000DEA40
		public string Command
		{
			get
			{
				return "removegas";
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06002ACE RID: 10958 RVA: 0x000E0847 File Offset: 0x000DEA47
		public string Description
		{
			get
			{
				return "Removes an amount of gases.";
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06002ACF RID: 10959 RVA: 0x000E084E File Offset: 0x000DEA4E
		public string Help
		{
			get
			{
				return "removegas <X> <Y> <GridId> <amount> <ratio>\nIf <ratio> is true, amount will be treated as the ratio of gas to be removed.";
			}
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x000E0858 File Offset: 0x000DEA58
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 5)
			{
				return;
			}
			int x;
			int y;
			EntityUid id;
			float amount;
			bool ratio;
			if (!int.TryParse(args[0], out x) || !int.TryParse(args[1], out y) || !EntityUid.TryParse(args[2], ref id) || !float.TryParse(args[3], out amount) || !bool.TryParse(args[4], out ratio))
			{
				return;
			}
			AtmosphereSystem atmosphereSystem = EntitySystem.Get<AtmosphereSystem>();
			Vector2i indices;
			indices..ctor(x, y);
			GasMixture tile = atmosphereSystem.GetTileMixture(new EntityUid?(id), null, indices, true);
			if (tile == null)
			{
				shell.WriteLine("Invalid coordinates or tile.");
				return;
			}
			if (ratio)
			{
				tile.RemoveRatio(amount);
				return;
			}
			tile.Remove(amount);
		}
	}
}
