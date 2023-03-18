using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200029A RID: 666
	public static class StaticPowerSystem
	{
		// Token: 0x06000D8F RID: 3471 RVA: 0x00046FEA File Offset: 0x000451EA
		[NullableContext(1)]
		public static bool IsPowered(this EntitySystem system, EntityUid uid, IEntityManager entManager, [Nullable(2)] ApcPowerReceiverComponent receiver = null)
		{
			return (receiver != null || entManager.TryGetComponent<ApcPowerReceiverComponent>(uid, ref receiver)) && receiver.Powered;
		}
	}
}
