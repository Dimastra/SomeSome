using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction
{
	// Token: 0x02000567 RID: 1383
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class ConstructionGraphNode
	{
		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x00037B4B File Offset: 0x00035D4B
		// (set) Token: 0x060010D9 RID: 4313 RVA: 0x00037B53 File Offset: 0x00035D53
		[DataField("node", false, 1, true, false, null)]
		public string Name { get; private set; }

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060010DA RID: 4314 RVA: 0x00037B5C File Offset: 0x00035D5C
		[ViewVariables]
		public IReadOnlyList<ConstructionGraphEdge> Edges
		{
			get
			{
				return this._edges;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060010DB RID: 4315 RVA: 0x00037B64 File Offset: 0x00035D64
		[ViewVariables]
		public IReadOnlyList<IGraphAction> Actions
		{
			get
			{
				return this._actions;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060010DC RID: 4316 RVA: 0x00037B6C File Offset: 0x00035D6C
		// (set) Token: 0x060010DD RID: 4317 RVA: 0x00037B74 File Offset: 0x00035D74
		[Nullable(2)]
		[DataField("entity", false, 1, false, false, null)]
		public string Entity { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x060010DE RID: 4318 RVA: 0x00037B80 File Offset: 0x00035D80
		[return: Nullable(2)]
		public ConstructionGraphEdge GetEdge(string target)
		{
			foreach (ConstructionGraphEdge edge in this._edges)
			{
				if (edge.Target == target)
				{
					return edge;
				}
			}
			return null;
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x00037BB8 File Offset: 0x00035DB8
		public int? GetEdgeIndex(string target)
		{
			for (int i = 0; i < this._edges.Length; i++)
			{
				if (this._edges[i].Target == target)
				{
					return new int?(i);
				}
			}
			return null;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00037C00 File Offset: 0x00035E00
		public bool TryGetEdge(string target, [Nullable(2)] [NotNullWhen(true)] out ConstructionGraphEdge edge)
		{
			ConstructionGraphEdge edge2;
			edge = (edge2 = this.GetEdge(target));
			return edge2 != null;
		}

		// Token: 0x04000FCE RID: 4046
		[DataField("actions", false, 1, false, true, null)]
		private IGraphAction[] _actions = Array.Empty<IGraphAction>();

		// Token: 0x04000FCF RID: 4047
		[DataField("edges", false, 1, false, false, null)]
		private ConstructionGraphEdge[] _edges = Array.Empty<ConstructionGraphEdge>();
	}
}
