using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radio.Components
{
	// Token: 0x02000224 RID: 548
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class EncryptionKeyHolderComponent : Component
	{
		// Token: 0x04000615 RID: 1557
		[ViewVariables]
		[DataField("keysUnlocked", false, 1, false, false, null)]
		public bool KeysUnlocked = true;

		// Token: 0x04000616 RID: 1558
		[ViewVariables]
		[DataField("keysExtractionMethod", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string KeysExtractionMethod = "Screwing";

		// Token: 0x04000617 RID: 1559
		[ViewVariables]
		[DataField("keySlots", false, 1, false, false, null)]
		public int KeySlots = 2;

		// Token: 0x04000618 RID: 1560
		[ViewVariables]
		[DataField("keyExtractionSound", false, 1, false, false, null)]
		public SoundSpecifier KeyExtractionSound = new SoundPathSpecifier("/Audio/Items/pistol_magout.ogg", null);

		// Token: 0x04000619 RID: 1561
		[ViewVariables]
		[DataField("keyInsertionSound", false, 1, false, false, null)]
		public SoundSpecifier KeyInsertionSound = new SoundPathSpecifier("/Audio/Items/pistol_magin.ogg", null);

		// Token: 0x0400061A RID: 1562
		[ViewVariables]
		public Container KeyContainer;

		// Token: 0x0400061B RID: 1563
		public const string KeyContainerName = "key_slots";

		// Token: 0x0400061C RID: 1564
		[ViewVariables]
		public HashSet<string> Channels = new HashSet<string>();

		// Token: 0x0400061D RID: 1565
		[Nullable(2)]
		[ViewVariables]
		public string DefaultChannel;
	}
}
