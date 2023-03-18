using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lock
{
	// Token: 0x02000358 RID: 856
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(LockSystem)
	})]
	public sealed class LockComponent : Component
	{
		// Token: 0x040009C0 RID: 2496
		[DataField("locked", false, 1, false, false, null)]
		[ViewVariables]
		public bool Locked = true;

		// Token: 0x040009C1 RID: 2497
		[DataField("lockOnClick", false, 1, false, false, null)]
		[ViewVariables]
		public bool LockOnClick;

		// Token: 0x040009C2 RID: 2498
		[DataField("unlockingSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier UnlockSound = new SoundPathSpecifier("/Audio/Machines/door_lock_off.ogg", null);

		// Token: 0x040009C3 RID: 2499
		[DataField("lockingSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier LockSound = new SoundPathSpecifier("/Audio/Machines/door_lock_on.ogg", null);
	}
}
