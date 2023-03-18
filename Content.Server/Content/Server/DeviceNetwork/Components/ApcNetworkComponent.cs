using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.DeviceNetwork.Components
{
	// Token: 0x0200058C RID: 1420
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ApcNetworkSystem)
	})]
	[ComponentProtoName("ApcNetworkConnection")]
	public sealed class ApcNetworkComponent : Component
	{
		// Token: 0x0400130B RID: 4875
		[Nullable(2)]
		[ViewVariables]
		public Node ConnectedNode;
	}
}
