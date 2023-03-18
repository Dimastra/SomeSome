using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pulling.Components
{
	// Token: 0x0200023F RID: 575
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedPullingStateManagementSystem)
	})]
	public sealed class SharedPullerComponent : Component
	{
		// Token: 0x1700013A RID: 314
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x00017178 File Offset: 0x00015378
		public float WalkSpeedModifier
		{
			get
			{
				if (this.Pulling != null)
				{
					return 0.9f;
				}
				return 1f;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x000171A0 File Offset: 0x000153A0
		public float SprintSpeedModifier
		{
			get
			{
				if (this.Pulling != null)
				{
					return 0.9f;
				}
				return 1f;
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x000171C8 File Offset: 0x000153C8
		// (set) Token: 0x0600067D RID: 1661 RVA: 0x000171D0 File Offset: 0x000153D0
		[ViewVariables]
		public EntityUid? Pulling { get; set; }

		// Token: 0x0600067E RID: 1662 RVA: 0x000171DC File Offset: 0x000153DC
		protected override void OnRemove()
		{
			if (this.Pulling != null)
			{
				Logger.ErrorS("c.go.c.pulling", "PULLING STATE CORRUPTION IMMINENT IN PULLER {0} - OnRemove called when Pulling is set!", new object[]
				{
					base.Owner
				});
			}
			base.OnRemove();
		}

		// Token: 0x04000672 RID: 1650
		[DataField("needsHands", false, 1, false, false, null)]
		public bool NeedsHands = true;
	}
}
