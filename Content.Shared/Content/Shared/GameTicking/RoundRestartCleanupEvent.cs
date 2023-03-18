using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000462 RID: 1122
	[NetSerializable]
	[Serializable]
	public sealed class RoundRestartCleanupEvent : EntityEventArgs
	{
	}
}
