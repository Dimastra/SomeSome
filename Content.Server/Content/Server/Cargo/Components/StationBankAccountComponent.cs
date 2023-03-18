using System;
using Content.Shared.Cargo;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006EE RID: 1774
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedCargoSystem)
	})]
	public sealed class StationBankAccountComponent : Component
	{
		// Token: 0x040016C6 RID: 5830
		[ViewVariables]
		[DataField("balance", false, 1, false, false, null)]
		public int Balance = 2000;

		// Token: 0x040016C7 RID: 5831
		[ViewVariables]
		[DataField("increasePerSecond", false, 1, false, false, null)]
		public int IncreasePerSecond = 1;
	}
}
