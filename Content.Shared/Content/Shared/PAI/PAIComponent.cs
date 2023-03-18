using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.PAI
{
	// Token: 0x020002A6 RID: 678
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PAIComponent : Component
	{
		// Token: 0x040007AF RID: 1967
		[Nullable(2)]
		[DataField("midiAction", false, 1, true, true, null)]
		public InstantAction MidiAction;
	}
}
