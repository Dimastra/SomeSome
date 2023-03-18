using System;
using Content.Server.Clothing;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.EntitySystems
{
	// Token: 0x0200030A RID: 778
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(FoodSystem),
		typeof(DrinkSystem),
		typeof(MaskSystem)
	})]
	public sealed class IngestionBlockerComponent : Component
	{
		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x0005148A File Offset: 0x0004F68A
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x00051492 File Offset: 0x0004F692
		[ViewVariables]
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;
	}
}
