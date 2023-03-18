using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A8 RID: 1448
	[DataDefinition]
	public sealed class ExplodeBehavior : IThresholdBehavior
	{
		// Token: 0x06001E16 RID: 7702 RVA: 0x0009F3A0 File Offset: 0x0009D5A0
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			system.ExplosionSystem.TriggerExplosive(owner, null, true, null, null, cause);
		}
	}
}
