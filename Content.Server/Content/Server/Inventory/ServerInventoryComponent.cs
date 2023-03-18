using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Server.Inventory
{
	// Token: 0x02000443 RID: 1091
	[RegisterComponent]
	[ComponentReference(typeof(InventoryComponent))]
	public sealed class ServerInventoryComponent : InventoryComponent
	{
	}
}
