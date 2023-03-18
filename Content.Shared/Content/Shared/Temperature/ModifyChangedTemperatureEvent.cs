using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature
{
	// Token: 0x020000DD RID: 221
	public sealed class ModifyChangedTemperatureEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0000BAD6 File Offset: 0x00009CD6
		public SlotFlags TargetSlots { get; } = -4097;

		// Token: 0x0600026A RID: 618 RVA: 0x0000BADE File Offset: 0x00009CDE
		public ModifyChangedTemperatureEvent(float temperature)
		{
			this.TemperatureDelta = temperature;
		}

		// Token: 0x040002D5 RID: 725
		public float TemperatureDelta;
	}
}
