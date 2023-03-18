using System;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration
{
	// Token: 0x02000744 RID: 1860
	[NetSerializable]
	[Serializable]
	public sealed class SetOutfitEuiState : EuiStateBase
	{
		// Token: 0x040016D1 RID: 5841
		public EntityUid TargetEntityId;
	}
}
