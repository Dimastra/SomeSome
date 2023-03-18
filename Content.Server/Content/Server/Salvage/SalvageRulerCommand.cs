using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Salvage
{
	// Token: 0x02000221 RID: 545
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class SalvageRulerCommand : IConsoleCommand
	{
		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000AC5 RID: 2757 RVA: 0x00038374 File Offset: 0x00036574
		public string Command
		{
			get
			{
				return "salvageruler";
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x0003837B File Offset: 0x0003657B
		public string Description
		{
			get
			{
				return Loc.GetString("salvage-ruler-command-description");
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000AC7 RID: 2759 RVA: 0x00038387 File Offset: 0x00036587
		public string Help
		{
			get
			{
				return Loc.GetString("salvage-ruler-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x000383B0 File Offset: 0x000365B0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 0)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError(Loc.GetString("shell-only-players-can-run-this-command"));
				return;
			}
			EntityUid? entity = player.AttachedEntity;
			if (entity == null)
			{
				shell.WriteError(Loc.GetString("shell-must-be-attached-to-entity"));
				return;
			}
			TransformComponent entityTransform = this._entities.GetComponent<TransformComponent>(entity.Value);
			Box2 total = Box2.UnitCentered;
			bool first = true;
			foreach (MapGridComponent mapGrid in this._maps.GetAllMapGrids(entityTransform.MapID))
			{
				Matrix3 worldMatrix = this._entities.GetComponent<TransformComponent>(mapGrid.Owner).WorldMatrix;
				Box2 localAABB = mapGrid.LocalAABB;
				Box2 aabb = worldMatrix.TransformBox(ref localAABB);
				if (first)
				{
					total = aabb;
				}
				else
				{
					total = total.ExtendToContain(aabb.TopLeft);
					total = total.ExtendToContain(aabb.TopRight);
					total = total.ExtendToContain(aabb.BottomLeft);
					total = total.ExtendToContain(aabb.BottomRight);
				}
				first = false;
			}
			shell.WriteLine(total.ToString());
		}

		// Token: 0x040006A3 RID: 1699
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x040006A4 RID: 1700
		[Dependency]
		private readonly IMapManager _maps;
	}
}
