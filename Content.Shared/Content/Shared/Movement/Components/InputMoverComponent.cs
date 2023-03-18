using System;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002E8 RID: 744
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class InputMoverComponent : Component
	{
		// Token: 0x17000190 RID: 400
		// (get) Token: 0x0600085B RID: 2139 RVA: 0x0001C8D8 File Offset: 0x0001AAD8
		public bool Sprinting
		{
			get
			{
				return (this.HeldMoveButtons & MoveButtons.Walk) == MoveButtons.None;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x0001C8E6 File Offset: 0x0001AAE6
		// (set) Token: 0x0600085D RID: 2141 RVA: 0x0001C8EE File Offset: 0x0001AAEE
		[ViewVariables]
		public bool CanMove { get; set; } = true;

		// Token: 0x0400086D RID: 2157
		[ViewVariables]
		[DataField("toParent", false, 1, false, false, null)]
		public bool ToParent;

		// Token: 0x0400086E RID: 2158
		public GameTick LastInputTick;

		// Token: 0x0400086F RID: 2159
		public ushort LastInputSubTick;

		// Token: 0x04000870 RID: 2160
		public Vector2 CurTickWalkMovement;

		// Token: 0x04000871 RID: 2161
		public Vector2 CurTickSprintMovement;

		// Token: 0x04000872 RID: 2162
		public MoveButtons HeldMoveButtons;

		// Token: 0x04000873 RID: 2163
		public EntityUid? RelativeEntity;

		// Token: 0x04000874 RID: 2164
		[ViewVariables]
		public Angle TargetRelativeRotation = Angle.Zero;

		// Token: 0x04000875 RID: 2165
		[ViewVariables]
		public Angle RelativeRotation;

		// Token: 0x04000876 RID: 2166
		[ViewVariables]
		public float LerpAccumulator;

		// Token: 0x04000877 RID: 2167
		public const float LerpTime = 1f;
	}
}
