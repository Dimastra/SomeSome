using System;
using Robust.Shared.GameObjects;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x02000379 RID: 889
	public interface IRotatableNode
	{
		// Token: 0x06001228 RID: 4648
		bool RotateNode(in MoveEvent ev);
	}
}
