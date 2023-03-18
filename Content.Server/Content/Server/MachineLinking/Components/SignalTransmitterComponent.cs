using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.MachineLinking.System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x02000405 RID: 1029
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SignalLinkerSystem)
	})]
	public sealed class SignalTransmitterComponent : Component
	{
		// Token: 0x04000CF4 RID: 3316
		[DataField("transmissionRange", false, 1, false, false, null)]
		[ViewVariables]
		public float TransmissionRange = 30f;

		// Token: 0x04000CF5 RID: 3317
		[Nullable(1)]
		[DataField("outputs", false, 1, false, false, null)]
		[Access]
		public Dictionary<string, List<PortIdentifier>> Outputs = new Dictionary<string, List<PortIdentifier>>();
	}
}
