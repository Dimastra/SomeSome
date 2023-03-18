using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Explosion.Components
{
	// Token: 0x0200051F RID: 1311
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TriggerOnProximityComponent : SharedTriggerOnProximityComponent
	{
		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001B45 RID: 6981 RVA: 0x00092645 File Offset: 0x00090845
		// (set) Token: 0x06001B46 RID: 6982 RVA: 0x0009264D File Offset: 0x0009084D
		[DataField("shape", false, 1, true, false, null)]
		public IPhysShape Shape { get; set; } = new PhysShapeCircle(2f);

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001B47 RID: 6983 RVA: 0x00092656 File Offset: 0x00090856
		// (set) Token: 0x06001B48 RID: 6984 RVA: 0x0009265E File Offset: 0x0009085E
		[DataField("requiresAnchored", false, 1, false, false, null)]
		public bool RequiresAnchored { get; set; } = true;

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001B49 RID: 6985 RVA: 0x00092667 File Offset: 0x00090867
		// (set) Token: 0x06001B4A RID: 6986 RVA: 0x0009266F File Offset: 0x0009086F
		[ViewVariables]
		[DataField("cooldown", false, 1, false, false, null)]
		public float Cooldown { get; set; } = 5f;

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x00092678 File Offset: 0x00090878
		// (set) Token: 0x06001B4C RID: 6988 RVA: 0x00092680 File Offset: 0x00090880
		[ViewVariables]
		[DataField("triggerSpeed", false, 1, false, false, null)]
		public float TriggerSpeed { get; set; } = 3.5f;

		// Token: 0x04001182 RID: 4482
		public const string FixtureID = "trigger-on-proximity-fixture";

		// Token: 0x04001183 RID: 4483
		public readonly HashSet<PhysicsComponent> Colliding = new HashSet<PhysicsComponent>();

		// Token: 0x04001185 RID: 4485
		[DataField("animationDuration", false, 1, false, false, null)]
		public float AnimationDuration = 0.3f;

		// Token: 0x04001187 RID: 4487
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x04001189 RID: 4489
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x0400118B RID: 4491
		[DataField("repeating", false, 1, false, false, null)]
		internal bool Repeating = true;
	}
}
