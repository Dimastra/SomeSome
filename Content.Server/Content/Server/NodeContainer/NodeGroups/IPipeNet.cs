using System;
using Content.Server.Atmos;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000385 RID: 901
	public interface IPipeNet : INodeGroup, IGasMixtureHolder
	{
		// Token: 0x0600126F RID: 4719
		void Update();
	}
}
