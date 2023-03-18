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
	// Token: 0x020000B3 RID: 179
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MultipleToolComponent : Component
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000A95A File Offset: 0x00008B5A
		[DataField("entries", false, 1, true, false, null)]
		public MultipleToolComponent.ToolEntry[] Entries { get; } = Array.Empty<MultipleToolComponent.ToolEntry>();

		// Token: 0x04000273 RID: 627
		[ViewVariables]
		public uint CurrentEntry;

		// Token: 0x04000274 RID: 628
		[ViewVariables]
		public string CurrentQualityName = string.Empty;

		// Token: 0x04000275 RID: 629
		[ViewVariables]
		public bool UiUpdateNeeded;

		// Token: 0x04000276 RID: 630
		[DataField("statusShowBehavior", false, 1, false, false, null)]
		public bool StatusShowBehavior = true;

		// Token: 0x02000793 RID: 1939
		[NullableContext(2)]
		[Nullable(0)]
		[DataDefinition]
		public sealed class ToolEntry
		{
			// Token: 0x0400179F RID: 6047
			[Nullable(1)]
			[DataField("behavior", false, 1, true, false, null)]
			public PrototypeFlags<ToolQualityPrototype> Behavior = new PrototypeFlags<ToolQualityPrototype>();

			// Token: 0x040017A0 RID: 6048
			[DataField("useSound", false, 1, false, false, null)]
			public SoundSpecifier Sound;

			// Token: 0x040017A1 RID: 6049
			[DataField("changeSound", false, 1, false, false, null)]
			public SoundSpecifier ChangeSound;

			// Token: 0x040017A2 RID: 6050
			[DataField("sprite", false, 1, false, false, null)]
			public SpriteSpecifier Sprite;
		}
	}
}
