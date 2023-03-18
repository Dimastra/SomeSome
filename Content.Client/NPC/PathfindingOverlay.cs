using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Shared.NPC;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.NPC
{
	// Token: 0x02000211 RID: 529
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PathfindingOverlay : Overlay
	{
		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x000392B9 File Offset: 0x000374B9
		public override OverlaySpace Space
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x000535A8 File Offset: 0x000517A8
		public PathfindingOverlay(IEntityManager entManager, IEyeManager eyeManager, IInputManager inputManager, IMapManager mapManager, IResourceCache cache, PathfindingSystem system)
		{
			this._entManager = entManager;
			this._eyeManager = eyeManager;
			this._inputManager = inputManager;
			this._mapManager = mapManager;
			this._system = system;
			this._font = new VectorFont(cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 10);
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x000535FC File Offset: 0x000517FC
		protected override void Draw(in OverlayDrawArgs args)
		{
			DrawingHandleBase drawingHandle = args.DrawingHandle;
			DrawingHandleScreen drawingHandleScreen = drawingHandle as DrawingHandleScreen;
			if (drawingHandleScreen != null)
			{
				this.DrawScreen(args, drawingHandleScreen);
				return;
			}
			DrawingHandleWorld drawingHandleWorld = drawingHandle as DrawingHandleWorld;
			if (drawingHandleWorld == null)
			{
				return;
			}
			this.DrawWorld(args, drawingHandleWorld);
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x00053640 File Offset: 0x00051840
		private void DrawScreen(OverlayDrawArgs args, DrawingHandleScreen screenHandle)
		{
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			Box2 box;
			box..ctor(mapCoordinates.Position - 8f, mapCoordinates.Position + 8f);
			EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
			if ((this._system.Modes & PathfindingDebugMode.Crumb) != PathfindingDebugMode.None && mapCoordinates.MapId == args.MapId)
			{
				bool flag = false;
				foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapCoordinates.MapId, box, false))
				{
					Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
					TransformComponent transformComponent;
					if (!flag && this._system.Breadcrumbs.TryGetValue(mapGridComponent.Owner, out dictionary) && entityQuery.TryGetComponent(mapGridComponent.Owner, ref transformComponent))
					{
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = transformComponent.GetWorldPositionRotationMatrixWithInv();
						Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
						Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
						Box2 box2 = box.Enlarged(-8f);
						Box2 box3 = item2.TransformBox(ref box2);
						foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair in dictionary)
						{
							if (!flag)
							{
								Vector2i vector2i = keyValuePair.Key * 8;
								Box2 box4;
								box4..ctor(vector2i, vector2i + 8);
								if (box4.Intersects(ref box3))
								{
									PathfindingBreadcrumb? pathfindingBreadcrumb = null;
									float num = float.MaxValue;
									foreach (PathfindingBreadcrumb pathfindingBreadcrumb2 in keyValuePair.Value)
									{
										float length = (item.Transform(this._system.GetCoordinate(keyValuePair.Key, pathfindingBreadcrumb2.Coordinates)) - mapCoordinates.Position).Length;
										if (length < num)
										{
											num = length;
											pathfindingBreadcrumb = new PathfindingBreadcrumb?(pathfindingBreadcrumb2);
										}
									}
									if (pathfindingBreadcrumb != null)
									{
										StringBuilder stringBuilder = new StringBuilder();
										string str = "Point coordinates: ";
										PathfindingBreadcrumb value = pathfindingBreadcrumb.Value;
										string value2 = str + value.Coordinates.ToString();
										string value3 = "Grid coordinates: " + this._system.GetCoordinate(keyValuePair.Key, pathfindingBreadcrumb.Value.Coordinates).ToString();
										string str2 = "Layer: ";
										value = pathfindingBreadcrumb.Value;
										string value4 = str2 + value.Data.CollisionLayer.ToString();
										string str3 = "Mask: ";
										value = pathfindingBreadcrumb.Value;
										string value5 = str3 + value.Data.CollisionMask.ToString();
										stringBuilder.AppendLine(value2);
										stringBuilder.AppendLine(value3);
										stringBuilder.AppendLine(value4);
										stringBuilder.AppendLine(value5);
										stringBuilder.AppendLine("Flags:");
										foreach (PathfindingBreadcrumbFlag pathfindingBreadcrumbFlag in Enum.GetValues<PathfindingBreadcrumbFlag>())
										{
											if ((pathfindingBreadcrumbFlag & pathfindingBreadcrumb.Value.Data.Flags) != PathfindingBreadcrumbFlag.None)
											{
												string value6 = "- " + pathfindingBreadcrumbFlag.ToString();
												stringBuilder.AppendLine(value6);
											}
										}
										screenHandle.DrawString(this._font, mouseScreenPosition.Position, stringBuilder.ToString());
										flag = true;
										break;
									}
								}
							}
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.Poly) != PathfindingDebugMode.None && mapCoordinates.MapId == args.MapId)
			{
				MapGridComponent mapGridComponent2;
				TransformComponent transformComponent2;
				if (!this._mapManager.TryFindGridAt(mapCoordinates, ref mapGridComponent2) || !entityQuery.TryGetComponent(mapGridComponent2.Owner, ref transformComponent2))
				{
					return;
				}
				Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary2;
				if (!this._system.Polys.TryGetValue(mapGridComponent2.Owner, out dictionary2))
				{
					return;
				}
				Vector2i gridIndices = mapGridComponent2.GetTileRef(mapCoordinates).GridIndices;
				Vector2i key = gridIndices / 8;
				Dictionary<Vector2i, List<DebugPathPoly>> dictionary3;
				List<DebugPathPoly> list;
				if (!dictionary2.TryGetValue(key, out dictionary3) || !dictionary3.TryGetValue(new Vector2i(gridIndices.X % 8, gridIndices.Y % 8), out list))
				{
					return;
				}
				Matrix3 invWorldMatrix = transformComponent2.InvWorldMatrix;
				DebugPathPoly debugPathPoly = null;
				foreach (DebugPathPoly debugPathPoly2 in list)
				{
					if (debugPathPoly2.Box.Contains(invWorldMatrix.Transform(mapCoordinates.Position), true))
					{
						debugPathPoly = debugPathPoly2;
						break;
					}
				}
				if (debugPathPoly != null)
				{
					new StringBuilder();
				}
			}
		}

		// Token: 0x06000DDD RID: 3549 RVA: 0x00053B54 File Offset: 0x00051D54
		private void DrawWorld(OverlayDrawArgs args, DrawingHandleWorld worldHandle)
		{
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			Box2 box;
			box..ctor(mapCoordinates.Position - Vector2.One / 4f, mapCoordinates.Position + Vector2.One / 4f);
			EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
			if ((this._system.Modes & PathfindingDebugMode.Breadcrumbs) != PathfindingDebugMode.None && mapCoordinates.MapId == args.MapId)
			{
				foreach (MapGridComponent mapGridComponent in this._mapManager.FindGridsIntersecting(mapCoordinates.MapId, box, false))
				{
					Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary;
					TransformComponent transformComponent;
					if (this._system.Breadcrumbs.TryGetValue(mapGridComponent.Owner, out dictionary) && entityQuery.TryGetComponent(mapGridComponent.Owner, ref transformComponent))
					{
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv = transformComponent.GetWorldPositionRotationMatrixWithInv();
						Matrix3 item = worldPositionRotationMatrixWithInv.Item3;
						Matrix3 item2 = worldPositionRotationMatrixWithInv.Item4;
						worldHandle.SetTransform(ref item);
						Box2 box2 = item2.TransformBox(ref box);
						foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair in dictionary)
						{
							Vector2i vector2i = keyValuePair.Key * 8;
							Box2 box3;
							box3..ctor(vector2i, vector2i + 8);
							if (box3.Intersects(ref box2))
							{
								foreach (PathfindingBreadcrumb pathfindingBreadcrumb in keyValuePair.Value)
								{
									if (!pathfindingBreadcrumb.Equals(PathfindingBreadcrumb.Invalid))
									{
										bool flag = pathfindingBreadcrumb.Data.CollisionMask != 0 || pathfindingBreadcrumb.Data.CollisionLayer != 0;
										Color color;
										if ((pathfindingBreadcrumb.Data.Flags & PathfindingBreadcrumbFlag.Space) != PathfindingBreadcrumbFlag.None)
										{
											color = Color.Green;
										}
										else if (flag)
										{
											color = Color.Blue;
										}
										else
										{
											color = Color.Orange;
										}
										Vector2 coordinate = this._system.GetCoordinate(keyValuePair.Key, pathfindingBreadcrumb.Coordinates);
										worldHandle.DrawRect(new Box2(coordinate - 0.0625f, coordinate + 0.0625f), color.WithAlpha(0.25f), true);
									}
								}
							}
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.Polys) != PathfindingDebugMode.None && mapCoordinates.MapId == args.MapId)
			{
				foreach (MapGridComponent mapGridComponent2 in this._mapManager.FindGridsIntersecting(args.MapId, box, false))
				{
					Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary2;
					TransformComponent transformComponent2;
					if (this._system.Polys.TryGetValue(mapGridComponent2.Owner, out dictionary2) && entityQuery.TryGetComponent(mapGridComponent2.Owner, ref transformComponent2))
					{
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv2 = transformComponent2.GetWorldPositionRotationMatrixWithInv();
						Matrix3 item3 = worldPositionRotationMatrixWithInv2.Item3;
						Matrix3 item4 = worldPositionRotationMatrixWithInv2.Item4;
						worldHandle.SetTransform(ref item3);
						Box2 box4 = item4.TransformBox(ref box);
						foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> keyValuePair2 in dictionary2)
						{
							Vector2i vector2i2 = keyValuePair2.Key * 8;
							Box2 box5;
							box5..ctor(vector2i2, vector2i2 + 8);
							if (box5.Intersects(ref box4))
							{
								foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> keyValuePair3 in keyValuePair2.Value)
								{
									foreach (DebugPathPoly debugPathPoly in keyValuePair3.Value)
									{
										worldHandle.DrawRect(debugPathPoly.Box, Color.Green.WithAlpha(0.25f), true);
										worldHandle.DrawRect(debugPathPoly.Box, Color.Red, false);
									}
								}
							}
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.PolyNeighbors) != PathfindingDebugMode.None && mapCoordinates.MapId == args.MapId)
			{
				foreach (MapGridComponent mapGridComponent3 in this._mapManager.FindGridsIntersecting(args.MapId, box, false))
				{
					Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> dictionary3;
					TransformComponent transformComponent3;
					if (this._system.Polys.TryGetValue(mapGridComponent3.Owner, out dictionary3) && entityQuery.TryGetComponent(mapGridComponent3.Owner, ref transformComponent3))
					{
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv3 = transformComponent3.GetWorldPositionRotationMatrixWithInv();
						Matrix3 item5 = worldPositionRotationMatrixWithInv3.Item3;
						Matrix3 item6 = worldPositionRotationMatrixWithInv3.Item4;
						worldHandle.SetTransform(ref item5);
						Box2 box6 = item6.TransformBox(ref box);
						foreach (KeyValuePair<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>> keyValuePair4 in dictionary3)
						{
							Vector2i vector2i3 = keyValuePair4.Key * 8;
							Box2 box7;
							box7..ctor(vector2i3, vector2i3 + 8);
							if (box7.Intersects(ref box6))
							{
								foreach (KeyValuePair<Vector2i, List<DebugPathPoly>> keyValuePair5 in keyValuePair4.Value)
								{
									foreach (DebugPathPoly debugPathPoly2 in keyValuePair5.Value)
									{
										foreach (EntityCoordinates entityCoordinates in debugPathPoly2.Neighbors)
										{
											Color color2;
											Vector2 vector;
											if (entityCoordinates.EntityId != debugPathPoly2.GraphUid)
											{
												color2 = Color.Green;
												MapCoordinates mapCoordinates2 = entityCoordinates.ToMap(this._entManager);
												if (mapCoordinates2.MapId != args.MapId)
												{
													continue;
												}
												vector = item6.Transform(mapCoordinates2.Position);
											}
											else
											{
												color2 = Color.Blue;
												vector = entityCoordinates.Position;
											}
											worldHandle.DrawLine(debugPathPoly2.Box.Center, vector, color2);
										}
									}
								}
							}
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.Chunks) != PathfindingDebugMode.None)
			{
				foreach (MapGridComponent mapGridComponent4 in this._mapManager.FindGridsIntersecting(args.MapId, args.WorldBounds, false))
				{
					Dictionary<Vector2i, List<PathfindingBreadcrumb>> dictionary4;
					TransformComponent transformComponent4;
					if (this._system.Breadcrumbs.TryGetValue(mapGridComponent4.Owner, out dictionary4) && entityQuery.TryGetComponent(mapGridComponent4.Owner, ref transformComponent4))
					{
						ValueTuple<Vector2, Angle, Matrix3, Matrix3> worldPositionRotationMatrixWithInv4 = transformComponent4.GetWorldPositionRotationMatrixWithInv();
						Matrix3 item7 = worldPositionRotationMatrixWithInv4.Item3;
						Matrix3 item8 = worldPositionRotationMatrixWithInv4.Item4;
						worldHandle.SetTransform(ref item7);
						Box2 box8 = item8.TransformBox(ref args.WorldBounds);
						foreach (KeyValuePair<Vector2i, List<PathfindingBreadcrumb>> keyValuePair6 in dictionary4)
						{
							Vector2i vector2i4 = keyValuePair6.Key * 8;
							Box2 box9;
							box9..ctor(vector2i4, vector2i4 + 8);
							if (box9.Intersects(ref box8))
							{
								worldHandle.DrawRect(box9, Color.Red, false);
							}
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.Routes) != PathfindingDebugMode.None)
			{
				foreach (ValueTuple<TimeSpan, PathRouteMessage> valueTuple in this._system.Routes)
				{
					foreach (DebugPathPoly debugPathPoly3 in valueTuple.Item2.Path)
					{
						TransformComponent transformComponent5;
						if (this._entManager.TryGetComponent<TransformComponent>(debugPathPoly3.GraphUid, ref transformComponent5))
						{
							Matrix3 worldMatrix = transformComponent5.WorldMatrix;
							worldHandle.SetTransform(ref worldMatrix);
							worldHandle.DrawRect(debugPathPoly3.Box, Color.Orange.WithAlpha(0.1f), true);
						}
					}
				}
			}
			if ((this._system.Modes & PathfindingDebugMode.RouteCosts) != PathfindingDebugMode.None)
			{
				EntityUid entityUid = EntityUid.Invalid;
				foreach (ValueTuple<TimeSpan, PathRouteMessage> valueTuple2 in this._system.Routes)
				{
					float num = valueTuple2.Item2.Costs.Values.Max();
					foreach (KeyValuePair<DebugPathPoly, float> keyValuePair7 in valueTuple2.Item2.Costs)
					{
						DebugPathPoly debugPathPoly4;
						float num2;
						keyValuePair7.Deconstruct(out debugPathPoly4, out num2);
						DebugPathPoly debugPathPoly5 = debugPathPoly4;
						float num3 = num2;
						if (entityUid != debugPathPoly5.GraphUid)
						{
							TransformComponent transformComponent6;
							if (!this._entManager.TryGetComponent<TransformComponent>(debugPathPoly5.GraphUid, ref transformComponent6))
							{
								continue;
							}
							entityUid = debugPathPoly5.GraphUid;
							Matrix3 worldMatrix = transformComponent6.WorldMatrix;
							worldHandle.SetTransform(ref worldMatrix);
						}
						worldHandle.DrawRect(debugPathPoly5.Box, new Color(0f, num3 / num, 1f - num3 / num, 0.1f), true);
					}
				}
			}
			worldHandle.SetTransform(ref Matrix3.Identity);
		}

		// Token: 0x040006D2 RID: 1746
		private readonly IEntityManager _entManager;

		// Token: 0x040006D3 RID: 1747
		private readonly IEyeManager _eyeManager;

		// Token: 0x040006D4 RID: 1748
		private readonly IInputManager _inputManager;

		// Token: 0x040006D5 RID: 1749
		private readonly IMapManager _mapManager;

		// Token: 0x040006D6 RID: 1750
		private readonly PathfindingSystem _system;

		// Token: 0x040006D7 RID: 1751
		private readonly Font _font;
	}
}
