using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Storage.Components
{
	// Token: 0x0200016A RID: 362
	[RegisterComponent]
	public sealed class CursedEntityStorageComponent : Component
	{
		// Token: 0x0400042C RID: 1068
		[Nullable(1)]
		[DataField("cursedSound", false, 1, false, false, null)]
		public SoundSpecifier CursedSound = new SoundPathSpecifier("/Audio/Effects/teleport_departure.ogg", null);
	}
}
