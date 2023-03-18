using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pulling.Components
{
	// Token: 0x0200023C RID: 572
	[NullableContext(2)]
	[Nullable(0)]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedPullingStateManagementSystem)
	})]
	[RegisterComponent]
	public sealed class SharedPullableComponent : Component
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x000170A1 File Offset: 0x000152A1
		// (set) Token: 0x0600066D RID: 1645 RVA: 0x000170A9 File Offset: 0x000152A9
		public EntityUid? Puller { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600066E RID: 1646 RVA: 0x000170B2 File Offset: 0x000152B2
		// (set) Token: 0x0600066F RID: 1647 RVA: 0x000170BA File Offset: 0x000152BA
		public string PullJointId { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x000170C4 File Offset: 0x000152C4
		public bool BeingPulled
		{
			get
			{
				return this.Puller != null;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000671 RID: 1649 RVA: 0x000170DF File Offset: 0x000152DF
		// (set) Token: 0x06000672 RID: 1650 RVA: 0x000170E7 File Offset: 0x000152E7
		[Access]
		public EntityCoordinates? MovingTo { get; set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x000170F0 File Offset: 0x000152F0
		// (set) Token: 0x06000674 RID: 1652 RVA: 0x000170F8 File Offset: 0x000152F8
		[Access]
		[ViewVariables]
		[DataField("fixedRotation", false, 1, false, false, null)]
		public bool FixedRotationOnPull { get; set; }

		// Token: 0x06000675 RID: 1653 RVA: 0x00017104 File Offset: 0x00015304
		protected override void OnRemove()
		{
			if (this.Puller != null)
			{
				Logger.ErrorS("c.go.c.pulling", "PULLING STATE CORRUPTION IMMINENT IN PULLABLE {0} - OnRemove called when Puller is set!", new object[]
				{
					base.Owner
				});
			}
			base.OnRemove();
		}

		// Token: 0x0400066E RID: 1646
		[Access]
		[ViewVariables]
		public bool PrevFixedRotation;
	}
}
