using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NodeContainer
{
	// Token: 0x02000377 RID: 887
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class NodeContainerComponent : Component
	{
		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x0005ED97 File Offset: 0x0005CF97
		[DataField("nodes", true, 1, false, false, null)]
		public Dictionary<string, Node> Nodes { get; } = new Dictionary<string, Node>();

		// Token: 0x06001223 RID: 4643 RVA: 0x0005ED9F File Offset: 0x0005CF9F
		public T GetNode<[Nullable(0)] T>(string identifier) where T : Node
		{
			return (T)((object)this.Nodes[identifier]);
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x0005EDB4 File Offset: 0x0005CFB4
		[NullableContext(2)]
		public bool TryGetNode<[Nullable(0)] T>(string identifier, [NotNullWhen(true)] out T node) where T : Node
		{
			if (identifier == null)
			{
				node = default(T);
				return false;
			}
			Node i;
			if (this.Nodes.TryGetValue(identifier, out i))
			{
				T t = i as T;
				if (t != null)
				{
					node = t;
					return true;
				}
			}
			node = default(T);
			return false;
		}

		// Token: 0x04000B31 RID: 2865
		[DataField("examinable", false, 1, false, false, null)]
		public bool Examinable;
	}
}
