using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004BB RID: 1211
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EnsnareableComponent : Component
	{
		// Token: 0x04000DC8 RID: 3528
		[ViewVariables]
		[DataField("walkSpeed", false, 1, false, false, null)]
		public float WalkSpeed = 1f;

		// Token: 0x04000DC9 RID: 3529
		[ViewVariables]
		[DataField("sprintSpeed", false, 1, false, false, null)]
		public float SprintSpeed = 1f;

		// Token: 0x04000DCA RID: 3530
		[ViewVariables]
		[DataField("isEnsnared", false, 1, false, false, null)]
		public bool IsEnsnared;

		// Token: 0x04000DCB RID: 3531
		[Nullable(1)]
		public Container Container;

		// Token: 0x04000DCC RID: 3532
		[DataField("sprite", false, 1, false, false, null)]
		public string Sprite;

		// Token: 0x04000DCD RID: 3533
		[DataField("state", false, 1, false, false, null)]
		public string State;
	}
}
