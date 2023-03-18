using System;
using System.Runtime.CompilerServices;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000381 RID: 897
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class NodeGroupAttribute : Attribute
	{
		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001268 RID: 4712 RVA: 0x0005F31C File Offset: 0x0005D51C
		public NodeGroupID[] NodeGroupIDs { get; }

		// Token: 0x06001269 RID: 4713 RVA: 0x0005F324 File Offset: 0x0005D524
		public NodeGroupAttribute(params NodeGroupID[] nodeGroupTypes)
		{
			this.NodeGroupIDs = nodeGroupTypes;
		}
	}
}
