using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Prototypes
{
	// Token: 0x02000579 RID: 1401
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("constructionGraph", 1)]
	public sealed class ConstructionGraphPrototype : IPrototype, ISerializationHooks
	{
		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001125 RID: 4389 RVA: 0x0003880C File Offset: 0x00036A0C
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06001126 RID: 4390 RVA: 0x00038814 File Offset: 0x00036A14
		[Nullable(2)]
		[DataField("start", false, 1, false, false, null)]
		public string Start { [NullableContext(2)] get; }

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0003881C File Offset: 0x00036A1C
		[ViewVariables]
		public IReadOnlyDictionary<string, ConstructionGraphNode> Nodes
		{
			get
			{
				return this._nodes;
			}
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00038824 File Offset: 0x00036A24
		void ISerializationHooks.AfterDeserialization()
		{
			this._nodes.Clear();
			foreach (ConstructionGraphNode graphNode in this._graph)
			{
				if (string.IsNullOrEmpty(graphNode.Name))
				{
					throw new InvalidDataException("Name of graph node is null in construction graph " + this.ID + "!");
				}
				this._nodes[graphNode.Name] = graphNode;
			}
			if (string.IsNullOrEmpty(this.Start) || !this._nodes.ContainsKey(this.Start))
			{
				throw new InvalidDataException("Starting node for construction graph " + this.ID + " is null, empty or invalid!");
			}
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x000388F0 File Offset: 0x00036AF0
		[return: Nullable(2)]
		public ConstructionGraphEdge Edge(string startNode, string nextNode)
		{
			return this._nodes[startNode].GetEdge(nextNode);
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00038904 File Offset: 0x00036B04
		public bool TryPath(string startNode, string finishNode, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out ConstructionGraphNode[] path)
		{
			ConstructionGraphNode[] array;
			path = (array = this.Path(startNode, finishNode));
			return array != null;
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00038924 File Offset: 0x00036B24
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public string[] PathId(string startNode, string finishNode)
		{
			ConstructionGraphNode[] path = this.Path(startNode, finishNode);
			if (path == null)
			{
				return null;
			}
			string[] nodes = new string[path.Length];
			for (int i = 0; i < path.Length; i++)
			{
				nodes[i] = path[i].Name;
			}
			return nodes;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00038964 File Offset: 0x00036B64
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public ConstructionGraphNode[] Path(string startNode, string finishNode)
		{
			ValueTuple<string, string> tuple = new ValueTuple<string, string>(startNode, finishNode);
			if (this._paths.ContainsKey(tuple))
			{
				return this._paths[tuple];
			}
			Dictionary<ConstructionGraphNode, ConstructionGraphNode> pathfindingForStart;
			if (this._pathfinding.ContainsKey(startNode))
			{
				pathfindingForStart = this._pathfinding[startNode];
			}
			else
			{
				pathfindingForStart = (this._pathfinding[startNode] = this.PathsForStart(startNode));
			}
			ConstructionGraphNode start = this._nodes[startNode];
			ConstructionGraphNode current = this._nodes[finishNode];
			List<ConstructionGraphNode> path = new List<ConstructionGraphNode>();
			while (current != start)
			{
				if (current == null || !pathfindingForStart.ContainsKey(current))
				{
					this._paths[tuple] = null;
					return null;
				}
				path.Add(current);
				current = pathfindingForStart[current];
			}
			path.Reverse();
			return this._paths[tuple] = path.ToArray();
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00038A3C File Offset: 0x00036C3C
		[return: Nullable(new byte[]
		{
			1,
			1,
			2
		})]
		private Dictionary<ConstructionGraphNode, ConstructionGraphNode> PathsForStart(string start)
		{
			ConstructionGraphNode startNode = this._nodes[start];
			Queue<ConstructionGraphNode> frontier = new Queue<ConstructionGraphNode>();
			Dictionary<ConstructionGraphNode, ConstructionGraphNode> cameFrom = new Dictionary<ConstructionGraphNode, ConstructionGraphNode>();
			frontier.Enqueue(startNode);
			cameFrom[startNode] = null;
			while (frontier.Count != 0)
			{
				ConstructionGraphNode current = frontier.Dequeue();
				foreach (ConstructionGraphEdge edge in current.Edges)
				{
					ConstructionGraphNode edgeNode = this._nodes[edge.Target];
					if (!cameFrom.ContainsKey(edgeNode))
					{
						frontier.Enqueue(edgeNode);
						cameFrom[edgeNode] = current;
					}
				}
			}
			return cameFrom;
		}

		// Token: 0x04000FE9 RID: 4073
		private readonly Dictionary<string, ConstructionGraphNode> _nodes = new Dictionary<string, ConstructionGraphNode>();

		// Token: 0x04000FEA RID: 4074
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1,
			2,
			1
		})]
		private readonly Dictionary<ValueTuple<string, string>, ConstructionGraphNode[]> _paths = new Dictionary<ValueTuple<string, string>, ConstructionGraphNode[]>();

		// Token: 0x04000FEB RID: 4075
		[Nullable(new byte[]
		{
			1,
			1,
			1,
			1,
			2
		})]
		private readonly Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode>> _pathfinding = new Dictionary<string, Dictionary<ConstructionGraphNode, ConstructionGraphNode>>();

		// Token: 0x04000FEE RID: 4078
		[DataField("graph", false, 0, false, false, null)]
		private List<ConstructionGraphNode> _graph = new List<ConstructionGraphNode>();
	}
}
