using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Components
{
	// Token: 0x020005F8 RID: 1528
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MovespeedModifierMetabolismComponent : Component
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x0003CBB1 File Offset: 0x0003ADB1
		// (set) Token: 0x06001293 RID: 4755 RVA: 0x0003CBB9 File Offset: 0x0003ADB9
		[ViewVariables]
		public float WalkSpeedModifier { get; set; }

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x0003CBC2 File Offset: 0x0003ADC2
		// (set) Token: 0x06001295 RID: 4757 RVA: 0x0003CBCA File Offset: 0x0003ADCA
		[ViewVariables]
		public float SprintSpeedModifier { get; set; }

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06001296 RID: 4758 RVA: 0x0003CBD3 File Offset: 0x0003ADD3
		// (set) Token: 0x06001297 RID: 4759 RVA: 0x0003CBDB File Offset: 0x0003ADDB
		[ViewVariables]
		public TimeSpan ModifierTimer { get; set; } = TimeSpan.Zero;

		// Token: 0x06001298 RID: 4760 RVA: 0x0003CBE4 File Offset: 0x0003ADE4
		[NullableContext(1)]
		public override ComponentState GetComponentState()
		{
			return new MovespeedModifierMetabolismComponent.MovespeedModifierMetabolismComponentState(this.WalkSpeedModifier, this.SprintSpeedModifier, this.ModifierTimer);
		}

		// Token: 0x02000854 RID: 2132
		[NetSerializable]
		[Serializable]
		public sealed class MovespeedModifierMetabolismComponentState : ComponentState
		{
			// Token: 0x1700052B RID: 1323
			// (get) Token: 0x06001959 RID: 6489 RVA: 0x0004FE25 File Offset: 0x0004E025
			public float WalkSpeedModifier { get; }

			// Token: 0x1700052C RID: 1324
			// (get) Token: 0x0600195A RID: 6490 RVA: 0x0004FE2D File Offset: 0x0004E02D
			public float SprintSpeedModifier { get; }

			// Token: 0x1700052D RID: 1325
			// (get) Token: 0x0600195B RID: 6491 RVA: 0x0004FE35 File Offset: 0x0004E035
			public TimeSpan ModifierTimer { get; }

			// Token: 0x0600195C RID: 6492 RVA: 0x0004FE3D File Offset: 0x0004E03D
			public MovespeedModifierMetabolismComponentState(float walkSpeedModifier, float sprintSpeedModifier, TimeSpan modifierTimer)
			{
				this.WalkSpeedModifier = walkSpeedModifier;
				this.SprintSpeedModifier = sprintSpeedModifier;
				this.ModifierTimer = modifierTimer;
			}
		}
	}
}
