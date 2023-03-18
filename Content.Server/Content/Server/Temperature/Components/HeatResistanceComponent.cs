using System;
using Content.Server.Clothing.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Temperature.Components
{
	// Token: 0x02000127 RID: 295
	[RegisterComponent]
	public sealed class HeatResistanceComponent : Component
	{
		// Token: 0x06000557 RID: 1367 RVA: 0x0001A2DC File Offset: 0x000184DC
		public int GetHeatResistance()
		{
			EntityUid? slotEntity;
			GloveHeatResistanceComponent gloves;
			if (EntitySystem.Get<InventorySystem>().TryGetSlotEntity(base.Owner, "gloves", out slotEntity, null, null) && IoCManager.Resolve<IEntityManager>().TryGetComponent<GloveHeatResistanceComponent>(slotEntity, ref gloves))
			{
				return gloves.HeatResistance;
			}
			return int.MinValue;
		}
	}
}
