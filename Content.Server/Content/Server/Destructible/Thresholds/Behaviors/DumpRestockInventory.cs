using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.VendingMachines.Restock;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Content.Shared.VendingMachines;
using Robust.Shared.GameObjects;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
	// Token: 0x020005A5 RID: 1445
	[DataDefinition]
	[Serializable]
	public sealed class DumpRestockInventory : IThresholdBehavior
	{
		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001E0E RID: 7694 RVA: 0x0009F09E File Offset: 0x0009D29E
		// (set) Token: 0x06001E0F RID: 7695 RVA: 0x0009F0A6 File Offset: 0x0009D2A6
		[DataField("offset", false, 1, false, false, null)]
		public float Offset { get; set; } = 0.5f;

		// Token: 0x06001E10 RID: 7696 RVA: 0x0009F0B0 File Offset: 0x0009D2B0
		[NullableContext(1)]
		public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
		{
			VendingMachineRestockComponent packagecomp;
			TransformComponent xform;
			if (!system.EntityManager.TryGetComponent<VendingMachineRestockComponent>(owner, ref packagecomp) || !system.EntityManager.TryGetComponent<TransformComponent>(owner, ref xform))
			{
				return;
			}
			string randomInventory = RandomExtensions.Pick<string>(system.Random, packagecomp.CanRestock);
			VendingMachineInventoryPrototype packPrototype;
			if (!system.PrototypeManager.TryIndex<VendingMachineInventoryPrototype>(randomInventory, ref packPrototype))
			{
				return;
			}
			foreach (KeyValuePair<string, uint> keyValuePair in packPrototype.StartingInventory)
			{
				string text;
				uint num;
				keyValuePair.Deconstruct(out text, out num);
				string entityId = text;
				int toSpawn = (int)Math.Round((double)(num * this.Percent));
				if (toSpawn != 0)
				{
					if (EntityPrototypeHelpers.HasComponent<StackComponent>(entityId, system.PrototypeManager, system.ComponentFactory))
					{
						EntityUid spawned = system.EntityManager.SpawnEntity(entityId, xform.Coordinates.Offset(system.Random.NextVector2(-this.Offset, this.Offset)));
						system.StackSystem.SetCount(spawned, toSpawn, null);
						system.EntityManager.GetComponent<TransformComponent>(spawned).LocalRotation = system.Random.NextAngle();
					}
					else
					{
						for (int i = 0; i < toSpawn; i++)
						{
							EntityUid spawned2 = system.EntityManager.SpawnEntity(entityId, xform.Coordinates.Offset(system.Random.NextVector2(-this.Offset, this.Offset)));
							system.EntityManager.GetComponent<TransformComponent>(spawned2).LocalRotation = system.Random.NextAngle();
						}
					}
				}
			}
		}

		// Token: 0x04001340 RID: 4928
		[DataField("percent", false, 1, true, false, null)]
		public float Percent = 0.5f;
	}
}
