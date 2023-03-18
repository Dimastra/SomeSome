using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Inventory.Controls
{
	// Token: 0x0200007B RID: 123
	[NullableContext(1)]
	public interface IItemslotUIContainer
	{
		// Token: 0x06000293 RID: 659
		bool TryRegisterButton(SlotControl control, string newSlotName);

		// Token: 0x06000294 RID: 660
		bool TryAddButton(SlotControl control);
	}
}
