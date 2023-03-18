using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.TextScreen;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Client.TextScreen
{
	// Token: 0x020000F8 RID: 248
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TextScreenVisualsComponent : Component
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060006FB RID: 1787 RVA: 0x00024CF9 File Offset: 0x00022EF9
		// (set) Token: 0x060006FC RID: 1788 RVA: 0x00024D01 File Offset: 0x00022F01
		[DataField("color", false, 1, false, false, null)]
		public Color Color { get; set; } = Color.FloralWhite;

		// Token: 0x0400032F RID: 815
		public const float PixelSize = 0.03125f;

		// Token: 0x04000331 RID: 817
		[DataField("activated", false, 1, false, false, null)]
		public bool Activated;

		// Token: 0x04000332 RID: 818
		[DataField("currentMode", false, 1, false, false, null)]
		public TextScreenMode CurrentMode;

		// Token: 0x04000333 RID: 819
		[DataField("targetTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan TargetTime = TimeSpan.Zero;

		// Token: 0x04000334 RID: 820
		[DataField("textOffset", false, 1, false, false, null)]
		[ViewVariables]
		public Vector2 TextOffset = new Vector2(0f, 0.25f);

		// Token: 0x04000335 RID: 821
		[DataField("textLength", false, 1, false, false, null)]
		public int TextLength = 5;

		// Token: 0x04000336 RID: 822
		[DataField("text", false, 1, false, false, null)]
		[ViewVariables]
		public string Text = "";

		// Token: 0x04000337 RID: 823
		public string TextToDraw = "";

		// Token: 0x04000338 RID: 824
		[Nullable(new byte[]
		{
			1,
			1,
			2
		})]
		[DataField("layerStatesToDraw", false, 1, false, false, null)]
		public Dictionary<string, string> LayerStatesToDraw = new Dictionary<string, string>();
	}
}
