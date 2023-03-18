using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000615 RID: 1557
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class CartridgeLoaderComponent : Component
	{
		// Token: 0x040012CB RID: 4811
		public const string CartridgeSlotId = "Cartridge-Slot";

		// Token: 0x040012CC RID: 4812
		[DataField("cartridgeSlot", false, 1, false, false, null)]
		public ItemSlot CartridgeSlot = new ItemSlot();

		// Token: 0x040012CD RID: 4813
		[DataField("preinstalled", false, 1, false, false, null)]
		public List<string> PreinstalledPrograms = new List<string>();

		// Token: 0x040012CE RID: 4814
		[ViewVariables]
		public EntityUid? ActiveProgram;

		// Token: 0x040012CF RID: 4815
		[ViewVariables]
		public readonly List<EntityUid> BackgroundPrograms = new List<EntityUid>();

		// Token: 0x040012D0 RID: 4816
		[DataField("installedCartridges", false, 1, false, false, null)]
		public List<EntityUid> InstalledPrograms = new List<EntityUid>();

		// Token: 0x040012D1 RID: 4817
		[DataField("diskSpace", false, 1, false, false, null)]
		public int DiskSpace = 5;

		// Token: 0x040012D2 RID: 4818
		[DataField("uiKey", true, 1, true, false, typeof(EnumSerializer))]
		public Enum UiKey;
	}
}
