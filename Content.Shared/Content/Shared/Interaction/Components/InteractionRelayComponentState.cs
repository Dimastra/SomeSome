using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Interaction.Components
{
	// Token: 0x020003E0 RID: 992
	[NetSerializable]
	[Serializable]
	public sealed class InteractionRelayComponentState : ComponentState
	{
		// Token: 0x06000BA1 RID: 2977 RVA: 0x00026429 File Offset: 0x00024629
		public InteractionRelayComponentState(EntityUid? relayEntity)
		{
			this.RelayEntity = relayEntity;
		}

		// Token: 0x04000B53 RID: 2899
		public EntityUid? RelayEntity;
	}
}
