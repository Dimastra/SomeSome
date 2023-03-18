using System;
using System.Runtime.CompilerServices;
using Content.Server.Plants.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Plants.Components
{
	// Token: 0x020002D7 RID: 727
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PottedPlantHideSystem)
	})]
	public sealed class PottedPlantHideComponent : Component
	{
		// Token: 0x040008AD RID: 2221
		[Nullable(1)]
		[DataField("rustleSound", false, 1, false, false, null)]
		public SoundSpecifier RustleSound = new SoundPathSpecifier("/Audio/Effects/plant_rustle.ogg", null);
	}
}
