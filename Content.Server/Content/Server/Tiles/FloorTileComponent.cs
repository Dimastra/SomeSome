using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.Tiles
{
	// Token: 0x02000120 RID: 288
	[RegisterComponent]
	public sealed class FloorTileComponent : Component
	{
		// Token: 0x04000320 RID: 800
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("outputs", false, 1, false, false, typeof(PrototypeIdListSerializer<ContentTileDefinition>))]
		public List<string> OutputTiles;

		// Token: 0x04000321 RID: 801
		[Nullable(1)]
		[DataField("placeTileSound", false, 1, false, false, null)]
		public SoundSpecifier PlaceTileSound = new SoundPathSpecifier("/Audio/Items/genhit.ogg", null);
	}
}
