using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000BC RID: 188
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ToolComponent : Component
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0000A9EC File Offset: 0x00008BEC
		// (set) Token: 0x0600020F RID: 527 RVA: 0x0000A9F4 File Offset: 0x00008BF4
		[DataField("qualities", false, 1, false, false, null)]
		public PrototypeFlags<ToolQualityPrototype> Qualities { get; set; } = new PrototypeFlags<ToolQualityPrototype>();

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000A9FD File Offset: 0x00008BFD
		// (set) Token: 0x06000211 RID: 529 RVA: 0x0000AA05 File Offset: 0x00008C05
		[ViewVariables]
		[DataField("speed", false, 1, false, false, null)]
		public float SpeedModifier { get; set; } = 1f;

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000AA0E File Offset: 0x00008C0E
		// (set) Token: 0x06000213 RID: 531 RVA: 0x0000AA16 File Offset: 0x00008C16
		[Nullable(2)]
		[DataField("useSound", false, 1, false, false, null)]
		public SoundSpecifier UseSound { [NullableContext(2)] get; [NullableContext(2)] set; }
	}
}
