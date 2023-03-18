using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Movement.Components
{
	// Token: 0x02000393 RID: 915
	[RegisterComponent]
	public sealed class StressTestMovementComponent : Component
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060012BC RID: 4796 RVA: 0x00061170 File Offset: 0x0005F370
		// (set) Token: 0x060012BD RID: 4797 RVA: 0x00061178 File Offset: 0x0005F378
		public float Progress { get; set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x00061181 File Offset: 0x0005F381
		// (set) Token: 0x060012BF RID: 4799 RVA: 0x00061189 File Offset: 0x0005F389
		public Vector2 Origin { get; set; }

		// Token: 0x060012C0 RID: 4800 RVA: 0x00061192 File Offset: 0x0005F392
		protected override void Startup()
		{
			base.Startup();
			this.Origin = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(base.Owner).WorldPosition;
		}
	}
}
