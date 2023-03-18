using System;
using System.Runtime.CompilerServices;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged
{
	// Token: 0x02000043 RID: 67
	[RegisterComponent]
	[Virtual]
	public class MagazineAmmoProviderComponent : AmmoProviderComponent
	{
		// Token: 0x040000BA RID: 186
		[Nullable(2)]
		[ViewVariables]
		[DataField("soundAutoEject", false, 1, false, false, null)]
		public SoundSpecifier SoundAutoEject = new SoundPathSpecifier("/Audio/Weapons/Guns/EmptyAlarm/smg_empty_alarm.ogg", null);

		// Token: 0x040000BB RID: 187
		[ViewVariables]
		[DataField("autoEject", false, 1, false, false, null)]
		public bool AutoEject;
	}
}
