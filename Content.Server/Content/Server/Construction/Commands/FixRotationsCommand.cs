using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Power.Components;
using Content.Shared.Administration;
using Content.Shared.Construction;
using Content.Shared.Tag;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Construction.Commands
{
	// Token: 0x02000629 RID: 1577
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	internal sealed class FixRotationsCommand : IConsoleCommand
	{
		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06002198 RID: 8600 RVA: 0x000AEE8B File Offset: 0x000AD08B
		public string Command
		{
			get
			{
				return "fixrotations";
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06002199 RID: 8601 RVA: 0x000AEE92 File Offset: 0x000AD092
		public string Description
		{
			get
			{
				return "Sets the rotation of all occluders, low walls and windows to south.";
			}
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x0600219A RID: 8602 RVA: 0x000AEE99 File Offset: 0x000AD099
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <gridId> | " + this.Command;
			}
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x000AEEB8 File Offset: 0x000AD0B8
		public void Execute(IConsoleShell shell, string argsOther, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityQuery<TransformComponent> xformQuery = entityManager.GetEntityQuery<TransformComponent>();
			int num = args.Length;
			EntityUid? gridId;
			if (num == 0)
			{
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid playerEntity = entityUid.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						gridId = xformQuery.GetComponent(playerEntity).GridUid;
						goto IL_B5;
					}
				}
				shell.WriteError("Only a player can run this command.");
				return;
			}
			if (num != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			EntityUid id;
			if (!EntityUid.TryParse(args[0], ref id))
			{
				shell.WriteError(args[0] + " is not a valid entity.");
				return;
			}
			gridId = new EntityUid?(id);
			IL_B5:
			MapGridComponent grid;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!IoCManager.Resolve<IMapManager>().TryGetGrid(gridId, ref grid))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No grid exists with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!entityManager.EntityExists(grid.Owner))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Grid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have an associated grid entity.");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			int changed = 0;
			TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
			foreach (EntityUid child in xformQuery.GetComponent(grid.Owner).ChildEntities)
			{
				if (entityManager.EntityExists(child))
				{
					bool valid = false;
					OccluderComponent occluder;
					if (entityManager.TryGetComponent<OccluderComponent>(child, ref occluder))
					{
						valid |= occluder.Enabled;
					}
					valid |= entityManager.HasComponent<SharedCanBuildWindowOnTopComponent>(child);
					valid |= entityManager.HasComponent<CableComponent>(child);
					valid |= tagSystem.HasTag(child, "ForceFixRotations");
					valid &= !tagSystem.HasTag(child, "ForceNoFixRotations");
					if (valid)
					{
						TransformComponent childXform = xformQuery.GetComponent(child);
						if (childXform.LocalRotation != Angle.Zero)
						{
							childXform.LocalRotation = Angle.Zero;
							changed++;
						}
					}
				}
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Changed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(changed);
			defaultInterpolatedStringHandler.AppendLiteral(" entities. If things seem wrong, reconnect.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
