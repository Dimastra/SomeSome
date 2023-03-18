using System;
using Content.Shared.Extinguisher;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Server.Extinguisher
{
	// Token: 0x02000506 RID: 1286
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FireExtinguisherSystem)
	})]
	public sealed class FireExtinguisherComponent : SharedFireExtinguisherComponent
	{
	}
}
