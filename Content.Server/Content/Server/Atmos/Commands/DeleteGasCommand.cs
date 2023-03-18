using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B3 RID: 1971
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class DeleteGasCommand : IConsoleCommand
	{
		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06002ABE RID: 10942 RVA: 0x000E025C File Offset: 0x000DE45C
		public string Command
		{
			get
			{
				return "deletegas";
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06002ABF RID: 10943 RVA: 0x000E0263 File Offset: 0x000DE463
		public string Description
		{
			get
			{
				return "Removes all gases from a grid, or just of one type if specified.";
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06002AC0 RID: 10944 RVA: 0x000E026C File Offset: 0x000DE46C
		public string Help
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Usage: ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <GridId> <Gas> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <GridId> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <Gas> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		// Token: 0x06002AC1 RID: 10945 RVA: 0x000E02F0 File Offset: 0x000DE4F0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			Gas? gas = null;
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			EntityUid? gridId;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			switch (args.Length)
			{
			case 0:
			{
				if (player == null)
				{
					shell.WriteLine("A grid must be specified when the command isn't used by a player.");
					return;
				}
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEntity = attachedEntity.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						gridId = entMan.GetComponent<TransformComponent>(playerEntity).GridUid;
						if (gridId == null)
						{
							shell.WriteLine("You aren't on a grid to delete gas from.");
							return;
						}
						break;
					}
				}
				shell.WriteLine("You have no entity to get a grid from.");
				return;
			}
			case 1:
			{
				EntityUid number;
				if (!EntityUid.TryParse(args[0], ref number))
				{
					if (player == null)
					{
						shell.WriteLine("A grid id must be specified if not using this command as a player.");
						return;
					}
					EntityUid? attachedEntity = player.AttachedEntity;
					if (attachedEntity != null)
					{
						EntityUid playerEntity2 = attachedEntity.GetValueOrDefault();
						if (playerEntity2.Valid)
						{
							gridId = entMan.GetComponent<TransformComponent>(playerEntity2).GridUid;
							if (gridId == null)
							{
								shell.WriteLine("You aren't on a grid to delete gas from.");
								return;
							}
							Gas parsedGas;
							if (!Enum.TryParse<Gas>(args[0], true, out parsedGas))
							{
								shell.WriteLine(args[0] + " is not a valid gas name.");
								return;
							}
							gas = new Gas?(parsedGas);
							break;
						}
					}
					shell.WriteLine("You have no entity from which to get a grid id.");
					return;
				}
				else
				{
					gridId = new EntityUid?(number);
				}
				break;
			}
			case 2:
			{
				EntityUid first;
				if (!EntityUid.TryParse(args[0], ref first))
				{
					shell.WriteLine(args[0] + " is not a valid integer for a grid id.");
					return;
				}
				gridId = new EntityUid?(first);
				if (gridId.Value.IsValid())
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
					defaultInterpolatedStringHandler.AppendLiteral(" is not a valid grid id.");
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				Gas parsedGas2;
				if (!Enum.TryParse<Gas>(args[1], true, out parsedGas2))
				{
					shell.WriteLine(args[1] + " is not a valid gas.");
					return;
				}
				gas = new Gas?(parsedGas2);
				break;
			}
			default:
				shell.WriteLine(this.Help);
				return;
			}
			MapGridComponent mapGridComponent;
			if (!IoCManager.Resolve<IMapManager>().TryGetGrid(gridId, ref mapGridComponent))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No grid exists with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			AtmosphereSystem atmosphereSystem = EntitySystem.Get<AtmosphereSystem>();
			int tiles = 0;
			float moles = 0f;
			if (gas == null)
			{
				using (IEnumerator<GasMixture> enumerator = atmosphereSystem.GetAllMixtures(gridId.Value, true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GasMixture tile = enumerator.Current;
						if (!tile.Immutable)
						{
							tiles++;
							moles += tile.TotalMoles;
							tile.Clear();
						}
					}
					goto IL_307;
				}
			}
			foreach (GasMixture tile2 in atmosphereSystem.GetAllMixtures(gridId.Value, true))
			{
				if (!tile2.Immutable)
				{
					tiles++;
					moles += tile2.TotalMoles;
					tile2.SetMoles(gas.Value, 0f);
				}
			}
			IL_307:
			if (gas == null)
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Removed ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(moles);
				defaultInterpolatedStringHandler.AppendLiteral(" moles from ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(tiles);
				defaultInterpolatedStringHandler.AppendLiteral(" tiles.");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Removed ");
			defaultInterpolatedStringHandler.AppendFormatted<float>(moles);
			defaultInterpolatedStringHandler.AppendLiteral(" moles of gas ");
			defaultInterpolatedStringHandler.AppendFormatted<Gas?>(gas);
			defaultInterpolatedStringHandler.AppendLiteral(" from ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(tiles);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
