using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Instruments
{
	// Token: 0x0200044C RID: 1100
	[RegisterComponent]
	public sealed class SwappableInstrumentComponent : Component
	{
		// Token: 0x04000DEA RID: 3562
		[Nullable(new byte[]
		{
			1,
			1,
			0
		})]
		[DataField("instrumentList", false, 1, true, false, null)]
		public Dictionary<string, ValueTuple<byte, byte>> InstrumentList = new Dictionary<string, ValueTuple<byte, byte>>();
	}
}
