using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000401 RID: 1025
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SignalLinkerSystem)
	})]
	public sealed class SignalReceiverComponent : Component
	{
		// Token: 0x04000CE3 RID: 3299
		[Nullable(1)]
		[DataField("inputs", false, 1, false, false, null)]
		public Dictionary<string, List<PortIdentifier>> Inputs = new Dictionary<string, List<PortIdentifier>>();
	}
}
