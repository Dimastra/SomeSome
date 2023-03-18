using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost
{
	// Token: 0x02000456 RID: 1110
	[NetSerializable]
	[Serializable]
	public sealed class GhostReturnToBodyRequest : EntityEventArgs
	{
	}
}
