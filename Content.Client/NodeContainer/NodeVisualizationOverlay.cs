using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Resources;
using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.NodeContainer
{
	// Token: 0x0200021A RID: 538
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeVisualizationOverlay : Overlay
	{
		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000E0E RID: 3598 RVA: 0x000392B9 File Offset: 0x000374B9
		public override OverlaySpace Space
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06000E0F RID: 3599 RVA: 0x00054D8C File Offset: 0x00052F8C
		public NodeVisualizationOverlay(NodeGroupSystem system, EntityLookupSystem lookup, IMapManager mapManager, IInputManager inputManager, IResourceCache cache, IEntityManager entityManager)
		{
			this._system = system;
			this._lookup = lookup;
			this._mapManager = mapManager;
			this._inputManager = inputManager;
			this._entityManager = entityManager;
			this._font = cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
		}

		// Token: 0x06000E10 RID: 3600 RVA: 0x00054DEE File Offset: 0x00052FEE
		protected override void Draw(in OverlayDrawArgs args)
		{
			if ((args.Space & 4) != null)
			{
				this.DrawWorld(args);
				return;
			}
			if ((args.Space & 2) != null)
			{
				this.DrawScreen(args);
			}
		}

		// Token: 0x06000E11 RID: 3601 RVA: 0x00054E14 File Offset: 0x00053014
		private void DrawScreen(in OverlayDrawArgs args)
		{
			Vector2 position = this._inputManager.MouseScreenPosition.Position;
			this._mouseWorldPos = args.ViewportControl.ScreenToMap(new Vector2(position.X, position.Y)).Position;
			if (this._hovered == null)
			{
				return;
			}
			ValueTuple<int, int> value = this._hovered.Value;
			int item = value.Item1;
			int item2 = value.Item2;
			NodeVis.GroupData groupData = this._system.Groups[item];
			NodeVis.NodeDatum nodeDatum = this._system.NodeLookup[new ValueTuple<int, int>(item, item2)];
			TransformComponent component = this._entityManager.GetComponent<TransformComponent>(nodeDatum.Entity);
			MapGridComponent mapGridComponent;
			if (!this._mapManager.TryGetGrid(component.GridUid, ref mapGridComponent))
			{
				return;
			}
			Vector2i value2 = mapGridComponent.TileIndicesFor(component.Coordinates);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
			appendInterpolatedStringHandler.AppendLiteral("entity: ");
			appendInterpolatedStringHandler.AppendFormatted<EntityUid>(nodeDatum.Entity);
			appendInterpolatedStringHandler.AppendLiteral("\n");
			stringBuilder3.Append(ref appendInterpolatedStringHandler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
			appendInterpolatedStringHandler.AppendLiteral("group id: ");
			appendInterpolatedStringHandler.AppendFormatted(groupData.GroupId);
			appendInterpolatedStringHandler.AppendLiteral("\n");
			stringBuilder4.Append(ref appendInterpolatedStringHandler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			appendInterpolatedStringHandler.AppendLiteral("node: ");
			appendInterpolatedStringHandler.AppendFormatted(nodeDatum.Name);
			appendInterpolatedStringHandler.AppendLiteral("\n");
			stringBuilder5.Append(ref appendInterpolatedStringHandler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			appendInterpolatedStringHandler.AppendLiteral("type: ");
			appendInterpolatedStringHandler.AppendFormatted(nodeDatum.Type);
			appendInterpolatedStringHandler.AppendLiteral("\n");
			stringBuilder6.Append(ref appendInterpolatedStringHandler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
			appendInterpolatedStringHandler.AppendLiteral("grid pos: ");
			appendInterpolatedStringHandler.AppendFormatted<Vector2i>(value2);
			appendInterpolatedStringHandler.AppendLiteral("\n");
			stringBuilder7.Append(ref appendInterpolatedStringHandler);
			stringBuilder.Append(groupData.DebugData);
			args.ScreenHandle.DrawString(this._font, position + new ValueTuple<float, float>(20f, -20f), stringBuilder.ToString());
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x00055068 File Offset: 0x00053268
		private void DrawWorld(in OverlayDrawArgs overlayDrawArgs)
		{
			DrawingHandleWorld worldHandle = overlayDrawArgs.WorldHandle;
			IEye eye = overlayDrawArgs.Viewport.Eye;
			MapId mapId = (eye != null) ? eye.Position.MapId : default(MapId);
			if (mapId == MapId.Nullspace)
			{
				return;
			}
			this._hovered = null;
			Box2 box = Box2.CenteredAround(this._mouseWorldPos, new ValueTuple<float, float>(0.25f, 0.25f));
			Box2 worldAABB = overlayDrawArgs.WorldAABB;
			EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
			foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapId, worldAABB, false))
			{
				foreach (EntityUid entityUid in this._lookup.GetEntitiesIntersecting(mapGridComponent.Owner, worldAABB, 46))
				{
					ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>[] array;
					if (this._system.Entities.TryGetValue(entityUid, out array))
					{
						Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>> orNew = Extensions.GetOrNew<EntityUid, Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>>>(this._gridIndex, mapGridComponent.Owner);
						EntityCoordinates coordinates = entityQuery.GetComponent(entityUid).Coordinates;
						if (!float.IsNaN(coordinates.Position.X) && !float.IsNaN(coordinates.Position.Y))
						{
							List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>> orNew2 = Extensions.GetOrNew<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>>(orNew, mapGridComponent.TileIndicesFor(coordinates));
							foreach (ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> valueTuple in array)
							{
								NodeVis.GroupData item = valueTuple.Item1;
								NodeVis.NodeDatum item2 = valueTuple.Item2;
								if (!this._system.Filtered.Contains(item.GroupId))
								{
									orNew2.Add(new ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>(item, item2));
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<EntityUid, Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>>> keyValuePair in this._gridIndex)
			{
				EntityUid entityUid2;
				Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>> dictionary;
				keyValuePair.Deconstruct(out entityUid2, out dictionary);
				EntityUid entityUid3 = entityUid2;
				Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>> dictionary2 = dictionary;
				MapGridComponent grid = this._mapManager.GetGrid(entityUid3);
				ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = this._entityManager.GetComponent<TransformComponent>(grid.Owner).GetWorldPositionRotationMatrixWithInv();
				Matrix3 item3 = worldPositionRotationMatrixWithInv.Item3;
				Matrix3 item4 = worldPositionRotationMatrixWithInv.Item4;
				Box2 box2 = item4.TransformBox(ref box);
				foreach (KeyValuePair<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>> keyValuePair2 in dictionary2)
				{
					Vector2i vector2i;
					List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>> list;
					keyValuePair2.Deconstruct(out vector2i, out list);
					Vector2i vector2i2 = vector2i;
					List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>> list2 = list;
					Vector2 vector = vector2i2 + (float)grid.TileSize / 2f;
					list2.Sort(NodeVisualizationOverlay.NodeDisplayComparer.Instance);
					float num = (float)(-(float)(list2.Count - 1)) * 0.1875f / 2f;
					foreach (ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> valueTuple2 in list2)
					{
						NodeVis.GroupData item5 = valueTuple2.Item1;
						NodeVis.NodeDatum item6 = valueTuple2.Item2;
						Vector2 vector2 = vector + new ValueTuple<float, float>(num, num);
						if (box2.Contains(vector2, true))
						{
							this._hovered = new ValueTuple<int, int>?(new ValueTuple<int, int>(item5.NetId, item6.NetId));
						}
						this._nodeIndex[new ValueTuple<int, int>(item5.NetId, item6.NetId)] = new NodeVisualizationOverlay.NodeRenderData(item5, item6, vector2);
						num += 0.1875f;
					}
				}
				worldHandle.SetTransform(ref item3);
				foreach (NodeVisualizationOverlay.NodeRenderData nodeRenderData in this._nodeIndex.Values)
				{
					Vector2 nodePos = nodeRenderData.NodePos;
					Box2 box3 = Box2.CenteredAround(nodePos, new ValueTuple<float, float>(0.25f, 0.25f));
					NodeVis.GroupData groupData = nodeRenderData.GroupData;
					Color color = groupData.Color;
					if (this._hovered == null)
					{
						color.A = 0.5f;
					}
					else if (this._hovered.Value.Item1 != groupData.NetId)
					{
						color.A = 0.2f;
					}
					else
					{
						color.A = 0.75f + MathF.Sin(this._time * 4f) * 0.25f;
					}
					worldHandle.DrawRect(box3, color, true);
					foreach (int item7 in nodeRenderData.NodeDatum.Reachable)
					{
						NodeVisualizationOverlay.NodeRenderData nodeRenderData2;
						if (this._nodeIndex.TryGetValue(new ValueTuple<int, int>(groupData.NetId, item7), out nodeRenderData2))
						{
							worldHandle.DrawLine(nodePos, nodeRenderData2.NodePos, color);
						}
					}
				}
				this._nodeIndex.Clear();
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
			this._gridIndex.Clear();
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x000555F0 File Offset: 0x000537F0
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			this._time += args.DeltaSeconds;
		}

		// Token: 0x040006F0 RID: 1776
		private readonly NodeGroupSystem _system;

		// Token: 0x040006F1 RID: 1777
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040006F2 RID: 1778
		private readonly IMapManager _mapManager;

		// Token: 0x040006F3 RID: 1779
		private readonly IInputManager _inputManager;

		// Token: 0x040006F4 RID: 1780
		private readonly IEntityManager _entityManager;

		// Token: 0x040006F5 RID: 1781
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private readonly Dictionary<ValueTuple<int, int>, NodeVisualizationOverlay.NodeRenderData> _nodeIndex = new Dictionary<ValueTuple<int, int>, NodeVisualizationOverlay.NodeRenderData>();

		// Token: 0x040006F6 RID: 1782
		[Nullable(new byte[]
		{
			1,
			1,
			1,
			0,
			1,
			1
		})]
		private readonly Dictionary<EntityUid, Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>>> _gridIndex = new Dictionary<EntityUid, Dictionary<Vector2i, List<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>>>();

		// Token: 0x040006F7 RID: 1783
		private readonly Font _font;

		// Token: 0x040006F8 RID: 1784
		private Vector2 _mouseWorldPos;

		// Token: 0x040006F9 RID: 1785
		[TupleElementNames(new string[]
		{
			"group",
			"node"
		})]
		[Nullable(0)]
		private ValueTuple<int, int>? _hovered;

		// Token: 0x040006FA RID: 1786
		private float _time;

		// Token: 0x0200021B RID: 539
		[NullableContext(0)]
		private sealed class NodeDisplayComparer : IComparer<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>
		{
			// Token: 0x06000E14 RID: 3604 RVA: 0x00055610 File Offset: 0x00053810
			public int Compare([Nullable(new byte[]
			{
				0,
				1,
				1
			})] ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> x, [Nullable(new byte[]
			{
				0,
				1,
				1
			})] ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> y)
			{
				NodeVis.GroupData item = x.Item1;
				NodeVis.NodeDatum item2 = x.Item2;
				NodeVis.GroupData item3 = y.Item1;
				NodeVis.NodeDatum item4 = y.Item2;
				int num = item.NetId.CompareTo(item3.NetId);
				if (num != 0)
				{
					return num;
				}
				return item2.NetId.CompareTo(item4.NetId);
			}

			// Token: 0x040006FB RID: 1787
			[Nullable(1)]
			public static readonly NodeVisualizationOverlay.NodeDisplayComparer Instance = new NodeVisualizationOverlay.NodeDisplayComparer();
		}

		// Token: 0x0200021C RID: 540
		[Nullable(0)]
		private sealed class NodeRenderData
		{
			// Token: 0x06000E17 RID: 3607 RVA: 0x00055670 File Offset: 0x00053870
			public NodeRenderData(NodeVis.GroupData groupData, NodeVis.NodeDatum nodeDatum, Vector2 nodePos)
			{
				this.GroupData = groupData;
				this.NodeDatum = nodeDatum;
				this.NodePos = nodePos;
			}

			// Token: 0x040006FC RID: 1788
			public NodeVis.GroupData GroupData;

			// Token: 0x040006FD RID: 1789
			public NodeVis.NodeDatum NodeDatum;

			// Token: 0x040006FE RID: 1790
			public Vector2 NodePos;
		}
	}
}
