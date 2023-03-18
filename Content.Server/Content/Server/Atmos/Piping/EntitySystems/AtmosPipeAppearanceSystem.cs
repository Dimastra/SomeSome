using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.EntitySystems
{
	// Token: 0x0200075F RID: 1887
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosPipeAppearanceSystem : EntitySystem
	{
		// Token: 0x060027FE RID: 10238 RVA: 0x000D194A File Offset: 0x000CFB4A
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PipeAppearanceComponent, NodeGroupsRebuilt>(new ComponentEventRefHandler<PipeAppearanceComponent, NodeGroupsRebuilt>(this.OnNodeUpdate), null, null);
		}

		// Token: 0x060027FF RID: 10239 RVA: 0x000D1966 File Offset: 0x000CFB66
		private void OnNodeUpdate(EntityUid uid, PipeAppearanceComponent component, ref NodeGroupsRebuilt args)
		{
			this.UpdateAppearance(args.NodeOwner, null, null, null);
		}

		// Token: 0x06002800 RID: 10240 RVA: 0x000D1978 File Offset: 0x000CFB78
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, AppearanceComponent appearance = null, NodeContainerComponent container = null, TransformComponent xform = null)
		{
			if (!base.Resolve<AppearanceComponent, NodeContainerComponent, TransformComponent>(uid, ref appearance, ref container, ref xform, false))
			{
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				return;
			}
			bool anyPipeNodes = false;
			HashSet<EntityUid> connected = new HashSet<EntityUid>();
			foreach (Node node in container.Nodes.Values)
			{
				if (node is PipeNode)
				{
					anyPipeNodes = true;
					foreach (Node connectedNode in node.ReachableNodes)
					{
						if (connectedNode is PipeNode)
						{
							connected.Add(connectedNode.Owner);
						}
					}
				}
			}
			if (!anyPipeNodes)
			{
				return;
			}
			PipeDirection netConnectedDirections = PipeDirection.None;
			Vector2i tile = grid.TileIndicesFor(xform.Coordinates);
			foreach (EntityUid neighbour in connected)
			{
				Vector2i vector2i = grid.TileIndicesFor(base.Transform(neighbour).Coordinates);
				PipeDirection pipeDirection = netConnectedDirections;
				int num;
				int num2;
				(vector2i - tile).Deconstruct(ref num, ref num2);
				PipeDirection pipeDirection2;
				switch (num)
				{
				case -1:
					if (num2 != 0)
					{
						goto IL_14F;
					}
					pipeDirection2 = PipeDirection.West;
					break;
				case 0:
					if (num2 != -1)
					{
						if (num2 != 1)
						{
							goto IL_14F;
						}
						pipeDirection2 = PipeDirection.North;
					}
					else
					{
						pipeDirection2 = PipeDirection.South;
					}
					break;
				case 1:
					if (num2 != 0)
					{
						goto IL_14F;
					}
					pipeDirection2 = PipeDirection.East;
					break;
				default:
					goto IL_14F;
				}
				IL_152:
				netConnectedDirections = (pipeDirection | pipeDirection2);
				continue;
				IL_14F:
				pipeDirection2 = PipeDirection.None;
				goto IL_152;
			}
			this._appearance.SetData(uid, PipeVisuals.VisualState, netConnectedDirections, appearance);
		}

		// Token: 0x040018E9 RID: 6377
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040018EA RID: 6378
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
