using System;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Systems;
using Content.Shared.PDA;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Access.Components
{
	// Token: 0x0200077C RID: 1916
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedIdCardSystem),
		typeof(SharedPDASystem),
		typeof(SharedAgentIdCardSystem)
	})]
	public sealed class IdCardComponent : Component
	{
		// Token: 0x0400175C RID: 5980
		[DataField("fullName", false, 1, false, false, null)]
		[Access]
		public string FullName;

		// Token: 0x0400175D RID: 5981
		[DataField("jobTitle", false, 1, false, false, null)]
		public string JobTitle;
	}
}
