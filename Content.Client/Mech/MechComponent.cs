using System;
using Content.Shared.Mech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Mech
{
	// Token: 0x0200023C RID: 572
	[RegisterComponent]
	[NetworkedComponent]
	[ComponentReference(typeof(SharedMechComponent))]
	public sealed class MechComponent : SharedMechComponent
	{
	}
}
