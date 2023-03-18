using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Ghost.Components;
using Content.Server.Warps;
using Content.Shared.Administration;
using Content.Shared.Follower;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200086C RID: 2156
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class WarpCommand : IConsoleCommand
	{
		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06002F21 RID: 12065 RVA: 0x000F40AC File Offset: 0x000F22AC
		public string Command
		{
			get
			{
				return "warp";
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06002F22 RID: 12066 RVA: 0x000F40B3 File Offset: 0x000F22B3
		public string Description
		{
			get
			{
				return "Teleports you to predefined areas on the map.";
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06002F23 RID: 12067 RVA: 0x000F40BA File Offset: 0x000F22BA
		public string Help
		{
			get
			{
				return "warp <location>\nLocations you can teleport to are predefined by the map. You can specify '?' as location to get a list of valid locations.";
			}
		}

		// Token: 0x06002F24 RID: 12068 RVA: 0x000F40C4 File Offset: 0x000F22C4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			WarpCommand.<>c__DisplayClass7_0 CS$<>8__locals1 = new WarpCommand.<>c__DisplayClass7_0();
			CS$<>8__locals1.<>4__this = this;
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("Only players can use this command");
				return;
			}
			if (args.Length != 1)
			{
				shell.WriteLine("Expected a single argument.");
				return;
			}
			CS$<>8__locals1.location = args[0];
			if (CS$<>8__locals1.location == "?")
			{
				string locations = string.Join(", ", this.GetWarpPointNames());
				shell.WriteLine(locations);
				return;
			}
			if (player.Status == 3)
			{
				EntityUid? attachedEntity = player.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEntity = attachedEntity.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						MapId currentMap = this._entManager.GetComponent<TransformComponent>(playerEntity).MapID;
						EntityUid? currentGrid = this._entManager.GetComponent<TransformComponent>(playerEntity).GridUid;
						ValueTuple<EntityCoordinates, bool> valueTuple = (from p in this._entManager.EntityQuery<WarpPointComponent>(true)
						where p.Location == CS$<>8__locals1.location
						select new ValueTuple<EntityCoordinates, bool>(CS$<>8__locals1.<>4__this._entManager.GetComponent<TransformComponent>(p.Owner).Coordinates, p.Follow)).OrderBy(([TupleElementNames(new string[]
						{
							"Coordinates",
							"Follow"
						})] ValueTuple<EntityCoordinates, bool> p) => p.Item1, Comparer<EntityCoordinates>.Create(delegate(EntityCoordinates a, EntityCoordinates b)
						{
							EntityUid? aGrid = a.GetGridUid(CS$<>8__locals1.<>4__this._entManager);
							EntityUid? bGrid = b.GetGridUid(CS$<>8__locals1.<>4__this._entManager);
							if (aGrid == bGrid)
							{
								return 0;
							}
							if (aGrid == currentGrid)
							{
								return -1;
							}
							if (bGrid == currentGrid)
							{
								return 1;
							}
							MapId mapA = a.GetMapId(CS$<>8__locals1.<>4__this._entManager);
							MapId mapB = a.GetMapId(CS$<>8__locals1.<>4__this._entManager);
							if (mapA == mapB)
							{
								return 0;
							}
							if (mapA == currentMap)
							{
								return -1;
							}
							if (mapB == currentMap)
							{
								return 1;
							}
							return 0;
						})).FirstOrDefault<ValueTuple<EntityCoordinates, bool>>();
						EntityCoordinates coords = valueTuple.Item1;
						bool follow = valueTuple.Item2;
						if (coords.EntityId == EntityUid.Invalid)
						{
							shell.WriteError("That location does not exist!");
							return;
						}
						if (follow && this._entManager.HasComponent<GhostComponent>(playerEntity))
						{
							this._entManager.System<FollowerSystem>().StartFollowingEntity(playerEntity, coords.EntityId);
							return;
						}
						TransformComponent component = this._entManager.GetComponent<TransformComponent>(playerEntity);
						component.Coordinates = coords;
						component.AttachToGridOrMap();
						PhysicsComponent physics;
						if (this._entManager.TryGetComponent<PhysicsComponent>(playerEntity, ref physics))
						{
							this._entManager.System<SharedPhysicsSystem>().SetLinearVelocity(playerEntity, Vector2.Zero, true, true, null, physics);
						}
						return;
					}
				}
			}
			shell.WriteLine("You are not in-game!");
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x000F42C8 File Offset: 0x000F24C8
		private IEnumerable<string> GetWarpPointNames()
		{
			return (from p in this._entManager.EntityQuery<WarpPointComponent>(true)
			select p.Location into p
			where p != null
			orderby p
			select p).Distinct<string>();
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x000F4352 File Offset: 0x000F2552
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(new string[]
				{
					"?"
				}.Concat(this.GetWarpPointNames()), "<warp point | ?>");
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C65 RID: 7269
		[Dependency]
		private readonly IEntityManager _entManager;
	}
}
