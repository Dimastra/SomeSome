using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Paper
{
	// Token: 0x020001EE RID: 494
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class PaperVisualsComponent : Component
	{
		// Token: 0x04000662 RID: 1634
		[DataField("backgroundImagePath", false, 1, false, false, null)]
		public string BackgroundImagePath;

		// Token: 0x04000663 RID: 1635
		[DataField("backgroundPatchMargin", false, 1, false, false, null)]
		public Box2 BackgroundPatchMargin;

		// Token: 0x04000664 RID: 1636
		[DataField("backgroundModulate", false, 1, false, false, null)]
		public Color BackgroundModulate = Color.White;

		// Token: 0x04000665 RID: 1637
		[DataField("backgroundImageTile", false, 1, false, false, null)]
		public bool BackgroundImageTile;

		// Token: 0x04000666 RID: 1638
		[DataField("backgroundScale", false, 1, false, false, null)]
		public Vector2 BackgroundScale = Vector2.One;

		// Token: 0x04000667 RID: 1639
		[DataField("headerImagePath", false, 1, false, false, null)]
		public string HeaderImagePath;

		// Token: 0x04000668 RID: 1640
		[DataField("headerImageModulate", false, 1, false, false, null)]
		public Color HeaderImageModulate = Color.White;

		// Token: 0x04000669 RID: 1641
		[DataField("headerMargin", false, 1, false, false, null)]
		public Box2 HeaderMargin;

		// Token: 0x0400066A RID: 1642
		[DataField("contentImagePath", false, 1, false, false, null)]
		public string ContentImagePath;

		// Token: 0x0400066B RID: 1643
		[DataField("contentImageModulate", false, 1, false, false, null)]
		public Color ContentImageModulate = Color.White;

		// Token: 0x0400066C RID: 1644
		[DataField("contentMargin", false, 1, false, false, null)]
		public Box2 ContentMargin;

		// Token: 0x0400066D RID: 1645
		[DataField("contentImageNumLines", false, 1, false, false, null)]
		public int ContentImageNumLines = 1;

		// Token: 0x0400066E RID: 1646
		[DataField("fontAccentColor", false, 1, false, false, null)]
		public Color FontAccentColor = new Color(37, 37, 42, byte.MaxValue);

		// Token: 0x0400066F RID: 1647
		[DataField("maxWritableArea", false, 1, false, false, null)]
		public Vector2? MaxWritableArea;
	}
}
