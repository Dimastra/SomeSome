using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Robust.Shared.GameObjects;

// Token: 0x0200000A RID: 10
public sealed class InhaleLocationEvent : EntityEventArgs
{
	// Token: 0x0400000D RID: 13
	[Nullable(2)]
	public GasMixture Gas;
}
