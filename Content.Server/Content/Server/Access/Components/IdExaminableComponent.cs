using System;
using Content.Server.Access.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Access.Components
{
	// Token: 0x02000881 RID: 2177
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(IdExaminableSystem)
	})]
	public sealed class IdExaminableComponent : Component
	{
	}
}
