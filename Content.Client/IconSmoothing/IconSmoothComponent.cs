using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.IconSmoothing
{
	// Token: 0x020002C7 RID: 711
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class IconSmoothComponent : Component
	{
		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060011D9 RID: 4569 RVA: 0x00069B02 File Offset: 0x00067D02
		[Nullable(2)]
		[ViewVariables]
		[DataField("key", false, 1, false, false, null)]
		public string SmoothKey { [NullableContext(2)] get; }

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060011DA RID: 4570 RVA: 0x00069B0A File Offset: 0x00067D0A
		[ViewVariables]
		[DataField("base", false, 1, false, false, null)]
		public string StateBase { get; } = string.Empty;

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x060011DB RID: 4571 RVA: 0x00069B12 File Offset: 0x00067D12
		// (set) Token: 0x060011DC RID: 4572 RVA: 0x00069B1A File Offset: 0x00067D1A
		internal int UpdateGeneration { get; set; }

		// Token: 0x040008C2 RID: 2242
		[Nullable(0)]
		public ValueTuple<EntityUid?, Vector2i>? LastPosition;

		// Token: 0x040008C5 RID: 2245
		[ViewVariables]
		[DataField("mode", false, 1, false, false, null)]
		public IconSmoothingMode Mode;
	}
}
