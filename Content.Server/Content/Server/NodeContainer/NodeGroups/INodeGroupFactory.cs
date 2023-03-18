using System;
using System.Runtime.CompilerServices;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000382 RID: 898
	[NullableContext(1)]
	public interface INodeGroupFactory
	{
		// Token: 0x0600126A RID: 4714
		void Initialize();

		// Token: 0x0600126B RID: 4715
		INodeGroup MakeNodeGroup(NodeGroupID id);
	}
}
