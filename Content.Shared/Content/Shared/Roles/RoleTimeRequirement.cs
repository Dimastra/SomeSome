using System;
using System.Runtime.CompilerServices;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Roles
{
	// Token: 0x020001E7 RID: 487
	public sealed class RoleTimeRequirement : JobRequirement
	{
		// Token: 0x0400058C RID: 1420
		[Nullable(1)]
		[DataField("role", false, 1, false, false, typeof(PrototypeIdSerializer<PlayTimeTrackerPrototype>))]
		public string Role;

		// Token: 0x0400058D RID: 1421
		[DataField("time", false, 1, false, false, null)]
		public TimeSpan Time;

		// Token: 0x0400058E RID: 1422
		[DataField("inverted", false, 1, false, false, null)]
		public bool Inverted;
	}
}
