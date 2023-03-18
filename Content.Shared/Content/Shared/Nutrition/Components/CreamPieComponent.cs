using System;
using System.Runtime.CompilerServices;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components
{
	// Token: 0x020002AF RID: 687
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(SharedCreamPieSystem)
	})]
	[RegisterComponent]
	public sealed class CreamPieComponent : Component
	{
		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x00019F2D File Offset: 0x0001812D
		[DataField("paralyzeTime", false, 1, false, false, null)]
		public float ParalyzeTime { get; } = 1f;

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x00019F35 File Offset: 0x00018135
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound { get; } = new SoundCollectionSpecifier("desecration", null);

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00019F3D File Offset: 0x0001813D
		// (set) Token: 0x060007B8 RID: 1976 RVA: 0x00019F45 File Offset: 0x00018145
		[ViewVariables]
		public bool Splatted { get; set; }

		// Token: 0x040007CA RID: 1994
		public const string PayloadSlotName = "payloadSlot";
	}
}
