using System;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.Nodes;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028E RID: 654
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CableVisSystem : EntitySystem
	{
		// Token: 0x06000D1F RID: 3359 RVA: 0x00044BEB File Offset: 0x00042DEB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CableVisComponent, NodeGroupsRebuilt>(new ComponentEventRefHandler<CableVisComponent, NodeGroupsRebuilt>(this.UpdateAppearance), null, null);
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x00044C08 File Offset: 0x00042E08
		private void UpdateAppearance(EntityUid uid, CableVisComponent cableVis, ref NodeGroupsRebuilt args)
		{
			if (cableVis.Node == null)
			{
				return;
			}
			NodeContainerComponent nodeContainer;
			AppearanceComponent appearance;
			if (!base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) || !base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			TransformComponent transform = base.Transform(uid);
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(transform.GridUid, ref grid))
			{
				return;
			}
			WireVisDirFlags mask = WireVisDirFlags.None;
			Vector2i tile = grid.TileIndicesFor(transform.Coordinates);
			foreach (Node reachable in nodeContainer.GetNode<CableNode>(cableVis.Node).ReachableNodes)
			{
				if (reachable is CableNode)
				{
					TransformComponent otherTransform = base.Transform(reachable.Owner);
					Vector2i diff = grid.TileIndicesFor(otherTransform.Coordinates) - tile;
					WireVisDirFlags wireVisDirFlags = mask;
					int num;
					int num2;
					diff.Deconstruct(ref num, ref num2);
					WireVisDirFlags wireVisDirFlags2;
					switch (num)
					{
					case -1:
						if (num2 != 0)
						{
							goto IL_F8;
						}
						wireVisDirFlags2 = WireVisDirFlags.West;
						break;
					case 0:
						if (num2 != -1)
						{
							if (num2 != 1)
							{
								goto IL_F8;
							}
							wireVisDirFlags2 = WireVisDirFlags.North;
						}
						else
						{
							wireVisDirFlags2 = WireVisDirFlags.South;
						}
						break;
					case 1:
						if (num2 != 0)
						{
							goto IL_F8;
						}
						wireVisDirFlags2 = WireVisDirFlags.East;
						break;
					default:
						goto IL_F8;
					}
					IL_FB:
					mask = (wireVisDirFlags | wireVisDirFlags2);
					continue;
					IL_F8:
					wireVisDirFlags2 = WireVisDirFlags.None;
					goto IL_FB;
				}
			}
			this._appearance.SetData(uid, WireVisVisuals.ConnectedMask, mask, appearance);
		}

		// Token: 0x040007EF RID: 2031
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040007F0 RID: 2032
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
