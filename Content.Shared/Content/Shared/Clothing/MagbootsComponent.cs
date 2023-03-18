using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Clothing
{
	// Token: 0x020005A6 RID: 1446
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MagbootsComponent : Component
	{
		// Token: 0x04001047 RID: 4167
		[Nullable(1)]
		[DataField("toggleAction", false, 1, true, false, null)]
		public InstantAction ToggleAction = new InstantAction();

		// Token: 0x04001048 RID: 4168
		[ViewVariables]
		public bool On;

		// Token: 0x0200084F RID: 2127
		[NetSerializable]
		[Serializable]
		public sealed class MagbootsComponentState : ComponentState
		{
			// Token: 0x17000528 RID: 1320
			// (get) Token: 0x06001950 RID: 6480 RVA: 0x0004FDA5 File Offset: 0x0004DFA5
			public bool On { get; }

			// Token: 0x06001951 RID: 6481 RVA: 0x0004FDAD File Offset: 0x0004DFAD
			public MagbootsComponentState(bool on)
			{
				this.On = on;
			}
		}
	}
}
