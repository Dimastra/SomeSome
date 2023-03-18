using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.VendingMachines;
using Content.Shared.VendingMachines;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A6 RID: 1446
	[DataDefinition]
	[Serializable]
	public sealed class EjectVendorItems : IThresholdBehavior
	{
		// Token: 0x06001E12 RID: 7698 RVA: 0x0009F27C File Offset: 0x0009D47C
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			VendingMachineComponent vendingcomp;
			TransformComponent xform;
			if (!system.EntityManager.TryGetComponent<VendingMachineComponent>(owner, ref vendingcomp) || !system.EntityManager.TryGetComponent<TransformComponent>(owner, ref xform))
			{
				return;
			}
			VendingMachineSystem vendingMachineSystem = EntitySystem.Get<VendingMachineSystem>();
			List<VendingMachineInventoryEntry> inventory = vendingMachineSystem.GetAvailableInventory(owner, vendingcomp);
			if (inventory.Count <= 0)
			{
				return;
			}
			float toEject = Math.Min((float)inventory.Count * this.Percent, (float)this.Max);
			int i = 0;
			while ((float)i < toEject)
			{
				vendingMachineSystem.EjectRandom(owner, true, true, vendingcomp);
				i++;
			}
		}

		// Token: 0x04001342 RID: 4930
		[DataField("percent", false, 1, true, false, null)]
		public float Percent = 0.25f;

		// Token: 0x04001343 RID: 4931
		[DataField("max", false, 1, false, false, null)]
		public int Max = 3;
	}
}
