using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.GameTicking.Rules.Components
{
	// Token: 0x020004CE RID: 1230
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(NukeopsRuleSystem)
	})]
	public sealed class NukeOperativeSpawnerComponent : Component
	{
		// Token: 0x04001016 RID: 4118
		[DataField("name", false, 1, false, false, null)]
		public string OperativeName = "";

		// Token: 0x04001017 RID: 4119
		[DataField("rolePrototype", false, 1, false, false, null)]
		public string OperativeRolePrototype = "";

		// Token: 0x04001018 RID: 4120
		[DataField("startingGearPrototype", false, 1, false, false, null)]
		public string OperativeStartingGear = "";
	}
}
