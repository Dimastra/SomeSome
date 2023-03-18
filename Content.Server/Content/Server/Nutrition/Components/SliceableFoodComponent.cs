using System;
using System.Runtime.CompilerServices;
using Content.Server.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031C RID: 796
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SliceableFoodSystem)
	})]
	internal sealed class SliceableFoodComponent : Component
	{
		// Token: 0x0400099D RID: 2461
		[DataField("slice", false, 1, false, false, null)]
		[ViewVariables]
		public string Slice = string.Empty;

		// Token: 0x0400099E RID: 2462
		[DataField("sound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Items/Culinary/chop.ogg", null);

		// Token: 0x0400099F RID: 2463
		[DataField("count", false, 1, false, false, null)]
		[ViewVariables]
		public ushort TotalCount = 5;

		// Token: 0x040009A0 RID: 2464
		[ViewVariables]
		public ushort Count;
	}
}
