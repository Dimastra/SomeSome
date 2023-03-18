using System;
using Content.Server.Physics.Controllers;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Physics.Components
{
	// Token: 0x020002DD RID: 733
	[RegisterComponent]
	public sealed class RandomWalkComponent : Component
	{
		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0004CA41 File Offset: 0x0004AC41
		// (set) Token: 0x06000EF4 RID: 3828 RVA: 0x0004CA49 File Offset: 0x0004AC49
		[DataField("minStepCooldown", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan MinStepCooldown { get; internal set; } = TimeSpan.FromSeconds(2.0);

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0004CA52 File Offset: 0x0004AC52
		// (set) Token: 0x06000EF6 RID: 3830 RVA: 0x0004CA5A File Offset: 0x0004AC5A
		[DataField("maxStepCooldown", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan MaxStepCooldown { get; internal set; } = TimeSpan.FromSeconds(5.0);

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0004CA63 File Offset: 0x0004AC63
		// (set) Token: 0x06000EF8 RID: 3832 RVA: 0x0004CA6B File Offset: 0x0004AC6B
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(RandomWalkController)
		})]
		public TimeSpan NextStepTime { get; internal set; }

		// Token: 0x040008C7 RID: 2247
		[DataField("minSpeed", false, 1, false, false, null)]
		[ViewVariables]
		public float MinSpeed = 7.5f;

		// Token: 0x040008C8 RID: 2248
		[DataField("maxSpeed", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxSpeed = 10f;

		// Token: 0x040008C9 RID: 2249
		[DataField("accumulatorRatio", false, 1, false, false, null)]
		[ViewVariables]
		public float AccumulatorRatio;

		// Token: 0x040008CA RID: 2250
		[DataField("stepOnStartup", false, 1, false, false, null)]
		[ViewVariables]
		public bool StepOnStartup;
	}
}
