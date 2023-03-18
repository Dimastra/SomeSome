using System;
using Content.Server.Access.Systems;
using Content.Shared.Access.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Server.Access.Components
{
	// Token: 0x02000880 RID: 2176
	[RegisterComponent]
	[ComponentReference(typeof(SharedIdCardConsoleComponent))]
	[Access(new Type[]
	{
		typeof(IdCardConsoleSystem)
	})]
	public sealed class IdCardConsoleComponent : SharedIdCardConsoleComponent
	{
	}
}
