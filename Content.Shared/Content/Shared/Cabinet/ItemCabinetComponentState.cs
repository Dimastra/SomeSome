using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cabinet
{
	// Token: 0x0200063E RID: 1598
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ItemCabinetComponentState : ComponentState
	{
		// Token: 0x06001339 RID: 4921 RVA: 0x0003FF91 File Offset: 0x0003E191
		public ItemCabinetComponentState(SoundSpecifier doorSound, bool opened, string openState, string closedState)
		{
			this.DoorSound = doorSound;
			this.Opened = opened;
			this.OpenState = openState;
			this.ClosedState = closedState;
		}

		// Token: 0x0400133B RID: 4923
		public SoundSpecifier DoorSound;

		// Token: 0x0400133C RID: 4924
		public bool Opened;

		// Token: 0x0400133D RID: 4925
		public string OpenState;

		// Token: 0x0400133E RID: 4926
		public string ClosedState;
	}
}
