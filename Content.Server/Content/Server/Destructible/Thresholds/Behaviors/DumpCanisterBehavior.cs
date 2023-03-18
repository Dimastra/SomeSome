using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A4 RID: 1444
	[DataDefinition]
	[Serializable]
	public sealed class DumpCanisterBehavior : IThresholdBehavior
	{
		// Token: 0x06001E0C RID: 7692 RVA: 0x0009F07C File Offset: 0x0009D27C
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			system.EntityManager.EntitySysManager.GetEntitySystem<GasCanisterSystem>().PurgeContents(owner, null, null);
		}
	}
}
