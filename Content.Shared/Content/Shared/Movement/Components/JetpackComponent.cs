using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002E9 RID: 745
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class JetpackComponent : Component
	{
		// Token: 0x04000879 RID: 2169
		[ViewVariables]
		[DataField("moleUsage", false, 1, false, false, null)]
		public float MoleUsage = 0.012f;

		// Token: 0x0400087A RID: 2170
		[Nullable(1)]
		[DataField("toggleAction", false, 1, true, false, null)]
		public InstantAction ToggleAction = new InstantAction();

		// Token: 0x0400087B RID: 2171
		[ViewVariables]
		[DataField("acceleration", false, 1, false, false, null)]
		public float Acceleration = 1f;

		// Token: 0x0400087C RID: 2172
		[ViewVariables]
		[DataField("friction", false, 1, false, false, null)]
		public float Friction = 0.3f;

		// Token: 0x0400087D RID: 2173
		[ViewVariables]
		[DataField("weightlessModifier", false, 1, false, false, null)]
		public float WeightlessModifier = 1.2f;
	}
}
