using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Destructible;
using Content.Server.NPC.Components;
using Content.Shared.Access.Components;
using Content.Shared.Administration;
using Content.Shared.Doors.Components;
using Content.Shared.NPC;
using Robust.Server.Player;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033A RID: 826
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PathfindingSystem : SharedPathfindingSystem
	{
		// Token: 0x06001132 RID: 4402 RVA: 0x0005939C File Offset: 0x0005759C
		private PathResult UpdateAStarPath(AStarPathRequest request)
		{
			if (request.Start.Equals(request.End))
			{
				return PathResult.Path;
			}
			if (request.Task.IsCanceled)
			{
				return PathResult.NoPath;
			}
			PathPoly currentNode = null;
			if (!request.Started)
			{
				request.Frontier = new PriorityQueue<ValueTuple<float, PathPoly>>(PathfindingSystem.PathPolyComparer);
				request.Started = true;
			}
			else
			{
				if (request.Frontier.Count == 0)
				{
					return PathResult.NoPath;
				}
				currentNode = request.Frontier.Peek().Item2;
				if (!currentNode.IsValid())
				{
					return PathResult.NoPath;
				}
				PathPoly parentNode;
				if (request.CameFrom.TryGetValue(currentNode, out parentNode) && !parentNode.IsValid())
				{
					return PathResult.NoPath;
				}
			}
			request.Stopwatch.Restart();
			PathPoly startNode = this.GetPoly(request.Start);
			PathPoly endNode = this.GetPoly(request.End);
			if (startNode == null || endNode == null)
			{
				return PathResult.NoPath;
			}
			currentNode = startNode;
			request.Frontier.Add(new ValueTuple<float, PathPoly>(0f, startNode));
			request.CostSoFar[startNode] = 0f;
			int count = 0;
			bool arrived = false;
			while (request.Frontier.Count > 0 && count < 512)
			{
				if (count % 20 == 0 && count > 0 && request.Stopwatch.Elapsed > PathfindingSystem.PathTime)
				{
					return PathResult.Continuing;
				}
				count++;
				currentNode = request.Frontier.Take().Item2;
				float distance;
				if ((request.Distance > 0f && currentNode.Coordinates.TryDistance(this.EntityManager, request.End, ref distance) && distance <= request.Distance) || currentNode.Equals(endNode))
				{
					arrived = true;
					break;
				}
				foreach (PathPoly neighbor in currentNode.Neighbors)
				{
					float tileCost = this.GetTileCost(request, currentNode, neighbor);
					if (!tileCost.Equals(0f))
					{
						float gScore = request.CostSoFar[currentNode] + tileCost;
						float nextValue;
						if (!request.CostSoFar.TryGetValue(neighbor, out nextValue) || gScore < nextValue)
						{
							request.CameFrom[neighbor] = currentNode;
							request.CostSoFar[neighbor] = gScore;
							float hScore = this.OctileDistance(endNode, neighbor) * 1.001f;
							float fScore = gScore + hScore;
							request.Frontier.Add(new ValueTuple<float, PathPoly>(fScore, neighbor));
						}
					}
				}
			}
			if (!arrived)
			{
				return PathResult.NoPath;
			}
			Queue<PathPoly> route = this.ReconstructPath(request.CameFrom, currentNode);
			Queue<EntityCoordinates> path = new Queue<EntityCoordinates>(route.Count);
			foreach (PathPoly node in route)
			{
				if (!node.IsValid())
				{
					return PathResult.NoPath;
				}
				path.Enqueue(node.Coordinates);
			}
			request.Polys = route;
			return PathResult.Path;
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00059684 File Offset: 0x00057884
		private PathResult UpdateBFSPath(IRobustRandom random, BFSPathRequest request)
		{
			if (request.Task.IsCanceled)
			{
				return PathResult.NoPath;
			}
			PathPoly currentNode;
			if (!request.Started)
			{
				request.Frontier = new PriorityQueue<ValueTuple<float, PathPoly>>(PathfindingSystem.PathPolyComparer);
				request.Started = true;
			}
			else
			{
				if (request.Frontier.Count == 0)
				{
					return PathResult.NoPath;
				}
				currentNode = request.Frontier.Peek().Item2;
				if (!currentNode.IsValid())
				{
					return PathResult.NoPath;
				}
				PathPoly parentNode;
				if (request.CameFrom.TryGetValue(currentNode, out parentNode) && !parentNode.IsValid())
				{
					return PathResult.NoPath;
				}
			}
			request.Stopwatch.Restart();
			PathPoly startNode = this.GetPoly(request.Start);
			if (startNode == null)
			{
				return PathResult.NoPath;
			}
			request.Frontier.Add(new ValueTuple<float, PathPoly>(0f, startNode));
			request.CostSoFar[startNode] = 0f;
			int count = 0;
			while (request.Frontier.Count > 0 && count < 512 && count < request.ExpansionLimit)
			{
				if (count % 20 == 0 && count > 0 && request.Stopwatch.Elapsed > PathfindingSystem.PathTime)
				{
					return PathResult.Continuing;
				}
				count++;
				currentNode = request.Frontier.Take().Item2;
				foreach (PathPoly neighbor in currentNode.Neighbors)
				{
					float tileCost = this.GetTileCost(request, currentNode, neighbor);
					if (!tileCost.Equals(0f))
					{
						float gScore = request.CostSoFar[currentNode] + tileCost;
						float nextValue;
						if (!request.CostSoFar.TryGetValue(neighbor, out nextValue) || gScore < nextValue)
						{
							request.CameFrom[neighbor] = currentNode;
							request.CostSoFar[neighbor] = gScore;
							request.Frontier.Add(new ValueTuple<float, PathPoly>(gScore, neighbor));
						}
					}
				}
			}
			if (request.CostSoFar.Count == 0)
			{
				return PathResult.NoPath;
			}
			PathPoly pathPoly;
			float num;
			RandomExtensions.Pick<KeyValuePair<PathPoly, float>>(random, request.CostSoFar).Deconstruct(out pathPoly, out num);
			currentNode = pathPoly;
			Queue<PathPoly> route = this.ReconstructPath(request.CameFrom, currentNode);
			Queue<EntityCoordinates> path = new Queue<EntityCoordinates>(route.Count);
			foreach (PathPoly node in route)
			{
				if (!node.IsValid())
				{
					return PathResult.NoPath;
				}
				path.Enqueue(node.Coordinates);
			}
			request.Polys = route;
			return PathResult.Path;
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0005990C File Offset: 0x00057B0C
		private Queue<PathPoly> ReconstructPath(Dictionary<PathPoly, PathPoly> path, PathPoly currentNodeRef)
		{
			List<PathPoly> running = new List<PathPoly>
			{
				currentNodeRef
			};
			while (path.ContainsKey(currentNodeRef))
			{
				PathPoly previousCurrent = currentNodeRef;
				currentNodeRef = path[currentNodeRef];
				path.Remove(previousCurrent);
				running.Add(currentNodeRef);
			}
			running = this.Simplify(running, 0f);
			running.Reverse();
			return new Queue<PathPoly>(running);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00059964 File Offset: 0x00057B64
		private float GetTileCost(PathRequest request, PathPoly start, PathPoly end)
		{
			float modifier = 1f;
			if ((end.Data.Flags & PathfindingBreadcrumbFlag.Space) != PathfindingBreadcrumbFlag.None)
			{
				return 0f;
			}
			if ((request.CollisionLayer & end.Data.CollisionMask) != 0 || (request.CollisionMask & end.Data.CollisionLayer) != 0)
			{
				bool isDoor = (end.Data.Flags & PathfindingBreadcrumbFlag.Door) > PathfindingBreadcrumbFlag.None;
				bool isAccess = (end.Data.Flags & PathfindingBreadcrumbFlag.Access) > PathfindingBreadcrumbFlag.None;
				if (isDoor && !isAccess && (request.Flags & PathFlags.Interact) != PathFlags.None)
				{
					modifier += 0.5f;
				}
				else if (isDoor && isAccess && (request.Flags & PathFlags.Prying) != PathFlags.None)
				{
					modifier += 10f;
				}
				else
				{
					if ((request.Flags & PathFlags.Smashing) == PathFlags.None || end.Data.Damage <= 0f)
					{
						return 0f;
					}
					modifier += 10f + end.Data.Damage / 100f;
				}
			}
			return modifier * this.OctileDistance(end, start);
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x00059A54 File Offset: 0x00057C54
		public List<PathPoly> Simplify(List<PathPoly> vertices, float tolerance = 0f)
		{
			if (vertices.Count <= 3)
			{
				return vertices;
			}
			List<PathPoly> simplified = new List<PathPoly>();
			for (int i = 0; i < vertices.Count; i++)
			{
				PathPoly prev = vertices[(i == 0) ? (vertices.Count - 1) : (i - 1)];
				PathPoly current = vertices[i];
				PathPoly next = vertices[(i + 1) % vertices.Count];
				PathfindingData prevData = prev.Data;
				PathfindingData currentData = current.Data;
				PathfindingData nextData = next.Data;
				if (i == 0 || i == vertices.Count - 1 || !prevData.Equals(currentData) || !currentData.Equals(nextData) || !this.IsCollinear(prev, current, next, tolerance))
				{
					simplified.Add(current);
				}
			}
			if (simplified.Count == 0)
			{
				simplified.Add(vertices[0]);
				simplified.Add(vertices[vertices.Count - 1]);
			}
			return simplified;
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00059B35 File Offset: 0x00057D35
		private bool IsCollinear(PathPoly prev, PathPoly current, PathPoly next, float tolerance)
		{
			return this.FloatInRange(this.Area(prev, current, next), -tolerance, tolerance);
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00059B4C File Offset: 0x00057D4C
		private float Area(PathPoly a, PathPoly b, PathPoly c)
		{
			float num;
			float num2;
			a.Box.Center.Deconstruct(ref num, ref num2);
			float num3 = num;
			float ay = num2;
			b.Box.Center.Deconstruct(ref num2, ref num);
			float bx = num2;
			float by = num;
			c.Box.Center.Deconstruct(ref num, ref num2);
			float cx = num;
			float cy = num2;
			return num3 * (by - cy) + bx * (cy - ay) + cx * (ay - by);
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00059BC5 File Offset: 0x00057DC5
		private bool FloatInRange(float value, float min, float max)
		{
			return value >= min && value <= max;
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00059BD4 File Offset: 0x00057DD4
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("nav");
			this._playerManager.PlayerStatusChanged += this.OnPlayerChange;
			this.InitializeGrid();
			base.SubscribeNetworkEvent<RequestPathfindingDebugMessage>(new EntitySessionEventHandler<RequestPathfindingDebugMessage>(this.OnBreadcrumbs), null, null);
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00059C28 File Offset: 0x00057E28
		public override void Shutdown()
		{
			base.Shutdown();
			this._subscribedSessions.Clear();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerChange;
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00059C54 File Offset: 0x00057E54
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateGrid();
			this._stopwatch.Restart();
			int amount = Math.Min(256, this._pathRequests.Count);
			PathResult[] results = ArrayPool<PathResult>.Shared.Rent(amount);
			Parallel.For(0, amount, delegate(int i)
			{
				if (this._stopwatch.Elapsed >= PathfindingSystem.PathTime)
				{
					results[i] = PathResult.Continuing;
					return;
				}
				PathRequest request = this._pathRequests[i];
				try
				{
					AStarPathRequest astar = request as AStarPathRequest;
					if (astar == null)
					{
						BFSPathRequest bfs = request as BFSPathRequest;
						if (bfs == null)
						{
							throw new NotImplementedException();
						}
						results[i] = this.UpdateBFSPath(this._random, bfs);
					}
					else
					{
						results[i] = this.UpdateAStarPath(astar);
					}
				}
				catch (Exception)
				{
					results[i] = PathResult.NoPath;
					throw;
				}
			});
			int offset = 0;
			for (int j = 0; j < amount; j++)
			{
				int resultIndex = j + offset;
				PathRequest path = this._pathRequests[resultIndex];
				PathResult result = results[j];
				if (path.Task.Exception != null)
				{
					throw path.Task.Exception;
				}
				if (result > PathResult.Path)
				{
					if (result != PathResult.Continuing)
					{
						throw new NotImplementedException();
					}
				}
				else
				{
					this.SendDebug(path);
					this._pathRequests.RemoveAt(resultIndex);
					offset--;
					path.Tcs.SetResult(result);
					this.SendRoute(path);
				}
			}
			ArrayPool<PathResult>.Shared.Return(results, false);
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00059D64 File Offset: 0x00057F64
		public bool TryCreatePortal(EntityCoordinates coordsA, EntityCoordinates coordsB, out int handle)
		{
			EntityUid? mapUidA = coordsA.GetMapUid(this.EntityManager);
			EntityUid? mapUid = coordsB.GetMapUid(this.EntityManager);
			handle = -1;
			if (mapUidA != mapUid || mapUidA == null)
			{
				return false;
			}
			EntityUid? gridUidA = coordsA.GetGridUid(this.EntityManager);
			EntityUid? gridUidB = coordsB.GetGridUid(this.EntityManager);
			GridPathfindingComponent gridA;
			GridPathfindingComponent gridB;
			if (!base.TryComp<GridPathfindingComponent>(gridUidA, ref gridA) || !base.TryComp<GridPathfindingComponent>(gridUidB, ref gridB))
			{
				return false;
			}
			int portalIndex = this._portalIndex;
			this._portalIndex = portalIndex + 1;
			handle = portalIndex;
			PathPortal portal = new PathPortal(handle, coordsA, coordsB);
			this._portals[handle] = portal;
			Vector2i originA = this.GetOrigin(coordsA, gridUidA.Value);
			Vector2i originB = this.GetOrigin(coordsB, gridUidB.Value);
			gridA.PortalLookup.Add(portal, originA);
			gridB.PortalLookup.Add(portal, originB);
			GridPathfindingChunk chunkA = this.GetChunk(originA, gridUidA.Value, null);
			GridPathfindingChunk chunk = this.GetChunk(originB, gridUidB.Value, null);
			chunkA.Portals.Add(portal);
			chunk.Portals.Add(portal);
			this.DirtyChunk(gridUidA.Value, coordsA);
			this.DirtyChunk(gridUidB.Value, coordsB);
			return true;
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x00059ED0 File Offset: 0x000580D0
		public bool RemovePortal(int handle)
		{
			PathPortal portal;
			if (!this._portals.TryGetValue(handle, out portal))
			{
				return false;
			}
			this._portals.Remove(handle);
			EntityUid? gridUidA = portal.CoordinatesA.GetGridUid(this.EntityManager);
			EntityUid? gridUidB = portal.CoordinatesB.GetGridUid(this.EntityManager);
			GridPathfindingComponent gridA;
			GridPathfindingComponent gridB;
			if (!base.TryComp<GridPathfindingComponent>(gridUidA, ref gridA) || !base.TryComp<GridPathfindingComponent>(gridUidB, ref gridB))
			{
				return false;
			}
			gridA.PortalLookup.Remove(portal);
			gridB.PortalLookup.Remove(portal);
			GridPathfindingChunk chunkA = this.GetChunk(this.GetOrigin(portal.CoordinatesA, gridUidA.Value), gridUidA.Value, gridA);
			GridPathfindingChunk chunk = this.GetChunk(this.GetOrigin(portal.CoordinatesB, gridUidB.Value), gridUidB.Value, gridB);
			chunkA.Portals.Remove(portal);
			chunk.Portals.Remove(portal);
			this.DirtyChunk(gridUidA.Value, portal.CoordinatesA);
			this.DirtyChunk(gridUidB.Value, portal.CoordinatesB);
			return true;
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00059FDC File Offset: 0x000581DC
		public Task<PathResultEvent> GetRandomPath(EntityUid entity, float maxRange, CancellationToken cancelToken, int limit = 40, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetRandomPath>d__31 <GetRandomPath>d__;
			<GetRandomPath>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetRandomPath>d__.<>4__this = this;
			<GetRandomPath>d__.entity = entity;
			<GetRandomPath>d__.maxRange = maxRange;
			<GetRandomPath>d__.cancelToken = cancelToken;
			<GetRandomPath>d__.limit = limit;
			<GetRandomPath>d__.flags = flags;
			<GetRandomPath>d__.<>1__state = -1;
			<GetRandomPath>d__.<>t__builder.Start<PathfindingSystem.<GetRandomPath>d__31>(ref <GetRandomPath>d__);
			return <GetRandomPath>d__.<>t__builder.Task;
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x0005A04C File Offset: 0x0005824C
		public Task<float?> GetPathDistance(EntityUid entity, EntityCoordinates end, float range, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPathDistance>d__32 <GetPathDistance>d__;
			<GetPathDistance>d__.<>t__builder = AsyncTaskMethodBuilder<float?>.Create();
			<GetPathDistance>d__.<>4__this = this;
			<GetPathDistance>d__.entity = entity;
			<GetPathDistance>d__.end = end;
			<GetPathDistance>d__.range = range;
			<GetPathDistance>d__.cancelToken = cancelToken;
			<GetPathDistance>d__.flags = flags;
			<GetPathDistance>d__.<>1__state = -1;
			<GetPathDistance>d__.<>t__builder.Start<PathfindingSystem.<GetPathDistance>d__32>(ref <GetPathDistance>d__);
			return <GetPathDistance>d__.<>t__builder.Task;
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x0005A0BC File Offset: 0x000582BC
		public Task<PathResultEvent> GetPath(EntityUid entity, EntityUid target, float range, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPath>d__33 <GetPath>d__;
			<GetPath>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetPath>d__.<>4__this = this;
			<GetPath>d__.entity = entity;
			<GetPath>d__.target = target;
			<GetPath>d__.range = range;
			<GetPath>d__.cancelToken = cancelToken;
			<GetPath>d__.flags = flags;
			<GetPath>d__.<>1__state = -1;
			<GetPath>d__.<>t__builder.Start<PathfindingSystem.<GetPath>d__33>(ref <GetPath>d__);
			return <GetPath>d__.<>t__builder.Task;
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x0005A12C File Offset: 0x0005832C
		public Task<PathResultEvent> GetPath(EntityUid entity, EntityCoordinates start, EntityCoordinates end, float range, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPath>d__34 <GetPath>d__;
			<GetPath>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetPath>d__.<>4__this = this;
			<GetPath>d__.entity = entity;
			<GetPath>d__.start = start;
			<GetPath>d__.end = end;
			<GetPath>d__.range = range;
			<GetPath>d__.cancelToken = cancelToken;
			<GetPath>d__.flags = flags;
			<GetPath>d__.<>1__state = -1;
			<GetPath>d__.<>t__builder.Start<PathfindingSystem.<GetPath>d__34>(ref <GetPath>d__);
			return <GetPath>d__.<>t__builder.Task;
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x0005A1A4 File Offset: 0x000583A4
		public Task<PathResultEvent> GetPathSafe(EntityUid entity, EntityCoordinates start, EntityCoordinates end, float range, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPathSafe>d__35 <GetPathSafe>d__;
			<GetPathSafe>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetPathSafe>d__.<>4__this = this;
			<GetPathSafe>d__.entity = entity;
			<GetPathSafe>d__.start = start;
			<GetPathSafe>d__.end = end;
			<GetPathSafe>d__.range = range;
			<GetPathSafe>d__.cancelToken = cancelToken;
			<GetPathSafe>d__.flags = flags;
			<GetPathSafe>d__.<>1__state = -1;
			<GetPathSafe>d__.<>t__builder.Start<PathfindingSystem.<GetPathSafe>d__35>(ref <GetPathSafe>d__);
			return <GetPathSafe>d__.<>t__builder.Task;
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x0005A21C File Offset: 0x0005841C
		public Task<PathResultEvent> GetPath(EntityCoordinates start, EntityCoordinates end, float range, int layer, int mask, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPath>d__36 <GetPath>d__;
			<GetPath>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetPath>d__.<>4__this = this;
			<GetPath>d__.start = start;
			<GetPath>d__.end = end;
			<GetPath>d__.range = range;
			<GetPath>d__.layer = layer;
			<GetPath>d__.mask = mask;
			<GetPath>d__.cancelToken = cancelToken;
			<GetPath>d__.flags = flags;
			<GetPath>d__.<>1__state = -1;
			<GetPath>d__.<>t__builder.Start<PathfindingSystem.<GetPath>d__36>(ref <GetPath>d__);
			return <GetPath>d__.<>t__builder.Task;
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x0005A29C File Offset: 0x0005849C
		public void GetPathEvent(EntityUid uid, EntityCoordinates start, EntityCoordinates end, float range, CancellationToken cancelToken, PathFlags flags = PathFlags.None)
		{
			PathfindingSystem.<GetPathEvent>d__37 <GetPathEvent>d__;
			<GetPathEvent>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GetPathEvent>d__.<>4__this = this;
			<GetPathEvent>d__.uid = uid;
			<GetPathEvent>d__.start = start;
			<GetPathEvent>d__.end = end;
			<GetPathEvent>d__.range = range;
			<GetPathEvent>d__.cancelToken = cancelToken;
			<GetPathEvent>d__.<>1__state = -1;
			<GetPathEvent>d__.<>t__builder.Start<PathfindingSystem.<GetPathEvent>d__37>(ref <GetPathEvent>d__);
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x0005A300 File Offset: 0x00058500
		[NullableContext(2)]
		public PathPoly GetPoly(EntityCoordinates coordinates)
		{
			EntityUid? gridUid = coordinates.GetGridUid(this.EntityManager);
			GridPathfindingComponent comp;
			TransformComponent xform;
			if (!base.TryComp<GridPathfindingComponent>(gridUid, ref comp) || !base.TryComp<TransformComponent>(gridUid, ref xform))
			{
				return null;
			}
			Vector2 localPos = xform.InvWorldMatrix.Transform(coordinates.ToMapPos(this.EntityManager));
			Vector2i origin = this.GetOrigin(localPos);
			GridPathfindingChunk chunk;
			if (!this.TryGetChunk(origin, comp, out chunk))
			{
				return null;
			}
			Vector2 chunkPos;
			chunkPos..ctor(MathHelper.Mod(localPos.X, 8f), MathHelper.Mod(localPos.Y, 8f));
			foreach (PathPoly poly in chunk.Polygons[(int)chunkPos.X * 8 + (int)chunkPos.Y])
			{
				if (poly.Box.Contains(localPos, true))
				{
					return poly;
				}
			}
			return null;
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x0005A400 File Offset: 0x00058600
		private PathRequest GetRequest(EntityUid entity, EntityCoordinates start, EntityCoordinates end, float range, CancellationToken cancelToken, PathFlags flags)
		{
			int layer = 0;
			int mask = 0;
			FixturesComponent fixtures;
			if (base.TryComp<FixturesComponent>(entity, ref fixtures))
			{
				ValueTuple<int, int> hardCollision = this._physics.GetHardCollision(entity, fixtures);
				layer = hardCollision.Item1;
				mask = hardCollision.Item2;
			}
			return new AStarPathRequest(start, end, flags, range, layer, mask, cancelToken);
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x0005A448 File Offset: 0x00058648
		public PathFlags GetFlags(EntityUid uid)
		{
			NPCComponent npc;
			if (!base.TryComp<NPCComponent>(uid, ref npc))
			{
				return PathFlags.None;
			}
			return this.GetFlags(npc.Blackboard);
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0005A470 File Offset: 0x00058670
		public PathFlags GetFlags(NPCBlackboard blackboard)
		{
			PathFlags flags = PathFlags.None;
			bool pry;
			if (blackboard.TryGetValue<bool>("NavPry", out pry, this.EntityManager) && pry)
			{
				flags |= PathFlags.Prying;
			}
			bool smash;
			if (blackboard.TryGetValue<bool>("NavSmash", out smash, this.EntityManager) && smash)
			{
				flags |= PathFlags.Smashing;
			}
			bool interact;
			if (blackboard.TryGetValue<bool>("NavInteract", out interact, this.EntityManager) && interact)
			{
				flags |= PathFlags.Interact;
			}
			return flags;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0005A4D4 File Offset: 0x000586D4
		private Task<PathResultEvent> GetPath(PathRequest request, bool safe = false)
		{
			PathfindingSystem.<GetPath>d__42 <GetPath>d__;
			<GetPath>d__.<>t__builder = AsyncTaskMethodBuilder<PathResultEvent>.Create();
			<GetPath>d__.<>4__this = this;
			<GetPath>d__.request = request;
			<GetPath>d__.safe = safe;
			<GetPath>d__.<>1__state = -1;
			<GetPath>d__.<>t__builder.Start<PathfindingSystem.<GetPath>d__42>(ref <GetPath>d__);
			return <GetPath>d__.<>t__builder.Task;
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0005A528 File Offset: 0x00058728
		private DebugPathPoly GetDebugPoly(PathPoly poly)
		{
			List<EntityCoordinates> neighbors = new List<EntityCoordinates>(poly.Neighbors.Count);
			foreach (PathPoly neighbor in poly.Neighbors)
			{
				neighbors.Add(neighbor.Coordinates);
			}
			return new DebugPathPoly
			{
				GraphUid = poly.GraphUid,
				ChunkOrigin = poly.ChunkOrigin,
				TileIndex = poly.TileIndex,
				Box = poly.Box,
				Data = poly.Data,
				Neighbors = neighbors
			};
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0005A5DC File Offset: 0x000587DC
		private void SendDebug(PathRequest request)
		{
			if (this._subscribedSessions.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<ICommonSession, PathfindingDebugMode> session in this._subscribedSessions)
			{
				if ((session.Value & PathfindingDebugMode.Routes) != PathfindingDebugMode.None)
				{
					base.RaiseNetworkEvent(new PathRouteMessage(request.Polys.Select(new Func<PathPoly, DebugPathPoly>(this.GetDebugPoly)).ToList<DebugPathPoly>(), new Dictionary<DebugPathPoly, float>()), session.Key.ConnectedClient);
				}
			}
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0005A67C File Offset: 0x0005887C
		private void OnBreadcrumbs(RequestPathfindingDebugMessage msg, EntitySessionEventArgs args)
		{
			IPlayerSession pSession = (IPlayerSession)args.SenderSession;
			if (!this._adminManager.HasAdminFlag(pSession, AdminFlags.Debug))
			{
				return;
			}
			PathfindingDebugMode sessions = Extensions.GetOrNew<ICommonSession, PathfindingDebugMode>(this._subscribedSessions, args.SenderSession);
			if (msg.Mode == PathfindingDebugMode.None)
			{
				this._subscribedSessions.Remove(args.SenderSession);
				return;
			}
			sessions = msg.Mode;
			this._subscribedSessions[args.SenderSession] = sessions;
			if (this.IsCrumb(sessions))
			{
				this.SendBreadcrumbs(pSession);
			}
			if (this.IsPoly(sessions))
			{
				this.SendPolys(pSession);
			}
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0005A70F File Offset: 0x0005890F
		private bool IsCrumb(PathfindingDebugMode mode)
		{
			return (mode & (PathfindingDebugMode.Breadcrumbs | PathfindingDebugMode.Crumb)) > PathfindingDebugMode.None;
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0005A717 File Offset: 0x00058917
		private bool IsPoly(PathfindingDebugMode mode)
		{
			return (mode & (PathfindingDebugMode.Chunks | PathfindingDebugMode.Polys | PathfindingDebugMode.PolyNeighbors | PathfindingDebugMode.Poly)) > PathfindingDebugMode.None;
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0005A720 File Offset: 0x00058920
		private bool IsRoute(PathfindingDebugMode mode)
		{
			return (mode & (PathfindingDebugMode.Routes | PathfindingDebugMode.RouteCosts)) > PathfindingDebugMode.None;
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x0005A72C File Offset: 0x0005892C
		private void SendBreadcrumbs(ICommonSession pSession)
		{
			PathBreadcrumbsMessage msg = new PathBreadcrumbsMessage();
			foreach (GridPathfindingComponent comp in base.EntityQuery<GridPathfindingComponent>(true))
			{
				msg.Breadcrumbs.Add(comp.Owner, new Dictionary<Vector2i, List<PathfindingBreadcrumb>>(comp.Chunks.Count));
				foreach (KeyValuePair<Vector2i, GridPathfindingChunk> chunk in comp.Chunks)
				{
					List<PathfindingBreadcrumb> data = this.GetCrumbs(chunk.Value);
					msg.Breadcrumbs[comp.Owner].Add(chunk.Key, data);
				}
			}
			base.RaiseNetworkEvent(msg, pSession.ConnectedClient);
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0005A818 File Offset: 0x00058A18
		private void SendRoute(PathRequest request)
		{
			if (this._subscribedSessions.Count == 0)
			{
				return;
			}
			List<DebugPathPoly> polys = new List<DebugPathPoly>();
			Dictionary<DebugPathPoly, float> costs = new Dictionary<DebugPathPoly, float>();
			foreach (PathPoly poly in request.Polys)
			{
				polys.Add(this.GetDebugPoly(poly));
			}
			foreach (KeyValuePair<PathPoly, float> keyValuePair in request.CostSoFar)
			{
				PathPoly pathPoly;
				float num;
				keyValuePair.Deconstruct(out pathPoly, out num);
				PathPoly poly2 = pathPoly;
				float value = num;
				costs.Add(this.GetDebugPoly(poly2), value);
			}
			PathRouteMessage msg = new PathRouteMessage(polys, costs);
			foreach (KeyValuePair<ICommonSession, PathfindingDebugMode> session in this._subscribedSessions)
			{
				if (this.IsRoute(session.Value))
				{
					base.RaiseNetworkEvent(msg, session.Key.ConnectedClient);
				}
			}
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0005A954 File Offset: 0x00058B54
		private void SendPolys(ICommonSession pSession)
		{
			PathPolysMessage msg = new PathPolysMessage();
			foreach (GridPathfindingComponent comp in base.EntityQuery<GridPathfindingComponent>(true))
			{
				msg.Polys.Add(comp.Owner, new Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>(comp.Chunks.Count));
				foreach (KeyValuePair<Vector2i, GridPathfindingChunk> chunk in comp.Chunks)
				{
					Dictionary<Vector2i, List<DebugPathPoly>> data = this.GetPolys(chunk.Value);
					msg.Polys[comp.Owner].Add(chunk.Key, data);
				}
			}
			base.RaiseNetworkEvent(msg, pSession.ConnectedClient);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0005AA40 File Offset: 0x00058C40
		private void SendBreadcrumbs(GridPathfindingChunk chunk, EntityUid gridUid)
		{
			if (this._subscribedSessions.Count == 0)
			{
				return;
			}
			PathBreadcrumbsRefreshMessage msg = new PathBreadcrumbsRefreshMessage
			{
				Origin = chunk.Origin,
				GridUid = gridUid,
				Data = this.GetCrumbs(chunk)
			};
			foreach (KeyValuePair<ICommonSession, PathfindingDebugMode> session in this._subscribedSessions)
			{
				if (this.IsCrumb(session.Value))
				{
					base.RaiseNetworkEvent(msg, session.Key.ConnectedClient);
				}
			}
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x0005AAE4 File Offset: 0x00058CE4
		private void SendPolys(GridPathfindingChunk chunk, EntityUid gridUid, List<PathPoly>[] tilePolys)
		{
			if (this._subscribedSessions.Count == 0)
			{
				return;
			}
			Dictionary<Vector2i, List<DebugPathPoly>> data = new Dictionary<Vector2i, List<DebugPathPoly>>(tilePolys.Length);
			double extent = Math.Sqrt((double)tilePolys.Length);
			int x = 0;
			while ((double)x < extent)
			{
				int y = 0;
				while ((double)y < extent)
				{
					byte index = this.GetIndex(x, y);
					data[new Vector2i(x, y)] = tilePolys[(int)index].Select(new Func<PathPoly, DebugPathPoly>(this.GetDebugPoly)).ToList<DebugPathPoly>();
					y++;
				}
				x++;
			}
			PathPolysRefreshMessage msg = new PathPolysRefreshMessage
			{
				Origin = chunk.Origin,
				GridUid = gridUid,
				Polys = data
			};
			foreach (KeyValuePair<ICommonSession, PathfindingDebugMode> session in this._subscribedSessions)
			{
				if (this.IsPoly(session.Value))
				{
					base.RaiseNetworkEvent(msg, session.Key.ConnectedClient);
				}
			}
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x0005ABE8 File Offset: 0x00058DE8
		private List<PathfindingBreadcrumb> GetCrumbs(GridPathfindingChunk chunk)
		{
			List<PathfindingBreadcrumb> crumbs = new List<PathfindingBreadcrumb>(chunk.Points.Length);
			for (int x = 0; x < 32; x++)
			{
				for (int y = 0; y < 32; y++)
				{
					crumbs.Add(chunk.Points[x, y]);
				}
			}
			return crumbs;
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0005AC34 File Offset: 0x00058E34
		private Dictionary<Vector2i, List<DebugPathPoly>> GetPolys(GridPathfindingChunk chunk)
		{
			Dictionary<Vector2i, List<DebugPathPoly>> polys = new Dictionary<Vector2i, List<DebugPathPoly>>(chunk.Polygons.Length);
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					byte index = this.GetIndex(x, y);
					polys[new Vector2i(x, y)] = chunk.Polygons[(int)index].Select(new Func<PathPoly, DebugPathPoly>(this.GetDebugPoly)).ToList<DebugPathPoly>();
				}
			}
			return polys;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0005AC9C File Offset: 0x00058E9C
		private void OnPlayerChange([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 2 || !this._subscribedSessions.ContainsKey(e.Session))
			{
				return;
			}
			this._subscribedSessions.Remove(e.Session);
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0005ACD0 File Offset: 0x00058ED0
		public float EuclideanDistance(PathPoly start, PathPoly end)
		{
			float num;
			float num2;
			this.GetDiff(start, end).Deconstruct(ref num, ref num2);
			float num3 = num;
			float dy = num2;
			return MathF.Sqrt(num3 * num3 + dy * dy);
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x0005AD00 File Offset: 0x00058F00
		public float ManhattanDistance(PathPoly start, PathPoly end)
		{
			float num;
			float num2;
			this.GetDiff(start, end).Deconstruct(ref num, ref num2);
			float num3 = num;
			float dy = num2;
			return num3 + dy;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0005AD28 File Offset: 0x00058F28
		public float OctileDistance(PathPoly start, PathPoly end)
		{
			float num;
			float num2;
			this.GetDiff(start, end).Deconstruct(ref num, ref num2);
			float dx = num;
			float dy = num2;
			return dx + dy + -0.59000003f * Math.Min(dx, dy);
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0005AD60 File Offset: 0x00058F60
		private Vector2 GetDiff(PathPoly start, PathPoly end)
		{
			Vector2 startPos = start.Box.Center;
			Vector2 endPos = end.Box.Center;
			if (end.GraphUid != start.GraphUid)
			{
				TransformComponent startXform;
				TransformComponent endXform;
				if (!base.TryComp<TransformComponent>(start.GraphUid, ref startXform) || !base.TryComp<TransformComponent>(end.GraphUid, ref endXform))
				{
					return Vector2.Zero;
				}
				endPos = startXform.InvWorldMatrix.Transform(endXform.WorldMatrix.Transform(endPos));
			}
			Vector2 diff = startPos - endPos;
			return Vector2.Abs(ref diff);
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0005ADF0 File Offset: 0x00058FF0
		private void InitializeGrid()
		{
			base.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInit), null, null);
			base.SubscribeLocalEvent<GridRemovalEvent>(new EntityEventHandler<GridRemovalEvent>(this.OnGridRemoved), null, null);
			base.SubscribeLocalEvent<GridPathfindingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GridPathfindingComponent, EntityUnpausedEvent>(this.OnGridPathPause), null, null);
			base.SubscribeLocalEvent<GridPathfindingComponent, ComponentShutdown>(new ComponentEventHandler<GridPathfindingComponent, ComponentShutdown>(this.OnGridPathShutdown), null, null);
			base.SubscribeLocalEvent<CollisionChangeEvent>(new EntityEventRefHandler<CollisionChangeEvent>(this.OnCollisionChange), null, null);
			base.SubscribeLocalEvent<PhysicsBodyTypeChangedEvent>(new EntityEventRefHandler<PhysicsBodyTypeChangedEvent>(this.OnBodyTypeChange), null, null);
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChange), null, null);
			base.SubscribeLocalEvent<MoveEvent>(new EntityEventRefHandler<MoveEvent>(this.OnMoveEvent), null, null);
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0005AEA0 File Offset: 0x000590A0
		private void OnTileChange(ref TileChangedEvent ev)
		{
			if (ev.OldTile.IsEmpty == ev.NewTile.Tile.IsEmpty)
			{
				return;
			}
			this.DirtyChunk(ev.Entity, base.Comp<MapGridComponent>(ev.Entity).GridTileToLocal(ev.NewTile.GridIndices));
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0005AEF3 File Offset: 0x000590F3
		private void OnGridPathPause(EntityUid uid, GridPathfindingComponent component, ref EntityUnpausedEvent args)
		{
			component.NextUpdate += args.PausedTime;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0005AF0C File Offset: 0x0005910C
		private void OnGridPathShutdown(EntityUid uid, GridPathfindingComponent component, ComponentShutdown args)
		{
			foreach (KeyValuePair<Vector2i, GridPathfindingChunk> chunk in component.Chunks)
			{
				foreach (List<PathPoly> poly in chunk.Value.Polygons)
				{
					this.ClearTilePolys(poly);
				}
			}
			component.DirtyChunks.Clear();
			component.Chunks.Clear();
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0005AF98 File Offset: 0x00059198
		private void UpdateGrid()
		{
			if (this.PauseUpdating)
			{
				return;
			}
			TimeSpan curTime = this._timing.CurTime;
			this._stopwatch.Restart();
			ParallelOptions options = new ParallelOptions
			{
				MaxDegreeOfParallelism = this._parallel.ParallelProcessCount
			};
			AllEntityQueryEnumerator<GridPathfindingComponent> query = base.AllEntityQuery<GridPathfindingComponent>();
			for (;;)
			{
				GridPathfindingComponent comp;
				if (!query.MoveNext(ref comp))
				{
					break;
				}
				MapGridComponent mapGridComp;
				if (comp.DirtyChunks.Count != 0 && !(comp.NextUpdate < curTime) && base.TryComp<MapGridComponent>(comp.Owner, ref mapGridComp))
				{
					List<PathPortal> dirtyPortals = comp.DirtyPortals;
					dirtyPortals.Clear();
					GridPathfindingChunk[] dirt = new GridPathfindingChunk[comp.DirtyChunks.Count];
					int idx = 0;
					foreach (Vector2i origin in comp.DirtyChunks)
					{
						GridPathfindingChunk chunk = this.GetChunk(origin, comp.Owner, comp);
						dirt[idx] = chunk;
						idx++;
					}
					GridPathfindingChunk[] dirt2 = dirt;
					int i;
					for (i = 0; i < dirt2.Length; i++)
					{
						GridPathfindingChunk chunk2 = dirt2[i];
						foreach (KeyValuePair<PathPortal, PathPoly> keyValuePair in chunk2.PortalPolys)
						{
							PathPortal pathPortal;
							PathPoly pathPoly;
							keyValuePair.Deconstruct(out pathPortal, out pathPoly);
							PathPoly poly = pathPoly;
							this.ClearPoly(poly);
						}
						chunk2.PortalPolys.Clear();
						foreach (PathPortal portal in chunk2.Portals)
						{
							dirtyPortals.Add(portal);
						}
					}
					Parallel.For(0, dirt.Length, options, delegate(int i)
					{
						EntityQuery<AccessReaderComponent> accessQuery = this.GetEntityQuery<AccessReaderComponent>();
						EntityQuery<DestructibleComponent> destructibleQuery = this.GetEntityQuery<DestructibleComponent>();
						EntityQuery<DoorComponent> doorQuery = this.GetEntityQuery<DoorComponent>();
						EntityQuery<FixturesComponent> fixturesQuery = this.GetEntityQuery<FixturesComponent>();
						EntityQuery<PhysicsComponent> physicsQuery = this.GetEntityQuery<PhysicsComponent>();
						EntityQuery<TransformComponent> xformQuery = this.GetEntityQuery<TransformComponent>();
						this.BuildBreadcrumbs(dirt[i], mapGridComp, accessQuery, destructibleQuery, doorQuery, fixturesQuery, physicsQuery, xformQuery);
					});
					for (int it = 0; it < 4; it++)
					{
						int it1 = it;
						Parallel.For(0, dirt.Length, options, delegate(int j)
						{
							GridPathfindingChunk chunk4 = dirt[j];
							int num = Math.Abs(chunk4.Origin.X % 2);
							int y = Math.Abs(chunk4.Origin.Y % 2);
							if (num * 2 + y != it1)
							{
								return;
							}
							this.ClearOldPolys(chunk4);
						});
					}
					for (int it2 = 0; it2 < 4; it2++)
					{
						int it1 = it2;
						Parallel.For(0, dirt.Length, options, delegate(int j)
						{
							GridPathfindingChunk chunk4 = dirt[j];
							int num = Math.Abs(chunk4.Origin.X % 2);
							int y = Math.Abs(chunk4.Origin.Y % 2);
							if (num * 2 + y != it1)
							{
								return;
							}
							this.BuildNavmesh(chunk4, comp);
						});
					}
					foreach (PathPortal portal2 in dirtyPortals)
					{
						PathPoly polyA = this.GetPoly(portal2.CoordinatesA);
						PathPoly polyB = this.GetPoly(portal2.CoordinatesB);
						if (polyA != null && polyB != null)
						{
							GridPathfindingChunk chunkA = this.GetChunk(polyA.ChunkOrigin, polyA.GraphUid, null);
							GridPathfindingChunk chunk3 = this.GetChunk(polyB.ChunkOrigin, polyB.GraphUid, null);
							chunkA.PortalPolys.TryAdd(portal2, polyA);
							chunk3.PortalPolys.TryAdd(portal2, polyB);
							this.AddNeighbors(polyA, polyB);
						}
					}
					comp.DirtyChunks.Clear();
				}
			}
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0005B338 File Offset: 0x00059538
		private bool IsBodyRelevant(PhysicsComponent body)
		{
			return body.Hard && body.BodyType == 4 && ((body.CollisionMask & 65) != 0 || (body.CollisionLayer & 30) != 0);
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0005B368 File Offset: 0x00059568
		private void OnCollisionChange(ref CollisionChangeEvent ev)
		{
			if (!this.IsBodyRelevant(ev.Body))
			{
				return;
			}
			TransformComponent xform = base.Transform(ev.Body.Owner);
			if (xform.GridUid == null)
			{
				return;
			}
			this.DirtyChunk(xform.GridUid.Value, xform.Coordinates);
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0005B3C4 File Offset: 0x000595C4
		private void OnBodyTypeChange(ref PhysicsBodyTypeChangedEvent ev)
		{
			TransformComponent xform;
			if (ev.Component.CanCollide && this.IsBodyRelevant(ev.Component) && base.TryComp<TransformComponent>(ev.Entity, ref xform) && xform.GridUid != null)
			{
				this.DirtyChunk(xform.GridUid.Value, xform.Coordinates);
			}
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0005B428 File Offset: 0x00059628
		private void OnMoveEvent(ref MoveEvent ev)
		{
			PhysicsComponent body;
			if (!base.TryComp<PhysicsComponent>(ev.Sender, ref body) || body.BodyType != 4 || base.HasComp<MapGridComponent>(ev.Sender) || ev.OldPosition.Equals(ev.NewPosition))
			{
				return;
			}
			EntityUid? gridUid = ev.Component.GridUid;
			EntityUid? oldGridUid = (ev.OldPosition.EntityId == ev.NewPosition.EntityId) ? gridUid : ev.OldPosition.GetGridUid(this.EntityManager);
			if (oldGridUid == gridUid && oldGridUid == null)
			{
				return;
			}
			if (oldGridUid != null && gridUid != null)
			{
				Vector2i origin2 = this.GetOrigin(ev.OldPosition, oldGridUid.Value);
				Vector2i origin = this.GetOrigin(ev.NewPosition, gridUid.Value);
				if (origin2 == origin)
				{
					this.DirtyChunk(oldGridUid.Value, ev.NewPosition);
					return;
				}
			}
			if (oldGridUid != null)
			{
				this.DirtyChunk(oldGridUid.Value, ev.OldPosition);
			}
			if (gridUid != null)
			{
				this.DirtyChunk(gridUid.Value, ev.NewPosition);
			}
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0005B580 File Offset: 0x00059780
		private void OnGridInit(GridInitializeEvent ev)
		{
			base.EnsureComp<GridPathfindingComponent>(ev.EntityUid);
			MapGridComponent mapGrid = base.Comp<MapGridComponent>(ev.EntityUid);
			for (double x = Math.Floor((double)mapGrid.LocalAABB.Left); x <= Math.Ceiling((double)(mapGrid.LocalAABB.Right + 8f)); x += 8.0)
			{
				for (double y = Math.Floor((double)mapGrid.LocalAABB.Bottom); y <= Math.Ceiling((double)(mapGrid.LocalAABB.Top + 8f)); y += 8.0)
				{
					this.DirtyChunk(ev.EntityUid, mapGrid.GridTileToLocal(new Vector2i((int)x, (int)y)));
				}
			}
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0005B636 File Offset: 0x00059836
		private void OnGridRemoved(GridRemovalEvent ev)
		{
			base.RemComp<GridPathfindingComponent>(ev.EntityUid);
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0005B648 File Offset: 0x00059848
		private void DirtyChunk(EntityUid gridUid, EntityCoordinates coordinates)
		{
			GridPathfindingComponent comp;
			if (!base.TryComp<GridPathfindingComponent>(gridUid, ref comp))
			{
				return;
			}
			TimeSpan currentTime = this._timing.CurTime;
			if (comp.NextUpdate < currentTime)
			{
				comp.NextUpdate = currentTime + PathfindingSystem.UpdateCooldown;
			}
			comp.DirtyChunks.Add(this.GetOrigin(coordinates, gridUid));
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x0005B6A0 File Offset: 0x000598A0
		private GridPathfindingChunk GetChunk(Vector2i origin, EntityUid uid, [Nullable(2)] GridPathfindingComponent component = null)
		{
			if (!base.Resolve<GridPathfindingComponent>(uid, ref component, true))
			{
				throw new InvalidOperationException();
			}
			GridPathfindingChunk chunk;
			if (component.Chunks.TryGetValue(origin, out chunk))
			{
				return chunk;
			}
			chunk = new GridPathfindingChunk
			{
				Origin = origin
			};
			component.Chunks[origin] = chunk;
			return chunk;
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0005B6EC File Offset: 0x000598EC
		private bool TryGetChunk(Vector2i origin, GridPathfindingComponent component, [Nullable(2)] [NotNullWhen(true)] out GridPathfindingChunk chunk)
		{
			return component.Chunks.TryGetValue(origin, out chunk);
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0005B6FB File Offset: 0x000598FB
		private byte GetIndex(int x, int y)
		{
			return (byte)(x * 8 + y);
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0005B703 File Offset: 0x00059903
		private Vector2i GetOrigin(Vector2 localPos)
		{
			return new Vector2i((int)Math.Floor((double)(localPos.X / 8f)), (int)Math.Floor((double)(localPos.Y / 8f)));
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x0005B730 File Offset: 0x00059930
		private Vector2i GetOrigin(EntityCoordinates coordinates, EntityUid gridUid)
		{
			Vector2 localPos = base.Transform(gridUid).InvWorldMatrix.Transform(coordinates.ToMapPos(this.EntityManager));
			return new Vector2i((int)Math.Floor((double)(localPos.X / 8f)), (int)Math.Floor((double)(localPos.Y / 8f)));
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0005B78C File Offset: 0x0005998C
		private unsafe void BuildBreadcrumbs(GridPathfindingChunk chunk, MapGridComponent grid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<AccessReaderComponent> accessQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DestructibleComponent> destructibleQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<DoorComponent> doorQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<FixturesComponent> fixturesQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> physicsQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			new Stopwatch().Start();
			PathfindingBreadcrumb[,] points = chunk.Points;
			Vector2i gridOrigin = chunk.Origin * 8;
			ValueList<EntityUid> tileEntities = default(ValueList<EntityUid>);
			List<PathPoly>[] chunkPolys = chunk.BufferPolygons;
			for (int i = 0; i < chunkPolys.Length; i++)
			{
				chunkPolys[i].Clear();
			}
			ValueList<Box2i> tilePolys = new ValueList<Box2i>(4);
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					Vector2i tilePos = new Vector2i(x, y) + gridOrigin;
					tilePolys.Clear();
					PathfindingBreadcrumbFlag flags = grid.GetTileRef(tilePos).Tile.IsEmpty ? PathfindingBreadcrumbFlag.Space : PathfindingBreadcrumbFlag.None;
					tileEntities.Clear();
					EntityUid? ent;
					while (grid.GetAnchoredEntitiesEnumerator(tilePos).MoveNext(ref ent))
					{
						PhysicsComponent body;
						if (physicsQuery.TryGetComponent(ent, ref body) && this.IsBodyRelevant(body))
						{
							tileEntities.Add(ent.Value);
						}
					}
					for (int subX = 0; subX < 4; subX++)
					{
						for (int subY = 0; subY < 4; subY++)
						{
							int xOffset = x * 4 + subX;
							int yOffset = y * 4 + subY;
							Vector2 localPos;
							localPos..ctor(0.125f + (float)gridOrigin.X + (float)x + (float)subX / 4f, 0.125f + (float)gridOrigin.Y + (float)y + (float)subY / 4f);
							int collisionMask = 0;
							int collisionLayer = 0;
							float damage = 0f;
							foreach (EntityUid ent2 in tileEntities)
							{
								FixturesComponent fixtures;
								if (fixturesQuery.TryGetComponent(ent2, ref fixtures))
								{
									foreach (Fixture fixture in fixtures.Fixtures.Values)
									{
										if (fixture.Hard && ((collisionMask & fixture.CollisionMask) != fixture.CollisionMask || (collisionLayer & fixture.CollisionLayer) != fixture.CollisionLayer))
										{
											bool intersects = false;
											FixtureProxy[] proxies = fixture.Proxies;
											for (int m = 0; m < proxies.Length; m++)
											{
												if (proxies[m].AABB.Contains(localPos, true))
												{
													intersects = true;
												}
											}
											TransformComponent xform;
											if (intersects && xformQuery.TryGetComponent(ent2, ref xform) && this._fixtures.TestPoint(fixture.Shape, new Transform(xform.LocalPosition, xform.LocalRotation), localPos))
											{
												collisionLayer |= fixture.CollisionLayer;
												collisionMask |= fixture.CollisionMask;
											}
										}
									}
									if (accessQuery.HasComponent(ent2))
									{
										flags |= PathfindingBreadcrumbFlag.Access;
									}
									if (doorQuery.HasComponent(ent2))
									{
										flags |= PathfindingBreadcrumbFlag.Door;
									}
									DestructibleComponent damageable;
									if (destructibleQuery.TryGetComponent(ent2, ref damageable))
									{
										damage += this._destructible.DestroyedAt(ent2, damageable).Float();
									}
								}
							}
							PathfindingBreadcrumbFlag pathfindingBreadcrumbFlag = flags & PathfindingBreadcrumbFlag.Space;
							PathfindingBreadcrumb crumb = new PathfindingBreadcrumb
							{
								Coordinates = new Vector2i(xOffset, yOffset),
								Data = new PathfindingData(flags, collisionLayer, collisionMask, damage)
							};
							points[xOffset, yOffset] = crumb;
						}
					}
					PathfindingData data = points[x * 4, y * 4].Data;
					Vector2i start = Vector2i.Zero;
					for (int j = 0; j < 16; j++)
					{
						int ix = j / 4;
						int iy = j % 4;
						int nextX = (j + 1) / 4;
						int nextY = (j + 1) % 4;
						if (iy == 3 || !points[x * 4 + nextX, y * 4 + nextY].Data.Equals(data))
						{
							tilePolys.Add(new Box2i(start, new Vector2i(ix, iy)));
							if (j < 15)
							{
								start..ctor(nextX, nextY);
								data = points[x * 4 + nextX, y * 4 + nextY].Data;
							}
						}
					}
					bool anyCombined = true;
					while (anyCombined)
					{
						anyCombined = false;
						for (int k = 0; k < tilePolys.Count; k++)
						{
							Box2i poly = *tilePolys[k];
							data = points[x * 4 + poly.Left, y * 4 + poly.Bottom].Data;
							for (int l = k + 1; l < tilePolys.Count; l++)
							{
								Box2i nextPoly = *tilePolys[l];
								PathfindingData nextData = points[x * 4 + nextPoly.Left, y * 4 + nextPoly.Bottom].Data;
								if (poly.Bottom == nextPoly.Bottom && poly.Top == nextPoly.Top && poly.Right + 1 == nextPoly.Left && data.Equals(nextData))
								{
									tilePolys.RemoveAt(l);
									l--;
									poly..ctor(poly.Left, poly.Bottom, poly.Right + 1, poly.Top);
									anyCombined = true;
								}
							}
							*tilePolys[k] = poly;
						}
					}
					List<PathPoly> tilePoly = chunkPolys[x * 8 + y];
					Vector2 polyOffset = gridOrigin + new Vector2((float)x, (float)y);
					foreach (Box2i poly2 in tilePolys)
					{
						Box2 box;
						box..ctor(poly2.BottomLeft / 4f + polyOffset, (poly2.TopRight + Vector2i.One) / 4f + polyOffset);
						PathfindingData polyData = points[x * 4 + poly2.Left, y * 4 + poly2.Bottom].Data;
						HashSet<PathPoly> neighbors = new HashSet<PathPoly>();
						tilePoly.Add(new PathPoly(grid.Owner, chunk.Origin, this.GetIndex(x, y), box, polyData, neighbors));
					}
				}
			}
			this.SendBreadcrumbs(chunk, grid.Owner);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0005BE24 File Offset: 0x0005A024
		private void ClearTilePolys(List<PathPoly> polys)
		{
			foreach (PathPoly poly in polys)
			{
				this.ClearPoly(poly);
			}
			polys.Clear();
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0005BE78 File Offset: 0x0005A078
		private void ClearPoly(PathPoly poly)
		{
			foreach (PathPoly pathPoly in poly.Neighbors)
			{
				pathPoly.Neighbors.Remove(poly);
			}
			poly.Data.Flags = PathfindingBreadcrumbFlag.Invalid;
			poly.Neighbors.Clear();
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x0005BEE8 File Offset: 0x0005A0E8
		private void ClearOldPolys(GridPathfindingChunk chunk)
		{
			List<PathPoly>[] chunkPolys = chunk.Polygons;
			List<PathPoly>[] bufferPolygons = chunk.BufferPolygons;
			for (int x = 0; x < 8; x++)
			{
				int y = 0;
				while (y < 8)
				{
					int index = x * 8 + y;
					List<PathPoly> polys = bufferPolygons[index];
					List<PathPoly> existing = chunkPolys[index];
					bool isEquivalent = true;
					if (polys.Count != existing.Count)
					{
						goto IL_9A;
					}
					for (int i = 0; i < existing.Count; i++)
					{
						PathPoly ePoly = existing[i];
						PathPoly poly = polys[i];
						if (!ePoly.IsEquivalent(poly))
						{
							isEquivalent = false;
							break;
						}
						ePoly.Data.Damage = poly.Data.Damage;
					}
					if (!isEquivalent)
					{
						goto IL_9A;
					}
					IL_AB:
					y++;
					continue;
					IL_9A:
					this.ClearTilePolys(existing);
					existing.AddRange(polys);
					goto IL_AB;
				}
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0005BFB8 File Offset: 0x0005A1B8
		private void BuildNavmesh(GridPathfindingChunk chunk, GridPathfindingComponent component)
		{
			new Stopwatch().Start();
			List<PathPoly>[] chunkPolys = chunk.Polygons;
			GridPathfindingChunk leftChunk;
			component.Chunks.TryGetValue(chunk.Origin + new Vector2i(-1, 0), out leftChunk);
			GridPathfindingChunk bottomChunk;
			component.Chunks.TryGetValue(chunk.Origin + new Vector2i(0, -1), out bottomChunk);
			GridPathfindingChunk rightChunk;
			component.Chunks.TryGetValue(chunk.Origin + new Vector2i(1, 0), out rightChunk);
			GridPathfindingChunk topChunk;
			component.Chunks.TryGetValue(chunk.Origin + new Vector2i(0, 1), out topChunk);
			for (int x = 0; x < 8; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					byte index = this.GetIndex(x, y);
					List<PathPoly> tile = chunkPolys[(int)index];
					byte i = 0;
					while ((int)i < tile.Count)
					{
						PathPoly poly = tile[(int)i];
						Box2 enlarged = poly.Box.Enlarged(0.125f);
						byte j = i + 1;
						while ((int)j < tile.Count)
						{
							PathPoly neighbor = tile[(int)j];
							Box2 enlargedNeighbor = neighbor.Box.Enlarged(0.125f);
							Box2 box = enlarged.Intersect(ref enlargedNeighbor);
							if (Box2.Area(ref box) > 0.125f)
							{
								this.AddNeighbors(poly, neighbor);
							}
							j += 1;
						}
						for (int ix = -1; ix <= 1; ix++)
						{
							for (int iy = -1; iy <= 1; iy++)
							{
								if (ix == 0 || iy == 0)
								{
									int neighborX = x + ix;
									int neighborY = y + iy;
									byte neighborIndex = this.GetIndex(neighborX, neighborY);
									List<PathPoly> neighborTile;
									if (neighborX < 0)
									{
										if (leftChunk == null)
										{
											goto IL_263;
										}
										neighborX = 7;
										neighborIndex = this.GetIndex(neighborX, neighborY);
										neighborTile = leftChunk.Polygons[(int)neighborIndex];
									}
									else if (neighborY < 0)
									{
										if (bottomChunk == null)
										{
											goto IL_263;
										}
										neighborY = 7;
										neighborIndex = this.GetIndex(neighborX, neighborY);
										neighborTile = bottomChunk.Polygons[(int)neighborIndex];
									}
									else if (neighborX >= 8)
									{
										if (rightChunk == null)
										{
											goto IL_263;
										}
										neighborX = 0;
										neighborIndex = this.GetIndex(neighborX, neighborY);
										neighborTile = rightChunk.Polygons[(int)neighborIndex];
									}
									else if (neighborY >= 8)
									{
										if (topChunk == null)
										{
											goto IL_263;
										}
										neighborY = 0;
										neighborIndex = this.GetIndex(neighborX, neighborY);
										neighborTile = topChunk.Polygons[(int)neighborIndex];
									}
									else
									{
										neighborTile = chunkPolys[(int)neighborIndex];
									}
									byte k = 0;
									while ((int)k < neighborTile.Count)
									{
										PathPoly neighbor2 = neighborTile[(int)k];
										Box2 enlargedNeighbor2 = neighbor2.Box.Enlarged(0.125f);
										Box2 box = enlarged.Intersect(ref enlargedNeighbor2);
										if (Box2.Area(ref box) > 0.125f)
										{
											this.AddNeighbors(poly, neighbor2);
										}
										k += 1;
									}
								}
								IL_263:;
							}
						}
						i += 1;
					}
				}
			}
			this.SendPolys(chunk, component.Owner, chunkPolys);
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0005C283 File Offset: 0x0005A483
		private void AddNeighbors(PathPoly polyA, PathPoly polyB)
		{
			polyA.Neighbors.Add(polyB);
			polyB.Neighbors.Add(polyA);
		}

		// Token: 0x04000A44 RID: 2628
		private const int NodeLimit = 512;

		// Token: 0x04000A45 RID: 2629
		private static readonly PathfindingSystem.PathComparer PathPolyComparer = new PathfindingSystem.PathComparer();

		// Token: 0x04000A46 RID: 2630
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04000A47 RID: 2631
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000A48 RID: 2632
		[Dependency]
		private readonly IParallelManager _parallel;

		// Token: 0x04000A49 RID: 2633
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A4A RID: 2634
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000A4B RID: 2635
		[Dependency]
		private readonly DestructibleSystem _destructible;

		// Token: 0x04000A4C RID: 2636
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x04000A4D RID: 2637
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000A4E RID: 2638
		private ISawmill _sawmill;

		// Token: 0x04000A4F RID: 2639
		private readonly Dictionary<ICommonSession, PathfindingDebugMode> _subscribedSessions = new Dictionary<ICommonSession, PathfindingDebugMode>();

		// Token: 0x04000A50 RID: 2640
		[ViewVariables]
		private readonly List<PathRequest> _pathRequests = new List<PathRequest>(256);

		// Token: 0x04000A51 RID: 2641
		private static readonly TimeSpan PathTime = TimeSpan.FromMilliseconds(3.0);

		// Token: 0x04000A52 RID: 2642
		private const int PathTickLimit = 256;

		// Token: 0x04000A53 RID: 2643
		private int _portalIndex;

		// Token: 0x04000A54 RID: 2644
		private readonly Dictionary<int, PathPortal> _portals = new Dictionary<int, PathPortal>();

		// Token: 0x04000A55 RID: 2645
		private static readonly TimeSpan UpdateCooldown = TimeSpan.FromSeconds(0.45);

		// Token: 0x04000A56 RID: 2646
		public const int PathfindingCollisionMask = 30;

		// Token: 0x04000A57 RID: 2647
		public const int PathfindingCollisionLayer = 65;

		// Token: 0x04000A58 RID: 2648
		public bool PauseUpdating;

		// Token: 0x04000A59 RID: 2649
		private readonly Stopwatch _stopwatch = new Stopwatch();

		// Token: 0x02000969 RID: 2409
		[NullableContext(0)]
		private sealed class PathComparer : IComparer<ValueTuple<float, PathPoly>>
		{
			// Token: 0x0600323F RID: 12863 RVA: 0x00101A66 File Offset: 0x000FFC66
			public int Compare([Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<float, PathPoly> x, [Nullable(new byte[]
			{
				0,
				1
			})] ValueTuple<float, PathPoly> y)
			{
				return y.Item1.CompareTo(x.Item1);
			}
		}
	}
}
