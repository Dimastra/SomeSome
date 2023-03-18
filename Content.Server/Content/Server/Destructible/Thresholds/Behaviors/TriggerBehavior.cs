using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005AF RID: 1455
	[DataDefinition]
	public sealed class TriggerBehavior : IThresholdBehavior
	{
		// Token: 0x06001E29 RID: 7721 RVA: 0x0009F770 File Offset: 0x0009D970
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			system.TriggerSystem.Trigger(owner, cause);
		}
	}
}
