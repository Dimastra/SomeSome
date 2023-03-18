using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Robust.Shared.GameObjects;

// Token: 0x02000008 RID: 8
public sealed class GasAnalyzerScanEvent : EntityEventArgs
{
	// Token: 0x04000008 RID: 8
	[Nullable(new byte[]
	{
		2,
		1,
		2
	})]
	public Dictionary<string, GasMixture> GasMixtures;

	// Token: 0x04000009 RID: 9
	public bool DeviceFlipped;
}
