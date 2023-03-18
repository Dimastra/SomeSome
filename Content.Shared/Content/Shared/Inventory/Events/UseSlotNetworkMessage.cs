using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003C0 RID: 960
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class UseSlotNetworkMessage : EntityEventArgs
	{
		// Token: 0x06000B06 RID: 2822 RVA: 0x00024554 File Offset: 0x00022754
		public UseSlotNetworkMessage(string slot)
		{
			this.Slot = slot;
		}

		// Token: 0x04000AFB RID: 2811
		public readonly string Slot;
	}
}
