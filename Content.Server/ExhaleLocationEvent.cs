using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Robust.Shared.GameObjects;

// Token: 0x0200000B RID: 11
public sealed class ExhaleLocationEvent : EntityEventArgs
{
	// Token: 0x0400000E RID: 14
	[Nullable(2)]
	public GasMixture Gas;
}
