using System;
using Content.Shared.CombatMode;
using Robust.Shared.GameObjects;

namespace Content.Server.CombatMode
{
	// Token: 0x02000632 RID: 1586
	[RegisterComponent]
	[ComponentReference(typeof(SharedCombatModeComponent))]
	public sealed class CombatModeComponent : SharedCombatModeComponent
	{
	}
}
